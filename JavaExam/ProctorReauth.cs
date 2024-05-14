using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class ProctorReauth : Form
    {
        public ProctorReauth()
        {
            InitializeComponent();
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string enteredEmail = textBox1.Text;
            string enteredPassword = textBox2.Text;

            GlobalProctor.FetchProctorByEmailAndPassword(enteredEmail, enteredPassword);

            if (GlobalProctor.LoggedInProctor == null)
            {
                MessageBox.Show("Invalid credentials.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (GlobalUser.LoggedInUser.ProctorId != GlobalProctor.LoggedInProctor.ProctorId)
            {
                MessageBox.Show("Proctor does not match the student's proctor.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (checkBox1.CheckState == CheckState.Unchecked)
            {
                MessageBox.Show("The agreement was not accepted! Accept the agreement, and try again", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                    Splash splash = new Splash();
                    splash.Show();
                    Hide();// 
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Agreement ag = new Agreement();
            ag.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
