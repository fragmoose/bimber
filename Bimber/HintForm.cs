using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bimber
{
    public partial class HintForm : Form
    {
        public HintForm(string hintText = "Default hint text")
        {
            InitializeComponent();

            // Set form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.LightYellow;
            this.Opacity = 0.9;

            // Create a label for the text
            Label hintLabel = new Label();
            hintLabel.Text = hintText;
            hintLabel.AutoSize = true;
            hintLabel.Padding = new Padding(5);
            this.Controls.Add(hintLabel);

            // Size the form to fit the label
            this.ClientSize = hintLabel.Size;

            // Position the form - ADD THIS LINE
            PositionForm();
        }

        // Add this method to position the form
        private void PositionForm()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(
                workingArea.Right - this.Width - 10,  // 10 pixels from right
                workingArea.Bottom - this.Height - 10 // 10 pixels from bottom
            );
        }
        private void HintForm_Load(object sender, EventArgs e)
        {
            PositionForm();
        }
    }
}
