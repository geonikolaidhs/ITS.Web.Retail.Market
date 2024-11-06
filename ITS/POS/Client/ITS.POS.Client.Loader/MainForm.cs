using POSLoader.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;
using Microsoft.Win32;
using System.Management;
using ITS.POS.Client.Loader.Helpers;

namespace POSLoader
{
    public partial class MainForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hwnd);

        #region Constants
        private const int TOTAL_CHECKS = 6;//Ο αριθμός των συνολικών ελέγχων που πρέπει να γίνουν προκειμένου
        //να είμαστε σίγουροι ότι το POS είναι έτοιμο να ξεκινήσει


        /*
         * το αρχείο που χρειάζεται για να ξεκινήσει το POS. Αν δεν υπάρχει σημαίνει ότι
         * πρόκειται για νέα εγκατάσταση
         */
        private const string GLOBAL_CONFIG_FILE = ".\\Configuration\\Globals.xml";
        private const string CONFIG_FOLDER = ".\\Configuration";
        private const string BATCH_FILE = "POS.bat";
        private const string WEB_SERVICE_FILE = "/POSUpdateService.asmx";//Προσοχή στο πρώτο '/'!!!
        private const string POSLOADER_ZIP_FILE = "POSLoader.zip";
        private const string VNC_INSTALLER = "tightvnc.msi";
        private const string POS_EXE = "ITS.POS.Client.exe";
        private const string POS_DB_ZIP = "dbs.zip";
        private const string MODULES_FOLDER = ".\\Modules";

        /*
         * Log Settins
         */
        private const string LOG_FOLDER = ".\\Log";
        #endregion

        public LogHelper Logger { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            DeleteFile(BATCH_FILE);
            this.Text += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static void DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            //TODO Start POS and Exit
            //logger.StopLogging();
            //Environment.Exit(0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(CONFIG_FOLDER))
            {
                Directory.CreateDirectory(CONFIG_FOLDER);
            }
            if (!Directory.Exists(LOG_FOLDER))
            {
                Directory.CreateDirectory(LOG_FOLDER);
            }
            string fileName = String.Format("{0}\\{1:yyyy-MM-dd_hh-mm-ss-tt}.txt", LOG_FOLDER, DateTime.Now);
            Logger = new LogHelper(rtxtbxLog, fileName);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    e.Cancel = false;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }

        }

        private void DownloadPOSPrebuiltFiles()
        {
            if (!File.Exists(".\\POSMaster") && !File.Exists(".\\POSVersions"))
            {
                using (WebClient myWebClient = new WebClient())
                {
                    try
                    {
                        string file_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + "/POS/POSDatabase.zip";
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(file_url, POS_DB_ZIP);
                        Unzip(POS_DB_ZIP, ExtractExistingFileAction.DoNotOverwrite, ".");
                        if (File.Exists(POS_DB_ZIP))
                        {
                            File.Delete(POS_DB_ZIP);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public class SettingFile
        {
            public string Filepath { get; set; }
            public string WebserviceMethod { get; set; }
            public bool IsMandatory { get; set; }
            public bool ForceFileDeletion { get; set; }

            public SettingFile(string filepath, string webServiceMethod, bool isMandatory, bool forceFileDeletion = true)
            {
                this.Filepath = filepath;
                this.WebserviceMethod = webServiceMethod;
                this.IsMandatory = IsMandatory;
                this.ForceFileDeletion = forceFileDeletion;
            }
        }

        private void LoaderProcess()
        {
            //var list = GetInstalledPrograms();

            progressBarLoading.Maximum = TOTAL_CHECKS;
            progressBarLoading.Value = 0;


            ////key = filepath, value = WebServiceMethod, isMandatory
            List<SettingFile> settingsFiles = new List<SettingFile>()
            {
               new SettingFile( CONFIG_FOLDER + "\\Devices.xml", "GetDevicesXml",true),
               new SettingFile( CONFIG_FOLDER + "\\Globals.xml", "GetGlobalsXml",true),
               new SettingFile( CONFIG_FOLDER + "\\ReceiptFormat.xml", "GetReceiptFormat",true),
               new SettingFile( CONFIG_FOLDER + "\\XReportFormat.xml","GetXFormat",true),
               new SettingFile( CONFIG_FOLDER +"\\ZReportFormat.xml", "GetZFormat",true),
               new SettingFile( CONFIG_FOLDER + "\\ActionLevels.xml", "GetActionLevelsXml",false),
               new SettingFile( MODULES_FOLDER + "\\CustomForm.dll", "GetCustomLayout",false,false),
               new SettingFile( CONFIG_FOLDER + "\\EntityUpdaterModes.xml", "GetUpdaterModesXml",false),
               new SettingFile( CONFIG_FOLDER + "\\CustomActionCodes.xml","GetCustomActionCodesXml",false),
               new SettingFile( CONFIG_FOLDER + "\\ReportSettings.xml","GetReportSettingsXml",false),
               //new SettingFile( MODULES_FOLDER + "\\ITS.POS.Reports.dll", "GetPOSReportLibrary",false,false)
            };


            //Βήμα 1ο : Έλεγχος αν πρόκειται για νέο POS που δεν έχει ακόμη ρυθμιστεί καθόλου
            if (!File.Exists(GLOBAL_CONFIG_FILE))
            {
                Logger.Warning("Please provide Server Settings to set up your POS.");
                using (SettingsForm settingsForm = new SettingsForm(GLOBAL_CONFIG_FILE, WEB_SERVICE_FILE, Logger))
                {
                    settingsForm.Location = new Point(Left + (Width - settingsForm.Width) / 2, this.Top);
                    settingsForm.ShowDialog();
                }
                Logger.Success("Your POS has been succesfully connected with the specified Server.");
            }
            progressBarLoading.Value++;

            DownloadPOSPrebuiltFiles();

            //Βήμα 2ο : Έλεγχος ύπαρξης νέας έκδοσης του POSLoader!
            Logger.Message("Checking Version.Please wait...");

            bool hasConnection = true;

            try
            {
                Version posLoaderVersion = GetPOSVersion();
                if (Assembly.GetExecutingAssembly().GetName().Version < posLoaderVersion)
                {
                    Logger.Warning("New Version found!.Downloading.Please wait...");
                    DownloadPOSLoader();
                    CreateBatchFile();
                    using (Process batProcess = new Process())
                    {
                        batProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        batProcess.StartInfo.CreateNoWindow = true;
                        batProcess.StartInfo.UseShellExecute = false;
                        batProcess.StartInfo.FileName = BATCH_FILE;
                        batProcess.Start();

                        Environment.Exit(1);
                    }
                }
                else if (Assembly.GetExecutingAssembly().GetName().Version > posLoaderVersion)
                {
                    Logger.Error("Store Controller version is older than loader. Please update Store Controller. \nStore Controller Version:" + posLoaderVersion + ", Loader Version:" + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }
            }
            catch (WebException ex)
            {
                Logger.Warning("There was an error while trying to communicate with the server: " + ex.Message);
                hasConnection = false;
            }


            progressBarLoading.Value++;

            //Βήμα 3ο : Έλεγχος ύπαρξης των αρχείων ρυθμίσεων
            Logger.Message("Checking settings files.Please Wait...");

            string web_service_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + WEB_SERVICE_FILE;
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("posid", ConfigHelper.DeviceId(GLOBAL_CONFIG_FILE));
            foreach (SettingFile settingsFile in settingsFiles)// (KeyValuePair<string, Tuple<string, bool>> pair in settingsFiles)
            {
                if (hasConnection)
                {
                    if (!String.IsNullOrEmpty(settingsFile.WebserviceMethod))//if (!String.IsNullOrEmpty(pair.Value.Item1))
                    {
                        object dataToSave = null;
                        try
                        {
                            dataToSave = WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, settingsFile.WebserviceMethod, args);//dataToSave = WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, pair.Value.Item1, args);

                            if (settingsFile.ForceFileDeletion && File.Exists(settingsFile.Filepath))//if (File.Exists(pair.Key))  //Since the call was successfull, regardless if dataToSave is null, file must be cleared.
                            {                           //This is to prevent optional files from remaining after they have been cleared from the web
                                File.Delete(settingsFile.Filepath);//File.Delete(pair.Key);
                            }

                            if (dataToSave != null)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(settingsFile.Filepath)));//Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(pair.Key)));
                            }

                            if (dataToSave is string)
                            {
                                XmlDocument xml_doc = new XmlDocument();
                                xml_doc.LoadXml((string)dataToSave);
                                xml_doc.Save(settingsFile.Filepath);//xml_doc.Save(pair.Key);
                            }
                            else if (dataToSave is byte[])
                            {
                                if (((byte[])dataToSave).Length == 1)
                                {
                                    int enumValue = ((int)((byte[])dataToSave)[0]);//BitConverter.ToInt32(((byte[])dataToSave), 0);
                                    ITS.Retail.Platform.Enumerations.eServiceResponce responce = (ITS.Retail.Platform.Enumerations.eServiceResponce)enumValue;
                                    if (responce == ITS.Retail.Platform.Enumerations.eServiceResponce.EMPTY_RESPONCE
                                        && File.Exists(settingsFile.Filepath))
                                    {
                                        File.Delete(settingsFile.Filepath);
                                    }
                                }
                                else
                                {
                                    File.WriteAllBytes(settingsFile.Filepath, (byte[])dataToSave);//File.WriteAllBytes(pair.Key, (byte[])dataToSave);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("An Error occured trying to save '" + settingsFile.Filepath + "'. " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));//Logger.Error("An Error occured trying to save '" + pair.Key + "'. " + ex.Message);
                            if (dataToSave is string)
                            {
                                Logger.Error("Returned value is: " + dataToSave);
                            }
                            return;
                        }
                    }
                }
                else if (settingsFile.IsMandatory && !File.Exists(settingsFile.Filepath))//else if (pair.Value.Item2 == true && !File.Exists(pair.Key)) //if it's mandatory and doesn't exist, then throw error
                {
                    Logger.Error("Configuration file '" + settingsFile.Filepath + "' is missing. Cannot start the application.");//Logger.Error("Configuration file '" + pair.Value.Item1 + "' is missing. Cannot start the application.");
                    return;
                }
            }

            if (hasConnection)
            {
                try
                {
                    string web_method = "GetSpecificDeviceDLLLinks";
                    args = new Dictionary<string, object>();
                    args.Add("posid", ConfigHelper.DeviceId(GLOBAL_CONFIG_FILE));
                    string[] file_urls = (string[])WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, web_method, args);
                    if (file_urls != null)
                    {
                        foreach (string file_url in file_urls)
                        {
                            if (String.IsNullOrEmpty(file_url))
                            {
                                throw new Exception("Could not Download File from remote server, because server gave a bad responce");
                            }
                            if (file_url.ToUpper().Contains("ERROR"))
                            {
                                throw new Exception(file_url);
                            }

                            using (WebClient myWebClient = new WebClient())
                            {
                                // Download the Web resource and save it into the current filesystem folder.

                                string fileName = file_url.Substring(file_url.LastIndexOf('/') + 1);
                                string filepath = MODULES_FOLDER + "\\" + fileName;
                                if (!Directory.Exists(MODULES_FOLDER))
                                {
                                    Directory.CreateDirectory(MODULES_FOLDER);
                                }
                                myWebClient.DownloadFile(file_url, filepath);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    string error_message = exc.Message;
                    if (exc.InnerException != null && exc.InnerException.Message.Length > 0)
                    {
                        error_message += Environment.NewLine + exc.InnerException.Message;
                    }
                    Logger.Error(error_message);
                    throw;
                }

                Logger.Message("Settings files are up to date.");
            }
            progressBarLoading.Value++;

            //Βήμα 4ο : Αναβάθμιση POS ΑΝ ΚΑΙ ΜΟΝΟ ΑΝ ΧΡΕΙΑΖΕΤΑΙ
            Logger.Message("Checking for new version.");
            Version POSVersion = new Version(0, 0, 0, 0);
            if (File.Exists(POS_EXE))
            {
                POSVersion = AssemblyName.GetAssemblyName(POS_EXE).Version;
            }
            else if (!hasConnection)
            {
                Logger.Error("Main application is missing. Cannot start the application.");
                return;
            }
            if (!POSVersion.Equals(Assembly.GetExecutingAssembly().GetName().Version))
            {
                if (!hasConnection)
                {
                    Logger.Error("Main application has invalid version. Cannot start the application.");
                    return;
                }
                Logger.Message("New Version found.Please wait while downloading...");
                DownloadPOSApplication();
            }

            //Βήμα 4Α: Εγκατάσταση VNC


            //Βήμα 5ο : Εκκίνηση POS και κλείσιμο τρέχουσας εφαρμογής
            Logger.Message("ITS POSClient will automatically start.Please wait.");
            try
            {
                Process proc = System.Diagnostics.Process.Start(POS_EXE, "");
                Application.DoEvents();
                Application.DoEvents();
                int hwnd = (int)proc.MainWindowHandle;
                if (hwnd != 0)
                {
                    //if the handle is other than 0, then set the active window
                    SetActiveWindow(hwnd);
                }
                else
                {
                    //we can assume that it is fully hidden or minimized, so lets show it!
                    ShowWindow(proc.Handle, ShowWindowEnum.Restore);
                    SetActiveWindow((int)proc.MainWindowHandle);
                }
                Environment.Exit(0);
            }
            catch (Exception exc)
            {
                string error_message = exc.Message;
                if (exc.InnerException != null && exc.InnerException.Message.Length > 0)
                {
                    error_message += Environment.NewLine + exc.InnerException.Message;
                }
                Logger.Error(error_message);
            }
        }


        private List<InstalledApplication> GetInstalledPrograms()
        {
            List<InstalledApplication> listToReturn = new List<InstalledApplication>();
            RegistryKey key;

            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                listToReturn.Add(new InstalledApplication()
                {
                    Caption = subkey.GetValue("DisplayName") as string,
                    Version = subkey.GetValue("DisplayVersion") as string
                });


            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                listToReturn.Add(new InstalledApplication()
                {
                    Caption = subkey.GetValue("DisplayName") as string,
                    Version = subkey.GetValue("DisplayVersion") as string
                });
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                listToReturn.Add(new InstalledApplication()
                {
                    Caption = subkey.GetValue("DisplayName") as string,
                    Version = subkey.GetValue("DisplayVersion") as string
                });
            }
            return listToReturn;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {

        }

        private void CreateBatchFile()
        {
            DeleteFile(BATCH_FILE);
            string new_line = Environment.NewLine;
            string bat_commands = "@ECHO OFF" + new_line +
                                   "set POSLOADER_TIMES=0" + new_line +
                                   "set POSLOADER_MAX_TIMES=150" + new_line +
                                   ":START" + new_line +
                                   "ping 192.0.2.2 -n 1 -w 100 > nul" + new_line +
                                   "tasklist /fi \"IMAGENAME EQ POSLOADER.EXE\" | find /i /n \"POSLOADER.exe\" > nul" + new_line +
                                   "set /a POSLOADER_TIMES+=1" + new_line +
                                   "if %POSLOADER_TIMES% GTR  %POSLOADER_MAX_TIMES% GOTO ERR" + new_line +
                                   "if %ERRORLEVEL% equ 0 goto START" + new_line +
                                   "rem ECHO \"FINISH\"" + new_line +
                                   "copy /y TEMP\\*.*" + new_line +
                                   "start ITS.POS.Client.Loader.exe" + new_line +
                                   "GOTO END" + new_line +
                                   ":ERR" + new_line +
                                   "ECHO \"REACHED MAX TIMES\"" + new_line +
                                   ":END" + new_line;
            File.WriteAllText(BATCH_FILE, bat_commands);
        }

        private Version GetPOSVersion()
        {
            int deviceID = ConfigHelper.DeviceId(GLOBAL_CONFIG_FILE);
            if (deviceID != int.MinValue)
            {
                try
                {
                    string web_service_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + WEB_SERVICE_FILE;
                    string web_method = "GetPOSVersion";
                    Dictionary<string, object> args = new Dictionary<string, object>();
                    object v = WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, web_method, args);
                    string version = v as string;
                    string[] version_number = version.Split('.').ToArray();
                    return new Version(int.Parse(version_number[0]), int.Parse(version_number[1]), int.Parse(version_number[2]), int.Parse(version_number[3]));
                }
                catch (Exception exc)
                {
                    string error_message = exc.Message;
                    if (exc.InnerException != null && exc.InnerException.Message.Length > 0)
                    {
                        error_message += Environment.NewLine + exc.InnerException.Message;
                    }
                    Logger.Error(error_message);
                    throw;
                }
            }
            else
            {
                throw new Exception("Invalide Device ID!");
            }
        }


        private void DownloadVNCInstaller()
        {
            if (!File.Exists(".\\" + VNC_INSTALLER))
            {
                using (WebClient myWebClient = new WebClient())
                {
                    try
                    {
                        string file_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + "/" + VNC_INSTALLER;
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(file_url, VNC_INSTALLER);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }


        private void DownloadPOSLoader()
        {
            try
            {
                string web_service_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + WEB_SERVICE_FILE;
                string web_method = "GetPOSLoaderLink";
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("posid", ConfigHelper.DeviceId(GLOBAL_CONFIG_FILE));
                string file_url = (string)WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, web_method, args);
                if (String.IsNullOrEmpty(file_url))
                {
                    throw new Exception("Could not Download File from remote server, because server gave a bad responce");
                }
                if (file_url.ToUpper().Contains("ERROR"))
                {
                    throw new Exception(file_url);
                }

                using (WebClient myWebClient = new WebClient())
                {
                    // Download the Web resource and save it into the current filesystem folder.
                    myWebClient.DownloadFile(file_url, POSLOADER_ZIP_FILE);
                    if (!Directory.Exists("TEMP"))
                    {
                        Directory.CreateDirectory("TEMP");
                    }
                    Unzip(POSLOADER_ZIP_FILE, ExtractExistingFileAction.OverwriteSilently, "TEMP");
                    if (File.Exists(POSLOADER_ZIP_FILE))
                    {
                        File.Delete(POSLOADER_ZIP_FILE);
                    }
                }
            }
            catch (Exception exc)
            {
                string error_message = exc.Message;
                if (exc.InnerException != null && exc.InnerException.Message.Length > 0)
                {
                    error_message += Environment.NewLine + exc.InnerException.Message;
                }
                Logger.Error(error_message);
                throw;
            }
        }

        private void DownloadPOSApplication()
        {
            try
            {
                string web_service_url = ConfigHelper.WebServiceUrl(GLOBAL_CONFIG_FILE) + WEB_SERVICE_FILE;
                string web_method = "GetPOSClientLink";
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("posid", ConfigHelper.DeviceId(GLOBAL_CONFIG_FILE));
                string file_url = (string)WebServiceHelper.ExecuteCallWebServiceCommand(web_service_url, web_method, args);
                if (file_url == null || file_url.Length == 0)
                {
                    throw new Exception("Could not Download File from remote server, because server gave a bad response");
                }

                using (WebClient myWebClient = new WebClient())
                {
                    // Download the Web resource and save it into the current filesystem folder.
                    //MessageBox.Show(file_url);
                    myWebClient.DownloadFile(file_url, "POS.zip");
                    KillProcess(POS_EXE.Replace(".exe", ""));
                    Unzip("POS.zip", ExtractExistingFileAction.OverwriteSilently);
                    if (File.Exists("POS.zip"))
                    {
                        File.Delete("POS.zip");
                    }
                }
            }
            catch (Exception exc)
            {
                string error_message = exc.Message;
                if (exc.InnerException != null && exc.InnerException.Message.Length > 0)
                {
                    error_message += Environment.NewLine + exc.InnerException.Message;
                }
                Logger.Error(error_message);
                throw;
            }
        }

        private static void Unzip(string file, ExtractExistingFileAction defaultAction, string TargetDir = ".")
        {
            using (ZipFile zip = ZipFile.Read(file))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(TargetDir, defaultAction);
                }
            }
        }

        private void KillProcess(string process)
        {
            try
            {
                Process[] processes = Process.GetProcesses();//Process.GetProcessesByName(process);
                foreach (Process proc in processes)
                {
                    if (proc.ProcessName.Contains(process) && proc.ProcessName.ToUpper().Contains("ITS.POS.CLIENT.LOADER") == false)
                    {
                        proc.Kill();
                    }
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(ex.Message.ToString());
                Logger.Error(exc.Message);
                if (exc.InnerException != null && exc.InnerException.Message != null && exc.InnerException.Message.Length > 0)
                {
                    Logger.Error(exc.InnerException.Message);
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            LoaderProcess();
        }

    }
}
