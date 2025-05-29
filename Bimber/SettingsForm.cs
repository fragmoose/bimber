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
            hotkeyButton.Text = settings.Hotkey;

            // Setup language combo
            languageComboBox.Items.AddRange(new object[] { "English", "Polski" });
            languageComboBox.SelectedIndex = settings.Language == "pl" ? 1 : 0;

            // Localize form
            Text = Resources.Settings;
            apiKeyLabel.Text = Resources.ApiKeyLabel;
            startWithWindowsCheckBox.Text = Resources.StartWithWindows;
            hotkeyLabel.Text = Resources.HotkeyLabel;
            saveBtn.Text = Resources.Save;
            cancelBtn.Text = Resources.Cancel;
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            settings.ApiKey = apiKeyTextBox.Text;
            settings.StartWithWindows = startWithWindowsCheckBox.Checked;
            settings.Language = languageComboBox.SelectedIndex == 1 ? "pl" : "en";

            DialogResult = DialogResult.OK;
            Close();
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            // Implement hotkey capture dialog
            // For now just simulate setting Ctrl+Alt+H
            settings.Hotkey = "Ctrl+Alt+H";
            hotkeyButton.Text = settings.Hotkey;
        }
    }
}
