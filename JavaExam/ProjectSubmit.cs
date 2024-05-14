using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace JavaExam
{
	public partial class ProjectSubmit : Form
	{
		private List<string> ReadLinesFromFile(string filePath)
		{
			List<string> lines = new List<string>();

			using (StreamReader sr = new StreamReader(filePath))
			{
				while (!sr.EndOfStream)
				{
					lines.Add(sr.ReadLine());
				}
			}

			return lines;
		}
		private void UpdateLabels()
		{
				label9.Text =  GlobalUser.LoggedInUser.LastName;
				label10.Text = GlobalUser.LoggedInUser.FirstName;
                label11.Text = GlobalUser.LoggedInUser.Faculty;
                label13.Text = GlobalUser.LoggedInUser.Groupa; 	
		}
		private string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
        private string grade = "";
        int grade2 = 0;
        private double finalGrade = 0.00;
        private string task1S = "";
        private string task2S = "";
       private string task3S = "";
        private  string task4S = "";
        private string task1F = "";
        private string task2F = "";
        private string task3F = "";
        private string task4F = "";
        private string task1C = "";
        private string task2C = "";
        private string task3C = "";
        private string task4C = "";
        private string status = "";
        

        public ProjectSubmit()
		{
			InitializeComponent();
			UpdateLabels();
			TaskContents();
            Task1Status();
            Task2Status();
			Task3Status();
            Task4Status();
            Task1Feed();
            Task2Feed();
            Task3Feed();
            Task4Feed();
            
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
						label16.Text = finalGrade.ToString("0.00");
					}
					else
					{
                        label16.Text = finalGrade.ToString("0.00");
                    }
                    // Format the double to display with two decimal places
                    
                }
                else
                {
                    label16.Text = "Error: String could not be parsed to a double.";
                }
                /* grade = match.Groups[1].Value.Trim();
                 grade2 = Int32.Parse(grade);
                 finalGrade = grade2;
                 finalGradeLabel.Text = grade2.ToString(); */
            }
            grade2 = (int)finalGrade;
            if (grade2 >= 5)
            {
                status = "PROMOVAT";
            }
            else
            {
                status = "RESTANTIER";
            }
        }

		private void ProjectSubmit_Load(object sender, EventArgs e)
		{

		}

		private void SendEmailWithAttachments(string fromEmail, string toEmail, string subject, string body, string signature, List<string> attachmentFilePaths)
		{
			try
			{
				MailMessage mail = new MailMessage(fromEmail, toEmail);
				mail.Subject = subject;
				mail.Body = body + Environment.NewLine + Environment.NewLine + signature;

				foreach (string attachmentFilePath in attachmentFilePaths)
				{
					if (File.Exists(attachmentFilePath))
					{
						Attachment attachment = new Attachment(attachmentFilePath);
						mail.Attachments.Add(attachment);
					}
					else
					{
						Console.WriteLine("File not found: " + attachmentFilePath);
					}
				}

				var smtpClient = new SmtpClient("mail.infinity-atom.ro")
				{
					Port = 587,
					Credentials = new NetworkCredential("java.exam@infinity-atom.ro", "p@SSWORD20caractereabc1"),
					EnableSsl = true,
				};
				smtpClient.Send(mail);
				Console.WriteLine("Email sent successfully!");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error sending email: " + ex.Message);
			}
		}

		private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
		private readonly List<string> BlockedWebsites = new List<string>
{
	"www.messenger.com",
	"www.instagram.com",
	"www.facebook.com",
	"web.whatsapp.com",
	"chat.openai.com",
	"www.telegram.com",
	"www.signal.com",
	"www.discord.com",
	"www.github.com",
	"www.reddit.com",
	"www.twitter.com",
	"www.github.com",
	"www.tinder.com",
	"www.viber.com",
	"web.snapchat.com",
	"www.youtube.com"
};
		private void UnblockWebsites()
		{
			try
			{
				var hostsContent = File.ReadAllText(HostsFilePath);
				var lines = hostsContent.Split('\n');
				hostsContent = string.Join("\n", lines.Where(line => !BlockedWebsites.Any(website => line.Contains(website))));

				File.WriteAllText(HostsFilePath, hostsContent);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error unblocking websites: " + ex.Message);
			}
		}
        private void UpdateTasksInDatabase()
        {
            int studentId = GlobalUser.LoggedInUser.StudnetId; // Note the typo in `StudnetId`. It should be `StudentId`.
            GlobalBooking.FetchBookingByStudentId(studentId);
			int proctorId = GlobalProctor.LoggedInProctor.ProctorId;
            int bookingId = GlobalBooking.CurrentBooking.BookingId;

            using (var context = new JavaExamContext())
            {
				var newTask = new Task
				{
					BookingId = bookingId,
					ExamId = 1,
					ProctorId = proctorId,
					StudentId = studentId, // Assuming GlobalUser.LoggedInUser has the logged in student's details
					Task1Content = task1C,
					Task1State = task1S,
					Task1Explanation = task1F,
					Task2Content = task2C,
					Task2State = task2S,
					Task2Explanation = task2F,
					Task3Content = task3C,
					Task3State = task3S,
					Task3Explanation = task3F,
					Task4Content = task4C,
					Task4State = task4S,
					Task4Explanation = task4F,
					FinalGrade = Convert.ToInt32(finalGrade)
                };

                context.Tasks.Add(newTask);
                context.SaveChanges();
            }
        }
        private void RestartExplorer()
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
		}

		private void btnPrevious_Click(object sender, EventArgs e)
		{

			string sourceFilePath = @"C:\TaskWorker\TaskCreator\tasks.txt";
			string destinationFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\JavaExam\\" + "\\Cerinte.txt");

			// Copy the file from source to destination
			try
			{
				File.Copy(sourceFilePath, destinationFilePath, overwrite: false);
				Console.WriteLine("File copied successfully.");
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error copying file: {ex.Message}");
			}

			try
			{
				string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\JavaExam");
				string zipPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{label9.Text}_{label10.Text}.zip");

				// Create a new ZIP archive from the folder
				ZipFile.CreateFromDirectory(folderPath, zipPath);

				Console.WriteLine("ZIP archive created successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			List<string> attachmentFilePaths = new List<string>
			{
			$@"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{label9.Text}_{label10.Text}.zip")}"
			};

			string task1="";
			string task2="";
			string task3 = "";
			string task4 = "";
			string path = @"C:\TaskWorker\TaskCreator\tasks.txt";
			

			string pattern = @"Task 1:\s*(.*)";
			Match match = Regex.Match(File.ReadAllText(path), pattern);

			if (match.Success)
			{
				task1 = match.Groups[1].Value.Trim();
			}

			string pattern2 = @"Task 2:\s*(.*)";
			Match match2 = Regex.Match(File.ReadAllText(path), pattern2);

			if (match2.Success)
			{
				task2 = match2.Groups[1].Value.Trim();
			}

			string pattern3 = @"Task 3:\s*(.*)";
			Match match3 = Regex.Match(File.ReadAllText(path), pattern3);

			if (match3.Success)
			{
				task3 = match3.Groups[1].Value.Trim();
			}

			string pattern4 = @"Task 4:\s*(.*)";
			Match match4 = Regex.Match(File.ReadAllText(path), pattern4);

			if (match4.Success)
			{
				task4 = match4.Groups[1].Value.Trim();
			}

        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("dd.MM.yyyy");

        string body = $@"
EXAMEN PROGRAMARE IN JAVA DIN DATA: {formattedDate}

Nume Student: {label9.Text}
Prenume Student: {label10.Text}
Facultatea: {label11.Text}
Specializarea: x
Grupa: {label13.Text}

Nota obținută: {label16.Text}
PROMOVAT/RESPINS: {status}

=====Cerințele din examen====

Task 1: {task1}

Task 2: {task2}

Task 3: {task3}

Task 4: {task4}

În arhiva de mai sus găsiți și fișierul CSV care se află în interiorul folderului numit JavaExam

=====Performanța studentului evaluată de program====

Task 1: {task1S}
Task 1 Explanation: {task1F}

Task 2: {task2S}
Task 2 Explanation: {task2F}

Task 3: {task3S}
Task 3 Explanation: {task3F}

Task 4: {task4S}
Task 4 Explanation: {task4F}

Cele bune,";

string signature = @"
Proiectul JavaExam
Infinity Atom
Velicea Fabian Pavel
Student UniTBv- IESC- TSTC- An.III- Gr. 4LF612";


		SendEmailWithAttachments("java.exam@infinity-atom.ro", GlobalProctor.LoggedInProctor.Email.ToString(), $"Examenul de Java al lui {label9.Text} {label10.Text}", body, signature, attachmentFilePaths);
			UnblockWebsites();
			RestartExplorer();
			File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\users.txt");
            UpdateTasksInDatabase();
			File.Delete(@"C:\TaskWorker\TaskCreator\csvFile.csv");
             Environment.Exit(0);
		}
		private void Task1Status()
		{
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 1:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task1S= match1.Groups[1].Value.Trim();
            }
        }
        private void Task2Status()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 2:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task2S = match1.Groups[1].Value.Trim();
            }
        }
        private void Task3Status()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 3:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task3S = match1.Groups[1].Value.Trim();
            }
        }
        private void Task4Status()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 4:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task4S = match1.Groups[1].Value.Trim();
            }
        }
        private void Task1Feed()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 1 Explanation:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task1F = match1.Groups[1].Value.Trim();
            }
        }
        private void Task2Feed()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 2 Explanation:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task2F = match1.Groups[1].Value.Trim();
            }
        }
        private void Task3Feed()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 3 Explanation:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task3F = match1.Groups[1].Value.Trim();
            }
        }
        private void Task4Feed()
        {
            string path = @"C:\TaskWorker\TaskEvaluator\result.txt";
            string pattern1 = @"Task 4 Explanation:\s*(.*)";
            Match match1 = Regex.Match(File.ReadAllText(path), pattern1);

            if (match1.Success)
            {
                task4F = match1.Groups[1].Value.Trim();
            }
        }
        
        private void TaskContents()
        {
            string task1 = "";
            string task2 = "";
            string task3 = "";
            string task4 = "";
            string path = @"C:\TaskWorker\TaskCreator\tasks.txt";


            string pattern = @"Task 1:\s*(.*)";
            Match match = Regex.Match(File.ReadAllText(path), pattern);

            if (match.Success)
            {
                task1 = match.Groups[1].Value.Trim();
            }

            string pattern2 = @"Task 2:\s*(.*)";
            Match match2 = Regex.Match(File.ReadAllText(path), pattern2);

            if (match2.Success)
            {
                task2 = match2.Groups[1].Value.Trim();
            }

            string pattern3 = @"Task 3:\s*(.*)";
            Match match3 = Regex.Match(File.ReadAllText(path), pattern3);

            if (match3.Success)
            {
                task3 = match3.Groups[1].Value.Trim();
            }

            string pattern4 = @"Task 4:\s*(.*)";
            Match match4 = Regex.Match(File.ReadAllText(path), pattern4);

            if (match4.Success)
            {
                task4 = match4.Groups[1].Value.Trim();
            }
			task1C = task1;
			task2C = task2;
			task3C = task3;
			task4C= task4;
        }
       
        

        private void Status()
		{

		}
        private void label10_Click(object sender, EventArgs e)
		{

		}
		
		private void panel3_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label14_Click(object sender, EventArgs e)
		{

		}

		private void panel2_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void label13_Click(object sender, EventArgs e)
		{

		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void label12_Click(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{

		}

		private void label11_Click(object sender, EventArgs e)
		{

		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		private void label9_Click(object sender, EventArgs e)
		{

		}

		private void label8_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
