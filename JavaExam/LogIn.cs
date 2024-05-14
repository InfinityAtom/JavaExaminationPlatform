using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace JavaExam
{
    public partial class LogIn : Form
    {
        public string StudentLName;
        public string StudentFName;
        public string StudentFaculty;
        public string StudentSpec;
        public string StudentGroup;
        private const string registryPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
        public LogIn()
        {
            InitializeComponent();
            this.pictureBox6.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("Accessibility");
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            this.pictureBox4.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("email");
            this.pictureBox5.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("password");
            this.pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("login");
        }


        public string connectionString = @"Server=(localdb)\HotelMgmSystem;Database=db;Trusted_Connection=True;"
;


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
        static string TrimStringFromCharacter(string input, char character)
        {
            int indexOfCharacter = input.IndexOf(character);
            return indexOfCharacter >= 0 ? input.Substring(0, indexOfCharacter).Trim() : input;
        }
        private async void button1_Click_1Async(object sender, EventArgs e)
        {

            button1.Text = "Loading...";
            button1.Enabled = false;
            try 
            { 
              using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string query = "SELECT * FROM Studenti WHERE Email=@Email AND Password=@Password";  // Adjust table name if needed

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", textBox1.Text);
                    // Ideally, hash the password and then compare. For now, assuming plain text:
                    command.Parameters.AddWithValue("@Password", ComputeSha256Hash(textBox2.Text));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Studenti loggedInStudent = new Studenti
                            {
                                StudnetId = (int)reader["StudnetId"],
                                ProctorId = (int)reader["ProctorId"],
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Faculty = reader["Faculty"].ToString(),
                                Year = reader["Year"].ToString(),
                                Groupa = reader["Groupa"].ToString(),
                                Blind = (int)reader["Blind"],
                                Cheater = (int)reader["Cheater"],

                            };
                            GlobalUser.LoggedInUser = loggedInStudent;

                            if (loggedInStudent.Email == "tes23231231432t@test.com" || loggedInStudent.Email == "elena.ghimbasan@student.unitbv.ro")
                            {
                                GlobalUser.SpecialUser = true;
                            }
                            else
                            {
                                GlobalUser.SpecialUser = false;
                            }

                                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                                string filePath = Path.Combine(appDataPath, "users.txt");

                                string[] lines = {
                                    $"{loggedInStudent.LastName}",
                                    $"{loggedInStudent.FirstName}",
                                    $"{loggedInStudent.Faculty}",
                                    $"{loggedInStudent.SpecializationId}",
                                    $"{loggedInStudent.Groupa}"
                                };

                            File.WriteAllLines(filePath, lines);
                           

                                if (!(File.Exists(@"C:\TaskWorker\ExamBackup.json")))
                            {
                                FirstCheck fc = new FirstCheck();
                                fc.Show();
                                Hide();
                            }
                            else
                            {
                              dynamic jsonObj;
                              string json = File.ReadAllText(@"C:\TaskWorker\ExamBackup.json");
                              jsonObj = JsonConvert.DeserializeObject(json);
                                    string savedUserExam = jsonObj.loggedInUser;
                                    GlobalPath.IJPath = jsonObj.examPath;
                                    if(savedUserExam == loggedInStudent.Email)
                                    {
                                        ReloadExam re = new ReloadExam();
                                        re.Show();
                                        Hide();
                                    }
                                    else
                                    {
                                        FirstCheck fc = new FirstCheck();
                                        fc.Show();
                                        Hide();
                                        File.Delete(json);
                                    }
                            }
                            
                        }
                        else
                        {
                            
                            MessageBox.Show("Invalid login credentials.");
                            button1.Enabled = true;
                            button1.Text = "Log In";
                        }
                    }
                }
            }
            }
            catch(Exception ex) 
            { 
                MessageBox.Show($"{ex}","Error",MessageBoxButtons.OK, MessageBoxIcon.Stop);
                button1.Enabled = true;
                button1.Text = "Log In";
            }
          
        }


        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }




        private void LogIn_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void LogIn_FormClosed(object sender, FormClosedEventArgs e)
        {
             Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C START explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restarting explorer.exe: " + ex.Message);
            }
             Environment.Exit(0);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            AccesibilityForm fc = new AccesibilityForm();
            fc.Show();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
            {
                if (key != null)
                {
                    key.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
                }
            }
        }
    }
}

