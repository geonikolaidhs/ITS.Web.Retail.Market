using System;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Common.ViewModel
{
    public class CriteriaFieldAttribute : Attribute
    {
        public String FieldName { get; set; }
        public object NullValue { get; set; }
        //public String ValuePrefix { get; set; }
        //public String ValueSufix { get; set; }
        public CustomBinaryOperatorType OperatorType { get; set; }
        public Type PreProcessorType { get; set; }
        public GroupOperatorType GroupOperator { get; set; }
        public bool SearchExactPhrase { get; set; }

        public CriteriaFieldAttribute(String fieldName)
        {
            FieldName = fieldName;
            NullValue = null;
            OperatorType = CustomBinaryOperatorType.Equal;
            //ValuePrefix = string.Empty;
            //ValueSufix = string.Empty;
            GroupOperator = GroupOperatorType.Or;
            SearchExactPhrase = false;
        }

        public BinaryOperatorType BinaryOperatorType
        {
            get
            {
                return (BinaryOperatorType)this.OperatorType;
            }
        }
    }
}