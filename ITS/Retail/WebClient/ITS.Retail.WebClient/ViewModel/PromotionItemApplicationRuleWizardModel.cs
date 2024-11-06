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
    public class PromotionItemApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "Item")]
        public override Type PersistedType { get { return typeof(PromotionItemApplicationRule); } }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid? Item { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public decimal QuantityOrValue
        {
            get
            {
                return (TypeSelector == 0) ? Quantity : Value;
            }
            set
            {
                Quantity = (TypeSelector == 0) ? value : 0;
                Value = (TypeSelector == 1) ? value : 0;
            }
        }

        public int TypeSelector
        {
            get
            {
                return _TypeSelector;
            }
            set
            {
                decimal oldQoV = QuantityOrValue;
                _TypeSelector = value;
                QuantityOrValue = oldQoV;
            }
        }

        public override string Description
        {
            get
            {
                decimal valueToShow = (this.Quantity > 0 ? this.Quantity : this.Value);
                string stringToShow = (this.Quantity > 0 ? Resources.Quantity : Resources.Value);
                return string.Format("{0} >= {1} ({2})", this.ItemDescription, valueToShow, stringToShow);
            }
        }


        public override void UpdateModel(Session uow)
        {

            base.UpdateModel(uow);
            if (Item.HasValue)
            {
                Item itc = uow.GetObjectByKey<Item>(Item.Value);
                ItemDescription = itc == null ? "" : itc.Name;
            }
        }

        private int _TypeSelector;
    }
}
