using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class SplashTraining : Form
    {
        private string exePath = @"C:\TaskWorker\TaskCreatorSim\dist\TaskCreator\TaskCreator.exe";

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        private const int SW_MAXIMIZE = 3;
        private bool isIntelliJMaximized = false; // Flag to indicate if IntelliJ is maximized

        private void MaximizeIntelliJEditorWindow()
        {
            IntPtr hWndIntelliJ = IntPtr.Zero;

            while (hWndIntelliJ == IntPtr.Zero)
            {
                hWndIntelliJ = FindWindow("SunAwtFrame", null);
                Thread.Sleep(500); // Wait for 0.5 seconds before checking again
            }

            ShowWindow(hWndIntelliJ, SW_MAXIMIZE);
            isIntelliJMaximized = true;
        }

        public SplashTraining()
        {
            InitializeComponent();
            this.Show(); // Show form
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("jexam");
            RunExeInBackground(); // Run the EXE in the background
            OpenIntelliJWithProject();
        }
        private void OpenIntelliJWithProject()
        {
            string projectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JavaExam");
            string textRead = GlobalPath.IJPath;


            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = textRead,
                Arguments = $"\"{projectPath}\"",
                WindowStyle = ProcessWindowStyle.Maximized
            };
            Process.Start(psi);
            Thread maximizeThread = new Thread(MaximizeIntelliJEditorWindow);
            maximizeThread.Start();
        }

        private void RunExeInBackground()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                RunExeProgram(exePath);
                this.Invoke((Action)delegate {
                    Hide(); // Close splash form
                    DockerTraining dockerForm = new DockerTraining();
                    dockerForm.Show();
                });
            });
        }

        private Task<int> RunExeProgram(string exePath)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = exePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    tcs.SetResult(process.ExitCode);
                    process.Dispose();
                };

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
            }

            return tcs.Task;
        }


    }
}
