using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Bimber
{
    public partial class MainForm : Form
    {
        private readonly global::AppSettings settings;
        public MainForm()
        {
            InitializeComponent();
            settings = global::AppSettings.Load();
            InitializeLanguage();
            InitializeTrayMenu();
            // Register hotkey if set
            if (!string.IsNullOrEmpty(settings.Hotkey))
            {
                // Implement hotkey registration here
            }
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

                // Re-register hotkey if changed
            }
        }
        private void ExitApplication()
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide();
        }
        private HotkeyManager hotkeyManager;

        private void RegisterHotkey()
        {
            if (hotkeyManager != null)
            {
                hotkeyManager.Dispose();
            }

            if (!string.IsNullOrEmpty(settings.Hotkey))
            {
                // Parse the hotkey string and register it
                // This is simplified - you'll need proper parsing
                hotkeyManager = new HotkeyManager(Handle, 1);
                hotkeyManager.RegisterHotkey(Keys.H,
                    HotkeyManager.KeyModifiers.Control | HotkeyManager.KeyModifiers.Alt);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                // Handle hotkey press
                // Implement your functionality here
            }

            base.WndProc(ref m);
        }
        private void trayMenu_Opening(object sender, CancelEventArgs e)
        {
            // This method is called every time the context menu is about to open
            // You can update menu items here if needed

            // Example: Update the text of menu items when language changes
            settingsToolStripMenuItem.Text = Resources.Settings;
            exitToolStripMenuItem.Text = Resources.Exit;

            // You could also enable/disable items based on application state
            // settingsToolStripMenuItem.Enabled = SomeCondition;
        }
    }
}
