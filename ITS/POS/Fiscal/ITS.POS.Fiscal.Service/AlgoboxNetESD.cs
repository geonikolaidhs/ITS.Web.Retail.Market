using eFiles;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Hardware.Common;
using System.Globalization;

namespace ITS.POS.Fiscal
{
    public class AlgoboxNetESD : Device
    {
        public enum AlgoboxNetResult
        {
            LISENCE_NOT_FOUND = -70,
            WRONG_E_FILE_DATA = -63,
            ERROR_UPLOADING_FILES_TO_SERVER = -62,
            ERROR_WHILE_CREATING_S_TXT_FILE_ENCRYPTED = -61,
            ERROR_WHILE_CREATING_S_TXT_FILE_UNENCRYPTED = -60,
            INSUFFICIENT_DATA = -52,
            WRONG_DOCUMENT_TYPE = -51,
            NO_INFORMATION_SPECIFIED_IN_A_TXT = -50,
            THE_REPRINT_OPERATION_HAS_FAILED = -18,
            ERROR_IN_CONVERSION_TO_ELOT_928_CR_AND_LF_VIOLATION = -17,
            SIGN_VERIFICATION_ERROR = -16,
            MISSING_A_FILES_IN_RECOVERY = -15,
            LAST_Z_CHECKING_FAILED = -14,
            GENERAL_PATH_OR_FILE_ERROR = -13,
            NO_FILES_TO_RESIGN = -12,
            GENERAL_RESIGN_ERROR = -11,
            ERROR_IN_RELOADING_OF_LAST_DAY_SIGN_FILES = -10,
            INI_FILE_ERROR_IN_CREATE_OF_Z_FILES = -9,
            INI_FILE_ERROR_IN_CREATE_A_AND_B_FILES = -8,
            ERROR_IN_CONVERSION_TO_ELOT_928 = -7,
            GENERAL_FILE_TO_SIGN_ERROR = -6,
            CANNOT_COMMUNICATE_WITH_THE_BOX = -5,
            CANNOT_GET_A_REPLY_FROM_THE_BOX = -4,
            CANNOT_FIND_THE_BOX = -3,
            CANNOT_SETUP_COM_PORT_SPECIFIED = -2,
            CANNOT_OPEN_COM_PORT_SPECIFIED = -1,
            SUCCESS = 0,
            TOO_MANY_FIELDS = 1,
            LARGE_FIELD = 2,
            SMALL_FIELD = 3,
            WRONG_FIELD_LENGTH = 4,
            WRONG_FIELDS = 5,
            WRONG_COMMAND = 6,
            WRONG_PRINTING_TYPE = 9,
            DAY_IS_OPEN_EXECUTE_Z = 10,
            TECHNICIAN_IS_REQUIRED = 11,
            WRONG_DATE_TIME = 12,
            NO_RECORDS_IN_PERIOD = 13,
            BUSY = 14,
            NO_MORE_NAME_CHANGES = 15,
            DAY_IS_OPEN_CANNOT_EXECUTE = 16,
            BLOCK_IS_NOT_OPEN = 17,
            WRONG_INFO = 18,
            WRONG_SIGNATURE_INFO = 19,
            ISSUE_Z_24_HOURS_SINCE_LAST_Z = 20,
            WRONG_Z_NUMBER_Z_NOT_FOUND = 21,
            WRONG_Z_RECORD_TECHNICIAN_IS_REQUIRED = 21,
            USER_INNERVATION_IN_PROGRESS = 23,
            DAILY_SIGNATURE_LIMIT_REACHED_ISSUE_Z = 24,
            OUT_OF_PAPER = 25,
            PRINTER_DISCONNECTED = 26,
            FISCAL_MEMORY_DISCONNECTED = 27,
            DEVICE_ERROR_TECHNICIAN_IS_REQUIRED = 28,
            FISCAL_MEMORY_IS_FULL = 29,
            NO_DATA_TO_SIGN = 30,
            SIGNATURE_DOES_NOT_EXIST = 31,
            LOW_CMOS_BATTERY = 32,
            INVALID_COMMAND_DATA_RETRIEVAL_ACTIVE = 33,
            INVALID_COMMAND_DATA_RETRIEVAL_AFTER_CMOS_RESET = 34,
            CMOS_ERROR = 35,
            MORE_THAN_48_HOURS_SINCE_LAST_Z_CHECK_DATE_TIME = 36,
            INVALID_CHARACTERS_IN_DOCUMENT = 37,
            WRONG_FRAME = 96,
            ACTION_CANCELED_BY_USER = 97,
            NO_SIGNING_DUE_TO_DEVICE_FAILURE = 98,
            WRONG_BOX_SERIAL_NUMBER = 99,
            UNSUPPORTED_CONNECTION_TYPE = 100,
            INVALID_PROPERTY = 101
        }

