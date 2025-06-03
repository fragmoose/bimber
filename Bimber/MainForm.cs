using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

namespace Bimber
{
    public interface IImageUploader
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
    }

    public partial class MainForm : Form
    {
        private readonly UpdateChecker _updateChecker;
        private readonly UpdateInstaller _updateInstaller;
        private const string GitHubRepoUrl = "https://github.com/fragmoose/bimber";
        private readonly global::AppSettings settings;

        // Hotkey API
        private const int HOTKEY_ID = 0x0001;
        private const uint MOD_NONE = 0x0000;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;
        private const uint WM_HOTKEY = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainForm()
        {
            _updateChecker = new UpdateChecker(GitHubRepoUrl);
            _updateInstaller = new UpdateInstaller(GitHubRepoUrl);
            InitializeComponent();
            settings = global::AppSettings.Load();
            InitializeLanguage();
            InitializeTrayMenu();
            RegisterHotkey();
            CheckForUpdatesSilently();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    CaptureAndProcessScreenshot();
                }
                return;
            }
            base.WndProc(ref m);
        }

        private void InitializeLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            trayIcon.Text = Resources.AppTitle;
            updateStripMenuItem1.Text = Resources.updateCheck;
            logsStripMenuItem1.Text = Resources.Logs;
            settingsToolStripMenuItem.Text = Resources.Settings;
            exitToolStripMenuItem.Text = Resources.Exit;
        }
        
        private void InitializeTrayMenu()
        {
            var logViewer = new LogViewerManager();
            trayIcon.ContextMenuStrip = trayMenu;
            updateStripMenuItem1.Click += (s, e) => checkUpdate();
            logsStripMenuItem1.Click += (s, e) => logViewer.ShowLogs(); 
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
                RegisterHotkey();
            }
        }

        private void ExitApplication()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
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
            // Unregister existing hotkey first
            UnregisterHotKey(this.Handle, HOTKEY_ID);

            if (!string.IsNullOrEmpty(settings.Hotkey))
            {
                try
                {
                    var parts = settings.Hotkey.Split('+').Select(p => p.Trim()).ToArray();
                    uint modifiers = MOD_NONE;
                    uint keyCode = 0;

                    foreach (var part in parts)
                    {
                        if (part.Equals(Resources.ModifierCtrl, StringComparison.OrdinalIgnoreCase) ||
                            part.Equals("Control", StringComparison.OrdinalIgnoreCase) ||
                            part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase))
                        {
                            modifiers |= MOD_CONTROL;
                        }
                        else if (part.Equals(Resources.ModifierAlt, StringComparison.OrdinalIgnoreCase) ||
                                 part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                        {
                            modifiers |= MOD_ALT;
                        }
                        else if (part.Equals(Resources.ModifierShift, StringComparison.OrdinalIgnoreCase) ||
                                 part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                        {
                            modifiers |= MOD_SHIFT;
                        }
                        else
                        {
                            if (Enum.TryParse(part, true, out Keys key))
                            {
                                keyCode = (uint)key;
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid key: {part}");
                            }
                        }
                    }

                    if (keyCode != 0 && !RegisterHotKey(this.Handle, HOTKEY_ID, modifiers, keyCode))
                    {
                        MessageBox.Show(Resources.HotkeyRegistrationFailed, Resources.error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Resources.HotkeyRegistrationFailed}: {ex.Message}", Resources.error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CaptureAndProcessScreenshot()
        {
            var snipper = new ScreenSnipTool();
            snipper.StartSelection();

            IImageUploader uploader = settings.ImageUploaderType == "ImageUploader2"
                ? new ImageUploader2(settings)
                : new ImageUploader(settings);

            string message = "default text";

            snipper.SnipCompleted += async (snippedImage) =>
            {
                string localPath = string.Empty;
                string imageUrl = string.Empty;

                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        snippedImage.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;

                        imageUrl = await uploader.UploadImageAsync(memoryStream, $"bimber_{DateTime.Now:yyyyMMddHHmmss}.png");

                        Clipboard.SetText(imageUrl);
                        message = Resources.Message;
                        var hintDisplayer = new HintDisplayer();
                        hintDisplayer.ShowHint(message);

                        if (settings.SaveLocally && !string.IsNullOrEmpty(settings.LocalSavePath))
                        {
                            try
                            {
                                string fileName = $"bimber_{DateTime.Now:yyyyMMddHHmmss}.png";
                                localPath = Path.Combine(settings.LocalSavePath, fileName);
                                snippedImage.Save(localPath, ImageFormat.Png);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error saving locally: {ex.Message}");
                            }
                        }

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            LogToFile(localPath, imageUrl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "Error saving snip:" + ex.Message;
                    var hintDisplayer = new HintDisplayer();
                    hintDisplayer.ShowHint(message);
                }
                finally
                {
                    snippedImage.Dispose();
                }
            };
        }

        

        private void LogToFile(string localPath, string imageUrl)
        {
            try
            {
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{(string.IsNullOrEmpty(localPath) ? "N/A" : localPath)};{imageUrl}{Environment.NewLine}";
                File.AppendAllText(logFilePath, logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        private async void CheckForUpdatesSilently()
        {
            try
            {
                var updateAvailable = await _updateChecker.CheckForUpdatesAsync();
                if (updateAvailable)
                {
                    if (MessageBox.Show(Resources.UpdateAvailable,
                        "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        await InstallUpdateAsync();
                    }
                }
            }
            catch
            {
                // Silent fail
            }
        }

        private async void checkUpdate()
        {
            try
            {
                var updateAvailable = await _updateChecker.CheckForUpdatesAsync();
                if (updateAvailable)
                {
                    if (MessageBox.Show(Resources.UpdateAvailable,
                        "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        await InstallUpdateAsync();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.noUpdate, Resources.noUpdatetitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InstallUpdateAsync()
        {
            try
            {
                var progressForm = new Form
                {
                    Text = Resources.updating,
                    Width = 300,
                    Height = 100,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent
                };

                var progressBar = new ProgressBar { Dock = DockStyle.Fill };
                var statusLabel = new Label { Text = Resources.updating, Dock = DockStyle.Bottom };

                progressForm.Controls.Add(progressBar);
                progressForm.Controls.Add(statusLabel);
                progressForm.Show(this);

                await _updateInstaller.DownloadAndInstallUpdateAsync();
                progressForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trayMenu_Opening(object sender, CancelEventArgs e)
        {
            settingsToolStripMenuItem.Text = Resources.Settings;
            exitToolStripMenuItem.Text = Resources.Exit;
        }
    }
}