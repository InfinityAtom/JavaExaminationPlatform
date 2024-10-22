using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Diagnostics;

namespace JavaExam
{
    public partial class LogIn : Form
    {
        private WebView2 webView;
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
            this.pictureBox7.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("password");
            this.pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("login");
        }

        public string connectionString = @"Server=tcp:pep-web.database.windows.net,1433;Initial Catalog=db;Persist Security Info=False;User ID=sysadmin;Password=p@SSWORD20caractereabc1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=180;";

        private void HandleJavaScriptMessage(string email)
        {
            // Check if the email exists in the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Studenti WHERE Email=@Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Retrieve and handle the user details as described in your existing code
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
                                Passkey = (int)reader["Passkey"]
                            };

                            GlobalUser.LoggedInUser = loggedInStudent;

                            // Show account details in a message box
                            string accountDetails = $"Login successful!\n\n" +
                                                    $"Name: {loggedInStudent.FirstName} {loggedInStudent.LastName}\n" +
                                                    $"Email: {loggedInStudent.Email}\n" +
                                                    $"Faculty: {loggedInStudent.Faculty}\n" +
                                                    $"Year: {loggedInStudent.Year}\n" +
                                                    $"Group: {loggedInStudent.Groupa}";

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

                            HandleSuccessfulLogin(loggedInStudent);
                        }
                        else
                        {
                            MessageBox.Show("Invalid login credentials.");
                        }
                    }
                }
            }
        }

        private void HandleSuccessfulLogin(Studenti loggedInStudent)
        {
            // Implement your logic for what happens after a successful login
            // For example, navigating to another form, saving user details, etc.
            // Example:
            // MainForm mainForm = new MainForm();
            // mainForm.Show();
            // this.Hide();

            if (loggedInStudent.Email == "tes23231231432t@test.com" || loggedInStudent.Email == "elena.ghimbasan@student.unitbv.ro")
            {
                GlobalUser.SpecialUser = true;
            }
            else
            {
                GlobalUser.SpecialUser = false;
            }

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
                if (savedUserExam == loggedInStudent.Email)
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


        private async void button1_Click_1Async(object sender, EventArgs e)
        {
            button1.Text = "Loading...";
            button1.Enabled = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Studenti WHERE Email=@Email AND Password=@Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", textBox1.Text);
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
                                    Passkey = (int)reader["Passkey"]
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
                                    if (savedUserExam == loggedInStudent.Email)
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
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                button1.Enabled = true;
                button1.Text = "Log In";
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
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

        private void LogIn_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
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

        private void button4_Click(object sender, EventArgs e)
        {
            using (PasskeyLoginForm passkeyLoginForm = new PasskeyLoginForm())
            {
                if (passkeyLoginForm.ShowDialog() == DialogResult.OK)
                {
                    string email = passkeyLoginForm.LoggedInEmail;

                    // Now handle the email, e.g., check it against the database
                    HandleJavaScriptMessage(email);
                }
                else
                {
                    MessageBox.Show("Passkey login failed or was cancelled.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            SelectSimulation selectSimulation= new SelectSimulation();
            selectSimulation.Show();
            Hide();
        }
    }
}
