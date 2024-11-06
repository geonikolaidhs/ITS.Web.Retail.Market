using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PromotionPriceCatalogExecutionWizardModel : PromotionExecutionWizardModel
    {
        public override Guid? DiscountType { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "PriceCatalog", ResourceType = typeof(Resources))]
        public override Type PersistedType
        {
            get
            {
                return typeof(PromotionPriceCatalogExecution);
            }
        }

        public string PriceCatalogs;
        public bool OncePerItem;
        public eItemExecutionMode ExecutionMode;
        public decimal FinalUnitPrice;
    }
}