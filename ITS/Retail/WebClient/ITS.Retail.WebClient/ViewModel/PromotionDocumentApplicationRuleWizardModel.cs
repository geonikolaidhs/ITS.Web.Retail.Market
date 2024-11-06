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
    public class PromotionDocumentApplicationRuleWizardModel : PromotionApplicationRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "Document")]
        public override Type PersistedType { get { return typeof(PromotionDocumentApplicationRule); } }

        [System.ComponentModel.DataAnnotations.Range(0.001,Double.MaxValue)]
        public double ValueDouble
        {
            get
            {
                return (double)Value;
            }
        }
        
        public decimal Value { get; set; }
        public bool ValueIsRepeating { get; set; }

        public override string Description
        {

            get { return string.Format("{0} >= {1}", Resources.Document, this.Value) + (this.ValueIsRepeating ? "(" + Resources.ValueIsRepeating +")": ""); }
        }
    }
}
