using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System.ComponentModel.DataAnnotations;
using ITS.Retail.Platform;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Common.ViewModel
{
    public class MassivelyCreateCouponsViewModel : BasicViewModel
    {
        private bool _IsUnique;
        private DateTime _IsActiveUntilDate;
        private DateTime _IsActiveFromDate;
        private int _EndingNumber;
        private int _StartingNumber;
        private string _Prefix;
        private string _Suffix;
        private CouponAppliesOn _CouponAppliesOn;
        private CouponAmountType _CouponAmountType;
        private CouponAmountIsAppliedAs _CouponAmountIsAppliedAs;
        private decimal _Amount;
        private DiscountType _DiscountType;
        private PaymentMethod _PaymentMethod;
        private CouponCategory _CouponCategory;
        private bool _UsePadding;
        private int _PaddingLength;
        private string _PaddingCharacter;
        private Platform.Enumerations.PaddingDirection _PaddingDirection;
        

        [Required]
        public int StartingNumber
        {
            get
            {
                return _StartingNumber;
            }
            set
            {
                SetPropertyValue("StartingNumber", ref _StartingNumber, value);
            }
        }

        [Required]
        [CompareTo(ErrorMessageResourceName="Greater", ErrorMessageResourceType = typeof(ResourcesLib.Resources),
            OtherProperty = "StartingNumber", OperatorType = CompareOperatorType.GREATER | CompareOperatorType.EQUAL) ]
        public int EndingNumber
        {
            get
            {
                return _EndingNumber;
            }
            set
            {
                SetPropertyValue("EndingNumber", ref _EndingNumber, value);
            }
        }

        [Required]
        public DateTime IsActiveFromDate
        {
            get
            {
                return _IsActiveFromDate;
            }
            set
            {
                SetPropertyValue("IsActiveFromDate", ref _IsActiveFromDate, value);
            }
        }

        [Required]
        [CompareTo(ErrorMessageResourceName = "EndDateShouldBeGreaterThanStartDate", ErrorMessageResourceType = typeof(ResourcesLib.Resources),
            OtherProperty = "IsActiveFromDate", OperatorType = CompareOperatorType.GREATER | CompareOperatorType.EQUAL)]
        public DateTime IsActiveUntilDate
        {
            get
            {
                
                return _IsActiveUntilDate;
            }
            set
            {
                SetPropertyValue("IsActiveUntilDate", ref _IsActiveUntilDate, value);
            }
        }

        [Required]
        public bool IsUnique
        {
            get
            {
                return _IsUnique;
            }
            set
            {
                SetPropertyValue("IsUnique", ref _IsUnique, value);
            }
        }

        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                SetPropertyValue("Prefix", ref _Prefix, value);
            }
        }

        public string Suffix
        {
            get
            {
                return _Suffix;
            }
            set
            {
                SetPropertyValue("Suffix", ref _Suffix, value);
            }
        }

        [Required]
        public CouponAppliesOn CouponAppliesOn
        {
            get
            {
                return _CouponAppliesOn;
            }
            set
            {
                SetPropertyValue("CouponAppliesOn", ref _CouponAppliesOn, value);
            }
        }

        [Required]
        public CouponAmountType CouponAmountType
        {
            get
            {
                return _CouponAmountType;
            }
            set
            {
                SetPropertyValue("CouponAmountType", ref _CouponAmountType, value);
            }
        }

        [Required]
        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs
        {
            get
            {
                return _CouponAmountIsAppliedAs;
            }
            set
            {
                SetPropertyValue("CouponAmountIsAppliedAs", ref _CouponAmountIsAppliedAs, value);
            }
        }

        [Required]
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }
                
        public DiscountType DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }

        public PaymentMethod PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }

        [Required]
        public CouponCategory CouponCategory
        {
            get
            {
                return _CouponCategory;
            }
            set
            {
                SetPropertyValue("CouponCategory", ref _CouponCategory, value);
            }
        }

        [Required]
        public bool UsePadding
        {
            get
            {
                return _UsePadding;
            }
            set
            {
                SetPropertyValue("UsePadding", ref _UsePadding, value);
            }
        }

        public int PaddingLength
        {
            get
            {
                return _PaddingLength;
            }
            set
            {
                SetPropertyValue("PaddingLength", ref _PaddingLength, value);
            }
        }

        public string PaddingCharacter
        {
            get
            {
                return _PaddingCharacter;
            }
            set
            {
                SetPropertyValue("PaddingCharacter", ref _PaddingCharacter, value);
            }
        }

        public PaddingDirection PaddingDirection
        {
            get
            {
                return _PaddingDirection;
            }
            set
            {
                SetPropertyValue("PaddingDirection", ref _PaddingDirection, value);
            }
        }

        public bool IsValid(out string errorMessage)
        {   
            if (EndingNumber < StartingNumber)
            {
                errorMessage = Resources.InvalidValue;
                return false;
            }

            if (IsActiveUntilDate.Ticks < IsActiveFromDate.Ticks)
            {
                errorMessage = Resources.EndDateShouldBeGreaterThanStartDate;
                return false;
            }

            //if (CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.UNDEFINED)
            //{
            //    errorMessage = Resources.PLEASE_DEFINE_WHERE_COUPON_AMOUNT_IS_APPLIED;
            //    return false;
            //}

            //if (CouponAppliesOn == CouponAppliesOn.UNDEFINED)
            //{
            //    errorMessage = Resources.PLEASE_DEFINE_COUPON_APPLIED_MODE;
            //    return false;
            //}

            if ((CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.DISCOUNT && DiscountType == null)
                || (CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.PAYMENT_METHOD && PaymentMethod == null)
               )
            {
                errorMessage = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                return false;
            }

            if (
                    UsePadding && (
                                    string.IsNullOrEmpty(PaddingCharacter)
                                    || PaddingCharacter.Length != 1
                                    || PaddingLength < Math.Ceiling(Math.Log10(StartingNumber))
                                    || PaddingLength < Math.Ceiling(Math.Log10(EndingNumber))
                                  )
                )
            {
                errorMessage = Resources.PleaseSetPaddingSettings;
                return false;
            }

            if(CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.PAYMENT_METHOD && CouponAppliesOn == CouponAppliesOn.ITEM)
            {
                errorMessage = Resources.CannotApplyCouponAsPaymentMethodToItem;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public MassivelyCreateCouponsViewModel()
        {
            UsePadding = true;
            PaddingDirection = PaddingDirection.LEFT;
            PaddingCharacter = "0";
        }
    }
}
