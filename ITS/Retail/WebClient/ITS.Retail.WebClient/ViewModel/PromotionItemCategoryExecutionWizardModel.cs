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
    public class PromotionItemCategoryExecutionWizardModel : PromotionExecutionWizardModel
    {
        //[System.ComponentModel.DataAnnotations.Required]
        public override Guid? DiscountType { get; set; }
        
        [System.ComponentModel.DataAnnotations.Display(Name = "ItemCategory", ResourceType = typeof(Resources))]
        public override Type PersistedType { get { return typeof(PromotionItemCategoryExecution); } }

        //[System.ComponentModel.DataAnnotations.Required]
        public Guid? ItemCategory { get; set; }

        public string ItemCategoryDescription { get; set; }

        public decimal Quantity { get; set; }

        public bool OncePerItem { get; set; }

        public eItemExecutionMode ExecutionMode { get; set; }

        public decimal FinalUnitPrice { get; set; }


        public override void UpdateModel(Session uow)
        {

            base.UpdateModel(uow);
            if (ItemCategory.HasValue)
            {
                ItemCategory itc = uow.GetObjectByKey<ItemCategory>(ItemCategory.Value);
                ItemCategoryDescription = itc == null ? "" : itc.Description;
            }
        }
    }
}
