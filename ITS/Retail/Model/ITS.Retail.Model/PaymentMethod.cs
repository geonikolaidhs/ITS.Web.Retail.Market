//-----------------------------------------------------------------------
// <copyright file="PaymentMethod.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 185,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PaymentMethod", typeof(ResourcesLib.Resources))]

    public class PaymentMethod : Lookup2Fields
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("PaymentMethod.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private bool _IsNegative;
        private bool _UseEDPS;
        private bool _GiveChange;
        private ePaymentMethodType _PaymentMethodType;
        private bool _OpensDrawer;
        private bool _IncreasesDrawerAmount;
        private bool _UsesInstallments;
        private bool _ForceEdpsOffline;
        private bool _NeedsValidation;
        private bool _NeedsRatification;
        private bool _CanExceedTotal;
        private bool _AffectsCustomerBalance;
        private bool _UseCardlink;
        private bool _DisplayInCashCount;
        private bool _HandelsCurrencies;
        private int _CashierDeviceCode;

        [DisplayOrder(Order = 8)]
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

        
        [DisplayOrder(Order = 7)]
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

        
        [DisplayOrder(Order = 6)]
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


        
        [DisplayOrder(Order = 4)]
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

        
        [DisplayOrder(Order = 5)]
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


        [DisplayOrder(Order = 6)]
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

       


        [Association("PaymentMethod-UserDailyTotalsDetails")]
        public XPCollection<UserDailyTotalsDetail> UserDailyTotalsDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsDetail>("UserDailyTotalsDetails");
            }
        }

        [Association("PaymentMethod-UserDailyTotalsCashCountDetails")]
        public XPCollection<UserDailyTotalsCashCountDetail> UserDailyTotalsCashCountDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsCashCountDetail>("UserDailyTotalsCashCountDetails");
            }
        }

        [Association("PaymentMethod-DailyTotalsDetails")]
        public XPCollection<DailyTotalsDetail> DailyTotalsDetails
        {
            get
            {
                return GetCollection<DailyTotalsDetail>("DailyTotalsDetails");
            }
        }

        [Association("PaymentMethod-DocumentPayments")]
        public XPCollection<DocumentPayment> DocumentPayments
        {
            get
            {
                return GetCollection<DocumentPayment>("DocumentPayments");
            }
        }

        [Aggregated, Association("PaymentMethod-PaymentMethodFields")]
        public XPCollection<PaymentMethodField> PaymentMethodFields
        {
            get
            {
                return GetCollection<PaymentMethodField>("PaymentMethodFields");
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

        public bool AffectsCustomerBalance
        {
            get
            {
                return _AffectsCustomerBalance;
            }
            set
            {
                SetPropertyValue("AffectsCustomerBalance", ref _AffectsCustomerBalance, value);
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
        public int CashierDeviceCode
        {
            get
            {
                return _CashierDeviceCode;
            }
            set
            {
                SetPropertyValue("CashierDeviceCode", ref _CashierDeviceCode, value);
            }
        }
    }

}