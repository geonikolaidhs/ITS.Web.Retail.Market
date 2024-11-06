using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 900,
        Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("POS", typeof(ResourcesLib.Resources))]

    public class POS : Terminal
    {
        public POS() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POS(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.ReceiptVariableIdentifier = '@';
            this.CurrencySymbol = '€';
            this.CurrencyPattern = eCurrencyPattern.AFTER_NUMBER_WITH_SPACE;
            this._IsCashierRegister = false;
        }

        [Association("POS-UserDailyTotalss")]
        public XPCollection<UserDailyTotals> UserDailyTotalss
        {
            get
            {
                return GetCollection<UserDailyTotals>("UserDailyTotalss");
            }
        }

        [Association("POS-DailyTotalss")]
        public XPCollection<DailyTotals> DailyTotalss
        {
            get
            {
                return GetCollection<DailyTotals>("DailyTotalss");
            }
        }

        [Association("POS-ElectronicJournalFilePackages")]
        public XPCollection<ElectronicJournalFilePackage> ElectronicJournalFilePackages
        {
            get
            {
                return GetCollection<ElectronicJournalFilePackage>("ElectronicJournalFilePackages");
            }
        }



        private bool _PrintDiscountAnalysis;
        private DocumentSeries _ProFormaInvoiceDocumentSeries;
        private bool _UsesTouchScreen;
        private POSPrintFormat _ZFormat;
        private POSPrintFormat _XFormat;
        private POSPrintFormat _ReceiptFormat;
        private eCultureInfo _CultureInfo;
        private string _ABCDirectory;
        private char _ReceiptVariableIdentifier;
        private eFiscalDevice _FiscalDevice;
        private Customer _DefaultCustomer;
        private DocumentType _DefaultDocumentType;
        private DocumentType _ProFormaInvoiceDocumentType;
        private DocumentType _WithdrawalDocumentType;
        private DocumentType _DepositDocumentType;
        private DocumentSeries _WithdrawalDocumentSeries;
        private DocumentSeries _DepositDocumentSeries;
        private SpecialItem _WithdrawalItem;
        private SpecialItem _DepositItem;
        private DocumentSeries _DefaultDocumentSeries;
        private DocumentStatus _DefaultDocumentStatus;
        private PaymentMethod _DefaultPaymentMethod;
        private POSKeysLayout _POSKeysLayout;
        private POSLayout _POSLayout;
        private bool _UsesKeyLock;
        private POSActionLevelsSet _POSActionLevelsSet;
        private char _CurrencySymbol;
        private eCurrencyPattern _CurrencyPattern;
        private bool _AutoFocus;
        private bool _AutoIssueZEAFDSS;
        private bool _AsksForStartingAmount;
        private bool _EnableLowEndMode;
        private bool _DemoMode;
        private bool _AsksForFinalAmount;
        private bool _UseSliderPauseForm;
        private eForcedWithdrawMode _ForcedWithdrawMode;
        private decimal _ForcedWithdrawCashAmountLimit;
        private string _StandaloneFiscalOnErrorMessage;
        private DocumentType _SpecialProformaDocumentType;
        private DocumentSeries _SpecialProformaDocumentSeries;
        private bool _OnIssueXClosePacketOnCreditDevice;
        private bool _IsCashierRegister;
        private bool _UseCashCounter;

        public bool IsAlive
        {
            get
            {
                return (DateTime.Now.Ticks - Status.MachineStatusTicks < 30 * TimeSpan.TicksPerSecond);
            }
        }

        public Customer DefaultCustomer
        {
            get
            {
                return _DefaultCustomer;
            }
            set
            {
                SetPropertyValue("DefaultCustomer", ref _DefaultCustomer, value);
            }
        }

        public POSStatus Status
        {
            get
            {
                POSStatus posStatus = Session.FindObject<POSStatus>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("POSOid", Oid));
                if (posStatus == null)
                {
                    posStatus = new POSStatus(Session);
                    posStatus.POSOid = Oid;
                    posStatus.MachineStatus = eMachineStatus.UNKNOWN;
                    posStatus.Save();
                }
                return posStatus;
            }
        }

        public DocumentType DefaultDocumentType
        {
            get
            {
                return _DefaultDocumentType;
            }
            set
            {
                SetPropertyValue("DefaultDocumentType", ref _DefaultDocumentType, value);
            }
        }

        public DocumentType WithdrawalDocumentType
        {
            get
            {
                return _WithdrawalDocumentType;
            }
            set
            {
                SetPropertyValue("WithdrawalDocumentType", ref _WithdrawalDocumentType, value);
            }
        }

        public DocumentType DepositDocumentType
        {
            get
            {
                return _DepositDocumentType;
            }
            set
            {
                SetPropertyValue("DepositDocumentType", ref _DepositDocumentType, value);
            }
        }

        public DocumentType ProFormaInvoiceDocumentType
        {
            get
            {
                return _ProFormaInvoiceDocumentType;
            }
            set
            {
                SetPropertyValue("ProFormaInvoiceDocumentType", ref _ProFormaInvoiceDocumentType, value);
            }
        }


        public DocumentSeries ProFormaInvoiceDocumentSeries
        {
            get
            {
                return _ProFormaInvoiceDocumentSeries;
            }
            set
            {
                SetPropertyValue("ProFormaInvoiceDocumentSeries", ref _ProFormaInvoiceDocumentSeries, value);
            }
        }

        public DocumentSeries WithdrawalDocumentSeries
        {
            get
            {
                return _WithdrawalDocumentSeries;
            }
            set
            {
                SetPropertyValue("WithdrawalDocumentSeries", ref _WithdrawalDocumentSeries, value);
            }
        }

        public DocumentSeries DepositDocumentSeries
        {
            get
            {
                return _DepositDocumentSeries;
            }
            set
            {
                SetPropertyValue("DepositDocumentSeries", ref _DepositDocumentSeries, value);
            }
        }

        public SpecialItem WithdrawalItem
        {
            get
            {
                return _WithdrawalItem;
            }
            set
            {
                SetPropertyValue("WithdrawalItem", ref _WithdrawalItem, value);
            }
        }

        public SpecialItem DepositItem
        {
            get
            {
                return _DepositItem;
            }
            set
            {
                SetPropertyValue("DepositItem", ref _DepositItem, value);
            }
        }


        public DocumentSeries DefaultDocumentSeries
        {
            get
            {
                return _DefaultDocumentSeries;
            }
            set
            {
                SetPropertyValue("DefaultDocumentSeries", ref _DefaultDocumentSeries, value);
            }
        }

        public bool UsesKeyLock
        {
            get
            {
                return _UsesKeyLock;
            }
            set
            {
                SetPropertyValue("UsesKeyLock", ref _UsesKeyLock, value);
            }
        }

        public bool UsesTouchScreen
        {
            get
            {
                return _UsesTouchScreen;
            }
            set
            {
                SetPropertyValue("UsesTouchScreen", ref _UsesTouchScreen, value);
            }
        }

        public bool OnIssueXClosePacketOnCreditDevice
        {
            get
            {
                return _OnIssueXClosePacketOnCreditDevice;
            }
            set
            {
                SetPropertyValue("OnIssueXClosePacketOnCreditDevice", ref _OnIssueXClosePacketOnCreditDevice, value);
            }
        }



        public bool AutoFocus
        {
            get
            {
                return _AutoFocus;
            }
            set
            {
                SetPropertyValue("AutoFocus", ref _AutoFocus, value);
            }
        }

        public bool AsksForStartingAmount
        {
            get
            {
                return _AsksForStartingAmount;
            }
            set
            {
                SetPropertyValue("AsksForStartingAmount", ref _AsksForStartingAmount, value);
            }
        }

        public bool AsksForFinalAmount
        {
            get
            {
                return _AsksForFinalAmount;
            }
            set
            {
                SetPropertyValue("AsksForFinalAmount", ref _AsksForFinalAmount, value);
            }
        }


        public bool PrintDiscountAnalysis
        {
            get
            {
                return _PrintDiscountAnalysis;
            }
            set
            {
                SetPropertyValue("PrintDiscountAnalysis", ref _PrintDiscountAnalysis, value);
            }
        }

        public bool AutoIssueZEAFDSS
        {
            get
            {
                return _AutoIssueZEAFDSS;
            }
            set
            {
                SetPropertyValue("AutoIssueZEAFDSS", ref _AutoIssueZEAFDSS, value);
            }
        }

        public bool EnableLowEndMode
        {
            get
            {
                return _EnableLowEndMode;
            }
            set
            {
                SetPropertyValue("EnableLowEndMode", ref _EnableLowEndMode, value);
            }
        }


        public DocumentStatus DefaultDocumentStatus
        {
            get
            {
                return _DefaultDocumentStatus;
            }
            set
            {
                SetPropertyValue("DefaultDocumentStatus", ref _DefaultDocumentStatus, value);
            }
        }

        public PaymentMethod DefaultPaymentMethod
        {
            get
            {
                return _DefaultPaymentMethod;
            }
            set
            {
                SetPropertyValue("DefaultPaymentMethod", ref _DefaultPaymentMethod, value);
            }
        }

        public bool DemoMode
        {
            get
            {
                return _DemoMode;
            }
            set
            {
                SetPropertyValue("DemoMode", ref _DemoMode, value);
            }
        }


        public eForcedWithdrawMode ForcedWithdrawMode
        {
            get
            {
                return _ForcedWithdrawMode;
            }
            set
            {
                SetPropertyValue("ForcedWithdrawMode", ref _ForcedWithdrawMode, value);
            }
        }

        public decimal ForcedWithdrawCashAmountLimit
        {
            get
            {
                return _ForcedWithdrawCashAmountLimit;
            }
            set
            {
                SetPropertyValue("ForcedWithdrawCashAmountLimit", ref _ForcedWithdrawCashAmountLimit, value);
            }
        }

        [Association("POS-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

        public eFiscalDevice FiscalDevice
        {
            get
            {
                return _FiscalDevice;
            }
            set
            {
                SetPropertyValue("FiscalDevice", ref _FiscalDevice, value);
            }
        }

        public char ReceiptVariableIdentifier
        {
            get
            {
                return _ReceiptVariableIdentifier;
            }
            set
            {
                SetPropertyValue("ReceiptVariableIdentifier", ref _ReceiptVariableIdentifier, value);
            }
        }

        public char CurrencySymbol
        {
            get
            {
                return _CurrencySymbol;
            }
            set
            {
                SetPropertyValue("CurrencySymbol", ref _CurrencySymbol, value);
            }
        }

        public eCurrencyPattern CurrencyPattern
        {
            get
            {
                return _CurrencyPattern;
            }
            set
            {
                SetPropertyValue("CurrencyPattern", ref _CurrencyPattern, value);
            }
        }

        public string ABCDirectory
        {
            get
            {
                return _ABCDirectory;
            }
            set
            {
                SetPropertyValue("ABCDirectory", ref _ABCDirectory, value);
            }
        }

        public string StandaloneFiscalOnErrorMessage
        {
            get
            {
                return _StandaloneFiscalOnErrorMessage;
            }
            set
            {
                SetPropertyValue("StandaloneFiscalOnErrorMessage", ref _StandaloneFiscalOnErrorMessage, value);
            }
        }

        public eCultureInfo CultureInfo
        {
            get
            {
                return _CultureInfo;
            }
            set
            {
                SetPropertyValue("CultureInfo", ref _CultureInfo, value);
            }
        }

        public POSPrintFormat ReceiptFormat
        {
            get
            {
                return _ReceiptFormat;
            }
            set
            {
                SetPropertyValue("ReceiptFormat", ref _ReceiptFormat, value);
            }
        }

        public POSPrintFormat XFormat
        {
            get
            {
                return _XFormat;
            }
            set
            {
                SetPropertyValue("XFormat", ref _XFormat, value);
            }
        }

        public POSPrintFormat ZFormat
        {
            get
            {
                return _ZFormat;
            }
            set
            {
                SetPropertyValue("ZFormat", ref _ZFormat, value);
            }
        }

        [Association("POS-POSKeysLayouts")]
        public POSKeysLayout POSKeysLayout
        {
            get
            {
                return _POSKeysLayout;
            }
            set
            {
                SetPropertyValue("POSKeysLayout", ref _POSKeysLayout, value);
            }
        }

        [Association("POS-POSLayouts")]
        public POSLayout POSLayout
        {
            get
            {
                return _POSLayout;
            }
            set
            {
                SetPropertyValue("POSLayout", ref _POSLayout, value);
            }
        }

        [Association("POSs-POSActionLevelsSet")]
        public POSActionLevelsSet POSActionLevelsSet
        {
            get
            {
                return _POSActionLevelsSet;
            }
            set
            {
                SetPropertyValue("POSActionLevelsSet", ref _POSActionLevelsSet, value);
            }
        }

        [Association("POS-StoreDocumentSeriesType")]
        public XPCollection<POSDocumentSeries> POSDocumentSeries
        {
            get
            {
                return GetCollection<POSDocumentSeries>("POSDocumentSeries");
            }
        }


        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if ((ReceiptFormat != null && ReceiptFormat.FormatType != eFormatType.Receipt)
                || (XFormat != null && XFormat.FormatType != eFormatType.Receipt)
                || (ZFormat != null && ZFormat.FormatType != eFormatType.Z)
                )
            {
                string exception_message = "";
                if (ReceiptFormat != null && ReceiptFormat.FormatType != eFormatType.Receipt)
                {
                    exception_message += "Invalid Receipt Format" + Environment.NewLine;
                }
                if (XFormat != null && XFormat.FormatType != eFormatType.X)
                {
                    exception_message += "Invalid X Format" + Environment.NewLine;
                }
                if (ZFormat != null && ZFormat.FormatType != eFormatType.Z)
                {
                    exception_message += "Invalid Z Format" + Environment.NewLine;
                }

                if (!String.IsNullOrWhiteSpace(exception_message))
                {
                    throw new Exception(exception_message);
                }
            }
            base.OnSaving();
        }

        public DocumentType SpecialProformaDocumentType
        {
            get
            {
                return _SpecialProformaDocumentType;
            }
            set
            {
                SetPropertyValue("SpecialProformaDocumentType", ref _SpecialProformaDocumentType, value);
            }
        }

        public DocumentSeries SpecialProformaDocumentSeries
        {
            get
            {
                return _SpecialProformaDocumentSeries;
            }
            set
            {
                SetPropertyValue("SpecialProformaDocumentSeries", ref _SpecialProformaDocumentSeries, value);
            }
        }
        public bool IsCashierRegister
        {
            get
            {
                return _IsCashierRegister;
            }
            set
            {
                _IsCashierRegister = value;
            }
        }

        public bool UseSliderPauseForm
        {
            get
            {
                return _UseSliderPauseForm;
            }
            set
            {
                SetPropertyValue("UseSliderPauseForm", ref _UseSliderPauseForm, value);
            }

        }
        public bool UseCashCounter
        {
            get
            {
                return _UseCashCounter;
            }
            set
            {
                SetPropertyValue("UseCashCounter", ref _UseCashCounter, value);
            }
        }


        [Association("POS-POSReportSettings")]
        public XPCollection<POSReportSetting> POSReportSettings
        {
            get
            {
                return GetCollection<POSReportSetting>("POSReportSettings");
            }
        }

        [UpdaterIgnoreField]
        [Association("POS-PosOposReportSettings")]
        public XPCollection<PosOposReportSettings> PosOposReportSettings
        {
            get
            {
                return GetCollection<PosOposReportSettings>("PosOposReportSettings");
            }
        }
    }
}
