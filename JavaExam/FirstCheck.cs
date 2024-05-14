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
	public partial class FirstCheck : Form
	{
	//	public Studenti Data { get; set; }
		public FirstCheck()
		{
			InitializeComponent();
			UpdateLabels();
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            this.pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("EditResultsBanner");
        }

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

		private void groupBox2_Enter(object sender, EventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				int studentId = GlobalUser.LoggedInUser.StudnetId; // Note the typo in `StudnetId`. It should be `StudentId`.
				GlobalBooking.FetchBookingByStudentId(studentId);
				int bookingId = GlobalBooking.CurrentBooking.BookingId;
				string date = GlobalBooking.CurrentBooking.BookingDate.ToString();
				bool isBookingToday = (GlobalBooking.CurrentBooking.BookingDate >= DateTime.Today)&&(GlobalBooking.CurrentBooking.BookingDate < DateTime.Today.AddDays(1));

				if (bookingId != null)
				{
					if (isBookingToday == false)
					{
						if (MessageBox.Show($"Your exam is programmed on: {date}! You have no access to the exam, right now!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK)
						{
								 Environment.Exit(0);
						}
					}
					else 
					{ 
					Checking checking = new Checking();
					checking.Show();
					Hide();
					}
					
				}
				
			}
			catch(System.NullReferenceException) 
			{
                if (MessageBox.Show($"It looks like you didn't booked your exam yet! Book your exam, and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK)
                {
                     Environment.Exit(0);
                }
            }
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Hide();
			LogIn logIn = new LogIn();
			logIn.Show();
		}
		public void UpdateLabels()
		{
				lblNume.Text = GlobalUser.LoggedInUser.LastName;
				lblPrenume.Text = GlobalUser.LoggedInUser.FirstName;
                lblFacultate.Text = GlobalUser.LoggedInUser.Faculty;
                
				lblAn.Text = GlobalUser.LoggedInUser.Year;
				lblGrupa.Text = GlobalUser.LoggedInUser.Groupa;
		}
		private void FirstCheck_Load(object sender, EventArgs e)
		{
			
		}

        private void FirstCheck_FormClosed(object sender, FormClosedEventArgs e)
        {
			 Environment.Exit(0);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
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

        private void lblPrenume_Click(object sender, EventArgs e)
        {

        }
    }
}
