using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Common.ViewModel;

namespace ITS.POS.Client.Actions.ActionParameters
{
	public class ActionAddDocumentDiscountParams: ActionParams
	{
        public decimal ValueOrPercentage { get; set; }
        public DiscountType DiscountType { get; set; }
        public CouponViewModel CouponViewModel { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_DOCUMENT_DISCOUNT; }
        }

        /// <summary>
        /// Constructor for ActionAddDocumentDiscountParams (parameters for action ActionAddDocumentDiscount)
        /// </summary>
        /// <param name="valueOrPercentage">A number representing the discount amount either in currency either in percentage</param>
        /// <param name="discountType">A DiscountType object containg discount details</param>
        /// <param name="couponViewModel">Default value is null. A value is provided ONLY if a Coupon is used.In this case a TransactionCoupon object is created and saved</param>
        public ActionAddDocumentDiscountParams(decimal valueOrPercentage,DiscountType discountType,CouponViewModel couponViewModel = null)
		{
            this.ValueOrPercentage = valueOrPercentage;
            this.DiscountType = discountType;
            this.CouponViewModel = couponViewModel;
		}
	}
}
