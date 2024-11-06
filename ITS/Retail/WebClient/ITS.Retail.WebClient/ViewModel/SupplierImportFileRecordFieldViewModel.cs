using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
namespace ITS.Retail.WebClient.ViewModel
{
    public class SupplierImportFileRecordFieldViewModel : BasePersistableViewModel
    {
        public override Type PersistedType
        {
            get { return typeof(SupplierImportFileRecordField); }
        }

        private Guid? _SupplierImportMappingHeader;
        private string _ConstantValue;
        private string _DefaultValue;
        private bool _Trim;
        private string _PaddingCharacter;
        private bool _Padding;
        private int _Length;
        private int _Position;
        private string _PropertyName;
        private double _Multiplier;

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [System.ComponentModel.DataAnnotations.Display(Name = "PropertyName", ResourceType = typeof(Resources))]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                SetPropertyValue("PropertyName", ref _PropertyName, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Position", ResourceType = typeof(Resources))]
        [Binding("FieldPosition")]
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
        [Binding("FieldLength")]
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

        [System.ComponentModel.DataAnnotations.Display(Name = "Padding", ResourceType = typeof(Resources))]
        public bool Padding
        {
            get
            {
                return _Padding;
            }
            set
            {
                SetPropertyValue("Padding", ref _Padding, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "PaddingCharacter", ResourceType = typeof(Resources))]
        public string PaddingCharacter
        {
            get
            {
                return _PaddingCharacter;
            }
            set
            {
                SetPropertyValue("PaddingCharacter", ref _PaddingCharacter, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Trim", ResourceType = typeof(Resources))]
        public bool Trim
        {
            get
            {
                return _Trim;
            }
            set
            {
                SetPropertyValue("Trim", ref _Trim, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "DefaultValue", ResourceType = typeof(Resources))]
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                SetPropertyValue("DefaultValue", ref _DefaultValue, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "ConstantValue", ResourceType = typeof(Resources))]
        public string ConstantValue
        {
            get
            {
                return _ConstantValue;
            }
            set
            {
                SetPropertyValue("ConstantValue", ref _ConstantValue, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Multiplier", ResourceType = typeof(Resources))]
        public double Multiplier
        {
            get
            {
                return _Multiplier;
            }
            set
            {
                SetPropertyValue("Multiplier", ref _Multiplier, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "Mapping", ResourceType = typeof(Resources))]
        public Guid? SupplierImportMappingHeader
        {
            get
            {                
                return _SupplierImportMappingHeader;
            }
            set
            {
                SetPropertyValue("SupplierImportMappingHeader", ref _SupplierImportMappingHeader, value);
            }
        }
    }
}