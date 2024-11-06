using DevExpress.Xpo;
using ITS.Retail.Common.Attributes;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PromotionCustomDataViewApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {

        public PromotionCustomDataViewApplicationRuleWizardModel()
        {
            this.Parameters = new List<PromotionCustomDataViewParameterWizardModel>();
            this.Conditions = new List<PromotionCustomDataViewConditionViewModel>();
        }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid? CustomDataView { get; set; }
        public string CustomDataViewDescription { get; set; }

        [BindingList(Prefix = "parameter")]
        public List<PromotionCustomDataViewParameterWizardModel> Parameters {get; set;}
        [BindingList(Prefix = "condition")]
        public List<PromotionCustomDataViewConditionViewModel> Conditions { get; set; }

        public override string Description
        {
            get
            {
                return string.Format("{0} = {1}", Resources.DataView, this.CustomDataViewDescription);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "DataView")]
        public override Type PersistedType
        {
            get
            {
                return typeof(PromotionCustomDataViewApplicationRule);
            }
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            if (CustomDataView.HasValue)
            {
                CustomDataView customDataView = uow.GetObjectByKey<CustomDataView>(CustomDataView.Value);
                CustomDataViewDescription = customDataView == null ? "" : customDataView.Description;
            }
        }
    }
}