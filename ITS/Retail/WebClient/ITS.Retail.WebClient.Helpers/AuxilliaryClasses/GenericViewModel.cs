using ITS.Retail.Model;
using System;
using System.Collections.Generic;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{

    public class GenericViewModel
    {
        public List<string> PropertiesToShow { get; set; }

        public GenericViewModel()
        {
            this.PropertiesToShow = new List<string>();
        }
    }

    public class GenericViewModelDetail : GenericViewModel
    {
        public Type DetailType { get; set; }
        public string PropertyName { get; set; }
        public GenericViewModelMaster Master { get; set; }
    }

    public class GenericViewModelMaster : GenericViewModel
    {
        public GenericViewRuleset Ruleset { get; set; }
        public BasicObj Object { get; set; }
        public List<GenericViewModelDetail> Details { get; set; }
        public List<GenericViewModelDetailedProperty> DetailedProperties { get; set; }

        public GenericViewModelMaster()
        {
            this.Details = new List<GenericViewModelDetail>();
            this.DetailedProperties = new List<GenericViewModelDetailedProperty>();
        }
    }

    public class GenericViewModelDetailedProperty : GenericViewModelMaster
    {
        public GenericViewModelMaster Master { get; set; }
        public string PropertyName { get; set; }
    }

}