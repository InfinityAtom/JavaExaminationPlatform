using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;

namespace JavaExam
{
    public partial class ChatWindow : Form
    {
        private string inputFilePath = @"C:\TaskWorker\TaskHelper\input.txt";
        private string outputFilePath = @"C:\TaskWorker\TaskHelper\output.txt";
        private string taskFilePath = @"C:\TaskWorker\TaskCreator\tasks.txt";
        private Process pythonProcess;
        private FileSystemWatcher outputWatcher;

        public ChatWindow()
        {
            InitializeComponent();
            textBoxInput.Enabled = false;
            StartPythonProcess();
            AssignTaskButtonHandlers();
            SetupFileSystemWatcher();
        }

        private void StartPythonProcess()
        {
            pythonProcess = new Process();
            pythonProcess.StartInfo.FileName = @"C:\TaskWorker\TaskHelper\dist\main\main.exe";
            pythonProcess.StartInfo.CreateNoWindow = true;
            pythonProcess.StartInfo.UseShellExecute = false;
            pythonProcess.Start();
        }

        private void AssignTaskButtonHandlers()
        {
            buttonTask1.Click += OnTaskButtonClick;
            buttonTask2.Click += OnTaskButtonClick;
            buttonTask3.Click += OnTaskButtonClick;
            buttonTask4.Click += OnTaskButtonClick;
        }

        private async void OnTaskButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string taskNumber = button.Text.Split(' ')[1];
                AppendUserMessage($"Task {taskNumber}, please");
                await PrepareTaskInputFile(taskNumber);
                AppendAIMessage("Typing...");
                textBoxInput.Enabled = false;
            }
        }

        private async System.Threading.Tasks.Task PrepareTaskInputFile(string taskNumber)
        {
            string tasksContent = await File.ReadAllTextAsync(taskFilePath, Encoding.UTF8);
            string additionalPrompt = $"\n\nPlease give me only guidance (no code) to solve the task: {taskNumber}";
            await File.WriteAllTextAsync(inputFilePath, tasksContent + additionalPrompt);
        }

        private void SetupFileSystemWatcher()
        {
            outputWatcher = new FileSystemWatcher(Path.GetDirectoryName(outputFilePath))
            {
                Filter = Path.GetFileName(outputFilePath),
                NotifyFilter = NotifyFilters.LastWrite
            };
            outputWatcher.Changed += OnOutputFileChanged;
            outputWatcher.EnableRaisingEvents = true;
        }

        private DateTime lastReadTime = DateTime.MinValue; // To store the last read time
        private readonly object _lock = new object(); // To lock the read operation


        private string lastAppendedContent = string.Empty; // To store the last content added

        private void OnOutputFileChanged(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                DateTime currentReadTime = DateTime.Now;
                TimeSpan timeSinceLastRead = currentReadTime - lastReadTime;

                // Ensure at least 1 second has passed before processing again
                if (timeSinceLastRead.TotalMilliseconds > 1000)
                {
                    // Disable watcher temporarily to avoid re-triggering while processing
                    outputWatcher.EnableRaisingEvents = false;

                    // Ensure the file is fully written before reading
                    System.Threading.Tasks.Task.Delay(500).ContinueWith(t =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            // Read and clear the file in a single operation
                            string response = File.ReadAllText(outputFilePath);
                            File.WriteAllText(outputFilePath, string.Empty); // Clear the file before processing the content

                            // Only append if the new content is different from the last appended content
                            if (!string.IsNullOrEmpty(response) && response != lastAppendedContent)
                            {
                                // Append to RichTextBox
                                AppendAIMessage(response);

                                // Update the last appended content
                                lastAppendedContent = response;
                            }

                            textBoxInput.Enabled = true;

                            // Re-enable watcher after processing
                            outputWatcher.EnableRaisingEvents = true;
                        }));
                    });

                    lastReadTime = currentReadTime; // Update the last read time
                }
            }
        }




        private async System.Threading.Tasks.Task OnTextBoxInputKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string userInput = textBoxInput.Text.Trim();
                if (!string.IsNullOrEmpty(userInput))
                {
                    AppendUserMessage(userInput);

                    // Write the user input to input.txt
                    await File.WriteAllTextAsync(inputFilePath, userInput, Encoding.UTF8);

                    // Clear input.txt immediately after writing the content
                    await File.WriteAllTextAsync(inputFilePath, string.Empty);

                    AppendAIMessage("Typing...");
                    textBoxInput.Clear();
                    textBoxInput.Enabled = false;
                }
            }
        }


        // Method to append the user's message with markdown parsing
        private void AppendUserMessage(string message)
        {
            ApplyMarkdownFormatting($"[User]: {message}", HorizontalAlignment.Right, Color.Blue, FontStyle.Bold);
        }

        // Method to append the AI helper's message with markdown parsing
        private void AppendAIMessage(string message)
        {
            ApplyMarkdownFormatting($"[AI Helper]: {message}", HorizontalAlignment.Left, Color.Black, FontStyle.Italic);
        }

        // Method to apply markdown formatting to the messages
        private void ApplyMarkdownFormatting(string message, HorizontalAlignment alignment, Color color, FontStyle defaultStyle)
        {
            richTextBoxChat.SelectionAlignment = alignment;
            string[] tokens = ParseMarkdown(message);
            foreach (var token in tokens)
            {
                if (token.StartsWith("**") && token.EndsWith("**"))
                {
                    ApplyTextStyle(token.Trim('*'), FontStyle.Bold, color);
                }
                else if (token.StartsWith("*") && token.EndsWith("*"))
                {
                    ApplyTextStyle(token.Trim('*'), FontStyle.Italic, color);
                }
                else if (token.StartsWith("`") && token.EndsWith("`"))
                {
                    // Apply Courier New font and bold for inline code
                    ApplyTextStyle(token.Trim('`'), FontStyle.Bold, color, fontName: "Courier New");
                }
                else
                {
                    ApplyTextStyle(token, defaultStyle, color);
                }
            }
            richTextBoxChat.AppendText(Environment.NewLine); // Add spacing after message
            richTextBoxChat.ScrollToCaret(); // Ensure the latest message is visible
        }

        // Helper to apply text style to a specific token
        private void ApplyTextStyle(string text, FontStyle style, Color color, Color? backgroundColor = null, string fontName = null)
        {
            richTextBoxChat.SelectionFont = new Font(fontName ?? richTextBoxChat.Font.FontFamily.Name, richTextBoxChat.Font.Size, style);
            richTextBoxChat.SelectionColor = color;
            if (backgroundColor.HasValue)
            {
                richTextBoxChat.SelectionBackColor = backgroundColor.Value;
            }
            else
            {
                richTextBoxChat.SelectionBackColor = richTextBoxChat.BackColor;
            }
            richTextBoxChat.AppendText(text);
        }

        // Parse the markdown in the message
        private string[] ParseMarkdown(string message)
        {
            // Use Regex to split the message into parts: bold (**text**), italic (*text*), inline code (`code`)
            string pattern = @"(\*\*.*?\*\*|\*.*?\*|`.*?`)";
            return Regex.Split(message, pattern);
        }

        private void ChatWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                pythonProcess.Kill();
            }
        }
        

        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnTextBoxInputKeyPress(sender, e);
        }

        private void ChatWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
