using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Bimber
{
    public partial class SettingsForm : Form
    {
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
            var fileVersion = Assembly.GetEntryAssembly()
                        .GetName().Version.ToString();
            // Localize form
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
            logLinkLabel.Click += (s, e) => linkLabel1_LinkClicked(s, new LinkLabelLinkClickedEventArgs(null));
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            {
                if (string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
                {
                    MessageBox.Show(Resources.emptyapi, Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    apiKeyTextBox.Focus();
                    return;
                }

                settings.ApiKey = apiKeyTextBox.Text.Trim(); // Save trimmed value
                DialogResult = DialogResult.OK;
                Close();
            }
            settings.StartWithWindows = startWithWindowsCheckBox.Checked;
            settings.Language = languageComboBox.SelectedIndex == 1 ? "pl" : "en";
            settings.ImageUploaderType = comboBox1.SelectedIndex == 1 ? "ImageUploader2" : "ImageUploader";
            settings.SaveLocally = saveLocallyCheckBox.Checked;
            settings.LocalSavePath = folderPathTextBox.Text;
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
        private string newHotkey;
        private void StartHotkeyCapture()
        {
            isCapturingHotkey = true;
            newHotkey = settings.Hotkey; // Initialize with current hotkey
            hotkeyButton.Text = Resources.HotkeyPrompt;
            pressedKeys.Clear();

            // Subscribe to key events
            this.KeyPreview = true;
            this.KeyDown += HotkeyCapture_KeyDown!;
            this.KeyUp += HotkeyCapture_KeyUp!;
            this.LostFocus += HotkeyCapture_LostFocus!;

            hotkeyButton.Focus();
        }

        private void StopHotkeyCapture(bool saveHotkey = true)
        {
            if (!isCapturingHotkey) return;

            isCapturingHotkey = false;
            this.KeyPreview = false;

            // Unsubscribe from key events
            this.KeyDown -= HotkeyCapture_KeyDown!;
            this.KeyUp -= HotkeyCapture_KeyUp!;
            this.LostFocus -= HotkeyCapture_LostFocus!;

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
            // Don't process if we're not capturing
            if (!isCapturingHotkey) return;

            // Ignore modifier keys alone
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            // Suppress the key from further processing
            e.SuppressKeyPress = true;

            // Add the key if not already pressed
            if (!pressedKeys.Contains(e.KeyCode))
            {
                pressedKeys.Add(e.KeyCode);
                UpdateHotkeyDisplay();
            }
        }

        private void HotkeyCapture_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isCapturingHotkey) return;
            e.SuppressKeyPress = true;

            // Remove the released key
            pressedKeys.Remove(e.KeyCode);

            // For modifier keys, check the actual modifier state
            if (e.KeyCode == Keys.ControlKey && !Control.ModifierKeys.HasFlag(Keys.Control))
                pressedKeys.Remove(Keys.Control);
            if (e.KeyCode == Keys.ShiftKey && !Control.ModifierKeys.HasFlag(Keys.Shift))
                pressedKeys.Remove(Keys.Shift);
            if (e.KeyCode == Keys.Menu && !Control.ModifierKeys.HasFlag(Keys.Alt))
                pressedKeys.Remove(Keys.Alt);

            UpdateHotkeyDisplay();

            if (pressedKeys.Count == 0 && Control.ModifierKeys == Keys.None)
            {
                bool shouldSave = !string.IsNullOrEmpty(newHotkey) &&
                                 newHotkey != Resources.HotkeyPrompt &&
                                 newHotkey != (settings.Hotkey ?? Resources.SetHotkey);
                StopHotkeyCapture(shouldSave);
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

            // Add modifiers in consistent order using localized strings
            if (modifiers.HasFlag(Keys.Control))
                keyStrings.Add(Resources.ModifierCtrl);
            if (modifiers.HasFlag(Keys.Alt))
                keyStrings.Add(Resources.ModifierAlt);
            if (modifiers.HasFlag(Keys.Shift))
                keyStrings.Add(Resources.ModifierShift);

            // Add other keys (excluding modifiers)
            foreach (var key in keys)
            {
                if (key != Keys.ControlKey && key != Keys.Menu && key != Keys.ShiftKey &&
                    key != Keys.LWin && key != Keys.RWin && key != Keys.Control &&
                    key != Keys.Alt && key != Keys.Shift)
                {
                    keyStrings.Add(key.ToString());
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
                    // Set default path to Pictures/Bimber folder
                    string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    string defaultFolder = Path.Combine(picturesFolder, "Bimber");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(defaultFolder))
                    {
                        Directory.CreateDirectory(defaultFolder);
                    }

                    folderDialog.SelectedPath = defaultFolder;
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
                        // Get the path to the executable
                        string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                           Process.GetCurrentProcess().ProcessName + ".exe");

                        // Add to startup
                        key.SetValue(appName, executablePath);
                    }
                    else
                    {
                        // Remove from startup
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., show a message to the user)
                MessageBox.Show($"Failed to modify startup settings: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            try
            {
                if (File.Exists(logFilePath))
                {
                    // Read all lines from the log file
                    string[] logLines = File.ReadAllLines(logFilePath);

                    // Reverse the array to show newest entries first
                    Array.Reverse(logLines);

                    // Join the lines back together with newlines
                    string reversedLogContents = string.Join(Environment.NewLine, logLines);

                    // Display in a simple form
                    var logViewer = new Form
                    {
                        Text = Resources.UploadLog,
                        Width = 600,
                        Height = 400,
                        StartPosition = FormStartPosition.CenterParent
                    };

                    var textBox = new TextBox
                    {
                        Multiline = true,
                        Dock = DockStyle.Fill,
                        ScrollBars = ScrollBars.Both,
                        ReadOnly = true,
                        Text = reversedLogContents,
                        Font = new Font("Consolas", 10)
                    };

                    logViewer.Controls.Add(textBox);
                    logViewer.ShowDialog();
                }
                else
                {
                    MessageBox.Show(Resources.Nolog, Resources.UploadLog, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void apiKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            bool isValid = !string.IsNullOrWhiteSpace(apiKeyTextBox.Text);
            SaveBtn.Enabled = isValid;
            errorProvider1.SetError(apiKeyTextBox, isValid ? "" : Resources.emptyapi);
        }
    }
}