using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PromotionCustomDataViewParameterWizardModel : PromotionApplicationRuleDetailWizardModel
    {
        public override IEnumerable<PromotionRuleWizardModel> AllChilds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name { get; set; }

        public override Type PersistedType
        {
            get
            {
                return typeof(PromotionApplicationRuleDetailCustomDataViewParameter);
            }
        }

        public string Type { get; set; }

        public object Value { get; set; }

        public override bool Validate(out string message)
        {
            throw new NotImplementedException();
        }
    }
}