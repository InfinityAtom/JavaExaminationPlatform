using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;

namespace JavaExam
{
    public partial class infinityatom : Form
    {
        private int currentPage = 0;
        string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "special.pdf");
        private PdfDocument pdfDocument;
        public infinityatom()
        {
            InitializeComponent();
            LoadPdf();
        }
        private void LoadPdf()
        {
            pdfDocument = PdfDocument.Load(pdfFilePath);
            DisplayPage();
        }
        private void DisplayPage()
        {
            if (pdfDocument == null) return;

            // Adjust the DPI value to improve the quality (e.g., 144, 300, etc.)
            int dpi = 450;

            int width = (int)(pictureBox1.Width * dpi / 72.0);
            int height = (int)(pictureBox1.Height * dpi / 72.0);

            using (var image = pdfDocument.Render(currentPage, width, height, dpi, dpi, true))
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = new Bitmap(image);
            }
        }
        private void infinityatom_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Tutorial tutorial= new Tutorial();
            tutorial.Show();
            Hide();
        }
    }
}
