using ITS.POS.Client.Receipt;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IReceiptBuilder : IKernelModule
    {
        List<string> CreateReceiptHeader(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType);

        List<string> CreateReceiptFooter(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType);

        List<string> CreateReceiptBody(ReceiptSchema receiptSchema, object source, int lineChars, ConnectionType printerConnectionType);

        List<string> CreateDiscountPrintingLines(DocumentHeader header, int lineChars);

        List<string> CreatePointsPrintingLines(bool showTotals, int newPoints, int previousTotalPoints, int subtractedPoints, int lineChars, params string[] prefixLines);

        List<string> CreateSimplePrinterLines(eAlignment allignment, Device printer, bool addCutPaperCommand, params string[] lines);

        List<string> CreateFiscalVersionLines(string fiscalVersion, int lineChars, params string[] prefixLines);

        List<string> CreateWithdrawOrDepositLines(Device printer, User currentUser, string headerLine, decimal value, string store, IReceiptBuilder receiptBuilder, IConfigurationManager config, Reason reason = null, string comments = null, string userCode = null);

    }
}
