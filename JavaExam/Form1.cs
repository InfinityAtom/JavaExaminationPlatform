using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO; // Added
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace JavaExam
{
    public partial class Form1 : Form
	{
        public string currentJsonDate = "NaN";
		public Form1()
		{
			InitializeComponent();
            System.Threading.Tasks.Task.Run(() => UpdateJsonWithDate());
        }
        private async void UpdateJsonWithDate()
        {
            string jsonFilePath = @"C:\TaskWorker\Debug.json";

            while (true)
            {
                try
                {
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));

                    dynamic jsonObj;
                    if (File.Exists(jsonFilePath))
                    {
                        string json = File.ReadAllText(jsonFilePath);
                        jsonObj = JsonConvert.DeserializeObject(json);
                    }
                    else
                    {
                        jsonObj = new JObject();  // Create new JSON object
                    }

                    // Update with current date
                    jsonObj.currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    jsonObj.loggedInUser = "test@test.com";
                    currentJsonDate = (string)jsonObj.currentDate;
                    // Save back to file
                    string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, updatedJson);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions during file reading/writing
                    MessageBox.Show("Error updating JSON: " + ex.Message);
                }

                await System.Threading.Tasks.Task.Delay(5000); // 5 second delay
            }
        }
        private void button1_Click(object sender, EventArgs e)
		{
            MessageBox.Show("Testing for errors");
        }

		private void Form1_Load(object sender, EventArgs e)
		{

		}
		private void button2_Click(object sender, EventArgs e)
		{
            MessageBox.Show($"Current time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\nJson Time: {currentJsonDate}") ;
        }
	}
}
