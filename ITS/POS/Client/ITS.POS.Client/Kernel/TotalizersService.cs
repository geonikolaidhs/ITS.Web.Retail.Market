using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides functions for creating and updating daily and user-daily totals
    /// </summary>
    public class TotalizersService : ITotalizersService
    {
        ISessionManager SessionManager { get; set; }
        IPlatformRoundingHandler PlatformRoundingHandler { get; set; }

        public TotalizersService(ISessionManager sessionManager, IPlatformRoundingHandler platformRoundingHandler)
        {
            this.SessionManager = sessionManager;
            this.PlatformRoundingHandler = platformRoundingHandler;
        }

        public bool CheckIfMustIssueZ(DailyTotals currentDailyTotals)
        {
            double daysDif = DateTime.Now.Subtract(currentDailyTotals.FiscalDate).TotalDays;

            return daysDif > 1;

        }

        /// <summary>
        /// Δημιουργεί ενα νεο DailyTotal ( Z ). Νεα φορολογική ημέρα εργασίας 
        /// </summary>
        /// <returns>DailyTotals </returns>
        public DailyTotals CreateDailyTotals(Guid terminal, Guid currentStore, Guid currentDailyTotals, out DateTime currentFiscalDate)
        {
            DailyTotals dTotals;
            currentFiscalDate = DateTime.MinValue;
            if (currentDailyTotals != Guid.Empty)
            {
                dTotals = SessionManager.GetObjectByKey<DailyTotals>(currentDailyTotals);
                if (dTotals != null && dTotals.FiscalDateOpen)
                {
                    // το Z υπάρχει και είναι ανοικτό

                    double daysDif = DateTime.Now.Subtract(dTotals.FiscalDate).TotalDays;
                    //if (daysDif > 2)
                    //{
                    //    // Αν η τρέχουσα ημερομηνία είναι μεγαλύτερη απο την fiscalDate κατα 2 τουλαχιστον ημέρες ΄δωσε λάθος

                    //    throw new Exception("Fiscal Date period finished");
                    //}
                    currentFiscalDate = dTotals.FiscalDate;
                    return dTotals; // επέστερψε το Z που βρήκες;              
                }
            }

            // ή το Ζ δεν υπάρχει ή υπάρχει και είναι κλειστό 
            // δημιoυργισε ένα νέο.
            dTotals = new DailyTotals(SessionManager.GetSession<DailyTotals>()) { FiscalDate = DateTime.Now, FiscalDateOpen = true, Store = currentStore, POS = terminal };
            dTotals.CreatedByDevice = terminal.ToString();
            SessionManager.FillDenormalizedFields(dTotals);
            dTotals.Save();
            SessionManager.CommitTransactionsChanges();
            currentFiscalDate = dTotals.FiscalDate;
            return dTotals;
        }

        /// <summary>
        /// Ανοιγμα βάρδιας χρήστη
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserDailyTotals CreateUserDailtyTotals(Guid user, Guid currentDailyTotals, Guid currentUserDailyTotals, Guid currentTerminal, Guid currentStore)
        {
            UserDailyTotals udt;
            DailyTotals dt = SessionManager.GetObjectByKey<DailyTotals>(currentDailyTotals);
            if (dt == null)
            {
                throw new Exception("Fiscal Date not opened");
            }
            if (currentUserDailyTotals != Guid.Empty)
            {
                udt = SessionManager.GetObjectByKey<UserDailyTotals>(currentUserDailyTotals);
                if (udt != null && user == udt.User) // αν βρέθηκε και ο User είναι ο ιδιος τότε επέστρεψέ το.
                {
                    return udt;
                }
            }
            // δεν βρέθηκε ή ο User είναι διαφορετικός.
            try
            {
                udt = new UserDailyTotals(SessionManager.GetSession<UserDailyTotals>());
                udt.User = user;
                udt.DailyTotals = dt;
                udt.POS = currentTerminal;
                udt.Store = currentStore;
                udt.FiscalDate = dt.FiscalDate;
                udt.CreatedBy = user;
                udt.CreatedByDevice = dt.CreatedByDevice;
                udt.IsOpen = true;
                SessionManager.FillDenormalizedFields(udt);
                udt.Save();
                udt.Session.CommitTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Can not create User Daily Totals\n\n ERROR:{0}", ex.GetFullMessage()));
            }
            return udt;
        }

        public void IncreaseDrawer(User currentUser, DailyTotals currentDailyTotals, UserDailyTotals currentUserDaily, Store currentStore, ITS.POS.Model.Settings.POS currentPOS)
        {

            this.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.DRAWERS, Guid.Empty, currentPOS.Oid, currentStore.Oid, 1, 0).Save();
            SessionManager.CommitTransactionsChanges();
        }

        /// <summary>
        /// Αναζήτηση του DailyTotalDetail με βάση το RecordType και GUID από το έιδος του Record που ψάχνουμε )
        /// </summary>
        /// <param name="utotals"></param>
        /// <param name="recType"></param>
        /// <returns></returns>
        private DailyTotalsDetail SeekDailyTotalDetail(DailyTotals utotals, eDailyRecordTypes recType, Guid keyValue)
        {
            CriteriaOperator keyOperator;

            if (keyValue != Guid.Empty)
            {
                keyOperator = CriteriaOperator.Or(new BinaryOperator("DocumentType", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("VatFactor", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("Payment", keyValue, BinaryOperatorType.Equal));
            }
            else
            {
                keyOperator = null;
            }

            return SessionManager.FindObject<DailyTotalsDetail>(PersistentCriteriaEvaluationBehavior.InTransaction, CriteriaOperator.And(new BinaryOperator("DailyTotals", utotals, BinaryOperatorType.Equal),
            new BinaryOperator("DetailType", recType, BinaryOperatorType.Equal),
            keyOperator));
        }

        /// <summary>
        /// Ενημέρωση αθροιστή ημέρας
        /// </summary>
        /// <param name="userDetail"></param>
        /// <returns></returns>
        private DailyTotalsDetail CreateOrUpdateDailyTotalDetail(UserDailyTotalsDetail userDetail, eTotalizorAction action, decimal qtyValue, decimal amountValue, decimal vatAmount)
        {
            if (userDetail.UserDailyTotals.DailyTotals == null)
            {
                throw new Exception("DailyTotal does not EXIST");
            }
            if (userDetail == null)
            {
                return null;
            }

            Guid keyValue = userDetail.DocumentType;
            if (keyValue == Guid.Empty)
            {
                keyValue = userDetail.Payment;
            }
            if (keyValue == Guid.Empty)
            {
                keyValue = userDetail.VatFactor;
            }

            DailyTotalsDetail detail = SeekDailyTotalDetail(userDetail.UserDailyTotals.DailyTotals, userDetail.DetailType, keyValue);
            if (detail == null) // create new dailyTotalDetail
            {
                detail = new DailyTotalsDetail(userDetail.Session);
                detail.CreatedBy = userDetail.CreatedBy;
                detail.DailyTotals = userDetail.UserDailyTotals.DailyTotals;
                detail.DetailType = userDetail.DetailType;
                detail.VatFactor = userDetail.VatFactor;
                detail.DocumentType = userDetail.DocumentType;
                detail.Payment = userDetail.Payment;
                detail.CreatedByDevice = userDetail.UserDailyTotals.CreatedByDevice;
                detail.VatAmount = 0;
                detail.Amount = 0;
                detail.QtyValue = 0;
            }
            switch (action)
            {
                case eTotalizorAction.NONE:
                    return detail;
                case eTotalizorAction.INCREASE:
                    detail.QtyValue += qtyValue;
                    detail.Amount += amountValue;
                    detail.VatAmount += vatAmount;
                    break;
                case eTotalizorAction.DECREASE:
                    detail.QtyValue -= qtyValue;
                    detail.Amount -= amountValue;
                    detail.VatAmount -= vatAmount;
                    break;
                case eTotalizorAction.UPDATE:
                    //detail.QtyValue = qtyValue;
                    //detail.Amount = amountValue;
                    //detail.VatAmount = vatAmount;
                    ComputeDailyTotals(userDetail.UserDailyTotals.DailyTotals);
                    break;
                case eTotalizorAction.CLEAR:
                    detail.QtyValue = 0;
                    detail.Amount = 0;
                    detail.VatAmount = 0;
                    //ComputeDailyTotals(userDetail.UserDailyTotals.DailyTotals, true);
                    break;
            }
            SessionManager.FillDenormalizedFields(detail);
            return detail;
        }

        /// <summary>
        /// Επαναυπολογίζει τα σύνολα ημέρας
        /// </summary>
        /// <param name="dtotals"></param>
        /// <returns>DailyTotalsDetail</returns>
        private DailyTotals ComputeDailyTotals(DailyTotals dtotals, bool clear = false)
        {
            if (dtotals == null)
                throw new Exception("DailyTotals does not exist");
            if (!dtotals.FiscalDateOpen)
                throw new Exception("Daily Totals not opened");

            //dtotals.Session.Delete(dtotals.DailyTotalsDetails);

            foreach (DailyTotalsDetail dTotal in dtotals.DailyTotalsDetails)
            {
                dTotal.Amount = 0;
                dTotal.VatAmount = 0;
                dTotal.QtyValue = 0;
            }

            if (!clear)
            {
                foreach (UserDailyTotals userTotals in dtotals.UserDailyTotalss)
                {
                    foreach (UserDailyTotalsDetail userDetail in userTotals.UserDailyTotalsDetails)
                    {
                        DailyTotalsDetail dTotalDetail = CreateOrUpdateDailyTotalDetail(userDetail, eTotalizorAction.INCREASE, userDetail.QtyValue, userDetail.Amount, userDetail.VatAmount);
                        //if (dTotalDetail != null)
                        //    dtotals.DailyTotalsDetails.Add(dTotalDetail);
                    }
                }
            }
            return dtotals;
        }

        /// <summary>
        /// Αναζήτηση του UserDailyTotalDetail με βάση το RecordType και GUID από το έιδος του Record που ψάχνουμε )
        /// </summary>
        /// <param name="utotals"></param>
        /// <param name="recType"></param>
        /// <returns></returns>
        private UserDailyTotalsDetail SeekUserTotalDetail(UserDailyTotals utotals, eDailyRecordTypes recType, Guid keyValue)
        {
            CriteriaOperator keyOperator;

            if (recType == eDailyRecordTypes.DRAWERS || recType == eDailyRecordTypes.RETURNS ||
            recType == eDailyRecordTypes.LOYALTYPOINTS ||
            recType == eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL ||
            recType == eDailyRecordTypes.CANCELED_RETURNS ||
            recType == eDailyRecordTypes.DISCOUNTS ||
            recType == eDailyRecordTypes.ITEMS)
                keyOperator = null;
            else
                keyOperator = CriteriaOperator.Or(new BinaryOperator("DocumentType", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("VatFactor", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("Payment", keyValue, BinaryOperatorType.Equal));

            return SessionManager.FindObject<UserDailyTotalsDetail>(PersistentCriteriaEvaluationBehavior.InTransaction, CriteriaOperator.And(new BinaryOperator("UserDailyTotals", utotals, BinaryOperatorType.Equal),
            new BinaryOperator("DetailType", recType, BinaryOperatorType.Equal),
            keyOperator));
        }

        /// <summary>
        /// Κάνει ξανα αποφορολογοποίηση στο σύνολο του detail
        /// </summary>
        /// <param name="udt"></param>
        /// <param name="vatFactor"></param>
        /// <param name="currentUser"></param>
        /// <param name="currentDailyTotals"></param>
        /// <param name="currentTerminal"></param>
        /// <param name="currentStore"></param>
        private void RecalculateVatTotals(OwnerApplicationSettings appSettings, DocumentHeader header, UserDailyTotals udt, VatFactor vatFactor, Guid currentUser, Guid currentDailyTotals, Guid currentTerminal, Guid currentStore)
        {
            UserDailyTotalsDetail udtd = SeekUserTotalDetail(udt, eDailyRecordTypes.TAXRECORD, vatFactor.Oid);
            if (udtd == null)
                return;

            decimal vatAmountGrossTotal = udtd.Amount;
            decimal vatNetTotal = PlatformRoundingHandler.RoundDisplayValue(udtd.Amount / (decimal)(1 + vatFactor.Factor));
            decimal vatTotal = PlatformRoundingHandler.RoundDisplayValue(udtd.Amount - vatNetTotal);
            if (vatTotal != udtd.VatAmount)
            {
                int difference = vatTotal - udtd.VatAmount > 0 ? 1 : -1;
                CreateOrUpdateTotals(eTotalizorAction.UPDATE, currentUser, currentDailyTotals, udt.Oid, eDailyRecordTypes.TAXRECORD, vatFactor.Oid, currentTerminal,
                    currentStore, udtd.QtyValue, udtd.Amount, vatTotal);

                //Seek for DocumentDetails with same vatfactor
                //IEnumerable<DocumentDetail> possibleDetails = header.DocumentDetails.Where(g => g.VatFactorGuid == vatFactor.Oid && (g.TotalVatAmount - g.GrossTotal + g.GrossTotal / (1 + g.VatFactor)) * difference > 0);
                //int r = 0;
            }
        }

        /// <summary>
        /// Ενημερώνει τους αθροιστές
        /// </summary>
        /// <param name="userTotals"></param>
        /// <param name="rectype"></param>
        /// <param name="action"></param>
        /// <param name="keyValue"></param>
        /// <param name="qtyValue"></param>
        /// <param name="amountValue"></param>
        /// <returns></returns>
        private UserDailyTotals CreateOrUpdateTotals(eTotalizorAction action, Guid currentUser, Guid currentDailyTotals, Guid currentUserDailyTotals, eDailyRecordTypes rectype, Guid keyValue, Guid currentTerminal, Guid currentStore, decimal qtyValue = 0, decimal amountValue = 0, decimal vatAmount = 0, bool skipDailyTotals = false, bool skipUserDailyTotals = false)
        {
            UserDailyTotals usrTotals = SessionManager.GetObjectByKey<UserDailyTotals>(currentUserDailyTotals);
            if (usrTotals == null)
            {
                if (currentUserDailyTotals == null)
                    throw new Exception("User not Logged In");
                // Create new UserTotals
                usrTotals = CreateUserDailtyTotals(currentUser, currentDailyTotals, currentUserDailyTotals, currentTerminal, currentStore);
            }
            UserDailyTotalsDetail uddetail = SeekUserTotalDetail(usrTotals, rectype, keyValue);
            try
            {
                if (uddetail == null)
                {
                    uddetail = new UserDailyTotalsDetail(SessionManager.GetSession<UserDailyTotalsDetail>());
                    uddetail.UserDailyTotals = usrTotals;
                    uddetail.CreatedByDevice = usrTotals.CreatedByDevice;
                    uddetail.CreatedBy = currentUser;
                    uddetail.DetailType = rectype;
                    // Update Key Value
                    switch (rectype)
                    {
                        case eDailyRecordTypes.INVOICES:
                            // Μέτρημα πραστατικών
                            uddetail.VatFactor = Guid.Empty;
                            uddetail.Payment = Guid.Empty;
                            uddetail.DocumentType = keyValue;
                            break;
                        case eDailyRecordTypes.PAYMENTS:
                            // τρόπων πληρωμών
                            uddetail.VatFactor = Guid.Empty;
                            uddetail.Payment = keyValue;
                            uddetail.DocumentType = Guid.Empty;
                            break;
                        case eDailyRecordTypes.TAXRECORD:
                            //
                            uddetail.VatFactor = keyValue;
                            uddetail.Payment = Guid.Empty;
                            uddetail.DocumentType = Guid.Empty;
                            break;
                        default:
                            // 
                            uddetail.VatFactor = Guid.Empty;
                            uddetail.Payment = Guid.Empty;
                            uddetail.DocumentType = Guid.Empty;
                            break;
                    }
                }

                if (skipUserDailyTotals == false)
                {
                    // Update values 
                    if (action == eTotalizorAction.NONE)
                        return usrTotals;

                    switch (action)
                    {
                        case eTotalizorAction.INCREASE:
                            uddetail.QtyValue += qtyValue;
                            uddetail.Amount += amountValue;
                            uddetail.VatAmount += vatAmount;
                            break;
                        case eTotalizorAction.DECREASE:
                            uddetail.QtyValue -= qtyValue;
                            uddetail.Amount -= amountValue;
                            uddetail.VatAmount -= vatAmount;
                            break;
                        case eTotalizorAction.UPDATE:
                            uddetail.QtyValue = qtyValue;
                            uddetail.Amount = amountValue;
                            uddetail.VatAmount = vatAmount;
                            break;
                        case eTotalizorAction.CLEAR:
                            uddetail.QtyValue = 0;
                            uddetail.Amount = 0;
                            uddetail.VatAmount = 0;
                            break;
                    }
                    SessionManager.FillDenormalizedFields(uddetail);

                    uddetail.Save();
                }
                if (skipDailyTotals == false)
                {
                    CreateOrUpdateDailyTotalDetail(uddetail, action, qtyValue, amountValue, vatAmount).Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("UpdateUserTotalDetail ERROR : {0}", ex.GetFullMessage()));
            }
            return usrTotals;
        }

        /// <summary>
        /// Generates an XReport object used for printing
        /// </summary>
        /// <param name="udt"></param>
        public Report CreateXReport(OwnerApplicationSettings appSettings, DailyTotals dt, UserDailyTotals udt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType, bool asksForFinalAmount)
        {
            return CreateReport(appSettings, dt, udt, udt.UserDailyTotalsDetails, terminalDescription, ReceiptInvoiceType, ProformaInvoiceType, DefaultCustomerVatlevelOid, DepositInvoiceType, WithdrawInvoiceType, 0, udt.User, asksForFinalAmount);
        }

        /// <summary>
        /// Generates an Report object used for printing
        /// </summary>
        /// <param name="udt"></param>
        public Report CreateZReport(OwnerApplicationSettings appSettings, DailyTotals dt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType, bool asksForFinalAmount)
        {
            return CreateReport(appSettings, dt, null, dt.DailyTotalsDetails, terminalDescription, ReceiptInvoiceType, ProformaInvoiceType, DefaultCustomerVatlevelOid, DepositInvoiceType, WithdrawInvoiceType, dt.ZReportNumber, Guid.Empty, asksForFinalAmount);
        }

        private Report CreateReport<T>(OwnerApplicationSettings appSettings, DailyTotals dailyTotal, UserDailyTotals currentUserDailyTotal, XPCollection<T> totalDetails, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType,
            Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType, int ReportNumber, Guid user, bool asksForFinalAmount) where T : TotalDetail
        {
            User userObj = SessionManager.GetObjectByKey<User>(user);

            Report report = new Report()
            {
                TerminalDescription = terminalDescription,
                DateIssued = DateTime.Now,
                DrawersOpenOccured = 0,
                VatFactor1 = " ",
                VatFactor2 = " ",
                VatFactor3 = " ",
                VatFactor4 = " ",
                VatFactor5 = " ",
                PaymentMethod1 = " ",
                PaymentMethod2 = " ",
                PaymentMethod3 = " ",
                PaymentMethod4 = " ",
                PaymentMethod5 = " ",
                ReportNumber = ReportNumber,
                UserName = (userObj == null ? "" : userObj.POSUserName)
            };
            using (XPCollection<VatFactor> vcs = new XPCollection<VatFactor>(SessionManager.GetSession<VatFactor>(), new BinaryOperator("VatLevel", DefaultCustomerVatlevelOid), new SortProperty("Factor", DevExpress.Xpo.DB.SortingDirection.Ascending)))
            {
                foreach (VatFactor vf in vcs)
                {
                    VatCategory vc = SessionManager.GetObjectByKey<VatCategory>(vf.VatCategory);
                    if (vc != null)
                    {
                        switch (vc.MinistryVatCategoryCode)
                        {
                            case eMinistryVatCategoryCode.A:
                                report.VatFactor1 = vf.Factor.ToString("P2");
                                break;
                            case eMinistryVatCategoryCode.B:
                                report.VatFactor2 = vf.Factor.ToString("P2");
                                break;
                            case eMinistryVatCategoryCode.C:
                                report.VatFactor3 = vf.Factor.ToString("P2");
                                break;
                            case eMinistryVatCategoryCode.D:
                                report.VatFactor4 = vf.Factor.ToString("P2");
                                break;
                            case eMinistryVatCategoryCode.E:
                                report.VatFactor5 = vf.Factor.ToString("P2");
                                break;
                        }
                    }
                }

            }
            report.PaymentMethods = new List<ReportPaymentMethod>();
            List<PaymentMethod> paymentMethods;
            using (XPCollection<PaymentMethod> paymentMethodsFromDB = new XPCollection<PaymentMethod>(SessionManager.GetSession<PaymentMethod>(), null, new SortProperty("IncreasesDrawerAmount", DevExpress.Xpo.DB.SortingDirection.Ascending)))
            {
                paymentMethods = paymentMethodsFromDB.OrderBy(x => x.Code).Take(5).ToList();
                report.PaymentMethods.AddRange(paymentMethodsFromDB.Select(paymentMethod => new ReportPaymentMethod()
                {
                    PaymentMethodOid = paymentMethod.Oid,
                    PaymentMethodName = paymentMethod.Description == null ? "" : paymentMethod.Description.ToUpperGR(),
                    PaymentMethodCode = paymentMethod.Code,
                    IncreasesDrawerAmount = paymentMethod.IncreasesDrawerAmount
                }));
            }
            try
            {
                report.PaymentMethod1 = paymentMethods[0].Description == null ? "" : paymentMethods[0].Description.ToUpperGR();
                report.PaymentMethod1Guid = paymentMethods[0].Oid;
                report.PaymentMethod2 = paymentMethods[1].Description == null ? "" : paymentMethods[1].Description.ToUpperGR();
                report.PaymentMethod2Guid = paymentMethods[1].Oid;
                report.PaymentMethod3 = paymentMethods[2].Description == null ? "" : paymentMethods[2].Description.ToUpperGR();
                report.PaymentMethod3Guid = paymentMethods[2].Oid;
                report.PaymentMethod4 = paymentMethods[3].Description == null ? "" : paymentMethods[3].Description.ToUpperGR();
                report.PaymentMethod4Guid = paymentMethods[3].Oid;
                report.PaymentMethod5 = paymentMethods[4].Description == null ? "" : paymentMethods[4].Description.ToUpperGR();
                report.PaymentMethod5Guid = paymentMethods[4].Oid;
            }
            catch
            {

            }

            foreach (TotalDetail detail in totalDetails)
            {
                switch (detail.DetailType)
                {
                    case eDailyRecordTypes.CANCELED_DOCUMENT:
                        report.CanceledDocumentsQty += (int)detail.QtyValue;
                        report.CanceledDocumentsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL:
                        report.CanceledQty += (int)detail.QtyValue;
                        report.CanceledAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.CANCELED_RETURNS:
                        report.CanceledReturnsQty += (int)detail.QtyValue;
                        report.CanceledReturnsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.DISCOUNTS:
                        report.DiscountsQty += (int)detail.QtyValue;
                        report.DiscountsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.DOCUMENT_DISCOUNTS:
                        report.DocumentDiscountsQty += (int)detail.QtyValue;
                        report.DocumentDiscountsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.COUPONS:
                        report.CouponsQty += (int)detail.QtyValue;
                        report.CouponsAmmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.DRAWERS:
                        report.DrawersOpenOccured = (int)detail.QtyValue;//??? //= (int)detail.QtyValue;
                        //report.DrawersAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.INVOICES:
                        if (detail.DocumentType == ProformaInvoiceType)
                        {
                            report.InvoicesQty += (int)detail.QtyValue;
                            report.InvoicesAmount += detail.Amount;
                        }
                        else if (detail.DocumentType == ReceiptInvoiceType && detail.DocumentType != ProformaInvoiceType && detail.DocumentType != WithdrawInvoiceType && detail.DocumentType != DepositInvoiceType)
                        {
                            report.ReceiptsQty += (int)detail.QtyValue;
                            report.ReceiptsAmount += detail.Amount;
                        }
                        else if (detail.DocumentType == WithdrawInvoiceType)
                        {
                            report.WithdrawsQty += (int)detail.QtyValue;
                            report.WithdrawsAmount += detail.Amount;
                            report.WithdrawsNegativeAmount -= detail.Amount;
                        }
                        else if (detail.DocumentType == DepositInvoiceType)
                        {
                            report.DepositsQty += (int)detail.QtyValue;
                            report.DepositsAmount += detail.Amount;
                        }
                        else
                        {
                            report.OtherDocumentsQty += (int)detail.QtyValue;
                            report.OtherDocumentsAmount += detail.Amount;
                        }
                        break;
                    case eDailyRecordTypes.LOYALTYPOINTS:
                        report.LoyaltyPointsQty += detail.QtyValue;
                        report.LoyaltyPointsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.TAXRECORD:
                        VatFactor vatFactor = SessionManager.GetObjectByKey<VatFactor>(detail.VatFactor);
                        if (vatFactor == null)
                        {
                            continue;
                        }

                        VatCategory vatCategory = SessionManager.GetObjectByKey<VatCategory>(vatFactor.VatCategory);

                        if (vatCategory == null)
                        {
                            continue;
                        }

                        switch (vatCategory.MinistryVatCategoryCode)
                        {
                            case eMinistryVatCategoryCode.A:
                                report.GrossAmount1 += detail.Amount;
                                report.VatAmount1 += detail.VatAmount;
                                report.NetAmount1 += PlatformRoundingHandler.RoundDisplayValue(report.GrossAmount1 - detail.VatAmount);
                                break;
                            case eMinistryVatCategoryCode.B:
                                report.GrossAmount2 += detail.Amount;
                                report.VatAmount2 += detail.VatAmount;
                                report.NetAmount2 += PlatformRoundingHandler.RoundDisplayValue(report.GrossAmount2 - detail.VatAmount);
                                break;
                            case eMinistryVatCategoryCode.C:
                                report.GrossAmount3 += detail.Amount;
                                report.VatAmount3 += detail.VatAmount;
                                report.NetAmount3 += PlatformRoundingHandler.RoundDisplayValue(report.GrossAmount3 - detail.VatAmount);
                                break;
                            case eMinistryVatCategoryCode.D:
                                report.GrossAmount4 += detail.Amount;
                                report.VatAmount4 += detail.VatAmount;
                                report.NetAmount4 += PlatformRoundingHandler.RoundDisplayValue(report.GrossAmount4 - detail.VatAmount);
                                break;
                            case eMinistryVatCategoryCode.E:
                                report.GrossAmount5 += detail.Amount;
                                report.VatAmount5 += detail.VatAmount;
                                report.NetAmount5 += PlatformRoundingHandler.RoundDisplayValue(report.GrossAmount5 - detail.VatAmount);
                                break;
                        }

                        break;
                    case eDailyRecordTypes.ITEMS:
                        report.ItemsQty += detail.QtyValue;
                        report.ItemsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.PAYMENTS:
                        if (detail.Payment != Guid.Empty)
                        {
                            if (report.PaymentMethod1Guid == detail.Payment)
                            {
                                report.PaymentMethodAmount1 += detail.Amount;
                                report.PaymentMethodQty1 += detail.QtyValue;
                            }
                            else if (report.PaymentMethod2Guid == detail.Payment)
                            {
                                report.PaymentMethodAmount2 += detail.Amount;
                                report.PaymentMethodQty2 += detail.QtyValue;
                            }
                            else if (report.PaymentMethod3Guid == detail.Payment)
                            {
                                report.PaymentMethodAmount3 += detail.Amount;
                                report.PaymentMethodQty3 += detail.QtyValue;
                            }
                            else if (report.PaymentMethod4Guid == detail.Payment)
                            {
                                report.PaymentMethodAmount4 += detail.Amount;
                                report.PaymentMethodQty4 += detail.QtyValue;
                            }
                            else if (report.PaymentMethod5Guid == detail.Payment)
                            {
                                report.PaymentMethodAmount5 += detail.Amount;
                                report.PaymentMethodQty5 += detail.QtyValue;
                            }

                            ReportPaymentMethod paymentMethod = report.PaymentMethods.FirstOrDefault(method => method.PaymentMethodOid == detail.Payment);
                            if (paymentMethod != null)
                            {
                                paymentMethod.Qty += (int)detail.QtyValue;
                                paymentMethod.NegativeAmount -= detail.Amount;
                                paymentMethod.Amount += detail.Amount;
                            }
                        }
                        break;
                    case eDailyRecordTypes.CASH:
                        if (detail is UserDailyTotalsDetail)
                        {
                            report.CashierCashAmount += detail.Amount;
                        }
                        else
                        {
                            report.CashAmount += detail.Amount;
                        }
                        break;
                    case eDailyRecordTypes.RETURNS:
                        report.SwitchesReturnsQty += detail.QtyValue;
                        report.SwitchesReturnsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.STARTING_AMOUNT:
                        if (detail is UserDailyTotalsDetail)
                        {
                            report.StartingCashAmount = detail.Amount;
                        }
                        break;
                    case eDailyRecordTypes.CANCELLED_EDPS_PAYMENTS:
                        report.CanceledEDPSPayments += detail.QtyValue;
                        report.CanceledEDPSPaymentsAmount += detail.Amount;
                        break;
                    case eDailyRecordTypes.CANCELLED_CARDLINK_PAYMENTS:
                        report.CanceledCardLinkPayments += detail.QtyValue;
                        report.CanceledCardLinkPaymentsAmount += detail.Amount;
                        break;
                }
            }

            if (typeof(T) == typeof(UserDailyTotalsDetail))
            {
                report.CashAmount = this.GetTotalCashInPos(dailyTotal);
            }

            if (currentUserDailyTotal != null)
            {
                report.UserCashFinalAmount = currentUserDailyTotal.UserCashFinalAmount;
                if (asksForFinalAmount)
                {
                    report.CashAmountDifference = report.UserCashFinalAmount - report.CashAmount;
                }
            }

            report.TotalAmount = report.NetAmount1 + report.NetAmount2 + report.NetAmount3 + report.NetAmount4 + report.NetAmount5;
            report.TotalVatAmount = report.VatAmount1 + report.VatAmount2 + report.VatAmount3 + report.VatAmount4 + report.VatAmount5;

            if (typeof(T) != typeof(UserDailyTotalsDetail))
            {
                report.StartingCashAmount = this.GetDayStartingAmount(dailyTotal);
            }


            return report;
        }

        internal class VatFactorQtyAmount
        {
            public VatFactor VatFactor;
            public decimal Qty;
            public decimal Amount;
            public decimal Factor;
            public decimal NetAmount;
            public decimal VatAmount;
            public eMinistryVatCategoryCode VatCategoryCode;

        }

        public void UpdateTotalizers(IConfigurationManager config, DocumentHeader header, User currentUser, DailyTotals currentDailyTotals,
            UserDailyTotals currentUserDaily, Guid currentStoreOid, Guid currentPOSOid, bool increaseVatAndSales,
            bool totalVatCalculationPerReceipt, eTotalizorAction totalizorAction, bool isFiscalPrinter = false, decimal vatAGrossTotal = 0,
            decimal vatBGrossTotal = 0, decimal vatCGrossTotal = 0, decimal vatDGrossTotal = 0,
            decimal vatEGrossTotal = 0, decimal grossTotal = 0) //fiscal printer parameters
        {
            decimal totalItemsCount = 0;
            decimal totalItemsAmount = 0;
            header.DocumentDetails.Filter = null;

            List<VatFactorQtyAmount> vatFactorItemsCount = new List<VatFactorQtyAmount>();

            List<Tuple<Guid, decimal>> documentPaymentMethodAmounts = new List<Tuple<Guid, decimal>>();

            int canceledCount = 0;
            decimal canceledAmount = 0;

            int canceledReturnsCount = 0;
            decimal canceledReturnsAmount = 0;

            int canceledDocumentsCount = 0;
            decimal canceledDocumentsAmmount = 0;

            int loyaltyCount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.Points != 0).Count();
            decimal loyaltyAmount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.Points != 0).Sum(docDetail => docDetail.Points);

            //int discountsCount = header.DocumentDetails.Where(x => x.TotalDiscount !=0).Count();      ///????? tbc  
            //Μάλλον λάθος.Λογικά θα θέλεις τις εκτπώσεις των μη  ακυρωμένων γραμμών
            ///
            int discountsCount = header.IsCanceled ? 0 : header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalNonDocumentDiscount != 0).Count();
            decimal discountsAmount = header.IsCanceled ? 0 : header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalNonDocumentDiscount != 0).Sum(g => g.TotalDiscountIncludingVAT);///????

            int drawersCount = 0;        //???
            decimal drawersAmount = 0;    //???

            int couponsCount = 0;        //???  έχουμε κουπόνια είδους και κοπόνια εκπτωτικά!!!
            decimal couponsAmount = 0;    //???

            int itemsIncreasedCount = 0;        //???  δεν ξέρουμε ακόμη τι είναι
            decimal itemsIncreasedAmount = 0;    //???

            int documentsIncreasedCount = 0;        //???  δεν ξέρουμε ακόμη τι είναι
            decimal documentsIncreasedAmount = 0;    //???

            decimal switchesReturnsCount = 0;        //???  //TOCHECK
            decimal switchesReturnsAmount = 0;    //???

            int invoicesCount = header.IsCanceled ? 0 : 1;
            decimal invoicesAmount = header.IsCanceled ? 0 : header.GrossTotal;

            if (header.IsCanceled == true)
            {
                canceledDocumentsCount++;
                canceledDocumentsAmmount += header.GrossTotalBeforeDiscount; //???
            }
            else
            {
                if (increaseVatAndSales)
                {
                    foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
                    {

                        if (detail.Qty >= 0)
                        {
                            totalItemsCount += detail.Qty;
                            totalItemsAmount += detail.GrossTotal;
                        }
                        else
                        {
                            switchesReturnsAmount -= detail.GrossTotal;
                            switchesReturnsCount -= detail.Qty;
                        }

                        VatFactor itemVatFac = SessionManager.GetObjectByKey<VatFactor>(detail.VatFactorGuid);
                        VatFactorQtyAmount vatFactorCount = vatFactorItemsCount.Where(x => x.VatFactor == itemVatFac).FirstOrDefault();
                        if (vatFactorCount == null)
                        {
                            vatFactorCount = new VatFactorQtyAmount();
                            vatFactorCount.VatFactor = itemVatFac;
                            vatFactorCount.Factor = detail.VatFactor;
                            vatFactorCount.VatCategoryCode = detail.ItemVatCategoryMinistryCode;
                            vatFactorItemsCount.Add(vatFactorCount);
                        }
                        vatFactorCount.Amount += detail.GrossTotal;
                        vatFactorCount.NetAmount += detail.NetTotal;
                        vatFactorCount.VatAmount += detail.TotalVatAmount;
                        vatFactorCount.Qty += detail.Qty;
                    }
                }

                canceledCount += header.DocumentDetails.Where(x => x.IsCanceled == true && x.IsReturn == false).Count();
                canceledAmount += header.DocumentDetails.Where(x => x.IsCanceled == true && x.IsReturn == false).Sum(g => g.GrossTotal);

                canceledReturnsCount += header.DocumentDetails.Where(x => x.IsCanceled == true && x.IsReturn == true).Count();
                canceledReturnsAmount += header.DocumentDetails.Where(x => x.IsCanceled == true && x.IsReturn == true).Sum(g => g.GrossTotal);



                foreach (DocumentPayment documentPayment in header.DocumentPayments)
                {
                    Tuple<Guid, decimal> paymentMethodAmountTuple = documentPaymentMethodAmounts.Where(x => x.Key1 == documentPayment.PaymentMethod).FirstOrDefault();
                    if (paymentMethodAmountTuple == null)
                    {
                        paymentMethodAmountTuple = new Tuple<Guid, decimal>();
                        paymentMethodAmountTuple.Key1 = documentPayment.PaymentMethod;
                        documentPaymentMethodAmounts.Add(paymentMethodAmountTuple);
                    }
                    paymentMethodAmountTuple.Key2 += documentPayment.Amount;
                }
            }

            //Ενημέρωση των αθροιστών των ειδών, eDailyRecordTypes.ITEMS
            decimal itemsValue = (isFiscalPrinter && grossTotal != 0) ? grossTotal : totalItemsAmount;
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.ITEMS, Guid.Empty, currentPOSOid, currentStoreOid, totalItemsCount, itemsValue).Save();

            OwnerApplicationSettings appSettings = config.GetAppSettings();

            //Ενημέρωση των αθροιστών κατηγοριών ΦΠΑ, eDailyRecordTypes.TAXRECORD
            foreach (VatFactorQtyAmount vatTuple in vatFactorItemsCount)
            {
                if (isFiscalPrinter)
                {
                    decimal vatAmount = 0;
                    decimal amount = 0;
                    decimal netAmount = 0;
                    switch (vatTuple.VatCategoryCode)
                    {
                        case eMinistryVatCategoryCode.A:
                            amount = vatAGrossTotal;
                            netAmount = PlatformRoundingHandler.RoundDisplayValue((vatAGrossTotal) / (decimal)(1 + vatTuple.Factor));
                            vatAmount = PlatformRoundingHandler.RoundDisplayValue(vatAGrossTotal - netAmount);
                            break;
                        case eMinistryVatCategoryCode.B:
                            amount = vatBGrossTotal;
                            netAmount = PlatformRoundingHandler.RoundDisplayValue((vatBGrossTotal) / (decimal)(1 + vatTuple.Factor));
                            vatAmount = PlatformRoundingHandler.RoundDisplayValue(vatBGrossTotal - netAmount);
                            break;
                        case eMinistryVatCategoryCode.C:
                            amount = vatCGrossTotal;
                            netAmount = PlatformRoundingHandler.RoundDisplayValue((vatCGrossTotal) / (decimal)(1 + vatTuple.Factor));
                            vatAmount = PlatformRoundingHandler.RoundDisplayValue(vatCGrossTotal - netAmount);
                            break;
                        case eMinistryVatCategoryCode.D:
                            amount = vatDGrossTotal;
                            netAmount = PlatformRoundingHandler.RoundDisplayValue((vatDGrossTotal) / (decimal)(1 + vatTuple.Factor));
                            vatAmount = PlatformRoundingHandler.RoundDisplayValue(vatDGrossTotal - netAmount);
                            break;
                        case eMinistryVatCategoryCode.E:
                            amount = vatEGrossTotal;
                            netAmount = PlatformRoundingHandler.RoundDisplayValue((vatEGrossTotal) / (1 + vatTuple.Factor));
                            vatAmount = PlatformRoundingHandler.RoundDisplayValue(vatEGrossTotal - netAmount);
                            break;
                    }

                    this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                                eDailyRecordTypes.TAXRECORD, vatTuple.VatFactor.Oid, currentPOSOid, currentStoreOid, vatTuple.Qty, amount, vatAmount).Save();
                }
                else
                {
                    this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                        eDailyRecordTypes.TAXRECORD, vatTuple.VatFactor.Oid, currentPOSOid, currentStoreOid, vatTuple.Qty, vatTuple.Amount, vatTuple.VatAmount
                        ).Save();
                }
            }

            //Ενημέρωση των αθροιστών των πληρωμών, eDailyRecordTypes.PAYMENTS
            if (header.DocumentType != config.WithdrawalDocumentTypeOid && header.DocumentType != config.DepositDocumentTypeOid)
            {
                foreach (Tuple<Guid, decimal> paymentTuple in documentPaymentMethodAmounts)
                {
                    this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                        eDailyRecordTypes.PAYMENTS, paymentTuple.Key1, currentPOSOid, currentStoreOid, (paymentTuple.Key2 > 0 ? 1 : 0), paymentTuple.Key2).Save();
                }
            }

            //Ενημέρωση των αθροιστών των πόντων eDailyRecordTypes.LOYALTYPOINTS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.LOYALTYPOINTS, Guid.Empty, currentPOSOid, currentStoreOid, loyaltyCount, loyaltyAmount).Save();

            //Ενημέρωση των αθροιστών των ακυρομένων eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.CANCELED_DOCUMENT, Guid.Empty, currentPOSOid, currentStoreOid, canceledDocumentsCount, canceledDocumentsAmmount).Save();

            //Ενημέρωση των αθροιστών των ακυρομένων eDailyRecordTypes.CANCELED_DOCUMENT
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL, Guid.Empty, currentPOSOid, currentStoreOid, canceledCount, canceledAmount).Save();

            //Ενημέρωση των αθροιστών των ακυρομένων eDailyRecordTypes.CANCELED_DOCUMENT
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.CANCELED_RETURNS, Guid.Empty, currentPOSOid, currentStoreOid, canceledReturnsCount, canceledReturnsAmount).Save();

            //Ενημέρωση των αθροιστών των εκπτώσεων eDailyRecordTypes.DISCOUNTS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.DISCOUNTS, Guid.Empty, currentPOSOid, currentStoreOid, discountsCount, discountsAmount).Save();

            //Ενημέρωση των αθροιστών των εκπτώσεων eDailyRecordTypes.DOCUMENT_DISCOUNTS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.DOCUMENT_DISCOUNTS, Guid.Empty, currentPOSOid, currentStoreOid, (header.AllDocumentHeaderDiscounts > 0) ? 1 : 0, header.AllDocumentHeaderDiscounts).Save();

            //Ενημέρωση των αθροιστών των κουπονιών eDailyRecordTypes.COUPONS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.COUPONS, Guid.Empty, currentPOSOid, currentStoreOid, couponsCount, couponsAmount).Save();

            //Ενημέρωση των αθροιστών των αναλήψεων eDailyRecordTypes.DRAWERS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.DRAWERS, Guid.Empty, currentPOSOid, currentStoreOid, drawersCount, drawersAmount).Save();

            if (header.IsDayStartingAmount && header.DocumentType == config.DepositDocumentTypeOid)
            {
                //Ενημέρωση του παγίου της Ημέρας
                this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.STARTING_AMOUNT, header.DocumentType, currentPOSOid, currentStoreOid, invoicesCount, invoicesAmount).Save();
            }
            else if (header.IsShiftStartingAmount &&
                (header.DocumentType == config.DepositDocumentTypeOid || header.DocumentType == config.WithdrawalDocumentTypeOid))
            {
                //Ενημέρωση του παγίου της βάρδιας
                decimal startingCash = GetTotalCashInPos(currentDailyTotals);
                if (header.DocumentType == config.DepositDocumentTypeOid)
                {
                    startingCash += invoicesAmount;
                }
                else
                {
                    startingCash -= invoicesAmount;
                }

                this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.STARTING_AMOUNT, header.DocumentType, currentPOSOid, currentStoreOid, invoicesCount, startingCash, skipDailyTotals: true).Save();

                this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.INVOICES, header.DocumentType, currentPOSOid, currentStoreOid, invoicesCount, invoicesAmount, skipUserDailyTotals: true).Save();
            }
            else
            {
                //Ενημέρωση των αθροιστών των προτιμολογίων/αποδείξεων/αναλήψεων/καταθέσων eDailyRecordTypes.INVOICES
                this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.INVOICES, header.DocumentType, currentPOSOid, currentStoreOid, invoicesCount, invoicesAmount).Save();
            }

            //Ενημέρωση των αθροιστών των επιστροφών eDailyRecordTypes.RETURNS
            this.CreateOrUpdateTotals(totalizorAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                eDailyRecordTypes.RETURNS, Guid.Empty, currentPOSOid, currentStoreOid, switchesReturnsCount, switchesReturnsAmount).Save();

            if (header.IsCanceled == false)
            {
                DocumentType dtype = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
                IEnumerable<DocumentPayment> cashPayments = header.DocumentPayments.Where(payment => payment.IncreasesDrawerAmount);
                if (cashPayments.Count() > 0)
                {
                    decimal cashSum = cashPayments.Sum(payment => payment.Amount);
                    eTotalizorAction cashAction = totalizorAction;
                    if (totalizorAction == eTotalizorAction.INCREASE)
                    {
                        cashAction = dtype.ValueFactor >= 0 ? eTotalizorAction.INCREASE : eTotalizorAction.DECREASE;
                    }
                    else if (totalizorAction == eTotalizorAction.DECREASE)
                    {
                        cashAction = dtype.ValueFactor >= 0 ? eTotalizorAction.DECREASE : eTotalizorAction.INCREASE;
                    }

                    this.CreateOrUpdateTotals(cashAction, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                        eDailyRecordTypes.CASH, Guid.Empty, currentPOSOid, currentStoreOid, 0, cashSum).Save();
                }
            }

            //EDPS Cancels
            IEnumerable<DocumentPaymentEdps> canceledEdpsPayments = header.DocumentPaymentsEdps.Where(x => x.DocumentPayment == Guid.Empty).GroupBy(x => x.ReceiptNumber).Select(x => x.First());
            if (canceledEdpsPayments.Count() > 0)
            {
                this.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.CANCELLED_EDPS_PAYMENTS, Guid.Empty, currentPOSOid, currentStoreOid, canceledEdpsPayments.Count(), canceledEdpsPayments.Sum(x => x.Amount));
            }

            //CARDlINK CANCELS 
            IEnumerable<DocumentPaymentCardlink> canceledCardLinkPayments = header.DocumentPaymentsCardlink.Where(x => x.DocumentPayment == Guid.Empty).GroupBy(x => x.DocumentHeader).Select(x => x.First());
            if (canceledCardLinkPayments.Count() > 0)
            {
                this.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
                    eDailyRecordTypes.CANCELLED_CARDLINK_PAYMENTS, Guid.Empty, currentPOSOid, currentStoreOid, canceledCardLinkPayments.Count(), canceledCardLinkPayments.Sum(x => x.Amount));
            }


            SessionManager.CommitTransactionsChanges();

            if (!totalVatCalculationPerReceipt)
            {
                foreach (VatFactorQtyAmount vatTuple in vatFactorItemsCount)
                {
                    RecalculateVatTotals(appSettings, header, currentUserDaily, vatTuple.VatFactor, currentUser.Oid, currentDailyTotals.Oid, currentPOSOid, currentStoreOid);
                }
            }


            header.IsAddedToTotals = true;
            header.Save();
            SessionManager.CommitTransactionsChanges();

        }

        public void SetVatTotalsFromFiscalPrinter(DailyTotals currentDailyTotals, decimal fiscalVatAmountA, decimal fiscalVatAmountB, decimal fiscalVatAmountC, decimal fiscalVatAmountD, decimal fiscalVatAmountE)
        {
            IEnumerable<DailyTotalsDetail> taxDetails = currentDailyTotals.DailyTotalsDetails.Where(detail => detail.DetailType == eDailyRecordTypes.TAXRECORD);
            foreach (DailyTotalsDetail taxDetail in taxDetails)
            {
                VatFactor factor = SessionManager.GetObjectByKey<VatFactor>(taxDetail.VatFactor);
                VatCategory category = SessionManager.GetObjectByKey<VatCategory>(factor == null ? Guid.Empty : factor.VatCategory);
                if (category != null)
                {
                    switch (category.MinistryVatCategoryCode)
                    {
                        case eMinistryVatCategoryCode.A:
                            taxDetail.VatAmount = fiscalVatAmountA;
                            break;
                        case eMinistryVatCategoryCode.B:
                            taxDetail.VatAmount = fiscalVatAmountB;
                            break;
                        case eMinistryVatCategoryCode.C:
                            taxDetail.VatAmount = fiscalVatAmountC;
                            break;
                        case eMinistryVatCategoryCode.D:
                            taxDetail.VatAmount = fiscalVatAmountD;
                            break;
                        case eMinistryVatCategoryCode.E:
                            taxDetail.VatAmount = fiscalVatAmountE;
                            break;
                    }

                }
                taxDetail.Save();
            }
            currentDailyTotals.Session.CommitTransaction();
        }

        public void FixDocumentVatDeviations(IDocumentService documentService, IConfigurationManager config, DailyTotals currentDailyTotals, VatFactor vatFactor, OwnerApplicationSettings ownerApplicationSettings)
        {
            DailyTotalsDetail detail = SeekDailyTotalDetail(currentDailyTotals, eDailyRecordTypes.TAXRECORD, vatFactor.Oid);
            if (detail == null)
            {
                return;
            }

            decimal totalDetailGrossTotal = detail.Amount;
            decimal totalDetailVatTotal = detail.VatAmount;

            List<Guid> oids = currentDailyTotals.UserDailyTotalss.Select(x => x.Oid).ToList();


            List<Guid> posDocumentOids = documentService.GetAllValidPosDocumentTypes(config, SessionManager).Where(
             x => x.Oid != config.ProFormaInvoiceDocumentTypeOid &&
              x.Oid != config.SpecialProformaDocumentTypeOid &&
              x.Oid != config.WithdrawalDocumentTypeOid &&
              x.Oid != config.DepositDocumentTypeOid &&
              x.IncreaseVatAndSales == true &&
              x.JoinInTotalizers == true
              ).Select(z => z.Oid).ToList();



            // DocumentType defaultType = SessionManager.GetObjectByKey<DocumentType>(config.DefaultDocumentTypeOid);
            OwnerApplicationSettings owner = config.GetAppSettings();
            // bool isDefaultTypeWholeSale = defaultType.IsForWholesale;

            XPCollection<DocumentHeader> allZheaders = new XPCollection<DocumentHeader>(SessionManager.GetSession<DocumentHeader>(),
                          CriteriaOperator.And(new InOperator("UserDailyTotals", oids), new BinaryOperator("IsCanceled", false), new InOperator("DocumentType", posDocumentOids)));

            decimal headersVatTotalAmount = allZheaders.Select(x => x.DocumentDetails.Where(y => y.VatFactorGuid != null && y.VatFactorGuid == vatFactor.Oid && y.IsCanceled == false).Sum(z => z.TotalVatAmount)).Sum();

            decimal deviation = (decimal)PlatformRoundingHandler.RoundDisplayValue(totalDetailVatTotal - headersVatTotalAmount);
            int decimals = (int)ownerApplicationSettings.DisplayDigits;

            decimal deviationStep = (decimal)(1.0 / Math.Pow(10, decimals));

            int order = deviation > 0 ? 1 : -1;
            deviation *= order;

            int i = 0;
            List<DocumentDetail> details = allZheaders.SelectMany(x => x.DocumentDetails).Where(x => x.VatFactorGuid != null && x.VatFactorGuid == vatFactor.Oid && x.IsCanceled ==
                    false && x.IsReturn == false && x.IsTax == false &&
                    (-x.TotalVatAmount + x.GrossTotal - x.GrossTotal / (1 + x.VatFactor)) * order > 0).ToList();
            List<DocumentDetail> fixedDetails = new List<DocumentDetail>();

            while (deviation >= deviationStep && i < details.Count)
            {

                details[i].TotalVatAmount = PlatformRoundingHandler.RoundDisplayValue(details[i].TotalVatAmount + (deviationStep * order));
                details[i].NetTotal = details[i].GrossTotal - details[i].TotalVatAmount;
                if (details[i].TotalDiscount == 0)
                {
                    details[i].NetTotalBeforeDiscount = details[i].NetTotal;
                    details[i].GrossTotalBeforeDiscount = details[i].GrossTotal;
                    details[i].TotalVatAmountBeforeDiscount = details[i].TotalVatAmount;
                    details[i].GrossTotalBeforeDocumentDiscount = details[i].GrossTotal;
                }
                details[i].Save();
                fixedDetails.Add(details[i]);
                i++;
                deviation -= deviationStep;
            }

            List<DocumentHeader> headersToRecalculate = fixedDetails.GroupBy(x => x.DocumentHeader).Select(x => x.Key).ToList();

            for (i = 0; i < headersToRecalculate.Count; i++)
            {
                DocumentHeader header = headersToRecalculate[i];
                documentService.RecalculateDocumentCosts(header, false);
                header.Save();
            }

            SessionManager.CommitTransactionsChanges();
        }

        public decimal GetTotalCashInPos(DailyTotals dailyTotals)
        {
            IEnumerable<DailyTotalsDetail> cashDetails = dailyTotals.DailyTotalsDetails.Where(dailyTotalDetail => dailyTotalDetail.DetailType == eDailyRecordTypes.CASH
                                                                                            || dailyTotalDetail.DetailType == eDailyRecordTypes.CASH_DIFFERENCE
                                                                                            || dailyTotalDetail.DetailType == eDailyRecordTypes.END_SHIFT_USER_COUNT_DOWN);
            return (cashDetails.Count() == 0) ? 0m : cashDetails.Sum(dailyTotalDetail => dailyTotalDetail.Amount);
        }

        public decimal GetDayStartingAmount(DailyTotals dailyTotals)
        {
            IEnumerable<DailyTotalsDetail> startingAmountDetails = dailyTotals.DailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.STARTING_AMOUNT);
            return (startingAmountDetails.Count() == 0) ? 0m : startingAmountDetails.Sum(x => x.Amount);
        }

        public void SaveUserDailyTotalCashDifference(IAppContext appContext, decimal cashDifference, decimal userClosingShiftCount, Guid currentUserOid, Guid currentPOSOid, Guid currentStoreOid)
        {
            CreateOrUpdateTotals(eTotalizorAction.UPDATE, currentUserOid, appContext.CurrentDailyTotals.Oid, appContext.CurrentUserDailyTotals.Oid, eDailyRecordTypes.CASH_DIFFERENCE, Guid.Empty, currentPOSOid, currentStoreOid, 1, cashDifference);
            CreateOrUpdateTotals(eTotalizorAction.UPDATE, currentUserOid, appContext.CurrentDailyTotals.Oid, appContext.CurrentUserDailyTotals.Oid, eDailyRecordTypes.END_SHIFT_USER_COUNT_DOWN, Guid.Empty, currentPOSOid, currentStoreOid, 1, -userClosingShiftCount);
        }

        public int GetNumberOfDocumentsInDailyTotals(DailyTotals dailyTotals, Guid documentType)
        {
            IEnumerable<DailyTotalsDetail> effectiveDetails = dailyTotals.DailyTotalsDetails.Where(detail => detail.DetailType == eDailyRecordTypes.INVOICES && detail.DocumentType == documentType);
            if (effectiveDetails.Count() == 0)
            {
                return 0;
            }
            else if (effectiveDetails.Count() == 1)
            {
                return (int)(effectiveDetails.First().QtyValue);
            }
            throw new ArgumentOutOfRangeException("Multiple Receipt Records in Z");
        }
    }
}
