using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaExam
{
    public partial class SelectSimulation : Form
    {
        private static string intelliJPath;
        public SelectSimulation()
        {
            InitializeComponent(); 
            this.pictureBox1.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("SplashLogo");
            this.pictureBox2.Image = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject("brandLogo");
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalSim.IsSimulation=true;
            GlobalSim.IsTrainingMode=true;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "users.txt");

            string[] lines = {
                                    $"{"Default"}",
                                    $"{"User"}",
                                    $"{"-"}",
                                    $"{"0"}",
                                    $"{"0"}"
                                };

            File.WriteAllLines(filePath, lines);
            if(MessageBox.Show("YOU WILL START A SIMULATED VERSION OF THIS EXAM. NO RESULTS WILL BE SAVED TO THE DATABASE! DO YOU WISH TO CONTINUE?","WARNING!",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
            {

                if (IsIntelliJInstalled() == true)
                {
                    GlobalPath.IJPath = intelliJPath + "\\bin\\idea64.exe";
                    IntelliJVersionSelector t = new IntelliJVersionSelector();
                    t.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("WE COULDN'T FIND ANY INSTACE OF INTELLIJ INSTALLED IN YOUR SYSTEM. YOU'LL HAVE TO MANUALLY INPUT THE PATH, IF IT'S NOT STORED IN THE DEFAULT PLACE", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    IntelliJVersionSelector t = new IntelliJVersionSelector();
                    t.Show();
                    Hide();
                }
            }
        }
        private static bool IsIntelliJInstalled()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    string rootDirectory = drive.RootDirectory.FullName;
                    string[] possiblePaths = {
                    Path.Combine(rootDirectory, "Program Files", "JetBrains"),
                    Path.Combine(rootDirectory, "Program Files (x86)", "JetBrains")
                };

                    foreach (string path in possiblePaths)
                    {
                        if (Directory.Exists(path))
                        {
                            string[] directories = Directory.GetDirectories(path);
                            foreach (string directory in directories)
                            {
                                if (directory.Contains("IntelliJ"))
                                {
                                    intelliJPath = directory;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Drive {drive.Name} is not ready.");
                }
            }

            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            GlobalSim.IsSimulation = true;
            GlobalSim.IsTrainingMode = false;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "users.txt");

            string[] lines = {
                                    $"{"Default"}",
                                    $"{"User"}",
                                    $"{"-"}",
                                    $"{"0"}",
                                    $"{"0"}"
                                };


            File.WriteAllLines(filePath, lines);
            if (MessageBox.Show("YOU WILL START A SIMULATED VERSION OF THIS EXAM. NO RESULTS WILL BE SAVED TO THE DATABASE! DO YOU WISH TO CONTINUE?", "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if(IsIntelliJInstalled()==true) 
                {
                GlobalPath.IJPath = intelliJPath + "\\bin\\idea64.exe";
                IntelliJVersionSelector t = new IntelliJVersionSelector();
                t.Show();
                Hide();
                }
                else
                {
                    MessageBox.Show("WE COULDN'T FIND ANY INSTACE OF INTELLIJ INSTALLED IN YOUR SYSTEM. YOU'LL HAVE TO MANUALLY INPUT THE PATH, IF IT'S NOT STORED IN THE DEFAULT PLACE", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    IntelliJVersionSelector t = new IntelliJVersionSelector();
                    t.Show();
                    Hide();
                }

                
            }
        }
    }
}
