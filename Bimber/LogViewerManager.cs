using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bimber
{
    public class LogViewerManager
    {
        private ViewLog _logViewer;
        private PreviewUrlHolder _urlHolder = new PreviewUrlHolder();
        private ToolTip _copyToolTip;
        private bool _disposed = false;
        public bool IsViewerDisposed => _logViewer?.IsDisposed ?? true;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_logViewer != null && !_logViewer.IsDisposed)
                    {
                        _logViewer.Dispose();
                    }
                    _copyToolTip?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public LogViewerManager()
        {
            _logViewer = new ViewLog();
            _copyToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Info,
                ToolTipTitle = "Success",
                AutoPopDelay = 2000,
                InitialDelay = 0,
                ReshowDelay = 0
            };

            InitializeViewer();
        }

        private void InitializeViewer()
        {
            _logViewer.Text = Resources.UploadLog;
            _logViewer.StartPosition = FormStartPosition.CenterParent;

            ConfigureDataGridView();
            SetupEventHandlers();
        }

        public void ShowLogs()
        {
            // Create a new instance if the form was disposed
            if (_logViewer == null || _logViewer.IsDisposed)
            {
                _logViewer = new ViewLog();
                InitializeViewer(); // Re-initialize the form
            }

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            try
            {
                if (File.Exists(logFilePath))
                {
                    ConfigureDataGridView();
                    LoadLogsFromFile(logFilePath);
                    _logViewer.Show();
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




        private void LoadLogsFromFile(string filePath)
        {
            string[] logLines = File.ReadAllLines(filePath);
            Array.Reverse(logLines);

            // Calculate column widths (30%, 30%, 40%)
            int totalWidth = _logViewer.dataGridView1.ClientSize.Width;
            int timestampWidth = (int)(totalWidth * 0.30);
            int localPathWidth = (int)(totalWidth * 0.30);
            int imageUrlWidth = (int)(totalWidth * 0.40);

            // Set column widths
            _logViewer.dataGridView1.Columns["Timestamp"].Width = timestampWidth;
            _logViewer.dataGridView1.Columns["LocalPath"].Width = localPathWidth;
            _logViewer.dataGridView1.Columns["ImageURL"].Width = imageUrlWidth;

            // Disable auto-sizing to enforce fixed widths
            _logViewer.dataGridView1.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _logViewer.dataGridView1.Columns["LocalPath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _logViewer.dataGridView1.Columns["ImageURL"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            // Load data
            foreach (var line in logLines)
            {
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    _logViewer.dataGridView1.Rows.Add(
                        parts[0].Trim(),
                        parts.Length > 1 ? parts[1].Trim() : "N/A",
                        parts[parts.Length - 1].Trim()
                    );
                }
            }

            // Style the ImageURL column (keep your existing styling)
            _logViewer.dataGridView1.Columns["ImageURL"].DefaultCellStyle.ForeColor = Color.Blue;
            _logViewer.dataGridView1.Columns["ImageURL"].DefaultCellStyle.Font =
                new Font(_logViewer.dataGridView1.Font, FontStyle.Underline);
        }

        private void ConfigureDataGridView()
        {
            _logViewer.dataGridView1.Rows.Clear();
            _logViewer.dataGridView1.Columns.Clear();

            _logViewer.dataGridView1.AllowUserToAddRows = false;
            _logViewer.dataGridView1.AllowUserToDeleteRows = false;
            _logViewer.dataGridView1.ReadOnly = true;
            _logViewer.dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _logViewer.dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            _logViewer.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            _logViewer.dataGridView1.RowHeadersVisible = false;
            _logViewer.dataGridView1.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Consolas", 10),
                SelectionBackColor = Color.LightGray,
                SelectionForeColor = Color.Black
            };

            _logViewer.dataGridView1.Columns.Add("Timestamp", Resources.Timestamp);
            _logViewer.dataGridView1.Columns.Add("LocalPath", Resources.LocalPath);
            _logViewer.dataGridView1.Columns.Add("ImageURL", Resources.ImageURL);
        }

        

        private void SetupEventHandlers()
        {
            bool suppressContextMenu = false;

            _logViewer.dataGridView1.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    var hitTest = _logViewer.dataGridView1.HitTest(ev.X, ev.Y);
                    if (hitTest.Type == DataGridViewHitTestType.Cell &&
                        hitTest.ColumnIndex == _logViewer.dataGridView1.Columns["ImageURL"].Index)
                    {
                        suppressContextMenu = true;
                    }
                }
            };

            _logViewer.dataGridView1.ContextMenuStripChanged += (s, ev) =>
            {
                if (suppressContextMenu)
                {
                    _logViewer.dataGridView1.ContextMenuStrip = null;
                    suppressContextMenu = false;
                }
            };
            _logViewer.FormClosed += (s, ev) =>
            {
                // Clear references to avoid accessing disposed objects
                _logViewer.pictureBox1?.Image?.Dispose();
                _copyToolTip.Dispose();
            };

            _logViewer.dataGridView1.CellMouseEnter += (s, ev) =>
            {
                if (ev.ColumnIndex == _logViewer.dataGridView1.Columns["ImageURL"].Index && ev.RowIndex >= 0)
                {
                    string url = _logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex].Value?.ToString();

                    if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
                        url != _urlHolder.CurrentPreviewUrl)
                    {
                        _urlHolder.CurrentPreviewUrl = url;
                        _ = LoadImagePreviewAsync(url);
                    }
                }
            };

            _logViewer.dataGridView1.CellMouseLeave += (s, ev) =>
            {
                if (ev.ColumnIndex == _logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    _urlHolder.CurrentPreviewUrl = null;
                }
            };
            _logViewer.dataGridView1.CellClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == _logViewer.dataGridView1.Columns["LocalPath"].Index)
                {
                    var cell = _logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex];
                    string localPath = cell.Value?.ToString();

                    if (!string.IsNullOrEmpty(localPath) && localPath != "N/A" && File.Exists(localPath))
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = localPath,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to open file: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };
            _logViewer.dataGridView1.CellMouseDown += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == _logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    var cell = _logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex];
                    string url = cell.Value?.ToString();

                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            Clipboard.SetText(url);

                            if (ev.Button == MouseButtons.Left && _logViewer.pictureBox1.Visible)
                            {
                                ShowCopyConfirmation();
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

            _logViewer.dataGridView1.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == _logViewer.dataGridView1.Columns["ImageURL"].Index)
                {
                    string url = _logViewer.dataGridView1.Rows[ev.RowIndex].Cells[ev.ColumnIndex].Value?.ToString();
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

            _logViewer.FormClosed += (s, ev) =>
            {
                _logViewer.pictureBox1?.Image?.Dispose();
                _copyToolTip.Dispose();
            };
        }

        private async Task LoadImagePreviewAsync(string imageUrl)
        {
            try
            {
                // Check if the form or pictureBox is disposed
                if (_logViewer.IsDisposed || _logViewer.pictureBox1.IsDisposed)
                    return;

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

                    // Check again after await
                    if (_logViewer.IsDisposed || _logViewer.pictureBox1.IsDisposed)
                        return;

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        var newImage = Image.FromStream(ms);

                        // More disposal checks
                        if (_logViewer.IsDisposed || _logViewer.pictureBox1.IsDisposed)
                        {
                            newImage.Dispose();
                            return;
                        }

                        // Safe to update the UI
                        _logViewer.Invoke((MethodInvoker)delegate {
                            if (!_logViewer.IsDisposed && !_logViewer.pictureBox1.IsDisposed)
                            {
                                _logViewer.pictureBox1.Image?.Dispose();
                                _logViewer.pictureBox1.Image = newImage;
                                _logViewer.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                newImage.Dispose();
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image preview: {ex.Message}");
            }
        }

        private void ShowCopyConfirmation()
        {
            _logViewer.Invoke(new Action(() =>
            {
                var hintDisplayer = new HintDisplayer();
                hintDisplayer.ShowHint(Resources.Message);
            }));
        }

        private class PreviewUrlHolder
        {
            public string CurrentPreviewUrl { get; set; }
        }
    }
}