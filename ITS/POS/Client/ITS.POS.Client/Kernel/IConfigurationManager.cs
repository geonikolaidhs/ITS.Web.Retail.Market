using ITS.POS.Client.Helpers;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Client.Receipt;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ITS.POS.Client.Kernel
{
    public interface IConfigurationManager : IKernelModule
    {
        Guid CurrentStoreOid { get; set; }
        Guid CurrentTerminalOid { get; set; }
        int TerminalID { get; set; }
        Guid DefaultCustomerOid { get; set; }
        Guid DefaultDocumentTypeOid { get; set; }
        Guid ProFormaInvoiceDocumentTypeOid { get; set; }
        Guid ProFormaInvoiceDocumentSeriesOid { get; set; }
        Guid SpecialProformaDocumentTypeOid { get; set; }
        Guid SpecialProformaDocumentSeriesOid { get; set; }
        Guid DepositDocumentTypeOid { get; set; }
        Guid DepositDocumentSeriesOid { get; set; }
        Guid DepositItemOid { get; set; }
        Guid WithdrawalDocumentTypeOid { get; set; }
        Guid WithdrawalDocumentSeriesOid { get; set; }
        Guid WithdrawalItemOid { get; set; }
        Guid DefaultDocumentStatusOid { get; set; }
        Guid DefaultPaymentMethodOid { set; get; }
        Guid DefaultDocumentSeriesOid { get; set; }
        bool UsesTouchScreen { get; set; }
        bool UsesKeyLock { get; set; }
        bool AutoFocus { get; set; }
        bool AsksForStartingAmount { get; set; }
        bool AsksForFinalAmount { get; set; }
        bool POSSellsInactiveItems { get; set; }
        bool AutoIssueZEAFDSS { get; set; }
        bool EnableLowEndMode { get; set; }
        bool DemoMode { get; set; }
        bool UseSliderPauseForm { get; set; }
        bool UseCashCounter { get; set; }
        eForcedWithdrawMode ForcedWithdrawMode { get; set; }
        eDocumentDetailPrintDescription DocumentDetailPrintDescription { get; set; }
        decimal ForcedWithdrawCashAmountLimit { get; set; }
        bool PrintDiscountAnalysis { get; set; }
        ReceiptSchema ReceiptSchema { get; set; }
        ReceiptSchema ΧReportSchema { get; set; }
        ReceiptSchema ZReportSchema { get; set; }
        string ReceiptVariableIdentifier { get; set; }
        string CurrencySymbol { get; set; }
        eCurrencyPattern CurrencyPattern { get; set; }
        int CurrencyLocation { get; set; }
        string ABCDirectory { get; set; }
        eFiscalMethod FiscalMethod { get; }
        eFiscalDevice FiscalDevice { get; set; }
        eLocale Locale { get; set; }
        List<Image> PauseFormImages { get; set; }
        string StoreControllerWebServiceURL { get; set; }
        bool CheckIfSettingsAreValid(out string message);
        OwnerApplicationSettings GetAppSettings();
        void ReloadApplicationSettings();

        List<POSDocumentReportSettings> DocumentReports { get; set; }

        List<DatabaseCommand> DbCommands { get; set; }

        Receipt.ReceiptSchema GetReceiptSchema(Model.Transactions.DocumentHeader documentHeader);
    }
}
