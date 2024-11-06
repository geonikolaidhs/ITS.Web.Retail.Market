using DevExpress.Data.Filtering;
using DevExpress.Xpo;
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
    public class SupplierImportFileRecordHeaderViewModel : BasePersistableViewModel
    {
        public SupplierImportFileRecordHeaderViewModel()
        {
            SupplierImportFileRecordFields = new List<SupplierImportFileRecordFieldViewModel>();
            //DetailSupplierImportFileRecordHeaders = new List<SupplierImportFileRecordHeaderViewModel>();
        }

        public override Type PersistedType
        {
            get { return typeof(SupplierImportFileRecordHeader); }
        }

        private string _HeaderCode;
        private int _Position;
        private string _TabDelimitedString;
        private bool _IsTabDelimited;
        private string _EntityName;
        private int _Length;
        private int _Order;

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [System.ComponentModel.DataAnnotations.Display(Name = "EntityName", ResourceType = typeof(Resources))]
        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [System.ComponentModel.DataAnnotations.Display(Name = "HeaderCode", ResourceType = typeof(Resources))]
        public string HeaderCode
        {
            get
            {
                return _HeaderCode;
            }
            set
            {
                SetPropertyValue("HeaderCode", ref _HeaderCode, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Position", ResourceType = typeof(Resources))]
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Length", ResourceType = typeof(Resources))]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                SetPropertyValue("Length", ref _Length, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "IsTabDelimited", ResourceType = typeof(Resources))]
        public bool IsTabDelimited
        {
            get
            {
                return _IsTabDelimited;
            }
            set
            {
                SetPropertyValue("IsTabDelimited", ref _IsTabDelimited, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "TabDelimitedString", ResourceType = typeof(Resources))]
        public string TabDelimitedString
        {
            get
            {
                return _TabDelimitedString;
            }
            set
            {
                SetPropertyValue("TabDelimitedString", ref _TabDelimitedString, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "ImportOrder", ResourceType = typeof(Resources))]
        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                SetPropertyValue("Order", ref _Order, value);
            }
        }


        private Guid? _DocumentType;
        [System.ComponentModel.DataAnnotations.Display(Name = "DocumentType", ResourceType = typeof(Resources))]
        public Guid? DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        private string _DocumentTypeDescription;
        [PersistableViewModel(NotPersistant = true)]
        [System.ComponentModel.DataAnnotations.Display(Name = "DocumentType", ResourceType = typeof(Resources))]
        public string DocumentTypeDescription
        {
            get
            {
                return _DocumentTypeDescription;
            }
            set
            {
                SetPropertyValue("DocumentTypeDescription", ref _DocumentTypeDescription, value);
            }
        }

        private Guid? _DocumentStatus;
        [System.ComponentModel.DataAnnotations.Display(Name = "DocumentStatus", ResourceType = typeof(Resources))]
        public Guid? DocumentStatus
        {
            get
            {
                return _DocumentStatus;
            }
            set
            {
                SetPropertyValue("DocumentStatus", ref _DocumentStatus, value);
            }
        }

        private string _DocumentStatusDescription;
        [PersistableViewModel(NotPersistant = true)]
        [System.ComponentModel.DataAnnotations.Display(Name = "DocumentStatus", ResourceType = typeof(Resources))]
        public string DocumentStatusDescription
        {
            get
            {
                return _DocumentStatusDescription;
            }
            set
            {
                SetPropertyValue("DocumentStatusDescription", ref _DocumentStatusDescription, value);
            }
        }

        private Guid? _MasterSupplierImportFileRecordHeader;
        [System.ComponentModel.DataAnnotations.Display(Name = "MasterSupplierImportFileRecordHeader", ResourceType = typeof(Resources))]
        public Guid? MasterSupplierImportFileRecordHeader
        {
            get
            {
                return _MasterSupplierImportFileRecordHeader;
            }
            set
            {
                SetPropertyValue("MasterSupplierImportFileRecordHeader", ref _MasterSupplierImportFileRecordHeader, value);
            }
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);

            if (DocumentType.HasValue)
            {
                DocumentType documentType = uow.GetObjectByKey<DocumentType>(DocumentType.Value);
                DocumentTypeDescription = documentType == null ? null : documentType.Description;
            }

            if (DocumentStatus.HasValue)
            {
                DocumentStatus documentStatus = uow.GetObjectByKey<DocumentStatus>(DocumentStatus.Value);
                DocumentStatusDescription = documentStatus == null ? null : documentStatus.Description;
            }
        }

        public List<SupplierImportFileRecordFieldViewModel> SupplierImportFileRecordFields { get; set; }

        //public List<SupplierImportFileRecordHeaderViewModel> DetailSupplierImportFileRecordHeaders { get; set; }

    }
}