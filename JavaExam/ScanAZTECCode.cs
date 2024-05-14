using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing; // Add ZXing namespace
using ZXing.Common;
using ZXing.Aztec;
using ZXing.Windows.Compatibility;
using Microsoft.EntityFrameworkCore;
using System.Media;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;


namespace JavaExam
{
    public partial class ScanAZTECCode : Form
    {
        private volatile bool _shouldStop = false;
        private SpeechRecognizer speechRecognition;
        public SpeechRecognitionEngine recognizer;
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection filterInfoCollection;
        
        private VideoCaptureDevice videoCaptureDevice;
        private string decodedString; // String to store the decoded data
        private string pathAudio = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Pep\Accessibility\Voices";
        public ScanAZTECCode()
        {
            InitializeComponent();
            speechRecognition = new SpeechRecognizer();

            pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("Accessibility");
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
            pictureBoxVideo.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("WarnCamera");

            // Initialize devices and recognition
            InitializeDevicesAndRecognition();
            PlaySoundsSequentially();

        }

        private void InitializeDevicesAndRecognition()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboCamera.Items.Add(filterInfo.Name);
            cboCamera.SelectedIndex = 0;

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
            timer1.Start();
            recognizer = new SpeechRecognitionEngine();
            Choices commands = new Choices();
            commands.Add(new string[] { "REPEAT", "AGAIN" });
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commands);
            Grammar grammar = new Grammar(gb);

            recognizer.LoadGrammarAsync(grammar);
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text.ToUpper() == "AGAIN" || e.Result.Text.ToUpper() == "REPEAT")
            {
                if (recognizer != null)
                {
                    recognizer.RecognizeAsyncCancel();
                } // Stop recognizing when the command is detected
                PlaySoundsSequentially();
            }
        }


        private void ScanAZTECCode_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            videoCaptureDevice.SignalToStop();
            if (recognizer != null)
            {
                recognizer.RecognizeAsyncCancel();
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBoxVideo.Invoke(new MethodInvoker(delegate { pictureBoxVideo.Image = frame; }));
        }

        private void ScanAZTECCode_Load(object sender, EventArgs e)
        {

        }
        public async void PlaySoundsSequentially()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                PlaySound(pathAudio + @"\firstStep.wav");

            });

            // Start listening after playing the sounds
            StartListening();
        }


        private void PlaySound(string filePath)
        {
            using (SoundPlayer player = new SoundPlayer(filePath))
            {
                player.PlaySync(); // Plays the sound synchronously
            }
        }

        private void StartListening()
        {
            recognizer.RecognizeAsync(RecognizeMode.Single);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
            timer1.Start();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            BarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode((Bitmap)pictureBoxVideo.Image);
            if (result != null)
            {
                if (result.Text == "WRONGSIDEPLEASEFLIPTHEPAGEANDSCANTHEDOCUMENTAGAIN")
                {
                    // Stop the current audio (if any) and play the specific audio
                    if (recognizer != null)
                    {
                        recognizer.RecognizeAsyncCancel(); // Stops recognizing
                    }

                    PlaySound(pathAudio + @"\wrongPaperSide.wav"); // Play error message immediately
                }
                else if (result.Text.Length == 64 && CheckHashValid(result.Text))
                {
                    timer1.Stop();
                    videoCaptureDevice.SignalToStop();

                    pictureBoxVideo.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("PEP");
                    decodedString = result.Text;
                    LoginStudent(decodedString);

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
                                if(recognizer!= null) 
                                {
                                    recognizer.RecognizeAsyncCancel();
                                }
                                
                                 // Stops recognizing

                                BookingError bookingError = new BookingError();
                                bookingError.Show();
                                Hide();
                            }
                            else
                            {

                                ProctorBlind proctor = new ProctorBlind();
                                proctor.Show();
                                Hide();
                            }

                        }

                    }
                    catch (System.NullReferenceException)
                    {
                        recognizer.RecognizeAsyncCancel(); // Stops recognizing
                        BookingError bookingError = new BookingError();
                        bookingError.Show();
                        Hide();
                    }


                }
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
                    lblFName.Text = GlobalUser.LoggedInUser.FirstName;
                    lblLname.Text = GlobalUser.LoggedInUser.LastName;
                    lblEmail.Text = GlobalUser.LoggedInUser.Email;
                    lblFaculty.Text = GlobalUser.LoggedInUser.Faculty;
                    lblGroup.Text = GlobalUser.LoggedInUser.Groupa;
                    lblCode.Text = decodedString.ToString();
                }
                else
                {
                    // If the login code is invalid, don't show an error message since we continue scanning.
                    // MessageBox.Show("Invalid Login Code"); // This line can be commented out or removed.
                }
            }
        }

        private void lblLname_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            videoCaptureDevice.SignalToStop();
            videoCaptureDevice.NewFrame -= new NewFrameEventHandler(VideoCaptureDevice_NewFrame);
            videoCaptureDevice = null;
        }

        private void ScanAZTECCode_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible==false)
            {
                videoCaptureDevice.SignalToStop();
                if (recognizer != null)
                {
                    recognizer.RecognizeAsyncCancel();
                }
            }
        }
    }
}
