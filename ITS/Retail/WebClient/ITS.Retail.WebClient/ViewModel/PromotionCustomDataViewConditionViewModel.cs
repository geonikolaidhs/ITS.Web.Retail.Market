using ITS.Retail.Common.Attributes;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PromotionCustomDataViewConditionViewModel
    {
        public string DataViewColumn { get; set; }
        [BindingListAttributeSuffix( Suffix = "VI")]
        public ComparisonOperator ComparisonOperator { get; set; }
        public object Value { get; set; }
    }
}