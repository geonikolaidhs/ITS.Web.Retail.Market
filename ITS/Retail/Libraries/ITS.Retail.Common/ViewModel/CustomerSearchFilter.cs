using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;

namespace ITS.Retail.Common.ViewModel
{
    public class CustomerSearchFilter : BaseSearchFilter
    {
        private Guid _Owner;
        private string _CardID;
        private string _TaxCode;
        private DateTime? _CreatedOn;
        private DateTime? _UpdatedOn;
        private string _CustomerCode;
        private bool? _IsActive;
        private string _CustomerName;
        public CustomerSearchFilter()
        {
            CardID = string.Empty;
            TaxCode = string.Empty;
            CustomerCode = string.Empty;
            CustomerName = string.Empty;
            
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
       
        [CriteriaField("CardID", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                SetPropertyValue("CardID", ref _CardID, value);
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
        public string CustomerCode
        {
            get
            {
                return _CustomerCode;
            }
            set
            {
                SetPropertyValue("CustomerCode", ref _CustomerCode, value);
            }
        }

        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                SetPropertyValue("CustomerName", ref _CustomerName, value);
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
            if (String.IsNullOrWhiteSpace(this.CustomerName) == false)
            {
                extraCriteria.Add(CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), this.CustomerName),
                                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), this.CustomerName),
                                    new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), this.CustomerName)));
            }
            return extraCriteria;
        }

    }
}
