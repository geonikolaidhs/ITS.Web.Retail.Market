using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;

namespace ITS.Retail.Common.ViewModel
{
    public class SupplierSearchFilter : BaseSearchFilter
    {
        private string _TaxCode;
        private DateTime? _CreatedOn;
        private DateTime? _UpdatedOn;
        private string _SupplierCode;
        private bool? _IsActive;
        private string _SupplierName;
        private Guid _Owner;

        public SupplierSearchFilter()
        {
            SupplierName = string.Empty;
            SupplierCode = string.Empty;
            TaxCode = string.Empty;
        }
        [CriteriaField("CreatedOnTicks", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]

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

        [CriteriaField("Trader.TaxCode", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }

        [CriteriaField("UpdatedOnTicks", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]

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

        [CriteriaField("Code", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string SupplierCode
        {
            get
            {
                return _SupplierCode;
            }
            set
            {
                SetPropertyValue("SupplierCode", ref _SupplierCode, value);
            }
        }

      //  [CriteriaField("Trader.FirstName", OperatorType = BinaryOperatorType.Like, NullValue = "")]
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                SetPropertyValue("SupplierName", ref _SupplierName, value);
            }
        }

        [CriteriaField("IsActive")]
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

        [CriteriaField("Owner.Oid")]
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

        protected override List<CriteriaOperator> BuildExtraCriteria()
        {
            List<CriteriaOperator> extraCriteria = base.BuildExtraCriteria();
            if (string.IsNullOrWhiteSpace(SupplierName) == false)
            {
                extraCriteria.Add(CriteriaOperator.Or(
                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), this.SupplierName),
                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), this.SupplierName),
                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), this.SupplierName)));
            }
            return extraCriteria;
        }
    }
}
