using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.ViewModel
{
    public class SupplierImportMappingDetailViewModel : BasePersistableViewModel
    {
        public override Type PersistedType
        {
            get { return typeof(SupplierImportMappingDetail); }
        }

        private string _ReplacedString;
        private string _InitialString;

        [System.ComponentModel.DataAnnotations.Display(Name = "InitialString", ResourceType = typeof(Resources))]
        public string InitialString
        {
            get
            {
                return _InitialString;
            }
            set
            {
                SetPropertyValue("InitialString", ref _InitialString, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "TargetString", ResourceType = typeof(Resources))]
        public string ReplacedString
        {
            get
            {
                return _ReplacedString;
            }
            set
            {
                SetPropertyValue("ReplacedString", ref _ReplacedString, value);
            }
        }
    }
}
