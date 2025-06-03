using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Bimber
{
    public partial class SettingsForm : Form
    {
        private bool _suppressContextMenu = false;
        private readonly global::AppSettings settings;
        public SettingsForm(global::AppSettings currentSettings)
        {
            InitializeComponent();
            settings = currentSettings;
            InitializeForm();
        }
        
        private void InitializeForm()
        {
            apiKeyTextBox.Text = settings.ApiKey;
            startWithWindowsCheckBox.Checked = settings.StartWithWindows;
            hotkeyButton.Text = settings.Hotkey ?? Resources.SetHotkey;
            saveLocallyCheckBox.Checked = settings.SaveLocally;
            folderPathTextBox.Text = settings.LocalSavePath;
            folderPathTextBox.Enabled = settings.SaveLocally;

            // Setup language combo
            languageComboBox.Items.AddRange(new object[] { "English", "Polski" });
            languageComboBox.SelectedIndex = settings.Language == "pl" ? 1 : 0;

            // Setup image uploader type combo
            comboBox1.Items.AddRange(new object[] { "pixvid.org", "fivemanage.com" });
            comboBox1.SelectedIndex = settings.ImageUploaderType == "ImageUploader2" ? 1 : 0;

            var fileVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

            // Localize form
            var logViewer = new LogViewerManager();
            Text = Resources.Settings + " Bimber v" + fileVersion;
            apiKeyLabel.Text = Resources.ApiKeyLabel;
            startWithWindowsCheckBox.Text = Resources.StartWithWindows;
            hotkeyLabel.Text = Resources.HotkeyLabel;
            SaveBtn.Text = Resources.Save;
            CancelBtn.Text = Resources.Cancel;
            languageLabel.Text = Resources.labelLanguage;
            saveLocallyCheckBox.Text = Resources.SaveLocally;
            pathTextLabel.Text = Resources.pathTextLabel;
            browseFolderButton.Text = Resources.Browse;
            logLinkLabel.Text = Resources.LogLinkLabel;
            linkLabel1.Text = Resources.About;
            logLinkLabel.Click += (s, e) => logViewer.ShowLogs();
        }        
        
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
            {
                MessageBox.Show(Resources.emptyapi, Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                apiKeyTextBox.Focus();
                return;
            }

            settings.ApiKey = apiKeyTextBox.Text.Trim();
            settings.StartWithWindows = startWithWindowsCheckBox.Checked;
            settings.Language = languageComboBox.SelectedIndex == 1 ? "pl" : "en";
            settings.ImageUploaderType = comboBox1.SelectedIndex == 1 ? "ImageUploader2" : "ImageUploader";
            settings.SaveLocally = saveLocallyCheckBox.Checked;
            settings.LocalSavePath = folderPathTextBox.Text;
            settings.Hotkey = hotkeyButton.Text != Resources.SetHotkey ? hotkeyButton.Text : null;

            SetStartupWithWindows(settings.StartWithWindows);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private bool isCapturingHotkey = false;
        private string newHotkey;

        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            if (!isCapturingHotkey)
            {
                StartHotkeyCapture();
            }
            else
            {
                StopHotkeyCapture();
            }
        }

        private void StartHotkeyCapture()
        {
            isCapturingHotkey = true;
            newHotkey = settings.Hotkey;
            hotkeyButton.Text = Resources.HotkeyPrompt;
            pressedKeys.Clear();

            this.KeyPreview = true;
            this.KeyDown += HotkeyCapture_KeyDown;
            this.KeyUp += HotkeyCapture_KeyUp;
            this.LostFocus += HotkeyCapture_LostFocus;

            hotkeyButton.Focus();
        }

        private void StopHotkeyCapture(bool saveHotkey = true)
        {
            if (!isCapturingHotkey) return;

            isCapturingHotkey = false;
            this.KeyPreview = false;

            this.KeyDown -= HotkeyCapture_KeyDown;
            this.KeyUp -= HotkeyCapture_KeyUp;
            this.LostFocus -= HotkeyCapture_LostFocus;

            if (saveHotkey)
            {
                if (!string.IsNullOrEmpty(newHotkey) && newHotkey != Resources.HotkeyPrompt)
                {
                    settings.Hotkey = newHotkey;
                    hotkeyButton.Text = settings.Hotkey;
                }
            }
            else
            {
                hotkeyButton.Text = settings.Hotkey ?? Resources.SetHotkey;
            }
        }

        private void HotkeyCapture_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isCapturingHotkey) return;

            e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Escape)
            {
                StopHotkeyCapture(false);
                return;
            }

            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            if (!pressedKeys.Contains(e.KeyCode))
            {
                pressedKeys.Add(e.KeyCode);
                UpdateHotkeyDisplay();
            }
        }

        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey || key == Keys.Menu || key == Keys.ShiftKey ||
                   key == Keys.LWin || key == Keys.RWin ||
                   key == Keys.Control || key == Keys.Alt || key == Keys.Shift;
        }

        private string GetKeyDisplayName(Keys key)
        {
            switch (key)
            {
                case Keys.Oemcomma: return ",";
                case Keys.OemPeriod: return ".";
                case Keys.OemQuestion: return "/";
                default: return key.ToString();
            }
        }

        private void HotkeyCapture_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isCapturingHotkey) return;
            e.SuppressKeyPress = true;

            pressedKeys.Remove(e.KeyCode);

            if (pressedKeys.Count == 0 && Control.ModifierKeys == Keys.None)
            {
                bool shouldSave = !string.IsNullOrEmpty(newHotkey) &&
                                 newHotkey != Resources.HotkeyPrompt &&
                                 newHotkey != (settings.Hotkey ?? Resources.SetHotkey);
                StopHotkeyCapture(shouldSave);
            }
            else
            {
                UpdateHotkeyDisplay();
            }
        }

        private void HotkeyCapture_LostFocus(object sender, EventArgs e)
        {
            if (isCapturingHotkey)
            {
                StopHotkeyCapture(saveHotkey: false);
            }
        }

        private void UpdateHotkeyDisplay()
        {
            if (pressedKeys.Count > 0)
            {
                newHotkey = ConvertKeysToString(pressedKeys);
                hotkeyButton.Text = newHotkey;
            }
            else
            {
                hotkeyButton.Text = Resources.HotkeyPrompt;
            }
        }

        private string ConvertKeysToString(IEnumerable<Keys> keys)
        {
            var keyStrings = new List<string>();
            var modifiers = Control.ModifierKeys;

            if (modifiers.HasFlag(Keys.Control))
                keyStrings.Add(Resources.ModifierCtrl);
            if (modifiers.HasFlag(Keys.Alt))
                keyStrings.Add(Resources.ModifierAlt);
            if (modifiers.HasFlag(Keys.Shift))
                keyStrings.Add(Resources.ModifierShift);

            foreach (var key in keys)
            {
                if (!IsModifierKey(key))
                {
                    keyStrings.Add(GetKeyDisplayName(key));
                }
            }

            return string.Join("+", keyStrings);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.ImageUploaderType = comboBox1.SelectedIndex == 1 ? "ImageUploader2" : "ImageUploader";
        }

        private void saveLocallyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            folderPathTextBox.Enabled = saveLocallyCheckBox.Checked;
            browseFolderButton.Enabled = saveLocallyCheckBox.Checked;
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(folderPathTextBox.Text))
                {
                    folderDialog.SelectedPath = folderPathTextBox.Text;
                }
                else
                {
                    string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    string defaultFolder = Path.Combine(picturesFolder, "Bimber");

                    if (!Directory.Exists(defaultFolder))
                    {
                        Directory.CreateDirectory(defaultFolder);
                    }

                    folderDialog.SelectedPath = defaultFolder;
                    folderPathTextBox.Text = folderDialog.SelectedPath;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    folderPathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void startWithWindowsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetStartupWithWindows(startWithWindowsCheckBox.Checked);
        }

        private void SetStartupWithWindows(bool enable)
        {
            const string appName = "Bimber";
            const string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (key == null) return;

                    if (enable)
                    {
                        string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                       Process.GetCurrentProcess().ProcessName + ".exe");
                        key.SetValue(appName, executablePath);
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to modify startup settings: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void apiKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            bool isValid = !string.IsNullOrWhiteSpace(apiKeyTextBox.Text);
            SaveBtn.Enabled = isValid;
            errorProvider1.SetError(apiKeyTextBox, isValid ? "" : Resources.emptyapi);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var aboutForm = new AboutBox1();
            aboutForm.ShowDialog();
        }
     

        
    }
}