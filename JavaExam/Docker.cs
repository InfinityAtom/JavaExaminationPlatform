using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace JavaExam
{

	public partial class Docker : Form
	{
		public string Overview="";
		public string Task1="";
		public string Task2="";
		public string Task3="";
		public string Task4= "";
		public string path = @"C:\TaskWorker\TaskCreator\tasks.txt";
		public string fileContent = File.ReadAllText(@"C:\TaskWorker\TaskCreator\tasks.txt");
		private const string registryPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
		public int selectedTask = 0;
		public bool com1=false;
		public bool com2=false;
		public bool com3=false;
		public bool com4=false;
		public bool mark1=false;
		public bool mark2=false;
		public bool mark3= false;
		public bool mark4 = false;
		public bool feed1 = false;
		public bool feed2 = false;
		public bool feed3 = false;
		public bool feed4 = false;
		public int numberFeedback = 0;
		public int numberCompleted = 0;
		public int numberMarked = 0;
		public string FinalPath = "";
		public bool FeedbackOn=false;
        public bool ReviewOn = false;
		public int countTimerPresses = 0;
        private System.Threading.Timer updateTimer;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        [DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]


		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		private const uint SWP_NOZORDER = 0x0004;
		private const uint SWP_SHOWWINDOW = 0x0040;
		private TimeSpan timeLeft;
		public string windowName = "SunAwtFrame";
		public bool LockExit = true;
		public string examstartdate;
		public string remainingBackupTimeinSeconds;
		public Docker()
		{
			InitializeComponent();
            this.reviewImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.button3.BackgroundImage = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ios-timer-4");
            CreateCsvFile(fileContent);
			File.Copy(FinalPath, @"C:\TaskWorker\TaskCreator\csvFile.csv", true);
			selectedTask = 0;
			LockKeys();
			//this.brandImage .Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
			this.TopMost = true;
			this.StartPosition = FormStartPosition.Manual;
			this.Width = Screen.PrimaryScreen.WorkingArea.Width;
			this.Location = new System.Drawing.Point(0, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
			this.Resize += OnFormResize;
			this.Move += OnFormMove;
			this.FormClosed += OnFormClosed;
			//BlockWebsites(); // Add this line
			DockAppToTop();
            this.Shown += Docker_Shown;
            SetWindowDisplayAffinity(this.Handle, 0);
            string pattern = @"Overview:(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern, RegexOptions.Singleline);

			if (match.Success)
			{
				Overview = match.Groups[1].Value.Trim();
				richTextBox1.Text = Overview;
			}
            string pattern2 = @"Domain:\s*(.*)";
            Match match2 = Regex.Match(File.ReadAllText(path), pattern2);

            if (match2.Success)
            {
                Overview = match2.Groups[1].Value.Trim();
                label4.Text = Overview;
            }

			if(!(File.Exists(@"C:\TaskWorker\ExamBackup.json")))
			{
            timeLeft = TimeSpan.FromSeconds(2700);
			label1.Text = timeLeft.ToString(@"hh\:mm\:ss");
			timer1.Start();
                examstartdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //System.Threading.Tasks.Task.Run(() => UpdateJsonWithDate());
                StartJsonUpdateAndSaveTask();
            }
			else
			{
            dynamic jsonObj;
                string json = File.ReadAllText(@"C:\TaskWorker\ExamBackup.json");
                jsonObj = JsonConvert.DeserializeObject(json);
				int remainingSeconds = jsonObj.remainingTimeInSeconds;
                timeLeft = TimeSpan.FromSeconds(remainingSeconds);
				label1.Text = timeLeft.ToString(@"hh\:mm\:ss");
				timer1.Start();
                //examstartdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //System.Threading.Tasks.Task.Run(() => UpdateJsonWithDate());
				List<int> completedBackup = new List<int>();
                List<int> reviewBackup = new List<int>();
                List<int> feedbackBackup = new List<int>();
				if(jsonObj.completedTasks.ToObject<List<int>>() != null) { 
                completedBackup.AddRange(jsonObj.completedTasks.ToObject<List<int>>());
				GlobalCompleted.tasks.UnionWith(completedBackup);
				GlobalPath.IJPath = jsonObj.examPath;
                }
				if (jsonObj.reviewTasks.ToObject<List<int>>() != null)
				{
					reviewBackup.AddRange(jsonObj.reviewTasks.ToObject<List<int>>());
					GlobalReview.tasks.UnionWith(reviewBackup);
				}
				if(jsonObj.feedbackTasks.ToObject<List<int>>() != null)
				{
				feedbackBackup.AddRange(jsonObj.feedbackTasks.ToObject<List<int>>());
				GlobalFeedback.tasks.UnionWith(feedbackBackup);
				}
				if(GlobalCompleted.tasks.Contains(1)) this.completedImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
				if(GlobalCompleted.tasks.Contains(2)) this.completedImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
				if(GlobalCompleted.tasks.Contains(3)) this.completedImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                if(GlobalCompleted.tasks.Contains(4)) this.completedImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                if (GlobalReview.tasks.Contains(1)) this.reviewImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                if (GlobalReview.tasks.Contains(2)) this.reviewImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                if (GlobalReview.tasks.Contains(3)) this.reviewImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                if (GlobalReview.tasks.Contains(4)) this.reviewImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                if (GlobalFeedback.tasks.Contains(1)) this.feedbackImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                if (GlobalFeedback.tasks.Contains(2)) this.feedbackImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                if (GlobalFeedback.tasks.Contains(3)) this.feedbackImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                if (GlobalFeedback.tasks.Contains(4)) this.feedbackImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                StartJsonUpdateAndSaveTask();
            }


        }

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;
        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_MAXIMIZEBOX = 0x10000;
        public void BlockIntelliJCapture()
        {
            IntPtr intelliJHandle = FindWindow("SunAwtFrame", null); // You can also specify the window title if needed
            if (intelliJHandle != IntPtr.Zero)
            {
                SetWindowDisplayAffinity(intelliJHandle, 1);
            }
        }

        public void AllowIntelliJCapture()
        {
            IntPtr intelliJHandle = FindWindow("SunAwtFrame", null);
            if (intelliJHandle != IntPtr.Zero)
            {
                SetWindowDisplayAffinity(intelliJHandle, 0);
            }
        }
        public void ControlIntelliJWindow()
        {
            IntPtr intelliJHandle = FindWindow(windowName, null);
            if (intelliJHandle != IntPtr.Zero)
            {
                int style = GetWindowLong(intelliJHandle, GWL_STYLE);

                // Remove the system menu (which contains the close option)
                style &= ~WS_SYSMENU;

                // Remove the minimize and maximize box
                style &= ~WS_MINIMIZEBOX;
                style &= ~WS_MAXIMIZEBOX;

                SetWindowLong(intelliJHandle, GWL_STYLE, style);
            }
        }
       
        private void Docker_Shown(object sender, EventArgs e)
        {
            ControlIntelliJWindow();
        }




        private void DockAppToTop()
		{
			IntPtr hWndApp = FindWindow("SunAwtFrame", null);// For IntelliJ: SunAwtFrame
			if (hWndApp != IntPtr.Zero)
			{
				SetWindowPos(hWndApp, IntPtr.Zero, 0, 0, Screen.PrimaryScreen.WorkingArea.Width,
					Screen.PrimaryScreen.WorkingArea.Height - this.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
			}
			else
			{
				MessageBox.Show("IntelliJ not found. Please open IntelliJ before running this application.", "Error launching the Exam", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				 Environment.Exit(0);
			}
		}

		public void LockKeys()
		{
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
			{
				if (key != null)
				{
					key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
				}
			}
		}

		public void UnlockKeys()
		{
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
			{
				if (key != null)
				{
					key.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
				}
			}
		}

		private void OnFormMove(object sender, EventArgs e)
		{
			DockAppToTop();
		}

		private void OnFormResize(object sender, EventArgs e)
		{
			DockAppToTop();
		}
		private void KillExplorer()
		{
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
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error killing explorer.exe: " + ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}
		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			//UnblockWebsites();
			IntPtr hWndApp = FindWindow("SunAwtFrame", null);
			if (hWndApp != IntPtr.Zero)
			{
				SetWindowPos(hWndApp, IntPtr.Zero, 0, 0, Screen.PrimaryScreen.WorkingArea.Width,
					Screen.PrimaryScreen.WorkingArea.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
			}

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void button5_Click(object sender, EventArgs e)
		{
			selectedTask = 2;
			string pattern = @"Task 2:\s*(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern);

			if (match.Success)
			{
				Task2 = match.Groups[1].Value.Trim();
				richTextBox1.Text = Task2;
			}
		}

		private void button8_Click(object sender, EventArgs e)
		{
			switch(selectedTask) 
			{
				case 1:
					if (com1 == false)
					{
						this.completedImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
						com1 = true;
						GlobalCompleted.tasks.Add(1);
						break;
					}
					else
					{
                        this.completedImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com1 = false;
                        GlobalCompleted.tasks.Remove(1);
                        break;
                    }
                
                case 2:
					if (com2 == false)
					{
						this.completedImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
						com2 = true;
						GlobalCompleted.tasks.Add(2);
						break;
					}
					else
					{
                        this.completedImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com2 = false;
                        GlobalCompleted.tasks.Remove(2);
                        break;
                    }
                    
                case 3:
					if (com3 == false)
					{
						this.completedImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
						com3 = true;
						GlobalCompleted.tasks.Add(3);
						break;
					}
					else
					{
                        this.completedImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com3 = false;
                        GlobalCompleted.tasks.Remove(3);
                        break;
                    }
					
                case 4:
					if (com4 == false)
					{
						this.completedImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
						com4 = true;
						GlobalCompleted.tasks.Add(4);
						break;
					}
					else
					{
                        this.completedImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com4 = false;
                        GlobalCompleted.tasks.Remove(4);
                        break;
                    }
                    
            }

		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (!(GlobalFeedback.tasks.IsNullOrEmpty()) && FeedbackOn==false)
            {
				if (MessageBox.Show("Are you sure do you want to continue submitting the project?\nYou are going to give feedback for the questions you marked for feedback.\n\nKeep in mind that there is no way back to solve your tasks anymore if you press Yes!", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{	
							GiveFeedback gf = new GiveFeedback();
							FeedbackOn = true;
                            gf.Show();
                            AllowIntelliJCapture();
                            SetWindowDisplayAffinity(this.Handle, 0);
                            UnlockKeys();
							LockExit = false;
                    timer1.Stop();
                    Hide();
				}
			}

            if (!(GlobalReview.tasks.IsNullOrEmpty()) && FeedbackOn==false)
            {
                if (MessageBox.Show("Are you sure do you want to continue submitting the project?\nThere are tasks that are marked for review\n\nKeep in mind that there is no way back to solve your tasks anymore if you press Yes!", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ty gf = new ty();
					ReviewOn=true;
                    gf.Show();
                    AllowIntelliJCapture();
                    SetWindowDisplayAffinity(this.Handle, 0);
                    UnlockKeys();
                    LockExit = false;
                    timer1.Stop();
                    Hide();
                }
            }
            else if (!(GlobalReview.tasks.IsNullOrEmpty()) && FeedbackOn == true)
            {
                if (MessageBox.Show("Are you sure do you want to continue submitting the project?\nYou are going to give feedback for the questions you marked for feedback.\n\nKeep in mind that there is no way back to solve your tasks anymore if you press Yes!", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GiveFeedback gf = new GiveFeedback();
                    FeedbackOn = true;
                    gf.Show();
                    AllowIntelliJCapture();
                    SetWindowDisplayAffinity(this.Handle, 0);
                    UnlockKeys();
                    LockExit = false;
                    timer1.Stop();
                    Hide();
                }
            }
            else if (GlobalReview.tasks.IsNullOrEmpty() && (GlobalFeedback.tasks.IsNullOrEmpty()))
				{
                if (MessageBox.Show("Are you sure do you want to continue submitting the project?", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ty gf = new ty();
                    gf.Show();
                    AllowIntelliJCapture();
                    SetWindowDisplayAffinity(this.Handle, 0);
                    UnlockKeys();
                    LockExit = false;
                    timer1.Stop();
                    Hide();
                }
            }
        }

		private void label3_Click(object sender, EventArgs e)
		{

		}
		private void RestartExplorer()
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
		}
		private void button13_Click(object sender, EventArgs e)
		{
            selectedTask = 0;
            string pattern = @"Overview:(.*)";
            Match match = Regex.Match(File.ReadAllText(path), pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                Overview = match.Groups[1].Value.Trim();
                richTextBox1.Text = Overview;
            }
        }
		
		//private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
		
	//	private readonly List<string> BlockedWebsites = new List<string>
/*{
	"www.messenger.com",
	"www.instagram.com",
	"www.facebook.com",
	"web.whatsapp.com",
	"chat.openai.com",
	"www.telegram.com",
	"www.signal.com",
	"www.discord.com",
	"www.github.com",
	"www.reddit.com",
	"www.twitter.com",
	"www.github.com",
	"www.tinder.com",
	"www.viber.com",
	"web.snapchat.com",
	"www.youtube.com",
	"platform.openai.com",
	""
};*/
/*
		private void BlockWebsites()
		{
			try
			{
				var hostsContent = File.ReadAllText(HostsFilePath);

				foreach (var website in BlockedWebsites)
				{
					hostsContent += $"\n127.0.0.1 {website}";
				}

				File.WriteAllText(HostsFilePath, hostsContent);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error blocking websites: " + ex.Message);
			}
		}
		private void UnblockWebsites()
		{
			try
			{
				var hostsContent = File.ReadAllText(HostsFilePath);
				var lines = hostsContent.Split('\n');
				hostsContent = string.Join("\n", lines.Where(line => !BlockedWebsites.Any(website => line.Contains(website))));

				File.WriteAllText(HostsFilePath, hostsContent);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error unblocking websites: " + ex.Message);
			}
		}*/
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (timeLeft > TimeSpan.Zero)
			{
				timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
				label1.Text = timeLeft.ToString(@"hh\:mm\:ss");
				if(label1.Text=="00:05:00")
				{
					label1.BackColor = Color.Yellow;
					label1.ForeColor= Color.Black;
					MessageBox.Show("5 MINUTES LEFT OF YOUR EXAM","ATTENTION",MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
                if (label1.Text == "00:01:00")
                {
                    label1.BackColor = Color.Red;
                    label1.ForeColor = Color.White;
                    MessageBox.Show("1 MINUTE LEFT OF YOUR EXAM", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
			else
			{
				timer1.Stop();
				label1.Text = "00:00:00";
                timeExpired ps = new timeExpired();
                ps.Show();
                UnlockKeys();
                AllowIntelliJCapture();
                SetWindowDisplayAffinity(this.Handle, 0);
                LockExit = false;
                Hide();
			}
		}

		private void Docker_Load(object sender, EventArgs e)
		{
			
		}

        private void StartJsonUpdateAndSaveTask()
        {
            // Set up the timer to trigger every 10 seconds
            updateTimer = new System.Threading.Timer(
                e => UpdateJsonFileAndSendSave(),
                null,
                TimeSpan.Zero, // Start immediately
                TimeSpan.FromSeconds(10)); // Trigger every 10 seconds
        }

        // This is the method that the timer will call
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void UpdateJsonFileAndSendSave()
        {
            string jsonFilePath = @"C:\TaskWorker\ExamBackup.json";
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));

                dynamic jsonObj;
                if (File.Exists(jsonFilePath))
                {
                    string json = File.ReadAllText(jsonFilePath);
                    jsonObj = JsonConvert.DeserializeObject(json);
                }
                else
                {
                    jsonObj = new JObject();  // Create new JSON object
                }

                // Update with current date
                string[] timeParts = label1.Text.Split(':');
                int hours = int.Parse(timeParts[0]);
                int minutes = int.Parse(timeParts[1]);
                int seconds = int.Parse(timeParts[2]);

                // Converting the label's time to seconds
                int totalLabelSeconds = (hours * 3600) + (minutes * 60) + seconds;

				jsonObj.remainingTimeInSeconds = totalLabelSeconds;
                jsonObj.loggedInUser = GlobalUser.LoggedInUser.Email;
                jsonObj.completedTasks = new JArray(GlobalCompleted.tasks); 
                jsonObj.reviewTasks = new JArray(GlobalReview.tasks);      
                jsonObj.feedbackTasks = new JArray(GlobalFeedback.tasks);
				jsonObj.examPath = GlobalPath.IJPath;
                string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);
                IntPtr intelliJHandle = FindWindow("SunAwtFrame", null);
                if (intelliJHandle != IntPtr.Zero)
                {
                    // Ensure this runs on the background thread and doesn't block the UI
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        SetForegroundWindow(intelliJHandle);
                        SendKeys.SendWait("^s");
                    });
                }
            }
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }
        }


        /*
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private async void UpdateJsonWithDate()
        {
            string jsonFilePath = @"C:\TaskWorker\ExamBackup.json";

            while (true)
            {
                try
                {
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));

                    dynamic jsonObj;
                    if (File.Exists(jsonFilePath))
                    {
                        string json = File.ReadAllText(jsonFilePath);
                        jsonObj = JsonConvert.DeserializeObject(json);
                    }
                    else
                    {
                        jsonObj = new JObject();  // Create new JSON object
                    }

					// Update with current date
					jsonObj.startDate = examstartdate;
                    jsonObj.savedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    jsonObj.loggedInUser =GlobalUser.LoggedInUser.Email;
					jsonObj.completedTasks = GlobalCompleted.tasks;
                    jsonObj.reviewTasks = GlobalReview.tasks;
					jsonObj.feedbackTasks= GlobalFeedback.tasks;

                    IntPtr intelliJHandle = FindWindow("SunAwtFrame", null);
                    if (intelliJHandle != IntPtr.Zero)
                    {
                        // Set the focus to the IntelliJ window
                        SetForegroundWindow(intelliJHandle);

                        // Simulate CTRL + S for "Save All" in IntelliJ
                        SendKeys.SendWait("^s");
                    }
                    string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJson);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions during file reading/writing
                    MessageBox.Show("Error updating JSON: " + ex.Message);
                }

                await System.Threading.Tasks.Task.Delay(10000); // 10 second delay
            }
		
        }*/



        protected override void OnFormClosing(FormClosingEventArgs e)
		{
		
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you really sure you want to do this?\nThe timer will continue running\nTHIS OPERATION IS IRREVERSIBLE!", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{

				string DesktopFolder=Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				string javaexamfolder = Path.Combine(DesktopFolder, "JavaExam");
				string javatempfolder = Path.Combine(DesktopFolder, "JavaTemp");

				DeleteFolder(javaexamfolder+"\\out");
				DeleteFolder(javaexamfolder+"\\JavaExam\\src");
				CreateFolder(javaexamfolder+"\\JavaExam\\src");
				CopyFile(javatempfolder + "\\Main.java", javaexamfolder +"\\JavaExam\\src\\Main.java");
				com1 = false;
				com2 = false;
				com3 = false;
				com4 = false;
				mark1 = false;
				mark2 = false;
				mark3 = false;
				mark4 = false;
				feed1 = false;
				feed2 = false;
				feed3 = false;
				feed4 = false;
				button4.BackColor = Color.White;
				button5.BackColor = Color.White;
				button6.BackColor = Color.White;
				button7.BackColor = Color.White;
				Splash2 splash2 = new Splash2();
				splash2.Show();
				//this.Hide();
			}
		}
		private static void DeleteFolder(string folderPath)
		{
			try
			{
				// Check if the folder exists
				if (Directory.Exists(folderPath))
				{
					// Delete the folder and all its contents
					Directory.Delete(folderPath, true);
					Console.WriteLine($"Folder deleted: {folderPath}");
				}
				else
				{
					Console.WriteLine($"Folder not found: {folderPath}");
				}
			}
			catch (IOException ioEx)
			{
				Console.WriteLine($"Error deleting folder: {ioEx.Message}");
			}
			catch (UnauthorizedAccessException uaEx)
			{
				Console.WriteLine($"Access denied: {uaEx.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}
		private static void CopyDirectory(string sourcePath, string destinationPath)
		{
			// Check if the source folder exists
			if (!Directory.Exists(sourcePath))
			{
				Console.WriteLine($"Source folder not found: {sourcePath}");
				return;
			}

			// Create the destination folder if it doesn't exist
			if (!Directory.Exists(destinationPath))
			{
				Directory.CreateDirectory(destinationPath);
			}

			// Copy files in the source folder to the destination folder
			foreach (string filePath in Directory.GetFiles(sourcePath))
			{
				string fileName = Path.GetFileName(filePath);
				string destFilePath = Path.Combine(destinationPath, fileName);
				File.Copy(filePath, destFilePath, true);
			}

			// Recursively copy subfolders in the source folder to the destination folder
			foreach (string folderPath in Directory.GetDirectories(sourcePath))
			{
				string folderName = Path.GetFileName(folderPath);
				string destFolderPath = Path.Combine(destinationPath, folderName);
				CopyDirectory(folderPath, destFolderPath);
			}
		}
		private static void CopyFile(string sourceFilePath, string destinationFilePath)
		{
			try
			{
				File.Copy(sourceFilePath, destinationFilePath);
				Console.WriteLine("File copied successfully!");
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error copying file:");
				Console.WriteLine(ex.Message);
			}
		}
		private static void CreateFolder (string folderName)
		{
			try
			{
				Directory.CreateDirectory(folderName);
				Console.WriteLine("New folder created successfully!");
			}
			catch (IOException ex)
			{
				Console.WriteLine("Error creating new folder:");
				Console.WriteLine(ex.Message);
			}
		}
		private void button11_Click(object sender, EventArgs e)
		{
			string csvFileNamePattern = @"CSV file:\s*(.*)";
			Match fileNameMatch = Regex.Match(fileContent, csvFileNamePattern);
			string csvFileName = fileNameMatch.Groups[1].Value.Trim();
			CSV csv = new CSV();
			csv.Text= csvFileName;
			csv.Show();
		}

		private void pictureBox1_Click_1(object sender, EventArgs e)
		{

		}

		private void brandImage_Click(object sender, EventArgs e)
		{
		}
		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}
		public void CreateCsvFile(string inputText)
		{
			// Extract the CSV file name
			string csvFileNamePattern = @"CSV file:\s*(.*)";
			Match fileNameMatch = Regex.Match(inputText, csvFileNamePattern);
			string csvFileName = fileNameMatch.Groups[1].Value.Trim();

			// Extract the CSV content between <csv> and </csv>
			string csvContentPattern = @"<csv>(.*?)<\/csv>";
			Match contentMatch = Regex.Match(inputText, csvContentPattern, RegexOptions.Singleline);
			string csvContent = contentMatch.Groups[1].Value.Trim();

			// Save the CSV file to the user's Desktop in the "JavaExam\JavaExam" folder
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string outputDirectory = Path.Combine(desktopPath, "JavaExam", "JavaExam");
			
			string outputFilePath = Path.Combine(outputDirectory, csvFileName);	
			File.WriteAllText(outputFilePath, csvContent);
			FinalPath= outputFilePath;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			selectedTask = 0;
			string pattern = @"Overview:(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern, RegexOptions.Singleline);

			if (match.Success)
			{
				Overview = match.Groups[1].Value.Trim();
				richTextBox1.Text = Overview;
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			selectedTask = 1;
			string pattern = @"Task 1:\s*(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern);

			if (match.Success)
			{
				Task1 = match.Groups[1].Value.Trim();
				richTextBox1.Text = Task1;
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			selectedTask = 3;
			string pattern = @"Task 3:\s*(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern);

			if (match.Success)
			{
				Task3 = match.Groups[1].Value.Trim();
				richTextBox1.Text = Task3;
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			selectedTask = 4;
			string pattern = @"Task 4:\s*(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern);

			if (match.Success)
			{
				Task4 = match.Groups[1].Value.Trim();
				richTextBox1.Text = Task4;
			}
		}

		private void button10_Click(object sender, EventArgs e)
		{
            switch (selectedTask)
            {
                case 1:
					if (mark1 == false)
					{
						this.reviewImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
						mark1 = true;
						GlobalReview.tasks.Add(1);
                        break;
                    }
					else
					{
                        this.reviewImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                        mark1 = false;
                        GlobalReview.tasks.Remove(1);
                        break;
                    }
                case 2:
                    if (mark2 == false)
					{ 
                    this.reviewImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                    mark2 = true;
                    GlobalReview.tasks.Add(2);
					break;
                    }
					else
					{
                    this.reviewImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                    mark2 = false;
                    GlobalReview.tasks.Remove(2);
                    break;
                    }
                    
                case 3:
                    if (mark3 == false)
                    {
                    this.reviewImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                    mark3 = true;
                    GlobalReview.tasks.Add(3);
					break;
                    }
					else
					{
                    this.reviewImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                    mark3 = false;
                    GlobalReview.tasks.Remove(3);
                    break;
                    }
                    
                case 4:
					if (mark4 == false)
					{
						this.reviewImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
						mark4 = true;
						GlobalReview.tasks.Add(4);
						break;
					}
					else
					{
                        this.reviewImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                        mark4 = false;
                        GlobalReview.tasks.Remove(4);
                        break;
                    }
                    
            }
        }

		private void button9_Click(object sender, EventArgs e)
		{
            switch (selectedTask)
            {
                case 1:
					if (feed1 == false)
					{
						this.feedbackImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
						feed1 = true;
						GlobalFeedback.tasks.Add(1);
						break;
					}
					else
					{
                        this.feedbackImage1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed1 = false;
                        GlobalFeedback.tasks.Remove(1);
                        break;
                    }
                    
                case 2:
					if (feed2 == false)
					{
						this.feedbackImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
						feed2 = true;
						GlobalFeedback.tasks.Add(2);
						break;
					}
					else
					{
                        this.feedbackImage2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed2 = false;
						GlobalFeedback.tasks.Remove(2);
                        break;
                    }
                    
                case 3:
					if (feed3 == false)
					{
						this.feedbackImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
						feed3 = true;
						GlobalFeedback.tasks.Add(3);
						break;
					}
                    else
					{
                        this.feedbackImage3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed3 = false;
                        GlobalFeedback.tasks.Remove(3);
                        break;
                    }
                case 4:
					if (feed4 == false)
					{
						this.feedbackImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
						feed4 = true;
						GlobalFeedback.tasks.Add(4);
						break;
					}
					else
					{
                        this.feedbackImage4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed4 = false;
                        GlobalFeedback.tasks.Remove(4);
                        break;
                    }
                    
            }
        }

		private void Docker_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = LockExit;
			base.OnClosing(e);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void feedbackImage1_Click(object sender, EventArgs e)
        {

        }

        private void completedImage1_Click(object sender, EventArgs e)
        {

        }

        private void button8_MouseHover(object sender, EventArgs e)
        {
			//button8.BackColor = Color.PaleGreen;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
          //  button8.BackColor = Color.White;
        }

        private void button9_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void button9_MouseHover_1(object sender, EventArgs e)
        {
          //  button9.BackColor = Color.SkyBlue;
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
          //  button9.BackColor = Color.White;
        }

        private void button10_MouseHover(object sender, EventArgs e)
        {
          //  button10.BackColor = Color.LightCoral;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
           // button10.BackColor = Color.White;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void reviewImage4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // Toggle the state of countTimerPresses
            countTimerPresses = (countTimerPresses == 0) ? 1 : 0;

            // Update the label's ForeColor based on the new state of countTimerPresses
            if (countTimerPresses == 1)
            {
                label1.ForeColor = Color.FromArgb(0, 0, 64); // Dark blue
            }
            else
            {
                label1.ForeColor = Color.White; // White
            }
        }
    }

}