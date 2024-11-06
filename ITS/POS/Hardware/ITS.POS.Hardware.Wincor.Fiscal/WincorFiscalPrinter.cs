using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using ITS.POS.Hardware.Common.Exceptions;
using ITS.POS.Hardware.Common;
using System.Reflection;
using ITS.POS.Resources;

namespace ITS.POS.Hardware.Wincor.Fiscal
{
    [Flags]
    public enum CommandFlagRequirements
    {
        NONE = 0,
        DAY_OPEN = 1,
        DAY_NOT_OPEN = 2,
        TRANSACTION_OPEN = 4,
        TRANSACTION_NOT_OPEN = 8,
        TRANSACTION_IN_PAYMENT = 16,
        TRANSACTION_NOT_IN_PAYMENT = 32,
        EJ_DATA_PENDING = 64,
        DOCUMENT_OPEN = 128,
        DOCUMENT_NOT_OPEN = 256
    }

    public class WincorFiscalPrinter : FiscalPrinter
    {
        const byte ESC = 0x1B;
        const byte MFB = 0x80;
        const byte MFB1 = 0x81;
        const byte MFB2 = 0x82;
        const byte MFE = 0x83;

        NumberFormatInfo NumberFormat = new NumberFormatInfo() { CurrencyDecimalSeparator = ",", NumberDecimalSeparator = ",", NumberGroupSeparator = ".", CurrencyGroupSeparator = "." };
        Encoding Encoding = Encoding.GetEncoding(1253);
        int MFC_INFO_ResponseBytesCountForGR = 54;
        int MFC_SYNC_ResponseBytesCount = 7;

        public WincorFiscalPrinter(ConnectionType conType, String deviceName, int posid,int lineChars,int commandChars)
            : base(conType, deviceName, posid,lineChars,commandChars)
        {
            FiscalStatus = new WincorFiscalPrinterStatus();
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            try
            {
                DeviceResult result = ReadDeviceStatus();
                if (result == DeviceResult.SUCCESS)
                {
                    message = string.Empty;
                    return eDeviceCheckResult.SUCCESS;
                }
                message = "OFFLINE";
                return eDeviceCheckResult.FAILURE;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return eDeviceCheckResult.FAILURE;
            }
        }

        public override Version FiscalVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override DeviceResult InitializeConnection()
        {
            DeviceResult result = base.InitializeConnection();
            serialPort.DtrEnable = true;


            if (result != DeviceResult.SUCCESS)
            {
                return result;
            }
            else
            {
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.ErrorReceived += serialPort_ErrorReceived;
                return DeviceResult.SUCCESS;
            }
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public WincorFiscalPrinterStatus NativeFiscalStatus
        {
            get
            {
                return (WincorFiscalPrinterStatus)this.FiscalStatus;
            }
        }

        public override DeviceResult OpenFiscalDay(int posID)
        {
            //ESC MFB “aa” ESC MFB1 “bf” <POSID> ESC MFE
            WincorFiscalPrinterStatusCode result;
            int retries = 10;
            do
            {
                List<byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'a', (byte)'a', ESC, MFB1, (byte)'b', (byte)'f' };
                bytesToSend.AddRange(Encoding.GetBytes(posID.ToString()).Take(3));
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                result = SendCommand(bytesToSend.ToArray(), out response, throwException: false);
                this.ReadDeviceStatus();
                retries--;
                if (NativeFiscalStatus.DayOpen == false)
                {
                    Thread.Sleep(500);
                }

            } while (retries > 0 && NativeFiscalStatus.DayOpen == false);

            if (NativeFiscalStatus.DayOpen == false && result != WincorFiscalPrinterStatusCode.SUCCESS)
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)result, "Cannot Open Day. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
            else
            {
                return DeviceResult.SUCCESS;
            }
        }

