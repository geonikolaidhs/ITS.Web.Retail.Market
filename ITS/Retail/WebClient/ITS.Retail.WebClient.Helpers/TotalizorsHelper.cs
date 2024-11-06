using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class TotalizorsHelper
    {
        /// <summary>
        /// Δημιουργεί ενα νεο DailyTotal ( Z ). Νεα φορολογική ημέρα εγρασίας 
        /// </summary>
        /// <returns>DailyTotals </returns>
        public static DailyTotals CreateDailyTotals(ITS.Retail.Model.POS terminal, Guid currentStore, Guid currentDailyTotals, out DateTime currentFiscalDate)
        {
            UnitOfWork uow = terminal.Session as UnitOfWork;
            DailyTotals dTotals;
            currentFiscalDate = DateTime.MinValue;
            if (currentDailyTotals != Guid.Empty)
            {
                dTotals = uow.GetObjectByKey<DailyTotals>(currentDailyTotals);
                if (dTotals != null && dTotals.FiscalDateOpen)
                {
                    // το Z υπάρχει και είναι ανοικτό

                    double daysDif = DateTime.Now.Subtract(dTotals.FiscalDate).TotalDays;
                    //if (daysDif > 2)
                    //{
                    //    // Αν η τρέχουσα ημερομινά είναι μεγαλύτερη απο την fiscalDate κατα 2 τουλαχιστον ημέρες ΄δωσε λάθος

                    //    throw new Exception("Fiscal Date period finished");
                    //}
                    currentFiscalDate = dTotals.FiscalDate;
                    return dTotals; // επέστερψε το Z που βρήκες;              
                }
            }

            // ή το Ζ δεν υπάρχει ή υπάρχει και είναι κλειστό 
            // δημιoυργισε ένα νέο.
            dTotals = new DailyTotals(uow) { FiscalDate = DateTime.Now, FiscalDateOpen = true, Store = terminal.Session.GetObjectByKey<Store>(currentStore), POS = terminal };
            dTotals.CreatedByDevice = terminal.ToString();
            dTotals.Save();
            uow.CommitTransaction();
            currentFiscalDate = dTotals.FiscalDate;
            return dTotals;
        }


        /// <summary>
        /// Ανοιγμα βάρδιας χρήστη
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserDailyTotals CreateUserDailtyTotals(UnitOfWork uow, Guid user, Guid currentDailyTotals, Guid currentUserDailyTotals, Guid currentTerminal, Guid currentStore)
        {
            UserDailyTotals udt;
            DailyTotals dt = uow.GetObjectByKey<DailyTotals>(currentDailyTotals);
            if (dt == null)
            {
                throw new Exception("Fiscal Date not opened");
            }
            if (currentUserDailyTotals != Guid.Empty)
            {
                udt = uow.GetObjectByKey<UserDailyTotals>(currentUserDailyTotals);
                if (udt != null && user.Equals(udt.User)) // αν βρέθηκε και ο User είναι ο ιδιος τότε επέστρεψέ το.
                {
                    return udt;
                }
            }
            // δεν βρέθηκε ή ο User είναι διαφορετικός.
            try
            {
                udt = new UserDailyTotals(uow);
                udt.User = uow.GetObjectByKey<User>(user);
                udt.DailyTotals = dt;
                udt.POS = uow.GetObjectByKey<ITS.Retail.Model.POS>(currentTerminal);
                udt.Store = uow.GetObjectByKey<Store>(currentStore);
                udt.FiscalDate = dt.FiscalDate;
                udt.CreatedBy = uow.GetObjectByKey<User>(user);
                udt.CreatedByDevice = dt.CreatedByDevice;
                udt.IsOpen = true;
                udt.Save();
                udt.Session.CommitTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Can not create User Daily Totals\n\n ERROR:{0}", ex.Message));
            }
            return udt;
        }

        //public static void IncreaseDrawer(User currentUser, DailyTotals currentDailyTotals, UserDailyTotals currentUserDaily, Store currentStore, POS currentPOS)
        //{

        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.DRAWERS, Guid.Empty, currentPOS.Oid, currentStore.Oid, 1, 0).Save();
        //    currentUser.Session.CommitChanges();
        //}
        /// <summary>
        /// Αναζήτηση του DailyTotalDetail με βάση το RecordType και GUID από το έιδος του Record που ψάχνουμε )
        /// </summary>
        /// <param name="utotals"></param>
        /// <param name="recType"></param>
        /// <returns></returns>
        private static DailyTotalsDetail SeekDailyTotalDetail(DailyTotals utotals, eDailyRecordTypes recType, Guid keyValue)
        {
            CriteriaOperator keyOperator;

            if (!keyValue.Equals(Guid.Empty))
                keyOperator = CriteriaOperator.Or(new BinaryOperator("DocumentType", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("VatFactor", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("Payment", keyValue, BinaryOperatorType.Equal));
            else
                keyOperator = null;

            return utotals.Session.FindObject<DailyTotalsDetail>(PersistentCriteriaEvaluationBehavior.InTransaction, CriteriaOperator.And(new BinaryOperator("DailyTotals", utotals, BinaryOperatorType.Equal),
            new BinaryOperator("DetailType", recType, BinaryOperatorType.Equal),
            keyOperator));
        }

        /// <summary>
        /// Ενημέρωση αθροιστή ημέρας
        /// </summary>
        /// <param name="userDetail"></param>
        /// <returns></returns>
        private static DailyTotalsDetail CreateOrUpdateDailyTotalDetail(UserDailyTotalsDetail userDetail, eTotalizorAction action, double qtyValue, double ammountValue)
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
            if (keyValue.Equals(Guid.Empty))
            {
                keyValue = userDetail.Payment.Oid;
            }
            if (userDetail.Payment.Equals(Guid.Empty))
            {
                keyValue = userDetail.VatFactor.Oid;
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
                detail.VatAmount = userDetail.VatAmount;
            }
            switch (action)
            {
                case eTotalizorAction.NONE:
                    return detail;
                case eTotalizorAction.INCREASE:
                    detail.QtyValue += qtyValue;
                    detail.Amount += ammountValue;
                    break;
                case eTotalizorAction.DECREASE:
                    detail.QtyValue -= qtyValue;
                    detail.Amount -= ammountValue;
                    break;
                case eTotalizorAction.UPDATE:
                    ComputeDailyTotals(userDetail.UserDailyTotals.DailyTotals);
                    break;
                case eTotalizorAction.CLEAR:
                    ComputeDailyTotals(userDetail.UserDailyTotals.DailyTotals, true);
                    break;
            }
            return detail;
        }

        /// <summary>
        /// Επαναυπολογίζει τα σύνολα ημέρας
        /// </summary>
        /// <param name="dtotals"></param>
        /// <returns>DailyTotalsDetail</returns>
        private static DailyTotals ComputeDailyTotals(DailyTotals dtotals, bool clear = false)
        {
            if (dtotals == null)
                throw new Exception("DailyTotals does not exist");
            if (!dtotals.FiscalDateOpen)
                throw new Exception("Daily Totals not opened");

            dtotals.Session.Delete(dtotals.DailyTotalsDetails);
            if (!clear)
            {
                foreach (UserDailyTotals userTotals in dtotals.UserDailyTotalss)
                {
                    foreach (UserDailyTotalsDetail userDetail in userTotals.UserDailyTotalsDetails)
                    {
                        DailyTotalsDetail dTotalDetail = CreateOrUpdateDailyTotalDetail(userDetail, eTotalizorAction.INCREASE, userDetail.QtyValue, userDetail.Amount);
                        if (dTotalDetail != null)
                            dtotals.DailyTotalsDetails.Add(dTotalDetail);
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
        private static UserDailyTotalsDetail SeekUserTotalDetail(UserDailyTotals utotals, eDailyRecordTypes recType, Guid keyValue)
        {
            CriteriaOperator keyOperator;

            if (recType == eDailyRecordTypes.DRAWERS ||
            recType == eDailyRecordTypes.LOYALTYPOINTS ||
            recType == eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL ||
            recType == eDailyRecordTypes.DISCOUNTS ||
            recType == eDailyRecordTypes.ITEMS)
                keyOperator = null;
            else
                keyOperator = CriteriaOperator.Or(new BinaryOperator("DocumentType", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("VatFactor", keyValue, BinaryOperatorType.Equal),
                new BinaryOperator("Payment", keyValue, BinaryOperatorType.Equal));

            return utotals.Session.FindObject<UserDailyTotalsDetail>(PersistentCriteriaEvaluationBehavior.InTransaction, CriteriaOperator.And(new BinaryOperator("UserDailyTotals", utotals, BinaryOperatorType.Equal),
            new BinaryOperator("DetailType", recType, BinaryOperatorType.Equal),
            keyOperator));
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
        private static UserDailyTotals CreateOrUpdateTotals(eTotalizorAction action, Guid currentUser, Guid currentDailyTotals, Guid currentUserDailyTotals, eDailyRecordTypes rectype, Guid keyValue, Guid currentTerminal, Guid currentStore, double qtyValue = 0, double amountValue = 0, double vatAmount = 0)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            UserDailyTotals usrTotals = uow.GetObjectByKey<UserDailyTotals>(currentUserDailyTotals);
            if (usrTotals == null)
            {
                if (currentUserDailyTotals == null)
                    throw new Exception("User not Logged In");
                // Create new UserTotals
                usrTotals = CreateUserDailtyTotals(uow,currentUser, currentDailyTotals, currentUserDailyTotals, currentTerminal, currentStore);
            }
            UserDailyTotalsDetail uddetail = SeekUserTotalDetail(usrTotals, rectype, keyValue);
            try
            {
                if (uddetail == null)
                {
                    uddetail = new UserDailyTotalsDetail(uow);
                    uddetail.UserDailyTotals = usrTotals;
                    uddetail.CreatedByDevice = usrTotals.CreatedByDevice;
                    uddetail.CreatedBy = uow.GetObjectByKey<User>(currentUser);
                    uddetail.DetailType = rectype;
                    // Update Key Value
                    switch (rectype)
                    {
                        case eDailyRecordTypes.INVOICES:
                            // Μέτρημα πραστατικών
                            uddetail.VatFactor = null;
                            uddetail.Payment = null;
                            uddetail.DocumentType = keyValue;
                            break;
                        case eDailyRecordTypes.PAYMENTS:
                            // τρόπων πληρωμών
                            uddetail.VatFactor = null;
                            uddetail.Payment = uow.GetObjectByKey<PaymentMethod>(keyValue);
                            uddetail.DocumentType = Guid.Empty;
                            break;
                        case eDailyRecordTypes.TAXRECORD:
                            //
                            uddetail.VatFactor = uow.GetObjectByKey<VatFactor>(keyValue);
                            uddetail.Payment = null;
                            uddetail.DocumentType = Guid.Empty;
                            break;
                        default:
                            // 
                            uddetail.VatFactor = null;
                            uddetail.Payment = null;
                            uddetail.DocumentType = Guid.Empty;
                            break;
                    }
                }
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
                uddetail.Save();
                CreateOrUpdateDailyTotalDetail(uddetail, action, qtyValue, amountValue).Save();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("UpdateUserTotalDetail ERROR : {0}", ex.Message));
            }
            return usrTotals;
        }
        /// <summary>
        /// Generates an XReport object used for printing
        /// </summary>
        /// <param name="udt"></param>
        public static Report CreateXReport(UserDailyTotals udt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType)
        {
            return CreateReport(udt.UserDailyTotalsDetails, terminalDescription, ReceiptInvoiceType, ProformaInvoiceType, DefaultCustomerVatlevelOid, DepositInvoiceType, WithdrawInvoiceType);
        }

        /// <summary>
        /// Generates an Report object used for printing
        /// </summary>
        /// <param name="udt"></param>
        public static Report CreateZReport(DailyTotals dt, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType)
        {
            return CreateReport(dt.DailyTotalsDetails, terminalDescription, ReceiptInvoiceType, ProformaInvoiceType, DefaultCustomerVatlevelOid, DepositInvoiceType, WithdrawInvoiceType);
        }

        private static Report CreateReport<T>(XPCollection<T> totalDetails, string terminalDescription, Guid ReceiptInvoiceType, Guid ProformaInvoiceType, Guid DefaultCustomerVatlevelOid, Guid DepositInvoiceType, Guid WithdrawInvoiceType) where T : TotalDetail
        {
            UnitOfWork uow = totalDetails.Session as UnitOfWork;
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
                PaymentMethod5 = " "
            };
            List<VatFactor> vatFactors;
            using (XPCollection<VatFactor> vcs = new XPCollection<VatFactor>(uow, new BinaryOperator("VatLevel", DefaultCustomerVatlevelOid), new SortProperty("Factor", DevExpress.Xpo.DB.SortingDirection.Ascending)))
            {
                vatFactors = vcs.OrderBy(x => x.Factor).Take(5).ToList();
            }

            try
            {
                report.VatFactor1 = vatFactors[0].Factor.ToString("P2");// .Description;
                report.VatFactor2 = vatFactors[1].Factor.ToString("P2");
                report.VatFactor3 = vatFactors[2].Factor.ToString("P2");
                report.VatFactor4 = vatFactors[3].Factor.ToString("P2");
                report.VatFactor5 = vatFactors[4].Factor.ToString("P2");
            }
            catch
            {

            }

            List<PaymentMethod> paymentMethods;
            using (XPCollection<PaymentMethod> vcs = new XPCollection<PaymentMethod>(uow))
            {
                paymentMethods = vcs.OrderBy(x => x.Code).Take(5).ToList();
            }
            try
            {
                report.PaymentMethod1 = paymentMethods[0].Description;
                report.PaymentMethod2 = paymentMethods[1].Description;
                report.PaymentMethod3 = paymentMethods[2].Description;
                report.PaymentMethod4 = paymentMethods[3].Description;
                report.PaymentMethod5 = paymentMethods[4].Description;
            }
            catch
            {

            }

            if (totalDetails.Count > 0)
            {
                //Type type = totalDetails.First().GetType();
                //foreach (TotalDetail detail in totalDetails)
                foreach (TotalDetail detail in totalDetails)
                {
                    PaymentMethod payment = null;
                    VatFactor vatFactor = null;
                    if (detail is UserDailyTotalsDetail)
                    {
                        payment = (detail as UserDailyTotalsDetail).Payment;
                        vatFactor = (detail as UserDailyTotalsDetail).VatFactor;
                    }
                    else if(detail is DailyTotalsDetail)
                    {
                        payment = (detail as DailyTotalsDetail).Payment;
                        vatFactor = (detail as DailyTotalsDetail).VatFactor;
                    }

                    switch (detail.DetailType)
                    {
                        case eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL:
                            report.CanceledQty += (int)detail.QtyValue;
                            report.CanceledAmount += detail.Amount;
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
                            else if (detail.DocumentType == ReceiptInvoiceType)
                            {
                                report.ReceiptsQty += (int)detail.QtyValue;
                                report.ReceiptsAmount += detail.Amount;
                            }
                            else if (detail.DocumentType == WithdrawInvoiceType)
                            {
                                report.WithdrawsQty += (int)detail.QtyValue;
                                report.WithdrawsAmount += detail.Amount;
                            }
                            else if (detail.DocumentType == DepositInvoiceType)
                            {
                                report.DepositsQty += (int)detail.QtyValue;
                                report.DepositsAmount += detail.Amount;
                            }
                            break;
                        case eDailyRecordTypes.LOYALTYPOINTS:
                            report.LoyaltyPointsQty += (int)detail.QtyValue;
                            report.LoyaltyPointsAmount += detail.Amount;
                            break;
                        case eDailyRecordTypes.TAXRECORD:
                            if (vatFactor != null)
                            {
                                if (report.VatFactor1 != null && (vatFactor.Factor.ToString("P2") == report.VatFactor1))
                                {
                                    report.VatAmount1 += detail.VatAmount;
                                    report.GrossAmount1 += detail.Amount;
                                    report.NetAmount1 += detail.Amount - detail.VatAmount;
                                }
                                else if (report.VatFactor2 != null && (vatFactor.Factor.ToString("P2") == report.VatFactor2))
                                {
                                    report.VatAmount2 += detail.VatAmount;
                                    report.GrossAmount2 += detail.Amount;
                                    report.NetAmount2 += detail.Amount - detail.VatAmount;
                                }
                                else if (report.VatFactor3 != null && (vatFactor.Factor.ToString("P2") == report.VatFactor3))
                                {
                                    report.VatAmount3 += detail.VatAmount;
                                    report.GrossAmount3 += detail.Amount;
                                    report.NetAmount3 += detail.Amount - detail.VatAmount;
                                }
                                else if (report.VatFactor4 != null && (vatFactor.Factor.ToString("P2") == report.VatFactor4))
                                {
                                    report.VatAmount4 += detail.VatAmount;
                                    report.GrossAmount4 += detail.Amount;
                                    report.NetAmount4 += detail.Amount - detail.VatAmount;
                                }
                                else if (report.VatFactor5 != null && (vatFactor.Factor.ToString("P2") == report.VatFactor5))
                                {
                                    report.VatAmount5 += detail.VatAmount;
                                    report.GrossAmount5 += detail.Amount;
                                    report.NetAmount5 += detail.Amount - detail.VatAmount;
                                }
                            }
                            break;
                        case eDailyRecordTypes.ITEMS:
                            report.ItemsQty += (int)detail.QtyValue;
                            report.ItemsAmount += detail.Amount;
                            break;
                        case eDailyRecordTypes.PAYMENTS:
                            if (payment != null)
                            {
                                if (report.PaymentMethod1 != null && (payment.Description == report.PaymentMethod1))
                                {
                                    report.PaymentMethodAmount1 += detail.Amount;
                                }
                                else if (report.PaymentMethod2 != null && (payment.Description == report.PaymentMethod2))
                                {
                                    report.PaymentMethodAmount2 += detail.Amount;
                                }
                                else if (report.PaymentMethod3 != null && (payment.Description == report.PaymentMethod3))
                                {
                                    report.PaymentMethodAmount3 += detail.Amount;
                                }
                                else if (report.PaymentMethod4 != null && (payment.Description == report.PaymentMethod4))
                                {
                                    report.PaymentMethodAmount4 += detail.Amount;
                                }
                                else if (report.PaymentMethod5 != null && (payment.Description == report.PaymentMethod5))
                                {
                                    report.PaymentMethodAmount5 += detail.Amount;
                                }
                            }
                            break;
                        case eDailyRecordTypes.CASH:
                            report.CashAmount += detail.Amount;
                            break;
                    }
                }
            }
            report.TotalAmount = report.NetAmount1 + report.NetAmount2 + report.NetAmount3 + report.NetAmount4 + report.NetAmount5;
            report.TotalVatAmount = report.VatAmount1 + report.VatAmount2 + report.VatAmount3 + report.VatAmount4 + report.VatAmount5;


            return report;
        }

        internal class VatFactorQtyAmount
        {
            public Guid VatFactorID;
            public double Qty;
            public double Amount;
            public double Factor;
            public double NetAmount;
            public double VatAmount;


        }


        //public static void UpdateTotalizers(DocumentHeader header, User currentUser, DailyTotals currentDailyTotals, UserDailyTotals currentUserDaily, Store currentStore,
        //    POS currentPOS, bool increaseVatAndSales)
        //{
        //    double totalItemsCount = 0;
        //    double totalItemsAmount = 0;
        //    header.DocumentDetails.Filter = null;

        //    List<VatFactorQtyAmount> vatFactorItemsCount = new List<VatFactorQtyAmount>();

        //    List<Tuple<Guid, double>> documentPaymentMethodAmounts = new List<Tuple<Guid, double>>();

        //    int canceledCount = 0;
        //    double canceledAmount = 0;

        //    int canceledDocumentsCount = 0;
        //    double canceledDocumentsAmmount = 0;

        //    int loyaltyCount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalPoints != 0).Count();        //???
        //    double loyaltyAmount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalPoints != 0).Sum(docDetail => docDetail.TotalPoints);    //???

        //    //int discountsCount = header.DocumentDetails.Where(x => x.TotalDiscount !=0).Count();      ///????? tbc  
        //    //Μάλλον λάθος.Λογικά θα θέλεις τις εκτπώσεις των μη  ακυρωμένων γραμμών
        //    ///            
        //    //int discountsCount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalDiscountIncludingVAT != 0).Count();
        //    int discountsCount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalDiscount != 0).Count();
        //    //double discountsAmount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalDiscountIncludingVAT != 0).Sum(g => g.TotalDiscountIncludingVAT);///????
        //    double discountsAmount = header.DocumentDetails.Where(docDetail => docDetail.IsCanceled == false && docDetail.TotalDiscount != 0).Sum(g => g.TotalDiscount);///????

        //    int drawersCount = 0;        //???
        //    double drawersAmount = 0;    //???

        //    int couponsCount = 0;        //???  έχουμε κουπόνια είδους και κοπόνια εκπτωτικά!!!
        //    double couponsAmount = 0;    //???

        //    int itemsIncreasedCount = 0;        //???  δεν ξέρουμε ακόμη τι είναι
        //    double itemsIncreasedAmount = 0;    //???

        //    int documentsIncreasedCount = 0;        //???  δεν ξέρουμε ακόμη τι είναι
        //    double documentsIncreasedAmount = 0;    //???

        //    int switchesReturnsCount = 0;        //???  //TOCHECK
        //    double switchesReturnsAmount = 0;    //???

        //    int invoicesCount = 1;
        //    //double invoicesAmount = header.GrossTotalAfterDocumentDiscount;
        //    double invoicesAmount = header.GrossTotal;

        //    if (header.IsCanceled == true)
        //    {
        //        canceledDocumentsCount++;
        //        canceledDocumentsAmmount += header.GrossTotal; //???
        //    }
        //    else
        //    {
        //        if (increaseVatAndSales)
        //        {
        //            foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
        //            {

        //                totalItemsCount += detail.Qty;
        //                totalItemsAmount += detail.GrossTotalAfterDocumentDiscount;


        //                //Item item = SessionHelper.Master.GetObjectByKey<Item>(detail.Item);
        //                VatFactor itemVatFac = uow.GetObjectByKey<VatFactor>(detail.VatFactor);
        //                VatFactorQtyAmount vatFactorCount = vatFactorItemsCount.Where(x => x.VatFactorID == itemVatFac.Oid).FirstOrDefault();
        //                if (vatFactorCount == null)
        //                {
        //                    vatFactorCount = new VatFactorQtyAmount();
        //                    vatFactorCount.VatFactorID = itemVatFac.Oid;
        //                    vatFactorCount.Factor = detail.VatFactor;
        //                    vatFactorItemsCount.Add(vatFactorCount);

        //                }
        //                vatFactorCount.Amount += detail.GrossTotalAfterDocumentDiscount;
        //                vatFactorCount.NetAmount += detail.NetTotalAfterDocumentDiscount;
        //                vatFactorCount.VatAmount += detail.TotalVatAmountAfterDocumentDiscount;
        //                vatFactorCount.Qty += detail.Qty;
        //            }
        //        }

        //        foreach (DocumentDetail canceledDetail in header.DocumentDetails.Where(x => x.IsCanceled == true))
        //        {
        //            canceledCount++;
        //            canceledAmount += canceledDetail.GrossTotalAfterDocumentDiscount;
        //        }

        //        foreach (DocumentPayment documentPayment in header.DocumentPayments)//Payments)
        //        {
        //            //Tuple<Guid, double> paymentMethodAmountTuple = documentPaymentMethodAmounts.Where(x => x.Key1 == documentPayment.PaymentMethod).FirstOrDefault();
        //            Tuple<Guid, double> paymentMethodAmountTuple = documentPaymentMethodAmounts.Where(x => x.Key1 == documentPayment.PaymentMethod).FirstOrDefault();
        //            if (paymentMethodAmountTuple == null)
        //            {
        //                paymentMethodAmountTuple = new Tuple<Guid, double>();
        //                paymentMethodAmountTuple.Key1 = documentPayment.PaymentMethod;
        //                documentPaymentMethodAmounts.Add(paymentMethodAmountTuple);
        //            }
        //            paymentMethodAmountTuple.Key2 += documentPayment.Amount;
        //        }
        //    }

        //    //Ενημέρωση των αθροιστών των ειδών, eDailyRecordTypes.ITEMS
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.ITEMS, Guid.Empty, currentPOS.Oid, currentStore.Oid, totalItemsCount, totalItemsAmount).Save();

        //    //Ενημέρωση των αθροιστών κατηγοριών ΦΠΑ, eDailyRecordTypes.TAXRECORD
        //    foreach (VatFactorQtyAmount vatTuple in vatFactorItemsCount)
        //    {

        //        TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //            eDailyRecordTypes.TAXRECORD, vatTuple.VatFactorID, currentPOS.Oid, currentStore.Oid, vatTuple.Qty, vatTuple.Amount, vatTuple.VatAmount
        //            ).Save();
        //    }

        //    //Ενημέρωση των αθροιστών των πληρωμών, eDailyRecordTypes.PAYMENTS
        //    foreach (Tuple<Guid, double> paymentTuple in documentPaymentMethodAmounts)
        //    {
        //        TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //            eDailyRecordTypes.PAYMENTS, paymentTuple.Key1, currentPOS.Oid, currentStore.Oid, (paymentTuple.Key2 > 0 ? 1 : 0), paymentTuple.Key2).Save();
        //    }

        //    //Ενημέρωση των αθροιστών των πόντων eDailyRecordTypes.LOYALTYPOINTS
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.LOYALTYPOINTS, Guid.Empty, currentPOS.Oid, currentStore.Oid, loyaltyCount, loyaltyAmount).Save();

        //    //Ενημέρωση των αθροιστών των ακυρομένων eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL, Guid.Empty, currentPOS.Oid, currentStore.Oid, canceledDocumentsCount, canceledDocumentsCount).Save();

        //    //Ενημέρωση των αθροιστών των ακυρομένων eDailyRecordTypes.CANCELED_DOCUMENT
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.CANCELED_DOCUMENT, Guid.Empty, currentPOS.Oid, currentStore.Oid, canceledCount, canceledAmount).Save();

        //    //Ενημέρωση των αθροιστών των εκπτώσεων eDailyRecordTypes.DISCOUNTS
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.DISCOUNTS, Guid.Empty, currentPOS.Oid, currentStore.Oid, discountsCount, discountsAmount).Save();

        //    //Ενημέρωση των αθροιστών των εκπτώσεων eDailyRecordTypes.DOCUMENT_DISCOUNTS
        //    /*TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.DOCUMENT_DISCOUNTS, Guid.Empty, currentPOS.Oid, currentStore.Oid, (header.DocumentDiscountPercentage > 0) ? 1 : 0, header.DocumentDiscountValue).Save();
        //    */
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.DOCUMENT_DISCOUNTS, Guid.Empty, currentPOS.Oid, currentStore.Oid, header.DocumentDiscount).Save();

        //    //Ενημέρωση των αθροιστών των κουπονιών eDailyRecordTypes.COUPONS
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.COUPONS, Guid.Empty, currentPOS.Oid, currentStore.Oid, couponsCount, couponsAmount).Save();

        //    //Ενημέρωση των αθροιστών των αναλήψεων eDailyRecordTypes.DRAWERS
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.DRAWERS, Guid.Empty, currentPOS.Oid, currentStore.Oid, drawersCount, drawersAmount).Save();

        //    //Ενημέρωση των αθροιστών των προτιμολογίων/αποδείξεων/αναλήψεων/καταθέσων eDailyRecordTypes.INVOICES
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.INVOICES, header.DocumentType.Oid, currentPOS.Oid, currentStore.Oid, invoicesCount, invoicesAmount).Save();

        //    //Ενημέρωση των αθροιστών των επιστροφών eDailyRecordTypes.INVOICES
        //    TotalizorsHelper.CreateOrUpdateTotals(eTotalizorAction.INCREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.RETURNS, header.DocumentType.Oid, currentPOS.Oid, currentStore.Oid, switchesReturnsCount, switchesReturnsAmount).Save();

        //    DocumentType dtype = header.Session.GetObjectByKey<DocumentType>(header.DocumentType);
        //    TotalizorsHelper.CreateOrUpdateTotals(dtype.ValueFactor >= 0 ? eTotalizorAction.INCREASE : eTotalizorAction.DECREASE, currentUser.Oid, currentDailyTotals.Oid, currentUserDaily.Oid,
        //        eDailyRecordTypes.CASH, header.DocumentType.Oid, currentPOS.Oid, currentStore.Oid, 0, invoicesAmount).Save();

        //    header.Session.CommitChanges();
        //}

    }
}
