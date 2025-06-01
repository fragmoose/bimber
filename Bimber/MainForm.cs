using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Bimber
{
    public partial class MainForm : Form
    {
        private readonly global::AppSettings settings;

        // Low-level keyboard hook for global hotkeys
        private IntPtr _hookID = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        // Track currently pressed keys
        private readonly HashSet<Keys> currentlyPressedKeys = new HashSet<Keys>();
        private Keys[] currentHotkeyParts = Array.Empty<Keys>();
        private bool hotkeyRegistered = false;

        // Windows API imports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public MainForm()
        {
            InitializeComponent();
            settings = global::AppSettings.Load();
            InitializeLanguage();
            InitializeTrayMenu();
            RegisterHotkey();
        }

        private void InitializeLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            trayIcon.Text = Resources.AppTitle;
            settingsToolStripMenuItem.Text = Resources.Settings;
            exitToolStripMenuItem.Text = Resources.Exit;
        }

        private void InitializeTrayMenu()
        {
            trayIcon.ContextMenuStrip = trayMenu;
            settingsToolStripMenuItem.Click += (s, e) => ShowSettings();
            exitToolStripMenuItem.Click += (s, e) => ExitApplication();
        }

        private void ShowSettings()
        {
            var settingsForm = new SettingsForm(settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settings.Save();
                InitializeLanguage();
                RegisterHotkey(); // Re-register hotkey if changed
            }
        }

        private void ExitApplication()
        {
            // Unhook the keyboard hook
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
            }

            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide();
        }

        private void RegisterHotkey()
        {
            // Remove existing hook if any
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }

            // Clear current state
            currentlyPressedKeys.Clear();
            hotkeyRegistered = false;

            if (!string.IsNullOrEmpty(settings.Hotkey))
            {
                try
                {
                    // Parse the hotkey string from settings
                    currentHotkeyParts = ParseHotkeyString(settings.Hotkey);
                    if (currentHotkeyParts.Length > 0)
                    {
                        _hookID = SetHook(HookCallback);
                        hotkeyRegistered = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing hotkey: {ex.Message}");
                    currentHotkeyParts = Array.Empty<Keys>();
                }
            }
        }

        private Keys[] ParseHotkeyString(string hotkeyString)
        {
            if (string.IsNullOrEmpty(hotkeyString))
                return Array.Empty<Keys>();

            var parts = hotkeyString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(p => p.Trim())
                                   .ToArray();

            var keys = new List<Keys>();

            foreach (var part in parts)
            {
                // Handle localized modifier keys
                if (part.Equals(Resources.ModifierCtrl, StringComparison.OrdinalIgnoreCase) ||
                    part.Equals("Control", StringComparison.OrdinalIgnoreCase) ||
                    part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase))
                {
                    keys.Add(Keys.Control);
                }
                else if (part.Equals(Resources.ModifierAlt, StringComparison.OrdinalIgnoreCase) ||
                         part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                {
                    keys.Add(Keys.Alt);
                }
                else if (part.Equals(Resources.ModifierShift, StringComparison.OrdinalIgnoreCase) ||
                         part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                {
                    keys.Add(Keys.Shift);
                }
                else
                {
                    // Parse regular keys
                    if (Enum.TryParse(part, true, out Keys key))
                    {
                        // Handle special cases
                        if (part.Equals("Escape", StringComparison.OrdinalIgnoreCase))
                            key = Keys.Escape;
                        else if (part.Equals("Return", StringComparison.OrdinalIgnoreCase))
                            key = Keys.Return;
                        else if (part.Equals("Enter", StringComparison.OrdinalIgnoreCase))
                            key = Keys.Enter;
                        else if (part.Equals("Space", StringComparison.OrdinalIgnoreCase))
                            key = Keys.Space;

                        keys.Add(key);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid key: {part}");
                    }
                }
            }

            return keys.ToArray();
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                // Update currently pressed keys
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    currentlyPressedKeys.Add(key);
                }
                else
                {
                    currentlyPressedKeys.Remove(key);
                }

                // Get current modifier keys
                var modifiers = Control.ModifierKeys;

                // Check if the current combination matches our hotkey
                if (wParam == (IntPtr)WM_KEYDOWN && hotkeyRegistered)
                {
                    bool allRequiredKeysPressed = currentHotkeyParts.All(k =>
                        currentlyPressedKeys.Contains(k) ||
                        (k == Keys.Control && modifiers.HasFlag(Keys.Control)) ||
                        (k == Keys.Alt && modifiers.HasFlag(Keys.Alt)) ||
                        (k == Keys.Shift && modifiers.HasFlag(Keys.Shift)));

                    bool exactMatch = currentHotkeyParts.Length ==
                        currentlyPressedKeys.Count(k =>
                            !IsModifierKey(k) ||
                            currentHotkeyParts.Contains(k));

                    if (allRequiredKeysPressed && exactMatch)

                    {
                        this.Invoke((MethodInvoker)delegate {
                            // Create an instance of the snipping tool
                            var snipper = new ScreenSnipTool();
                            // Start the snipping process
                            snipper.StartSelection();
                            // Create ImageUploader instance with your settings
                            var uploader = new ImageUploader(settings);
                            string message = "default text";
                            // Subscribe to the completion event
                            snipper.SnipCompleted += async (snippedImage) =>
                            {
                                try
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        // Save image to memory stream
                                        snippedImage.Save(memoryStream, ImageFormat.Png);
                                        memoryStream.Position = 0; // Reset stream position

                                        // Upload the image
                                        string imageUrl = await uploader.UploadImageAsync(memoryStream, $"screenshot_{DateTime.Now:yyyyMMddHHmmss}.png");

                                        // Do something with the URL (copy to clipboard, show in UI, etc.)
                                        Clipboard.SetText(imageUrl);
                                        // Show hint (optional)
                                        message = "Zrzut ekranu przesłany pomyślnie!\nURL:" + imageUrl;
                                        Clipboard.SetText(imageUrl);
                                        HintForm hint = new HintForm(message);
                                        hint.Show();

                                        // Auto-close hint after 3 seconds
                                        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                                        timer.Interval = 3000;
                                        timer.Tick += (s, e) => {
                                            hint.Close();
                                            timer.Stop();
                                            timer.Dispose();
                                        };
                                        timer.Start();
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    message = "Błąd podczas przesyłania zrzutu:" + ex.Message;
                                    HintForm hint = new HintForm(message);
                                    hint.Show();

                                    // Auto-close hint after 3 seconds
                                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                                    timer.Interval = 3000;
                                    timer.Tick += (s, e) => {
                                        hint.Close();
                                        timer.Stop();
                                        timer.Dispose();
                                    };
                                    timer.Start();
                                }
                                finally
                                {
                                    // Always dispose the image
                                    snippedImage.Dispose();
                                }
                            };

                            

                            
                        });

                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey || key == Keys.Control ||
                   key == Keys.ShiftKey || key == Keys.Shift ||
                   key == Keys.Menu || key == Keys.Alt ||
                   key == Keys.LWin || key == Keys.RWin;
        }

        private void trayMenu_Opening(object sender, CancelEventArgs e)
        {
            settingsToolStripMenuItem.Text = Resources.Settings;
            exitToolStripMenuItem.Text = Resources.Exit;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}