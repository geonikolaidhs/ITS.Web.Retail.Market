using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace UpdaterDesktop
{
    public partial class frmUpdate : Form
    {
        public frmUpdate()
        {
            InitializeComponent();
        }

        private void lblMessage_TextChanged(object sender, EventArgs e)
        {
            txtLog.Text = txtLog.Text + Environment.NewLine + lblMessage.Text;
        }

        private void frmUpdate_Activated(object sender, EventArgs e)
        {
            BeginUpdate();
        }

        private bool CreateBackup(ApplicationSettingsForUpdater applicationUpdaterSettings, String physical_path)
        {
            try
            {
                //backing up configurations
                if (applicationUpdaterSettings.filesTobeBackedUp.Length > 0)
                {
                    lblMessage.Text = "Backing up configurations";
                    foreach (String filetoSave in applicationUpdaterSettings.filesTobeBackedUp)
                    {
                        if (File.Exists(filetoSave))
                        {
                            String backupFile = physical_path + "\\" + Path.GetFileName(filetoSave);
                            if (File.Exists(backupFile))
                            {
                                File.Move(backupFile, backupFile + "." + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Date + "." +
                                    DateTime.Now.Hour + "." + DateTime.Now.Minute);
                            }
                            File.Copy(filetoSave, backupFile);
                        }
                        else
                        {
                            lblMessage.Text = filetoSave + " does not exist";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void BeginUpdate()
        {
            //TODO to become parametric parameters=?

            ApplicationSettingsForUpdater applicationUpdaterSettings = new ApplicationSettingsForUpdater();
            //TODO ReadSettings

            Application.DoEvents();
            lblMessage.Text = "Starting Update Process";
            Application.DoEvents();
            lblMessage.Text = "Getting Filelist....";           
            Application.DoEvents();
            MobileWebService.MobileWebService ws = new MobileWebService.MobileWebService();

            String physpath = Environment.GetEnvironmentVariable("TMP")+"\\its";
            Directory.CreateDirectory(physpath);

            //TODO backup all files in applicationUpdaterSettings.filesTobeBackedUp
            if(!CreateBackup(applicationUpdaterSettings,physpath)){
                MessageBox.Show("Could not complete backup!");
            }

            String[][] filelist = ws.GetDestkopFilelist();

            foreach (String[] fileArray in filelist)
            {
                String filename = fileArray[0];
                lblMessage.Text = "Examination of " + filename;
                String fileHash = fileArray[1];
                String fileUrl = fileArray[2];
                String localFileHash = GetFileHash(physpath + "\\" + filename);

                if (localFileHash != fileHash)
                {
                    lblMessage.Text = "Downloading " + filename;
                    if (File.Exists(physpath + "\\" + filename))
                        File.Delete(physpath + "\\" + filename);
                    if (DownloadFile(fileUrl, (physpath + "\\" + filename)))
                    {
                        lblMessage.Text = filename + " succesfully downloaded.";
                    }
                    else
                    {
                        lblMessage.Text = filename + " has not been downloaded succesfully.";
                    }
                }
                else
                {
                    lblMessage.Text = filename + " is up to date";
                }
            }

            //uninstall previous version
            lblMessage.Text = "Uninstalling previous version";
            using (Process uninstall = Process.Start("msiexec", "/uninstall \"{1A00046E-55AB-4B42-A0B8-F61B200FA4F6}\" /passive"))
            {
                uninstall.WaitForExit();
            }
            //install new version
            lblMessage.Text = "Installing New version";
            using (Process install = Process.Start("msiexec", "/i \"" + physpath + "\\Mobile@StoreDesktop.msi\" /passive"))
            {
                install.WaitForExit();
            }

            lblMessage.Text = "Closing updater and starting up App";
            //System.Diagnostics.Process.Start("\""+Environment.GetEnvironmentVariable("ProgramFiles(x86)")+"\\ITS S.A\\Mobile@Store\\Desktop\\Mobile@store.exe\"", "");
            System.Diagnostics.Process.Start("\"" + applicationUpdaterSettings.installation_path+"\\"+applicationUpdaterSettings.executableAfterInstall+"\"","");
            this.Close();
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
                request.Timeout = 100000; // 100 seconds

                response = request.GetResponse();
                pbProgress.Minimum = 0;
                pbProgress.Maximum = unchecked((int)response.ContentLength);

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
                    pbProgress.Value = totalBytesRead;
                    fileStream.Write(buffer, 0, bytesRead);
                }

                // we got to this point with no exception. Ok.
                success = true;
            }
            catch (Exception)
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

        protected String GetFileHash(String file)
        {
            String strResult = "";
            //String strHashData = "";

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
                catch (Exception )
                {
                    return "-1";
                }
            }

            return (strResult);
        }

    }
}
