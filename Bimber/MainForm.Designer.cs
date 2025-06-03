namespace Bimber
{
    partial class MainForm
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

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            exitToolStripMenuItem = new ToolStripMenuItem();
            trayIcon = new NotifyIcon(components);
            trayMenu = new ContextMenuStrip(components);
            updateStripMenuItem1 = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            trayMenu.SuspendLayout();
            SuspendLayout();
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // trayIcon
            // 
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Icon = (Icon)resources.GetObject("trayIcon.Icon");
            trayIcon.Text = "Bimber";
            trayIcon.Visible = true;
            // 
            // trayMenu
            // 
            trayMenu.Items.AddRange(new ToolStripItem[] { updateStripMenuItem1, settingsToolStripMenuItem, exitToolStripMenuItem });
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new Size(181, 92);
            trayMenu.Opening += trayMenu_Opening;
            // 
            // updateStripMenuItem1
            // 
            updateStripMenuItem1.Name = "updateStripMenuItem1";
            updateStripMenuItem1.Size = new Size(180, 22);
            updateStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(180, 22);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(273, 187);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            ShowInTaskbar = false;
            Text = "Form1";
            WindowState = FormWindowState.Minimized;
            Load += MainForm_Load;
            trayMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem settingsToolStripMenuItem;
    
    private void MainForm_Load(object sender, EventArgs e)
        {
            // Your form load initialization code here
            // Example:
            this.Hide(); // For background applications
            trayIcon.Visible = true;
        }
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem updateStripMenuItem1;
    } }
