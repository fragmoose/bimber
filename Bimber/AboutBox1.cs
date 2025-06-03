using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Bimber
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();

            // Fixed string formatting (removed curly braces from Resources access)
            this.Text = string.Format(Resources.About, AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = $"{Resources.Version} {AssemblyVersion}"; // Using string interpolation
   
            this.author.Text = $"{Resources.author} fragmoose";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                var titleAttr = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyTitleAttribute>();
                return titleAttr?.Title ??
                       System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }

        public string AssemblyVersion
        {
            get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }



        public string AssemblyProduct
        {
            get => Assembly.GetExecutingAssembly()
                   .GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;
        }




        #endregion

        private void author_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/fragmoose/bimber",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}