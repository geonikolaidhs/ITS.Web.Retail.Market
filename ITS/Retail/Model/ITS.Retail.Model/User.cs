//-----------------------------------------------------------------------
// <copyright file="User.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{
    [Updater(Order = 130, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("User", typeof(ResourcesLib.Resources))]

    public class User : BaseObj, IUser
    {
        public User()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public User(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.Email = string.Empty;
            this.IsB2CUser = false;
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:

                    if (owner == null)
                    {
                        throw new Exception("User.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new NotOperator(new AggregateOperand("Role.RoleEntityAccessPermisions", Aggregate.Exists)),
                                               new ContainsOperator("UserTypeAccesses",
                                                                   CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName),
                                                                                        new BinaryOperator("EntityOid", owner.Oid)
                                                                                       )
                                                                   )
                                              );
                    break;

                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("User.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = CriteriaOperator.And(
                                CriteriaOperator.Or(
                                    new NotOperator(new AggregateOperand("Role.RoleEntityAccessPermisions", Aggregate.Exists)),
                                    new ContainsOperator("UserTypeAccesses", CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName), new BinaryOperator("EntityOid", owner.Oid)))
                                ),
                                new NotOperator(new NullOperator("POSUserName"))
                            );
                    break;
            }

            return crop;
        }

        public User(User u)
            : base()
        {
            _FullName = u.FullName;
            _Password = u.Password;
            _UserName = u.UserName;
        }

        private DocumentHeader _WishList;
        private bool _IsB2CUser;
        private DateTime _LastLoginDate;
        private string _Comment;
        private DateTime _FailedPasswordAttemptWindowStart;
        private int _FailedPasswordAttemptCount;
        private DateTime _LastLockedOutDate;
        private DateTime _LastActivityDate;
        private bool _IsLockedOut;
        private DateTime _FailedPasswordAnswerAttemptWindowStart;
        private int _FailedPasswordAnswerAttemptCount;
        private string _PasswordAnswer;
        private string _PasswordQuestion;
        private bool _IsApproved;
        private string _Email;
        private string _Key;
        private Store _CentralStore;
        private bool _IsCentralStore;
        private bool _TermsAccepted;
        private string _UserName;
        private string _FullName;
        private string _Password;
        private string _TaxCode;
        private Role _Role;
        private string _AuthToken;

        [Indexed("GCRecord", Unique = true)]
        [DescriptionField]
        [DisplayOrder(Order = 10)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }


        [DisplayOrder(Order = 1)]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                SetPropertyValue("FullName", ref _FullName, value);
            }
        }


        [DisplayOrder(Order = 11)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }

        [DisplayOrder(Order = 2)]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }
        [Association("User-UserTypeAccesses"), Aggregated]
        public XPCollection<UserTypeAccess> UserTypeAccesses
        {
            get
            {
                return GetCollection<UserTypeAccess>("UserTypeAccesses");
            }
        }

        [Association("Role-Users")]
        [DisplayOrder(Order = 7)]
        public Role Role
        {
            get
            {
                return _Role;
            }
            set
            {
                SetPropertyValue("Role", ref _Role, value);
            }
        }


        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                SetPropertyValue("Key", ref _Key, value);
            }
        }

        [Association("User-TablePermissions")]
        public XPCollection<TablePermission> TablePermissions
        {
            get
            {
                return GetCollection<TablePermission>("TablePermissions");
            }
        }

        [Association("User-UserDailyTotalss")]
        public XPCollection<UserDailyTotals> UserDailyTotals
        {
            get
            {
                return GetCollection<UserDailyTotals>("UserDailyTotals");
            }
        }

        [NonPersistent]
        [DisplayOrder(Order = 3)]
        public string AssociatedCustomers
        {
            get
            {
                var EntitiesOid = from obj in UserTypeAccesses
                                  select obj.EntityOid;

                XPCollection<Customer> customers = new XPCollection<Customer>(Session, new InOperator("Oid", EntitiesOid.ToList()));

                string returnValue = "";
                foreach (Customer cust in customers)
                {
                    returnValue += cust.Trader.TaxCode + " : " + cust.CompanyName + " , ";
                }

                if (returnValue.Length != 0)
                {
                    returnValue = returnValue.Substring(0, returnValue.Length - 3);
                }
                return returnValue;
            }
        }

        [NonPersistent]
        public string AssociatedSuppliers
        {
            get
            {
                var EntitiesOid = from obj in UserTypeAccesses
                                  select obj.EntityOid;
                XPCollection<CompanyNew> suppliers = new XPCollection<CompanyNew>(Session, new InOperator("Oid", EntitiesOid.ToList()));
                string returnValue = "";
                foreach (CompanyNew supl in suppliers)
                {
                    returnValue += supl.Trader.TaxCode + " : " + supl.CompanyName + " , ";
                }

                if (returnValue.Length != 0)
                {
                    returnValue = returnValue.Substring(0, returnValue.Length - 3);
                }
                return returnValue;
            }
        }

        [NonPersistent]
        public string FullUsername
        {
            get
            {
                return this.UserName + @"/" + this.POSUserName;
            }
        }

        public string AuthToken
        {
            get
            {
                return _AuthToken;
            }
            set
            {
                SetPropertyValue("AuthToken", ref _AuthToken, value);
            }
        }


        public bool TermsAccepted
        {
            get
            {
                return _TermsAccepted;
            }
            set
            {
                SetPropertyValue("TermsAccepted", ref _TermsAccepted, value);
            }
        }

        [DisplayOrder(Order = 8)]
        public bool IsCentralStore
        {
            get
            {
                return _IsCentralStore;
            }
            set
            {
                SetPropertyValue("IsCentralStore", ref _IsCentralStore, value);
            }
        }


        [Association("CentralStore-Users")]
        public Store CentralStore
        {
            get
            {
                return _CentralStore;
            }
            set
            {
                SetPropertyValue("CentralStore", ref _CentralStore, value);
            }
        }

        private string _POSUserName;
        [DisplayOrder(Order = 4)]
        public string POSUserName
        {
            get
            {
                return _POSUserName;
            }
            set
            {
                SetPropertyValue("POSUserName", ref _POSUserName, value);
            }
        }


        private string _POSPassword;
        [DisplayOrder(Order = 5)]
        public string POSPassword
        {
            get
            {
                return _POSPassword;
            }
            set
            {
                SetPropertyValue("POSPassword", ref _POSPassword, value);
            }
        }

        private ePOSUserLevel _POSUserLevel;
        [DisplayOrder(Order = 6)]
        public ePOSUserLevel POSUserLevel
        {
            get
            {
                return _POSUserLevel;
            }
            set
            {
                SetPropertyValue("POSUserLevel", ref _POSUserLevel, value);
            }
        }

        [DisplayOrder(Order = 9)]
        public bool IsApproved
        {
            get
            {
                return _IsApproved;
            }
            set
            {
                SetPropertyValue("IsApproved", ref _IsApproved, value);
            }
        }

        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                SetPropertyValue("Email", ref _Email, value);
            }
        }


        public string PasswordQuestion
        {
            get
            {
                return _PasswordQuestion;
            }
            set
            {
                SetPropertyValue("PasswordQuestion", ref _PasswordQuestion, value);
            }
        }


        public string PasswordAnswer
        {
            get
            {
                return _PasswordAnswer;
            }
            set
            {
                SetPropertyValue("PasswordAnswer", ref _PasswordAnswer, value);
            }
        }


        public int FailedPasswordAnswerAttemptCount
        {
            get
            {
                return _FailedPasswordAnswerAttemptCount;
            }
            set
            {
                SetPropertyValue("FailedPasswordAnswerAttemptCount", ref _FailedPasswordAnswerAttemptCount, value);
            }
        }


        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get
            {
                return _FailedPasswordAnswerAttemptWindowStart;
            }
            set
            {
                SetPropertyValue("FailedPasswordAnswerAttemptWindowStart", ref _FailedPasswordAnswerAttemptWindowStart, value);
            }
        }


        public bool IsLockedOut
        {
            get
            {
                return _IsLockedOut;
            }
            set
            {
                SetPropertyValue("IsLockedOut", ref _IsLockedOut, value);
            }
        }


        public DateTime LastActivityDate
        {
            get
            {
                return _LastActivityDate;
            }
            set
            {
                SetPropertyValue("LastActivityDate", ref _LastActivityDate, value);
            }
        }


        public DateTime LastLockedOutDate
        {
            get
            {
                return _LastLockedOutDate;
            }
            set
            {
                SetPropertyValue("LastLockedOutDate", ref _LastLockedOutDate, value);
            }
        }


        public int FailedPasswordAttemptCount
        {
            get
            {
                return _FailedPasswordAttemptCount;
            }
            set
            {
                SetPropertyValue("FailedPasswordAttemptCount", ref _FailedPasswordAttemptCount, value);
            }
        }


        public DateTime FailedPasswordAttemptWindowStart
        {
            get
            {
                return _FailedPasswordAttemptWindowStart;
            }
            set
            {
                SetPropertyValue("FailedPasswordAttemptWindowStart", ref _FailedPasswordAttemptWindowStart, value);
            }
        }


        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                SetPropertyValue("Comment", ref _Comment, value);
            }
        }


        public DateTime LastLoginDate
        {
            get
            {
                return _LastLoginDate;
            }
            set
            {
                SetPropertyValue("LastLoginDate", ref _LastLoginDate, value);
            }
        }

        public bool IsPOSUser
        {
            get
            {
                if (Role == null)
                {
                    return false;
                }
                return Role.Type == eRoleType.CompanyUser || Role.Type == eRoleType.CompanyAdministrator;
            }
        }

        public bool IsB2CUser
        {
            get
            {
                return _IsB2CUser;
            }
            set
            {
                SetPropertyValue("IsB2CUser", ref _IsB2CUser, value);
            }
        }

        public EntityAccessPermision GetPermission(Type entityType)
        {
            string type = entityType.ToString();
            if (Role == null || Role.Oid == Guid.Empty) return null;
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Role.Oid", Role.Oid), new BinaryOperator("EntityType", type));
            RoleEntityAccessPermision foundIt = this.Session.FindObject<RoleEntityAccessPermision>(crop);
            return foundIt.EnityAccessPermision;
        }

        [Association("User-GridSettings"), Aggregated]
        public XPCollection<GridSettings> GridSettings
        {
            get
            {
                return GetCollection<GridSettings>("GridSettings");
            }
        }


        [Aggregated, Association("User-WidgetManagers")]
        public XPCollection<WidgetManager> WidgetManagers
        {
            get
            {
                return GetCollection<WidgetManager>("WidgetManagers");
            }
        }


        [Aggregated, Association("User-SpreadSheet")]
        public XPCollection<SpreadSheet> SpreadSheet
        {
            get
            {
                return GetCollection<SpreadSheet>("SpreadSheet");
            }
        }

        [UpdaterIgnoreField]
        public DocumentHeader WishList
        {
            get
            {
                return _WishList;
            }
            set
            {
                SetPropertyValue("WishList", ref _WishList, value);
            }
        }


        public List<DailyCashierShift> GetDailyCashierShifts(DateTime DateFrom, DateTime DateTo, List<Guid> storeOids)
        {
            List<DailyCashierShift> DailyCashierShifts = new List<DailyCashierShift>();
            DateTime startDate = DateFrom.Date;
            DateTime endDate = DateTo.Date;
            List<Store> stores = new XPCollection<Store>(this.Session, CriteriaOperator.And(new InOperator("Oid", storeOids))).ToList();
            foreach (Store store in stores)
            {
                while (startDate <= endDate)
                {
                    List<CashierShift> Shifts = GetCashierShifts(startDate, store);
                    if (Shifts != null && Shifts.Count > 0)
                    {
                        DailyCashierShift DailyCashierShift = new DailyCashierShift();
                        DailyCashierShift.ReportDate = startDate;
                        DailyCashierShift.User = this;
                        DailyCashierShift.Store = store;
                        DailyCashierShift.CashierShifts = new List<CashierShift>();
                        DailyCashierShift.CashierShifts.AddRange(Shifts);
                        DailyCashierShifts.Add(DailyCashierShift);
                    }
                    DateTime tempDate = startDate.AddDays(1);
                    startDate = tempDate;
                }
                DailyCashierShifts.ForEach(x => x.TotalSellValue = x.CashierShifts.Sum(z => z.ShiftSellValue));
            }
            return DailyCashierShifts.OrderBy(x => x.ReportDate).ToList();
        }


        private List<CashierShift> GetCashierShifts(DateTime DailyDate, Store store)
        {

            List<CashierShift> Shifts = new List<CashierShift>();
            List<DocumentHeader> allInvoiceHeaders = new List<DocumentHeader>();
            List<DocumentHeader> PosProformaHeaders = new List<DocumentHeader>();
            List<DocumentHeader> headersToCharge = new List<DocumentHeader>();
            List<DocumentHeader> TransformedFromProformaHeaders = new List<DocumentHeader>();
            StoreControllerSettings settings = store.StoreControllerSettings;
            DateTime startDate = DailyDate.Date;
            DateTime endDate = startDate.AddHours(23).AddMinutes(59);

            List<UserDailyTotals> userDailys = new XPCollection<UserDailyTotals>(this.Session, CriteriaOperator.And(
                    new BetweenOperator("FiscalDate", startDate, endDate),
                    new BinaryOperator("Store.Oid", store.Oid, BinaryOperatorType.Equal),
                    new BinaryOperator("User.Oid", this.Oid, BinaryOperatorType.Equal)
             )).ToList();

            CriteriaOperator ValidDocHeaderCriteria = CriteriaOperator.And(
          new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
          new BinaryOperator("DocumentSeries.IsCancelingSeries", false, BinaryOperatorType.Equal),
          new OperandProperty("GCRecord").IsNull(),
          new OperandProperty("CanceledByDocument").IsNull(),
          new BinaryOperator("Store.Oid", store.Oid, BinaryOperatorType.Equal),
          new BinaryOperator("FinalizedDate", startDate, BinaryOperatorType.GreaterOrEqual),
          new BinaryOperator("FinalizedDate", endDate, BinaryOperatorType.LessOrEqual)
       );
            CriteriaOperator isProformaCrit = CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", settings.ProformaDocumentType.Oid, BinaryOperatorType.Equal));
            CriteriaOperator isNotProformaCrit = CriteriaOperator.And(
                                                                   new BinaryOperator("DocumentNumber", 0, BinaryOperatorType.Greater),
                                                                   new OperandProperty("POS").IsNull(),
                                                                   new BinaryOperator("DocumentType.DisplayInCashCount", true, BinaryOperatorType.Equal),
                                                                   new BinaryOperator("DocumentType.Oid", settings.ProformaDocumentType.Oid, BinaryOperatorType.NotEqual),
                                                                   new BinaryOperator("ChargedToUser.Oid", this.Oid, BinaryOperatorType.Equal));


            List<Guid> PosProformaOids = new List<Guid>();
            allInvoiceHeaders = new XPCollection<DocumentHeader>(this.Session, CriteriaOperator.And(ValidDocHeaderCriteria, isNotProformaCrit)).ToList() ?? new List<DocumentHeader>();
            PosProformaHeaders = new XPCollection<DocumentHeader>(this.Session, CriteriaOperator.And(ValidDocHeaderCriteria, isProformaCrit)).ToList();
            int shifts = userDailys.Count + (allInvoiceHeaders.Count > 0 ? 1 : 0);

            if (shifts > 0)
            {
                if (userDailys.Count > 0)
                {
                    List<CashierShift> posShifts = CreateCashierShiftsFromUserDailyTotals(userDailys, settings);
                    if (posShifts != null && posShifts.Count > 0)
                    {
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.ShiftType = ShiftType.POS));
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.User = this));
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.Store = store));
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.Company = store.Owner));
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.ReportDate = startDate));
                        posShifts.ForEach(f => f.CashierShiftReportLines.ForEach(x => x.Device = "POS " + f.UserDailyTotals?.POS?.ID.ToString()));
                        posShifts.ForEach(x => x.ReportDate = startDate);
                        Shifts.AddRange(posShifts);
                    }
                }
                if (allInvoiceHeaders.Count > 0)
                {
                    CashierShift shift = CreateCashierShiftFromReception(settings, allInvoiceHeaders, PosProformaHeaders);
                    if (shift != null)
                    {
                        shift.CashierShiftReportLines.ForEach(x => x.User = this);
                        shift.CashierShiftReportLines.ForEach(x => x.Store = store);
                        shift.CashierShiftReportLines.ForEach(x => x.Company = store?.Owner);
                        shift.CashierShiftReportLines.ForEach(x => x.ReportDate = startDate);
                        shift.CashierShiftReportLines.ForEach(x => x.ShiftType = ShiftType.RECEPTION);
                        shift.CashierShiftReportLines.ForEach(x => x.Device = "StoreController ");
                        shift.ReportDate = startDate;
                        Shifts.Add(shift);
                    }
                }
            }
            return Shifts;
        }


        private List<CashierShift> CreateCashierShiftsFromUserDailyTotals(List<UserDailyTotals> userDailyTotals, StoreControllerSettings settings)
        {
            List<CashierShift> shifts = new List<CashierShift>();
            foreach (UserDailyTotals udt in userDailyTotals)
            {
                CashierShift CashierShift = CreateCashierShiftFromPOS(udt, settings);
                if (CashierShift != null)
                {
                    shifts.Add(CashierShift);
                }
            }
            return shifts;
        }

        private CashierShift CreateCashierShiftFromPOS(UserDailyTotals userDailyTotals, StoreControllerSettings settings)
        {
            CashierShift shift = new CashierShift();
            int index = 0;
            decimal startingAmount = userDailyTotals.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.STARTING_AMOUNT).FirstOrDefault()?.Amount ?? 0m;
            shift.Device = userDailyTotals.POS.Name;
            try
            {
                List<CashierShiftReportLine> paymentReportLines = new List<CashierShiftReportLine>();
                //οι τύποι παραστατικών για ανάληψη/κατάθεση
                DocumentType depositDocumentType = settings.DepositDocumentType;
                DocumentType withdrawlDocumentType = settings.WithdrawalDocumentType;
                decimal previousDrawerValue = 0;

                //Δημιουργούμε CashierDailyTotal για το Πάγιο, που προστίθεται στη λίστα του αποτελέσματος
                //στην αντιστοίχιση στο πεδίο που καταχωρεί ο χρήστης (CountedAmount) βάζουμε 0, το ποσο του παγίου του θα το προσμετρήσει στα παραδοτέα μετρητά.
                CashierShiftReportLine pagioPayment = new CashierShiftReportLine();
                pagioPayment.AfftectsDrawer = true;
                pagioPayment.Description = "Πάγιο";
                pagioPayment.CashierAmount = 0;
                pagioPayment.CashierQty = 0;
                pagioPayment.index = 0;
                pagioPayment.QtyDifference = 0;
                pagioPayment.LineType = eCashierLineType.Payment;
                pagioPayment.DeviceQty = startingAmount > 0 ? 1 : 0;
                pagioPayment.DeviceAmount = startingAmount;
                pagioPayment.DeviceDrawer = pagioPayment.DeviceAmount;
                pagioPayment.PaymentMethod = null;
                pagioPayment.DocumentType = null;
                pagioPayment.ValueDifference = pagioPayment.CashierAmount - pagioPayment.DeviceAmount;
                pagioPayment.DrawerDifference = 0 + pagioPayment.ValueDifference;
                paymentReportLines.Add(pagioPayment);
                index++;
                previousDrawerValue = pagioPayment.DeviceAmount;

                // οι καταθέσεις/αναλήψεις του Pos grouped/summed από τα UserDailyTotalsDetails
                UserDailyTotalsDetail posDeposit = null;
                posDeposit = userDailyTotals.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.INVOICES && x.DocumentType == depositDocumentType.Oid && x.Amount != 0).FirstOrDefault();
                UserDailyTotalsDetail posWithdraw = null;
                posWithdraw = userDailyTotals.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.INVOICES && x.DocumentType == withdrawlDocumentType.Oid && x.Amount != 0).FirstOrDefault();

                //λίστες από τις καταθέσεις/αναλήψεις του χειριστή
                List<UserDailyTotalsCashCountDetail> userDeposits = new List<UserDailyTotalsCashCountDetail>();
                userDeposits = userDailyTotals.UserDailyTotalsCashCountDetails.Where(x => x.DocumentType != null && x.DetailType == eCashCountRecordTypes.COUNTED_DOCUMENTS &&
                                                                                                   x.DocumentType.Oid == depositDocumentType.Oid)?.ToList() ?? new List<UserDailyTotalsCashCountDetail>();

                List<UserDailyTotalsCashCountDetail> userwithdrawls = new List<UserDailyTotalsCashCountDetail>();
                userwithdrawls = userDailyTotals.UserDailyTotalsCashCountDetails.Where(x => x.DocumentType != null && x.DetailType == eCashCountRecordTypes.COUNTED_DOCUMENTS &&
                                                                                                   x.DocumentType.Oid == withdrawlDocumentType.Oid)?.ToList() ?? new List<UserDailyTotalsCashCountDetail>();
                //οι καταθέσεις/αναλήψεις του χειριστή grouped/summed
                UserDailyTotalsCashCountDetail userDeposit = null;
                UserDailyTotalsCashCountDetail userWithdraw = null;
                if (userDeposits.Count > 0)
                {
                    userDeposit = new UserDailyTotalsCashCountDetail(this.Session);
                    foreach (UserDailyTotalsCashCountDetail dtl in userDeposits)
                    {
                        userDeposit.Amount = userDeposit.Amount + dtl.Amount;
                        userDeposit.DocumentType = depositDocumentType;
                        userDeposit.CountedAmount = userDeposit.CountedAmount + dtl.CountedAmount;
                        userDeposit.QtyValue = userDeposit.QtyValue + dtl.QtyValue;
                        userDeposit.DetailType = dtl.DetailType;
                    }
                    userDeposit.Amount = userDeposit.Amount * depositDocumentType.ValueFactor;
                }
                if (userwithdrawls.Count > 0)
                {
                    userWithdraw = new UserDailyTotalsCashCountDetail(this.Session);
                    foreach (UserDailyTotalsCashCountDetail dtl in userwithdrawls)
                    {
                        userWithdraw.Amount = userWithdraw.Amount + dtl.Amount;
                        userWithdraw.DocumentType = withdrawlDocumentType;
                        userWithdraw.CountedAmount = userWithdraw.CountedAmount + dtl.CountedAmount;
                        userWithdraw.QtyValue = userWithdraw.QtyValue + dtl.QtyValue;
                        userWithdraw.DetailType = dtl.DetailType;
                    }
                    userWithdraw.Amount = userWithdraw.Amount * withdrawlDocumentType.ValueFactor;
                }
                //Βρίσκουμε το σύνολο των πληρωμών που θα συμμετέχουν στο αποτέλεσμα
                List<PaymentMethod> TotalPayments = new List<PaymentMethod>();
                List<UserDailyTotalsCashCountDetail> CashierGroupedPayments = new List<UserDailyTotalsCashCountDetail>();

                // οι πληρωμές του POS grouped/summed     
                List<UserDailyTotalsDetail> PosPayments = new XPCollection<UserDailyTotalsDetail>(this.Session, CriteriaOperator.And(
                                                                        new BinaryOperator("UserDailyTotals.Oid", userDailyTotals.Oid, BinaryOperatorType.Equal))).
                                                                        Where(x => x.Payment != null && x.DetailType == eDailyRecordTypes.PAYMENTS).ToList() ?? new List<UserDailyTotalsDetail>();

                // οι πληρωμές που καταχώρησε ο χειριστής
                List<UserDailyTotalsCashCountDetail> AllRegisterPayments = new List<UserDailyTotalsCashCountDetail>();
                AllRegisterPayments = userDailyTotals.UserDailyTotalsCashCountDetails.Where(x => x.Payment != null && x.DetailType == eCashCountRecordTypes.COUNTED_PAYMENTS)?.ToList()
                                                                                                                                                       ?? new List<UserDailyTotalsCashCountDetail>();
                // Group/Sum στις πληρωμές που καταχώρησε ο χειριστής
                foreach (UserDailyTotalsCashCountDetail pay in AllRegisterPayments)
                {
                    UserDailyTotalsCashCountDetail cashierPayment = null;
                    if (CashierGroupedPayments.Count == 0 || !CashierGroupedPayments.Select(x => x.Payment?.Oid).Contains(pay?.Payment?.Oid ?? Guid.NewGuid()))
                    {
                        cashierPayment = new UserDailyTotalsCashCountDetail(this.Session);
                        cashierPayment.Payment = pay.Payment;
                        cashierPayment.Amount = pay.Amount;
                        cashierPayment.CountedAmount = pay.CountedAmount;
                        cashierPayment.QtyValue = pay.QtyValue;
                        cashierPayment.DetailType = pay.DetailType;
                        CashierGroupedPayments.Add(cashierPayment);
                    }
                    else
                    {
                        cashierPayment = CashierGroupedPayments.Where(x => x.Payment != null && x.Payment.Oid == pay.Payment.Oid).FirstOrDefault();
                        if (cashierPayment != null)
                        {
                            cashierPayment.Amount = cashierPayment.Amount + pay.Amount;
                            cashierPayment.QtyValue = cashierPayment.QtyValue + pay.QtyValue;
                            cashierPayment.CountedAmount = cashierPayment.CountedAmount + pay.CountedAmount;
                        }
                    }
                }
                //Δημιουργόυμε τη λίστα με τους τύπους πληρωμής(καθε τύπος πληρωμής μπορει να συμμετέχει μόνο μία φορά)
                PosPayments.Select(x => x.Payment)?.ToList()?.ForEach(x => TotalPayments.Add(x));
                CashierGroupedPayments.Select(x => x.Payment)?.ToList()?.ForEach(x => { if (!TotalPayments.Contains(x)) { TotalPayments.Add(x); }; });


                List<PaymentMethod> PaysIncreasesDrawer = TotalPayments.Where(x => x.IncreasesDrawerAmount).ToList();
                List<PaymentMethod> PaysNonIncreasesDrawer = TotalPayments.Where(x => x.IncreasesDrawerAmount == false).ToList();

                //Για κάθε ένα τύπο πληρωμής δημιουργούμε ένα CashierDailyTotal που προστίθεται στη λίστα του αποτελέσματος
                foreach (PaymentMethod paymethod in PaysIncreasesDrawer)
                {
                    CashierShiftReportLine line = new CashierShiftReportLine();
                    line.Oid = Guid.NewGuid();
                    line.index = index;
                    line.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Payment;
                    line.Description = paymethod.Description;
                    UserDailyTotalsDetail dtlPayment = PosPayments.Where(x => x.Payment?.Oid == paymethod.Oid && eDailyRecordTypes.PAYMENTS == x.DetailType).FirstOrDefault();
                    UserDailyTotalsCashCountDetail dtlCashier = CashierGroupedPayments.Where(x => x.Payment?.Oid == paymethod.Oid && eCashCountRecordTypes.COUNTED_PAYMENTS == x.DetailType).FirstOrDefault();
                    line.PaymentMethod = paymethod;
                    line.DeviceAmount = dtlPayment?.Amount ?? 0;
                    line.DeviceQty = dtlPayment?.QtyValue ?? 0;
                    line.CashierQty = dtlCashier?.QtyValue ?? 0;
                    line.CashierAmount = dtlCashier?.Amount ?? 0;
                    if (line.PaymentMethod.IncreasesDrawerAmount)
                    {
                        line.DeviceDrawer = previousDrawerValue + line.DeviceAmount;
                        line.AfftectsDrawer = true;
                        line.DrawerDifference = (paymentReportLines.Where(x => x.index == (index - 1)).FirstOrDefault()?.DrawerDifference ?? 0) + (line.CashierAmount - line.DeviceAmount);
                    }
                    else
                    {
                        line.DeviceDrawer = previousDrawerValue;
                        line.DrawerDifference = 0;
                    }
                    previousDrawerValue = line.DeviceDrawer;
                    line.ValueDifference = line.CashierAmount - line.DeviceAmount;
                    line.QtyDifference = line.CashierQty - line.DeviceQty;
                    paymentReportLines.Add(line);
                    index++;
                }


                //Δημιουργούμε CashierDailyTotal για τις καταθέσεις αν χρειάζεται, που προστίθεται στη λίστα του αποτελέσματος
                if (posDeposit != null || userDeposit != null)
                {
                    CashierShiftReportLine depositPayment = new CashierShiftReportLine();
                    depositPayment.Oid = Guid.NewGuid();
                    depositPayment.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Payment;
                    depositPayment.index = index;
                    depositPayment.Description = depositDocumentType.Description;
                    depositPayment.DocumentType = depositDocumentType;
                    depositPayment.DeviceAmount = depositDocumentType.ValueFactor * (posDeposit?.Amount ?? 0);
                    depositPayment.DeviceQty = posDeposit?.QtyValue ?? 0;
                    depositPayment.DeviceDrawer = previousDrawerValue + (depositPayment?.DeviceAmount ?? 0);
                    previousDrawerValue = depositPayment.DeviceDrawer;
                    depositPayment.AfftectsDrawer = true;
                    depositPayment.CashierQty = userDeposit?.QtyValue ?? 0;
                    depositPayment.CashierAmount = userDeposit?.Amount ?? 0;
                    depositPayment.ValueDifference = depositPayment.CashierAmount - depositPayment.DeviceAmount;
                    depositPayment.QtyDifference = depositPayment.CashierQty - depositPayment.DeviceQty;
                    depositPayment.DrawerDifference = (paymentReportLines.Where(x => x.index == (index - 1)).FirstOrDefault()?.DrawerDifference ?? 0) + (depositPayment.CashierAmount - depositPayment.DeviceAmount);
                    paymentReportLines.Add(depositPayment);
                    index++;
                }
                //Δημιουργούμε CashierDailyTotal για τις αναλήψεις αν χρειάζεται,που προστίθεται στη λίστα του αποτελέσματος
                if (posWithdraw != null || userWithdraw != null)
                {
                    CashierShiftReportLine withdrawPayment = new CashierShiftReportLine();
                    withdrawPayment.Oid = Guid.NewGuid();
                    withdrawPayment.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Payment;
                    withdrawPayment.index = index;
                    withdrawPayment.Description = withdrawlDocumentType.Description;
                    withdrawPayment.DocumentType = withdrawlDocumentType;
                    withdrawPayment.DeviceAmount = withdrawlDocumentType.ValueFactor * (posWithdraw?.Amount ?? 0);
                    withdrawPayment.DeviceQty = posWithdraw?.QtyValue ?? 0;
                    withdrawPayment.DeviceDrawer = previousDrawerValue + (withdrawPayment?.DeviceAmount ?? 0);
                    previousDrawerValue = withdrawPayment.DeviceDrawer;
                    withdrawPayment.AfftectsDrawer = true;
                    withdrawPayment.CashierQty = userWithdraw?.QtyValue ?? 0;
                    withdrawPayment.CashierAmount = userWithdraw?.Amount ?? 0;
                    withdrawPayment.ValueDifference = withdrawPayment.CashierAmount - withdrawPayment.DeviceAmount;
                    withdrawPayment.QtyDifference = withdrawPayment.CashierQty - withdrawPayment.DeviceQty;
                    withdrawPayment.DrawerDifference = (paymentReportLines.Where(x => x.index == (index - 1)).FirstOrDefault()?.DrawerDifference ?? 0) + (withdrawPayment.CashierAmount - withdrawPayment.DeviceAmount);
                    paymentReportLines.Add(withdrawPayment);
                    index++;
                }

                //Για κάθε ένα τύπο πληρωμής δημιουργούμε ένα CashierDailyTotal που προστίθεται στη λίστα του αποτελέσματος
                foreach (PaymentMethod paymethod in PaysNonIncreasesDrawer)
                {
                    CashierShiftReportLine line = new CashierShiftReportLine();
                    line.Oid = Guid.NewGuid();
                    line.index = index;
                    line.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Payment;
                    line.Description = paymethod.Description;
                    UserDailyTotalsDetail dtlPayment = PosPayments.Where(x => x.Payment?.Oid == paymethod.Oid && eDailyRecordTypes.PAYMENTS == x.DetailType).FirstOrDefault();
                    UserDailyTotalsCashCountDetail dtlCashier = CashierGroupedPayments.Where(x => x.Payment?.Oid == paymethod.Oid && eCashCountRecordTypes.COUNTED_PAYMENTS == x.DetailType).FirstOrDefault();
                    line.PaymentMethod = paymethod;
                    line.DeviceAmount = dtlPayment?.Amount ?? 0;
                    line.DeviceQty = dtlPayment?.QtyValue ?? 0;
                    line.CashierQty = dtlCashier?.QtyValue ?? 0;
                    line.CashierAmount = dtlCashier?.Amount ?? 0;
                    if (line.PaymentMethod.IncreasesDrawerAmount)
                    {
                        line.DeviceDrawer = previousDrawerValue + line.DeviceAmount;
                        line.AfftectsDrawer = true;
                        line.DrawerDifference = (paymentReportLines.Where(x => x.index == (index - 1)).FirstOrDefault()?.DrawerDifference ?? 0) + (line.CashierAmount - line.DeviceAmount);
                    }
                    else
                    {
                        line.DeviceDrawer = previousDrawerValue;
                        line.DrawerDifference = 0;
                    }
                    previousDrawerValue = line.DeviceDrawer;
                    line.ValueDifference = line.CashierAmount - line.DeviceAmount;
                    line.QtyDifference = line.CashierQty - line.DeviceQty;
                    paymentReportLines.Add(line);
                    index++;
                }


                // Για όλες τις πληρωμές καταχωρούμε στο πεδίο FinalDrawer  το υπολογιζόμενο τελικό ποσό που περιέχει το συρτάρι
                paymentReportLines.ForEach(x => x.FinalDrawer = (paymentReportLines.Where(z => z.AfftectsDrawer && (z.PaymentMethod != null || z.DocumentType != null)).Sum(z => z.DeviceAmount)) + startingAmount);
                decimal finalDrawerDifference = paymentReportLines.Where(x => x.AfftectsDrawer).Sum(x => x.ValueDifference);
                paymentReportLines.ForEach(x => x.FinalDrawerDifference = finalDrawerDifference);
                shift.ShiftType = ShiftType.POS;
                if (shift.CashierShiftReportLines == null)
                {
                    shift.CashierShiftReportLines = new List<CashierShiftReportLine>();
                    shift.CashierShiftReportLines.AddRange(paymentReportLines);
                }
                else
                {
                    shift.CashierShiftReportLines.AddRange(paymentReportLines);
                }
                shift.UserDailyTotals = userDailyTotals;
                shift.User = this;
            }
            catch (Exception ex)
            {
                this.Session.RollbackTransaction();
                ///TO DO Να μπεί ο Nlogger στο project του Retail Model
                var er = ex.Message;
            }
            this.Session.RollbackTransaction();

            ///----------------------------------------------------------------------------Documents Section--------------------------------------------------------------------------------------------------------------------------------///
            ///
            List<CashierShiftReportLine> documentReportLines = new List<CashierShiftReportLine>();
            try
            {
                List<DocumentType> TotalDocuments = new List<DocumentType>();
                List<UserDailyTotalsCashCountDetail> CashierGroupedDocuments = new List<UserDailyTotalsCashCountDetail>();
                List<Guid> validDocTypes = new List<Guid>();
                List<UserDailyTotalsDetail> PosDocuments = new List<UserDailyTotalsDetail>();

                List<DocumentType> validTypes = new XPCollection<DocumentType>(this.Session, CriteriaOperator.And(new BinaryOperator("DisplayInCashCount", true, BinaryOperatorType.Equal))).ToList();
                foreach (DocumentType t in validTypes)
                {
                    if (t?.MinistryDocumentType?.Code != "173")
                    {
                        validDocTypes.Add(t.Oid);
                    }
                }
                PosDocuments = userDailyTotals.UserDailyTotalsDetails.Where(x => x.DocumentType != null && x.DocumentType != Guid.Empty && x.DetailType == eDailyRecordTypes.INVOICES &&
                                                                                  x.DocumentType != userDailyTotals.POS.DepositDocumentType.Oid && x.DocumentType != userDailyTotals.POS.WithdrawalDocumentType.Oid && x.Amount != 0 &&
                                                                                  validDocTypes.Contains(x.DocumentType))?.ToList() ?? new List<UserDailyTotalsDetail>();



                List<UserDailyTotalsCashCountDetail> AllRegisterDocuments = userDailyTotals.UserDailyTotalsCashCountDetails.Where(x => x.DocumentType != null
                                                                                        && x.DocumentType.Oid != userDailyTotals.POS.WithdrawalDocumentType.Oid
                                                                                          && x.DocumentType.Oid != userDailyTotals.POS.DepositDocumentType.Oid && x.DetailType
                                                                                       == eCashCountRecordTypes.COUNTED_DOCUMENTS)?.ToList() ?? new List<UserDailyTotalsCashCountDetail>();
                foreach (UserDailyTotalsCashCountDetail doc in AllRegisterDocuments)
                {
                    UserDailyTotalsCashCountDetail cashierDocument = null;
                    if (CashierGroupedDocuments.Count == 0 || !CashierGroupedDocuments.Select(x => x.DocumentType?.Oid).Contains(doc?.DocumentType?.Oid ?? Guid.NewGuid()))
                    {
                        cashierDocument = new UserDailyTotalsCashCountDetail(this.Session);
                        cashierDocument.DocumentType = doc.DocumentType;
                        cashierDocument.Amount = doc.Amount;
                        cashierDocument.CountedAmount = doc.CountedAmount;
                        cashierDocument.QtyValue = doc.QtyValue;
                        cashierDocument.DetailType = doc.DetailType;
                        CashierGroupedDocuments.Add(cashierDocument);
                    }
                    else
                    {
                        cashierDocument = CashierGroupedDocuments.Where(x => x.DocumentType != null && x.DocumentType.Oid != Guid.Empty && x.DocumentType.Oid == doc.DocumentType.Oid).FirstOrDefault();
                        if (cashierDocument != null)
                        {
                            cashierDocument.Amount = cashierDocument.Amount + doc.Amount;
                            cashierDocument.QtyValue = cashierDocument.QtyValue + doc.QtyValue;
                            cashierDocument.CountedAmount = cashierDocument.CountedAmount + doc.CountedAmount;
                        }
                    }
                }

                if (PosDocuments != null && PosDocuments.Count > 0)
                {
                    foreach (UserDailyTotalsDetail dtl in PosDocuments.Where(x => x.Amount != 0).ToList())
                    {
                        DocumentType type = this.Session.GetObjectByKey<DocumentType>(dtl.DocumentType);
                        TotalDocuments.Add(type);
                    }
                }

                if (CashierGroupedDocuments != null && CashierGroupedDocuments.Count > 0)
                {
                    foreach (UserDailyTotalsCashCountDetail dtl in CashierGroupedDocuments.ToList())
                    {
                        if (!TotalDocuments.Select(x => x.Oid).ToList().Contains(dtl.DocumentType.Oid))
                        {
                            TotalDocuments.Add(dtl.DocumentType);
                        }
                    }
                }

                foreach (DocumentType docType in TotalDocuments)
                {
                    CashierShiftReportLine cashierDocument = new CashierShiftReportLine();
                    cashierDocument.Oid = Guid.NewGuid();
                    cashierDocument.index = index;
                    cashierDocument.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Document;
                    UserDailyTotalsDetail dtlDocument = PosDocuments.Where(x => x.DocumentType == docType.Oid && eDailyRecordTypes.INVOICES == x.DetailType && x.Amount != 0).FirstOrDefault();
                    UserDailyTotalsCashCountDetail dtlCashier = CashierGroupedDocuments.Where(x => x.DocumentType.Oid == docType.Oid && eCashCountRecordTypes.COUNTED_DOCUMENTS == x.DetailType).FirstOrDefault();
                    cashierDocument.DocumentType = docType;
                    decimal docAmount = docType.ValueFactor * dtlDocument?.Amount ?? 0;
                    cashierDocument.DeviceAmount = docAmount;
                    cashierDocument.DeviceQty = dtlDocument?.QtyValue ?? 0;
                    cashierDocument.DeviceDrawer = 0;
                    cashierDocument.Description = docType.Description;
                    cashierDocument.CashierQty = dtlCashier?.QtyValue ?? 0;
                    cashierDocument.CashierAmount = (dtlCashier?.Amount ?? 0) * docType.ValueFactor;
                    cashierDocument.ValueDifference = cashierDocument.CashierAmount - cashierDocument.DeviceAmount;
                    cashierDocument.QtyDifference = cashierDocument.CashierQty - cashierDocument.DeviceQty;
                    cashierDocument.IsProforma = userDailyTotals.POS?.ProFormaInvoiceDocumentType?.Oid == dtlDocument?.DocumentType ? true : false;
                    documentReportLines.Add(cashierDocument);
                    index++;
                }
                documentReportLines.ForEach(x => x.FinalDrawer = (documentReportLines.Where(z => z.AfftectsDrawer).Sum(z => z.DeviceAmount)) + startingAmount);

                DateTime startDate = userDailyTotals.FiscalDate.Date;
                DateTime endDate = startDate.AddHours(23).AddMinutes(59);



                CriteriaOperator ValidDocHeaderCriteria = CriteriaOperator.And(
                 new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
                 new BinaryOperator("CreatedBy.Oid", userDailyTotals.User.Oid, BinaryOperatorType.Equal),
                 new BinaryOperator("UserDailyTotals.Oid", userDailyTotals.Oid, BinaryOperatorType.Equal),
                 new BinaryOperator("DocumentSeries.IsCancelingSeries", false, BinaryOperatorType.Equal),
                 new OperandProperty("GCRecord").IsNull(),
                 new OperandProperty("CanceledByDocument").IsNull(),
                 new BinaryOperator("Store.Oid", userDailyTotals.Store.Oid, BinaryOperatorType.Equal),
                 new BinaryOperator("FinalizedDate", startDate, BinaryOperatorType.GreaterOrEqual),
                 new BinaryOperator("FinalizedDate", endDate, BinaryOperatorType.LessOrEqual)
               );
                CriteriaOperator isProformaCrit = CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", settings.ProformaDocumentType.Oid, BinaryOperatorType.Equal));


                List<DocumentHeader> ProformasDocs = new List<DocumentHeader>();

                ProformasDocs = new XPCollection<DocumentHeader>(this.Session, CriteriaOperator.And(ValidDocHeaderCriteria, isProformaCrit)).ToList();

                List<Guid> PosProformaOids = new List<Guid>();
                PosProformaOids = ProformasDocs.Select(x => x.Oid).ToList();
                List<DocumentHeader> TransformedFromProformaHeaders = new List<DocumentHeader>();

                //Βρίσκουμε τα μετασχηματισμένα παραστατικά που προήλθαν από προτιμολόγια
                CriteriaOperator TransformedCriteria = CriteriaOperator.And(
                    new BinaryOperator("InitialDocument.DocumentType.Oid", userDailyTotals.POS.ProFormaInvoiceDocumentType.Oid, BinaryOperatorType.Equal),
                    new BinaryOperator("DerivedDocument.IsCanceled", false, BinaryOperatorType.Equal),
                    new BinaryOperator("DerivedDocument.DocumentNumber", 0, BinaryOperatorType.Greater),
                    new OperandProperty("DerivedDocument.CanceledByDocument").IsNull(),
                    new BinaryOperator("DerivedDocument.Store.Oid", userDailyTotals.Store.Oid, BinaryOperatorType.Equal),
                    new BetweenOperator("DerivedDocument.FinalizedDate", startDate, endDate),
                    new BinaryOperator("DerivedDocument.IsCanceled", false, BinaryOperatorType.Equal),
                    new InOperator("InitialDocument.Oid", PosProformaOids));

                if (PosProformaOids != null && PosProformaOids.Count > 0)
                {
                    int NonTransformedProforma = 0;
                    TransformedFromProformaHeaders = new XPCollection<RelativeDocument>(this.Session, TransformedCriteria).Select(x => x.DerivedDocument).ToList();
                    NonTransformedProforma = TransformedFromProformaHeaders == null ? PosProformaOids.Count : PosProformaOids.Count - TransformedFromProformaHeaders.Count;
                    if (NonTransformedProforma > 0)
                    {
                        CashierShiftReportLine cashierDocument = new CashierShiftReportLine();
                        cashierDocument.Oid = Guid.NewGuid();
                        cashierDocument.AfftectsDrawer = false;
                        cashierDocument.Description = ResourcesLib.Resources.NonTransformed;
                        cashierDocument.DeviceAmount = 0;
                        cashierDocument.DeviceQty = NonTransformedProforma;
                        cashierDocument.CashierAmount = 0;
                        cashierDocument.CashierQty = 0;
                        cashierDocument.QtyDifference = 0;
                        cashierDocument.ValueDifference = 0;
                        cashierDocument.LineType = eCashierLineType.Document;
                        cashierDocument.index = index;
                        cashierDocument.DeviceDrawer = 0;
                        cashierDocument.IsWarningRow = true;
                        documentReportLines.Add(cashierDocument);
                        index++;
                    }

                }

                if (shift.CashierShiftReportLines == null)
                {
                    shift.CashierShiftReportLines = new List<CashierShiftReportLine>();
                    shift.CashierShiftReportLines.AddRange(documentReportLines);
                }
                else
                {
                    shift.CashierShiftReportLines.AddRange(documentReportLines);
                }
                shift.ShiftSellValue = GetShiftSelledItemValue(shift, null);
                shift.StartingAmount = startingAmount;
                return shift;
            }
            catch (Exception ex)
            {
                var er = ex.Message;
                this.Session.RollbackTransaction();
            }
            this.Session.RollbackTransaction();
            return shift;
        }


        private CashierShift CreateCashierShiftFromReception(StoreControllerSettings settings, List<DocumentHeader> allInvoiceHeaders, List<DocumentHeader> TransformedFromProformaHeaders)
        {
            CashierShift shift = new CashierShift();
            List<Guid> transformedOids = new List<Guid>();
            transformedOids = TransformedFromProformaHeaders.Select(x => x.Oid).ToList();
            List<DocumentHeader> headersToCharge = allInvoiceHeaders.Where(z => !transformedOids.Contains(z.Oid)).ToList();
            List<CashierShiftReportLine> paymentReportLines = new List<CashierShiftReportLine>();

            decimal startingAmount = 0;
            decimal previousDrawerValue = 0;
            int index = 0;
            shift.Device = "StoreController";
            Dictionary<PaymentMethod, decimal> DocPayments = new Dictionary<PaymentMethod, decimal>();
            foreach (DocumentHeader header in headersToCharge)
            {
                foreach (DocumentPayment payment in header.DocumentPayments.Where(x => x.IsDeleted == false).OrderByDescending(x => x.PaymentMethod.IncreasesDrawerAmount))
                {
                    decimal amount = payment.Amount * header.DocumentType.ValueFactor;
                    if (!DocPayments.ContainsKey(payment.PaymentMethod))
                    {
                        DocPayments.Add(payment.PaymentMethod, amount);
                    }
                    else
                    {

                        DocPayments[payment.PaymentMethod] = DocPayments[payment.PaymentMethod] + amount;
                    }
                }
            }

            foreach (var payment in DocPayments)
            {
                CashierShiftReportLine pay = new CashierShiftReportLine();
                pay.Oid = Guid.NewGuid();
                pay.index = index;
                pay.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Payment;
                pay.Description = payment.Key.Description;
                pay.PaymentMethod = payment.Key;
                pay.DeviceAmount = payment.Value;
                pay.CashierQty = 0m;
                pay.CashierAmount = 0m;
                pay.DeviceQty = DocPayments.Where(x => x.Key.Oid == payment.Key.Oid)?.ToList()?.Count ?? 0;
                if (pay.PaymentMethod.IncreasesDrawerAmount)
                {
                    pay.DeviceDrawer = previousDrawerValue + pay.DeviceAmount;
                    pay.AfftectsDrawer = true;
                    pay.DrawerDifference = (paymentReportLines.Where(x => x.index == (index - 1)).FirstOrDefault()?.DrawerDifference ?? 0) + (pay.CashierAmount - pay.DeviceAmount);
                }
                else
                {
                    pay.DeviceDrawer = previousDrawerValue;
                    pay.DrawerDifference = 0;
                }
                previousDrawerValue = pay.DeviceDrawer;
                pay.ValueDifference = 0 - pay.DeviceAmount;
                pay.QtyDifference = 0 - pay.DeviceQty;
                paymentReportLines.Add(pay);
                index++;
            }

            paymentReportLines.ForEach(x => x.FinalDrawer = (paymentReportLines.Where(z => z.AfftectsDrawer).Sum(z => z.DeviceAmount)) + startingAmount);
            decimal finalDrawerDifference = paymentReportLines.Where(x => x.AfftectsDrawer).Sum(x => x.ValueDifference);
            paymentReportLines.ForEach(x => x.FinalDrawerDifference = finalDrawerDifference);
            if (shift.CashierShiftReportLines == null)
            {
                shift.CashierShiftReportLines = new List<CashierShiftReportLine>();
                shift.CashierShiftReportLines.AddRange(paymentReportLines);
            }
            else
            {
                shift.CashierShiftReportLines.AddRange(paymentReportLines);
            }

            List<CashierShiftReportLine> documentReportLines = new List<CashierShiftReportLine>();
            foreach (DocumentHeader header in headersToCharge)
            {
                if (!documentReportLines.Select(x => x.DocumentType.Oid).ToList().Contains(header.DocumentType.Oid))
                {
                    CashierShiftReportLine cashierDocument = new CashierShiftReportLine();
                    cashierDocument.Oid = Guid.NewGuid();
                    cashierDocument.index = index;
                    cashierDocument.LineType = ITS.Retail.Model.NonPersistant.eCashierLineType.Document;
                    cashierDocument.DocumentType = header.DocumentType;
                    decimal docAmount = header.DocumentType.ValueFactor * header.GrossTotal;
                    cashierDocument.DeviceAmount = docAmount;
                    cashierDocument.DeviceQty = 1;
                    cashierDocument.Description = header.DocumentType.Description;
                    cashierDocument.CashierQty = 0;
                    cashierDocument.CashierAmount = 0;
                    cashierDocument.ValueDifference = cashierDocument.CashierAmount - cashierDocument.DeviceAmount;
                    cashierDocument.QtyDifference = cashierDocument.CashierQty - cashierDocument.DeviceQty;
                    cashierDocument.IsProforma = settings.ProformaDocumentType?.Oid == header.DocumentType.Oid ? true : false;
                    documentReportLines.Add(cashierDocument);
                    index++;
                }
                else
                {
                    CashierShiftReportLine cashierDocument = documentReportLines.Where(x => x.DocumentType.Oid == header.DocumentType.Oid).FirstOrDefault();
                    decimal docAmount = header.DocumentType.ValueFactor * header.GrossTotal;
                    cashierDocument.DeviceAmount = cashierDocument.DeviceAmount + docAmount;
                    cashierDocument.DeviceQty = cashierDocument.DeviceQty + 1;
                    cashierDocument.CashierQty = 0;
                    cashierDocument.CashierAmount = 0;
                    cashierDocument.ValueDifference = cashierDocument.CashierAmount - cashierDocument.DeviceAmount;
                    cashierDocument.QtyDifference = cashierDocument.CashierQty - cashierDocument.DeviceQty;
                    cashierDocument.IsProforma = settings.ProformaDocumentType?.Oid == header.DocumentType.Oid ? true : false;
                }
            }

            if (shift.CashierShiftReportLines == null)
            {
                shift.CashierShiftReportLines = new List<CashierShiftReportLine>();
                shift.CashierShiftReportLines.AddRange(documentReportLines);
            }
            else
            {
                shift.CashierShiftReportLines.AddRange(documentReportLines);
            }

            shift.Oid = Guid.NewGuid();
            shift.ShiftType = ShiftType.RECEPTION;
            shift.ShiftSellValue = GetShiftSelledItemValue(null, headersToCharge);
            shift.User = this;
            shift.StartingAmount = startingAmount;
            return shift;
        }



        private decimal GetShiftSelledItemValue(CashierShift shift, List<DocumentHeader> headersToCharge)
        {
            List<Guid> ParticipatingDocTypes = new XPCollection<DocumentType>(this.Session, CriteriaOperator.And(new BinaryOperator("IncreaseVatAndSales", true, BinaryOperatorType.Equal)))?.Select(x => x.Oid)?.ToList() ?? new List<Guid>();

            Guid extraDoc = Guid.Empty;
            Guid.TryParse("23626947-bce4-49b5-a645-6a56bb399a84", out extraDoc);

            if (shift != null && shift.ShiftType == ShiftType.POS)
            {
                if (!ParticipatingDocTypes.Contains(shift.UserDailyTotals.POS.ProFormaInvoiceDocumentType.Oid))
                    ParticipatingDocTypes.Add(shift.UserDailyTotals.POS.ProFormaInvoiceDocumentType.Oid);

                if (extraDoc != Guid.Empty && !ParticipatingDocTypes.Contains(extraDoc))
                    ParticipatingDocTypes.Add(extraDoc);
                return shift.UserDailyTotals.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.INVOICES && ParticipatingDocTypes.Contains(x.DocumentType))?.Sum(x => x.Amount) ?? 0m;
            }
            else
            {
                return headersToCharge.Where(x => x.DocumentType.IncreaseVatAndSales || x.Oid == extraDoc)?.Sum(x => x.GrossTotal) ?? 0m;
            }

        }

        IRole IUser.Role
        {
            get
            {
                return this.Role;
            }
        }


    }
}
