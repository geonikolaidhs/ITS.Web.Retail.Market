using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.ViewModel
{
    public class CouponViewModel
    {
        public Guid Oid { get; set; }
        
        public Guid CouponMaskOid { get; set; }

        public string Code { get; set; }
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
        public int NumberOfTimesUsed { get; set; }
        public bool IsUnique { get; set; }
        public CouponAppliesOn CouponAppliesOn { get; set; }
        public CouponAmountType CouponAmountType { get; set; }
        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs { get; set; }
        public decimal Amount { get; set; }
        public Guid DiscountType { get; set; }
        public DateTime IsActiveFromDate { get; set; }
        public DateTime IsActiveUntilDate { get; set; }
        public Guid PaymentMethod { get; set; }
        public Guid CouponCategory { get; set; }
        public string CouponCategoryDescription { get; set; }

        public string PropertyName { get; set; }
        public string DecodedString { get; set; }

        public eDiscountType eDiscountType
        {
            get
            {
                switch (this.CouponAmountType)
                {
                    case CouponAmountType.PERCENTAGE:
                        return eDiscountType.PERCENTAGE;
                    case CouponAmountType.VALUE:
                        return eDiscountType.VALUE;
                    default:
                        return eDiscountType.VALUE;
                }
            }
        }

        /// <summary>
        /// Checks if the coupon can be used 
        /// </summary>
        public CouponCanBeUsedMessage CanBeUsed
        {
            get
            {
                long now = DateTime.Now.Ticks;
                if (now <= IsActiveFrom)
                {
                    return CouponCanBeUsedMessage.NOT_ACTIVE_YET;
                }

                if (IsActiveUntil <= now)
                {
                    return CouponCanBeUsedMessage.EXPIRED;
                }

                if (IsUnique && NumberOfTimesUsed > 0)
                {
                    return CouponCanBeUsedMessage.ALREADY_USED;
                }

                return CouponCanBeUsedMessage.USABLE;
            }
        }

        public bool IsValid(out string errorMessage)
        {
            if ((CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.DISCOUNT && DiscountType == null)
                || (CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.PAYMENT_METHOD && PaymentMethod == null))
            {
                errorMessage = "PLEASE_FILL_ALL_REQUIRED_FIELDS";
                return false;
            }
                       
            if (CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.PAYMENT_METHOD && CouponAppliesOn == CouponAppliesOn.ITEM)
            {
                errorMessage = "CannotApplyCouponAsPaymentMethodToItem";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
