using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware.Common.Exceptions;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ITS.POS.Hardware.Micrelec.Fiscal
{
    public static class extens
    {
        public static byte ReadByteExtended(this SerialPort port)
        {
            byte[] toRead = new byte[1];
            port.Read(toRead, 0, 1);
            return toRead[0];
        }
    }

    public class MicrelecFiscalPrinter :
        FiscalPrinter
    {

        private static Dictionary<int, string> _ErrorCodeDescriptionMapping = new Dictionary<int, string>()
        {
            { 0x00,"None"},
            { 0x01,"Check the command's field count)"},
            { 0x02,"A field is long: check it & retry"},
            { 0x03,"A field is small: check it & retry"},
            { 0x04,"A field size is wrong: check it & retry"},
            { 0x05,"Check ranges or types in command"},
            { 0x06,"Correct the request code (unknown)"},
            { 0x07,"The requested fiscal record number is wrong"},
            { 0x08,"The requested fiscal record type is wrong"},
            { 0x09,"Correct the specified printing style"},
            { 0x0A,"Issue a Z report to close the day"},
            { 0x0B,"Short the 'clock' jumper and retry"},
            { 0x0C,"Check the date/time range. Also check if date is prior to a date of a fiscal record"},
            { 0x0D,"No suggested action; the operation cannot be executed in the specified period"},
            { 0x0E,"Wait for the device to get ready"},
            { 0x0F,"No suggested action; the header programming cannot be executed because the Fiscal memory cannot hold more records"},
            { 0x10,"The specified command requires no open signature block for proceeding. Close the block and retry"},
            { 0x11,"Open a transaction first"},
            { 0x12,"Error in signing the electronic data"},
            { 0x13,"error in signing"},
            { 0x14,"Means that 24 hours passed from the last Z closure. Issue a Z and retry"},
            { 0x15,"The specified Z closure number does not exist. Pass an existing Z number"},
            { 0x16,"The requested Z record is unreadable (damaged). Device requires service"},
            { 0x17,"The user is accessing the device by manual operation. The protocol usage is suspended until the user terminates the keyboard browsing. Just wait or inform application user."},
            { 0x18,"Take a Z Report in order to continue issuing an invoice"},
            { 0x19,"Replace the paper roll and retry"},
            { 0x1A,"Printer disconnection. Service required"},
            { 0x1B,"Fiscal disconnection. Service required"},
            { 0x1C,"Mostly fiscal errors. Service required"},
            { 0x1D,"Need fiscal replacement. Service"},
            { 0x1E,"There are no data to be signed"},
            { 0x1F,"The signature number is not in range"},
            { 0x20,"If problem persists, service required"},
            { 0x21,"Close the day to reprint signature"},
            { 0x22,"Signature cannot be reprinted due to CMO error. Call service"},
            { 0x23,"This means that the RTC has invalid Data and needs to be reprogrammed. As a consequence, service is needed"},
            { 0x24,"The Jumper are on, They must be removed for the operation to continue."},
            { 0x25,"Error Sale type It must be S/V/R"},
            { 0x26,"Department’s code number out of range (1-5)"},
            { 0x27,"The VAT rate sent by the PC isn’t equal to the CITIZEN CT-S601’s one"},
            { 0x28,"Payment’s code is out of range (1-3) 1=CASH, 2=CARD, 3=CREDIT"},
            { 0x29,"Connection with Printer Head cannot be established"},
            { 0x2A,"The printer tray is opened"},
            { 0x2B,"The slip printer is not ready"},
            { 0x2C,"The printer's Head is damaged"},
            { 0x2D,"Sensor is damaged"},
            { 0x2E,"The Sensor cannot read"},
            { 0x2F,"There are illegal receipts in the journal that must be read"},
            { 0x30,"There are legal receipts in the journal that must be read"},
            { 0x31,"The requested illegal receipt doesn’t exist in the electronic journal"},
            { 0x32,"CARD reading problem"},
            { 0x33,"The requested legal receipt doesn’t exist in the electronic journal"},
            { 0x34,"There are no more receipts to be read in the CARD"},
            { 0x35,"CITIZEN CT-S601 must first be told about the reading of the CARD before the CARD’s reading begins"},
            { 0x36,"The CARD’s reading isn’t finished"},
            { 0x37,"A record hasn’t been read"},
            { 0x38,"The CARD’s reading was successful"},
            { 0x39,"Error reading the CARD, please try again"},
            { 0x3A,"CITIZEN CT-S601 must first be told about the reading of the CARD before the CARD’s reading begins"},
            { 0x3B,"DAY isn’t opened and no transactions are present"},
            { 0x3C,"No more than 6 comment lines can be printed on the receipt"},
            { 0x3D,"The CARD’s data transfer to the PC isn’t over yet"},
            { 0x3E,"Printer is disconnected"},
            { 0x3F,"Another CITIZEN CT-S601’s function is in progress"},
            { 0x40,"There is no opened receipt"},
            { 0x41,"There is an opened receipt"},
            { 0x42,"No more VTA codes can be programmed in the fiscal memory"},
            { 0x43,"Cash in is in progress"},
            { 0x44,"Cash out is in progress"},
            { 0x45,"Payment is in progress"},
            { 0x46,"No zero Discount/Markup is allowed"},
            { 0x47,"Greater Discount than the CITIZEN CT-S601’s VAT amount"},
            { 0x48,"The discount exceeds the minimum transaction amount"},
            { 0x49,"VAT’s allocation’s totals do not match"},
            { 0x4A,"No negative sales-transactions are allowed"},
            { 0x4B,"The receipt must be closed in order for the function to continue"},
            { 0x4C,"CARD is full, it must be read"},
            { 0x4D,"The VAT rate cannot be 0"},
            { 0x4E,"No equal VAT rates in different categories"},
            { 0x4F,"Zero sale’s price cannot occur"},
            { 0x50,"There are no transactions-A X Report cannot be issued"},
            { 0x51,"DATE/TIME Error. Call service"},
            { 0x52,"CARD error. The CITIZEN CT-S601 cannot perform sales"},
            { 0x53,"PLU Internal Code Error (1-200)"},
            { 0x54,"Category Code Error (1-20)"},
            { 0x55,"Department Code Error (1-5)"},
            { 0x56,"The BMP Index Number is not correct"},
            { 0x57,"Turn off the CITIZEN CT-S601 and try again"},
            { 0x58,"The Flash CARD must be read. The machine is in an after-CMOS status"},
            { 0x59,"There is no payment amount to be cancelled"},
            { 0x5A,"A zero payment cannot be cancelled"},
            { 0x5B,"The CITIZEN CT-S601 is not in payment mode"},
            { 0x5C,"The Barcode Data are not valid"},
            { 0x5D,"The BMP Data are damaged"},
            { 0x5E,"Wrong clerk index"},
            { 0x5F,"Wrong clerk password"},
            { 0x60,"Wrong Price"},
            { 0x61,"Invalid Discount/Markup Type"},
            { 0x62,"Wrong Discount/Markup Index"},
            { 0x63,"Maximum Number of Sales in Receipt"},
            { 0x64,"Battery Li error"},
            { 0x65,"Access Denied for current clerk"},
            { 0x66,"Wrong Baud Rate"},
            { 0x67,"Quantity error"},
            { 0x68,"After Ticket Discount"},
            { 0x69,"The ticket is inactive"},
            { 0x6A,"Discount/Markup limit error"},
            { 0x6B,"Blank Description is not allowed"},
            { 0x6C,"Error in barcode"},
            { 0x6D,"The receipt cannot close, negative total"},
            { 0x6E,"Wrong Client index"},
            { 0x6F,"Wrong Client code"},
            { 0x70,"This Payment type cannot give change"},
            { 0x71,"Must insert amount for payment"},
            { 0x72,"The header is same with previous"},
            { 0x73,"There is an error and must use printer keyboard"},
            { 0x74,"Total of receipt exceed the limit"},
            { 0x75,"Daily total sales exceed the limit"},
            { 0x76,"There is a problem with fiscal communication"},
            { 0x77,"NAND memory is full"},
            { 0x78,"Wrong AFM"},
            { 0x79,"The Electronic Journal is empty"},
            { 0x7A,"Invalid IP Address"},
            { 0x7B,"Refund is not allowed"},
            { 0x7C,"Void is not allowed"},
            { 0x7D,"Out of range amount"},
            { 0x7E,"The header must have at least 1 line"},
            { 0x7F,"Clerk is inactive"},
            { 0x80,"There are not daily transactions"},
            { 0x81,"You must programming AFM"},
            { 0x82,"Format SD fail, SD is unformatted"},
            { 0x83,"Wrong Time"},
            { 0x84,"You must call Technician"},
            { 0x85,"Cannot open EJ file"},
            { 0x86,"Cannot write EJ file"},
            { 0x87,"Cannot read EJ file"},
            { 0x88,"Wrong AES Code"},
            { 0x89,"Wrong Coupon Index/Barcode"},
            { 0x8A,"Error in Ethernet communication"},
            { 0x8B,"Error while upload files in GGPS"}
            };


        protected virtual Dictionary<int, string> ErrorCodeDescriptionMapping
        {
            get
            {
                return _ErrorCodeDescriptionMapping;
            }
        }

        protected NumberFormatInfo NumberFormat = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." };

        protected readonly string STX = "\x0002";
        protected readonly string ETX = "\x0003";
        protected readonly string ENQ = "\x0005";
        protected readonly string ACK = "\x0006";
        protected readonly string NAK = "\x0015";
        protected readonly string CAN = "\x0018";
        protected readonly byte bSTX = 2;
        protected readonly byte bETX = 3;
        protected readonly byte bENQ = 5;
        protected readonly byte bACK = 6;
        protected readonly byte bNAK = 0x15;
        protected readonly byte bCAN = 0x18;
        protected int MessFL = 0;
        protected int MessFL1 = 0;

        protected StreamWriter log = null;
        private byte[] _byteResult;
        private int _LineChars;
        private int _CommandChars;

        ~MicrelecFiscalPrinter()
        {
            if (log != null)
            {

                log.Dispose();
            }

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

        protected string PrepareString(string input, int maxChars = 30)
        {
            string output = input ?? "";
            return String.Concat(output.Replace('/', '\\').Take(maxChars));
        }

        public MicrelecFiscalPrinter(ConnectionType conType, String deviceName, int posID, int lineChars, int commandChars)
            : base(conType, deviceName, posID, lineChars, commandChars)
        {
            FiscalStatus = new MicrelecFiscalPrinterStatus();
            FiscalDayStartedOnFirstReceipt = true;
            NumberOfReceiptsIncludeCanceled = true;
            _LineChars = lineChars;
            _CommandChars = commandChars > 0 ? commandChars : 30;
        }

        public override bool IsTotalVatCalculationPerReceipt
        {
            get { return false; }
        }

        public override Version FiscalVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }


        public override DeviceResult OpenFiscalDay(int posID)
        {
            //No need to do anything, just check device
            this.OpenLegalReceipt();
            this.CancelLegalReceipt();
            return ReadDeviceStatus();
        }

        public override double MaximumPaymentAmount
        {
            get { return 9999999.99; }
        }

        public override DeviceResult ReadDeviceStatus()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("?/", out fields, true, false);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult GetTransactionInfo(out double vatAGrossTotal, out double vatBGrossTotal, out double vatCGrossTotal, out double vatDGrossTotal, out double vatEGrossTotal,
            out int receiptNumber, out double grossTotal)
        {

            String[] fields;
            MicrelecStatusCode result = SendCommand("9/", out fields);
            try
            {
                double.TryParse(fields[0], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatAGrossTotal);
                double.TryParse(fields[1], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatBGrossTotal);
                double.TryParse(fields[2], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatCGrossTotal);
                double.TryParse(fields[3], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatDGrossTotal);
                double.TryParse(fields[4], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatEGrossTotal);
                int.TryParse(fields[5], out receiptNumber);
                double.TryParse(fields[6], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out grossTotal);

                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                vatAGrossTotal = 0;
                vatBGrossTotal = 0;
                vatCGrossTotal = 0;
                vatDGrossTotal = 0;
                vatEGrossTotal = 0;
                receiptNumber = 0;
                grossTotal = 0;
                return DeviceResult.FAILURE;
            }
        }
        public override DeviceResult IssueZReport(String pathToAbc, out int zReportNumber, out string pathToEJFiles)
        {
            String abcPath = pathToAbc.TrimEnd('\\') + "\\";
            pathToEJFiles = "";
            zReportNumber = 0;
            //Step 1: Start Read fiscal Memory
            String[] fields;
            MicrelecStatusCode res;//= SendCommand("A/", out fields);
            StreamWriter writer = null;
            int retries = 0;

            do
            {
                res = SendCommand("A/", out fields, throwException: false);
                //Application.DoEvents();
                //Thread.Sleep(50);
                retries++;
            } while (res != MicrelecStatusCode.SUCCESS && retries < 100); // busy or code 128, Asks for Zero Z, send again

            if (res != MicrelecStatusCode.SUCCESS)
            {
                if (this.ErrorCodeDescriptionMapping.ContainsKey((int)res))
                {
                    throw new POSFiscalPrinterException(this.DeviceName, (int)res, this.ErrorCodeDescriptionMapping[(int)res]);
                }
                else
                {
                    throw new POSFiscalPrinterException(this.DeviceName, (int)res, "Unknown Error Code: " + (int)res);
                }
            }

            //Step 2: Reading fiscal Memory
            bool finishedReadingFiles = false;

            int result;
            //List<String> files = new List<string>();

            do
            {
                retries = 0;
                do
                {
                    res = SendCommand("Q/", out fields, throwException: false);
                    //Application.DoEvents();
                    //Thread.Sleep(50);
                    retries++;
                } while (res != MicrelecStatusCode.SUCCESS && retries < 100);

                if (res != MicrelecStatusCode.SUCCESS)
                {
                    if (this.ErrorCodeDescriptionMapping.ContainsKey((int)res))
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, (int)res, this.ErrorCodeDescriptionMapping[(int)res]);
                    }
                    else
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, (int)res, "Unknown Error Code: " + (int)res);
                    }
                }

                int.TryParse(fields[0], out result);
                //result: fields[0]
                //z number: fields[1]
                //filename: fields[2]

                switch (result)
                {
                    case 0: //file start
                        string zReportNumberString = fields[1].Trim();
                        if (Int32.TryParse(zReportNumberString, out zReportNumber) == false)
                        {
                            throw new POSFiscalPrinterException(this.DeviceName, -10001, "Error Getting Z report Number: " + fields[1].Trim());
                        }

                        string fileDirectory = abcPath + "\\Z-" + zReportNumberString;
                        pathToEJFiles = fileDirectory;
                        string filename = fileDirectory + "\\" + fields[2];

                        if (!Directory.Exists(fileDirectory))
                            Directory.CreateDirectory(fileDirectory);
                        writer = new StreamWriter(filename, false, Encoding.Default);
                        //files.Add(filename);
                        break;
                    case 1: //file line
                        string str = fields[1];
                        //if (writer == null)
                        //{
                        //    string filename2 = abcPath + "\\Z-sketo.txt";
                        //    writer = new StreamWriter(filename2, true, Encoding.Default);
                        //}
                        writer.Write(str);
                        break;
                    case 2: //file end
                        if (writer != null)
                        {
                            writer.Close();
                            //writer.Dispose();
                            writer = null;
                        }
                        break;
                    case 3: //all files are sent
                        if (writer != null)
                        {
                            writer.Close();
                            //writer.Dispose();
                            writer = null;
                        }
                        finishedReadingFiles = true;
                        break;
                }
            } while (finishedReadingFiles == false || res != MicrelecStatusCode.SUCCESS);

            if (writer != null)
            {
                writer.Close();
                //writer.Dispose();
                writer = null;
            }


            //No exception up to this point: files were writen successfully

            res = SendCommand("x/2/0", out fields);


            return DeviceResult.SUCCESS;
        }

        public override DeviceResult CutPaper()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("p/2/", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult OpenLegalReceipt()
        {
            //Opens automatically. No need to implement

            return DeviceResult.SUCCESS;

            //String[] fields;
            //MicrelecStatusCode result = SendCommand("O/0/", out fields);
            //return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult CloseLegalReceipt(bool openDrawer)
        {
            //drawer opens automatically, no need to handle

            String[] fields;
            MicrelecStatusCode result = SendCommand("O/1/", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult CancelLegalReceipt()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("O/2/", out fields, throwException: false);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult IssueXReport()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("x/1/", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SellItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal,
            eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal)
        {
            ItemDescription = PrepareString(ItemDescription, _CommandChars);
            additionalInfo = PrepareString(additionalInfo, _CommandChars);
            additionalInfo2 = PrepareString(additionalInfo2, 16);

            //Example "3/S/PLU CODE/ITEM-1/ADDITIONAL INFO/BARCODE/1.000/100.00/1/4/KATEGIRI CODE/56";
            String command = String.Format(NumberFormat, "3/S//{0}/{1}/{2}/{3:#####.000}/{4:########.00}/{5}/{6:0.00}//", ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, (int)vatCode, vatFactor * 100);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public DeviceResult SendItemToDevice(int index, string Code, string ItemDescription, double itemPrice1, double itemMaxPrice, double itemPrice2, int Points, double stockQTY, int vatCode)
        {
            try
            {
                ItemDescription = PrepareString(ItemDescription, 19);
                Code = PrepareString(Code, 15);

                //Example P/100/123456789012345/PLU_DESCR./1/2.00/100.00/3.00/10/999999 / 1001100 /
                String command = String.Format(NumberFormat, "P/{0}/{1}/{2}/{3}/{4:#####.00}/{5:#####.00}/{6:#####.00}/{7}/{8:#####.00}/1001100/", index, Code, ItemDescription, vatCode, itemPrice1, itemMaxPrice, itemPrice2, Points, stockQTY);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult GetTotalSales(out string HexResult)
        {
            try
            {
                //Example "0/"
                String command = String.Format(NumberFormat, "0/");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult GetTotalDailySalesOfItem(out string HexResult)
        {
            try
            {
                //Example "k/"
                String command = String.Format(NumberFormat, "k/");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult GetDeviceInfo(out string HexResult)
        {
            try
            {
                //Example "a/"
                String command = String.Format(NumberFormat, "a/");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult ProgramDevice(int PosNumber, string deviceName, out string HexResult)
        {
            try
            {
                /*Example 
                    S/111111011102220001000101001/10/1000/100/10000/10000/2/MAT
                    R&D/29/29/29/29/29/29/29/29/29/27/27/27/27/27/27/27/27/27//1//
                    /7////////40/100/10/
                */
                deviceName = PrepareString(deviceName, 8);
                String command = String.Format(NumberFormat, "S/111111011102220001000101001/10/1000/100/10000/10000/{0}/{1}/29/29/29/29/29/29/29/29/29/27/27/27/27/27/27/27/27/27//1///7////////40/100/10/", PosNumber, deviceName);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult ProgramPaymentType(int paymentCode, string paymentDescription, bool active, out string HexResult)
        {
            try
            {
                /*Example Y/5/card_5/CARD///100010010/1//*/
                string paymentDescr = PrepareString(paymentDescription, 8);
                string paymentShortcutDescr = PrepareString(paymentDescription, 4);
                String command = String.Format(NumberFormat, "Y/{0}/{1}/{2}///{3}00010010/1//", paymentCode, paymentDescr, paymentShortcutDescr, active ? 1 : 0);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult ReadPaymentType(int paymentCode, out string HexResult)
        {
            try
            {
                /*Example "y/2/"*/
                String command = String.Format(NumberFormat, "y/{0}/", paymentCode);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult ReadDailyPaymentMethods(int paymentCode, out string HexResult)
        {
            try
            {
                //Example "y/1/" */
                String command = String.Format(NumberFormat, "y/{0}/", paymentCode);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult TestSend(string message, out string HexResult)
        {
            try
            {
                String command = String.Format(NumberFormat, message);
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult IssueXreport(out string HexResult)
        {
            try
            {
                //Example: "x/1////

                String command = String.Format(NumberFormat, "x/1////");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DeviceResult IssueZReport(out string HexResult)
        {
            try
            {
                int zReportNumber = -1;
                String command = String.Format(NumberFormat, "x/7////");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                //return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;

                if (result == MicrelecStatusCode.NOZERODM)
                {
                    return DeviceResult.SUCCESS;
                }
                int retries = 0;
                do
                {
                    retries++;
                    Thread.Sleep(2000);
                    result = SendCommand("?/", out fields, true, false);
                } while ((int)result != 79 && retries <= 20);
                if ((int)result == 79)
                {
                    int internalRetries = 0;
                    do
                    {
                        Thread.Sleep(1000);
                        internalRetries++;
                        result = this.SendCommand("x/1////", out fields, throwException: false);
                    } while (result == MicrelecStatusCode.FISCAL_NOT_READY && internalRetries <= 20);

                    if (result == MicrelecStatusCode.SUCCESS)
                    {
                        internalRetries = 0;
                        do
                        {
                            Thread.Sleep(1000);
                            internalRetries++;
                            result = this.SendCommand("i/", out fields);
                        } while (result == MicrelecStatusCode.FISCAL_NOT_READY && internalRetries <= 20);
                        if (result == MicrelecStatusCode.SUCCESS)
                        {
                            int.TryParse(fields[2], out zReportNumber);
                            return DeviceResult.SUCCESS;
                        }
                    }
                }
                return DeviceResult.FAILURE;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DeviceResult IssueZReportCashierRegister(string pathToAbc, out int zReportNumber, out string pathToEJFiles, out string HexResult)
        {
            pathToEJFiles = null;
            zReportNumber = -1;
            string[] fields;
            MicrelecStatusCode res = this.SendCommand("x/7////", out fields, throwException: false);
            if (res == MicrelecStatusCode.NOZERODM)
            {
                HexResult = String.Join("/", fields);
                return DeviceResult.SUCCESS;
            }
            int retries = 0;
            do
            {
                retries++;
                Thread.Sleep(2000);
                res = SendCommand("?/", out fields, true, false);
            } while (res != MicrelecStatusCode.SUCCESS && retries <= 20);
            if (res == MicrelecStatusCode.SUCCESS)
            {
                int internalRetries = 0;
                do
                {
                    Thread.Sleep(1000);
                    internalRetries++;
                    res = this.SendCommand("x/10////", out fields, throwException: false);
                } while (res == MicrelecStatusCode.FISCAL_NOT_READY && internalRetries <= 20);

                if (res == MicrelecStatusCode.SUCCESS)
                {
                    internalRetries = 0;
                    do
                    {
                        Thread.Sleep(1000);
                        internalRetries++;
                        res = this.SendCommand("i/", out fields);
                    } while (res == MicrelecStatusCode.FISCAL_NOT_READY && internalRetries <= 20);
                    if (res == MicrelecStatusCode.SUCCESS)
                    {
                        int.TryParse(fields[2], out zReportNumber);
                        HexResult = String.Join("/", fields);
                        return DeviceResult.SUCCESS;
                    }
                }
            }
            HexResult = String.Join("/", fields);
            return DeviceResult.FAILURE;
        }

        public void ClosePort()
        {
            if (serialPort != null)
            {
                serialPort.Close();
            }
        }
        public DeviceResult GetDeviceParameters(out string HexResult)
        {
            try
            {
                //Example "s/"
                String command = String.Format(NumberFormat, "s/");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult GetEthernetParameters(out string HexResult)
        {
            try
            {
                //Example ",/0//"
                String command = String.Format(NumberFormat, ",/0/");
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DeviceResult SetEthernetParameters(int portNumber, int WatchdogTime, int Delay, string IP, string RemoteIP, string Gateway, string DNS1, string DNS2, string MASK,
                                                  int EnableEthernt, int EnableDHCP, int TCPUDP, int EnableWatchdogTimer, int CloseOpenReceipt, out string HexResult)
        {
            try
            {
                /*Example  \/0/9100/20/15/192.168.0.10/0.0.0.0/192.168.0.1/192.168.0.1/2
                    55.255.255.0//300112/120000/100100000/
                */
                String command = String.Format(NumberFormat, "\\/0/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}//300112/120000/{8}{9}{10}{11}{12}0000/",
                                                portNumber, WatchdogTime, Delay, IP, RemoteIP, Gateway, DNS1, MASK,
                                                EnableEthernt, EnableDHCP, TCPUDP, EnableWatchdogTimer, CloseOpenReceipt
                                                );
                string[] fields;
                MicrelecStatusCode result = SendCommand(command, out fields);
                HexResult = String.Join("/", fields);
                return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override DeviceResult ReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            ItemDescription = PrepareString(ItemDescription, _CommandChars);
            additionalInfo = PrepareString(additionalInfo, _CommandChars);
            additionalInfo2 = PrepareString(additionalInfo2, 16);

            //Example "3/S/PLU CODE/ITEM-1/ADDITIONAL INFO/BARCODE/1.000/100.00/1/4/KATEGIRI CODE/56";
            String command = String.Format(NumberFormat, "3/R//{0}/{1}/{2}/{3:#####.000}/{4:########.00}/{5}/{6:0.00}//", ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, (int)vatCode, vatFactor * 100);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult AddLineDiscount(eMinistryVatCategoryCode vatCode, string reason, string reasonExtended, double amount)
        {
            int ivc = (int)vatCode;
            reason = PrepareString(reason, _CommandChars);
            reasonExtended = PrepareString(reasonExtended, _CommandChars);

            //Example "4/1/1/DISCOUNT IN SALES/EXTRA DESCR/12.75/0/0/0/0/0"
            String command = String.Format(NumberFormat, "4/1/{0}/{1}/{2}/{3:########.00}/0/0/0/0/0/", ivc, reason, reasonExtended, amount);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult AddSubtotalDiscount(string reason, string reasonExtended, double amountVatA, double amountVatB, double amountVatC, double amountVatD, double amountVatE, double totalAmount)
        {
            if (Math.Round((amountVatA + amountVatB + amountVatC + amountVatD + amountVatE), 2, MidpointRounding.AwayFromZero) != Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero))
            {
                return DeviceResult.INVALIDPROPERTY;
            }

            reason = PrepareString(reason, _CommandChars);
            reasonExtended = PrepareString(reasonExtended, _CommandChars);

            //Example "4/1/1/DISCOUNT IN SALES/EXTRA DESCR/12.75/0/0/0/0/0"
            String command = String.Format(NumberFormat, "4/2/1/{0}/{1}/{2:########.00}/{3:########.00}/{4:########.00}/{5:########.00}/{6:########.00}/{7:########.00}/",
                                                                reason, reasonExtended, totalAmount, amountVatA, amountVatB, amountVatC, amountVatD, amountVatE);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult AddPayment(double amount, ePaymentMethodType paymentMethod, out double remainingBalance, string description = "", string extraDescription = "")
        {
            if (paymentMethod == ePaymentMethodType.UNDEFINED)
            {
                throw new ArgumentOutOfRangeException("paymentMethod", "Undefined Payment Method");
            }

            if (paymentMethod != ePaymentMethodType.CARDS && paymentMethod != ePaymentMethodType.CASH && paymentMethod != ePaymentMethodType.CREDIT)
            {
                paymentMethod = ePaymentMethodType.CARDS;
            }

            if (amount > this.MaximumPaymentAmount)
            {
                throw new ArgumentOutOfRangeException("amount", Resources.POSClientResources.INVALID_AMOUNT + ": " + amount);
            }

            String[] fields;

            description = PrepareString(description, _CommandChars);
            extraDescription = PrepareString(extraDescription, _CommandChars);

            //Example "5/2/CREDIT CARD/DINERS-12345678/12.56/87"
            int method = (int)paymentMethod;

            String command = String.Format(NumberFormat, "5/{0}/{1}/{2}/{3:########.00}/", method, description, extraDescription, amount);

            MicrelecStatusCode result = SendCommand(command, out fields);
            if (fields.Count() > 0)
            {
                remainingBalance = double.Parse(fields[0], NumberFormat);
            }
            else
            {
                remainingBalance = 0;
            }
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }


        public virtual void ReadClock(out DateTime fdpclock)
        {
            fdpclock = new DateTime();
            String[] fields;
            MicrelecStatusCode result = SendCommand("t/", out fields);

        }

        public virtual void DisplayMessage(int line, String message)
        {
            String[] fields;
            if (line < 0 || line > 2)
                throw new POSException("Invalid line");
            MicrelecStatusCode result = SendCommand("7/" + line.ToString() + "/" + message + "/", out fields);
        }

        public virtual void ReadVersion(out String vendor, out String device, out String version)
        {
            version = device = vendor = "";
            String[] fields;
            MicrelecStatusCode result = SendCommand("v/", out fields);
        }

        public virtual void ReadDeviceID(out String deviceID)
        {
            deviceID = "";
            String[] fields;
            MicrelecStatusCode result = SendCommand("a/", out fields);
        }

        public override DeviceResult PrintIllegal(params FiscalLine[] lines)
        {
            foreach (FiscalLine line in lines)
            {
                String[] fields;
                string value = PrepareString(line.Value, _LineChars);
                value = String.IsNullOrWhiteSpace(value) ? " " : value;
                MicrelecStatusCode result = SendCommand(String.Format("P/{0}/{1}/", value, (int)line.Type), out fields);
                if (result != MicrelecStatusCode.SUCCESS)
                {
                    return DeviceResult.FAILURE;
                }
            }

            this.Feed(4);
            return this.CutPaper();
        }

        //public virtual DeviceResult PrintIllegalWithHeader(FiscalLine[] lines)
        //{
        //    FiscalLine[] head;
        //    ReadHeaderCommand(out head);
        //    List<FiscalLine> alllines = new List<FiscalLine>();
        //    alllines.AddRange(head);
        //    alllines.AddRange(lines);

        //    return PrintIllegal(alllines.ToArray());
        //}

        public override DeviceResult Feed(int lines = 1)
        {
            string[] fields;
            MicrelecStatusCode result = SendCommand(String.Format("F/{0}", lines), out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        protected virtual MicrelecStatusCode HandleResult(byte[] result, out string[] fields, bool throwException)
        {

            fields = new string[0];
            if (ValidateCheckSum(result))
            {

                _byteResult = result;

                String str = Encoding.GetEncoding(1253).GetString(result, 0, result.Length);
                Trace.WriteLine("Result:" + str);
                String[] splittedString = str.Split('/');
                fields = new string[splittedString.Length - 4];
                Array.Copy(splittedString, 3, fields, 0, splittedString.Length - 4);

                int resultCode = Convert.ToInt32(splittedString[0], 16);


                byte deviceStatus, fiscalStatus;
                byte.TryParse(splittedString[1], out deviceStatus);
                byte.TryParse(splittedString[2], out fiscalStatus);
                MicrelecFiscalPrinterStatus status = FiscalStatus as MicrelecFiscalPrinterStatus;
                status.CutterError = (deviceStatus & 0x80) != 0;
                status.PrinterTimeout = (deviceStatus & 0x40) != 0;
                status.FiscalFileFull = (deviceStatus & 0x20) != 0;
                status.PrinterOffline = (deviceStatus & 0x10) != 0;
                status.BatteryWarning = (deviceStatus & 0x8) != 0;
                status.PrinterPaperEnd = (deviceStatus & 0x4) != 0;
                status.FatalError = (deviceStatus & 0x2) != 0;
                status.DeviceBusy = (deviceStatus & 0x1) != 0;
                Trace.WriteLine("DeviceBusy:" + status.DeviceBusy);
                //status.OpenReceipt = (fiscalStatus & 0x80) != 0;  //reserved - not used
                status.ReportOpen = (fiscalStatus & 0x40) != 0;
                status.CashOutOpen = (fiscalStatus & 0x20) != 0;
                status.CashInOpen = (fiscalStatus & 0x10) != 0;
                status.TransactionInPayment = (fiscalStatus & 0x8) != 0;
                status.TransactionOpen = (fiscalStatus & 0x4) != 0;
                status.DayOpen = (fiscalStatus & 0x2) != 0;
                status.DrawerOpen = (fiscalStatus & 0x1) != 0;
                MicrelecStatusCode value = (MicrelecStatusCode)resultCode;
                if (value != MicrelecStatusCode.SUCCESS && throwException)
                {
                    if (value == MicrelecStatusCode.NOZERODM || value == MicrelecStatusCode.NOSALESZEROPRICE)
                    {
                        //value = MicrelecStatusCode.SUCCESS;
                        return value;
                    }
                    if (value == MicrelecStatusCode.DEVICE_IS_BUSY_IN_ANOTHER_TASK)
                    {
                        return value;
                    }
                    if (this.ErrorCodeDescriptionMapping.ContainsKey(resultCode))
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, resultCode, this.ErrorCodeDescriptionMapping[resultCode]);
                    }
                    else
                    {
                        throw new POSFiscalPrinterException(this.DeviceName, resultCode, "UNKNOWN ERROR OCCURED: " + resultCode);
                    }
                }
                return value;
            }
            if (throwException)
                throw new POSException("Fiscal Checksum Error");
            return MicrelecStatusCode.RECEIVED_CHECKSUM_INVALID;
        }

        protected virtual MicrelecStatusCode SendCommand(string command, out string[] fields, bool forceReset = true, bool throwException = true, bool writeLog = false, bool checkDeviceStatus = true)
        {
            String command1 = (command.EndsWith("/")) ? command : command + "/";
            Trace.WriteLine("Send:" + command1);

            MicrelecStatusCode result;
            int busyRetryCounter = 0;
            do
            {
                if (busyRetryCounter > 0)
                {
                    Thread.Sleep(3000);
                }
                result = SendCommand(Encoding.GetEncoding(1253).GetBytes(command1), out fields, forceReset, throwException, checkDeviceStatus);
                if (writeLog)
                {
                    if (log == null)
                        log = new StreamWriter("c:\\FiscalPrinter_commandlog.txt");
                    log.Write("Command: " + command + ": Result: " + result.ToString());
                    for (int i = 0; i < fields.Length; i++)
                    {
                        log.Write(String.Format(" field[{0}] = '{1}'", i, fields[i]));
                    }
                    log.WriteLine();
                }
                busyRetryCounter++;
            } while (result == MicrelecStatusCode.DEVICE_IS_BUSY_IN_ANOTHER_TASK && busyRetryCounter < 3);
            return result;
        }

        protected virtual MicrelecStatusCode SendCommand(byte[] command, out string[] fields, bool forceReset, bool throwException = true, bool checkDeviceStatus = true)
        {
            byte[] reply = null;
            fields = new string[0];
            if (command[0] != (byte)'?' && checkDeviceStatus)
            {
                //ReadDeviceStatus();
                int retries2 = 20;
                while (((MicrelecFiscalPrinterStatus)this.FiscalStatus).DeviceBusy && retries2 > 0)
                {
                    ReadDeviceStatus();
                    retries2--;
                }
                if (((MicrelecFiscalPrinterStatus)this.FiscalStatus).DeviceBusy)
                {
                    return MicrelecStatusCode.DEVICE_IS_BUSY_IN_ANOTHER_TASK;
                }
            }
            if (ConType.Equals(ConnectionType.COM))
            {
                if (serialPort == null)
                {
                    InitializeConnection();
                }
                if (serialPort.IsOpen == false)
                {
                    serialPort.Open();
                }
                if (serialPort.IsOpen == false)
                {
                    throw new POSFiscalPrinterException(this.DeviceName, -1000, "Cannot Open COM Port");
                }
                int checkSum = ComputeCheckSum(command, false);
                int c2 = checkSum % 10;
                int c1 = (checkSum - c2) / 10;
                byte[] command2send = new byte[command.Length + 2];
                Array.Copy(command, command2send, command.Length);
                command2send[command.Length] = (byte)(c1 + 48);
                command2send[command.Length + 1] = (byte)(c2 + 48);

                bool ackReceived = false;
                int retries = 0;
                if (forceReset)
                {
                    while (!ackReceived && retries < 3)
                    {
                        //Forcing Fiscal Device to reset its state to idle
                        serialPort.Write(CAN);
                        serialPort.DiscardOutBuffer();
                        serialPort.DiscardInBuffer();

                        //Quering device if is capable of handling requests
                        serialPort.Write(ENQ);
                        ackReceived = WaitFor(bACK);
                        retries++;
                    }
                }
                else
                {
                    //Quering device if is capable of handling requests
                    serialPort.Write(ENQ);
                    while (!ackReceived && retries < 3)
                    {
                        ackReceived = WaitFor(bACK);
                        retries++;
                    }
                }
                if (!ackReceived)
                {
                    if (throwException)
                    {
                        throw new POSException("Fiscal Printer cannot handle");
                    }
                    return MicrelecStatusCode.FISCAL_NOT_READY;
                }
                // Sending request / command
                int Left = 0;
                int index = 0;
                int counter = command.Length - 1;

                while (index <= counter)
                {
                    Left = (Left + command[index]) % (256);
                    checked { ++index; }
                }
                retries = 0;
                bool transmissionSuccessful = false;
                while (retries < 3 && transmissionSuccessful == false)
                {
                    //Trying at most three times
                    this.SendData(new byte[] { bSTX });
                    this.SendData(command2send);
                    this.SendData(new byte[] { bETX });
                    //this.SendData(command);
                    // this.SendData(finalExt);

                    if (this.WaitSerialPortResponse(10000) && this.serialPort.ReadByteExtended() == bACK)
                    {
                        transmissionSuccessful = true;
                    }
                    retries++;
                }
                if (transmissionSuccessful == false)
                {
                    //if transmission failed throw exception
                    if (throwException)
                        throw new POSException("Error sending request to the Fiscal Printer");
                    return MicrelecStatusCode.FISCAL_NOT_READY;
                }
                //waiting response
                retries = 0;
                while (retries < 3)
                {
                    if (WaitSerialPortResponse(7000))
                    {
                        if (serialPort.ReadByteExtended() == bSTX)
                        {
                            int internalRetries = 0;
                            List<byte> receivedBytes = new List<byte>();
                            while (internalRetries < 3)
                            {
                                if (WaitSerialPortResponse(5000))
                                {
                                    byte b = (byte)serialPort.ReadByteExtended();
                                    if (b == bETX)
                                    {
                                        if (receivedBytes.Count < 8)
                                        {
                                            serialPort.Write(NAK);
                                            break;
                                        }
                                        reply = receivedBytes.ToArray();
                                        serialPort.Write(ACK);
                                        MicrelecStatusCode result = HandleResult(reply, out fields, throwException);
                                        return result;
                                    }
                                    receivedBytes.Add(b);
                                }
                                else
                                {
                                    internalRetries++;
                                }
                            }
                        }
                    }
                    retries++;
                }
            }
            else
            {
                if (tcpClient == null)
                {
                    InitializeEthernetConnection();
                }
                if (tcpClient.Connected == false)
                {
                    tcpClient.Close();
                    tcpClient = null;
                    InitializeEthernetConnection();
                }
                if (tcpClient.Connected == false)
                {
                    throw new POSFiscalPrinterException(this.DeviceName, -1000, "Cannot cοnnect to Ethernet. Check IP and Port");
                }
                NetworkStream netStream = tcpClient.GetStream();
                try
                {
                    bool transmissionSuccessful = false;
                    int checkSum = ComputeCheckSum(command, false);
                    int c2 = checkSum % 10;
                    int c1 = (checkSum - c2) / 10;
                    byte[] command2send = new byte[command.Length + 2];
                    Array.Copy(command, command2send, command.Length);
                    command2send[command.Length] = (byte)(c1 + 48);
                    command2send[command.Length + 1] = (byte)(c2 + 48);

                    bool ackReceived = false;
                    int retries = 0;
                    if (forceReset)
                    {
                        while (!ackReceived && retries < 3)
                        {
                            netStream.Write(command2send, 0, command2send.Length);
                            if (netStream.CanRead)
                            {
                                List<byte> allBytes = new List<byte>();
                                int timeout = 300000;
                                if (timeout > 0)
                                {
                                    netStream.ReadTimeout = timeout;
                                }

                                int read = 0;
                                byte[] buffer = new byte[sizeof(char)];
                                char ch = '\x1B';
                                string received = "";
                                do
                                {
                                    read = netStream.Read(buffer, 0, buffer.Length);
                                    ch = (char)buffer[0];
                                    allBytes.AddRange(buffer.Take(read));
                                    received += System.Text.Encoding.UTF8.GetString(buffer, 0, read);
                                }
                                while (netStream.DataAvailable && ch != '\x1B');
                                transmissionSuccessful = true;
                                ackReceived = true;
                                MicrelecStatusCode result = HandleResult(allBytes.ToArray(), out fields, throwException);
                                if (result == MicrelecStatusCode.NOSALESZEROPRICE)
                                {
                                    result = MicrelecStatusCode.SUCCESS;
                                }
                                return result;
                            }
                            ackReceived = true;
                        }
                        return MicrelecStatusCode.SUCCESS;
                    }
                }
                catch (SocketException ex)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw;
                }
                finally
                {
                    if (netStream != null)
                    {
                        netStream.Flush();
                        netStream.Close();
                        netStream.Dispose();
                        netStream = null;
                    }
                    if (tcpClient != null && tcpClient.Connected)
                    {
                        tcpClient.Close();
                    }
                }
            }
            return MicrelecStatusCode.SUCCESS;
        }

        bool WaitFor(byte b)
        {
            if (!serialPort.IsOpen)
            {
                return false;
            }
            if (!WaitSerialPortResponse(250))
                return false;
            //int ch = serialPort.ReadByteExtended();
            //return (int)b == ch;
            int bytes = serialPort.BytesToRead;
            byte[] comBuffer = new byte[bytes];
            serialPort.Read(comBuffer, 0, bytes);
            string str;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            str = enc.GetString(comBuffer);
            return true;
        }
        protected bool SendData(byte[] s)
        {
            bool flag1 = true;
            if (!this.serialPort.IsOpen)
                return false;
            if (s.Length == 0)
                return true;
            bool flag2;
            try
            {
                int length = s.Length;
                int counter = 1;
                while (counter <= length)
                {
                    if (this.serialPort.Handshake == Handshake.RequestToSend)
                    {
                        do
                        {
                            MessFL1 = checked(MessFL1 + 1);
                            if (MessFL1 >= 5000)
                                MessFL1 = 2005;
                            if (MessFL1 == 2000)
                                flag1 = false;
                        }
                        while (!serialPort.CtsHolding);
                        if (MessFL1 > 0)
                            MessFL1 = 0;
                    }
                    if (flag1)
                        serialPort.Write(s, checked(counter - 1), 1);
                    checked { ++counter; }
                }
                flag2 = flag1;
            }
            catch (Exception)
            {
                flag2 = false;
            }
            return flag2;
        }

        protected virtual bool ValidateCheckSum(byte[] package)
        {
            int computedCheckSum;
            int receivedCheckSum = (package[package.Length - 2] - 48) * 10 + package[package.Length - 1] - 48;
            computedCheckSum = ComputeCheckSum(package, true);

            if (computedCheckSum % 100 == receivedCheckSum)
                return true;
            return false;
        }

        private static int ComputeCheckSum(byte[] package, bool isReceivedPackage)
        {
            int sum = 0, offset = isReceivedPackage ? 2 : 0;
            for (int i = 0; i < package.Length - offset; i++)
            {
                sum += package[i];
                sum = sum % 256;
            }
            return sum % 100;
        }

        public override DeviceResult ReceiptPaymentMode(double grossTotal)
        {
            //No need to implement. Occurs automatically

            return DeviceResult.SUCCESS;
        }

        public override DeviceResult GetCurrentDayReceiptsCount(out int receiptsCount)
        {

            receiptsCount = -1;
            String command = "0";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            if (fields.Length > 6)
            {
                int.TryParse(fields[6], out receiptsCount);
            }

            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override bool HasSlipStation
        {
            get { return false; }
        }


        public override DeviceResult SlipStationPrint(params FiscalLine[] lines)
        {
            return DeviceResult.STATIONNOTSUPPORTED;
        }


        public override DeviceResult SlipStationHasPaper(out bool hasPaper)
        {
            hasPaper = false;
            return DeviceResult.STATIONNOTSUPPORTED;
        }

        //public DeviceResult CancelReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, eMinistryVatCategoryCode vatCode, double vatFactor)
        //{
        //    throw new NotSupportedException();
        //}

        public DeviceResult CancelItemSell(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            ItemDescription = PrepareString(ItemDescription, _CommandChars);
            additionalInfo = PrepareString(additionalInfo, _CommandChars);
            additionalInfo2 = PrepareString(additionalInfo2, 16);

            //Example "3/S/PLU CODE/ITEM-1/ADDITIONAL INFO/BARCODE/1.000/100.00/1/4/KATEGIRI CODE/56";
            String command = String.Format(NumberFormat, "3/V//{0}/{1}/{2}/{3:#####.000}/{4:########.00}/{5}/{6:0.00}//", ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, (int)vatCode, vatFactor * 100);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public DeviceResult CancelPreviousTransaction()
        {
            String command = "V";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SellItemAndCancelIt(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity,
            double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal)
        {
            DeviceResult result = this.SellItem(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, lineTotal, vatCode, vatFactor, supportsDecimal);
            result = this.CancelItemSell(ItemDescription, additionalInfo, additionalInfo2, itemQuantity, finalUnitPrice, vatCode, vatFactor);

            return result;
        }

        public override DeviceResult ReturnItemAndCancelIt(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            return DeviceResult.SUCCESS;
        }

        public override DeviceResult ReprintZReportsDateToDate(DateTime fromDate, DateTime toDate, eReprintZReportsMode mode)
        {
            //example "f/010113/310813"
            string fromDateStr = fromDate.ToString("ddMMyy");
            string toDateStr = toDate.ToString("ddMMyy");

            String command = "f/" + fromDateStr + "/" + toDateStr;
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReprintZReports(int fromZ, int toZ, eReprintZReportsMode mode)
        {
            //Example "z/150/320";
            String command = "z/" + fromZ + "/" + toZ;
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }
        //TODO remove 
        protected override int LineCharsForIllegalPrinting
        {
            get;
        }

        public override DeviceResult ReprintReceiptsOfCurrentZ(int fromReceiptNumber, int toReceiptNumber)
        {
            throw new NotSupportedException();
        }

        public override DeviceResult CashIn(double amount, string message)
        {
            String command = String.Format(NumberFormat, "6/0/{0:########.00}/{1}", amount, PrepareString(message));
            string[] fields;
            MicrelecStatusCode result;
            try
            {
                result = SendCommand(command, out fields);
            }
            catch (Exception ex)
            {
                result = SendCommand("O/2/", out fields);
                throw ex;
            }
            result = SendCommand("O/1/", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult CashOut(double amount, string message)
        {
            String command = String.Format(NumberFormat, "6/1/{0:########.00}/{1}", amount, PrepareString(message));
            string[] fields;
            MicrelecStatusCode result;
            try
            {
                result = SendCommand(command, out fields);
            }
            catch (Exception ex)
            {
                result = SendCommand("O/2/", out fields);
                throw ex;
            }

            result = SendCommand("O/1/", out fields);

            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult PrintFiscalMemoryBlocks(int fromBlock, int toBlock)
        {
            throw new NotSupportedException();
        }

        public override DeviceResult ReceiptSubtotal(string info)
        {
            String command = "o";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SetHeader(params FiscalLine[] lines)
        {
            if (lines.Length > 8)
                return DeviceResult.FAILURE;
            String command = "H/";
            foreach (FiscalLine val in lines)
            {
                if (String.IsNullOrWhiteSpace(val.Value))
                {
                    command += "//";
                }
                else
                {
                    command += (int)val.Type + "/" + val.Value + "/";
                }
            }
            String[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReadHeader(out FiscalLine[] lines)
        {
            lines = new FiscalLine[8];
            String[] fields;
            MicrelecStatusCode result = SendCommand("h/", out fields);
            for (int i = 0; i < 7; i++)
            {
                lines[i] = new FiscalLine();
                if (String.IsNullOrWhiteSpace(fields[2 * i]) == false)
                {
                    lines[i].Value = fields[2 * i + 1];
                    lines[i].Type = (ePrintType)(int.Parse(fields[2 * i]));
                }
            }

            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SetVatRates(double vatRateA, double vatRateB, double vatRateC, double vatRateD, double vatRateE)
        {
            vatRateA = vatRateA <= 1 ? vatRateA * 100 : vatRateA;
            vatRateB = vatRateB <= 1 ? vatRateB * 100 : vatRateB;
            vatRateC = vatRateC <= 1 ? vatRateC * 100 : vatRateC;
            vatRateD = vatRateD <= 1 ? vatRateD * 100 : vatRateD;

            //E is always zero, command does not ask for it
            String command = String.Format(NumberFormat, "b/{0:########.00}/{1:########.00}/{2:########.00}/{3:########.00}", vatRateA, vatRateB, vatRateC, vatRateD);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReadVatRates(out double vatRateA, out double vatRateB, out double vatRateC, out double vatRateD, out double vatRateE)
        {
            String command = "e";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            if (result == MicrelecStatusCode.SUCCESS)
            {
                if (fields.Length < 4)
                {
                    throw new POSUserVisibleException("Fiscal Success but returned less fields than expected");
                }
                double.TryParse(fields[0], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatRateA);
                double.TryParse(fields[1], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatRateB);
                double.TryParse(fields[2], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatRateC);
                double.TryParse(fields[3], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatRateD);
                double.TryParse(fields[4], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out vatRateE);
                vatRateA /= 100;
                vatRateB /= 100;
                vatRateC /= 100;
                vatRateD /= 100;
                vatRateE /= 100;
            }
            else
            {
                throw new POSUserVisibleException("Fiscal Error:" + this.ErrorCodeDescriptionMapping[(int)result]);
            }
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult SetCashierID(string id)
        {
            //Do nothing, not supported
            return DeviceResult.SUCCESS;
        }

        public override DeviceResult GetDayAmounts(out double vatAmountA, out double vatAmountB, out double vatAmountC, out double vatAmountD, out double vatAmountE,
                                                   out double netAmountA, out double netAmountB, out double netAmountC, out double netAmountD, out double netAmountE)
        {
            throw new NotSupportedException();
            //String command = "0";
            //string[] fields;
            //MicrelecStatusCode result = SendCommand(command, out fields);
            //if (result == MicrelecStatusCode.SUCCESS)
            //{
            //    vatAmountA = 0;
            //    vatAmountB = 0;
            //    vatAmountC = 0;
            //    vatAmountD = 0;
            //    vatAmountE = 0;

            //    netAmountA = 0;
            //    netAmountB = 0;
            //    netAmountC = 0;
            //    netAmountD = 0;
            //    netAmountE = 0;
            //    if (fields.Length < 4)
            //    {
            //        throw new POSUserVisibleException("Fiscal Success but returned less fields than expected");
            //    }
            //    double.TryParse(fields[0], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountA);
            //    double.TryParse(fields[1], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountB);
            //    double.TryParse(fields[2], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountC);
            //    double.TryParse(fields[3], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountD);
            //    double.TryParse(fields[4], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountE);

            //    double vatRateA; double vatRateB; double vatRateC; double vatRateD; double vatRateE;

            //    DeviceResult result2 = ReadVatRates(out vatRateA, out vatRateB, out vatRateC, out vatRateD, out vatRateE);

            //    if (result2 == DeviceResult.SUCCESS)
            //    {
            //        if (netAmountA > 0)
            //        {
            //            vatAmountA = Math.Round(netAmountA - Math.Round((netAmountA / (1 + vatRateA)), 2), 2);
            //            //netAmountA = Math.Round(netAmountA - vatAmountA, 2);
            //        }
            //        if (netAmountB > 0)
            //        {
            //            vatAmountB = Math.Round(netAmountB - Math.Round((netAmountB / (1 + vatRateB)), 2), 2);
            //            //netAmountB = Math.Round(netAmountB - vatAmountB, 2);
            //        }
            //        if (netAmountC > 0)
            //        {
            //            vatAmountC = Math.Round(netAmountC - Math.Round((netAmountC / (1 + vatRateC)), 2), 2);
            //            //netAmountC = Math.Round(netAmountC - vatAmountC, 2);
            //        }
            //        if (netAmountD > 0)
            //        {
            //            vatAmountD = Math.Round(netAmountD - Math.Round((netAmountD / (1 + vatRateD)), 2), 2);
            //            //netAmountD = Math.Round(netAmountD - vatAmountD, 2);
            //        }
            //        if (netAmountE > 0)
            //        {
            //            vatAmountE = Math.Round(netAmountE - Math.Round((netAmountE / (1 + vatRateE)), 2), 2);
            //            //netAmountE = Math.Round(netAmountE - vatAmountE, 2);
            //        }

            //    }
            //}
            //else
            //{
            //    throw new POSUserVisibleException("Fiscal Error:" + this.ErrorCodeDescriptionMapping[(int)result]);
            //}
            //return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override bool CheckIfCanPrintIllegal(out string reason)
        {
            ////No special restrictions
            reason = null;
            return true;
        }

        public override DeviceResult OpenDrawer(string openDrawerCustomString)
        {
            string[] fields;
            MicrelecStatusCode result = SendCommand("p/1/", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public MicrelecStatusCode ReadParameters()
        {
            string[] fields;
            return SendCommand("s/", out fields, throwException: false);
        }
        public static byte[] ConvertToByteArray(String binaryString)
        {
            try
            {
                byte[] byteArray;
                return byteArray = Enumerable.Range(0, int.MaxValue / 8)
                          .Select(i => i * 8)
                          .TakeWhile(i => i < binaryString.Length)
                          .Select(i => binaryString.Substring(i, 8))
                          .Select(s => Convert.ToByte(s, 2))
                          .ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string GetResponse(NetworkStream stream)
        {
            byte[] data = new byte[1024];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                do
                {
                    stream.Read(data, 0, data.Length);
                    memoryStream.Write(data, 0, data.Length);
                } while (stream.DataAvailable);

                return Encoding.ASCII.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            }
        }
        public static byte[] StrToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }
        public eMachineStatus ReadStatusOfDevice()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("?/", out fields, true, false);
            byte[] bytesResult = fields.Select(Byte.Parse).ToArray();
            eMachineStatus status = GetDeviceStatus(_byteResult);
            return status;
        }

        private eMachineStatus GetDeviceStatus(byte[] result)
        {
            string[] fields = new string[0];
            if (ValidateCheckSum(result))
            {
                String str = Encoding.GetEncoding(1253).GetString(result, 0, result.Length);
                Trace.WriteLine("Result:" + str);
                String[] splittedString = str.Split('/');
                fields = new string[splittedString.Length - 4];
                Array.Copy(splittedString, 3, fields, 0, splittedString.Length - 4);

                int resultCode = Convert.ToInt32(splittedString[0], 16);

                byte deviceStatus, fiscalStatus;
                byte.TryParse(splittedString[1], out deviceStatus);
                byte.TryParse(splittedString[2], out fiscalStatus);
                MicrelecFiscalPrinterStatus status = FiscalStatus as MicrelecFiscalPrinterStatus;
                status.CutterError = (deviceStatus & 0x80) != 0;
                status.PrinterTimeout = (deviceStatus & 0x40) != 0;
                status.FiscalFileFull = (deviceStatus & 0x20) != 0;
                status.PrinterOffline = (deviceStatus & 0x10) != 0;
                status.BatteryWarning = (deviceStatus & 0x8) != 0;
                status.PrinterPaperEnd = (deviceStatus & 0x4) != 0;
                status.FatalError = (deviceStatus & 0x2) != 0;
                status.DeviceBusy = (deviceStatus & 0x1) != 0;
                Trace.WriteLine("DeviceBusy:" + status.DeviceBusy);
                //status.OpenReceipt = (fiscalStatus & 0x80) != 0;  //reserved - not used
                status.ReportOpen = (fiscalStatus & 0x40) != 0;
                status.CashOutOpen = (fiscalStatus & 0x20) != 0;
                status.CashInOpen = (fiscalStatus & 0x10) != 0;
                status.TransactionInPayment = (fiscalStatus & 0x8) != 0;
                status.TransactionOpen = (fiscalStatus & 0x4) != 0;
                status.DayOpen = (fiscalStatus & 0x2) != 0;
                status.DrawerOpen = (fiscalStatus & 0x1) != 0;


                if (status.DrawerOpen && !status.DayOpen && !status.TransactionOpen && !status.CashInOpen)
                {
                    return eMachineStatus.DAYSTARTED;
                }
                if (status.DrawerOpen && status.DayOpen && status.TransactionOpen)
                {
                    return eMachineStatus.OPENDOCUMENT;
                }
                else if (status.DrawerOpen && status.DayOpen)
                {
                    return eMachineStatus.SALE;
                }
                else if (status.DrawerOpen && status.CashInOpen)
                {
                    return eMachineStatus.OPENDOCUMENT_PAYMENT;
                }
                else if (!status.DayOpen)
                {
                    return eMachineStatus.CLOSED;
                }
                else
                {
                    return eMachineStatus.UNKNOWN;
                }
            }
            else
            {
                return eMachineStatus.UNKNOWN;
            }
        }
    }
}
