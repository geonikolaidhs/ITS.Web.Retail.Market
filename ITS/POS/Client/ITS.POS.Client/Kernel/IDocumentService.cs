using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Common.ViewModel;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using NLog;
using System;
using System.Collections.Generic;

namespace ITS.POS.Client.Kernel
{
    public interface IDocumentService : IIntermediateDocumentService, IKernelModule
    {
        /// <summary>
        /// Checks if a Document Sequence exists for the given Document Series
        /// </summary>
        /// <param name="documentSeriesOid"></param>
        /// <returns></returns>
        bool SequenceExists(Guid documentSeriesOid);

        bool ZSequenceExists(Guid terminalOid);


        /// <summary>
        /// Δημιουργία νέου παραστικού
        /// </summary>
        /// <param name="division"></param>
        /// <param name="docType"></param>
        /// <param name="docSeries"></param>
        /// <param name="store"></param>
        /// <param name="customer"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        DocumentHeader CreateDocumentHeader(eDivision division, Guid pos, Guid docType,
            Guid docSeries, Guid store, Guid customer, string customerCode, string customerName, Guid priceCatalogPolicy, Guid status, Guid userDailyTotals);

        void ClearPayments(DocumentHeader header);

        DocumentHeader CreateWithdrawOrDeposit(User currentUser, UserDailyTotals currentUserDailyTotals, Guid docType, Guid docSeries, decimal value,
            SpecialItem item, Guid priceCatalogPolicy, string customDescription, bool isShiftStartingDeposit, bool isDayStartingDeposit, Guid? reasonGuid = null, string comment = null, string taxCode = null);


        /// <summary>
        /// Δημιουργία γραμμης παραστατικού
        /// </summary>
        /// <param name="header">Παραστατικό</param>
        /// <param name="user">Χρήστης</param>
        /// <param name="code">Κωδικός είδους</param>
        /// <param name="qty">Ποσότητα</param>
        /// <param name="secondDiscount">Δεύτερη έκπτωση</param>
        /// <param name="price">Τιμή</param>
        /// <returns></returns>
        DocumentDetail CreateDocumentLine(DocumentHeader header, Guid user,
            Item item, PriceCatalogDetail pcd, Barcode userBarcode, decimal Qty, decimal customPrice, bool hasCustomPrice, bool isReturn, string customDescription = "");

        void CancelDocument(DocumentHeader document, DateTime fiscalDate, int docNo = 0, bool isFiscalPrinterHandled = false);

        bool CheckLoyaltyRefund(DocumentHeader header);

        /// <summary>
        /// Gets the discount distributed at the document lines by the document header, per vat category
        /// </summary>
        /// <param name="header"></param>
        /// <param name="vatADiscount"></param>
        /// <param name="vatBDiscount"></param>
        /// <param name="vatCDiscount"></param>
        /// <param name="vatDDiscount"></param>
        /// <param name="vatEDiscount"></param>
        void GetDocumentDiscountPerVatCategory(DocumentHeader header, out decimal vatADiscount, out decimal vatBDiscount, out decimal vatCDiscount, out decimal vatDDiscount, out decimal vatEDiscount);

        /// <summary>
        /// Ακύρωση γραμμής παραστατικού και ενημέρωση του header του παραστατικού.
        /// </summary>
        /// <param name="line"></param>
        void CancelDocumentLine(DocumentDetail line);

        /// <summary>
        /// Changes the unit price of the detail and recomputes its header
        /// </summary>
        /// <param name="line"></param>
        /// <param name="newUnitPrice"></param>
        void ChangeDocumentLinePrice(DocumentDetail line, decimal newUnitPrice);

        DocumentDetailDiscount CreateOrUpdateCustomDiscount(DiscountType discountType, decimal valueOrPercentage, DocumentDetail detail);

        IEnumerable<DocumentVatInfo> GetDocumentVatAnalysis(DocumentHeader documentHeader);

        /// <summary>
        /// Υπολογισμος Header Παραστατικού
        /// </summary>
        /// <param name="header"></param>
        /// <param name="ActionDelete"></param>
        /// <param name="newLine"></param>
        /// <param name="OldLine"></param>
        /// <returns></returns>
        DocumentHeader ComputeDocumentHeader(ref DocumentHeader header, bool actionDelete, DocumentDetail currentLine);

