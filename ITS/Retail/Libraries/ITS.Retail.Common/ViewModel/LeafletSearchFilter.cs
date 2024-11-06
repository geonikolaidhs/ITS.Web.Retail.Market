using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;

namespace ITS.Retail.Common.ViewModel
{
    public class LeafletSearchFilter:BaseSearchFilter
    {
        private string _Code;
        private string _Description;
        private bool? _IsActive;
        private bool? _ImportedFromERP;

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

        [CriteriaField("ImportedFromERP")]
        public bool? ImportedFromERP
        {
            get
            {
                return _ImportedFromERP;
            }
            set
            {
                SetPropertyValue("ImportedFromERP", ref _ImportedFromERP, value);
            }
        }

        public void LeafletSearchCriteria()
        {
            StartDate = DateTime.Now.Date;
            EndDate = StartDate.Value.AddDays(1).AddSeconds(-1);

            StartDateTime = StartDate.Value.AddDays(-1).AddMilliseconds(1);
            EndDateTime = StartDate.Value.AddDays(1).AddMilliseconds(-1);
        }

        protected DateTime? _StartDate, _EndDate;
        private DateTime? _StartDateTime;
        private DateTime? _EndDateTime;

        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]
        public DateTime? StartDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(StartDate, StartDateTime);
            }
        }

        [Binding("StartDate")]
        public DateTime? StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
                Notify("StartDate");
            }
        }

        [Binding("StartDateTime")]
        public DateTime? StartDateTime
        {
            get
            {
                return _StartDateTime;
            }
            set
            {
                SetPropertyValue("StartDateTime", ref _StartDateTime, value);
            }
        }

        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.LessOrEqual)]
        public DateTime? EndDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(EndDate, EndDateTime);
            }
        }

        [Binding("EndDate")]
        public DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                Notify("EndDate");
            }
        }

        [Binding("EndDateTime")]
        public DateTime? EndDateTime
        {
            get
            {
                return _EndDateTime;
            }
            set
            {
                SetPropertyValue("EndDateTime", ref _EndDateTime, value);
            }
        }
       
    }
}
