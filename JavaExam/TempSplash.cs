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
	public partial class TempSplash : Form
	{
		public TempSplash()
		{
			InitializeComponent();
			this.pictureBox2.Image =(System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
		}

		private void TempSplash_Load(object sender, EventArgs e)
		{
			Thread.Sleep(2000);
			Splash splash = new Splash();
			splash.Show();
			Hide();
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{

		}
	}
}