        DocumentDetail FindLine(string code, DocumentHeader header);

        void AssignDocumentNumber(DocumentHeader header, Guid currentPOSOid, Guid currentUserOid, int fiscalPrinterReceiptNumber = -1);

        int GetNextDocumentNumber(DocumentHeader header, Guid currentPOSOid, Guid currentUserOid);

        /// <summary>
        /// Calculates and applies the DocumentDiscountPercentagePerLine that must be applied to all the lines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="discount"></param>
        /// <param name="appSettings"></param>
        /// <param name="discountType"></param>
        /// <param name="couponViewModel">The view model for the selected coupon. Null if no coupon is used for the Document Discount.</param>
        void ApplyCustomDocumentHeaderDiscount(ref DocumentHeader header, decimal discount, DiscountType discountType, CouponViewModel couponViewModel = null);

        void FixFiscalPrinterDeviationsFromDiscounts(decimal fiscalPrinterVatGrossTotal, eMinistryVatCategoryCode vatCode, DocumentHeader header);

        /// <summary>
        /// Calculates and applies the PointsDiscountPercentagePerLine that must be applied to all the lines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="customer"></param>
        /// <param name="appSettings"></param>
        void ApplyLoyalty(DocumentHeader header, IActionManager actionManager);

        void ClearAppliedLoyalty(DocumentHeader header);

        bool CheckIfShouldOpenDrawer(DocumentHeader header);

        bool CheckIfShouldGiveChange(DocumentHeader header);

        /// <summary>
        /// Σε κάθε παραστατικό που εκδίδεται είναι απαραίτητη και η ύπαρξη μιας γραμμής που περιέχει τα οικονομικά στοιχεία που πρέπει να διαβιβαστούν στην βάση δεδομένων της ΓΓΠΣ.  
        ///Αυτά  έχουν περιγραφεί από την ΠΟΛ 1221 και είναι τα κάτωθι:
        ///α/α Πεδίου   Περιεχόμενο             Μήκος (χαρακτήρες)
        ///0        ΑΦΜ Εκδότη                              12
        ///1        ΑΦΜ Παραλήπτη                           12
        ///2        Αριθμός Κάρτας Αποδείξεων Πελάτη  *1    19
        ///3        Ημερομηνία και Ώρα                *4    12 
        ///4        Περιγραφή Παραστατικού            *2 (Μεταβλητό)
        ///5        Σειρά Θεώρησης                          10
        ///6        Αριθμός Παραστατικού                    10
        ///7        Καθαρό Ποσό Α                           18:2
        ///8        Καθαρό Ποσό Β                           18:2
        ///9        Καθαρό Ποσό Γ                           18:2
        ///10       Καθαρό Ποσό Δ                           18:2
        ///11       Καθαρό Ποσό Ε                           18:2
        ///12       ΦΠΑ Α                                   18:2
        ///13       ΦΠΑ Β                                   18:2
        ///14       ΦΠΑ Γ                                   18:2
        ///15       ΦΠΑ Δ                                   18:2
        ///16       Γενικό Σύνολο Παρ/κού                   18:2
        ///17       Κωδικός νομίσματος             
        ///
        ///*1    ΑΦΜ ή o αριθμό της πιστωτικής κάρτας του πελάτη αν δεν έχει κάρτα αποδείξεων.
        ///*2   Αν είναι γνωστός ο κωδικός του παραστατικού βάση της τυποποίησης των παραστατικών του taxis, τότε μπορεί να χρησιμοποιηθεί απ’ ευθείας στο πεδίο αυτό.
        ///Διαφορετικά μπορεί να χρησιμοποιηθεί η περιγραφή του παραστατικού. Κατόπιν χρειάζεται να προγραμματίσουμε στον driver την αντιστοίχηση της περιγραφής του πεδίου αυτού με τον σωστό κωδικό Taxis.
        ///*4 Η μορφοποίηση της ημερομηνίας είναι : YYYYMMDDHHmm 
        ///
        /// YYYY = έτος   ΜΜ = Μήνας  DD = Μέρα  HH = Ώρα  mm = Λεπτά
        /// EXAMPLE: 999999999/123456789/1234567890123456789/020420131200/173/A/1001/3.00/3.00/3.00/3.00/3.00/0.19/0.39/0.69/1.08/14.35/1
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        string CreateFiscalInfoLine(DocumentHeader header, Logger logger, string customerReceiptCard = null, int currencyCode = 0, string seperator = "/");

