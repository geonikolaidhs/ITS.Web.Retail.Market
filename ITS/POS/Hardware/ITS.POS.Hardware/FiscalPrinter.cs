using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Reflection;
using System.Net.Sockets;
using System.Net;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Base class for all fiscal printers. 
    /// The implementation for each fiscal printer must have its own assembly and version.
    /// </summary>
    [RequireOnlyOneSuccessfulDeviceType]
    public abstract class FiscalPrinter : Device, IDisposable, IDrawerPrinter
    {

        protected int PosID;
        protected SerialPort serialPort;
        protected TcpClient tcpClient;
        protected TcpListener tcpListener;
        protected int LineChars;
        protected int CommandChars;

        public FiscalPrinter(ConnectionType conType, String deviceName, int posID, int lineChars, int commandChars)
            : base()
        {
            ConType = conType;
            DeviceName = deviceName;
            this.PosID = posID;
            this.FiscalDayStartedOnFirstReceipt = false;
            LineChars = lineChars;
            CommandChars = commandChars;
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);

            //this.Settings.LineChars = this.LineCharsForIllegalPrinting;
        }

        /// <summary>
        /// The chars per line of the fiscal printer for Printing Illegal receipts. This will override the "Settings.LineChars" property that the user provided.
        /// </summary>
        protected abstract int LineCharsForIllegalPrinting { get; }

        /// <summary>
        /// Gets a collection of flags that describe the printer's status.
        /// </summary>
        public virtual FiscalPrinterStatus FiscalStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the version of the fiscal printer's assembly. Must be seperate from the rest of the application.
        /// </summary>
        public abstract Version FiscalVersion { get; }

        /// <summary>
        /// Returns true if the printer supports Slip Station printing, else returns false.
        /// </summary>
        public abstract bool HasSlipStation { get; }

        /// <summary>
        /// Returns the maximum payment amount that the printer supports.
        /// </summary>
        public abstract double MaximumPaymentAmount { get; }

        /// <summary>
        /// Returns true if VAT is calculated per receipt, else returns false.
        /// </summary>
        public abstract bool IsTotalVatCalculationPerReceipt { get; }

        public virtual DeviceResult InitializeConnection()
        {
            if (serialPort != null)
            {
                return DeviceResult.CONNECTIONNOTSUPPORTED;
            }
            serialPort = new SerialPort(this.Settings.COM.PortName, this.Settings.COM.BaudRate, this.Settings.COM.Parity, this.Settings.COM.DataBits, this.Settings.COM.StopBits);
            //serialPort.ReadBufferSize = 2048;
            //serialPort.WriteBufferSize = 512;
            serialPort.Encoding = Encoding.GetEncoding(1253);
            serialPort.Handshake = this.Settings.COM.Handshake;
            serialPort.WriteTimeout = this.Settings.COM.WriteTimeOut;


            return DeviceResult.SUCCESS;
        }
        /// <summary>
        /// opens tcp connection
        /// </summary>
        /// <returns></returns>
        public virtual DeviceResult InitializeEthernetConnection()
        {

            //string hostname = this.Settings.Ethernet.IPAddress;
            //int portno = this.Settings.Ethernet.Port;
            //IPAddress ipa = (IPAddress)Dns.GetHostAddresses(hostname)[0];

            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
                tcpClient = new TcpClient();
                tcpClient.Connect(this.Settings.Ethernet.IPAddress, this.Settings.Ethernet.Port);

                //System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                //sock.Connect(ipa, portno);
                //if (sock.Connected == true)  // Port is in use and connection is successful
                //{
                //    return DeviceResult.SUCCESS;
                //}
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
                throw;
            }
            catch (Exception exception)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
                throw;
            }
            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// Opens the fiscal day.
        /// </summary>
        /// <param name="posID"></param>
        /// <returns></returns>
        public abstract DeviceResult OpenFiscalDay(int posID);

        /// <summary>
        /// Closes the fiscal day and issues a Z report.
        /// </summary>
        /// <param name="pathToAbc"></param>
        /// <param name="zReportNumber"></param>
        /// <param name="pathToEJFiles"></param>
        /// <returns></returns>
        public abstract DeviceResult IssueZReport(String pathToAbc, out int zReportNumber, out string pathToEJFiles);

        /// <summary>
        /// Commands the printer to cut the station's paper
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult CutPaper();

        public virtual void Dispose()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Dispose();
            }
        }

        /// <summary>
        /// Returns the Difference Between Apllication receipts & Fiscsal Printer
        /// </summary>
        /// <returns></returns>
        public virtual int ReceiptsDifference(int fiscalPrinterReceipts, int applicationReceipts)
        {
            return fiscalPrinterReceipts - applicationReceipts;
        }

        /// <summary>
        /// Starts a legal receipt. Fiscal day must be opened.
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult OpenLegalReceipt();

        /// <summary>
        /// Finalizes an open legal receipt. A receipt must be already open.
        /// </summary>
        /// <param name="openDrawer"></param>
        /// <returns></returns>
        public abstract DeviceResult CloseLegalReceipt(bool openDrawer);

        /// <summary>
        /// Cancels an open legal receipt. A receipt must be already open.
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult CancelLegalReceipt();

        /// <summary>
        /// Issues a X report.
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult IssueXReport();

        /// <summary>
        /// Returns the total number of legal receipts in the current day.
        /// </summary>
        /// <param name="receiptsCount"></param>
        /// <returns></returns>
        public abstract DeviceResult GetCurrentDayReceiptsCount(out int receiptsCount);

        /// <summary>
        /// Gets the totals of the current open transaction.
        /// </summary>
        /// <param name="vatAGrossTotal"></param>
        /// <param name="vatBGrossTotal"></param>
        /// <param name="vatCGrossTotal"></param>
        /// <param name="vatDGrossTotal"></param>
        /// <param name="vatEGrossTotal"></param>
        /// <param name="receiptNumber"></param>
        /// <param name="grossTotal"></param>
        /// <returns></returns>
        public abstract DeviceResult GetTransactionInfo(out double vatAGrossTotal, out double vatBGrossTotal, out double vatCGrossTotal, out double vatDGrossTotal, out double vatEGrossTotal, out int receiptNumber, out double grossTotal);

        /// <summary>
        /// Returns true if the printer is in a status that can print illegal receipts.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public abstract bool CheckIfCanPrintIllegal(out string reason);

        /// <summary>
        /// Prints to the slip station. No need to implement if slip printing is not supported.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public abstract DeviceResult SlipStationPrint(params FiscalLine[] lines);

        /// <summary>
        /// Returns true if the slip station has paper. No need to implement if slip printing is not supported.
        /// </summary>
        /// <param name="hasPaper"></param>
        /// <returns></returns>
        public abstract DeviceResult SlipStationHasPaper(out bool hasPaper);

        /// <summary>
        /// Moves the current open legal receipt to payment mode.
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult ReceiptPaymentMode(double grossTotal);

        /// <summary>
        /// Sets the current open legal receipt to subtotal mode, so that discounts can be applied to the current total.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract DeviceResult ReceiptSubtotal(string info);

        /// <summary>
        /// Adds a discount to the current open legal receipt's subtotal. The receipt must be in subtotal mode.
        /// </summary>
        /// <param name="reason">Description to print in the receipt.</param>
        /// <param name="reasonExtended">Extra description to print in the receipt.</param>
        /// <param name="amountVatA">The amount of the discount that goes to VAT A.</param>
        /// <param name="amountVatB">The amount of the discount that goes to VAT B.</param>
        /// <param name="amountVatC">The amount of the discount that goes to VAT C.</param>
        /// <param name="amountVatD">The amount of the discount that goes to VAT D.</param>
        /// <param name="amountVatE">The amount of the discount that goes to VAT E.</param>
        /// <param name="totalAmount">The total amount of the discount.</param>
        /// <returns></returns>
        public abstract DeviceResult AddSubtotalDiscount(string reason, string reasonExtended, double amountVatA, double amountVatB, double amountVatC, double amountVatD, double amountVatE, double totalAmount);

        /// <summary>
        /// Adds a discount to the given VAT.
        /// </summary>
        /// <param name="vatCode"></param>
        /// <param name="reason"></param>
        /// <param name="reasonExtended"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public abstract DeviceResult AddLineDiscount(eMinistryVatCategoryCode vatCode, string reason, string reasonExtended, double amount);

        /// <summary>
        /// Adds an item to the current open legal receipt.
        /// </summary>
        /// <param name="ItemDescription"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="additionalInfo2"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="finalUnitPrice"></param>
        /// <param name="lineTotal"></param>
        /// <param name="vatCode"></param>
        /// <param name="vatFactor"></param>
        /// <param name="supportsDecimal"></param>
        /// <returns></returns>
        public abstract DeviceResult SellItem(String ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal);

        /// <summary>
        /// Adds an item return to the current open legal receipt.
        /// </summary>
        /// <param name="ItemDescription"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="additionalInfo2"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="finalUnitPrice"></param>
        /// <param name="lineTotal"></param>
        /// <param name="vatCode"></param>
        /// <param name="vatFactor"></param>
        /// <returns></returns>
        public abstract DeviceResult ReturnItem(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor);

        /// <summary>
        /// Adds a payment to the current open legal receipt. Receipt must be in payments mode.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="remainingBalance"></param>
        /// <param name="description"></param>
        /// <param name="extraDescription"></param>
        /// <returns></returns>
        public abstract DeviceResult AddPayment(double amount, ePaymentMethodType paymentMethod, out double remainingBalance, String description = "", string extraDescription = "");

        /// <summary>
        /// Checks communication with the device and updates the FiscalStatus property.
        /// </summary>
        /// <returns></returns>
        public abstract DeviceResult ReadDeviceStatus();

        /// <summary>
        /// Prints an illegal receipt with the given lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public abstract DeviceResult PrintIllegal(params FiscalLine[] lines);

        /// <summary>
        /// Commands the printer to feed the paper with the given amount of lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public abstract DeviceResult Feed(int lines = 1);

        /// <summary>
        /// Adds an item return to the current open legal receipt and cancels it immediatelly.
        /// Implement if you want the printer to print cancelations, else leave it with an empty body.
        /// </summary>
        /// <param name="ItemDescription"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="additionalInfo2"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="finalUnitPrice"></param>
        /// <param name="lineTotal"></param>
        /// <param name="vatCode"></param>
        /// <param name="vatFactor"></param>
        /// <returns></returns>
        public abstract DeviceResult ReturnItemAndCancelIt(string ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor);

        /// <summary>
        /// Adds an item to the current open legal receipt and cancels it immediatelly.
        /// Implement if you want the printer to print cancelations, else leave it with an empty body.
        /// </summary>
        /// <param name="ItemDescription"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="additionalInfo2"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="finalUnitPrice"></param>
        /// <param name="lineTotal"></param>
        /// <param name="vatCode"></param>
        /// <param name="vatFactor"></param>
        /// <param name="supportsDecimal"></param>
        /// <returns></returns>
        public abstract DeviceResult SellItemAndCancelIt(String ItemDescription, string additionalInfo, string additionalInfo2, double itemQuantity, double finalUnitPrice, double lineTotal, eMinistryVatCategoryCode vatCode, double vatFactor, bool supportsDecimal);

        /// <summary>
        /// Reprints Z reports with a date filter.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract DeviceResult ReprintZReportsDateToDate(DateTime fromDate, DateTime toDate, eReprintZReportsMode mode);

        /// <summary>
        /// Reprints Z reports with a number filter.
        /// </summary>
        /// <param name="fromZ"></param>
        /// <param name="toZ"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract DeviceResult ReprintZReports(int fromZ, int toZ, eReprintZReportsMode mode);

        /// <summary>
        /// Reprints receipts of the current fiscal day.
        /// </summary>
        /// <param name="fromReceiptNumber"></param>
        /// <param name="toReceiptNumber"></param>
        /// <returns></returns>
        public abstract DeviceResult ReprintReceiptsOfCurrentZ(int fromReceiptNumber, int toReceiptNumber);

        /// <summary>
        /// Adds a cash amount to the printer's totals.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract DeviceResult CashIn(double amount, string message);

        /// <summary>
        /// Removes a cash amount from the printer's totals.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract DeviceResult CashOut(double amount, string message);

        /// <summary>
        /// Prints raw fiscal memory blocks.
        /// </summary>
        /// <param name="fromBlock"></param>
        /// <param name="toBlock"></param>
        /// <returns></returns>
        public abstract DeviceResult PrintFiscalMemoryBlocks(int fromBlock, int toBlock);

        /// <summary>
        /// Sets the fiscal printer's VAT rates. 
        /// Beware, there is usually a limit of how many times the VAT rates can change.
        /// </summary>
        /// <param name="vatRateA"></param>
        /// <param name="vatRateB"></param>
        /// <param name="vatRateC"></param>
        /// <param name="vatRateD"></param>
        /// <param name="vatRateE"></param>
        /// <returns></returns>
        public abstract DeviceResult SetVatRates(double vatRateA, double vatRateB, double vatRateC, double vatRateD, double vatRateE);

        /// <summary>
        /// Sets the fiscal printer's header lines. 
        /// Beware, there is usually a limit of how many times the header can change.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public abstract DeviceResult SetHeader(params FiscalLine[] lines);

        /// <summary>
        /// Sets the cashier's ID that is printed on every receipt, if supported by the printer.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract DeviceResult SetCashierID(string id);

        /// <summary>
        /// Reads the header lines, if supported by the printer.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public abstract DeviceResult ReadHeader(out FiscalLine[] lines);

        /// <summary>
        /// Reads the current VAT rates of the printer.
        /// </summary>
        /// <param name="vatRateA"></param>
        /// <param name="vatRateB"></param>
        /// <param name="vatRateC"></param>
        /// <param name="vatRateD"></param>
        /// <param name="vatRateE"></param>
        /// <returns></returns>
        public abstract DeviceResult ReadVatRates(out double vatRateA, out double vatRateB, out double vatRateC, out double vatRateD, out double vatRateE);

        /// <summary>
        /// Gets the current open fiscal day's vat and net amounts.
        /// </summary>
        /// <param name="vatAmountA"></param>
        /// <param name="vatAmountB"></param>
        /// <param name="vatAmountC"></param>
        /// <param name="vatAmountD"></param>
        /// <param name="vatAmountE"></param>
        /// <param name="netAmountA"></param>
        /// <param name="netAmountB"></param>
        /// <param name="netAmountC"></param>
        /// <param name="netAmountD"></param>
        /// <param name="netAmountE"></param>
        /// <returns></returns>
        public abstract DeviceResult GetDayAmounts(out double vatAmountA, out double vatAmountB, out double vatAmountC, out double vatAmountD, out double vatAmountE,
                                                   out double netAmountA, out double netAmountB, out double netAmountC, out double netAmountD, out double netAmountE);

        protected virtual bool WaitSerialPortResponse(int timeout)
        {
            if (serialPort.IsOpen)
            {
                int ExpectedTicks = Environment.TickCount + timeout;
                do
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        return true;
                    }
                }
                while (Environment.TickCount <= ExpectedTicks);
            }
            Debug.WriteLine("Timeout");
            return false;
        }

        public abstract DeviceResult OpenDrawer(string openDrawerCustomString);

        public virtual bool FiscalDayStartedOnFirstReceipt { get; set; }

        public virtual bool NumberOfReceiptsIncludeCanceled { get; set; }
    }
}
