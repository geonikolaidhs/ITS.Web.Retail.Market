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
using ITS.Mobile.Updater.WRMMobileAtStore;

namespace ITS.Mobile.Updater
{
    public partial class Data : Form
    {
        public Data()
        {

            InitializeComponent();
        }

        private void Data_Load(object sender, EventArgs e)
        {
            this.Location = new Point(
            Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2,
            Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2);
            
        }

        private WRMMobileAtStore.WRMMobileAtStore GetWebservice()
        {
            WRMMobileAtStore.WRMMobileAtStore service = new WRMMobileAtStore.WRMMobileAtStore();
            string url = string.Format(@"http://{0}/WRMMobileAtStore.svc", AppSettings.ServerIP); 
            service.Url = url;
            service.Timeout = 10000;
            return service;

        }

        protected String GetFileHash(String file)
        {
            String strResult = "";

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
                    string exceptionError = ex.Message + "\r\n" + ex.StackTrace;
                    return "-1";
                }
            }

            return (strResult);
        }
        string updaterPath, storagePath;
        private bool StartUpdate()
        {
            try
            {
                updaterPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                storagePath = updaterPath.Substring(0, updaterPath.ToLower().IndexOf("\\updater"));
                Application.DoEvents();
                lblMessage.Text = "Getting Filelist...";
                WRMMobileAtStore.WRMMobileAtStore ws = GetWebservice();
                String[][] filelist = ws.GetMobileFilelist(AppSettings.Terminal.ID,true,AppSettings.Terminal.IP);
                                
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
                    {
                        lblMessage.Text = filename + " is up to date";
                    }
                    
                }
                //backing up configurations
                lblMessage.Text = "Backing up configurations";
                if (File.Exists(updaterPath + "\\dataLoggerConfig.xml"))
                {
                    File.Delete(updaterPath + "\\dataLoggerConfig.xml");
                }
                File.Copy(AppSettings.configurationLocation + "\\dataLoggerConfig.xml", updaterPath + "\\dataLoggerConfig.xml");
                try
                {                    
                    if (File.Exists(updaterPath + "\\TerminalSettings.xml"))
                    {
                        File.Delete(updaterPath + "\\TerminalSettings.xml");
                    }
                    File.Copy(AppSettings.configurationLocation + "\\TerminalSettings.xml", updaterPath + "\\TerminalSettings.xml");
                }
                catch
                {
                }
                //uninstall mobile@store
                lblMessage.Text = "Uninstalling previous version";
                using (Process uninstall = Process.Start("unload", "ITS S.A. WRM Mobile@Store"))
                {
                    uninstall.WaitForExit();
                    lblMessage.Text = "Uninstalling completed " + ((uninstall.ExitCode != 0) ? "unsuccessfully. Please revise the log for detailed information" : "sucessfully");
                }
                //install new version
                lblMessage.Text = "Installing New version";
                using (Process install = Process.Start("wceload", "/noui /delete 0 \"" + storagePath + "\\ITS.MobileAtStore.cab\""))
                {
                    install.WaitForExit();
                    lblMessage.Text = "Installation completed " + ((install.ExitCode != 0) ? "unsuccessfully. Please revise the log for detailed information" : "sucessfully");
                }

                lblMessage.Text = "Restoring configurations";
                File.Delete(AppSettings.configurationLocation + "\\dataLoggerConfig.xml");
                File.Copy(updaterPath+"\\dataLoggerConfig.xml", AppSettings.configurationLocation + "\\dataLoggerConfig.xml");                
                File.Delete(updaterPath + "\\dataLoggerConfig.xml");

                try
                {
                    File.Delete(AppSettings.configurationLocation + "\\TerminalSettings.xml");
                    File.Copy(updaterPath + "\\TerminalSettings.xml", AppSettings.configurationLocation + "\\TerminalSettings.xml");
                    File.Delete(updaterPath + "\\TerminalSettings.xml");
                }
                catch 
                { 
                }

                lblMessage.Text = "Closing updater and starting up Mobile@store";
                System.Diagnostics.Process.Start(AppSettings.configurationLocation + "\\Mobile@store.exe","");
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
                request.Timeout = 100000; // 100 seconds

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
                string exceptionError = exp.Message + "\r\n" + exp.StackTrace;
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