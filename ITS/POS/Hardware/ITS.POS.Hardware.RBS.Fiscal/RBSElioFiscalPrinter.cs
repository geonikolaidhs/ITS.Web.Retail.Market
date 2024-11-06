using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ITS.POS.Hardware.RBS.Fiscal
{
    public class RBSElioFiscalPrinter : MicrelecFiscalPrinter
    {
        public static event EventHandler ShowMessageHandlePrinterActions;
        public static event EventHandler ShowMessageIssueingZReport;
        private Dictionary<int, string> RBSElioWebErrorCodes = new Dictionary<int, string>(){
                                                                   {1,"The protocol command expects more fields"},
                                                                    {2,"A protocol command field is longer than expected"},
                                                                    {3,"A protocol command filed is smaller than expected"},
                                                                    {4,"Check the protocol command fields"},
                                                                    {5,"Check the protocol command fields"},
                                                                    {6,"The protocol command is not supported"},
                                                                    {7,"The PLU code doesn’t exist"},
                                                                    {8,"The DPT Code doesn’t exist"},
                                                                    {9,"Wrong VAT code"},
                                                                    {10,"The Clerk’s index number doesn’t exist"},
                                                                    {11,"Wrong Clerk’s password"},
                                                                    {12,"The payment code doesn’t exist"},
                                                                    {13,"The requested Fiscal record doesn’t exist"},
                                                                    {14,"The requested Fiscal record type doesn’t exist"},
                                                                    {15,"Printing type Error"},
                                                                    {16,"The day is open, issue a Z- Report first"},
                                                                    {17,"Disconnect Jumpers first"},
                                                                    {18,"Wrong TIME, not allowed operation"},
                                                                    {19,"CAN NOT PERFORM SALES"},
                                                                    {20,"A transaction is open, close the transaction first"},
                                                                    {21,"Receipt in Payment"},
                                                                    {22,"CASH IN/OUT transaction in progress"},
                                                                    {23,"Wrong VAT rate"},
                                                                    {24,"Price Error"},
                                                                    {25,"The online communication of the ECR is ON"},
                                                                    {26,"The ECR is busy, try again later"},
                                                                    {27,"Invalid sales operation"},
                                                                    {28,"Invalid Discount/Markup type"},
                                                                    {29,"No more headers can be programmed"},
                                                                    {30,"A user’s report is open"},
                                                                    {31,"A user’s report is open"},
                                                                    {32,"The Fiscal Memory has no transactions"},
                                                                    {33,"Discount/Markup index number Error"},
                                                                    {34,"You can’t program any more PLUs"},
                                                                    {35,"Error in BMP Data"},
                                                                    {36,"The BMP index number doesn’t exist"},
                                                                    {37,"The category index number doesn’t exist"},
                                                                    {38,"Printer ID index Error"},
                                                                    {39,"Error printing type"},
                                                                    {40,"Unit ID index Error"},
                                                                    {41,"No more sales can be performed"},
                                                                    {42,"Keyboard Error-or keyboard disconnected"},
                                                                    {43,"Battery Error."},
                                                                    {44,"Paper end."},
                                                                    {45,"Error with the cutter."},
                                                                    {46,"The printer is disconnected."},
                                                                    {47,"Printer overheated."},
                                                                    {48,"The fiscal memory is offline."},
                                                                    {49,"Fatal Error."},
                                                                    {50,"Jumpers are still ON."},
                                                                    {51,"The printer’s cover is open."},
                                                                    {52,"No more VATs can be programmed"},
                                                                    {53,"No more lines."},
                                                                    {54,"Into Menu."},
                                                                    {55,"No Ticket Discount."},
                                                                    {56,"Not supported."},
                                                                    {57,"Access not allowed for current clerk."},
                                                                    {58,"Wrong Baud Rate."},
                                                                    {60,"Inactive PLU."},
                                                                    {61,"Fiscal Memory is full."},
                                                                    {62,"Wrong Coupon Index"},
                                                                    {63,"Larger Quantity Value than the one allowed."},
                                                                    {64,"Inactive payment type."},
                                                                    {65,"Inactive DPT."},
                                                                    {66,"No more Stock"},
                                                                    {67,"Serial port error"},
                                                                    {68,"Cannot open Drawer."},
                                                                    {69,"Open ticket"},
                                                                    {70,"There is not an open receipt."},
                                                                    {71,"Error in SD reports."},
                                                                    {72,"Inactive ticket"},
                                                                    {73,"Wrong coupon"},
                                                                    {74,"Discount/Markup limit is exceeded."},
                                                                    {75,"Zero Discount/Markup."},
                                                                    {76,"Description is blank."},
                                                                    {77,"Wrong barcode"},
                                                                    {78,"Wrong Key code."},
                                                                    {79,"EJ Transfer"},
                                                                    {81,"The SD is full."},
                                                                    {82,"The SD is old."},
                                                                    {83,"Negative Receipt total."},
                                                                    {84,"Clientindex Error"},
                                                                    {85,"Wrong Client code."},
                                                                    {86,"Out of limits bonus."},
                                                                    {87,"Wrong Promotion link index."},
                                                                    {88,"WORD NOT ALLOWED"},
                                                                    {89,"Error in barcode data."},
                                                                    {90,"Change is not allowed for this Payment type."},
                                                                    {91,"Must insert the payment amount."},
                                                                    {92,"Same Header lines."},
                                                                    {93,"In Error Message."},
                                                                    {94,"Not found SD data."},
                                                                    {95,"There is no more data to read from SD."},
                                                                    {96,"Set the limit to read SD data."},
                                                                    {97,"Receipt sales amount exceeded."},
                                                                    {98,"Daily sales amount exceeded."},
                                                                    {99,"The SD is full."},
                                                                    {100,"Wrong ticket index"},
                                                                    {101,"Fiscal Communication problem."},
                                                                    {102,"Cannot transfer payment amount"},
                                                                    {104,"LCD disconnection"},
                                                                    {105,"SD disconnection"},
                                                                    {106,"Battery Error"},
                                                                    {107,"NAND Memory full"},
                                                                    {108,"Wrong TIN"},
                                                                    {109,"Empty EJ"},
                                                                    {110,"Invalid IP"},
                                                                    {111,"Invalid refund"},
                                                                    {112,"Invalid void"},
                                                                    {113,"Exceed amount limit"},
                                                                    {114,"Null header"},
                                                                    {115,"Inactive clerk"},
                                                                    {116,"Barcode error"},
                                                                    {117,"Need program TIN"},
                                                                    {118,"SD memory need format"},
                                                                    {119,"Wrong Date"},
                                                                    {120,"Wrong Time"},
                                                                    {121,"Call technician"},
                                                                    {122,"Cannot open EJ file"},
                                                                    {123,"Cannot write EJ file"},
                                                                    {124,"Cannot read EJ file"},
                                                                    {125,"Wrong GSIS AES Key"},
                                                                    {126,"Cannot change"},
                                                                    {127,"Ethernet communication error"},
                                                                    {128,"Send GSIS error"},
                                                                    {129,"Fiscal Blank"},
                                                                    {130,"Web Services file error"},
                                                                    {132,"Active GPRS"},
                                                                    {133,"Wrong Activate Services Key"},
                                                                    {134,"Cannot open file in SD"},
                                                                    {135,"Cannot write file in SD"},
                                                                    {136,"Need quantity"},
                                                                    {137,"Receipt closed"},
                                                                    {138,"Unauthorized function"},
                                                                    {139,"Already used coupon"},
                                                                    {140,"Invalid key for services’ activation or already activated"}
                                                               };

        protected override Dictionary<int, string> ErrorCodeDescriptionMapping
        {
            get
            {
                return RBSElioWebErrorCodes;
            }
        }
        public RBSElioFiscalPrinter(ConnectionType conType, String deviceName, int posID, int lineChars, int CommandChars)
            : base(conType, deviceName, posID, lineChars, CommandChars)
        {

        }

        public override DeviceResult PrintIllegal(params FiscalLine[] lines)
        {
            foreach (FiscalLine line in lines)
            {
                String[] fields;
                string value = PrepareString(line.Value);
                value = String.IsNullOrWhiteSpace(value) ? " " : value;
                MicrelecStatusCode result = SendCommand(String.Format("7/1/{0}/{1}/", (int)line.Type, value), out fields);
                if (result != MicrelecStatusCode.SUCCESS)
                {
                    return DeviceResult.FAILURE;
                }
            }

            this.Feed(4);
            return this.CutPaper();
        }



        public override DeviceResult IssueZReport(String pathToAbc, out int zReportNumber, out string pathToEJFiles)
        {

            pathToEJFiles = null;
            zReportNumber = -1;
            string[] fields;

            MicrelecStatusCode TransferEjResult;
            MicrelecStatusCode FiscalDataResult;
            MicrelecStatusCode Zresult;
            string zCommand = "x/7////";
            string ejCommand = "x/10////";
            string fiscalDataCommand = "i/";

            int retries = 0;
            do
            {
                bool reset = retries % 2 == 0;
                try
                {
                    Zresult = this.SendCommand(zCommand, out fields, forceReset: reset, throwException: false);
                    LogInfo("Zresult : " + Zresult.ToString() + " ,Try No : " + retries);
                }
                catch (Exception ex)
                {
                    Zresult = MicrelecStatusCode.FISCAL_NOT_READY;
                    LogError(ex.Message + ", " + zCommand + " ,Try No : " + retries);
                }
                if (Zresult == MicrelecStatusCode.NOSALESZEROPRICE || Zresult == MicrelecStatusCode.NOZERODM)
                {
                    Thread.Sleep(5000);
                    break;
                }
                retries++;
            } while (retries < 5);

            retries = 0;
            do
            {
                Thread.Sleep(3000);
                bool reset = retries % 2 == 0;
                try
                {
                    TransferEjResult = this.SendCommand(ejCommand, out fields, forceReset: reset, throwException: false, checkDeviceStatus: false);
                    LogInfo("TransferEjResult : " + TransferEjResult.ToString() + " ,Try No : " + retries);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message + ", " + ejCommand + " ,Try No : " + retries);
                    TransferEjResult = MicrelecStatusCode.FISCAL_NOT_READY;
                }
                try
                {
                    FiscalDataResult = this.SendCommand(fiscalDataCommand, out fields, forceReset: reset, throwException: false, checkDeviceStatus: false);
                    LogInfo("FiscalDataResult : " + FiscalDataResult.ToString() + " ,Try No : " + retries);
                }
                catch (Exception ex)
                {
                    fields = new List<string>().ToArray();
                    LogError(ex.Message + " , " + fiscalDataCommand + " ,Try No : " + retries);
                    FiscalDataResult = MicrelecStatusCode.FISCAL_NOT_READY;
                }
                Thread.Sleep(2000);
                retries++;
            } while (retries < 50 && fields.Length < 2);

            if (fields.Length > 2)
            {
                int.TryParse(fields[2], out zReportNumber);
                LogInfo("zReportNumber : " + zReportNumber.ToString() + " ,Try No : " + retries);
                return DeviceResult.SUCCESS;
            }
            else
            {
                ShowMessageHandlePrinterActions?.Invoke(this, EventArgs.Empty);
                Thread.Sleep(5000);
                try
                {
                    Zresult = this.SendCommand(zCommand, out fields, true, throwException: false, checkDeviceStatus: false);
                    LogInfo("Zresult : " + Zresult.ToString() + " ,Try No : " + retries);
                }
                catch (Exception ex)
                {
                    Zresult = MicrelecStatusCode.FISCAL_NOT_READY;
                    LogError(ex.Message + ", " + zCommand + " ,Try No : " + retries);
                }
                retries = 0;
                do
                {
                    Thread.Sleep(2000);
                    bool reset = retries % 2 == 0;
                    try
                    {
                        TransferEjResult = this.SendCommand(ejCommand, out fields, forceReset: reset, throwException: false, checkDeviceStatus: false);
                        LogInfo("TransferEjResult : " + TransferEjResult.ToString() + " ,Try No : " + retries);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message + ", " + ejCommand + " ,Try No : " + retries);
                        TransferEjResult = MicrelecStatusCode.FISCAL_NOT_READY;
                    }
                    try
                    {
                        FiscalDataResult = this.SendCommand(fiscalDataCommand, out fields, forceReset: reset, throwException: false, checkDeviceStatus: false);
                        LogInfo("FiscalDataResult : " + FiscalDataResult.ToString() + " ,Try No : " + retries);
                    }
                    catch (Exception ex)
                    {
                        fields = new List<string>().ToArray();
                        LogError(ex.Message + ", " + fiscalDataCommand + " ,Try No : " + retries);
                        FiscalDataResult = MicrelecStatusCode.FISCAL_NOT_READY;
                    }
                    Thread.Sleep(1000);
                    retries++;
                } while (retries < 50 && fields.Length < 2);

                if (fields.Length > 2)
                {
                    int.TryParse(fields[2], out zReportNumber);
                    return DeviceResult.SUCCESS;
                }
                else
                {
                    return DeviceResult.FAILURE;
                }
            }

        }

        public override DeviceResult CutPaper()
        {
            //Not supported
            return DeviceResult.SUCCESS;// ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        //public override DeviceResult CloseLegalReceipt(bool openDrawer)
        //{
        //    return DeviceResult.SUCCESS;
        //}

        public override DeviceResult CancelLegalReceipt()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("+/", out fields, throwException: false);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }


        public override DeviceResult SellItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal,
            eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal)
        {
            ItemDescription = PrepareString(ItemDescription, 20);
            additionalInfo = PrepareString(additionalInfo, 20);
            additionalInfo2 = PrepareString(additionalInfo2, 16);

            //Example "3/S/PLU CODE/ITEM-1/ADDITIONAL INFO/BARCODE/1.000/100.00/1/4/KATEGIRI CODE/56";
            String command = String.Format(NumberFormat, "3/S/{0}/{1}/{2:#####.000}/{3:########.00}/{4}/{5:0.00}//",
                ItemDescription, additionalInfo, itemQuantity, finalUnitPrice, (int)vatCode, vatFactor * 100);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
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
            result = SendCommand("5/0/0////", out fields);
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
                string errorMessage = ex.Message;
                //result = SendCommand("O/2/", out fields);
                throw;
            }

            result = SendCommand("5/0/0////", out fields);

            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReceiptSubtotal(string info)
        {
            String command = "U";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult ReadHeader(out FiscalLine[] lines)
        {
            lines = new FiscalLine[8];
            String[] fields;
            MicrelecStatusCode result = SendCommand("O/", out fields);
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

            description = PrepareString(description);
            extraDescription = PrepareString(extraDescription);

            //Example "5/1/10.00/EXTRA DESCR.//CARD/” (checksum)"
            int method = (int)paymentMethod;

            String command = String.Format(NumberFormat, "5/{0}/{1:########.00}/{2}//{3}/",
                method, amount, extraDescription, description);

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


        public override DeviceResult GetCurrentDayReceiptsCount(out int receiptsCount)
        {

            receiptsCount = -1;
            String command = "0";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            if (fields.Length > 1)
            {
                int.TryParse(fields[6], out receiptsCount);
            }

            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult Feed(int lines = 1)
        {
            string[] fields;
            MicrelecStatusCode result = SendCommand(String.Format("w/1/{0}", lines), out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }


        public override DeviceResult IssueXReport()
        {
            String[] fields;
            MicrelecStatusCode result = SendCommand("x/1////", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult AddLineDiscount(eMinistryVatCategoryCode vatCode, string reason, string reasonExtended, double amount)
        {
            int ivc = (int)vatCode;
            reason = PrepareString(reason);
            reasonExtended = PrepareString(reasonExtended);

            //         “4/1.00/disc/extra descr./0/0/1/” (checksum)
            //Example "4/1/1/DISCOUNT IN SALES/EXTRA DESCR/12.75/0/0/0/0/0"
            String command = String.Format(NumberFormat, "4/{0:########.00}/{1}/{2}/0/0/1/", amount, reason, reasonExtended);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult AddSubtotalDiscount(string reason, string reasonExtended, double amountVatA, double amountVatB,
            double amountVatC, double amountVatD, double amountVatE, double totalAmount)
        {
            if (Math.Round((amountVatA + amountVatB + amountVatC + amountVatD + amountVatE), 2, MidpointRounding.AwayFromZero) != Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero))
            {
                return DeviceResult.INVALIDPROPERTY;
            }

            reason = PrepareString(reason);
            reasonExtended = PrepareString(reasonExtended);

            //Example “4/1.00/disc/extra descr./0/0/1/” (checksum)
            String command = String.Format(NumberFormat, "4/{0:########.00}/{1}/{2}/0/1/1/", totalAmount, reason, reasonExtended);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }


        public override DeviceResult ReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor)
        {
            ItemDescription = PrepareString(ItemDescription, 20);
            additionalInfo = PrepareString(additionalInfo, 20);
            additionalInfo2 = PrepareString(additionalInfo2, 16);

            //Example "3/S/PLU CODE/ITEM-1/ADDITIONAL INFO/BARCODE/1.000/100.00/1/4/KATEGIRI CODE/56";
            String command = String.Format(NumberFormat, "3/R/{0}/{1}/{2:#####.000}/{3:########.00}/{4}/{5:0.00}//", ItemDescription, additionalInfo, itemQuantity, finalUnitPrice, (int)vatCode, vatFactor * 100);
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }
        public override DeviceResult OpenDrawer(string openDrawerCustomString)
        {
            string[] fields;
            MicrelecStatusCode result = SendCommand("q//", out fields);
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        public override DeviceResult GetDayAmounts(out double vatAmountA, out double vatAmountB, out double vatAmountC, out double vatAmountD, out double vatAmountE,
                                                    out double netAmountA, out double netAmountB, out double netAmountC, out double netAmountD, out double netAmountE)
        {
            //throw new NotSupportedException();
            String command = "0";
            string[] fields;
            MicrelecStatusCode result = SendCommand(command, out fields);
            if (result == MicrelecStatusCode.SUCCESS)
            {
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
                if (fields.Length < 4)
                {
                    throw new POSUserVisibleException("Fiscal Success but returned less fields than expected");
                }
                double.TryParse(fields[0], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountA);
                double.TryParse(fields[1], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountB);
                double.TryParse(fields[2], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountC);
                double.TryParse(fields[3], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountD);
                double.TryParse(fields[4], NumberStyles.AllowThousands | NumberStyles.Float, this.NumberFormat, out netAmountE);

                double vatRateA; double vatRateB; double vatRateC; double vatRateD; double vatRateE;

                DeviceResult result2 = ReadVatRates(out vatRateA, out vatRateB, out vatRateC, out vatRateD, out vatRateE);

                if (result2 == DeviceResult.SUCCESS)
                {
                    if (netAmountA > 0)
                    {
                        vatAmountA = Math.Round(netAmountA - Math.Round((netAmountA / (1 + vatRateA)), 2), 2);
                        //netAmountA = Math.Round(netAmountA - vatAmountA, 2);
                    }
                    if (netAmountB > 0)
                    {
                        vatAmountB = Math.Round(netAmountB - Math.Round((netAmountB / (1 + vatRateB)), 2), 2);
                        //netAmountB = Math.Round(netAmountB - vatAmountB, 2);
                    }
                    if (netAmountC > 0)
                    {
                        vatAmountC = Math.Round(netAmountC - Math.Round((netAmountC / (1 + vatRateC)), 2), 2);
                        //netAmountC = Math.Round(netAmountC - vatAmountC, 2);
                    }
                    if (netAmountD > 0)
                    {
                        vatAmountD = Math.Round(netAmountD - Math.Round((netAmountD / (1 + vatRateD)), 2), 2);
                        //netAmountD = Math.Round(netAmountD - vatAmountD, 2);
                    }
                    if (netAmountE > 0)
                    {
                        vatAmountE = Math.Round(netAmountE - Math.Round((netAmountE / (1 + vatRateE)), 2), 2);
                        //netAmountE = Math.Round(netAmountE - vatAmountE, 2);
                    }

                }
            }
            else
            {
                throw new POSUserVisibleException("Fiscal Error:" + this.ErrorCodeDescriptionMapping[(int)result]);
            }
            return result == MicrelecStatusCode.SUCCESS ? DeviceResult.SUCCESS : DeviceResult.FAILURE;
        }

        /// <summary>
        /// Explicit return 0 for RBS Fiscal Printer
        /// </summary>
        /// <returns></returns>
        //public override int ReceiptsDifference(int fiscalPrinterReceipts, int applicationReceipts)
        //{
        //    return 0;
        //}
    }
}
