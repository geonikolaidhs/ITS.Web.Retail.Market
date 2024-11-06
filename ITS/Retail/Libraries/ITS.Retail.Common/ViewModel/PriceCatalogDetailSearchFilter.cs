using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCatalogDetailSearchFilter: BaseSearchFilter
    {
        private Guid _Owner;
        private DateTime? _CreatedOn;
        private DateTime? _UpdatedOn;
        private string _Code;
        private string _Name;
        private bool? _IsActive;
        private string _Barcode;
        private string _DefaultSupplier;
        private Guid? _Seasonality;
        private Guid? _Buyer;
        private string _MotherCode;
        private Guid _PriceCatalog;

        public PriceCatalogDetailSearchFilter()
        {
            Code = string.Empty;
            Name = string.Empty;
            Barcode = string.Empty;
            DefaultSupplier = string.Empty;
            MotherCode = string.Empty;
        }

        [CriteriaField("Item.CreatedOnTicks", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]

        public long? CreatedOnTicks
        {
            get
            {
                return CreatedOn.HasValue ? (long?)CreatedOn.Value.Ticks : null;
            }
        }
        public DateTime? CreatedOn
        {
            get
            {
                return _CreatedOn;
            }
            set
            {
                SetPropertyValue("CreatedOn", ref _CreatedOn, value);
            }
        }

        [CriteriaField("Item.DefaultSupplier.CompanyName", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string DefaultSupplier
        {
            get
            {
                return _DefaultSupplier;
            }
            set
            {
                SetPropertyValue("DefaultSupplier", ref _DefaultSupplier, value);
            }
        }

        //[CriteriaField("DefaultBarcode", OperatorType = BinaryOperatorType.Like, NullValue = "")]
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

        [CriteriaField("Item.UpdatedOnTicks", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]

        public long? UpdatedOnTicks
        {
            get
            {
                return UpdatedOn.HasValue ? (long?)UpdatedOn.Value.Ticks : null;
            }
        }
        public DateTime? UpdatedOn
        {
            get
            {
                return _UpdatedOn;
            }
            set
            {
                SetPropertyValue("UpdatedOn", ref _UpdatedOn, value);
            }
        }

        [CriteriaField("Item.Code", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
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

        [CriteriaField("Item.Name", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        [CriteriaField("Item.MotherCode.Code", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string MotherCode
        {
            get
            {
                return _MotherCode;
            }
            set
            {
                SetPropertyValue("MotherCode", ref _MotherCode, value);
            }
        }

        [CriteriaField("Item.IsActive")]
        public bool? IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }



        [CriteriaField("Item.Seasonality.Oid")]
        public Guid? Seasonality
        {
            get
            {
                return _Seasonality;
            }
            set
            {
                SetPropertyValue("Seasonality", ref _Seasonality, value);
            }
        }

        [CriteriaField("Item.Buyer.Oid")]
        public Guid? Buyer
        {
            get
            {
                return _Buyer;
            }
            set
            {
                SetPropertyValue("Buyer", ref _Buyer, value);
            }
        }

        [CriteriaField("Item.Owner.Oid")]
        public Guid Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        [CriteriaField("PriceCatalog.Oid")]
        public Guid PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }

        protected override List<CriteriaOperator> BuildExtraCriteria()
        {
            List<CriteriaOperator> extraCriteria = base.BuildExtraCriteria();

            if (string.IsNullOrWhiteSpace(this.Barcode) == false)
            {
                extraCriteria.Add(new ContainsOperator("Item.ItemBarcodes", new BinaryOperator("Barcode.Code", this.Barcode.Replace('*', '%'), BinaryOperatorType.Like)));
            }

            return extraCriteria;
        }
    }
}
