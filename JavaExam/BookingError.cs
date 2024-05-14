using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class BookingError : Form
    {
        private SpeechRecognizer speechRecognition;
        public SpeechRecognitionEngine recognizer;
        private string pathAudio = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Pep\Accessibility\Voices";
        public BookingError()
        {
            InitializeComponent();
            pictureBox3.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("Accessibility");
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");

            // Initialize devices and recognition
            InitializeDevicesAndRecognition();

            // Start the audio playing and recognition sequence
            PlaySoundsSequentially();
        }
        private void InitializeDevicesAndRecognition()
        {
            

            recognizer = new SpeechRecognitionEngine();

            Choices commands = new Choices();
            commands.Add(new string[] { "REPEAT", "AGAIN" , "CLOSE"});
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
                recognizer.RecognizeAsyncStop(); // Stop recognizing when the command is detected
                PlaySoundsSequentially(); // Replay sounds and start recognizing again
            }
            if (e.Result.Text.ToUpper() == "CLOSE")
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
        }

        public async void PlaySoundsSequentially()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                PlaySound(pathAudio + @"\bookingError.wav");

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
    }
}

