using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using ITS.Retail.WebClient.AuxillaryClasses;
using System.Reflection;
using System.Collections;

namespace ITS.Retail.WebClient.ViewModel
{
    public abstract class PromotionExecutionWizardModel : PromotionThenWizardModel
    {
        public abstract Guid? DiscountType { get; set; }
        
        public string DiscountTypeDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "1", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InvalidValue")]
        public decimal Percentage
        {
            get
            {
                return _Percentage;
            }
            set
            {
                _Percentage = value;
                if (value != 0)
                {
                    ValueOrPercentage = value * 100;
                }
            }
        }

        [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InvalidValue")]
        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                if (value != 0)
                {
                    ValueOrPercentage = value;
                }
            }
        }

        [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InvalidValue")]
        public decimal ValueOrPercentage
        {
            get
            {                     
                return _ValueOrPercentage;
            }
            set
            {
                _ValueOrPercentage = value;
            }
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            if (DiscountType.HasValue)
            {
                DiscountType type = uow.GetObjectByKey<DiscountType>(DiscountType.Value);
                if (type != null)
                {
                    DiscountTypeDescription = type.Description;
                    if (this.ValueOrPercentage != 0)
                    {
                        if (type.eDiscountType == eDiscountType.PERCENTAGE)
                        {
                            this._Value = 0;
                            this._Percentage = this._ValueOrPercentage / 100;
                        }
                        else
                        {
                            this._Percentage = 0;
                            this._Value = this._ValueOrPercentage;
                        }
                    }
                    else
                    {
                        this._ValueOrPercentage = Math.Max(this._Value, this._Percentage * 100);
                    }
                }
            }
            else
            {
                DiscountTypeDescription = null;
                this._Value = 0;
                this._Percentage = 0;
            }
        }

        private decimal _ValueOrPercentage;
        private decimal _Percentage;
        private decimal _Value;

    }
}
