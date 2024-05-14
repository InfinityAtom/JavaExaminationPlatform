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
    public partial class AccesibilityForm : Form
    {
        public AccesibilityForm()
        {
            InitializeComponent();
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("Blind");
        }

        private void AccesibilityForm_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScanAZTECCode aZTECCode= new ScanAZTECCode();
            aZTECCode.Show();
            Hide();
        }
    }
}