        /// <summary>
        /// Only ConnectionType.Ethernet,COM and Emulated is supported.
        /// </summary>
        /// <param name="conType"></param>
        public AlgoboxNetESD(ConnectionType conType, string deviceName)
        {
            ConType = conType;
            DeviceName = deviceName;            
        }
        private NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." }; 
        public AlgoboxNetResult SignDocument(string fileToSign, string eRecordString, ref string exResult)
        {
            string port = null;

            switch (ConType)
            {
                case ConnectionType.COM:
                    if (String.IsNullOrEmpty(Settings.COM.PortName))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.COM.PortName;
                    break;
                case ConnectionType.EMULATED:
                    exResult = "EMULATED SIGNATURE 69F4D3750BA1CD6873BEE5B0FD2D49640BE5A634 0136 00000137 1311211608 EHR120064";
                    return AlgoboxNetResult.SUCCESS;
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.Ethernet.IPAddress;
                    break;
                default:
                    return AlgoboxNetResult.UNSUPPORTED_CONNECTION_TYPE;
            }

            AlgoDll_webUpdate.Box2013Class box2013 = new AlgoDll_webUpdate.Box2013Class();
            AlgoDll_webUpdate.BoxClass box = new AlgoDll_webUpdate.BoxClass();
            short result = 0;

            //int eResult = box.create_eRecord_from_string(eRecordString);        
            //eFiles.clsrecordHeader v = box.get_eRecord();
            //double d = v.get_netVatA();
            
            //if (eResult != 0)
                //return AlgoboxNetResult.WRONG_E_FILE_DATA;
            clsrecordHeader eRecord = new clsrecordHeader();
            /////
            bool eRecordCorrect = false;

            if (eRecordString.StartsWith("[<]") && eRecordString.EndsWith("[>]"))
            {
                try
                {
                    String[] fields = eRecordString.Substring(3, eRecordString.Length - 6).Split('/');
                    eRecord.set_vatNumberIssue(ref fields[0]);
                    eRecord.set_vatNumberReceive(ref fields[1]);
                    eRecord.set_nCitizenCard(ref fields[2]);
                    eRecord.set_issueDateTime(ref fields[3]);
                    eRecord.set_sDocumentType(ref fields[4]);
                    eRecord.set_sSeiraTheorisis(ref fields[5]);
                    eRecord.set_nDocumentNumber(ref fields[6]);
                    double v1,v2,v3,v4,v5,v6,v7,v8,v9,v10;
                    v1 = Double.Parse(fields[7], nfi); 
                    eRecord.set_netVatA(ref v1);
                    v2 = Double.Parse(fields[8], nfi); 
                    eRecord.set_netVatB(ref v2);
                    v3 = Double.Parse(fields[9], nfi); 
                    eRecord.set_netVatC(ref v3);
                    v4 = Double.Parse(fields[10], nfi);
                    eRecord.set_netVatD(ref v4);
                    v5 = Double.Parse(fields[11], nfi);
                    eRecord.set_netVatE(ref v5);
                    v6 = Double.Parse(fields[12], nfi);
                    eRecord.set_sumVatA(ref v6);
                    v7 = Double.Parse(fields[13], nfi);
                    eRecord.set_sumVatB(ref v7);
                    v8 = Double.Parse(fields[14], nfi);
                    eRecord.set_sumVatC(ref v8);
                    v9 = Double.Parse(fields[15], nfi);
                    eRecord.set_sumVatD(ref v9);
                    v10 = Double.Parse(fields[16], nfi);
                    eRecord.set_sumDocument(ref v10);
                    eRecord.set_nCurrencyID(ref fields[17]);
                    box.set_eRecord(ref eRecord);
                    eRecordCorrect = true;
                }
                catch (Exception)
                {
                }
            }
            if (eRecordCorrect == false)
                return AlgoboxNetResult.WRONG_E_FILE_DATA;
            /////
            bool suppressError = true; // hides message boxes
            string pass = "";
            bool dos = false;
            bool createTaxFiles = true;
            box.set_createTaxFiles(ref createTaxFiles);
            //box.GetDllPath(ref DllPath, ref boxfiles);
            //box.CheckBoxSystem(ref port, ref result, ref exResult, ref suppressError, ref pass);
            object boxAsObj = box as object;
            box2013.set_objHost(ref boxAsObj);
            box2013.Sign_(ref port, fileToSign, ref result, ref exResult, ref suppressError, ref pass, ref dos);

            box2013 = null;
            box = null;
            return (AlgoboxNetResult)result;
        }

