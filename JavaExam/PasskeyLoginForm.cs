using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class PasskeyLoginForm : Form
    {
        public string LoggedInEmail { get; private set; }

        public PasskeyLoginForm()
        {
            InitializeComponent();
        }

        private void PasskeyLoginForm_Load(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Delay(100); // Small delay to ensure the form and control are ready
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                if (webView21 != null)
                {
                    await webView21.EnsureCoreWebView2Async(null);

                    // Load the login page
                    webView21.Source = new Uri("http://localhost:8000");

                    // Handle messages from JavaScript
                    webView21.CoreWebView2.WebMessageReceived += WebView_CoreWebView2_WebMessageReceived;

                    // Optionally, trigger the authentication process
                    await webView21.CoreWebView2.ExecuteScriptAsync("getAssertion();");
                }
                else
                {
                    MessageBox.Show("WebView2 control is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 Initialization Error: {ex.Message}\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WebView_CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                LoggedInEmail = e.TryGetWebMessageAsString();
                if (!string.IsNullOrEmpty(LoggedInEmail))
                {
                    // Set the dialog result to OK and close the form
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Received empty email from JavaScript.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling message from JavaScript: {ex.Message}\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
