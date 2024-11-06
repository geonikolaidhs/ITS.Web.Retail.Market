//-----------------------------------------------------------------------
// <copyright file="DailyTotalsDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Data;

namespace ITS.Retail.Model
{
    public class DailyTotalsDetail : TotalDetail
    {
        public DailyTotalsDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DailyTotalsDetail(Session session)
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


        private DailyTotals _DailyTotals;
        [Association("DailyTotals-DailyTotalsDetails")]
        public DailyTotals DailyTotals
        {
            get
            {
                return _DailyTotals;
            }
            set
            {
                SetPropertyValue("DailyTotals", ref _DailyTotals, value);
            }
        }


        //private eDailyRecordsTypes _DetailType;
        //public eDailyRecordsTypes DetailType
        //{
        //    get
        //    {
        //        return _DetailType;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DetailType", ref _DetailType, value);
        //    }
        //}
        //private DocumentType _DocumentType;
        //public DocumentType DocumentType
        //{
        //    get
        //    {
        //        return _DocumentType;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DocumentType", ref _DocumentType, value);
        //    }
        //}


        //private VatCategory _VatCategory;
        //[Association("VatCategory-DailyTotalsDetails")]
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
        [Association("VatFactor-DailyTotalsDetails")]
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
        [Association("PaymentMethod-DailyTotalsDetails")]
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
        //    DailyTotalsDetail udtd = item as DailyTotalsDetail;
        //    DailyTotals = GetLookupObject<DailyTotals>(myses, udtd.DailyTotals) as DailyTotals;
        //    //VatCategory = GetLookupObject<VatCategory>(myses, udtd.VatCategory) as VatCategory;
        //    VatFactor = GetLookupObject<VatFactor>(myses, udtd.VatFactor) as VatFactor;
        //    Payment = GetLookupObject<PaymentMethod>(myses, udtd.Payment) as PaymentMethod;
        //    //DetailType = udtd.DetailType;
        //    //QtyValue = udtd.QtyValue;
        //    //Amount = udtd.Amount;
        //}

    }
}