        /// <summary>
        /// Generates a DocumentPaymentEdps record from the result of Edps Device
        /// </summary>
        /// <param name="edpsresult">The result of Edps device</param>
        /// <param name="session">The session where the DocumentPaymentEdps will be created</param>
        /// <returns>The DocumentPaymentEdps record</returns>
        DocumentPaymentEdps CreateDocumentPaymentEdps(EdpsDeviceResult edpsresult, decimal amount, Session session);


        /// <summary>
        /// Generates a DocumentPaymentCardlink record from the result of Edps Device
        /// </summary>
        /// <param name="cardlinkresult">The result of Edps device</param>
        /// <param name="session">The session where the DocumentPaymentEdps will be created</param>
        /// <returns>The DocumentPaymentEdps record</returns>
        DocumentPaymentCardlink CreateDocumentPaymentCardlink(CardlinkDeviceResult cardlinkresult, decimal amount, Session session);

        /// <summary>
        /// Creates a TransactionCoupon for DocumentHeader header based on CouponViewModel couponViewModel.Please provide ONLY ONE of documentDetailDiscount or documentPayment otherwise a POSException will be thrown!
        /// </summary>
        /// <param name="header">The relevant DocumentHeader that the Coupon is used.</param>
        /// <param name="couponViewModel">The view model for the selected coupon</param>
        /// <param name="documentDetailDiscount">Should be null if documentPayment is provided. Otherwise the DocumentDetailDiscount that is created by applying the coupon.</param>
        /// <param name="documentPayment">Should be null if documentDetailDiscount is provided. Otherwise the DocumentPayment that is created by applying the coupon.</param>
        /// <returns>Nothing. The TransactionCoupon is created by applying the coupon internally</returns>
        void CreateTransactionCoupon(DocumentHeader header, CouponViewModel couponViewModel, DocumentDetailDiscount documentDetailDiscount = null, DocumentPayment documentPayment = null);


        void FixDocumentDiscountDeviations(DocumentHeader header);

        void ClearDocumentTotalDiscounts(DocumentHeader documentHeader);

        /// <summary>
        /// Checks if a document is valid to move to payments mode
        /// </summary>
        /// <param name="documentHeader">The document header to be checked</param>
        /// <param name="message">The error message in case of invalid state</param>
        /// <returns>true if is valid, false otherwise</returns>
        bool CheckIfDocumentIsValidForMovingToPayment(DocumentHeader documentHeader, out string message);

        bool CheckIfCustomerIsValidForDocumentType(Guid documentType, Guid customer);

        List<DocumentDetail> PriceCatalogNotIncludedItems(IConfigurationManager configManager, ISessionManager sessionManager, DocumentHeader document, Guid priceCatalogOid);

        bool CheckIfDocumentDetailsAreValidForDocumentType(DocumentHeader documentHeader, out List<DocumentDetail> invalid);

        PriceCatalogDetail GetPriceCatalogDetail(IConfigurationManager configManager, ISessionManager sessionManager, Guid itemOid, Guid priceCatalogOid, Guid barcodeOid);

        decimal GetDefaultDocumentDiscountPercentage(DocumentHeader document);

        void ApplyDefaultDocumentDiscount(DocumentHeader header, decimal discountPercentage);

        void ApplyCustomerDiscount(DocumentHeader header, decimal discountPercentage);

        decimal GetTotalNonDiscountableValue(DocumentHeader document);

        decimal GetCustomerDiscountPercentage(Customer customer);

        void ApplyDocumentTotalDiscounts(DocumentHeader header);
        POSDocumentReportSettings GetReportForDocumentType(Guid docType, IConfigurationManager configManager);

        XPCollection<DocumentType> GetAllValidPosDocumentTypes(IConfigurationManager config, ISessionManager sessionManager);

        XPCollection<DocumentType> GetDefaultPosDocumentTypes(IConfigurationManager configurationManager, ISessionManager sessionManager);


    }
}
