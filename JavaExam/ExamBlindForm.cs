using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Speech.Recognition;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;
using Microsoft.EntityFrameworkCore;
using System.Media;

namespace JavaExam
{
    public partial class ExamBlindForm : Form
    {
        private SpeechRecognitionEngine recognizer;
        private List<Question> questions;
        private int currentQuestionIndex = 0;
        private int timeLeft = 3000;
        private SpeechSynthesizer azureSynthesizer;
        private SpeechSynthesizer azureSynthesizer2;
        private SpeechConfig speechConfig;
        private SpeechConfig speechConfig2;
        private int skippedQuestionsCount;
        private bool timeExpired;
        private string pathAudio = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Pep\Accessibility\Voices";
        public ExamBlindForm()
        {
            InitializeComponent();
            DebuggingLogin(); //Comment this line after tests are done
            lblTimer.Text = "00:50:00";
            this.Load += new EventHandler(ExamBlindForm_Load);
        }

        private void ExamBlindForm_Load(object sender, EventArgs e)
        {
            LoadQuestions(@"C:\TaskWorker\TaskCreatorBlind\tasks.json");
            InitializeAzureTextToSpeech();
            InitializeSpeechRecognition();
            StartExamTimer();
            if (questions != null && questions.Count > 0)
                ReadQuestion();
        }

        private void LoadQuestions(string filePath)
        {
            string jsonText = File.ReadAllText(filePath);
            questions = JsonConvert.DeserializeObject<List<Question>>(jsonText);
        }

