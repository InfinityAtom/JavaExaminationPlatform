using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class timeExpired : Form
    {
        public timeExpired()
        {
            InitializeComponent();
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("TIME");
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // Find the IntelliJ window using its class name "SunAwtFrame"
            IntPtr intelliJHandle = FindWindow("SunAwtFrame", null);
            if (intelliJHandle != IntPtr.Zero)
            {
                // Set the focus to the IntelliJ window
                SetForegroundWindow(intelliJHandle);

                // Simulate CTRL + S for "Save All" in IntelliJ
                SendKeys.SendWait("^s");

                // Give a small delay for the save operation to complete
                System.Threading.Thread.Sleep(1000);

                // Simulate ALT + F4 to close IntelliJ
                SendKeys.SendWait("%{F4}");
            }


            Splash3 S3 = new Splash3();
            S3.Show();
            Hide();
        }
    }
}
