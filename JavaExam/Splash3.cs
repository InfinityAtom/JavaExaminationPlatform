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
    public partial class Splash3 : Form
    {

        private string exePath = @"C:\TaskWorker\TaskParser\dist\main\main.exe";

        public Splash3()
        {
            InitializeComponent();
            this.Show();
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("jexam");

            RunScriptInBackground();
        }
        private void RunScriptInBackground()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                RunExeProgram(exePath);
                this.Invoke((Action)delegate {
                    // Close splash form

                    Splash4 sp4 = new Splash4();
                    sp4.Show();
                    Hide();
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

        private void Splash3_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
