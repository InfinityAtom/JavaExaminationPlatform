using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using PdfiumViewer;

namespace JavaExam
{
    public partial class Tutorial : Form
    {
        private int currentPage = 0;
        string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tutorial.pdf");
        private PdfDocument pdfDocument;
        public Tutorial()
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

            btnPrevious.Visible = currentPage > 0;
            btnNext.Visible = currentPage < pdfDocument.PageCount - 1;
            if (btnNext.Visible == false && btnPrevious.Visible == true)
            {
                button3.Visible = true;
            }
            else
            {
                button3.Visible = false;
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < pdfDocument.PageCount - 1)
            {
                currentPage++;
                DisplayPage();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                DisplayPage();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(GlobalSim.IsSimulation == true)
            {
                if(GlobalSim.IsTrainingMode== true) 
                {
                    SplashTraining splashtr = new SplashTraining();
                    splashtr.Show();
                    Hide();
                }
                else
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
                        string backgroundColor = "255 255 255"; // This would set it to white
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Colors", true);
                        if (key != null)
                        {
                            key.SetValue("Background", backgroundColor);
                            key.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error killing explorer.exe: " + ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                    SplashTraining splashtr = new SplashTraining();
                    splashtr.Show();
                    Hide();
                }
            }
            else
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
                    string backgroundColor = "255 255 255"; // This would set it to white
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Colors", true);
                    if (key != null)
                    {
                        key.SetValue("Background", backgroundColor);
                        key.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error killing explorer.exe: " + ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                Splash splash = new Splash();
                splash.Show();
                Hide();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Tutorial_Load(object sender, EventArgs e)
        {

        }
    }
}
