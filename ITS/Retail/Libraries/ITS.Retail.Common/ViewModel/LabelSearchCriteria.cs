using DevExpress.Data.Filtering;
using System;


namespace ITS.Retail.Common.ViewModel
{
    public class LabelSearchCriteria : BaseSearchFilter
    {
        public LabelSearchCriteria()
        {
            FromDate = DateTime.Now.Date;
            ToDate = FromDate.Value.AddDays(1).AddSeconds(-1);

            FromDateTime = FromDate.Value.AddDays(-1).AddMilliseconds(1);
            ToDateTime = FromDate.Value.AddDays(1).AddMilliseconds(-1);
        }

        protected DateTime? _FromDate, _ToDate;
        private DateTime? _FromDateTime;
        private DateTime? _ToDateTime;

        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]
        public DateTime? FromDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(FromDate, FromDateTime);
            }
        }

        [Binding("FromDate")]        
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                Notify("FromDate");
            }
        }

        [Binding("FromDateTime")]
        public DateTime? FromDateTime
        {
            get
            {
                return _FromDateTime;
            }
            set
            {
                SetPropertyValue("FromDateTime", ref _FromDateTime, value);
            }
        }

        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.LessOrEqual)]
        public DateTime? ToDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(ToDate, ToDateTime);
            }
        }

        [Binding("ToDate")]        
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value; 
                Notify("ToDate");
            }
        }

        [Binding("ToDateTime")]
        public DateTime? ToDateTime
        {
            get
            {
                return _ToDateTime;
            }
            set
            {
                SetPropertyValue("ToDateTime", ref _ToDateTime, value);
            }
        }
    }
}