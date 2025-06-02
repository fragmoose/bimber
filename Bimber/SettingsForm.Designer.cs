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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            startWithWindowsCheckBox = new CheckBox();
            hotkeyButton = new Button();
            languageComboBox = new ComboBox();
            languageLabel = new Label();
            SaveBtn = new Button();
            CancelBtn = new Button();
            hotkeyLabel = new Label();
            saveLocallyCheckBox = new CheckBox();
            folderPathTextBox = new TextBox();
            pathTextLabel = new Label();
            browseFolderButton = new Button();
            folderDialog = new FolderBrowserDialog();
            logLinkLabel = new LinkLabel();
            comboBox1 = new ComboBox();
            apiKeyTextBox = new TextBox();
            apiKeyLabel = new Label();
            errorProvider1 = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // startWithWindowsCheckBox
            // 
            startWithWindowsCheckBox.AutoSize = true;
            startWithWindowsCheckBox.Location = new Point(12, 306);
            startWithWindowsCheckBox.Name = "startWithWindowsCheckBox";
            startWithWindowsCheckBox.Size = new Size(128, 19);
            startWithWindowsCheckBox.TabIndex = 7;
            startWithWindowsCheckBox.Text = "Start with Windows";
            startWithWindowsCheckBox.UseVisualStyleBackColor = true;
            startWithWindowsCheckBox.CheckedChanged += startWithWindowsCheckBox_CheckedChanged;
            // 
            // hotkeyButton
            // 
            hotkeyButton.Location = new Point(12, 71);
            hotkeyButton.Name = "hotkeyButton";
            hotkeyButton.Size = new Size(128, 23);
            hotkeyButton.TabIndex = 2;
            hotkeyButton.Text = "Hotkey";
            hotkeyButton.UseVisualStyleBackColor = true;
            hotkeyButton.Click += hotkeyButton_Click;
            // 
            // languageComboBox
            // 
            languageComboBox.FormattingEnabled = true;
            languageComboBox.Location = new Point(12, 115);
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Size = new Size(128, 23);
            languageComboBox.TabIndex = 3;
            // 
            // languageLabel
            // 
            languageLabel.AutoSize = true;
            languageLabel.Location = new Point(10, 97);
            languageLabel.Name = "languageLabel";
            languageLabel.Size = new Size(59, 15);
            languageLabel.TabIndex = 1;
            languageLabel.Text = "Language";
            languageLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // SaveBtn
            // 
            SaveBtn.Location = new Point(12, 331);
            SaveBtn.Name = "SaveBtn";
            SaveBtn.Size = new Size(75, 23);
            SaveBtn.TabIndex = 8;
            SaveBtn.Text = "Save";
            SaveBtn.UseVisualStyleBackColor = true;
            SaveBtn.Click += SaveBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.Location = new Point(194, 331);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(75, 23);
            CancelBtn.TabIndex = 9;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // hotkeyLabel
            // 
            hotkeyLabel.AutoSize = true;
            hotkeyLabel.Location = new Point(10, 53);
            hotkeyLabel.Name = "hotkeyLabel";
            hotkeyLabel.Size = new Size(45, 15);
            hotkeyLabel.TabIndex = 1;
            hotkeyLabel.Text = "Hotkey";
            hotkeyLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // saveLocallyCheckBox
            // 
            saveLocallyCheckBox.AutoSize = true;
            saveLocallyCheckBox.Location = new Point(12, 144);
            saveLocallyCheckBox.Name = "saveLocallyCheckBox";
            saveLocallyCheckBox.Size = new Size(87, 19);
            saveLocallyCheckBox.TabIndex = 4;
            saveLocallyCheckBox.Text = "Save locally";
            saveLocallyCheckBox.UseVisualStyleBackColor = true;
            saveLocallyCheckBox.CheckedChanged += saveLocallyCheckBox_CheckedChanged;
            // 
            // folderPathTextBox
            // 
            folderPathTextBox.Location = new Point(12, 184);
            folderPathTextBox.Name = "folderPathTextBox";
            folderPathTextBox.Size = new Size(128, 23);
            folderPathTextBox.TabIndex = 5;
            // 
            // pathTextLabel
            // 
            pathTextLabel.AutoSize = true;
            pathTextLabel.Location = new Point(12, 166);
            pathTextLabel.Name = "pathTextLabel";
            pathTextLabel.Size = new Size(67, 15);
            pathTextLabel.TabIndex = 1;
            pathTextLabel.Text = "Folder path";
            pathTextLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // browseFolderButton
            // 
            browseFolderButton.Location = new Point(146, 184);
            browseFolderButton.Name = "browseFolderButton";
            browseFolderButton.Size = new Size(121, 23);
            browseFolderButton.TabIndex = 6;
            browseFolderButton.Text = "Browse";
            browseFolderButton.UseVisualStyleBackColor = true;
            browseFolderButton.Click += browseFolderButton_Click;
            // 
            // logLinkLabel
            // 
            logLinkLabel.AutoEllipsis = true;
            logLinkLabel.AutoSize = true;
            logLinkLabel.Location = new Point(12, 372);
            logLinkLabel.Name = "logLinkLabel";
            logLinkLabel.RightToLeft = RightToLeft.Yes;
            logLinkLabel.Size = new Size(27, 15);
            logLinkLabel.TabIndex = 10;
            logLinkLabel.TabStop = true;
            logLinkLabel.Text = "Log";
            logLinkLabel.TextAlign = ContentAlignment.MiddleRight;
            logLinkLabel.VisitedLinkColor = Color.Blue;
            logLinkLabel.LinkClicked += linkLabel1_LinkClicked;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(146, 23);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 1;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // apiKeyTextBox
            // 
            apiKeyTextBox.Location = new Point(10, 23);
            apiKeyTextBox.Name = "apiKeyTextBox";
            apiKeyTextBox.Size = new Size(128, 23);
            apiKeyTextBox.TabIndex = 0;
            apiKeyTextBox.TextChanged += apiKeyTextBox_TextChanged;
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new Point(12, 5);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new Size(47, 15);
            apiKeyLabel.TabIndex = 1;
            apiKeyLabel.Text = "API Key";
            apiKeyLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(277, 396);
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyTextBox);
            Controls.Add(logLinkLabel);
            Controls.Add(comboBox1);
            Controls.Add(browseFolderButton);
            Controls.Add(folderPathTextBox);
            Controls.Add(saveLocallyCheckBox);
            Controls.Add(CancelBtn);
            Controls.Add(SaveBtn);
            Controls.Add(languageComboBox);
            Controls.Add(hotkeyButton);
            Controls.Add(startWithWindowsCheckBox);
            Controls.Add(pathTextLabel);
            Controls.Add(languageLabel);
            Controls.Add(hotkeyLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            Text = "Settings";
            Load += SettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox startWithWindowsCheckBox;
        private Button hotkeyButton;
        private ComboBox languageComboBox;
        private Label languageLabel;
        private Button SaveBtn;
        private Button CancelBtn;
        private Label hotkeyLabel;
        private CheckBox saveLocallyCheckBox;
        private TextBox folderPathTextBox;
        private Label pathTextLabel;
        private Button browseFolderButton;
        private FolderBrowserDialog folderDialog;
        private LinkLabel logLinkLabel;
        private ComboBox comboBox1;
        private TextBox apiKeyTextBox;
        private Label apiKeyLabel;
        private ErrorProvider errorProvider1;
    }
}
