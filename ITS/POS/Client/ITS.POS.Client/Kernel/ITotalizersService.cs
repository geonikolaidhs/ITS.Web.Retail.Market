using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
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
    public interface ITotalizersService : IKernelModule
    {
        bool CheckIfMustIssueZ(DailyTotals currentDailyTotals);

        /// <summary>
        /// Δημιουργεί ενα νεο DailyTotal ( Z ). Νεα φορολογική ημέρα εγρασίας 
        /// </summary>
        /// <returns>DailyTotals </returns>
        DailyTotals CreateDailyTotals(Guid terminal, Guid currentStore, Guid currentDailyTotals, out DateTime currentFiscalDate);

        /// <summary>
        /// Ανοιγμα βάρδιας χρήστη
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        UserDailyTotals CreateUserDailtyTotals(Guid user, Guid currentDailyTotals, Guid currentUserDailyTotals, Guid currentTerminal, Guid currentStore);

        void IncreaseDrawer(User currentUser, DailyTotals currentDailyTotals, UserDailyTotals currentUserDaily, Store currentStore, ITS.POS.Model.Settings.POS currentPOS);

        /// <summary>
        /// Generates an XReport object used for printing
        /// </summary>
        /// <param name="udt"></param>
        Report CreateXReport(OwnerApplicationSettings appSettings, DailyTotals dt, UserDailyTotals udt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType, bool asksForFinalAmount);

        /// <summary>
        /// Generates an Report object used for printing
        /// </summary>
        /// <param name="udt"></param>
        Report CreateZReport(OwnerApplicationSettings appSettings, DailyTotals dt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType, bool asksForFinalAmount);

        void UpdateTotalizers(IConfigurationManager config, DocumentHeader header, User currentUser, DailyTotals currentDailyTotals,
            UserDailyTotals currentUserDaily, Guid currentStoreOid, Guid currentPOSOid, bool increaseVatAndSales,
            bool totalVatCalculationPerReceipt, eTotalizorAction totalizorAction, bool isFiscalPrinter = false, decimal vatAGrossTotal = 0,
            decimal vatBGrossTotal = 0, decimal vatCGrossTotal = 0, decimal vatDGrossTotal = 0,
            decimal vatEGrossTotal = 0, decimal grossTotal = 0);

        void SetVatTotalsFromFiscalPrinter(DailyTotals currentDailyTotals, decimal fiscalVatAmountA, decimal fiscalVatAmountB, decimal fiscalVatAmountC, decimal fiscalVatAmountD, decimal fiscalVatAmountE);

        void FixDocumentVatDeviations(IDocumentService documentService, IConfigurationManager config, DailyTotals currentDailyTotals, VatFactor vatFactor, OwnerApplicationSettings ownerApplicationSettings);

        decimal GetTotalCashInPos(DailyTotals dailyTotals);

        decimal GetDayStartingAmount(DailyTotals dailyTotals);

        void SaveUserDailyTotalCashDifference(IAppContext appContext, decimal cashDifference, decimal userClosingShiftCount, Guid currentUserOid, Guid currentPOSOid, Guid currentStoreOid);

        int GetNumberOfDocumentsInDailyTotals(DailyTotals dailyTotals, Guid defaultDocumentType);
    }
}
