using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class SupplierImportFilesSetViewModel : BasePersistableViewModel
    {

        public SupplierImportFilesSetViewModel()
        {
            SupplierImportFileRecordHeaders = new List<SupplierImportFileRecordHeaderViewModel>();
            SupplierImportMappingHeaders = new List<SupplierImportMappingHeaderViewModel>();
        }

        public override Type PersistedType
        {
            get { return typeof(SupplierImportFilesSet); }
        }

        //private string _ReferenceCode;
        private string _Description;
        private string _Code;
        private Guid? _Supplier;
        private string _SupplierCompanyName;

        [Binding("SupplierComboBox_VI")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Supplier", ResourceType = typeof(Resources))]
        public Guid? Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
            }
        }

        [PersistableViewModel(NotPersistant=true)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Supplier", ResourceType = typeof(Resources))]
        public string SupplierCompanyName
        {
            get
            {
                return _SupplierCompanyName;
            }
            set
            {
                SetPropertyValue("SupplierCompanyName", ref _SupplierCompanyName, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Description", ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
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

        [System.ComponentModel.DataAnnotations.Display(Name = "Code", ResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        private int? _CodePage;
        [System.ComponentModel.DataAnnotations.Display(Name = "CodePage", ResourceType = typeof(Resources))]
        public int? CodePage
        {
            get
            {
                return _CodePage;
            }
            set
            {
                SetPropertyValue("CodePage", ref _CodePage, value);
            }
        }
        
        public List<SupplierImportFileRecordHeaderViewModel> SupplierImportFileRecordHeaders { get; set; }

        public List<SupplierImportMappingHeaderViewModel> SupplierImportMappingHeaders { get; set; }

        public override void UpdateModel(DevExpress.Xpo.Session uow)
        {
            base.UpdateModel(uow);

            if(Supplier.HasValue)
            {
                SupplierNew supplier = uow.GetObjectByKey<SupplierNew>(Supplier.Value);
                SupplierCompanyName = supplier == null ? null : supplier.CompanyName;
            }
        }
    }
}