using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Threading;

namespace Updater
{
    public partial class Data : Form
    {
        public Data()
        {

            InitializeComponent();
        }

        ApplicationSettingsForUpdater settings;

        private void Data_Load(object sender, EventArgs e)
        {
            this.Location = new Point(
            Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2,
            Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2);

            
            using (StreamReader reader = new StreamReader(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)+"\\updater.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ApplicationSettingsForUpdater));

                var settingso = ser.Deserialize(reader);
                settings = settingso as ApplicationSettingsForUpdater;
                reader.Close();
            }
        }


        protected String GetFileHash(String file)
        {
            String strResult = "";
            String strHashData = "";

            byte[] arrbytHashValue;
            FileStream oFileStream = null;

            using (MD5CryptoServiceProvider oMD5Hasher = new MD5CryptoServiceProvider())
            {
                try
                {
                    oFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                    oFileStream.Close();
                    strResult = System.BitConverter.ToString(arrbytHashValue);
                }
                catch (System.Exception ex)
                {
                    return "-1";
                }
            }

            return (strResult);
        }
        string updaterPath, storagePath;
        private bool StartUpdate()
        {
            string applicationName = settings.applicationName;
                //"ITS S.A. Mobile@Store";
            string cabFile = settings.installationProcedure[0];
                //"RetailMobile.CAB";
            string applicationExecutableFile = settings.executableAfterInstall;

                //"RetailMobile.exe";


            try
            {
                updaterPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                storagePath = updaterPath.Substring(0, updaterPath.ToLower().IndexOf("\\updater"));
                Application.DoEvents();
                lblMessage.Text = "Getting Filelist...";

                String[][] filelist = settings.mobileFileListToDownload;

                foreach (String[] fileArray in filelist)
                {
                    String filename = fileArray[0];
                    lblMessage.Text = "Examination of " + filename;
                    String fileHash = fileArray[1];
                    String fileUrl = fileArray[2];
                    String localFileHash = GetFileHash(storagePath + "\\" + filename);

                    if (localFileHash != fileHash)
                    {
                        lblMessage.Text = "Downloading " + filename;
                        if (File.Exists(storagePath + "\\" + filename))
                            File.Delete(storagePath + "\\" + filename);
                        if (DownloadFile(fileUrl, (storagePath + "\\" + filename)))
                        {
                            lblMessage.Text = filename + " succesfully downloaded.";
                        }
                        else
                        {
                            lblMessage.Text = filename + " has not been downloaded succesfully.";
                        }
                    }
                    else
                        lblMessage.Text = filename + " is up to date";
                }


                lblMessage.Text = "Backing up configurations";

                String [] filesToBePreserved = settings.filesTobeBackedUp;
                foreach (string fileToPreserve in filesToBePreserved)
                {
                    string fileName = System.IO.Path.GetFileName(fileToPreserve);

                    string fileCopyPath = updaterPath + "\\" + fileName;
                    if (File.Exists(fileCopyPath))
                    {
                        File.Delete(fileCopyPath);
                    }
                    //string originalFilePath = AppSettings.configurationLocation + "\\" + fileToPreserve;
                    File.Copy(fileToPreserve, fileCopyPath);
                }


                lblMessage.Text = "Uninstalling previous version";
                using (Process uninstall = Process.Start("unload", applicationName))
                {
                    uninstall.WaitForExit();
                    lblMessage.Text = "Uninstalling completed " + ((uninstall.ExitCode != 0) ? "unsuccessfully. Please revise the log for detailed information" : "sucessfully");
                }

                lblMessage.Text = "Installing New version";
                string instruction = "/noui /delete 0 " + storagePath + "\\" + cabFile;
                using (Process install = Process.Start("wceload", instruction))
                {
                    install.WaitForExit();
                    lblMessage.Text = "Installation completed " + ((install.ExitCode != 0) ? "unsuccessfully. Please revise the log for detailed information" : "sucessfully");
                }

                lblMessage.Text = "Restoring configurations";
                foreach (string oldFile in filesToBePreserved)
                {
                    //string oldFile = AppSettings.configurationLocation + "\\" + fileName;
                    string fileName = System.IO.Path.GetFileName(oldFile);
                    string fileToBeRestored = updaterPath + "\\" + fileName;
                    File.Delete(oldFile);
                    File.Copy(fileToBeRestored, oldFile);
                    File.Delete(fileToBeRestored);
                    lblMessage.Text = "Closing updater and starting up Application";
                }


                System.Diagnostics.Process.Start(settings.executableAfterInstall, "");
                this.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Exception Occured:"+ex.Message;
                return false;
            }
            return true;
        }


        private bool DownloadFile(string url, string destination)
        {
            bool success = false;

            System.Net.HttpWebRequest request = null;
            System.Net.WebResponse response = null;
            Stream responseStream = null;
            FileStream fileStream = null;

            try
            {
                request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = Timeout.Infinite; // 100 seconds

                response = request.GetResponse();
                pbProcess.Minimum = 0;
                pbProcess.Maximum = unchecked((int)response.ContentLength);

                responseStream = response.GetResponseStream();


                fileStream = File.Open(destination, FileMode.Create, FileAccess.Write, FileShare.None);

                // read up to 400 kilobytes at a time
                int maxRead = 102400;
                byte[] buffer = new byte[maxRead];
                int bytesRead = 0;
                int totalBytesRead = 0;

                // loop until no data is returned
                while ((bytesRead = responseStream.Read(buffer, 0, maxRead)) > 0)
                {

                    totalBytesRead += bytesRead;
                    pbProcess.Value = totalBytesRead;
                    fileStream.Write(buffer, 0, bytesRead);
                }

                // we got to this point with no exception. Ok.
                success = true;
            }
            catch (Exception exp)
            {
                // something went terribly wrong.
                success = false;
            }
            finally
            {
                // cleanup all potentially open streams.

                if (null != responseStream)
                    responseStream.Close();
                if (null != response)
                    response.Close();
                if (null != fileStream)
                    fileStream.Close();
            }

            // if part of the file was written and the transfer failed, delete the partial file
            if (!success && File.Exists(destination))
                File.Delete(destination);

            return success;
        }

        private void lblMessage_TextChanged(object sender, EventArgs e)
        {
            txtLog.Focus();
            txtLog.Text = txtLog.Text + "\r\n" + lblMessage.Text;
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            Application.DoEvents();
        }

        private bool updatestarted = false;
        private void Data_Activated(object sender, EventArgs e)
        {
            if (!updatestarted)
            {
                updatestarted = true;
                this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;
                Application.DoEvents();
                StartUpdate();
                this.Close();                
            }
        }

        private void Data_Closing(object sender, CancelEventArgs e)
        {

        
            using (StreamWriter logfile = new StreamWriter(updaterPath+"\\updaterlog.txt", true))
            {
                logfile.WriteLine("Updater log");
                logfile.WriteLine("-----------");
                logfile.WriteLine(txtLog.Text);
            }
        }

    }
}