using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

namespace Setup
{
    public partial class MainForm : Form
    {
        private String InstallDir = "";
        private bool Installing = false;
        private bool stop = false;
        private List<String> AppFiles = new List<String>();

        public MainForm()
        {
            InitializeComponent();
            AppFiles.Add(Application.ExecutablePath.ToLower());
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            //Exit/cancel button clicked
            if (Installing)
            {
                //If its installing, cancel installation
                if (MessageBox.Show("Cancel Installation?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    stop = true;
            }
            else
            {
                //If not installing, exit program
                this.Close();
            }
        }

        private void InstallBtn_Click(object sender, EventArgs e)
        {
            Work.RunWorkerAsync(true);
        }
        private void StartInstall()
        {
            if (DirBox.Text == ProgramFiles() + "\\Empires\\" || Directory.Exists(DirBox.Text))
            {
                Work.ReportProgress(1001);

                DirectoryInfo i = new DirectoryInfo(DirBox.Text);
                i.Create();
                InstallDir = i.FullName;

                bool suc = Install();
                if (suc == false && InstallBtn.Text == "Install")
                {
                    Uninstall(true);
                    if (stop)
                        Work.ReportProgress(1007);
                    else
                        Work.ReportProgress(1005);
                }
                stop = false;
                Work.ReportProgress(1002);
                if (suc)
                {
                    Work.ReportProgress(1003);
                }
            }
            else
                MessageBox.Show("The selected directory does not exist");
        }
        private void CreateShortcut(String LinkPath, String IconPath, String DskPath, String description)
        {
            Setup.Shortcut link = new Shortcut();
            link.Path = LinkPath;
            link.Description = description;
            link.SetIcon(IconPath, 0);
            link.Save(DskPath);
        }
        private bool Install()
        {
            Work.ReportProgress(1004);
            WebClient WC = new WebClient();
            string path = "http://ammonitesoftware.dyndns.org:7035/Empires/";
            List<String> Files = new List<string>();
            try
            {
                Stream s = WC.OpenRead(path + "install.xml");
                StreamReader sr = new StreamReader(s);
                while (!sr.EndOfStream)
                    Files.Add(sr.ReadLine());
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to download file list\nInstallation Aborting");
                Work.ReportProgress(1005);
                return false;
            }
            Work.ReportProgress(1006);
            Directory.CreateDirectory(DirBox.Text);
            for (int i = 0; i < Files.Count + 1; i++)
            {
                try
                {
                    if (i == Files.Count)
                    {
                        WC.DownloadFile(path + "version.xml", InstallDir + "version.xml");
                        WC.DownloadFile(path + "dxwebsetup.exe", InstallDir + "dxwebsetup.exe");
                        WC.DownloadFile(path + "slimdx.msi", InstallDir + "slimdx.msi");
                    }
                    else
                    {
                        FileInfo f = new FileInfo(InstallDir + Files[i]);
                        if (!f.Directory.Exists)
                            f.Directory.Create();
                        if (!AppFiles.Contains(f.FullName.ToLower()))
                            WC.DownloadFile(path + Files[i].Replace("\\", "/"), InstallDir + Files[i]);
                        Work.ReportProgress(1008, (int)((1.0f / Files.Count) * 800));
                        Work.ReportProgress(1011, (i + 1).ToString() + " of " + Files.Count.ToString());
                    }
                    if (stop)
                    {
                        Work.ReportProgress(1007);
                        return false;
                    }
                }
                catch (Exception)
                {
                    if (MessageBox.Show("Unable to download a file", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    {
                        Work.ReportProgress(1005);
                        return false;
                    }
                    else
                    {
                        WC.Dispose();
                        WC = new WebClient();
                        i--;
                    }
                }
            }
            if (Shorcut.Checked)
            {
                Work.ReportProgress(1010);
                try
                {
                    CreateShortcut(InstallDir + "Launcher.exe", InstallDir + "Empires.exe", Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + "\\Empires.lnk", "Launch Empires");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create desktop shortcut");
                }
            }
            if (StartMenu.Checked)
            {
                Work.ReportProgress(1023);
                try
                {
                    String StartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                    String linkpath = Directory.CreateDirectory(StartMenuPath + "\\Ammonite Software\\Empires\\").FullName;
                    CreateShortcut(InstallDir + "Launcher.exe", InstallDir + "Empires.exe", linkpath + "Empires.lnk", "Launch Empires");
                    CreateShortcut(InstallDir + "Update.exe", InstallDir + "Update.exe", linkpath + "Update.lnk", "Check for Updates");
                    CreateShortcut(InstallDir + "Setup.exe", InstallDir + "Setup.exe", linkpath + "Uninstall.lnk", "Uninstall\\Reinstall Empires");
                    CreateShortcut("http://www.ammonitesoftware.com/", InstallDir + "Empires.exe", linkpath + "Visit Site.lnk", "Visit Ammonite Software online");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create start menu group");
                }
            }
            Work.ReportProgress(1012);
            try
            {
                RegistryKey UninstKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
                UninstKey.DeleteSubKey("Empires", false);
                RegistryKey ProdKey = UninstKey.CreateSubKey("Empires");
                ProdKey.SetValue("DisplayName", "Empires");
                ProdKey.SetValue("UninstallString", InstallDir + "Setup.exe");
                ProdKey.SetValue("InstallLocation", InstallDir);
                ProdKey.SetValue("Publisher", "Ammonite Software");
                ProdKey.SetValue("DisplayIcon", InstallDir + "Empires.exe");
                ProdKey.SetValue("URLInfoAbout", "http://www.ammonitesoftware.com");
                ProdKey.Close();
                UninstKey.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to register program");
                Work.ReportProgress(1005);
            }
            try
            {
                Work.ReportProgress(1022);
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo(InstallDir + "dxwebsetup.exe");
                p.StartInfo = psi;
                MessageBox.Show("Setup will now update required DirectX components");
                p.Start();
                p.WaitForExit();
                p.StartInfo.FileName = InstallDir + "slimdx.msi";
                MessageBox.Show("Setup will now update required SlimDX components");
                p.Start();
                p.WaitForExit();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to install a component");
            }
            Work.ReportProgress(1009);
            return true;
        }
        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DirBox.Text))
                Browse.SelectedPath = DirBox.Text;
            Browse.ShowDialog();
            if (Browse.SelectedPath != "")
                DirBox.Text = Browse.SelectedPath;
        }
        static string ProgramFiles()
        {
            return Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                RegistryKey ProdKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Empires", false);
                if (ProdKey != null)
                {
                    InstallDir = (string)ProdKey.GetValue("InstallLocation");
                    if (InstallDir != null)
                    {
                        DirBox.Text = InstallDir;
                        DirBox.Enabled = false;
                        BrowseBtn.Enabled = false;
                        InstallBtn.Text = "Reinstall";
                        LaunchBtn.Visible = true;
                        UninstBtn.Visible = true;
                    }
                    else
                        throw new Exception();
                }
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                DirBox.Text = ProgramFiles() + "\\Empires\\";
            }
        }

