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
    public class PromotionDocumentExecutionWizardModel : PromotionExecutionWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Document", ResourceType = typeof(Resources))]
        public override Type PersistedType { get { return typeof(PromotionDocumentExecution); } }

        public decimal Points { get; set; }

        public override Guid? DiscountType { get; set; }

        public bool KeepOnlyPoints { get; set; }
       
        
    }

    
}
