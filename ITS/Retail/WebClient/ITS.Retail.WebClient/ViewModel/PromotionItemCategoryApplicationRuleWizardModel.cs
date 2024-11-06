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
    public class PromotionItemCategoryApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "ItemCategory")]
        public override Type PersistedType { get { return typeof(PromotionItemCategoryApplicationRule); } }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid? ItemCategory { get; set; }
        public string ItemCategoryDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        
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

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            if (ItemCategory.HasValue)
            {
                ItemCategory itc = uow.GetObjectByKey<ItemCategory>(ItemCategory.Value);
                ItemCategoryDescription = itc == null ? "" : itc.Description;
            }
        }

        public override string Description
        {
            get
            {
                decimal valueToShow = (this.Quantity > 0 ? this.Quantity : this.Value);
                string stringToShow = (this.Quantity > 0 ? Resources.Quantity : Resources.Value);
                return string.Format("{0} >= {1} ({2})", this.ItemCategoryDescription, valueToShow, stringToShow);
            }
        }

        private int _TypeSelector;

    }
}