        private void LaunchBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(InstallDir + "Launcher.exe"))
                System.Diagnostics.Process.Start(InstallDir + "Launcher.exe");
            this.Close();
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((bool)e.Argument == true)
                StartInstall();
            else
                StartUninstall();
        }

        private void Work_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1001:
                    //Start install, disable gui
                    Installing = true;
                    DirBox.Enabled = false;
                    BrowseBtn.Enabled = false;
                    StartMenu.Enabled = false;
                    Shorcut.Enabled = false;
                    InstallBtn.Enabled = false;
                    LaunchBtn.Enabled = false;
                    UninstBtn.Enabled = false;
                    ExitBtn.Text = "Cancel";
                    break;
                case 1002:
                    //Install done, reenable gui
                    Installing = false;
                    Shorcut.Enabled = true;
                    StartMenu.Enabled = true;
                    InstallBtn.Enabled = true;
                    ExitBtn.Text = "Exit";
                    break;
                case 1003:
                    LaunchBtn.Enabled = true;
                    LaunchBtn.Visible = true;
                    UninstBtn.Visible = true;
                    UninstBtn.Enabled = true;
                    InstallBtn.Text = "Reinstall";
                    break;
                case 1004:
                    StatusLbl.Text = "Downloading file list...";
                    StatusBar.Value = 50;
                    break;
                case 1005:
                    StatusLbl.Text = "Installation Failed";
                    StatusBar.Value = 1000;
                    break;
                case 1006:
                    StatusLbl.Text = "Downloading and installing...";
                    StatusBar.Value = 100;
                    break;
                case 1007:
                    StatusLbl.Text = "Installation Aborted by User";
                    StatusBar.Value = 1000;
                    break;
                case 1008:
                    StatusBar.Value += (int)e.UserState;
                    break;
                case 1009:
                    StatusLbl.Text = "Installation Successful";
                    StatusBar.Value = 1000;
                    break;
                case 1010:
                    StatusLbl.Text = "Creating Desktop Shortcut...";
                    StatusBar.Value = 900;
                    break;
                case 1011:
                    StatusLbl.Text = "Downloading File " + (String)e.UserState;
                    break;
                case 1012:
                    StatusLbl.Text = "Registering...";
                    StatusBar.Value = 950;
                    break;
                case 1013:
                    //Uninstall, diable gui
                    DirBox.Enabled = false;
                    BrowseBtn.Enabled = false;
                    Shorcut.Enabled = false;
                    InstallBtn.Enabled = false;
                    StartMenu.Enabled = false;
                    LaunchBtn.Enabled = false;
                    UninstBtn.Enabled = false;
                    ExitBtn.Enabled = false;
                    StatusLbl.Text = "Starting uninstall...";
                    StatusBar.Value = 0;
                    break;
                case 1014:
                    StatusLbl.Text = "Uninstall Failed";
                    StatusBar.Value = 1000;
                    break;
                case 1015:
                    StatusLbl.Text = "Uninstalling...";
                    StatusBar.Value = 100;
                    break;
                case 1016:
                    StatusLbl.Text = "Deleting File " + (String)e.UserState;
                    break;
                case 1017:
                    StatusLbl.Text = "Wrapping up...";
                    StatusBar.Value = 950;
                    break;
                case 1018:
                    StatusLbl.Text = "Uninstall Successful";
                    StatusBar.Value = 1000;
                    break;
                case 1020:
                    //Uninstall done, reenable gui
                    Shorcut.Enabled = true;
                    InstallBtn.Enabled = true;
                    StartMenu.Enabled = true;
                    ExitBtn.Enabled = true;
                    UninstBtn.Enabled = true;
                    LaunchBtn.Enabled = false;
                    break;
                case 1021:
                    StatusLbl.Text = "Rolling back...";
                    StatusBar.Value = 100;
                    break;
                case 1022:
                    StatusLbl.Text = "Updating required components...";
                    StatusBar.Value = 975;
                    break;
                case 1023:
                    StatusLbl.Text = "Creating Start Menu Group...";
                    StatusBar.Value = 925;
                    break;
            }
        }

        private void UninstBtn_Click(object sender, EventArgs e)
        {
            Work.RunWorkerAsync(false);
        }
        private int GetFileCount(string path)
        {
            int count = 0;
            DirectoryInfo dir = new DirectoryInfo(path);
            count += dir.GetFiles().Length;
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                count += GetFileCount(di.FullName);
            }
            return count;
        }
        private int DeleteFiles(string path, int count, int numfiles, bool deldir)
        {
            int numnum = count;
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fi in dir.GetFiles())
            {
                Work.ReportProgress(1008, (int)((1.0f / numfiles) * 800));
                Work.ReportProgress(1016, numnum.ToString() + " of " + numfiles.ToString());
                if (!AppFiles.Contains(fi.FullName.ToLower()))
                    fi.Delete();
                numnum++;
            }
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                numnum += DeleteFiles(di.FullName, numnum, numfiles, true);
            }
            if (deldir)
                dir.Delete();
            return numnum;
        }
        private void StartUninstall()
        {
            if (Uninstall(false))
            {
                MessageBox.Show("Uninstall Successful");
                string batpat = new DirectoryInfo(InstallDir).Parent.FullName + "\\Uninstall.bat";
                StreamWriter sw = new StreamWriter(batpat);
                sw.WriteLine("cd /d %0\\..");
                sw.WriteLine(":loop\n");
                foreach (String s in AppFiles)
                {
                    sw.WriteLine("DEL \"" + s + "\"\n");
                    sw.WriteLine("IF EXIST \"" + s + "\" GOTO loop\n");
                }
                sw.WriteLine("rd \"" + InstallDir + "\" /S /Q\n");
                sw.WriteLine("DEL Uninstall.bat");
                sw.Flush();
                sw.Close();
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Verb = "runas";
                psi.FileName = "cmd.exe";
                psi.Arguments = "/c \"" + new DirectoryInfo(InstallDir).Parent.FullName + "\\Uninstall.bat\"";
                p.StartInfo = psi;
                p.Start();
                Application.Exit();
            }
            else
                Work.ReportProgress(1020);
        }
        private bool Uninstall(bool Rollback)
        {
            if (!Rollback)
            {
                Work.ReportProgress(1013);
                //Check to see if it is installed first
                try
                {
                    RegistryKey UninstKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
                    RegistryKey ProdKey = UninstKey.OpenSubKey("Empires");
                    if (ProdKey == null)
                    {
                        MessageBox.Show("Application is not installed\nand cannont be uninstalled");
                        Work.ReportProgress(1014);
                        return false;
                    }
                    InstallDir = (string)ProdKey.GetValue("InstallLocation");
                    if (InstallDir == null)
                        throw new Exception();
                    ProdKey.Close();
                    UninstKey.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to find install directory\nPlease uninstall manually");
                    Work.ReportProgress(1014);
                    return false;
                }
                Work.ReportProgress(1015);
            }
            else
                Work.ReportProgress(1021);
            try
            {
                if (Directory.Exists(InstallDir))
                {
                    int filecount = GetFileCount(InstallDir);
                    DeleteFiles(InstallDir, 1, filecount, false);
                }
                DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Ammonite Software\\");
                if (di.Exists)
                {
                    if(Directory.Exists(di.FullName + "Empires\\"))
                        Directory.Delete(di.FullName + "Empires\\", true);
                    di.Refresh();
                    if (di.GetDirectories().Length == 0 && di.GetFiles().Length == 0)
                        di.Delete();
                }
                String DskPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + "\\Empires.lnk";
                if (File.Exists(DskPath))
                    File.Delete(DskPath);
            }
            catch (Exception)
            {
                if (!Rollback)
                {
                    MessageBox.Show("Unable to delete a file\nPlease uninstall manually");
                    Work.ReportProgress(1014);
                    return false;
                }
            }
            Work.ReportProgress(1017);
            try
            {
                RegistryKey UninstKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
                UninstKey.DeleteSubKey("Empires");
                UninstKey.Close();
            }
            catch (Exception)
            {
                if (!Rollback)
                {
                    MessageBox.Show("Unable to remove from registry\nUninstall failed");
                    Work.ReportProgress(1014);
                    return false;
                }
            }
            if (!Rollback)
                Work.ReportProgress(1018);
            return true;
        }
    }
}