namespace Bimber
{
    partial class SettingsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Initialize form controls with current settings
            apiKeyTextBox.Text = settings.ApiKey;
            startWithWindowsCheckBox.Checked = settings.StartWithWindows;

            // Set language selection
            languageComboBox.SelectedIndex = settings.Language == "pl" ? 1 : 0;

            // Initialize hotkey display
            hotkeyButton.Text = string.IsNullOrEmpty(settings.Hotkey) ?
                "None" : settings.Hotkey;
        }
        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            apiKeyTextBox = new TextBox();
            apiKeyLabel = new Label();
            startWithWindowsCheckBox = new CheckBox();
            hotkeyButton = new Button();
            languageComboBox = new ComboBox();
            languageLabel = new Label();
            SaveBtn = new Button();
            CancelBtn = new Button();
            hotkeyLabel = new Label();
            SuspendLayout();
            // 
            // apiKeyTextBox
            // 
            apiKeyTextBox.Location = new Point(77, 6);
            apiKeyTextBox.Name = "apiKeyTextBox";
            apiKeyTextBox.Size = new Size(128, 23);
            apiKeyTextBox.TabIndex = 0;
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new Point(24, 9);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new Size(47, 15);
            apiKeyLabel.TabIndex = 1;
            apiKeyLabel.Text = "API Key";
            // 
            // startWithWindowsCheckBox
            // 
            startWithWindowsCheckBox.AutoSize = true;
            startWithWindowsCheckBox.Location = new Point(65, 93);
            startWithWindowsCheckBox.Name = "startWithWindowsCheckBox";
            startWithWindowsCheckBox.Size = new Size(128, 19);
            startWithWindowsCheckBox.TabIndex = 2;
            startWithWindowsCheckBox.Text = "Start with Windows";
            startWithWindowsCheckBox.UseVisualStyleBackColor = true;
            // 
            // hotkeyButton
            // 
            hotkeyButton.Location = new Point(77, 35);
            hotkeyButton.Name = "hotkeyButton";
            hotkeyButton.Size = new Size(128, 23);
            hotkeyButton.TabIndex = 3;
            hotkeyButton.Text = "Hotkey";
            hotkeyButton.UseVisualStyleBackColor = true;
            hotkeyButton.Click += hotkeyButton_Click;
            // 
            // languageComboBox
            // 
            languageComboBox.FormattingEnabled = true;
            languageComboBox.Location = new Point(77, 64);
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Size = new Size(128, 23);
            languageComboBox.TabIndex = 4;
            // 
            // languageLabel
            // 
            languageLabel.AutoSize = true;
            languageLabel.Location = new Point(12, 67);
            languageLabel.Name = "languageLabel";
            languageLabel.Size = new Size(59, 15);
            languageLabel.TabIndex = 1;
            languageLabel.Text = "Language";
            // 
            // SaveBtn
            // 
            SaveBtn.Location = new Point(12, 118);
            SaveBtn.Name = "SaveBtn";
            SaveBtn.Size = new Size(75, 23);
            SaveBtn.TabIndex = 5;
            SaveBtn.Text = "Save";
            SaveBtn.UseVisualStyleBackColor = true;
            SaveBtn.Click += SaveBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.Location = new Point(130, 118);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(75, 23);
            CancelBtn.TabIndex = 6;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // hotkeyLabel
            // 
            hotkeyLabel.AutoSize = true;
            hotkeyLabel.Location = new Point(26, 39);
            hotkeyLabel.Name = "hotkeyLabel";
            hotkeyLabel.Size = new Size(45, 15);
            hotkeyLabel.TabIndex = 1;
            hotkeyLabel.Text = "Hotkey";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(223, 158);
            Controls.Add(CancelBtn);
            Controls.Add(SaveBtn);
            Controls.Add(languageComboBox);
            Controls.Add(hotkeyButton);
            Controls.Add(startWithWindowsCheckBox);
            Controls.Add(languageLabel);
            Controls.Add(hotkeyLabel);
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyTextBox);
            MdiChildrenMinimizedAnchorBottom = false;
            Name = "SettingsForm";
            Text = "Settings";
            Load += SettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox apiKeyTextBox;
        private Label apiKeyLabel;
        private CheckBox startWithWindowsCheckBox;
        private Button hotkeyButton;
        private ComboBox languageComboBox;
        private Label languageLabel;
        private Button SaveBtn;
        private Button CancelBtn;
        private Label hotkeyLabel;
    }
}
