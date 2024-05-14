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
    public partial class Agreement : Form
    {
        public Agreement()
        {
            InitializeComponent();
            InitializeRichTextBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void InitializeRichTextBox()
        {
            try
            {
                // Path to your RTF file
                string filePath = @"C:\TaskWorker\agreement.rtf";

                // Reading the content of the file
                string rtfContent = File.ReadAllText(filePath);

                // Initializing the RichTextBox with the file content
                richTextBox1.Rtf = rtfContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Agreement_Load(object sender, EventArgs e)
        {

        }
    }
}