        public void CheckSystem(out short Result, out String Explanation)
        {
            string Port = null;
            switch (ConType)
            {
                case ConnectionType.COM:
                    if (String.IsNullOrEmpty(Settings.COM.PortName))
                    {
                        Result = -10000;
                        Explanation = "INVALID SETTING";
                        return;
                    }
                    Port = this.Settings.COM.PortName;
                    break;
                case ConnectionType.EMULATED:
                    Result = 0;
                    Explanation = "SUCCESS";
                    return;
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        Result = -10001;
                        Explanation = "INVALID SETTING";
                        return;
                    }
                    Port = this.Settings.Ethernet.IPAddress;
                    break;
                default:
                    Result = -10002;
                        Explanation = "UNSUPPORTED";
                        return;
            }
            AlgoDll_webUpdate.BoxClass box = new AlgoDll_webUpdate.BoxClass();
            bool SupressError = false;
            string Pass = "";
            Result = 0;
            Explanation = "";

            box.CheckBoxSystem(ref Port, ref Result, ref Explanation, ref SupressError, ref Pass);

        }

        public AlgoboxNetResult IssueZreport(ref string exResult)
        {
            string port = null;

            switch (ConType)
            {
                case ConnectionType.COM:
                    if (String.IsNullOrEmpty(Settings.COM.PortName))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.COM.PortName;
                    break;
                case ConnectionType.EMULATED:
                    exResult = "EMULATED SIGNATURE 69F4D3750BA1CD6873BEE5B0FD2D49640BE5A634 0136 00000137 1311211608 EHR120064";
                    return AlgoboxNetResult.SUCCESS;
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.Ethernet.IPAddress;
                    break;
                default:
                    return AlgoboxNetResult.UNSUPPORTED_CONNECTION_TYPE;
            }

            AlgoDll_webUpdate.Box2013Class box2013 = new AlgoDll_webUpdate.Box2013Class();
            AlgoDll_webUpdate.BoxClass box = new AlgoDll_webUpdate.BoxClass();

            short result = 0;
            bool suppressError = true; // hides message boxes
            string pass = "";
            bool createTaxFiles = true;
            box.set_createTaxFiles(ref createTaxFiles);
            object boxAsObj = box as object;
            box2013.set_objHost(ref boxAsObj);

            box2013.MakeZReport(ref port, ref result, ref exResult, ref suppressError, ref pass);
            return (AlgoboxNetResult)result;
        }

        public String GetSerialNumber()
        {

            string port = null;

            switch (ConType)
            {
                case ConnectionType.COM:
                    if (String.IsNullOrEmpty(Settings.COM.PortName))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY.ToString();
                    }
                    port = this.Settings.COM.PortName;
                    break;
                case ConnectionType.EMULATED:

                    return "EMULATED SN";
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY.ToString();
                    }
                    port = this.Settings.Ethernet.IPAddress;
                    break;
                default:
                    return AlgoboxNetResult.UNSUPPORTED_CONNECTION_TYPE.ToString();
            }
            AlgoDll_webUpdate.Box2013Class box2013 = new AlgoDll_webUpdate.Box2013Class();
            AlgoDll_webUpdate.BoxClass box = new AlgoDll_webUpdate.BoxClass();
            return box.get_getBoxSerialNumber(port);
        }

        public AlgoboxNetResult GetVersionInfo(ref string exResult)
        {
            string port = null;

            switch (ConType)
            {
                case ConnectionType.COM:
                    if (String.IsNullOrEmpty(Settings.COM.PortName))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.COM.PortName;
                    break;
                case ConnectionType.EMULATED:
                    exResult = "Application Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    return AlgoboxNetResult.SUCCESS;
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return AlgoboxNetResult.INVALID_PROPERTY;
                    }
                    port = this.Settings.Ethernet.IPAddress;
                    break;
                default:
                    return AlgoboxNetResult.UNSUPPORTED_CONNECTION_TYPE;
            }

            AlgoDll_webUpdate.Box2013Class box2013 = new AlgoDll_webUpdate.Box2013Class();
            AlgoDll_webUpdate.BoxClass box = new AlgoDll_webUpdate.BoxClass();



            short result = 0;
            bool suppressError = true; // hides message boxes
            string pass = "";
            bool createTaxFiles = true;
            box.set_createTaxFiles(ref createTaxFiles);
            object boxAsObj = box as object;
            box2013.set_objHost(ref boxAsObj);



            box2013.GetVersionInfo(ref port, ref result, ref exResult, ref suppressError, ref pass);
            return (AlgoboxNetResult)result;

        }

    }
}
