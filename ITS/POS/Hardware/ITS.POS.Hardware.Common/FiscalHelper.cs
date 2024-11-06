using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public static class FiscalHelper
    {

        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_GetStat([MarshalAs(UnmanagedType.AnsiBStr)] ref string strStat);

        [DllImport("DocMsign.DLL")]
        public static extern Int16 Upload_STXT_FILE(ref int ercode, string Server, int Port, string pathfilename, string UfileName, string password);

        public static void UploadCurrentZFile(string url, string aesKey, string abcFolder, string serialNumber, out string uploadZErrorMessage)
        {
            short uploadResult = 0;
            int uploadError = 0;
            uploadZErrorMessage = string.Empty;
            //uploadResult = Upload_STXT_FILE(ref uploadError, url, 80,
            //                        @"C:\BBox\DLD77000013\0068\DLD7700001319022723180068_s.txt", @"DLD7700001319022723180068_s.txt", aesKey);

            uploadResult = Upload_STXT_FILE(ref uploadError, url, 80, "", "", aesKey);
            if (uploadResult == 0 && uploadError != 0)
            {
                UploadMissingZFiles(url, aesKey, uploadError, abcFolder, serialNumber);
            }
            ConfirmZUploaded(url, aesKey, out uploadZErrorMessage);
        }


        public static void ConfirmZUploaded(string url, string aesKey, out string uploadZErrorMessage)
        {
            int uploadError = 0;
            uploadZErrorMessage = string.Empty;
            short uploadResult = Upload_STXT_FILE(ref uploadError, url, 80, "", "", aesKey);
            if (uploadResult == 18 || (uploadResult == 0 && uploadError == 0))
            {
                uploadZErrorMessage = "SUCCESS";
            }
            else
            {
                uploadZErrorMessage = POS.Resources.POSClientResources.ERRORZUPLOAD + " Error Code : " + uploadResult.ToString() + " Extra Info : " + uploadError.ToString();
            }
        }

        public static FileInfo Get_S_File(string basepath, int lastZUploaded, string serialNumber)
        {
            FileInfo result = null;
            try
            {
                if (Directory.Exists(basepath))
                {
                    string searchDir = Path.Combine(basepath, serialNumber);
                    lastZUploaded++;
                    string searchPattern = lastZUploaded.ToString().PadLeft(4, '0');
                    List<string> directories = Directory.GetDirectories(searchDir, searchPattern, SearchOption.TopDirectoryOnly).ToList();
                    if (directories != null && directories.Count == 1)
                    {
                        DirectoryInfo dir = new DirectoryInfo(directories.FirstOrDefault());
                        result = dir.GetFiles("*_s.txt")?.ToList()?.Where(x => x.Name.Length > 27).FirstOrDefault();
                    }
                }
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                return result;
            }
        }

        private static void UploadMissingZFiles(string url, string aesKey, int lastSuccessZUpload, string abcFolder, string serialNumber)
        {
            /// Βρίσκουμε το πιο πρόσφατο Ζ που έχει εκδόσει ο μηχανισμός από την CVB_FSL_GetStat
            int latestZNumber = 0;
            string stats = string.Empty;
            short getStatResult = CVB_FSL_GetStat(ref stats);
            if (getStatResult == 0)
            {
                string[] strArray = stats.Split(' ');
                if (strArray != null && strArray.Length > 1)
                {
                    if (int.TryParse(strArray[1], out latestZNumber))
                    {
                        if (latestZNumber > 0)
                        {

                            int maxTries = latestZNumber - lastSuccessZUpload;
                            ///Για κάθε ένα Ζ απο το τελευταίο που έχει γίνει Upload μέχρι το πιο πρόσφατο που έχει εκδόσει ο μηχανισμός
                            ///προσπαθούμε να κάνουμε Upload
                            while (lastSuccessZUpload <= latestZNumber && maxTries > 0)
                            {
                                FileInfo fileTosend = FiscalHelper.Get_S_File(abcFolder, lastSuccessZUpload, serialNumber);
                                if (fileTosend != null)
                                {
                                    int tries = 0;
                                    bool success = false;
                                    while (tries < 3 && !success)
                                    {
                                        try
                                        {
                                            int extraError = 0;
                                            short res = Upload_STXT_FILE(ref extraError, url, 80, fileTosend.FullName, fileTosend.Name, aesKey);
                                            success = ((res == 0 && lastSuccessZUpload == 0) || (res == 18)) ? true : false;
                                            tries++;
                                        }
                                        catch (Exception ex)
                                        {
                                            tries++;
                                        }
                                    }
                                }
                                lastSuccessZUpload++;
                                maxTries--;
                            }
                        }
                    }
                }
            }
        }


        public static void SendMail(string mailList, bool success, string smtpserver, string username, string password, string port)
        {

            if (string.IsNullOrEmpty(mailList))
            {
                return;
            }
            List<string> emails = new List<string>();

            if (mailList.Contains(','))
            {
                emails = mailList.Split(',').ToList();
            }
            else
            {
                emails.Add(mailList);
            }
            int smtpPort = int.TryParse(port, out smtpPort) ? smtpPort : 25;
            if (emails.Count > 0)
            {
                foreach (string emailAddress in emails)
                {
                    try
                    {
                        MailMessage mail = new MailMessage(username, emailAddress);
                        SmtpClient client = new SmtpClient();
                        client.Port = smtpPort;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Host = smtpserver;
                        client.Credentials = new System.Net.NetworkCredential(username, password);
                        string mes = success ? " Upload Successfully " : "Failed to Upload";
                        mail.Subject = DateTime.Now.Date.ToString() + " Z  " + mes;
                        mail.Body = mail.Subject;
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}
