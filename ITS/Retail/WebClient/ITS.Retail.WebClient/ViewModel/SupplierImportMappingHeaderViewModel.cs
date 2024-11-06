using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;
namespace ITS.Retail.WebClient.ViewModel
{
    public class SupplierImportMappingHeaderViewModel : BasePersistableViewModel
    {
        public override Type PersistedType
        {
            get { return typeof(SupplierImportMappingHeader); }
        }

        public SupplierImportMappingHeaderViewModel()
        {
            SupplierImportMappingDetails = new List<SupplierImportMappingDetailViewModel>();
        }

        // Fields...
        private string _Description;
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public List<SupplierImportMappingDetailViewModel> SupplierImportMappingDetails { get; set; }
    }
}