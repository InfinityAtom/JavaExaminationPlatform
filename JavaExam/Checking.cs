using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;

namespace JavaExam
{
	public partial class Checking : Form
	{
		public int count = 0;
        private static string intelliJPath; // Class-level variable to store the IntelliJ path
        public static bool IsInternetConnected()
		{
			try
			{
				using (var client = new WebClient())
				{
					using (client.OpenRead("http://clients3.google.com/generate_204"))
					{
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}
		public static bool JavaExamFolderExists()
		{

			// Combine the AppData path with the folder name
			string folderPath = Path.Combine (Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"JavaExam");

			// Check if the folder exists
			return Directory.Exists(folderPath);
		}
		public static bool JavaTempFolderExists()
		{

			// Combine the AppData path with the folder name
			string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JavaTemp");

			// Check if the folder exists
			return Directory.Exists(folderPath);
		}

		public static bool IsRunningWithAdminRights()
	{
		WindowsIdentity identity = WindowsIdentity.GetCurrent();
		WindowsPrincipal principal = new WindowsPrincipal(identity);

		return principal.IsInRole(WindowsBuiltInRole.Administrator);
	}
	public static bool AreTwoOrMoreScreensConnected()
		{
			return Screen.AllScreens.Length >= 2;
		}
		public static bool IsScreenDuplicated()
		{
			var screenBounds = Screen.AllScreens.Select(screen => screen.Bounds).ToList();
			for (int i = 0; i < screenBounds.Count; i++)
			{
				for (int j = i + 1; j < screenBounds.Count; j++)
				{
					if (screenBounds[i].Equals(screenBounds[j]))
					{
						return true;
					}
				}
			}
			return false;
		}
        private static bool IsIntelliJInstalled()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    string rootDirectory = drive.RootDirectory.FullName;
                    string[] possiblePaths = {
                    Path.Combine(rootDirectory, "Program Files", "JetBrains"),
                    Path.Combine(rootDirectory, "Program Files (x86)", "JetBrains")
                };

                    foreach (string path in possiblePaths)
                    {
                        if (Directory.Exists(path))
                        {
                            string[] directories = Directory.GetDirectories(path);
                            foreach (string directory in directories)
                            {
                                if (directory.Contains("IntelliJ"))
                                {
                                    intelliJPath = directory;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Drive {drive.Name} is not ready.");
                }
            }

            return false;
        }

        public void Check()
		{
			////Connectivity checking
			bool isConnected = IsInternetConnected();
			if (isConnected == true)
			{
				count++;
				pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button6.Visible = true;
			}
			////
			////JavaExam folder checking
			
			bool folderExists = JavaExamFolderExists();
			if (folderExists == true)
			{
				count++;
				pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button5.Visible = true;
			}
			////
			////JavaTemp folder checking
			
			bool folderExists2 = JavaTempFolderExists();
			if (folderExists2 == true)
			{
				count++;
				pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button4.Visible = true;
			}
			////
			////Screens Count
			bool twoOrMoreScreens = AreTwoOrMoreScreensConnected();
			//bool duplicatedScreens = IsScreenDuplicated();
			if (twoOrMoreScreens == false)
			{
				count++;
				pictureBox4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button3.Visible = true;
			}
			////
			////Administrative Rights
			bool isAdmin = IsRunningWithAdminRights();
			if(isAdmin==true)
			{
				count++;
				pictureBox5.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox5.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button2.Visible = true;
			}
			////
			////IntelliJ Check
			bool IntelliJcheck = IsIntelliJInstalled();
			if (IntelliJcheck==true)
			{
				count++;
				pictureBox6.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
			}
			else
			{
				pictureBox6.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
				button1.Visible = true;
			}
			////Launch exam Check
			if (count==6) 
			{
				button7.Visible = true;
			}
			else
			{
				button7.Visible=false;
			}
		}
		public Checking()
		{
			InitializeComponent();
            ////Initialising
            this.pictureBox8.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox7.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            button6.Visible = false;
			button5.Visible = false;
			button4.Visible = false;
			button3.Visible = false;
			button2.Visible = false;
			button1.Visible = false;
			button7.Visible=false;
			////
			Check();
            if (IsIntelliJInstalled())
            {
				GlobalPath.IJPath = intelliJPath + "\\bin\\idea64.exe";
            }
            else
            {
                MessageBox.Show("IntelliJ not found!", "IntelliJ Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }

        }

		private void panel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void button6_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Make Sure you have a stable internet connection.\nTry closing the app, connecting to the internet and then try again pressing on \"Refresh\"", "Connectivity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			MessageBox.Show("JavaExam folder (an important folder for running the exam) is missing!\nTry reinstalling the application, or checking your antivirus software", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			MessageBox.Show("JavaTemp folder (an important folder for running the exam) is missing!\nTry reinstalling the application, or checking your antivirus software", "Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Only one screen is allowed!\nDisconnect any external screens, then try again pressing on \"Refresh\"", "External Screens Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			////Initialising
			button6.Visible = false;
			button5.Visible = false;
			button4.Visible = false;
			button3.Visible = false;
			button2.Visible = false;
			button1.Visible = false;
			button7.Visible = false;
			count = 0;
			////
			Check();
		}

		private void button2_Click(object sender, EventArgs e)
		{


			if (MessageBox.Show("You need to run this app as an administrator!\nClick on OK, and the application will restart.\n\nYOU WILL HAVE TO LOG IN AGAIN!", "Administrative rights", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK && !IsRunningWithAdminRights())
				{
				// Start a new instance of the application with administrative rights
				var processStartInfo = new ProcessStartInfo
				{
					FileName = Application.ExecutablePath,
					Verb = "runas",
					UseShellExecute = true
				};

				try
				{
					Process.Start(processStartInfo);
					 Environment.Exit(0); // Close the current instance of the application
				}
				catch (Exception ex)
				{
					// Log the exception or show a message to the user
					MessageBox.Show("Failed to restart the application with administrative rights: " + ex.Message);
				}
			}
			else
			{
				MessageBox.Show("The application is already running with administrative rights.");
			}
		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			MessageBox.Show("IntelliJ IDEA is missing.\nIf you didn't installed it, download it from https://jetbrains.com and then try again by pressing on \"Refresh\"\nIf you however are sure that the app is intalled, try reinstalling it and then try again by pressing on \"Refresh\"", "IntelliJ Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void button7_Click(object sender, EventArgs e)
		{
			IntelliJVersionSelector tutorial = new IntelliJVersionSelector();
			tutorial.Show();
			Hide();
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

        private void Checking_FormClosed(object sender, FormClosedEventArgs e)
        {
			 Environment.Exit(0);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void Checking_Load(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
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