        public override DeviceResult IssueXReport()
        {
            WincorFiscalPrinterStatusCode result;
            //do some retries in case the printer is still printing and the flags are all wrong
            int retries = 2;
            do
            {
                if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_OPEN | CommandFlagRequirements.TRANSACTION_NOT_OPEN, forceCancelReceipt: true))
                {
                    //ESC MFB “ac” ESC MFE
                    byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'a', (byte)'c', ESC, MFE };
                    byte[] response;
                    result = SendCommand(bytesToSend, out response);
                }
                else
                {
                    result = WincorFiscalPrinterStatusCode.FAILURE;
                }

                retries--;
                if (result != WincorFiscalPrinterStatusCode.SUCCESS)
                {
                    Thread.Sleep(1000);
                }
            } while (retries > 0 && result != WincorFiscalPrinterStatusCode.SUCCESS);

            if (result != WincorFiscalPrinterStatusCode.SUCCESS)
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)result, "Cannot issue X. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }

            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public virtual WincorFiscalPrinterStatusCode GetCurrentZNumber(out int zReportNumber)
        {
            //EJ_GET_EOD_RANGE: ESC MFB p h ESC MFE ESC + 09h
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.FAILURE;
            zReportNumber = -1;
            if (CheckIfCommandCanExecute(CommandFlagRequirements.NONE))
            {
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'p', (byte)'h', ESC, MFE, ESC, (byte)'+', 0x09 };
                byte[] response;
                result = SendCommand(bytesToSend, out response, true);
                byte[] after_ESC_MFB1 = SeparateByteArrayAndGetLast(response, new byte[] { ESC, MFB1 });
                byte[] currentEOD = after_ESC_MFB1.Take(after_ESC_MFB1.Length - 2).ToArray();

                zReportNumber = int.Parse(Encoding.GetString(currentEOD));


            }
            return result;
        }

        protected byte[] GetEJFileSector(int zReportNumber, int sectorNumber)
        {

            bool isStructureCorrect = false;
            int maxTries = 100;
            int tries = 0;
            List<byte> response;
            int fileSectorStart = Environment.TickCount;
            do
            {
                int expectedNumberOfBytes = 0;
                response = new List<byte>(524);
                isStructureCorrect = false;

                tries++;
                try
                {
                    //// SEND COMMAND BEGIN
                    List<byte> command = new List<byte> { ESC, MFB, (byte)'p', (byte)'k', ESC, MFB1, (byte)'d', (byte)'a' }; //ESC MFB p k ESC MFB1 d a
                    command.AddRange(Encoding.GetBytes(zReportNumber.ToString()));      //<FromEOD> as ascii
                    command.AddRange(new byte[] { ESC, MFB1, (byte)'d', (byte)'i' });      //ESC MFB1 d i
                    command.AddRange(Encoding.GetBytes(sectorNumber.ToString())); //<SectorNumber> as ascii
                    command.AddRange(new byte[] { ESC, MFE, ESC, (byte)'+', 0x09 });      //ESC MFE ESC + 09h

                    if (serialPort == null)
                    {
                        InitializeConnection();
                    }
                    if (serialPort.IsOpen == false)
                    {
                        serialPort.Open();

                    }
                    if (serialPort.IsOpen == false || serialPort.CtsHolding == false)
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, -1000, String.Format(POSClientResources.CANNOT_OPEN_PORT_OR_PRINTER_IS_OFFLINE, this.Settings.COM.PortName));
                    }

                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();
                    serialPort.Write(command.ToArray(), 0, command.Count);

                    //Wait the printer to start filling the buffer
                    Thread.Sleep(200);
                    //// SEND COMMAND END

                    //// READ START
                    string message = "WincorFiscalPrinter.GetEJFileSector: Read Start (ms):" + (Environment.TickCount - fileSectorStart);
                    Debug.WriteLine(message);
                    LogTrace(message);
                    int currentByte = -1;


                    int internalMaxTries = 50;
                    int internalTries = 0;
                    int previousByte = 0;
                    do
                    {
                        if (serialPort.BytesToRead > 0)
                        {
                            currentByte = serialPort.ReadByte();
                            previousByte = response.LastOrDefault();
                            response.Add((byte)currentByte);
                            internalTries = 0;
                        }
                        else
                        {
                            message = "WincorFiscalPrinter.GetEJFileSector: No bytes to read. Current try: " + internalTries;
                            Debug.WriteLine(message);
                            LogTrace(message);
                            Thread.Sleep(200);
                            internalTries++;
                        }
                    }
                    //end of response at ESC MFE
                    while ((currentByte == MFE && previousByte == ESC) == false && internalTries < internalMaxTries);

                    message = "WincorFiscalPrinter.GetEJFileSector: Read End. Data length:" + response.Count;
                    Debug.WriteLine(message);
                    LogTrace(message);

                    ///check structure
                    if (response.Count >= 4 && response[0] == ESC && response[1] == 's')
                    {
                        byte DLH = response[2]; ////Data length byte (high)
                        byte DLL = response[3]; ////Data length byte (low)
                        expectedNumberOfBytes = DLH * 0x100 + DLL;
                        isStructureCorrect = expectedNumberOfBytes == (response.Count - 4);
                    }

                    if (isStructureCorrect == false)
                    {
                        message = "WincorFiscalPrinter.GetEJFileSector: Response structure is incorect. Current try: " + tries;
                        Debug.WriteLine(message);
                        LogTrace(message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    LogError(ex.Message);
                    isStructureCorrect = false;
                }

            } while (isStructureCorrect == false && tries < maxTries);

            if (tries == maxTries && isStructureCorrect == false)
            {
                string errorMessage = String.Format("Response Structure is incorrect. No of Tries: {0}, Response: {1}", tries, Encoding.GetString(response.ToArray()));
                Debug.WriteLine(errorMessage);
                LogError(errorMessage);
                throw new POSFiscalPrinterException(this.DeviceName, -600, errorMessage);
            }

            string responseInfo = "File sector read in (ms):" + (Environment.TickCount - fileSectorStart);
            Debug.WriteLine(responseInfo);
            LogTrace(responseInfo);
            return response.ToArray();
            ////READ END
        }


        protected virtual WincorFiscalPrinterStatusCode GetEJFileData(int zReportNumber, string pathToEJFiles)
        {
            byte[] bytesToSend;
            byte[] response;
            WincorFiscalPrinterStatusCode result;
            if (String.IsNullOrWhiteSpace(this.NativeFiscalStatus.SerialNumber))
            {
                this.ReadDeviceStatus();
            }

            string serialNumber = this.NativeFiscalStatus.SerialNumber;

            bool fileEnd = false;
            string fileName_A = serialNumber + String.Format("{0:0000}", zReportNumber) + DateTime.Now.ToString("yyMMdd") + "_a.txt";
            string fileName_B = serialNumber + String.Format("{0:0000}", zReportNumber) + DateTime.Now.ToString("yyMMdd") + "_b.txt";

            string fullFilePath_A = pathToEJFiles + "\\" + fileName_A;
            string fullFilePath_B = pathToEJFiles + "\\" + fileName_B;

            if (!Directory.Exists(pathToEJFiles))
            {
                Directory.CreateDirectory(pathToEJFiles);
            }

            int tries = 0;
            int timeout = 60000;
            int ejVerificationTimeout = 36000000;
            const int maxTries = 5;
            while (tries <= maxTries && fileEnd == false)
            {
                if (File.Exists(fullFilePath_A))
                {
                    File.Move(fullFilePath_A, fullFilePath_A + "_FAILURE_" + DateTime.Now.ToString("yyyy_MM_dd_mm_ss"));
                }

                using (FileStream writer = new FileStream(fullFilePath_A, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    try
                    {
                        int sectorNumber = 0;
                        while (!fileEnd)
                        {
                            string message = String.Format("WincorFiscalPrinter.GetEJFileData: Getting file sector {0}", sectorNumber);
                            Debug.WriteLine(message);
                            LogTrace(message);

                            response = GetEJFileSector(zReportNumber, sectorNumber);

                            message = String.Format("WincorFiscalPrinter.GetEJFileData: Read file sector {0}, Data Length: {1}", sectorNumber, response.Length);
                            Debug.WriteLine(message);
                            LogTrace(message);


                            List<int> allMFB1Indices = new List<int>();
                            List<byte> responseList = response.ToList();
                            for (int i = responseList.IndexOf(MFB1); i > -1; i = responseList.IndexOf(MFB1, i + 1))
                            {
                                if (i > 0 && response[i - 1] == ESC)
                                {
                                    allMFB1Indices.Add(i);
                                }
                            }


                            int fileDataStartIndex = allMFB1Indices.FirstOrDefault() + 1;
                            int fileDataEndIndex = response.Length - 2;

                            byte[] fileData = response.Skip(fileDataStartIndex).Take(fileDataEndIndex - fileDataStartIndex).ToArray();
                            writer.Write(fileData, 0, fileData.Length);

                            if (fileData.Length < 512) //end of file
                            {
                                fileEnd = true;
                            }
                            sectorNumber++;
                        }
                    }
                    catch (POSFiscalPrinterException ex)
                    {
                        if (ex.ErrorCode == -500 && tries < maxTries)
                        {
                            //Timeout Exception --> retry
                            tries++;
                            timeout = timeout * 2;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            //EJ_CONFIRM_EJMEM_DATA_TRANSFERRED
            /*ESC MFB “qc”
              ESC MFB1 “hc” <EJFileMessageDigest>
              ESC MFE*/

            string fileContent = File.ReadAllText(fullFilePath_A, Encoding);
            string fileSHA1 = SHA1Util.SHA1HashStringForString(fileContent, Encoding).ToUpper();
            string fileSHA1b = SHA1Util.SHA1HashStringForBytes(File.ReadAllBytes(fullFilePath_A));
            using (StreamWriter writer = new StreamWriter(fullFilePath_B, false, Encoding))
            {
                writer.Write(fileSHA1);
            }

            List<byte> ejVerificationCommandBytes = new List<byte> { ESC, MFB, (byte)'q', (byte)'c', ESC, MFB1, (byte)'h', (byte)'c' }; //ESC MFB “qc” ESC MFB1 “hc”
            ejVerificationCommandBytes.AddRange(Encoding.GetBytes(fileSHA1)); // <EJFileMessageDigest> as ASCII
            ejVerificationCommandBytes.AddRange(new byte[] { ESC, MFE }); // ESC MFE

            bytesToSend = ejVerificationCommandBytes.ToArray();
            result = SendCommand(bytesToSend, out response, checkForErrorResponseTimeout: ejVerificationTimeout);

            return result;
        }

        public override DeviceResult IssueZReport(String pathToAbc, out int zReportNumber, out string pathToEJFiles)
        {
            zReportNumber = -1; //TODO
            pathToEJFiles = ""; //TODO

            int retries = 4; //retry in case printer is still printing and flags are all wrong
            WincorFiscalPrinterStatusCode result;
            do
            {
                result = GetCurrentZNumber(out zReportNumber);

                if (result != WincorFiscalPrinterStatusCode.SUCCESS)
                {
                    retries--;
                    continue;
                }

                if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_OPEN | CommandFlagRequirements.TRANSACTION_NOT_OPEN, forceCancelReceipt: true))
                {
                    byte[] bytesToSend;
                    byte[] response;
                    bytesToSend = new byte[] { ESC, MFB, (byte)'a', (byte)'b', ESC, MFE }; //DAY END
                    result = SendCommand(bytesToSend, out response);
                    if (result == WincorFiscalPrinterStatusCode.SUCCESS)
                    {
                        pathToEJFiles = pathToAbc.TrimEnd('\\') + "\\Z-" + String.Format("{0:0000}", zReportNumber);
                        result = GetEJFileData(zReportNumber, pathToEJFiles);
                    }
                }
                else if (CheckIfCommandCanExecute(CommandFlagRequirements.EJ_DATA_PENDING))
                {
                    zReportNumber--; //After closing day current Z has gone up
                    pathToEJFiles = pathToAbc.TrimEnd('\\') + "\\Z-" + String.Format("{0:0000}", zReportNumber);
                    result = GetEJFileData(zReportNumber, pathToEJFiles);
                }
                else
                {
                    //TODO not sure if should throw error
                    result = WincorFiscalPrinterStatusCode.FAILURE;
                }
                retries--;
                if (result != WincorFiscalPrinterStatusCode.SUCCESS)
                {
                    Thread.Sleep(1000);
                }
            } while (retries > 0 && result != WincorFiscalPrinterStatusCode.SUCCESS);

            if (result != WincorFiscalPrinterStatusCode.SUCCESS)
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)result, "Cannot issue Z. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }

            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult CutPaper()
        {
            byte[] bytesToSend = new byte[] { 0x19 }; //TODO not working
            byte[] response;
            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response, throwException: false);

            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReceiptPaymentMode(double grossTotal)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN, false))
            {
                //ESC MFB “da” ESC MFB1 “ab” <TotalValue> ESC MFE
                string totalStr = (grossTotal * 100).ToString(NumberFormat);
                byte[] totalAsciiBytes = Encoding.GetBytes(totalStr);

                List<byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'d', (byte)'a', ESC, MFB1, (byte)'a', (byte)'b' };
                bytesToSend.AddRange(totalAsciiBytes);
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                //TODO not sure if should throw error
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot issue Z. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
                //return DeviceResult.FAILURE;
            }
        }

        public override DeviceResult OpenLegalReceipt()
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_OPEN | CommandFlagRequirements.TRANSACTION_NOT_OPEN, forceCancelReceipt: true))
            {
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'a', ESC, MFB1, (byte)'f', (byte)'b', 0x31, ESC, MFE };
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response);

                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                //TODO not sure if should throw error
                //throw new POSFiscalPrinterException(this.DeviceName, (int)WincoreEJStatusCode.EM_CMD_WRONG_ORDER, "Cannot issue Z. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen);
                return DeviceResult.FAILURE;
            }
        }

        public override DeviceResult SellItem(string ItemDescription, string additionalInfo, string additionalInfo2,
            double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {

                /* ARTICLE_SELL
                  ESC MFB “fa” ESC MFB1 “ba” <ArticleName>
                  ESC MFB1 “aa” <SinglePrice> ESC MFB1 “ca” <VATCategory>
                  [ESC MFB2 “bd” <PrintLinePre> ] [ESC MFB2 “be” <PrintLinePost> ]ESC MFE
                 */
                byte vatCategory = (byte)(0x30 + (int)vatCode - 1);
                byte[] trimedDescription = Encoding.GetBytes(ItemDescription).Take(22).ToArray();  //MAX 22 Bytes
                string totalStr = (lineTotal * 100).ToString(NumberFormat);//String.Format(NumberFormat, "{0:#.00}", total);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'f', (byte)'a', ESC, MFB1, (byte)'b', (byte)'a' };
                bytesToSend.AddRange(trimedDescription);
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'a', (byte)'a' });
                bytesToSend.AddRange(Encoding.GetBytes(totalStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'a', vatCategory });
                if (itemQuantity != 1 || supportsDecimal)
                {
                    string printLinePost = " " + itemQuantity.ToString("0.000", NumberFormat) + " X " + finalUnitPrice.ToString("0.00", NumberFormat);
                    byte[] printLinePostBytes = Encoding.GetBytes(printLinePost).Take(31).ToArray(); //max 31 bytes
                    bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'b', (byte)'e' });
                    bytesToSend.AddRange(printLinePostBytes);
                }

                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot add item '" + ItemDescription + "'. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult CloseLegalReceipt(bool openDrawer)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_IN_PAYMENT))
            {
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'b', ESC, MFE };
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response);

                //open drawer --> moved to special function
                //if (openDrawer)
                //{
                //    bytesToSend = new byte[] { ESC, 0x70, 0, 0x64, 0x64 };
                //    SendCommand(bytesToSend, out response, checkForError: false);
                //}

                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot close receipt. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + "TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);

            this.ReadDeviceStatus();
        }

        public override DeviceResult ReadDeviceStatus()
        {
            byte[] bytesToSend = new byte[] { ESC, (byte)'+', (byte)0x0A };
            byte[] response;
            bool done = false;
            int retries = 10;
            WincorFiscalPrinterStatusCode result;
            do
            {
                result = SendCommand(bytesToSend, out response, hasReturn: true, returnEndByte: null, responseBytesCount: MFC_INFO_ResponseBytesCountForGR, responseTimeout: 3000);
                bool hasGarbageBytes = false;
                if (serialPort.BytesToRead > 0)
                {
                    Debug.WriteLine("Read device status has " + serialPort.BytesToRead + " bytes that where not read!!");
                    hasGarbageBytes = true;
                }

                if (response.Length == MFC_INFO_ResponseBytesCountForGR && hasGarbageBytes == false)
                {
                    byte MFCStatusByte1 = response[5];
                    byte MFCStatusByte2 = response[6];
                    Debug.WriteLine("MFCStatusByte1 = " + MFCStatusByte1);
                    Debug.WriteLine("MFCStatusByte2 = " + MFCStatusByte2);
                    byte[] fiscalReceiptNumber = response.Skip(26).Take(6).ToArray();
                    string serialNumber = Encoding.GetString(response.Skip(response.Length - 12).Take(11).ToArray());

                    WincorFiscalPrinterStatus status = FiscalStatus as WincorFiscalPrinterStatus;

                    status.InServiceReset = (MFCStatusByte1 & 0x80) != 0;
                    status.TransactionInPayment = (MFCStatusByte1 & 0x40) != 0;
                    status.DocumentOpen = (MFCStatusByte1 & 0x20) != 0;
                    status.ReportOpen = (MFCStatusByte1 & 0x10) != 0;
                    status.TransactionOpen = (MFCStatusByte1 & 0x8) != 0;
                    status.DayOpen = (MFCStatusByte1 & 0x4) != 0;
                    status.InTrainingMode = (MFCStatusByte1 & 0x2) != 0;
                    status.IsBlocked = (MFCStatusByte1 & 0x1) != 0;

                    //unused
                    //(MFCStatusByte2 & 0x20) != 0;
                    //(MFCStatusByte2 & 0x10) != 0;
                    status.EJDataPending = (MFCStatusByte2 & 0x20) != 0;
                    status.FiscalFileFull = (MFCStatusByte2 & 0x10) != 0;
                    status.FiscalFileNearlyFull = (MFCStatusByte2 & 0x8) != 0;
                    status.CmdInterrupted = (MFCStatusByte2 & 0x4) != 0;
                    status.PrintoutInterrupted = (MFCStatusByte2 & 0x2) != 0;
                    status.InFiscalMode = (MFCStatusByte2 & 0x1) != 0;

                    if (String.IsNullOrWhiteSpace(status.SerialNumber))
                    {
                        status.SerialNumber = serialNumber;
                    }

                    done = true;
                }
                else
                {
                    retries--;
                    Thread.Sleep(500);
                    result = WincorFiscalPrinterStatusCode.FAILURE;
                }

            } while (retries > 0 && done == false);

            if (result != WincorFiscalPrinterStatusCode.SUCCESS)
            {
                string message = POSClientResources.FAILURE_READING_DEVICE_STATUS;//"Failure reading device status";
                if (WincorFiscalPrinterStatus.ErrorCodeDescriptionMapping.ContainsKey((int)result))
                {
                    message += WincorFiscalPrinterStatus.ErrorCodeDescriptionMapping[(int)result];
                }

                throw new POSFiscalPrinterException(this.DeviceName, (int)result, message);
            }

            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        /// <summary>
        /// Reads bytes from the port until 'readTo' byte is read or responseBytesCount is reached or timeout is reached
        /// </summary>
        /// <param name="port"></param>
        /// <param name="readTo"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected byte[] ReadResponse(SerialPort port, byte? readTo, int? responseBytesCount, int timeout, bool errorOnTimeout)
        {
            int start = Environment.TickCount;
            int currentTickCount;
            List<byte> response = new List<byte>();

            if (port.IsOpen)
            {
                int expectedTicks = Environment.TickCount + timeout;
                int currentByte = -1;
                do
                {
                    if (port.BytesToRead > 0)
                    {
                        currentByte = port.ReadByte();
                        response.Add((byte)currentByte);
                    }
                    //else
                    //{
                    //    Thread.Sleep(10);
                    //}
                    currentTickCount = Environment.TickCount;
                }
                while (currentTickCount <= expectedTicks && (currentByte != readTo || readTo == null) && (response.Count < responseBytesCount || responseBytesCount == null));
                if (currentTickCount > expectedTicks)
                {
                    Debug.WriteLine("Response Timeout (ms):" + (currentTickCount - start));
                    if (errorOnTimeout)
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, -500, String.Format(POSClientResources.TIMEOUT, this.Settings.COM.PortName));
                    }
                }
                else
                {
                    Debug.WriteLine("Response read in (ms):" + (Environment.TickCount - start));
                }
            }

            return response.ToArray();
        }



        protected virtual bool CheckIfCommandCanExecute(CommandFlagRequirements commandRequirements = CommandFlagRequirements.NONE, bool commandNeedsPaper = true, bool checkForInterupted = true, bool forceCancelReceipt = false)
        {
            this.ReadDeviceStatus(); //this calls MFC_INFO to update the status flags, before the execution of the command
            WincorFiscalPrinterStatus status = (this.FiscalStatus as WincorFiscalPrinterStatus);

            if (status.CmdInterrupted)
            {
                SPECIAL_CMD_RESTART();
                this.ReadDeviceStatus(); //update the flags
            }

            if ((checkForInterupted && status.PrintoutInterrupted) || forceCancelReceipt)
            {
                //CANNOT DO ANYTHING, MUST CALL SPECIAL_ALL_VOID

                this.CancelLegalReceipt();
                this.ReadDeviceStatus(); //update the flags
            }

            if (status.PrinterPaperEnd && commandNeedsPaper)
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_PRT_PAPER, WincorFiscalPrinterStatus.ErrorCodeDescriptionMapping[(int)WincorFiscalPrinterStatusCode.EM_PRT_PAPER]);
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.DAY_OPEN) && status.DayOpen == false)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.DAY_NOT_OPEN) && status.DayOpen)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.DOCUMENT_OPEN) && status.DocumentOpen == false)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.DOCUMENT_NOT_OPEN) && status.DocumentOpen)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.TRANSACTION_OPEN) && status.TransactionOpen == false)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.TRANSACTION_NOT_OPEN) && status.TransactionOpen)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.TRANSACTION_IN_PAYMENT) && status.TransactionInPayment == false)
            {
                return false;
            }

            if (commandRequirements.HasFlag(CommandFlagRequirements.EJ_DATA_PENDING) && status.EJDataPending == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles sending the command.
        /// </summary>
        /// <param name="command">The command's bytes.</param>
        /// <param name="response">The response if the command is a "GET" command.</param>
        /// <param name="hasReturn">If true the command is a "GET" command. Default false.</param>
        /// <param name="returnEndByte">The byte that marks the end of a command response. Default is MFE.</param>
        /// <param name="responseBytesCount">The number of bytes that the response is expected to have. Use for "GET" commands that have a static response length.</param>
        /// <param name="throwException">If true, will throw exception on command error. Default true.</param>
        /// <param name="responseTimeout">The maximum wait time in milliseconds to wait for a "GET" command's response. Default is 7000.</param>
        /// <param name="checkForError">For non-GET commands. If set to true, MFC_SYNC will be called to check the result of the executed command.</param>
        /// <param name="errorOnTimeout">If set to true, when a timeout occures an exception will be throws. Default is false.</param>
        /// <param name="checkForErrorResponseTimeout">The maximum wait time in milliseconds to wait for the MFC_SYNC command's response. Default is 5000.</param>
        /// <param name="throwErrorOnInvalidResponse">If true and the response structure that was read is not correct, an exception will be thrown. Default false.</param>
        /// <returns></returns>
        protected virtual WincorFiscalPrinterStatusCode SendCommand(byte[] command, out byte[] response, bool hasReturn = false, byte? returnEndByte = MFE, int? responseBytesCount = null,
                                                                    bool throwException = true, int responseTimeout = 7000, bool checkForError = true, bool errorOnTimeout = false,
                                                                    int checkForErrorResponseTimeout = 5000, bool throwErrorOnInvalidResponse = false)
        {
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.SUCCESS;
            byte[] MFC_INFO_command = new byte[] { ESC, (byte)'+', (byte)0x0A };
            byte[] MFC_SYNC_command = new byte[] { ESC, (byte)'+', (byte)0x08 };
            byte[] SPECIAL_ALL_VOID = new byte[] { ESC, MFB, (byte)'i', (byte)'i', ESC, MFE };
            //byte[] REAL_TIME_STATUS_TRANSMITION_1 = new byte[] { 0x10, 0x04, 0x01 }; //Real Time Status Transmission (DLE Sequence)
            //byte[] REAL_TIME_STATUS_TRANSMITION_2 = new byte[] { 0x10, 0x04, 0x02 }; //Real Time Status Transmission (DLE Sequence)
            //byte[] REAL_TIME_STATUS_TRANSMITION_3 = new byte[] { 0x10, 0x04, 0x03 }; //Real Time Status Transmission (DLE Sequence)
            //byte[] REAL_TIME_STATUS_TRANSMITION_4 = new byte[] { 0x10, 0x04, 0x04 }; //Real Time Status Transmission (DLE Sequence)

            response = new byte[0];
            if (serialPort == null)
            {
                InitializeConnection();
            }
            if (serialPort.IsOpen == false)
            {
                serialPort.Open();

            }
            if (serialPort.IsOpen == false || serialPort.CtsHolding == false)
            {
                throw new POSFiscalPrinterException(this.DeviceName, -1000, String.Format(POSClientResources.CANNOT_OPEN_PORT_OR_PRINTER_IS_OFFLINE, this.Settings.COM.PortName));
            }

            serialPort.DiscardOutBuffer();
            serialPort.DiscardInBuffer();
            serialPort.Write(command, 0, command.Length);

            if (hasReturn) //Get command, no need to call MFC_SYNC
            {
                int readResponseTries = 0;
                int maxTries = 10;
                bool responseIsValid = false;
                int expectedNumberOfBytes = 0;
                do
                {
                    Debug.WriteLine("Begin Read Response for: " + Encoding.GetString(command));
                    response = ReadResponse(serialPort, returnEndByte, responseBytesCount, responseTimeout, errorOnTimeout);
                    readResponseTries++;
                    if (response.Length >= 4 && response[0] == ESC && response[1] == 's')
                    {
                        byte DLH = response[2];
                        byte DLL = response[3];
                        expectedNumberOfBytes = DLH * 0x100 + DLL;
                        responseIsValid = expectedNumberOfBytes == (response.Count() - 4);
                    }

                    if (responseIsValid == false && readResponseTries <= maxTries)
                    {
                        string message = String.Format("Response read is invalid. Current try: {0}, Expected Data Bytes Count: {1}, Actual Data Bytes Count: {2}, Response: {3}", readResponseTries
                                                                                                                                                   , expectedNumberOfBytes
                                                                                                                                                   , response.Count() - 4
                                                                                                                                                   , Encoding.GetString(response));
                        Debug.WriteLine(message);
                        LogTrace(message);
                        ////give some time to the device and then send again
                        Thread.Sleep(100);
                        serialPort.DiscardOutBuffer();
                        serialPort.DiscardInBuffer();
                        serialPort.Write(command, 0, command.Length);
                    }

                } while (readResponseTries <= maxTries && responseIsValid == false);

                if (responseIsValid == false)
                {
                    string message = String.Format("Response read is invalid. Max tries reached: {0}, Response Data Bytes Count: {1}, Response: {2}, Command: {3}", readResponseTries
                                                                                                                                                      , response.Count() - 4
                                                                                                                                                      , Encoding.GetString(response)
                                                                                                                                                      , Encoding.GetString(command));
                    Debug.WriteLine(message);
                    LogError(message);
                    if (throwErrorOnInvalidResponse)
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, -400, message);
                    }
                }
            }
            else if (checkForError)
            {
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();
                serialPort.Write(MFC_SYNC_command, 0, MFC_SYNC_command.Length);
                Debug.WriteLine("Begin Read Response for MFC_SYNC for command '" + Encoding.GetString(command) + "'");
                byte[] mfcResponse = ReadResponse(serialPort, null, MFC_SYNC_ResponseBytesCount, checkForErrorResponseTimeout, false);
                if (serialPort.BytesToRead > 0)
                {
                    Debug.WriteLine("MFC_SYNC has " + serialPort.BytesToRead + " bytes that where not read!!");
                }

                byte[] lastErrorResult = mfcResponse.Skip(5).ToArray();//MFC_SYNC();
                if (lastErrorResult.Length == 2 && lastErrorResult[1] != 0)
                {
                    string message = "";
                    if (WincorFiscalPrinterStatus.ErrorCodeDescriptionMapping.ContainsKey(lastErrorResult[1]))
                    {
                        message = WincorFiscalPrinterStatus.ErrorCodeDescriptionMapping[lastErrorResult[1]];
                    }
                    else
                    {
                        message = "Unknown Error Code:" + lastErrorResult[1];
                    }

                    Debug.WriteLine(message);
                    if (throwException)
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, lastErrorResult[1], message);
                    }
                    else
                    {
                        result = (WincorFiscalPrinterStatusCode)lastErrorResult[1];
                    }
                }
            }

            return result;

        }

        public override double MaximumPaymentAmount
        {
            get { return 9999999.99; } //TODO check number from manual
        }

        public override bool IsTotalVatCalculationPerReceipt
        {
            get { return false; }  //TODO Test if it really is false
        }

        /// <summary>
        /// Must be called when the CMD_INTERRUPTED flag is on
        /// </summary>
        protected virtual void SPECIAL_CMD_RESTART()
        {
            //ESC MFB “ij” ESC MFE
            byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'i', (byte)'j', ESC, MFE };
            byte[] response;
            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response);

        }

        public override DeviceResult CancelLegalReceipt()
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN, checkForInterupted: false))
            {
                //ESC MFB “ii” ESC MFE
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'i', (byte)'i', ESC, MFE };
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response, throwException: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }

        public override DeviceResult GetTransactionInfo(out double vatAGrossTotal, out double vatBGrossTotal, out double vatCGrossTotal, out double vatDGrossTotal, out double vatEGrossTotal, out int receiptNumber, out double grossTotal)
        {
            vatAGrossTotal = vatBGrossTotal = vatCGrossTotal = vatDGrossTotal = vatEGrossTotal = grossTotal = receiptNumber = -1;
            //ESC MFB “kb” ESC MFE ESC “+”09H
            byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'k', (byte)'b', ESC, MFE, ESC, (byte)'+', 0x09 };
            byte[] response;

            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response, hasReturn: true, throwException: false);

            byte[] validPart = SeparateByteArray(response, new byte[] { ESC, (byte)'s' })[1].Skip(5).ToArray();
            byte[][] totalizors = SeparateByteArray(validPart, new byte[] { ESC, MFB1 });
            byte[] bruttoSales1 = totalizors[0];
            byte[] bruttoSales2 = totalizors[1];
            byte[] bruttoSales3 = totalizors[2];
            byte[] bruttoSales4 = totalizors[3];
            byte[] bruttoSales5 = totalizors[4].Take(totalizors[4].Length - 2).ToArray();

            Double.TryParse(Encoding.GetString(bruttoSales1), out vatAGrossTotal);
            Double.TryParse(Encoding.GetString(bruttoSales2), out vatBGrossTotal);
            Double.TryParse(Encoding.GetString(bruttoSales3), out vatCGrossTotal);
            Double.TryParse(Encoding.GetString(bruttoSales4), out vatDGrossTotal);
            Double.TryParse(Encoding.GetString(bruttoSales5), out vatEGrossTotal);

            vatAGrossTotal /= 100;
            vatBGrossTotal /= 100;
            vatCGrossTotal /= 100;
            vatDGrossTotal /= 100;
            vatEGrossTotal /= 100;

            ////Getting Day Counters to get legal receipts counter
            /*
                Day Counter
                0 – SALES_RECEIPTS
                1 – EJ_TICKETS
                2 –PRINTER_OFFLINE
                3 – COUPON
                4 – DISCOUNT
                5 – SURCHARGE
                6 – RETURN
                7 – VOID
                8 – DISCOUNT_ST
                9 – SURCHARGE_ST
                10 – CANCELLATION
                11 - LEGAL_TICKET
                12 - ILLEGAL_TICKETS
                13 – SERVICE
                14 – SURCHARGE_VOID
                15 – DISCOUNT_VOID
                16 – RETURN_VOID
            */
            bytesToSend = new byte[] { ESC, MFB, (byte)'k', (byte)'f', ESC, MFE, ESC, (byte)'+', 0x09 };
            result = SendCommand(bytesToSend, out response, hasReturn: true, throwException: false);
            validPart = SeparateByteArray(response, new byte[] { ESC, (byte)'s' })[1].Skip(5).ToArray();
            byte[][] counters = SeparateByteArray(validPart, new byte[] { ESC, MFB1 });
            receiptNumber = -1;
            int.TryParse(Encoding.GetString(counters[11]), out receiptNumber);
            if (receiptNumber != -1)
            {
                receiptNumber++;
            }

            grossTotal = vatAGrossTotal + vatBGrossTotal + vatCGrossTotal + vatDGrossTotal + vatEGrossTotal;
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        private byte[] SeparateByteArrayAndGetLast(byte[] source, byte[] separator)
        {
            for (var i = 0; i < source.Length; ++i)
            {
                if (Equals(source, separator, i))
                {
                    var index = i + separator.Length;
                    var part = new byte[source.Length - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    return part;
                }
            }
            throw new Exception("not found");
        }

        /// <summary>
        /// Seperates a byte array using an other byte array as delimiter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private byte[][] SeparateByteArray(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        private bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }

        public override DeviceResult AddSubtotalDiscount(string reason, string reasonExtended, double amountVatA, double amountVatB, double amountVatC, double amountVatD, double amountVatE, double totalAmount)
        {
            //ESC MFB “cb” ESC MFB1 “ac” <DiscountValue> [ESC MFB2 “bd” <PrintLinePre> ]ESC MFE

            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {
                byte[] reasonBytes = Encoding.GetBytes(reason).Take(31).ToArray();//max.31 bytes
                string amountStr = (totalAmount * 100).ToString(NumberFormat);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'c', (byte)'b', ESC, MFB1, (byte)'a', (byte)'c' };
                bytesToSend.AddRange(Encoding.GetBytes(amountStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'b', (byte)'d' });
                bytesToSend.AddRange(reasonBytes);
                bytesToSend.AddRange(new byte[] { ESC, MFE });
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;

            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot add subtotal discount. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult AddLineDiscount(eMinistryVatCategoryCode vatCode, string reason, string reasonExtended, double amount)
        {
            //ESC MFB “fc” ESC MFB1 “ac” <DiscountValue> ESC MFB1 “ca” <VATCategory> [ESC MFB2 “bd” <PrintLinePre> ] ESC MFE

            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {
                byte[] reasonBytes = Encoding.GetBytes(reason).Take(31).ToArray();//max.31 bytes
                byte vatCategory = (byte)(0x30 + (int)vatCode - 1);
                string amountStr = (amount * 100).ToString(NumberFormat);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'f', (byte)'c', ESC, MFB1, (byte)'a', (byte)'c' };
                bytesToSend.AddRange(Encoding.GetBytes(amountStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'a', vatCategory, ESC, MFB2, (byte)'b', (byte)'d' });
                bytesToSend.AddRange(reasonBytes);
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;

            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot add item discount. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult ReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            //ESC MFB “fh” ESC MFB1 “ba” <ArticleName> ESC MFB1 “aa” <SinglePrice> ESC MFB1 “ca” <VATCategory> [ESC MFB2 “bd” <PrintLinePre> ] ESC MFE
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {
                byte vatCategory = (byte)(0x30 + (int)vatCode - 1);
                byte[] trimedDescription = Encoding.GetBytes(ItemDescription).Take(22).ToArray();  //MAX 22 Bytes
                byte[] trimedAdditionalInfo = Encoding.GetBytes(additionalInfo).Take(31).ToArray();  //MAX 31 Bytes
                string totalStr = (lineTotal * 100).ToString(NumberFormat);//String.Format(NumberFormat, "{0:#.00}", total);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'f', (byte)'h', ESC, MFB1, (byte)'b', (byte)'a' };
                bytesToSend.AddRange(trimedDescription);
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'a', (byte)'a' });
                bytesToSend.AddRange(Encoding.GetBytes(totalStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'a', vatCategory, ESC, MFB2, (byte)'b', (byte)'d' });
                bytesToSend.AddRange(trimedAdditionalInfo);
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot add item '" + ItemDescription + "'. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult AddPayment(double amount, ePaymentMethodType paymentMethod, out double remainingBalance, string description, string extraDescription = "")
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_IN_PAYMENT))
            {
                //ESC MFB “ea” ESC MFB1 “af” <TenderValue> [ESC MFB2 “bc” <PrintLine>] ESC MFE
                remainingBalance = 0;
                string amountStr = (amount * 100).ToString(NumberFormat);
                byte[] amountAsciiBytes = Encoding.GetBytes(amountStr);
                byte[] descriptionBytes = Encoding.GetBytes(description).Take(21).ToArray(); //Max 21 bytes
                List<byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'e', (byte)'a', ESC, MFB1, (byte)'a', (byte)'f' };
                bytesToSend.AddRange(amountAsciiBytes);
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'b', (byte)'c' });
                bytesToSend.AddRange(descriptionBytes);
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot add payment. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult PrintIllegal(params FiscalLine[] lines)
        {
            int retries = 5;
            WincorFiscalPrinterStatusCode result;
            do
            {
                if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN | CommandFlagRequirements.DAY_OPEN, forceCancelReceipt: true))
                {
                    byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'a', ESC, MFB1, (byte)'f', (byte)'b', 0x34, ESC, MFE }; //OPEN ILLEGAL RECEIPT
                    byte[] response;
                    result = SendCommand(bytesToSend, out response, throwException: false);
                    if (result == WincorFiscalPrinterStatusCode.SUCCESS)
                    {
                        foreach (FiscalLine line in lines)
                        {
                            byte[] trimedLine = Encoding.GetBytes(line.Value).Where(ch => ch != ESC).Take(40).ToArray();  //MAX 40 Bytes
                            //ESC MFB “ja” ESC MFB1 “fa” <Station> ESC MFB1 “bc” <PrintLine> ESC MFE
                            List<byte> freeprintLineCommandBytes = new List<byte>() { ESC, MFB, (byte)'j', (byte)'a', ESC, MFB1, (byte)'f', (byte)'a', (byte)'3', ESC, MFB1, (byte)'b', (byte)'c' }; //constant part
                            freeprintLineCommandBytes.AddRange(trimedLine);
                            freeprintLineCommandBytes.AddRange(new byte[] { ESC, MFE });

                            bytesToSend = freeprintLineCommandBytes.ToArray();
                            result = SendCommand(bytesToSend, out response, checkForError: false);
                        }
                        ReadDeviceStatus();
                        int internalRetries = 5;
                        do
                        {
                            bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'b', ESC, MFE }; //CLOSE RECEIPT
                            result = SendCommand(bytesToSend, out response, throwException: false);
                            internalRetries--;
                            if (result != WincorFiscalPrinterStatusCode.SUCCESS)
                            {
                                Thread.Sleep(200);//give printer some time to print
                            }
                        } while (internalRetries > 0 && result != WincorFiscalPrinterStatusCode.SUCCESS);
                    }

                }
                else
                {
                    result = WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER;
                    Thread.Sleep(500);
                }
                retries--;
            } while (retries > 0 && result != WincorFiscalPrinterStatusCode.SUCCESS);

            if (result == WincorFiscalPrinterStatusCode.SUCCESS)
            {
                return DeviceResult.SUCCESS;
            }
            else
            {
                string errorMessage = String.Format("Cannot print illegal receipt (freeprint). Error Code: {0} DAY_OPEN: {1} TRANSACTION_OPEN: {2}",
                                                    (int)result,
                                                    NativeFiscalStatus.DayOpen,
                                                    NativeFiscalStatus.TransactionOpen
                                                   );

                throw new POSFiscalPrinterException(this.DeviceName, (int)result, errorMessage);
            }
        }

        public override DeviceResult Feed(int lines = 1)
        {
            //1B 64 n
            byte[] bytesToSend = new byte[] { 0x1B, 0x64, (byte)lines };
            byte[] response;
            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response, throwException: false);
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult GetCurrentDayReceiptsCount(out int receiptsCount)
        {
            //ESC MFB “kf” ESC MFE ESC “+”09H
            receiptsCount = -1;
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.FAILURE;
            if (CheckIfCommandCanExecute(CommandFlagRequirements.NONE))
            {
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'k', (byte)'f', ESC, MFE, ESC, (byte)'+', 0x09 };
                byte[] response;
                result = SendCommand(bytesToSend, out response, true);

                byte[][] splited = SeparateByteArray(response, new byte[] { ESC, MFB1 });
                byte[] legalTickets = splited[11];

                int.TryParse(Encoding.GetString(legalTickets), out receiptsCount);
            }
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override bool HasSlipStation
        {
            get { return true; }
        }

        public override DeviceResult SlipStationHasPaper(out bool hasPaper)
        {
            byte[] bytesToSend = new byte[] { 0x10, 0x04, 0x05 };
            byte[] response;
            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response, hasReturn: true, responseBytesCount: 1);

            if (response.Length > 0 && response[0] == 22)
            {
                hasPaper = true;
            }
            else
            {
                hasPaper = false;
            }

            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SlipStationPrint(params FiscalLine[] lines)
        {
            byte[] bytesToSend;
            byte[] response;
            WincorFiscalPrinterStatusCode result;

            if (CheckIfCommandCanExecute(CommandFlagRequirements.DOCUMENT_OPEN))  //close previous opened document
            {
                //ESC MFB “mg” ESC MFE  DOCUMENT_END
                bytesToSend = new byte[] { ESC, MFB, (byte)'m', (byte)'g', ESC, MFE };
                result = SendCommand(bytesToSend, out response);

            }

            if (CheckIfCommandCanExecute(CommandFlagRequirements.DOCUMENT_NOT_OPEN))
            {
                //ESC MFB “ma” ESC MFE  DOCUMENT_BEGIN
                bytesToSend = new byte[] { ESC, MFB, (byte)'m', (byte)'a', ESC, MFE };
                result = SendCommand(bytesToSend, out response);
            }

            //1B 63 30 04  (ESC 'c' '0' 04) Set station to document station
            bytesToSend = new byte[] { ESC, (byte)'c', (byte)'0', 0x04 };
            result = SendCommand(bytesToSend, out response);

            foreach (FiscalLine line in lines)
            {
                byte[] trimedLine = Encoding.GetBytes(line.Value).Take(51).ToArray();  //MAX 51 Bytes for station 4
                //ESC MFB “ja” ESC MFB1 “fa” <Station> ESC MFB1 “bc” <PrintLine> ESC MFE
                List<byte> freeprintLineCommandBytes = new List<byte>() { ESC, MFB, (byte)'j', (byte)'a', ESC, MFB1, (byte)'f', (byte)'a', (byte)'4', ESC, MFB1, (byte)'b', (byte)'c' }; //constant part
                freeprintLineCommandBytes.AddRange(trimedLine);
                freeprintLineCommandBytes.AddRange(new byte[] { ESC, MFE });

                bytesToSend = freeprintLineCommandBytes.ToArray();
                result = SendCommand(bytesToSend, out response);
            }

            //1B 63 30 04  (ESC 'c' '0' 03) Set station to receipt station
            bytesToSend = new byte[] { ESC, (byte)'c', (byte)'0', 0x03 };
            SendCommand(bytesToSend, out response);

            //ESC MFB “mg” ESC MFE  DOCUMENT_END
            bytesToSend = new byte[] { ESC, MFB, (byte)'m', (byte)'g', ESC, MFE };
            result = SendCommand(bytesToSend, out response);


            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public DeviceResult CancelReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            //ESC MFB “fi” ESC MFB1 “ba” <ArticleName> ESC MFB1 “aa” <SinglePrice> ESC MFB1 “ca” <VATCategory>ESC MFE
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {
                byte vatCategory = (byte)(0x30 + (int)vatCode - 1);
                byte[] trimedDescription = Encoding.GetBytes(ItemDescription).Take(22).ToArray();  //MAX 22 Bytes
                byte[] trimedAdditionalInfo = Encoding.GetBytes(additionalInfo).Take(31).ToArray();  //MAX 31 Bytes
                double total = finalUnitPrice * itemQuantity;
                string totalStr = (total * 100).ToString(NumberFormat);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'f', (byte)'i', ESC, MFB1, (byte)'b', (byte)'a' };
                bytesToSend.AddRange(trimedDescription);
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'a', (byte)'a' });
                bytesToSend.AddRange(Encoding.GetBytes(totalStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'a', vatCategory, ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot cancel return of item '" + ItemDescription + "'. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public DeviceResult CancelItemSell(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN | CommandFlagRequirements.TRANSACTION_NOT_IN_PAYMENT))
            {

                /* ARTICLE_SELL_VOID
                 * ESC MFB “fb” ESC MFB1 “ba” <ArticleName> ESC MFB1 “aa” <SinglePrice> ESC MFB1 “ca” <VATCategory> ESC MFE
                 */
                byte vatCategory = (byte)(0x30 + (int)vatCode - 1);
                byte[] trimedDescription = Encoding.GetBytes(ItemDescription).Take(22).ToArray();  //MAX 22 Bytes
                double total = finalUnitPrice * itemQuantity;
                string totalStr = (total * 100).ToString(NumberFormat);//String.Format(NumberFormat, "{0:#.00}", total);

                List<byte> bytesToSend = new List<byte>() { ESC, MFB, (byte)'f', (byte)'b', ESC, MFB1, (byte)'b', (byte)'a' };
                bytesToSend.AddRange(trimedDescription);
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'a', (byte)'a' });
                bytesToSend.AddRange(Encoding.GetBytes(totalStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'a', vatCategory, ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, checkForError: false);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot cancel sell of item  '" + ItemDescription + "'. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult ReturnItemAndCancelIt(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            return DeviceResult.SUCCESS;

            ////Removed. Uncomment to return to the previous functionality.
            ////---------

            // DeviceResult result = this.ReturnItem(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, lineTotal, vatCode, vatFactor);
            //result = this.CancelReturnItem(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, vatCode, vatFactor);
            //return result;

            ////---------
        }

        public override DeviceResult SellItemAndCancelIt(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity,
            double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal)
        {
            return DeviceResult.SUCCESS;

            ////Removed. Uncomment to return to the previous functionality.
            ////---------

            //DeviceResult result = this.SellItem(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, lineTotal, vatCode, vatFactor);
            //result = this.CancelItemSell(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, vatCode, vatFactor);
            //return result;

            ////---------
        }

        public override DeviceResult ReprintZReportsDateToDate(DateTime fromDate, DateTime toDate, eReprintZReportsMode mode)
        {
            //Non-Summarized
            //ESC MFB “gb” ESC MFB1 “ea” <FromDate> ESC MFB1 “eb” <ToDate> [ESC MFB2 “fa” <Station>] ESC MFE

            //Summarized
            //ESC MFB “gd” ESC MFB1 “ea” <FromDate> ESC MFB1 “eb” <ToDate> [ESC MFB2 “fa” <Station>] ESC MFE

            //Signatures REPORT_MD_DATE2DATE
            //ESC MFB “gg” ESC MFB1 “ea” <FromDate> ESC MFB1 “eb” <ToDate> [ESC MFB2 “fa” <Station>] ESC MFE

            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN))
            {
                string fromDateStr = fromDate.ToString("dd/MM/yy"); //format DD/MM/YY
                string toDateStr = toDate.ToString("dd/MM/yy");
                //char commandDifference = summarized ? 'd' : 'b';

                char commandDifference = '\0';
                switch (mode)
                {
                    case eReprintZReportsMode.ANALYTIC:
                        commandDifference = 'b';
                        break;
                    case eReprintZReportsMode.SUMMARIZED:
                        commandDifference = 'd';
                        break;
                    case eReprintZReportsMode.SIGNATURES:
                        commandDifference = 'g';
                        break;
                }


                List<Byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'g', (byte)commandDifference, ESC, MFB1, (byte)'e', (byte)'a', };
                bytesToSend.AddRange(Encoding.GetBytes(fromDateStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'e', (byte)'b' });
                bytesToSend.AddRange(Encoding.GetBytes(toDateStr));
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }

        public override DeviceResult ReprintZReports(int fromZ, int toZ, eReprintZReportsMode mode)
        {
            //Non-Summarized REPORT_EOD2EOD
            //ESC MFB “gc” ESC MFB1 “da” <FromBlock> ESC MFB1 “db” <ToBlock> [ESC MFB2 “fa” <Station>] ESC MFE

            //Summarized  REPORT_TOTAL_EOD2EOD
            //ESC MFB “ge” ESC MFB1 “da” <FromBlock> ESC MFB1 “db” <ToBlock> [ESC MFB2 “fa” <Station>] ESC MFE

            //Signatures REPORT_MD_EOD2EOD
            //ESC MFB “gh” ESC MFB1 “da” <FromBlock> ESC MFB1 “db” <ToBlock> [ESC MFB2 “fa” <Station>] ESC MFE  

            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN))
            {
                char commandDifference = '\0';
                switch (mode)
                {
                    case eReprintZReportsMode.ANALYTIC:
                        commandDifference = 'c';
                        break;
                    case eReprintZReportsMode.SUMMARIZED:
                        commandDifference = 'e';
                        break;
                    case eReprintZReportsMode.SIGNATURES:
                        commandDifference = 'h';
                        break;
                }

                List<Byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'g', (byte)commandDifference, ESC, MFB1, (byte)'d', (byte)'a', };
                bytesToSend.AddRange(Encoding.GetBytes(fromZ.ToString()));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'d', (byte)'b' });
                bytesToSend.AddRange(Encoding.GetBytes(toZ.ToString()));
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }


        public override DeviceResult ReprintReceiptsOfCurrentZ(int fromReceiptNumber, int toReceiptNumber)
        {
            //EJ_REPRINT_TICKET2TICKET_BY_DATE

            //ESC MFB q e [ESC MFB2 h i <EJMEMLabel>] [ESC MFB2 d a <FromEOD>] [ESC MFB2 d g <FromTicket>] [ESC MFB2 d h <ToTicket>] [ESC MFB2 h j <FromDateTime>] 
            //[ESC MFB2 h k <ToDateTime>] [ESC MFB2 f e <TicketType>] [ESC MFB2 f g <FromTicketTypeSeqNo>] [ESC MFB2 f h <ToTicketTypeSeqNo >] [ESC MFB2 f f <TicketStatus>] ESC MFE

            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN))
            {
                int currentZ = -1;
                GetCurrentZNumber(out currentZ);
                if (currentZ < 1)
                {
                    return DeviceResult.FAILURE;
                }

                List<Byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'q', (byte)'e', ESC, MFB2, (byte)'h', (byte)'i' };
                bytesToSend.AddRange(Encoding.GetBytes(String.Empty));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'d', (byte)'a' });
                bytesToSend.AddRange(Encoding.GetBytes(currentZ.ToString()));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'d', (byte)'g' });
                bytesToSend.AddRange(Encoding.GetBytes("0"));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'d', (byte)'h' });
                bytesToSend.AddRange(Encoding.GetBytes("65535"));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'f', (byte)'e' });
                bytesToSend.AddRange(Encoding.GetBytes("1")); //1 is legal receipts
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'f', (byte)'g' });
                bytesToSend.AddRange(Encoding.GetBytes(fromReceiptNumber.ToString()));
                bytesToSend.AddRange(new byte[] { ESC, MFB2, (byte)'f', (byte)'h' });
                bytesToSend.AddRange(Encoding.GetBytes(toReceiptNumber.ToString()));
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;

            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }

        protected override int LineCharsForIllegalPrinting
        {
            get;
        }

        public override DeviceResult CashIn(double amount, string message)
        {
            return CashInOrOutTransaction(amount, message, true);
        }

        public override DeviceResult CashOut(double amount, string message)
        {
            return CashInOrOutTransaction(amount, message, false);
        }

        protected virtual DeviceResult CashInOrOutTransaction(double amount, string message, bool isCashIn)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN))
            {
                byte receiptType = isCashIn == true ? (byte)0x36 : (byte)0x37;
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'a', ESC, MFB1, (byte)'f', (byte)'b', receiptType, ESC, MFE }; //OPEN CASH-IN/OUT RECEIPT
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend, out response);

                //SPECIAL_CASH_IN
                //ESC MFB “ia” ESC MFB1 “ag” <CashValue> ESC MFE

                //SPECIAL_CASH_OUT
                //ESC MFB “ib” ESC MFB1 “ag” <CashValue> ESC MFE

                byte commandDiff = isCashIn == true ? (byte)'a' : (byte)'b';
                string amountStr = (amount * 100).ToString(NumberFormat);
                List<byte> bytesToSendLst = new List<byte>() { ESC, MFB, (byte)'i', commandDiff, ESC, MFB1, (byte)'a', (byte)'g' };
                bytesToSendLst.AddRange(Encoding.GetBytes(amountStr));
                bytesToSendLst.AddRange(new byte[] { ESC, MFE });
                bytesToSend = bytesToSendLst.ToArray();
                result = SendCommand(bytesToSend, out response);

                byte[] freeprintLine = Encoding.GetBytes(message).Take(40).ToArray();  //MAX 40 Bytes

                List<byte> freeprintLineCommandBytes = new List<byte>() { ESC, MFB, (byte)'j', (byte)'a', ESC, MFB1, (byte)'f', (byte)'a', (byte)'3', ESC, MFB1, (byte)'b', (byte)'c' }; //constant part
                freeprintLineCommandBytes.AddRange(freeprintLine);
                freeprintLineCommandBytes.AddRange(new byte[] { ESC, MFE });

                bytesToSend = freeprintLineCommandBytes.ToArray();
                result = SendCommand(bytesToSend, out response); //freeprint


                bytesToSend = new byte[] { ESC, MFB, (byte)'b', (byte)'b', ESC, MFE }; //CLOSE RECEIPT
                result = SendCommand(bytesToSend, out response);

                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot print cash in. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult PrintFiscalMemoryBlocks(int fromBlock, int toBlock)
        {
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_NOT_OPEN))
            {
                //ESC MFB “ga” ESC MFB1 “da” <FromBlock> ESC MFB1 “db” <ToBlock> [ESC MFB2 “fa” <Station>] ESC MFE

                List<byte> bytesToSendLst = new List<byte>() { ESC, MFB, (byte)'g', (byte)'a', ESC, MFB1, (byte)'d', (byte)'a' };
                bytesToSendLst.AddRange(Encoding.GetBytes(fromBlock.ToString()));
                bytesToSendLst.AddRange(new byte[] { ESC, MFB1, (byte)'d', (byte)'b' });
                bytesToSendLst.AddRange(Encoding.GetBytes(toBlock.ToString()));
                bytesToSendLst.AddRange(new byte[] { ESC, MFE });
                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSendLst.ToArray(), out response);

                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;

            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot print Fiscal memory blocks. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }

        }

        public override DeviceResult ReceiptSubtotal(string info)
        {
            //ESC MFB “ca” [ESC MFB2 “be” <PrintLinePost> ] ESC MFE
            if (CheckIfCommandCanExecute(CommandFlagRequirements.TRANSACTION_OPEN))
            {
                List<byte> bytesToSendLst = new List<byte>() { ESC, MFB, (byte)'c', (byte)'a', ESC, MFB2, (byte)'b', (byte)'e' };
                bytesToSendLst.AddRange(Encoding.GetBytes(info).Take(31));  //max 31 bytes
                bytesToSendLst.AddRange(new byte[] { ESC, MFE });
                byte[] response;

                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSendLst.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot print Receipt subtotal. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        private byte[] GetPrintModeEscapeCommand(ePrintType type)
        {
            //From the manual:
            //“0” – Normal
            //“1” - Double Wide
            //“2” - Double High
            //“3” – Quadruple

            int mode = 0;

            if (type == ePrintType.DOUBLE_WIDTH)
            {
                mode = 1;
            }
            else if (type == ePrintType.DOUBLE_HEIGHT)
            {
                mode = 2;
            }
            else if (type == ePrintType.DOUBLE_BOTH)
            {
                mode = 3;
            }
            string modeAsAscii = mode.ToString();
            byte[] asciiModeBytes = Encoding.GetBytes(modeAsAscii);
            List<byte> cmd = new List<byte> { ESC, (byte)'!' };
            if (type == ePrintType.NORMAL)
            {
                cmd.Add(0x03);
            }
            else
            {
                cmd.AddRange(asciiModeBytes);
            }
            return cmd.ToArray();
        }

        private string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public override DeviceResult SetHeader(params FiscalLine[] lines)
        {
            //ESC MFB “hb” ESC MFB1 “bb” <ReceiptHeader> ESC MFE 
            //max. 6 lines and max. 239 bytes (CR LF included for each line)

            if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_NOT_OPEN))
            {
                string finalText = "";

                foreach (FiscalLine line in lines)
                {
                    string lineText = line.Value + "\r\n";
                    byte[] escCmd;
                    escCmd = GetPrintModeEscapeCommand(line.Type);

                    int lineMaxChars = 0;
                    int linePadChars = 0;
                    if (line.Type == ePrintType.DOUBLE_WIDTH || line.Type == ePrintType.DOUBLE_BOTH)
                    {
                        lineMaxChars = 20 - escCmd.Length;
                        linePadChars = 20;
                    }
                    else
                    {
                        lineMaxChars = 40 - escCmd.Length;
                        linePadChars = 40;
                    }

                    //move to center

                    lineText = TruncateLongString(lineText, lineMaxChars);
                    int spaces = linePadChars - lineText.Length;
                    int padLeft = spaces / 2;
                    string space = "".PadLeft(padLeft);
                    if (!String.IsNullOrWhiteSpace(lineText))
                    {
                        lineText = Encoding.GetString(escCmd) + space + lineText;
                    }
                    finalText += lineText;
                }

                finalText = finalText.TrimEnd() + "\r\n";

                List<byte> bytesToSendLst = new List<byte>() { ESC, MFB, (byte)'h', (byte)'b', ESC, MFB1, (byte)'b', (byte)'b' };
                bytesToSendLst.AddRange(Encoding.GetBytes(finalText));
                bytesToSendLst.AddRange(new byte[] { ESC, MFE, });
                byte[] response;

                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSendLst.ToArray(), out response);
                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            else
            {
                throw new POSFiscalPrinterException(this.DeviceName, (int)WincorFiscalPrinterStatusCode.EM_CMD_WRONG_ORDER, "Cannot Set Header. DAY_OPEN: " + NativeFiscalStatus.DayOpen + " TRANSACTION_OPEN: " + NativeFiscalStatus.TransactionOpen + " TRANSACTION_IN_PAYMENT: " + NativeFiscalStatus.TransactionInPayment);
            }
        }

        public override DeviceResult ReadHeader(out FiscalLine[] lines)
        {
            throw new NotSupportedException();
        }

        public override DeviceResult ReadVatRates(out double vatRateA, out double vatRateB, out double vatRateC, out double vatRateD, out double vatRateE)
        {
            List<byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'k', (byte)'l', ESC, MFE, ESC, (byte)'+', 0x09 };

            byte[] response;
            WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response, hasReturn: true);
            byte[] validPart = SeparateByteArray(response, new byte[] { ESC, (byte)'s' })[1].Skip(5).ToArray();
            byte[][] vatRates = SeparateByteArray(validPart, new byte[] { ESC, MFB1 });
            byte[] vatA = vatRates[0];
            byte[] vatB = vatRates[1];
            byte[] vatC = vatRates[2];
            byte[] vatD = vatRates[3];
            byte[] vatE = vatRates[4].Take(vatRates[4].Length - 2).ToArray();

            Double.TryParse(Encoding.GetString(vatA), out vatRateA);
            Double.TryParse(Encoding.GetString(vatB), out vatRateB);
            Double.TryParse(Encoding.GetString(vatC), out vatRateC);
            Double.TryParse(Encoding.GetString(vatD), out vatRateD);
            Double.TryParse(Encoding.GetString(vatE), out vatRateE);

            vatRateA /= 10000;
            vatRateB /= 10000;
            vatRateC /= 10000;
            vatRateD /= 10000;
            vatRateE /= 10000;
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SetVatRates(double vatRateA, double vatRateB, double vatRateC, double vatRateD, double vatRateE)
        {
            //ESC MFB “ha” ESC MFB1 “cb” <VATRate_A> ESC MFB1 “cc” <VATRate_B>
            //ESC MFB1 “cd” <VATRate_C> ESC MFB1 “ce” <VATRate_D>
            //ESC MFB1 “cf” <VATRate_E> ESC MFE
            if (vatRateA >= 1 ||
                vatRateB >= 1 ||
                vatRateC >= 1 ||
                vatRateD >= 1 ||
                vatRateE >= 1)
            {
                throw new ArgumentException("Vat Rates must be < 1");
            }


            if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_NOT_OPEN))
            {
                string vatRateAStr = (vatRateA * 10000).ToString("0000", NumberFormat);
                string vatRateBStr = (vatRateB * 10000).ToString("0000", NumberFormat);
                string vatRateCStr = (vatRateC * 10000).ToString("0000", NumberFormat);
                string vatRateDStr = (vatRateD * 10000).ToString("0000", NumberFormat);
                string vatRateEStr = (vatRateE * 10000).ToString("0000", NumberFormat);

                List<byte> bytesToSend = new List<byte> { ESC, MFB, (byte)'h', (byte)'a', ESC, MFB1, (byte)'c', (byte)'b' };
                bytesToSend.AddRange(Encoding.GetBytes(vatRateAStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'c' });
                bytesToSend.AddRange(Encoding.GetBytes(vatRateBStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'d' });
                bytesToSend.AddRange(Encoding.GetBytes(vatRateCStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'e' });
                bytesToSend.AddRange(Encoding.GetBytes(vatRateDStr));
                bytesToSend.AddRange(new byte[] { ESC, MFB1, (byte)'c', (byte)'f' });
                bytesToSend.AddRange(Encoding.GetBytes(vatRateEStr));
                bytesToSend.AddRange(new byte[] { ESC, MFE });

                byte[] response;
                WincorFiscalPrinterStatusCode result = SendCommand(bytesToSend.ToArray(), out response);

                return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }

            return DeviceResult.FAILURE;
        }

        public override DeviceResult SetCashierID(string id)
        {
            //ESC MFB “hf” ESC MFB1 “bg” <CashierID> ESC MFE
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.FAILURE;
            if (CheckIfCommandCanExecute(CommandFlagRequirements.NONE, commandNeedsPaper: false))
            {
                List<byte> bytesToSendLst = new List<byte>() { ESC, MFB, (byte)'h', (byte)'f', ESC, MFB1, (byte)'b', (byte)'g' };
                bytesToSendLst.AddRange(Encoding.GetBytes(id).Take(4));
                bytesToSendLst.AddRange(new byte[] { ESC, MFE });
                byte[] response;
                result = SendCommand(bytesToSendLst.ToArray(), out response, throwException: false);
            }
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult GetDayAmounts(out double vatAmountA, out double vatAmountB, out double vatAmountC, out double vatAmountD, out double vatAmountE,
                                                   out double netAmountA, out double netAmountB, out double netAmountC, out double netAmountD, out double netAmountE)
        {
            ////GET_DAY_VAT_AMOUNT
            ////ESC MFB “kd” ESC MFE ESC “+”09H 
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.FAILURE;
            vatAmountA = 0;
            vatAmountB = 0;
            vatAmountC = 0;
            vatAmountD = 0;
            vatAmountE = 0;
            netAmountA = 0;
            netAmountB = 0;
            netAmountC = 0;
            netAmountD = 0;
            netAmountE = 0;

            if (CheckIfCommandCanExecute(CommandFlagRequirements.DAY_OPEN))
            {
                byte[] bytesToSend = new byte[] { ESC, MFB, (byte)'k', (byte)'d', ESC, MFE, ESC, (byte)'+', 0x09 };
                byte[] response;

                result = SendCommand(bytesToSend, out response, hasReturn: true, throwException: false);

                byte[] validPart = SeparateByteArray(response, new byte[] { ESC, (byte)'s' })[1].Skip(5).ToArray();
                byte[][] vatAmounts = SeparateByteArray(validPart, new byte[] { ESC, MFB1 });
                byte[] vatA = vatAmounts[0];
                byte[] vatB = vatAmounts[1];
                byte[] vatC = vatAmounts[2];
                byte[] vatD = vatAmounts[3];
                byte[] vatE = vatAmounts[4].Take(vatAmounts[4].Length - 2).ToArray();

                Double.TryParse(Encoding.GetString(vatA), out vatAmountA);
                Double.TryParse(Encoding.GetString(vatB), out vatAmountB);
                Double.TryParse(Encoding.GetString(vatC), out vatAmountC);
                Double.TryParse(Encoding.GetString(vatD), out vatAmountD);
                Double.TryParse(Encoding.GetString(vatE), out vatAmountE);

                vatAmountA /= 10000;
                vatAmountB /= 10000;
                vatAmountC /= 10000;
                vatAmountD /= 10000;
                vatAmountE /= 10000;

                ////GET_DAY_NET_SALES
                ////ESC MFB “ke” ESC MFE ESC “+”09H
                bytesToSend = new byte[] { ESC, MFB, (byte)'k', (byte)'e', ESC, MFE, ESC, (byte)'+', 0x09 };

                result = SendCommand(bytesToSend, out response, hasReturn: true, throwException: false);

                validPart = SeparateByteArray(response, new byte[] { ESC, (byte)'s' })[1].Skip(5).ToArray();
                byte[][] netAmounts = SeparateByteArray(validPart, new byte[] { ESC, MFB1 });
                byte[] netA = netAmounts[0];
                byte[] netB = netAmounts[1];
                byte[] netC = netAmounts[2];
                byte[] netD = netAmounts[3];
                byte[] netE = netAmounts[4].Take(vatAmounts[4].Length - 2).ToArray();

                Double.TryParse(Encoding.GetString(netA), out netAmountA);
                Double.TryParse(Encoding.GetString(netB), out netAmountB);
                Double.TryParse(Encoding.GetString(netC), out netAmountC);
                Double.TryParse(Encoding.GetString(netD), out netAmountD);
                Double.TryParse(Encoding.GetString(netE), out netAmountE);

                netAmountA /= 10000;
                netAmountB /= 10000;
                netAmountC /= 10000;
                netAmountD /= 10000;
                netAmountE /= 10000;
            }
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override bool CheckIfCanPrintIllegal(out string reason)
        {
            reason = null;
            this.ReadDeviceStatus();

            if (this.NativeFiscalStatus.EJDataPending)
            {
                reason = Resources.POSClientResources.EJ_DATA_PENDING;
                return false;
            }

            if (this.NativeFiscalStatus.DayOpen == false)
            {
                reason = Resources.POSClientResources.FISCAL_PRINTER_DAY_NOT_OPEN;
                return false;
            }

            return true;
        }

        public override DeviceResult OpenDrawer(string openDrawerCustomString)
        {
            WincorFiscalPrinterStatusCode result = WincorFiscalPrinterStatusCode.FAILURE;
            byte[] bytesToSend = new byte[] { ESC, 0x70, 0, 0x64, 0x64 }, response;
            result = SendCommand(bytesToSend, out response, checkForError: false);
            return result == WincorFiscalPrinterStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }
    }

}
