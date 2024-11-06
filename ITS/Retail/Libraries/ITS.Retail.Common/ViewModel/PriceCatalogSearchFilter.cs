using DevExpress.Data.Filtering;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCatalogSearchFilter:BaseSearchFilter
    {
        private string _Code;
        private string _Description;
        private bool? _IsActive;

        [CriteriaField("Code", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
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


        [CriteriaField("Description", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
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
    }
}
