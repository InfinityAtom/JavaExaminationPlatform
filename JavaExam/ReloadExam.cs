using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace JavaExam
{
    public partial class ReloadExam : Form
    {

        private string filePath = @"C:\TaskWorker\ExamBackup.json";
        public ReloadExam()
        {
            InitializeComponent();
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("jexam");
            this.pictureBox7.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox8.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            exam.Text = "JavaExam";
            dynamic jsonObj;
            string json = File.ReadAllText(@"C:\TaskWorker\ExamBackup.json");
            jsonObj = JsonConvert.DeserializeObject(json);
            // Extract the remainingTimeInSeconds value
            int remainingTimeInSeconds = jsonObj.remainingTimeInSeconds;
            // Calculate minutes and seconds
            int minutes = remainingTimeInSeconds / 60;
            int seconds = remainingTimeInSeconds % 60;
            // Format the string to "xx minutes, yy seconds"
            string timeLeft = $"{minutes} minutes, {seconds} seconds";
            remainingTime.Text= timeLeft ;
            label9.Text = GlobalUser.LoggedInUser.Email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.Delete(filePath);
            LogIn logIn = new LogIn();
            logIn.Show();
            Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ProctorReauth pauth = new ProctorReauth();
            pauth.Show();
            Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void exam_Click(object sender, EventArgs e)
        {

        }

        private void remainingTime_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
