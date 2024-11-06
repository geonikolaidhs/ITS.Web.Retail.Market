using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Settings
{
    public class PaymentMethod : Lookup2Fields, IPaymentMethod
    {
        public PaymentMethod()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PaymentMethod(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public PaymentMethod(string code, string description)
            : base(code, description)
        {
        }
        public PaymentMethod(Session session, string code, string description)
            : base(session, code, description)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private bool _UsesInstallments;
        private bool _UseEDPS;
        private bool _GiveChange;
        private bool _OpensDrawer;
        private bool _NeedsRatification;
        private bool _IncreasesDrawerAmount;
        private ePaymentMethodType _PaymentMethodType;
        private bool _IsNegative;
        private bool _NeedsValidation;
        private bool _CanExceedTotal;
        private bool _ForceEdpsOffline;
        private bool _UseCardlink;
        private bool _DisplayInCashCount;
        private bool _HandelsCurrencies;
        public bool GiveChange
        {
            get
            {
                return _GiveChange;
            }
            set
            {
                SetPropertyValue("GiveChange", ref _GiveChange, value);
            }
        }


        public bool CanExceedTotal
        {
            get
            {
                return _CanExceedTotal;
            }
            set
            {
                SetPropertyValue("CanExceedTotal", ref _CanExceedTotal, value);
            }
        }


        public bool NeedsValidation
        {
            get
            {
                return _NeedsValidation;
            }
            set
            {
                SetPropertyValue("NeedsValidation", ref _NeedsValidation, value);
            }
        }

        
        public bool NeedsRatification
        {
            get
            {
                return _NeedsRatification;
            }
            set
            {
                SetPropertyValue("NeedsRatification", ref _NeedsRatification, value);
            }
        }


        public bool OpensDrawer
        {
            get
            {
                return _OpensDrawer;
            }
            set
            {
                SetPropertyValue("OpensDrawer", ref _OpensDrawer, value);
            }
        }

        public bool IncreasesDrawerAmount
        {
            get
            {
                return _IncreasesDrawerAmount;
            }
            set
            {
                SetPropertyValue("IncreasesDrawerAmount", ref _IncreasesDrawerAmount, value);
            }
        }


        
        public ePaymentMethodType PaymentMethodType
        {
            get
            {
                return _PaymentMethodType;
            }
            set
            {
                SetPropertyValue("PaymentMethodType", ref _PaymentMethodType, value);
            }
        }



        public bool UseEDPS
        {
            get
            {
                return _UseEDPS;
            }
            set
            {
                SetPropertyValue("UseEDPS", ref _UseEDPS, value);
            }
        }

        public bool IsNegative
        {
            get
            {
                return _IsNegative;
            }
            set
            {
                SetPropertyValue("IsNegative", ref _IsNegative, value);
            }
        }


        public bool UsesInstallments
        {
            get
            {
                return _UsesInstallments;
            }
            set
            {
                SetPropertyValue("UsesInstallments", ref _UsesInstallments, value);
            }
        }

        public bool ForceEdpsOffline
        {
            get
            {
                return _ForceEdpsOffline;
            }
            set
            {
                SetPropertyValue("ForceEdpsOffline", ref _ForceEdpsOffline, value);
            }
        }

        public bool UseCardlink
        {
            get
            {
                return _UseCardlink;
            }
            set
            {
                SetPropertyValue("UseCardlink", ref _UseCardlink, value);
            }
        }
        public bool DisplayInCashCount
        {
            get
            {
                return _DisplayInCashCount;
            }
            set
            {
                SetPropertyValue("DisplayInCashCount", ref _DisplayInCashCount, value);
            }
        }
        public bool HandelsCurrencies
        {
            get
            {
                return _HandelsCurrencies;
            }
            set
            {
                SetPropertyValue("HandelsCurrencies", ref _HandelsCurrencies, value);
            }
        }
        
    }
}
