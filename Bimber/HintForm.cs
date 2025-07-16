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

           
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.LightYellow;
            this.Opacity = 0.9;

            
            Label hintLabel = new Label();
            hintLabel.Text = hintText;
            hintLabel.AutoSize = true;
            hintLabel.Padding = new Padding(5);
            this.Controls.Add(hintLabel);

            
            this.ClientSize = hintLabel.Size;

            
            PositionForm();
        }

 
        private void PositionForm()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(
                workingArea.Right - this.Width - 10,  
                workingArea.Bottom - this.Height - 10 
            );
        }
        private void HintForm_Load(object sender, EventArgs e)
        {
            PositionForm();
        }
    }
}
