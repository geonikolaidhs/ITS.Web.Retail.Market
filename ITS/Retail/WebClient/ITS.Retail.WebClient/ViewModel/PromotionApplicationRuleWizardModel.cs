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
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.WebClient.ViewModel
{
    public abstract class PromotionApplicationRuleWizardModel : PromotionRuleWizardModel
    {
        public override bool Validate(out string message)
        {
            message = "";
            ValidationContext validationContext = new ValidationContext(this, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if(Validator.TryValidateObject(this, validationContext, validationResults, true) == false)
            {
                message = validationResults.Select(x=>x.ErrorMessage).Aggregate((x, y) => { return x + Environment.NewLine + y; });
                return false;
            }


            return IsDeleted == false;
        }

        public override IEnumerable<PromotionRuleWizardModel> AllChilds
        {
            get
            {
                return new List<PromotionRuleWizardModel>() { this };
            }
        }
    }
}
