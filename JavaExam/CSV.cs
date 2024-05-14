using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace JavaExam
{
	public partial class CSV : Form
	{
        public string csvFileName;
        public CSV()
        {
            InitializeComponent();

            // Extract CSV content
            string csvFileNamePattern = @"CSV file:\s*(.*)";
            Match fileNameMatch = Regex.Match(File.ReadAllText(@"C:\TaskWorker\TaskCreator\tasks.txt"), csvFileNamePattern);
            csvFileName = fileNameMatch.Groups[1].Value.Trim();
            label1.Text=csvFileName;

            string csvPath = @"C:\TaskWorker\TaskCreator\csvFile.csv";

            DataTable dt = new DataTable();
            string[] lines = File.ReadAllLines(csvPath);

            if (lines.Length > 0)
            {
                // Headers
                string firstLine = lines[0];
                string[] headerLabels = firstLine.Split(',');

                foreach (string header in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(header));
                }

                // Data rows
                for (int r = 1; r < lines.Length; r++)
                {
                    string[] items = lines[r].Split(',');
                    dt.Rows.Add(items);
                }
            }

            dataGridView1.DataSource = dt;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}

        private void CSV_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(@"JavaExam\"+ csvFileName);
            MessageBox.Show($"The path to the CSV has been copied successfully!({Clipboard.GetText()})\nUse it to reference the CSV file in your project!","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
