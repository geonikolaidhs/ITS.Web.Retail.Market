using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class CouponMaskViewModel : IPersistableViewModel
    {
        public Guid Oid { get; set; }


        public Type PersistedType
        {
            get
            {
                return typeof(CouponMask);
            }
        }


        public Guid Owner { get; set; }
        public string Description { get; set; }
        public long IsActiveUntil
        {
            get
            {
                return IsActiveUntilDate.Ticks;
            }
        }
        public long IsActiveFrom
        {
            get
            {
                return IsActiveFromDate.Ticks;
            }
        }


        public bool IsUnique { get; set; }
        public CouponAppliesOn CouponAppliesOn { get; set; }
        public CouponAmountType CouponAmountType { get; set; }
        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs { get; set; }
        public Guid DiscountType { get; set; }
        public DateTime IsActiveFromDate { get; set; }
        public DateTime IsActiveUntilDate { get; set; }
        public Guid PaymentMethod { get; set; }
        public string Mask { get; set; }
        public string Prefix { get; set; }

        public string EntityName
        {
            get
            {
                switch (CouponAmountIsAppliedAs)
                {
                    case Platform.Enumerations.CouponAmountIsAppliedAs.DISCOUNT:
                        return typeof(DocumentDetailDiscount).FullName;
                    case Platform.Enumerations.CouponAmountIsAppliedAs.PAYMENT_METHOD:
                        return typeof(PaymentMethod).FullName;
                    case Platform.Enumerations.CouponAmountIsAppliedAs.POINTS:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }


        public string PropertyName { get; set; }

        public bool IsDeleted { get; set; }

        public void UpdateModel(DevExpress.Xpo.Session uow)
        {
            this.UpdateProperties(uow);
        }

        public bool Validate(out string message)
        {
            message = string.Empty;
            return true;
        }
    }
}