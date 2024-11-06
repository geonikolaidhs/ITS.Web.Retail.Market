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
    
    public class PromotionCustomerApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "Customer")]
        public override Type PersistedType { get { return typeof(PromotionCustomerApplicationRule); } }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid? Customer { get; set; }
        public string CustomerDescription { get; set; }
        public override string Description
        {
            get { return string.Format("{0} = {1}", Resources.Customer, this.CustomerDescription); }
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            if (Customer.HasValue)
            {
                Customer itc = uow.GetObjectByKey<Customer>(Customer.Value);
                CustomerDescription = itc == null ? "" : itc.Description;
            }
        }
    }
}
