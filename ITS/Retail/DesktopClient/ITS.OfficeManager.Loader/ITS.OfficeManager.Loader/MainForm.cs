using Ionic.Zip;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
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

namespace ITS.OfficeManager.Loader
{
    public partial class MainForm : Form
    {

        //WIN32 API Calls
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

        //WIN32 API Calls end

        public const string CONFIGURATION_FOLDER = "\\ITS\\StoreController";
        public const string CONFIGURATION_FILE = "\\settings.xml";
        private const string WEB_SERVICE_FILE = "/POSUpdateService.asmx";//Προσοχή στο πρώτο '/'!!!
        private const string OM_EXE = "ITS.Office.Manager.exe";
        private const string OM_ZIP = "officemanager.zip";
        private const string OM_LOADER_ZIP = "officemanagerloader.zip";
        private const string OM_RELATIVE_URL = "/POS/";
        private const string BATCH_FILE = "loader.bat";


        private const string LOG_FOLDER = ".\\Log";

        public LogHelper Logger { get; private set; }

        StoreControllerClientSettings Settings = null;
        public MainForm()
        {
            InitializeComponent();
            if (!Directory.Exists(LOG_FOLDER))
            {
                Directory.CreateDirectory(LOG_FOLDER);
            }
            string fileName = String.Format("{0}\\{1:yyyy-MM-dd_hh-mm-ss-tt}.txt", LOG_FOLDER, DateTime.Now);
            Logger = new LogHelper(rtxtbxLog, fileName);
            this.Text += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                Logger.Message("Starting Office Manager Loader");
                //Load Settings
                LoadSettings();

                //Check Loader Version
                Logger.Message("Determining approprate Office Manager Loader Version.");
                Version remoteVersion = GetRemoteVersion();
                if (!CheckLoaderVersion(remoteVersion))
                {                   
                    Logger.Error("Application Halted.");
                    return;
                }

                //Check OM Version
                if (!CheckVersion(remoteVersion))
                {                   
                    Logger.Error("Application Halted.");
                    return;
                }


                StartOM();
            }
            catch(Exception ex)
            {
                Logger.Error("Unhandled Exception:" + ex.Message);
                Logger.Error("Application Halted.");
            }
        }

        private void StartOM()
        {
            try
            {
                Process proc = System.Diagnostics.Process.Start(OM_EXE, "");
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
                //Logger.Error(error_message);
            }
        }

        private void LoadSettings()
        {
            Logger.Message("Reading Settings");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + CONFIGURATION_FOLDER;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string SettingsFilePath = path + CONFIGURATION_FILE;
            Settings = new StoreControllerClientSettings(SettingsFilePath, false);
            Settings.Load();
            if (string.IsNullOrWhiteSpace(Settings.StoreControllerURL))
            {
                Logger.Warning("Settings Not Found. Please Provide StoreController URL");
                AskForStoreControllerURL();
                
                Settings.Save();
                Logger.Message("Settings has been saved successfully");
            }

        }

        private void AskForStoreControllerURL()
        {
            using (SettingsForm frmSettings = new SettingsForm(this.Settings, this.Logger))
            {
                frmSettings.Location = new Point(Left + (Width - frmSettings.Width) / 2, this.Top);
                frmSettings.ShowDialog();
            }
        }

