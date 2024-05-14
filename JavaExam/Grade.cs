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

namespace JavaExam
{
    public partial class Grade : Form
    {
        private string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
        private string grade = "";
        int grade2 = 0;
        private double finalGrade = 0.00;
        private string task1Check = "";
        private string task2Check = "";
        private string task3Check = "";
        private string task4Check = "";
        public Grade()
        {
            InitializeComponent();
            string pattern = @"Grade:\s*(.*)";
            Match match = Regex.Match(File.ReadAllText(path), pattern);

            if (match.Success)
            {
                grade = match.Groups[1].Value.Trim(); // example string value

                if (Double.TryParse(grade, out finalGrade))
                {
                    if(GlobalUser.SpecialUser==true)
                    {
                        finalGrade = finalGrade + 1.00;
                    }
                    // Format the double to display with two decimal places
                    finalGradeLabel.Text = finalGrade.ToString("0.00");
                }
                else
                {
                    finalGradeLabel.Text = "Error: String could not be parsed to a double.";
                }
                /* grade = match.Groups[1].Value.Trim();
                 grade2 = Int32.Parse(grade);
                 finalGrade = grade2;
                 finalGradeLabel.Text = grade2.ToString(); */
            }
            grade2 = (int)finalGrade;
            if(grade2>=5)
            {
                statusLabel.Text = "SUCCEEDED";
                this.statusImage.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
            }
            else
            {
                statusLabel.Text = "FAILED";
                this.statusImage.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
            }
            string pattern1 = @"Task 1:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task1Check = match1.Groups[1].Value.Trim();
            }
            string pattern2 = @"Task 2:\s*(.*)";
            Match match2 = Regex.Match(File.ReadAllText(path), pattern2);

            if (match2.Success)
            {
                task2Check = match2.Groups[1].Value.Trim();
            }
            string pattern3 = @"Task 3:\s*(.*)";
            Match match3 = Regex.Match(File.ReadAllText(path), pattern3);

            if (match3.Success)
            {
                task3Check = match3.Groups[1].Value.Trim();
            }
            string pattern4 = @"Task 4:\s*(.*)";
            Match match4 = Regex.Match(File.ReadAllText(path), pattern4);

            if (match4.Success)
            {
                task4Check = match4.Groups[1].Value.Trim();
            }
            //========================================================
            if(task1Check== "Correctly Solved")
            {
                this.task1Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
                string pattern11 = @"Task 1 Explanation:\s*(.*)";
                Match match11 = Regex.Match(File.ReadAllText(path), pattern11);

                if (match11.Success)
                {
                    task1ExplanationLabel.Text = match11.Groups[1].Value.Trim();
                }
            }

            if (task1Check == "Incorrectly Solved")
            {
                this.task1Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
                string pattern10 = @"Task 1 Explanation:\s*(.*)";
                Match match10 = Regex.Match(File.ReadAllText(path), pattern10);

                if (match10.Success)
                {
                    task1ExplanationLabel.Text = match10.Groups[1].Value.Trim();
                }
            }
            //===================================================
            if (task2Check == "Correctly Solved")
            {
                this.task2Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
                string pattern21 = @"Task 2 Explanation:\s*(.*)";
                Match match21 = Regex.Match(File.ReadAllText(path), pattern21);

                if (match21.Success)
                {
                    task2ExplanationLabel.Text = match21.Groups[1].Value.Trim();
                }
            }

            if (task2Check == "Incorrectly Solved")
            {
                this.task2Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
                string pattern20 = @"Task 2 Explanation:\s*(.*)";
                Match match20 = Regex.Match(File.ReadAllText(path), pattern20);

                if (match20.Success)
                {
                    task2ExplanationLabel.Text = match20.Groups[1].Value.Trim();
                }
            }
            //====================================================
            if (task3Check == "Correctly Solved")
            {
                this.task3Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
                string pattern31 = @"Task 3 Explanation:\s*(.*)";
                Match match31 = Regex.Match(File.ReadAllText(path), pattern31);

                if (match31.Success)
                {
                    task3ExplanationLabel.Text = match31.Groups[1].Value.Trim();
                }
            }

            if (task3Check == "Incorrectly Solved")
            {
                this.task3Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
                string pattern30 = @"Task 3 Explanation:\s*(.*)";
                Match match30 = Regex.Match(File.ReadAllText(path), pattern30);

                if (match30.Success)
                {
                    task3ExplanationLabel.Text = match30.Groups[1].Value.Trim();
                }
            }
            //========================================================
            if (task4Check == "Correctly Solved")
            {
                this.task4Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("ok");
                string pattern41 = @"Task 4 Explanation:\s*(.*)";
                Match match41 = Regex.Match(File.ReadAllText(path), pattern41);

                if (match41.Success)
                {
                    task4ExplanationLabel.Text = match41.Groups[1].Value.Trim();
                }
            }

            if (task4Check == "Incorrectly Solved")
            {
                this.task4Image.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("no");
                string pattern40 = @"Task 4 Explanation:\s*(.*)";
                Match match40 = Regex.Match(File.ReadAllText(path), pattern40);

                if (match40.Success)
                {
                    task4ExplanationLabel.Text = match40.Groups[1].Value.Trim();
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            ProjectSubmit ps = new ProjectSubmit();
            ps.Show();
            Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void statusImage_Click(object sender, EventArgs e)
        {

        }

        private void statusLabel_Click(object sender, EventArgs e)
        {

        }

        private void task2ExplanationLabel_Click(object sender, EventArgs e)
        {

        }

        private void task1ExplanationLabel_Click(object sender, EventArgs e)
        {

        }

        private void task3ExplanationLabel_Click(object sender, EventArgs e)
        {

        }

        private void task4ExplanationLabel_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void task4Image_Click(object sender, EventArgs e)
        {

        }

        private void task3Image_Click(object sender, EventArgs e)
        {

        }

        private void task2Image_Click(object sender, EventArgs e)
        {

        }

        private void task1Image_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Grade_Load(object sender, EventArgs e)
        {

        }
    }
}
