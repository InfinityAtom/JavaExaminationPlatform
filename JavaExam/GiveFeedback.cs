using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class GiveFeedback : Form
    {
        private int currentTaskIndex;
        private List<int> tasksToFeedback;
        private string[] feedbacks;

        public GiveFeedback()
        {
            InitializeComponent();
            LoadTaskStates();
            currentTaskIndex = -1;
            feedbacks = new string[tasksToFeedback.Count];
            DisplayNextTask();
        }

        private void LoadTaskStates()
        {
            tasksToFeedback = GlobalFeedback.tasks.ToList();

            if (tasksToFeedback.Count == 0)
            {
                MessageBox.Show("No tasks available for feedback.");
            }
        }

        private void DisplayNextTask()
        {
            while (++currentTaskIndex < tasksToFeedback.Count)
            {
                int taskNumber = tasksToFeedback[currentTaskIndex];
                lblSpec.Text = $"Task {taskNumber} content";

                string pattern = $@"Task {taskNumber}:\s*(.*)";
                string path = @"C:\TaskWorker\TaskCreator\tasks.txt";

                Match match = Regex.Match(File.ReadAllText(path), pattern);

                if (match.Success)
                {
                    taskContent.Text = match.Groups[1].Value.Trim();
                }

                return;
            }

            SaveFeedbacks();
            MoveToNextForm();
        }

        private void SaveFeedbacks()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string javaExamFolder = Path.Combine(desktopPath, "JavaExam");

            if (!Directory.Exists(javaExamFolder))
            {
                Directory.CreateDirectory(javaExamFolder);
            }

            for (int i = 0; i < feedbacks.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(feedbacks[i]))
                {
                    File.WriteAllText(Path.Combine(javaExamFolder, $"feedback{tasksToFeedback[i]}.txt"), feedbacks[i]);
                }
            }
        }

        private void MoveToNextForm()
        {
            ty thankYou = new ty();
            thankYou.Show();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentTaskIndex >= 0 && currentTaskIndex < tasksToFeedback.Count)
            {
                feedbacks[currentTaskIndex] = richTextBox1.Text;
                richTextBox1.Clear();
            }

            DisplayNextTask();
        }

        private void taskContent_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
