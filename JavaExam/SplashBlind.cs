using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class SplashBlind : Form
    {
        private string exePath = @"C:\TaskWorker\TaskCreatorBlind\dist\TaskCreator\TaskCreator.exe";
        private string pathAudio = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Pep\Accessibility\Voices";
        public SplashBlind()
        {
            InitializeComponent();
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("jexam");
            pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("Accessibility");
            pictureBox2.Controls.Add(pictureBox3);
            pictureBox3.Location=new Point(0,0);
            pictureBox3.BackColor = Color.Transparent;

            RunExeInBackground();
        }
        private void RunExeInBackground()
        {
            
            System.Threading.Tasks.Task.Run(() =>
            {
                
                RunExeProgram(exePath);
                this.Invoke((Action)delegate {
                    Hide(); // Close splash form
                    ExamBlindForm examBlindForm = new ExamBlindForm();
                    examBlindForm.Show();
                    Hide();
                });
            });
        }

        private void PlaySound(string filePath)
        {
            using (SoundPlayer player = new SoundPlayer(filePath))
            {
                player.Play(); // Plays the sound synchronously
            }
        }

        private Task<int> RunExeProgram(string exePath)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            PlaySound(pathAudio + @"\pleaseWait.wav");
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
