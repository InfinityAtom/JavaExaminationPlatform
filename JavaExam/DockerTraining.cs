using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class DockerTraining : Form
    {

        public string Overview = "";
        public string Task1 = "";
        public string Task2 = "";
        public string Task3 = "";
        public string Task4 = "";
        public string path = @"C:\TaskWorker\TaskCreator\tasks.txt";
        public string fileContent = File.ReadAllText(@"C:\TaskWorker\TaskCreator\tasks.txt");
        private const string registryPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
        public int selectedTask = 0;
        public bool com1 = false;
        public bool com2 = false;
        public bool com3 = false;
        public bool com4 = false;
        public bool mark1 = false;
        public bool mark2 = false;
        public bool mark3 = false;
        public bool mark4 = false;
        public bool feed1 = false;
        public bool feed2 = false;
        public bool feed3 = false;
        public bool feed4 = false;
        public int numberFeedback = 0;
        public int numberCompleted = 0;
        public int numberMarked = 0;
        public string FinalPath = "";
        public bool FeedbackOn = false;
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
        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;
        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_MAXIMIZEBOX = 0x10000;
        public DockerTraining()
        {
            InitializeComponent();

            this.reviewImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
            this.reviewImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
            this.completedImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
            this.feedbackImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");

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
            this.Shown += DockerTraining_Shown;
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


            if (GlobalSim.IsTrainingMode)
            {
                timeLeft = TimeSpan.Zero; // Start from 00:00:00
                button14.Enabled = true;
                button14.Visible = true;
            }
            else
            {
                // Your existing exam mode initialization
                timeLeft = TimeSpan.FromSeconds(2700);
                button14.Enabled = false;
                button14.Visible = false;
                KillExplorer();
                LockKeys();
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

        private void DockerTraining_Shown(object sender, EventArgs e)
        {
            ControlIntelliJWindow();
            timer1.Start();
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
            if (!GlobalSim.IsTrainingMode) // Only disable Task Manager if not in training mode
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                    }
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
            if (!GlobalSim.IsTrainingMode) // Only kill explorer if not in training mode
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
            switch (selectedTask)
            {
                case 1:
                    if (com1 == false)
                    {
                        this.completedImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                        com1 = true;
                        GlobalCompleted.tasks.Add(1);
                        break;
                    }
                    else
                    {
                        this.completedImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com1 = false;
                        GlobalCompleted.tasks.Remove(1);
                        break;
                    }

                case 2:
                    if (com2 == false)
                    {
                        this.completedImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                        com2 = true;
                        GlobalCompleted.tasks.Add(2);
                        break;
                    }
                    else
                    {
                        this.completedImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com2 = false;
                        GlobalCompleted.tasks.Remove(2);
                        break;
                    }

                case 3:
                    if (com3 == false)
                    {
                        this.completedImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                        com3 = true;
                        GlobalCompleted.tasks.Add(3);
                        break;
                    }
                    else
                    {
                        this.completedImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com3 = false;
                        GlobalCompleted.tasks.Remove(3);
                        break;
                    }

                case 4:
                    if (com4 == false)
                    {
                        this.completedImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-active");
                        com4 = true;
                        GlobalCompleted.tasks.Add(4);
                        break;
                    }
                    else
                    {
                        this.completedImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("check-inactive");
                        com4 = false;
                        GlobalCompleted.tasks.Remove(4);
                        break;
                    }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!(GlobalFeedback.tasks.IsNullOrEmpty()) && FeedbackOn == false)
            {
                if (MessageBox.Show("Are you sure do you want to continue submitting the project?\nYou are going to give feedback for the questions you marked for feedback.\n\nKeep in mind that there is no way back to solve your tasks anymore if you press Yes!", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GiveFeedback gf = new GiveFeedback();
                    FeedbackOn = true;
                    gf.Show();
                    SetWindowDisplayAffinity(this.Handle, 0);
                    UnlockKeys();
                    LockExit = false;
                    timer1.Stop();
                    Hide();
                }
            }

            if (!(GlobalReview.tasks.IsNullOrEmpty()) && FeedbackOn == false)
            {
                if (MessageBox.Show("Are you sure do you want to continue submitting the project?\nThere are tasks that are marked for review\n\nKeep in mind that there is no way back to solve your tasks anymore if you press Yes!", "IMPORTANT INFO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ty gf = new ty();
                    ReviewOn = true;
                    gf.Show();

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

                    SetWindowDisplayAffinity(this.Handle, 0);
                    UnlockKeys();
                    LockExit = false;
                    timer1.Stop();
                    Hide();
                }
            }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GlobalSim.IsTrainingMode)
            {
                // In training mode, count up from zero
                timeLeft = timeLeft.Add(TimeSpan.FromSeconds(1));
                label1.Text = timeLeft.ToString(@"hh\:mm\:ss");
            }
            else
            {
                // In exam mode, count down from 45 minutes (2700 seconds)
                if (timeLeft > TimeSpan.Zero)
                {
                    timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
                    label1.Text = timeLeft.ToString(@"hh\:mm\:ss");

                    // Warning notifications
                    if (label1.Text == "00:05:00")
                    {
                        label1.BackColor = Color.Yellow;
                        label1.ForeColor = Color.Black;
                        MessageBox.Show("5 MINUTES LEFT OF YOUR EXAM", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    SetWindowDisplayAffinity(this.Handle, 0);
                    LockExit = false;
                    Hide();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you really sure you want to do this?\nThe timer will continue running\nTHIS OPERATION IS IRREVERSIBLE!", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {

                string DesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string javaexamfolder = Path.Combine(DesktopFolder, "JavaExam");
                string javatempfolder = Path.Combine(DesktopFolder, "JavaTemp");

                DeleteFolder(javaexamfolder + "\\out");
                DeleteFolder(javaexamfolder + "\\JavaExam\\src");
                CreateFolder(javaexamfolder + "\\JavaExam\\src");
                CopyFile(javatempfolder + "\\Main.java", javaexamfolder + "\\JavaExam\\src\\Main.java");
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
                button12.BackColor = Color.White;
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
        private static void CreateFolder(string folderName)
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
            csv.Text = csvFileName;
            csv.Show();
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
            FinalPath = outputFilePath;
        }

        private void button12_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
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
                        this.reviewImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                        mark1 = true;
                        GlobalReview.tasks.Add(1);
                        break;
                    }
                    else
                    {
                        this.reviewImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                        mark1 = false;
                        GlobalReview.tasks.Remove(1);
                        break;
                    }
                case 2:
                    if (mark2 == false)
                    {
                        this.reviewImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                        mark2 = true;
                        GlobalReview.tasks.Add(2);
                        break;
                    }
                    else
                    {
                        this.reviewImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                        mark2 = false;
                        GlobalReview.tasks.Remove(2);
                        break;
                    }

                case 3:
                    if (mark3 == false)
                    {
                        this.reviewImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                        mark3 = true;
                        GlobalReview.tasks.Add(3);
                        break;
                    }
                    else
                    {
                        this.reviewImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
                        mark3 = false;
                        GlobalReview.tasks.Remove(3);
                        break;
                    }

                case 4:
                    if (mark4 == false)
                    {
                        this.reviewImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-active");
                        mark4 = true;
                        GlobalReview.tasks.Add(4);
                        break;
                    }
                    else
                    {
                        this.reviewImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("flag-inactive");
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
                        this.feedbackImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                        feed1 = true;
                        GlobalFeedback.tasks.Add(1);
                        break;
                    }
                    else
                    {
                        this.feedbackImage11.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed1 = false;
                        GlobalFeedback.tasks.Remove(1);
                        break;
                    }

                case 2:
                    if (feed2 == false)
                    {
                        this.feedbackImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                        feed2 = true;
                        GlobalFeedback.tasks.Add(2);
                        break;
                    }
                    else
                    {
                        this.feedbackImage22.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed2 = false;
                        GlobalFeedback.tasks.Remove(2);
                        break;
                    }

                case 3:
                    if (feed3 == false)
                    {
                        this.feedbackImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                        feed3 = true;
                        GlobalFeedback.tasks.Add(3);
                        break;
                    }
                    else
                    {
                        this.feedbackImage33.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed3 = false;
                        GlobalFeedback.tasks.Remove(3);
                        break;
                    }
                case 4:
                    if (feed4 == false)
                    {
                        this.feedbackImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-active");
                        feed4 = true;
                        GlobalFeedback.tasks.Add(4);
                        break;
                    }
                    else
                    {
                        this.feedbackImage44.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("bubble-inactive");
                        feed4 = false;
                        GlobalFeedback.tasks.Remove(4);
                        break;
                    }

            }
        }

        private void DockerTraining_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = LockExit;
            base.OnClosing(e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            countTimerPresses = (countTimerPresses == 0) ? 1 : 0;

            // Update the label's ForeColor based on the new state of countTimerPresses
            if (countTimerPresses == 1)
            {
                label1.ForeColor = Color.FromArgb(0, 64, 64); // Dark blue
            }
            else
            {
                label1.ForeColor = Color.White; // White
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ChatWindow chatWindow = new ChatWindow();
            chatWindow.Show();
        }

        private void DockerTraining_Load(object sender, EventArgs e)
        {

        }
    }
}
