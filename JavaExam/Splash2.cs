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
	public partial class Splash2 : Form
	{
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const int SW_MAXIMIZE = 3;
		private void MaximizeIntelliJEditorWindow()
		{
			IntPtr hWndIntelliJ = IntPtr.Zero;

			while (hWndIntelliJ == IntPtr.Zero)
			{
				hWndIntelliJ = FindWindow("SunAwtFrame", null);
				Thread.Sleep(500); // Wait for 0.5 seconds before checking again
			}

			ShowWindow(hWndIntelliJ, SW_MAXIMIZE);
		}
		private void ShowDockerFormAndCloseSplash()
		{
			Hide();
		}
		private void OpenIntelliJWithProject()
		{
			string projectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JavaExam");
			string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IJPath.txt");
			string textRead = GlobalPath.IJPath;
	

			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = textRead,
				Arguments = $"\"{projectPath}\"",
				WindowStyle = ProcessWindowStyle.Maximized
			}; // Update this path based on your IntelliJ installation

			Process.Start(psi);
			Thread maximizeThread = new Thread(MaximizeIntelliJEditorWindow);
			maximizeThread.Start();
		}
		public Splash2()
		{
			InitializeComponent();
			OpenIntelliJWithProject();
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("jexam");
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer
			{
				Interval = 5000, // 5 seconds
				Enabled = true
			};
			timer.Tick += (sender, e) =>
			{
				timer.Stop();
				ShowDockerFormAndCloseSplash();
			};

		}

		private void Splash2_Load(object sender, EventArgs e)
		{

		}

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}