        private void UpdateLoaderAndReload()
        {
            string loaderUrl = Settings.StoreControllerURL + OM_RELATIVE_URL + OM_LOADER_ZIP;
            string loaderLocalFile = ".\\" + OM_LOADER_ZIP;
            string unzipLocation = ".\\NewLoader";
            if (!Directory.Exists(unzipLocation))
            {
                Directory.CreateDirectory(unzipLocation);
            }
            Logger.Message("Downloading new Office Manager Loader...");
            DownloadAndUnzip(loaderUrl, loaderLocalFile, unzipLocation, true, true);
            Logger.Message("Restarting to new Office Manager Loader . . .");
            DeleteFile(BATCH_FILE);
            string new_line = Environment.NewLine;
            string bat_commands = "@ECHO OFF" + new_line +
                                   "set LOADER_TIMES=0" + new_line +
                                   "set LOADER_MAX_TIMES=150" + new_line +
                                   ":START" + new_line +
                                   "ping 192.0.2.2 -n 1 -w 100 > nul" + new_line +
                                   "tasklist /fi \"IMAGENAME EQ ITS.OFFICEMANAGER.LOADER.EXE\" | find /i /n \"ITS.OFFICEMANAGER.LOADER.EXE\" > nul" + new_line +
                                   "set /a LOADER_TIMES+=1" + new_line +
                                   "if %LOADER_TIMES% GTR  %LOADER_MAX_TIMES% GOTO ERR" + new_line +
                                   "if %ERRORLEVEL% equ 0 goto START" + new_line +
                                   "rem ECHO \"FINISH\"" + new_line +
                                   "copy /y NewLoader\\*.*" + new_line +
                                   "start ITS.OfficeManager.Loader.exe" + new_line +
                                   "GOTO END" + new_line +
                                   ":ERR" + new_line +
                                   "ECHO \"REACHED MAX TIMES\"" + new_line +
                                   ":END" + new_line;
            File.WriteAllText(BATCH_FILE, bat_commands);
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

        private void DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private void UpdateOMVersion()
        {
            string omUrl = Settings.StoreControllerURL + OM_RELATIVE_URL + OM_ZIP;
            string omLocalFile = ".\\" + OM_ZIP;
            string unzipLocation = ".\\";
            DownloadAndUnzip(omUrl, omLocalFile, unzipLocation, true, true);
        }


        private bool CheckLoaderVersion(Version remoteVersion)
        {
            Version loaderVersion = Assembly.GetEntryAssembly().GetName().Version;
            int comparison = loaderVersion.CompareTo(remoteVersion);
            if (comparison < 0)
            {
                //Remote Version is Bigger
                Logger.Message("Office Manager Loader should be updated.");
                UpdateLoaderAndReload();
            }
            else if (comparison == 0)
            {
                Logger.Message("Office Manager Loader is up to date.");
                return true;
            }
            Logger.Error(string.Format("Current Version ({0}) Greater than remote {1}. Please contact support.", loaderVersion.ToString(), remoteVersion.ToString()));
            return false;
        }



        private bool CheckVersion(Version remoteVersion)
        {
            Version OMVersion = new Version(0, 0, 0, 0);
            if (File.Exists(OM_EXE))
            {
                OMVersion = AssemblyName.GetAssemblyName(OM_EXE).Version;
            }
            int comparison = OMVersion.CompareTo(remoteVersion);
            if (comparison < 0)
            {
                //Remote Version is Bigger
                Logger.Message("Office Manager should be updated.");
                UpdateOMVersion();
                return true;
            }
            else if (comparison == 0)
            {
                Logger.Message("Office Manager is up to date.");
                return true;
            }
            Logger.Error(string.Format("Current Version ({0}) is greater than remote {1}. Please contact support.", OMVersion.ToString(), remoteVersion.ToString()));
            return false;
        }



        private Version GetRemoteVersion()
        {
            try
            {
                string web_service_url = Settings.StoreControllerURL + WEB_SERVICE_FILE;
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
                throw;
            }

        }

        //Download and unzip
        private void DownloadAndUnzip(string fileUrl, string saveFileLocation, string unzipLocation, bool unzip = true, bool delete = true)
        {
            using (WebClient myWebClient = new WebClient())
            {
                try
                {
                    // Download the Web resource and save it into the current filesystem folder.
                    myWebClient.DownloadFile(fileUrl, saveFileLocation);
                    if (unzip)
                    {
                        Logger.Message("Decompressing....");
                        Unzip(saveFileLocation, ExtractExistingFileAction.OverwriteSilently, unzipLocation);
                        Logger.Message("Completed.");
                        if (delete && File.Exists(saveFileLocation))
                        {
                            File.Delete(saveFileLocation);
                        }
                    }
                }
                catch (Exception exception)
                {
                    string exceptionMessage = exception.Message;
                }
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
    }
}
