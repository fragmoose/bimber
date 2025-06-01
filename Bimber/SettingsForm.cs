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

            // Setup language combo
            languageComboBox.Items.AddRange(new object[] { "English", "Polski" });
            languageComboBox.SelectedIndex = settings.Language == "pl" ? 1 : 0;

            // Localize form
            Text = Resources.Settings;
            apiKeyLabel.Text = Resources.ApiKeyLabel;
            startWithWindowsCheckBox.Text = Resources.StartWithWindows;
            hotkeyLabel.Text = Resources.HotkeyLabel;
            SaveBtn.Text = Resources.Save;
            CancelBtn.Text = Resources.Cancel;
            languageLabel.Text = Resources.labelLanguage;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            settings.ApiKey = apiKeyTextBox.Text;
            settings.StartWithWindows = startWithWindowsCheckBox.Checked;
            settings.Language = languageComboBox.SelectedIndex == 1 ? "pl" : "en";

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
    }
}