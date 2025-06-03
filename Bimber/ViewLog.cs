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
    public partial class ViewLog : Form
    {
        public ViewLog()
        {
            InitializeComponent();
            this.Text = Resources.Viewlog;
            
            pictureBox1.Parent = panelPreview; ;

            
            pictureBox1.Dock = DockStyle.Fill;

            
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;         
        }
    }
}
