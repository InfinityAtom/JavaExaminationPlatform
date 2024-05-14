using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
	public partial class IntelliJVersionSelector : Form
	{
		public IntelliJVersionSelector()
		{
			InitializeComponent();
            this.pictureBox8.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox7.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
			textBox1.Text = GlobalPath.IJPath;
        }

		private void IntelliJVersionSelector_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				// Set the filter for .exe files
				openFileDialog.Filter = "Executable Files (*.exe)|*.exe";
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					// Get the selected file's full path
					string filePath = openFileDialog.FileName;

					// Display the full path in the label
					textBox1.Text = filePath;
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);


			if (textBox1.Text!="" && textBox1.Text.EndsWith("idea64.exe")==true)
			{		
					GlobalPath.IJPath = textBox1.Text;
					ProctorLogin pl = new ProctorLogin();
					pl.Show();
					Hide();
			}
			else
			{
				MessageBox.Show("Invalid IntelliJ Path", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				textBox1.Text = "";
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

        private void IntelliJVersionSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
			 Environment.Exit(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C START explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restarting explorer.exe: " + ex.Message);
            }
             Environment.Exit(0);
        }
    }
}
