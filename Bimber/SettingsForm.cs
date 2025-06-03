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
            logLinkLabel.Click += (s, e) => linkLabel1_LinkClicked(s, new LinkLabelLinkClickedEventArgs(null));
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

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            var previewUrlHolder = new PreviewUrlHolder();

            try
            {
                if (File.Exists(logFilePath))
                {
                    string[] logLines = File.ReadAllLines(logFilePath);
                    Array.Reverse(logLines);

                    ViewLog logViewer = Application.OpenForms.OfType<ViewLog>().FirstOrDefault() ?? new ViewLog();

                    logViewer.Text = Resources.UploadLog;
                    logViewer.StartPosition = FormStartPosition.CenterParent;

                    // Configure DataGridView
                    ConfigureDataGridView(logViewer);

                    // Parse and add log entries
                    foreach (var line in logLines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length >= 2)
                        {
                            logViewer.dataGridView1.Rows.Add(
                                parts[0].Trim(),
                                parts.Length > 1 ? parts[1].Trim() : "N/A",
                                parts[parts.Length - 1].Trim()
                            );
                        }
                    }

                    // Configure URL column
                    logViewer.dataGridView1.Columns["ImageURL"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    logViewer.dataGridView1.Columns["ImageURL"].DefaultCellStyle.ForeColor = Color.Blue;
                    logViewer.dataGridView1.Columns["ImageURL"].DefaultCellStyle.Font =
                        new Font(logViewer.dataGridView1.Font, FontStyle.Underline);

                    // Initialize preview PictureBox if not exists
                    if (logViewer.pictureBox1 == null)
                    {
                        logViewer.pictureBox1 = new PictureBox
                        {
                            SizeMode = PictureBoxSizeMode.Zoom,
                            BackColor = Color.White,
                            Visible = false,
                        };
                        logViewer.Controls.Add(logViewer.pictureBox1);
                        //logViewer.pictureBox1.BringToFront();

                        // Position it (if needed)
                        logViewer.pictureBox1.Location = new Point(
                            logViewer.Width - logViewer.pictureBox1.Width - 20,
                            20
                        );
                    }



                    ToolTip copyToolTip = new ToolTip
                    {
                        IsBalloon = true,
                        ToolTipIcon = ToolTipIcon.Info,
                        ToolTipTitle = "Success",
                        AutoPopDelay = 2000,
                        InitialDelay = 0,
                        ReshowDelay = 0
                    };

                    // Setup event handlers
                    SetupGridViewEventHandlers(logViewer, copyToolTip, previewUrlHolder);

                    logViewer.Show();
                }
                else
                {
                    MessageBox.Show(Resources.Nolog, Resources.UploadLog,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log Reading error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView(ViewLog logViewer)
        {
            logViewer.dataGridView1.Rows.Clear();
            logViewer.dataGridView1.Columns.Clear();

            logViewer.dataGridView1.AllowUserToAddRows = false;
            logViewer.dataGridView1.AllowUserToDeleteRows = false;
            logViewer.dataGridView1.ReadOnly = true;
            logViewer.dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            logViewer.dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            logViewer.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            logViewer.dataGridView1.RowHeadersVisible = false;
            logViewer.dataGridView1.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Consolas", 10),
                SelectionBackColor = Color.LightGray,
                SelectionForeColor = Color.Black
            };

            logViewer.dataGridView1.Columns.Add("Timestamp", Resources.Timestamp);
            logViewer.dataGridView1.Columns.Add("LocalPath", Resources.LocalPath);
            logViewer.dataGridView1.Columns.Add("ImageURL", Resources.ImageURL);
        }
        private class PreviewUrlHolder
        {
            public string CurrentPreviewUrl { get; set; }
        }
        private void SetupGridViewEventHandlers(ViewLog logViewer, ToolTip copyToolTip, PreviewUrlHolder urlHolder)
        {
            bool suppressContextMenu = false;

            // Mouse down event for context menu suppression
            logViewer.dataGridView1.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    var hitTest = logViewer.dataGridView1.HitTest(ev.X, ev.Y);
                    if (hitTest.Type == DataGridViewHitTestType.Cell &&
                        hitTest.ColumnIndex == logViewer.dataGridView1.Columns["ImageURL"].Index)
                    {
                        suppressContextMenu = true;
                    }
                }
            };

            // Context menu handling
            logViewer.dataGridView1.ContextMenuStripChanged += (s, ev) =>
            {
                if (suppressContextMenu)
                {
                    logViewer.dataGridView1.ContextMenuStrip = null;
                    suppressContextMenu = false;
                }
            };

            // Image preview on mouse enter
            logViewer.dataGridView1.CellMouseEnter += (s, ev) =>
            {
                if (ev.ColumnIndex == logViewer.dataGridView1.Columns["ImageURL"].Index && ev.RowIndex >= 0)
                {
                    string url = logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex].Value?.ToString();

                    if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
                        url != urlHolder.CurrentPreviewUrl)
                    {
                        urlHolder.CurrentPreviewUrl = url;
                        logViewer.pictureBox1.Visible = true;
                        _ = LoadImagePreviewAsync(url, logViewer);
                    }
                }
            };

            // Hide preview on mouse leave
            logViewer.dataGridView1.CellMouseLeave += (s, ev) =>
            {
                if (ev.ColumnIndex == logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    urlHolder.CurrentPreviewUrl = null;
                    logViewer.pictureBox1.Visible = false;
                }
            };

            // Cell mouse click handlers
            logViewer.dataGridView1.CellMouseDown += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    var cell = logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex];
                    string url = cell.Value?.ToString();

                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            Clipboard.SetText(url);

                            if (ev.Button == MouseButtons.Left && logViewer.pictureBox1.Visible)
                            {
                                ShowCopyConfirmation(logViewer);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to copy URL: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Double click to open URL
            logViewer.dataGridView1.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    string url = logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = url,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Cannot open url: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };

            // Cleanup when form is closed
            logViewer.FormClosed += (s, ev) =>
            {
                logViewer.pictureBox1?.Image?.Dispose();
                copyToolTip.Dispose();
            };
        }



        private async Task LoadImagePreviewAsync(string imageUrl, ViewLog logViewer)
        {
            try
            {
                // Make sure PictureBox is visible and ready
                logViewer.pictureBox1.Visible = true;
                logViewer.pictureBox1.BringToFront();

                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        // Clear any existing image first
                        if (logViewer.pictureBox1.Image != null)
                        {
                            logViewer.pictureBox1.Image.Dispose();
                            logViewer.pictureBox1.Image = null;
                        }

                        Image image = Image.FromStream(ms);
                        logViewer.pictureBox1.Image = image;
                        logViewer.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        logViewer.pictureBox1.Refresh(); // Force immediate redraw
                    }
                }
            }
            catch (Exception ex)
            {
                logViewer.pictureBox1.Image = null;
                logViewer.pictureBox1.Visible = false;
                Console.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void ShowCopyConfirmation(ViewLog logViewer)
        {
            logViewer.Invoke(new Action(() =>
            {
                var hintDisplayer = new HintDisplayer();
                hintDisplayer.ShowHint(Resources.Message);
            }));
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