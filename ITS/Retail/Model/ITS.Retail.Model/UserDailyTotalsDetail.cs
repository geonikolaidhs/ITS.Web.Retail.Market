//-----------------------------------------------------------------------
// <copyright file="UserDailyTotalsDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Xpo;
using System.Data;

namespace ITS.Retail.Model
{    
    public class UserDailyTotalsDetail : TotalDetail
    {
        public UserDailyTotalsDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserDailyTotalsDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private UserDailyTotals _UserDailyTotals;
        [Association("UserDailyTotals-UserDailyTotalsDetails")]
        public UserDailyTotals UserDailyTotals
        {
            get
            {
                return _UserDailyTotals;
            }
            set
            {
                SetPropertyValue("UserDailyTotals", ref _UserDailyTotals, value);
            }
        }


        //private VatCategory _VatCategory;
        //[Association("VatCategory-UserDailyTotalsDetails")]
        //public VatCategory VatCategory
        //{
        //    get
        //    {
        //        return _VatCategory;
        //    }
        //    set
        //    {
        //        SetPropertyValue("VatCategory", ref _VatCategory, value);
        //    }
        //}

        private VatFactor _VatFactor;
        [Association("VatFactor-UserDailyTotalsDetails")]
        public VatFactor VatFactor
        {
            get
            {
                return _VatFactor;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatFactor, value);
            }
        }

        private PaymentMethod _Payment;
        [Association("PaymentMethod-UserDailyTotalsDetails")]
        public PaymentMethod Payment
        {
            get
            {
                return _Payment;
            }
            set
            {
                SetPropertyValue("Payment", ref _Payment, value);
            }
        }

        //private double _QtyValue;
        //public double QtyValue
        //{
        //    get
        //    {
        //        return _QtyValue;
        //    }
        //    set
        //    {
        //        SetPropertyValue("QtyValue", ref _QtyValue, value);
        //    }
        //}

        //private double _Amount;
        //public double Amount
        //{
        //    get
        //    {
        //        return _Amount;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Amount", ref _Amount, value);
        //    }
        //}


        //public override void GetData(Session myses, object item)
        //{
        //    base.GetData(myses, item);
        //    UserDailyTotalsDetail udtd = item as UserDailyTotalsDetail;
        //    UserDailyTotals = GetLookupObject<UserDailyTotals>(myses, udtd.UserDailyTotals) as UserDailyTotals;
        //   // VatCategory = GetLookupObject<VatCategory>(myses, udtd.VatCategory) as VatCategory;
        //    VatFactor = GetLookupObject<VatFactor>(myses, udtd.VatFactor) as VatFactor;
        //    Payment = GetLookupObject<PaymentMethod>(myses, udtd.Payment) as PaymentMethod;
        //    //DetailType = udtd.DetailType;
        //   // QtyValue = udtd.QtyValue;
        //   // Amount = udtd.Amount;
        //}

    }
}
