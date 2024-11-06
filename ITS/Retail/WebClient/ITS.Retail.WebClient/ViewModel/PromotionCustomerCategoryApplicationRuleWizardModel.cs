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
    public class PromotionCustomerCategoryApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "CustomerCategory")]
        public override Type PersistedType { get { return typeof(PromotionCustomerCategoryApplicationRule); } }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid? CustomerCategory { get; set; }

        public string CustomerCategoryDescription { get; set; }




        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            if (CustomerCategory.HasValue)
            {
                CustomerCategory itc = uow.GetObjectByKey<CustomerCategory>(CustomerCategory.Value);
                CustomerCategoryDescription = itc == null ? "" : itc.Description;
            }
        }

        public override string Description
        {
            get
            {
                return string.Format("{0} = {1}",Resources.CustomerCategory, this.CustomerCategoryDescription);
            }
        }

        //private int _TypeSelector;

    }
}
