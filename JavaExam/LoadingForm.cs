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
using Microsoft.Win32;
namespace JavaExam
{
    public partial class LoadingForm : Form
    {
        private Random random = new Random(); // Declare random here


        public LoadingForm()
        {
            InitializeComponent();
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C TASKKILL /F /IM explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);
                string backgroundColor = "255 255 255"; // This would set it to white
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Colors", true);
                if (key != null)
                {
                    key.SetValue("Background", backgroundColor);
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error killing explorer.exe: " + ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            timer1.Interval = random.Next(5000, 10000); // Random interval between 5 to 10 seconds
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();  // Stop the timer to prevent further ticks

            // Check if the LogIn form is already open
            if (Application.OpenForms.OfType<LogIn>().Any())
            {
                // If already open, don't create a new instance
                return;
            }

            // Show the next form (LogIn)
            LogIn loginForm = new LogIn();
            loginForm.Show();

            // Hide the current form
            this.Hide();
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {

        }
    }
}
