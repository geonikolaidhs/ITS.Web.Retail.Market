using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;
using System.IO;
using System.Threading;
using ITS.POS.Fiscal.Common;
using System.Security;


namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Represents an RBS EAFDSS device.
    /// </summary>
    public class RBSSign : Device
    {

        [DllImport("DocMsign.DLL")]
        public static extern Int16 VB_FSL_DisableCheckZFiles(int en);

        [DllImport("DocMsign.DLL")]
        public static extern Int16 FSL_Command(string strBase, string strCmd, string strDevFile, string strP1, string strP2);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 RET_FSL_SignDocument(string VBstrInfile, [In, Out, MarshalAs(UnmanagedType.AnsiBStr)] ref string Sign, [In, Out, MarshalAs(UnmanagedType.AnsiBStr)] ref string FilePath);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_SignDocument(string VBstrInfile, [MarshalAs(UnmanagedType.AnsiBStr)] ref string Sign);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 N_FSL_SignDocument(string VBstrInfile, [MarshalAs(UnmanagedType.AnsiBStr)] ref string Sign, [MarshalAs(UnmanagedType.AnsiBStr)] ref string Sign2);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_SelectDevice(string SerialNo, byte TType, string IPP, string VBstrBaseDir, byte port);

        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_IssueZreport();
        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_ReleaseDevice();
        [DllImport("DocMsign.DLL")]
        public static extern void VB_FSL_SetProgress(int fEnable);
        [DllImport("DocMsign.DLL")]
        public static extern void FSL_SetDebug(long fnDebug, byte strLogFilename, Boolean fDebugEnable);
        [DllImport("DocMsign.DLL")]
        public static extern void VB_FSL_SetDebug(int fnDebug);
        [DllImport("DocMsign.DLL")]
        public static extern void VB_FSL_ErrorToString(int iRet, [MarshalAs(UnmanagedType.AnsiBStr)] ref string strDescription, int ln);
        [DllImport("DocMsign.DLL")]
        public static extern void VB_FSL_ErrorsUI(int st);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 FSL_SetLanguage(int st);  //'For greek = 0 
        [DllImport("DocMsign.DLL")]
        public static extern void FSL_SetBackup(string bk);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_PopupControl();


        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_InvData(string AFM_publisher, string AFMRecipient, string CustomerID, string InvoiceType, string Seira, string InvoiceNo, string NET_A, string NET_B, string NET_C, string NET_D, string NET_E, string VAT_A, string VAT_B, string VAT_C, string VAT_D, string Total, string Currency, [In, Out, MarshalAs(UnmanagedType.AnsiBStr)] ref string Signature);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 Upload_STXT_FILE(ref int ercode, string Server, int Port, string pathfilename, string UfileName, string password);
        [DllImport("DocMsign.DLL")]
        public static extern void SHA1_GetSignature(string data, [In, Out, MarshalAs(UnmanagedType.AnsiBStr)] ref string Signature20, [In, Out, MarshalAs(UnmanagedType.AnsiBStr)] ref string Signature40);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 RegisterDevice(string key);


        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_GetStat([MarshalAs(UnmanagedType.AnsiBStr)] ref string strStat);

        [DllImport("DocMsign.DLL")]
        public static extern Int16 CVB_FSL_GetSerialNo([MarshalAs(UnmanagedType.AnsiBStr)] ref string strsno);


        [DllImport("DocMsign.DLL")]
        public static extern Int16 CRegisterDevice(string key, int pos);
        [DllImport("DocMsign.DLL")]
        public static extern Int16 CReadRegisteredDevice([MarshalAs(UnmanagedType.AnsiBStr)] ref string key, int pos);

        [DllImport("DocMsign.DLL")]
        public static extern void N_FSL_Get_E_Range(string path, string DateStart, string DateEnd, string ZnoStart, string ZnoEnd, int type);


        public override eDeviceCheckResult CheckDevice(out string message)
        {
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        protected byte COMPortByte { get; set; }

        /// <summary>
        /// Only ConnectionType.Ethernet and Emulated is supported.
        /// </summary>
        /// <param name="conType"></param>
        public RBSSign(ConnectionType conType, string deviceName, FiscalServiceSettings fiscalServiceSettings)
        {
            ConType = conType;
            DeviceName = deviceName;
            FiscalServiceSettings = fiscalServiceSettings;
            DeviceInitiated = false;
        }

        public String RegistrationNumber { get; set; }
        protected bool DeviceInitiated { get; set; }

        public override void AfterLoad(List<Device> devices)
        {
            LogTrace(this.GetType().Name + " AfterLoad started");
            base.AfterLoad(devices);
            LogTrace(this.GetType().Name + " base AfterLoad completed. Connection setup start.");
            if (this.ConType == ConnectionType.COM)
            {
                byte b;
                if (byte.TryParse(this.Settings.COM.PortName.ToUpper().Replace("COM", ""), out b) == false)
                {
                    throw new POSException("Incorrect COM");
                }
                this.COMPortByte = b;
            }
        }

        private FiscalServiceSettings FiscalServiceSettings { get; set; }

        public enum RBSSignResult
        {
            ERR_SUCCESS,
            ERR_BADARGUMENT,
            ERR_FILEIO_ERROR,
            ERR_COMMUNICATION_ERROR,
            ERR_UNRECOVERABLE_ERROR,
            ERR_UNEXPECTED_ERROR,
            ERR_INVALIDDEVICEID,
            ERR_INVALIDBASEDIR,
            ERR_DEVFILELOAD_ERR,
            ERR_NOTSELECTED,
            ERR_INVALIDINPUTFILE,
            ERR_DEVICE_NOT_FOUND,
            ERR_NETSYS_FAILED,
            ERR_SERVER_UNAVAILABLE,
            ERR_SERVER_COM_ERROR,
            ERR_NO_RESOURCES,
            ERR_INVALID_S_FILENAME,
            ERR_INVALID_SERIALNUMBER,
            ERR_Z_ALREADY_POSTED,
            ERR_PREVIOUS_Z_MISSED,
            ERR_IV_LENGTH,
            ERR_IPASSWORD_LENGTH,
            INVALID_PROPERTY,
            UNSUPPORTED_CONNECTION_TYPE,
        }

        public static string GetResultInfo(RBSSignResult rbsSignRsult)
        {
            switch (rbsSignRsult)
            {
                case RBSSignResult.ERR_SUCCESS:
                    return "No errors, success";
                case RBSSignResult.ERR_BADARGUMENT:
                    return "Bad parameter specified";
                case RBSSignResult.ERR_FILEIO_ERROR:
                    return "Error in file/filesystem operation";
                case RBSSignResult.ERR_COMMUNICATION_ERROR:
                    return "Communication with device failed";
                case RBSSignResult.ERR_UNRECOVERABLE_ERROR:
                    return "Hardware error: device needs service";
                case RBSSignResult.ERR_UNEXPECTED_ERROR:
                    return "Unexpected error, operation aborted";
                case RBSSignResult.ERR_INVALIDDEVICEID:
                    return "Serial number is not the one expected";
                case RBSSignResult.ERR_INVALIDBASEDIR:
                    return "Base directory specified is invalid";
                case RBSSignResult.ERR_DEVFILELOAD_ERR:
                    return "Failed loading device descriptor file";
                case RBSSignResult.ERR_NOTSELECTED:
                    return "Device specified is not selected";
                case RBSSignResult.ERR_INVALIDINPUTFILE:
                    return "File contains invalid characters or file size is invalid.";

                //(added since 2.0.0.1a)
                case RBSSignResult.ERR_DEVICE_NOT_FOUND:
                    return "The device specified is not connected";
                case RBSSignResult.ERR_NETSYS_FAILED:
                    return "Network I/O error";
                case RBSSignResult.ERR_SERVER_UNAVAILABLE:
                    return "Failure contacting proxy server";
                case RBSSignResult.ERR_SERVER_COM_ERROR:
                    return "Error communicating with proxy server";

                //(added since 2.0.0.2a)
                case RBSSignResult.ERR_NO_RESOURCES:
                    return "Error allocating system resources";

                //(added since 4.0.0.0)
                case RBSSignResult.ERR_INVALID_S_FILENAME:
                    return "Invalid S filename";
                case RBSSignResult.ERR_INVALID_SERIALNUMBER:
                    return "Invalid esd serial number that is been posted on server";
                case RBSSignResult.ERR_Z_ALREADY_POSTED:
                    return "The z number of the s file has already posted";
                case RBSSignResult.ERR_PREVIOUS_Z_MISSED:
                    return "The previous Z number isn't posted yet";
                case RBSSignResult.ERR_IV_LENGTH:
                    return "incorrect IV length";
                case RBSSignResult.ERR_IPASSWORD_LENGTH:
                    return "incorrect Password length";

                //??? Not on manual
                case RBSSignResult.INVALID_PROPERTY:
                case RBSSignResult.UNSUPPORTED_CONNECTION_TYPE:
                default:
                    return rbsSignRsult.ToString();
            }
        }

        /// <summary>
        /// Signs a document and returns the exit code.
        /// </summary>
        /// <param name="ABCDirectory">The ABC Directory</param>
        /// <param name="fileToSign">The path to the file to sign</param>
        /// <param name="signature">The electronic signature</param>
        /// <returns>Result:
        ///   0       ERR_SUCCESS                     No errors, success
        ///   1       ERR_BADARGUMENT                 Bad parameter specified
        ///   2       ERR_FILEIO_ERROR                Error in file/filesystem operation
        ///   3       ERR_COMMUNICATION_ERROR         Communication with device failed
        ///   4       ERR_UNRECOVERABLE_ERROR         Hardware error: device needs service
        ///   5       ERR_UNEXPECTED_ERROR            Unexpected error, operation aborted
        ///   6       ERR_INVALIDDEVICEID             Serial number is not the one expected
        ///   7       ERR_INVALIDBASEDIR              Base directory specified is invalid
        ///   8       ERR_DEVFILELOAD_ERR             Failed loading device descriptor file
        ///   9       ERR_NOTSELECTED                 Device specified is not selected
        ///  10       ERR_INVALIDINPUTFILE            File contains invalid characters or 
        ///                                           file size is invalid.
        ///  (added since 2.0.0.1a)
        ///  11       ERR_DEVICE_NOT_FOUND            The device specified is not connected
        ///  12       ERR_NETSYS_FAILED               Network I/O error
        ///  13       ERR_SERVER_UNAVAILABLE          Failure contacting proxy server
        ///  14       ERR_SERVER_COM_ERROR            Error communicating with proxy server
        ///  (added since 2.0.0.2a)
        ///  15       ERR_NO_RESOURCES                Error allocating system resources
        ///  (added since 4.0.0.0)
        ///  16       ERR_INVALID_S_FILENAME          Invalid S filename 
        ///  17       ERR_INVALID_SERIALNUMBER        Invalid esd serial number that is been posted on server 
        ///  18       ERR_Z_ALREADY_POSTED            The z number of the s file has already posted 
        ///  19       ERR_PREVIOUS_Z_MISSED           The previous Z number isn't posted yet
        ///  20       ERR_IV_LENGTH                   incorect IV length
        ///  21       ERR_IPASSWORD_LENGTH            incorect Password length  
        /// </returns>

        static object lockObject = new object();




        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        public RBSSignResult SignDocument(string ABCDirectory, string fileToSign, string eRecordString, ref string signature)
        {
            LogTrace(this.GetType().Name + " SignDocument started");
            lock (lockObject)
            {
                try
                {
                    signature = "";
                    string deviceLocation = Settings.Ethernet.IPAddress;
                    RBSSignResult result = InitiateDevice(ABCDirectory);
                    if (result == RBSSignResult.ERR_SUCCESS)
                    {
                        result = (RBSSignResult)CVB_FSL_SignDocument(fileToSign, ref signature);
                        if (result != RBSSignResult.ERR_SUCCESS)
                        {
                            LogTrace("Sign Document Fail at CVB_FSL_SignDocument with code " + result);
                            signature = "";
                        }
                        else if (eRecordString.StartsWith("[<]") && eRecordString.EndsWith("[>]"))
                        {

                            string afmPublisher, AFMRecipient, CustomerID, InvoiceType, Seira, InvoiceNo, neta, NET_B, netc, netd, nete, vata, vatb, vatc, vatd, Total, Currency;
                            try
                            {
                                String[] fields = eRecordString.Substring(3, eRecordString.Length - 6).Split('/');
                                afmPublisher = fields[0];
                                AFMRecipient = fields[1];
                                CustomerID = fields[2];
                                InvoiceType = fields[4];
                                Seira = fields[5];
                                InvoiceNo = fields[6];
                                neta = fields[7];
                                NET_B = fields[8];
                                netc = fields[9];
                                netd = fields[10];
                                nete = fields[11];
                                vata = fields[12];
                                vatb = fields[13];
                                vatc = fields[14];
                                vatd = fields[15];
                                Total = fields[16];
                                Currency = fields[17];

                                result = (RBSSignResult)CVB_FSL_InvData(afmPublisher, AFMRecipient, CustomerID, InvoiceType, Seira, InvoiceNo, neta, NET_B, netc, netd, nete, vata, vatb, vatc, vatd, Total, Currency, ref signature);
                                if (result == RBSSignResult.ERR_SUCCESS)
                                {
                                    signature = signature.Split('-')[0];
                                    LogTrace("CVB_FSL_InvData success with signature " + signature);
                                }
                                else
                                {
                                    LogTrace("CVB_FSL_InvData failed with code " + result);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogErrorException("Exception during Getting signature (CVB_FSL_InvData)", ex);
                                ReleaseDevice();
                                return RBSSignResult.ERR_UNRECOVERABLE_ERROR;
                            }
                        }
                        else
                        {
                            LogInfo("unexpected eRecordString " + eRecordString);
                        }
                        ReleaseDevice();
                    }
                    else
                    {
                        LogTrace("Sign Document Fail at InitateDevice with code " + result);
                    }
                    return result;
                }
                catch (Exception ex2) //(Win32Exception e)
                {
                    LogErrorException("Exception during Getting signature (outer)", ex2);
                    ReleaseDevice();
                    return RBSSignResult.ERR_UNRECOVERABLE_ERROR;
                }
            }
        }

        /// <summary>
        /// Issues a 'Z' report.
        /// </summary>
        /// <param name="ABCDirectory">The ABC Directory</param>
        /// <returns>Result:
        ///0       ERR_SUCCESS                     No errors, success
        ///1       ERR_BADARGUMENT                 Bad parameter specified
        ///2       ERR_FILEIO_ERROR                Error in file/filesystem operation
        ///3       ERR_COMMUNICATION_ERROR         Communication with device failed
        ///4       ERR_UNRECOVERABLE_ERROR         Hardware error: device needs service
        ///5       ERR_UNEXPECTED_ERROR            Unexpected error, operation aborted
        ///6       ERR_INVALIDDEVICEID             Serial number is not the one expected
        ///7       ERR_INVALIDBASEDIR              Base directory specified is invalid
        ///8       ERR_DEVFILELOAD_ERR             Failed loading device descriptor file
        ///9       ERR_NOTSELECTED                 Device specified is not selected
        ///10      ERR_INVALIDINPUTFILE            File contains invalid characters
        ///11      INVALID_PROPERTY                A Required connection propery was null or invalid.For Ethernet Connection the "IPAddress" property is required.
        ///12      UNSUPPORTED_CONNECTION_TYPE     Only ETHERNET connection type is supported
        /// </returns>

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public RBSSignResult IssueZreport(string ABCDirectory, out string uploadZErrorMessage)
        {
            lock (lockObject)
            {
                uploadZErrorMessage = string.Empty;
                LogTrace("Start Issuing Z ");
                RBSSignResult result = InitiateDevice(ABCDirectory);
                LogTrace("Result Frim Initiate Device : " + result.ToString());
                if (result == RBSSignResult.ERR_SUCCESS)
                {
                    result = (RBSSignResult)CVB_FSL_IssueZreport();
                    LogTrace("Result From IssueZ Call: " + result.ToString());
                }

                if (result == RBSSignResult.ERR_SUCCESS && FiscalServiceSettings.SendFiles)
                {
                    try
                    {
                        FiscalHelper.UploadCurrentZFile(FiscalServiceSettings.GgpsUrl, FiscalServiceSettings.AesKey, ABCDirectory, FiscalServiceSettings.EafdssSN, out uploadZErrorMessage);
                        if (!string.IsNullOrEmpty(uploadZErrorMessage) && uploadZErrorMessage != "SUCCESS")
                        {
                            LogError(uploadZErrorMessage);
                            if (FiscalServiceSettings.SendMailOnUploadFail)
                            {
                                FiscalHelper.SendMail(FiscalServiceSettings.MailList, false, FiscalServiceSettings.SmtpServer, FiscalServiceSettings.SmtpUser, FiscalServiceSettings.SmtpPass, FiscalServiceSettings.SmtpPort);
                            }
                        }
                        if (!string.IsNullOrEmpty(uploadZErrorMessage) && uploadZErrorMessage == "SUCCESS")
                        {
                            if (FiscalServiceSettings.SendMailOnUploadFail)
                            {
                                FiscalHelper.SendMail(FiscalServiceSettings.MailList, true, FiscalServiceSettings.SmtpServer, FiscalServiceSettings.SmtpUser, FiscalServiceSettings.SmtpPass, FiscalServiceSettings.SmtpPort);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogErrorException("Uploading Z File", ex);
                    }
                }
                ReleaseDevice();
                LogTrace("Device Released After Z Call");
                return result;
            }
        }


        private FileInfo Get_S_File(string basepath, int lastZUploaded)
        {
            FileInfo result = null;
            try
            {
                if (Directory.Exists(basepath))
                {
                    lastZUploaded++;
                    string searchPattern = lastZUploaded.ToString().PadLeft(4, '0');
                    List<string> directories = Directory.GetDirectories(basepath, searchPattern, SearchOption.TopDirectoryOnly).ToList();
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

        private RBSSignResult InitiateDevice(string abcpath, bool force = false)
        {
            LogTrace("InitiateDevice started");
            if (this.DeviceInitiated == true && force == false)
            {
                LogTrace("InitiateDevice not needed.");
                return RBSSignResult.ERR_SUCCESS;
            }
            int tries = 0;
            int maxTries = 4;
            RBSSignResult result = RBSSignResult.ERR_SERVER_COM_ERROR;
            while (tries < maxTries && result != RBSSignResult.ERR_SUCCESS)
            {
                LogTrace(String.Format("{0} of {1} tries", tries, maxTries));

                LogTrace("Setting RBS flags: Debug.");
                VB_FSL_SetDebug(0);                                                                     // Disables the Library Debug messages  
                LogTrace("Setting RBS flags: Language.");
                FSL_SetLanguage(0);                                                                     // 0 = Greek Language , 1= English
                LogTrace("Setting RBS flags: Progress.");
                VB_FSL_SetProgress(0);                                                                  // 0 = Disable Signing Progress Window, 1= Enable 
                LogTrace("Setting RBS flags: Error UI.");
                VB_FSL_ErrorsUI(0);
                // 0= Disables The Warning - Error -Status Messages, 1= Enables these messages.
                LogTrace("Registering RBS.");
                short a = RegisterDevice(this.RegistrationNumber);
                switch (this.ConType)
                {
                    case ConnectionType.COM:
                        // "***********"  = ESD Serial Number if all paces are '*' works with every serial. 
                        // 1 = Connection type for Serial Communication 
                        // "" = Ethernet or Proxy IP Field In case of these type of connection 
                        // "C\out\ = Path where the _a, _b files are stored after signing procedure
                        // 1 =COM1
                        LogTrace("Connecting to RBS through COM.");
                        result = (RBSSignResult)CVB_FSL_SelectDevice("***********", 1, "", abcpath, this.COMPortByte);
                        break;
                    case ConnectionType.ETHERNET:
                        String serial = "***********";
                        LogTrace("Connecting to RBS through ETHERNET.");
                        result = (RBSSignResult)CVB_FSL_SelectDevice(serial, 2, this.Settings.Ethernet.IPAddress, abcpath, 0);
                        break;
                    default:
                        result = RBSSignResult.UNSUPPORTED_CONNECTION_TYPE;
                        break;
                }
                LogTrace("Connection Result " + result);

                if (result != RBSSignResult.ERR_SUCCESS)
                {
                    tries++;
                    Thread.Sleep(200);
                }
                else
                {
                    DeviceInitiated = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the device's serial number.
        /// </summary>
        /// <param name="abcFilePath"></param>
        /// <returns></returns>
        public String GetSerialNumber(String abcFilePath)
        {
            lock (lockObject)
            {
                try
                {
                    string version = null;
                    RBSSignResult result = InitiateDevice(abcFilePath);
                    if (result == RBSSignResult.ERR_SUCCESS)
                    {
                        result = (RBSSignResult)CVB_FSL_GetSerialNo(ref version);
                        if (result != RBSSignResult.ERR_SUCCESS)
                        {
                            version = null;
                        }
                    }
                    else
                    {
                        LogTrace("GetSerialNumber Fail at InitiateDevice with code " + result);
                    }
                    ReleaseDevice();
                    return version;
                }
                catch
                {
                    ReleaseDevice();
                    return null;
                }
            }
        }

        /// <summary>
        /// Releases the device handler.
        /// </summary>
        public void ReleaseDevice()
        {
            if (this.DeviceInitiated)
            {
                int tries = 0;
                int maxTries = 4;
                RBSSignResult result = RBSSignResult.ERR_SERVER_COM_ERROR;
                while (tries < maxTries && result != RBSSignResult.ERR_SUCCESS)
                {
                    try
                    {
                        int releaseResult = CVB_FSL_ReleaseDevice();
                        if (releaseResult != 0)
                        {
                            tries++;
                            Thread.Sleep(200);
                        }
                        else
                        {
                            this.DeviceInitiated = false;
                            result = RBSSignResult.ERR_SUCCESS;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.Message;
                        tries++;
                        Thread.Sleep(200);
                    }
                }
            }
        }

    }
}