        private void InitializeAzureTextToSpeech()
        {
            speechConfig = SpeechConfig.FromSubscription("91075d902f3a470da0482d085c6b5e31", "francecentral");
            speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";
            azureSynthesizer = new SpeechSynthesizer(speechConfig);

            speechConfig2 = SpeechConfig.FromSubscription("91075d902f3a470da0482d085c6b5e31", "francecentral");
            speechConfig2.SpeechSynthesisVoiceName = "en-US-SteffanNeural";
            azureSynthesizer2 = new SpeechSynthesizer(speechConfig2);
        }
        public async void PlaySoundsSequentially()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
               // PlaySound(pathAudio + @"\InfinityAtomJingle.wav");

            }); 
        }


        private void PlaySound(string filePath)
        {
            using (SoundPlayer player = new SoundPlayer(filePath))
            {
                player.PlaySync(); // Plays the sound synchronously
            }
        }
        private void InitializeSpeechRecognition()
        {
            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();
            Choices commands = new Choices(new string[] { "A", "Alpha", "B", "Bravo", "C", "Charlie", "D", "Delta", "Skip", "Repeat", "Again", "Yes", "No" });
            System.Speech.Recognition.Grammar grammar = new System.Speech.Recognition.Grammar(new GrammarBuilder(commands));
            recognizer.LoadGrammar(grammar);
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            //recognizer.RecognizeAsync(RecognizeMode.Single);
        }

        private void StartExamTimer()
        {

            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                int hours = timeLeft / 3600;
                int minutes = (timeLeft % 3600) / 60;
                int seconds = timeLeft % 60;
                lblTimer.Text = $"{hours:00}:{minutes:00}:{seconds:00}";
                CheckTimeWarnings(minutes, seconds);
            }
            else
            {
                timer1.Stop();
                lblTimer.Text = "00:00:00";
                EndExam();
            }
        }

        private async void CheckTimeWarnings(int minutes, int seconds)
        {
            if (minutes == 10 && seconds == 0) await WarnTime("10 minutes left");
            else if (minutes == 5 && seconds == 0) await WarnTime("5 minutes left");
            else if (minutes == 1 && seconds == 0) await WarnTime("1 minute left");
            else if (minutes == 0 && seconds == 30) await WarnTime("30 seconds");
            else if (minutes == 0 && seconds == 15) await WarnTime("15 seconds");
            else if (minutes == 0 && seconds == 10) await WarnTime("10 seconds");
            else if (minutes == 0 && seconds == 5) await WarnTime("5 seconds");
            else if (minutes == 0 && seconds == 3) await WarnTime("3 seconds");
            else if (minutes == 0 && seconds == 1) await WarnTime("1 second");
        }

        private async System.Threading.Tasks.Task WarnTime(string message)
        {

            azureSynthesizer2.SpeakTextAsync(message).ConfigureAwait(false);

        }

        private async void ReadQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                recognizer.RecognizeAsyncCancel();
                int minutes = timeLeft / 60;
                int seconds = timeLeft % 60;
                string secondsText = seconds == 1 ? "second" : "seconds";
                var question = questions[currentQuestionIndex];
                lblQuestionNo.Text = question.questionNo.ToString().PadLeft(2, '0');
                lblQuestion.Text = question.question ?? "No question text available";
                lblA.Text = question.A ?? "?";
                lblB.Text = question.B ?? "?";
                lblC.Text = question.C ?? "?";
                lblD.Text = question.D ?? "?";
                if (skippedQuestionsCount > 0)
                {
                    if (skippedQuestionsCount == 1)
                    {
                        await azureSynthesizer2.SpeakTextAsync($"You have {skippedQuestionsCount.ToString()} skipped question! ").ConfigureAwait(true);
                    }
                    else
                    {
                        await azureSynthesizer2.SpeakTextAsync($"You have {skippedQuestionsCount.ToString()} skipped questions! ").ConfigureAwait(true);
                    }

                }
                if (questions.Count > 20)
                {
                    for (int i = 20; i <= questions.Count; i++)
                    {

                    }
                }
                await azureSynthesizer2.SpeakTextAsync($"This is question {question.questionNo.ToString()}").ConfigureAwait(true);
                if (minutes == 0)
                {
                    await azureSynthesizer2.SpeakTextAsync($"You have {seconds} {secondsText} left").ConfigureAwait(true);
                }
                if (minutes > 0)
                {
                    await azureSynthesizer2.SpeakTextAsync($"You have {minutes} minutes and {seconds} {secondsText} left").ConfigureAwait(true);
                }
                await azureSynthesizer.SpeakTextAsync(question.question ?? "No question text available").ConfigureAwait(true);
                await azureSynthesizer.SpeakTextAsync($"A: {lblA.Text}").ConfigureAwait(true);
                await azureSynthesizer.SpeakTextAsync($"B: {lblB.Text}").ConfigureAwait(true);
                await azureSynthesizer.SpeakTextAsync($"C: {lblC.Text}").ConfigureAwait(true);
                await azureSynthesizer.SpeakTextAsync($"D: {lblD.Text}").ConfigureAwait(true);
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }
        private bool CheckHashValid(string hash)
        {
            using (var dbContext = new JavaExamContext()) // Replace with your actual DbContext
            {
                return dbContext.LoginCodesBlinds.Any(lcb => lcb.LoginCode == hash);
            }
        }
        private void LoginStudent(string loginCode)
        {
            using (var dbContext = new JavaExamContext()) // Assuming you have a DbContext for your database
            {
                var studentLogin = dbContext.LoginCodesBlinds.Include(l => l.Student)
                                        .FirstOrDefault(l => l.LoginCode == loginCode);
                if (studentLogin != null)
                {
                    GlobalUser.LoggedInUser = studentLogin.Student;
                }
            }
        }
        private void DebuggingLogin()
        {
            LoginStudent("dcea1266cae62831d85101b0099fc04cfb02aba95808f8c33142236ef590cc76");
            try
            {
                int studentId = GlobalUser.LoggedInUser.StudnetId; // Note the typo in `StudnetId`. It should be `StudentId`.
                GlobalBooking.FetchBookingByStudentId(studentId);
                int bookingId = GlobalBooking.CurrentBooking.BookingId;
                string date = GlobalBooking.CurrentBooking.BookingDate.ToString();
                bool isBookingToday = (GlobalBooking.CurrentBooking.BookingDate >= DateTime.Today) && (GlobalBooking.CurrentBooking.BookingDate < DateTime.Today.AddDays(1));

                if (bookingId != null)
                {
                    if (isBookingToday == false)
                    {
                        if (MessageBox.Show("There is no booking asssociated to the login student, therefore the student could not be logged in now!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK) ;
                        {
                            Environment.Exit(0);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Student successfully logged in!", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }

            }
            catch (System.NullReferenceException)
            {
                if (MessageBox.Show("There is no booking asssociated to the login student, therefore the student could not be logged in now!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK) ;
                {
                    Environment.Exit(0);
                }
            }

        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            switch (recognizedText)
            {
                case "A":
                case "Alpha":
                case "B":
                case "Bravo":
                case "C":
                case "Charlie":
                case "D":
                case "Delta":
                    ConfirmAnswer(recognizedText);
                    break;
                case "Skip":
                    SkipQuestion();
                    break;
                case "Yes":
                    try
                    {
                        SaveAnswer();
                    }
                    catch(System.Exception ex) 
                    {
                        azureSynthesizer2.SpeakTextAsync("Select an answer, in order to submit, or skip the question by saying 'SKIP");
                    }
                    break;
                case "No":
                    {
                        lblA.BackColor = Color.White;
                        label7.ForeColor = Color.FromArgb(0, 0, 64);
                        lblB.BackColor = Color.White;
                        label8.ForeColor = Color.FromArgb(0, 0, 64);
                        lblC.BackColor = Color.White;
                        label9.ForeColor = Color.FromArgb(0, 0, 64);
                        lblD.BackColor = Color.White;
                        label10.ForeColor = Color.FromArgb(0, 0, 64);
                        azureSynthesizer2.SpeakTextAsync("Select another answer, then");
                        break;
                    }

                case "Repeat":
                case "Again":
                    ReadQuestion();
                    break;
            }
        }




        private void ConfirmAnswer(string answer)
        {
            if (answer[0] == 'A')
            {
                lblA.BackColor = Color.Goldenrod;
                label7.ForeColor = Color.Goldenrod;
            }
            if (answer[0] == 'B')
            {
                lblB.BackColor = Color.Goldenrod;
                label8.ForeColor = Color.Goldenrod;
            }
            if (answer[0] == 'C')
            {
                lblC.BackColor = Color.Goldenrod;
                label9.ForeColor = Color.Goldenrod;
            }
            if (answer[0] == 'D')
            {
                lblD.BackColor = Color.Goldenrod;
                label10.ForeColor = Color.Goldenrod;
            }
            string textToConfirm = $"You said: {answer[0]}. Is that correct? Say 'Yes' to confirm, 'No' to cancel, or 'Skip' to skip the question";
            azureSynthesizer2.SpeakTextAsync(textToConfirm);
            questions[currentQuestionIndex].SelectedAnswer = answer == "Skip" ? null : answer; // Temporarily hold answer, do not save yet  
        }

        private void SaveAnswer()
        {
            // Confirm the answer and save it to the question list
            string answer = questions[currentQuestionIndex].SelectedAnswer;
            CheckAnswer(answer.Substring(0, 1).ToUpper()); // This method now commits the answer
            if (answer[0] == 'A')
            {
                lblA.BackColor = Color.White;
                label7.ForeColor = Color.FromArgb(0, 0, 64);
            }
            if (answer[0] == 'B')
            {
                lblB.BackColor = Color.White;
                label8.ForeColor = Color.FromArgb(0, 0, 64);
            }
            if (answer[0] == 'C')
            {
                lblC.BackColor = Color.White;
                label10.ForeColor = Color.FromArgb(0, 0, 64);
            }
            if (answer[0] == 'D')
            {
                lblD.BackColor = Color.White;
                label10.ForeColor = Color.FromArgb(0, 0, 64);
            }
        }


        private void CheckAnswer(string selectedAnswer)
        {
            var question = questions[currentQuestionIndex];
            question.SelectedAnswer = selectedAnswer;
            if (selectedAnswer != null && selectedAnswer.Equals(question.answer))
            {
                GlobalExam.Score += 0.5;
            }
            SaveQuestionsToFile(); // Update JSON after each answer
            NextQuestion();
        }


        private void SkipQuestion()
        {
            skippedQuestionsCount++;
            questions[currentQuestionIndex].IsSkipped = true;
            questions.Add(questions[currentQuestionIndex]);
            NextQuestion();
        }

        private async System.Threading.Tasks.Task NextQuestion()
        {
            // Decrease skipped count if the current question was skipped and is now answered
            if (questions[currentQuestionIndex].IsSkipped && questions[currentQuestionIndex].SelectedAnswer != null)
            {
                skippedQuestionsCount--;
                questions[currentQuestionIndex].IsSkipped = false; // mark as no longer skipped
            }

            currentQuestionIndex++;

            // Check if there are any remaining skipped questions
            bool isLastQuestion = currentQuestionIndex >= questions.Count && skippedQuestionsCount == 0;

            if (currentQuestionIndex < questions.Count)
            {
                ReadQuestion();
            }
            else if (!isLastQuestion && questions.Any(q => q.IsSkipped))
            {
                currentQuestionIndex = questions.FindIndex(q => q.IsSkipped);
                ReadQuestion();
            }
            else
            {
                EndExam();
            }
        }




        private void EndExam()
        {
            if (!timeExpired)
            {
               // azureSynthesizer.SpeakTextAsync("Congratulations! You have reach the end of this exam");
                recognizer.RecognizeAsyncCancel();
                timer1.Stop();
                //PlaySoundsSequentially();
                MessageBox.Show("The exam is over!", "Exam Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                azureSynthesizer2.SpeakTextAsync("Unfortunately, your time has expired!.");
                recognizer.RecognizeAsyncCancel();
                timer1.Stop();
                MessageBox.Show("Time is over!", "Exam Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }

        }
        public void GenerateQuestionsJson()
        {
            var questions = new List<Question>();
            for (int i = 1; i <= 20; i++)
            {
                questions.Add(new Question
                {
                    questionNo = questions[currentQuestionIndex].questionNo,
                    question = questions[currentQuestionIndex].question,
                    A = questions[currentQuestionIndex].A,
                    B = questions[currentQuestionIndex].B,
                    C = questions[currentQuestionIndex].C,
                    D = questions[currentQuestionIndex].D,
                    answer = questions[currentQuestionIndex].answer,
                    IsSkipped = questions[currentQuestionIndex].IsSkipped,
                    SelectedAnswer = questions[currentQuestionIndex].SelectedAnswer
                });
            }

            string json = JsonConvert.SerializeObject(questions, Formatting.Indented);
            string filePath = GetFilePath();
            File.WriteAllText(filePath, json);
        }
        private string GetFilePath()
        {
            string userFirstName = GlobalUser.LoggedInUser.FirstName; // Replace with actual user data
            string userLastName = GlobalUser.LoggedInUser.LastName; ; // Replace with actual user data
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = @$"{appDataFolder}\Pep\Blind\";
            Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, $"result_{userFirstName}-{userLastName}.json");
        }

        private void SaveQuestionsToFile()
        {
            string json = JsonConvert.SerializeObject(questions, Formatting.Indented);
            File.WriteAllText(GetFilePath(), json);
        }
    }


    public class Question
    {
        public int questionNo { get; set; }
        public string question { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string answer { get; set; }
        public bool IsSkipped { get; set; }
        public string SelectedAnswer { get; set; }
    }


    public static class GlobalExam
    {
        public static double Score { get; set; }
    }
}
