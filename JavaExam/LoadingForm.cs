using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace JavaExam
{
    public partial class LoadingForm : Form
    {
        private HttpListener _httpListener;

        public LoadingForm()
        {
            InitializeComponent();
            
            StartServerAndTransitionAsync();
        }

        private void KillExplorerAndSetBackgroundColor()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C TASKKILL /F /IM explorer.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);

                string backgroundColor = "255 255 255"; // This would set it to white
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Colors", true);
                if (key != null)
                {
                    key.SetValue("Background", backgroundColor);
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error killing explorer.exe: " + ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private async void StartServerAndTransitionAsync()
        {
            // Start the server
            bool serverStarted = await StartServerAsync();

            if (serverStarted)
            {
                // Once the server is confirmed to be running, move to the next form
                MoveToNextForm();
            }
            else
            {
                MessageBox.Show("Failed to start the server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // Exit if the server couldn't start
            }
        }

        private System.Threading.Tasks.Task<bool> StartServerAsync()
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _httpListener = new HttpListener();
                    _httpListener.Prefixes.Add("http://localhost:8000/");
                    _httpListener.Start();
                    Console.WriteLine("Server started on http://localhost:8000/");

                    // Start a loop to handle requests (this runs in the background)
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        while (_httpListener.IsListening)
                        {
                            var context = _httpListener.GetContext(); // Block waiting for requests
                            HandleRequest(context);
                        }
                    });

                    return true; // Server started successfully
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting server: {ex.Message}");
                    return false; // Server failed to start
                }
            });
        }

        private void HandleRequest(HttpListenerContext context)
        {
            string requestedFile = context.Request.RawUrl.TrimStart('/').Replace("/", @"\");
            string filePath = Path.Combine(@"C:\Users\Fabian\source\repos\JavaExam\JavaExam\bin\Debug\net6.0-windows7.0", requestedFile);

            if (string.IsNullOrEmpty(requestedFile) || requestedFile == "index" || requestedFile == "index.html")
            {
                filePath = Path.Combine(@"C:\Users\Fabian\source\repos\JavaExam\JavaExam\bin\Debug\net6.0-windows7.0", "LogInWithPasskey.html");
            }

            if (File.Exists(filePath))
            {
                HttpListenerResponse response = context.Response;
                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentType = GetContentType(filePath);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
        }
        private void LoadingForm_Load(object sender, EventArgs e)
        {
            
        }
        private string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".html" => "text/html",
                ".js" => "application/javascript",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }

        private void MoveToNextForm()
        {
            // Hide the LoadingForm
            this.Hide();

            // Show the LogIn form
            LogIn loginForm = new LogIn();
            loginForm.Show();
        }

        private void LoadingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopServer(); // Stop the server when the form is closed
        }

        private void StopServer()
        {
            if (_httpListener != null && _httpListener.IsListening)
            {
                _httpListener.Stop();
                _httpListener.Close();
            }
        }
    }
}
