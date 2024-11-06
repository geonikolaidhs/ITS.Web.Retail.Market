using DevExpress.Data.Filtering;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Common.ViewModel
{
    public class LabelItemChangeCriteria : BaseSearchFilter
    {
        private DateTime? _TimeValueToTime;
        private DateTime? _TimeValueToDate;
        private DateTime? _TimeValueFromTime;
        private DateTime? _TimeValueFromDate;

        private bool _WithTimeValueFilter;
        private DateTime? _FromDateTime;
        private DateTime? _ToDateTime;
        private Guid? _ItemCategory;

        public LabelItemChangeCriteria()
        {
            FromDate = DateTime.Now.Date;
            ToDate =  FromDate.Value.AddDays(1).AddSeconds(-1);
            WithValueChangeOnly = true;
            WithTimeValueFilter = true;

            FromDateTime = FromDate.Value.AddDays(-1).AddMilliseconds(1);
            ToDateTime = FromDate.Value.AddDays(1).AddMilliseconds(-1);

            TimeValueFromDate = FromDate;//DateTime.Now.Date.AddDays(-1);
            TimeValueFromTime = FromDateTime;
            TimeValueToDate = ToDate;
            TimeValueToTime = ToDateTime;
        }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [CriteriaField("Item.Code", OperatorType = CustomBinaryOperatorType.GreaterOrEqual, NullValue = "")]
        public string FromCode { get; set; }

        [CriteriaField("Item.Code", OperatorType = CustomBinaryOperatorType.LessOrEqual, NullValue = "")]
        public string ToCode { get; set; }

        public string Barcode { get; set; }

        [CriteriaField("Item.Name", OperatorType = CustomBinaryOperatorType.Like, NullValue = "")]
        public string Description { get; set; }

        public bool WithValueChangeOnly { get; set; }
        
        public bool WithTimeValueFilter
        {
            get
            {
                return _WithTimeValueFilter;
            }
            set
            {
                SetPropertyValue("WithTimeValueFilter", ref _WithTimeValueFilter, value);
            }
        }

        public DateTime? TimeValueFromDate
        {
            get
            {
                return _TimeValueFromDate;
            }
            set
            {
                SetPropertyValue("TimeValueFromDate", ref _TimeValueFromDate, value);
            }
        }


        public DateTime? TimeValueFromTime
        {
            get
            {
                return _TimeValueFromTime;
            }
            set
            {
                SetPropertyValue("TimeValueFromTime", ref _TimeValueFromTime, value);
            }
        }


        public DateTime? TimeValueToDate
        {
            get
            {
                return _TimeValueToDate;
            }
            set
            {
                SetPropertyValue("TimeValueToDate", ref _TimeValueToDate, value);
            }
        }


        public DateTime? TimeValueToTime
        {
            get
            {
                return _TimeValueToTime;
            }
            set
            {
                SetPropertyValue("TimeValueToTime", ref _TimeValueToTime, value);
            }
        }

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

        public Guid? ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("Owner", ref _ItemCategory, value);
            }
        }

        protected DateTime dateFrom
        {
            get
            {
                DateTime? dateTimeValue = DateTimeHelper.GetDateTimeValue(FromDate, FromDateTime);
                return dateTimeValue.HasValue ? dateTimeValue.Value : DateTime.MinValue;
            }
        }

        protected DateTime dateTo
        {
            get
            {
                DateTime? dateTimeValue = DateTimeHelper.GetDateTimeValue(ToDate, ToDateTime);
                return dateTimeValue.HasValue ? dateTimeValue.Value : DateTime.MaxValue;
            }
        }


        protected DateTime valueDateFrom
        {
            get
            {
                DateTime? dateTimeValue = DateTimeHelper.GetDateTimeValue(this.TimeValueFromDate, this.TimeValueFromTime);
                return dateTimeValue.HasValue ? dateTimeValue.Value : DateTime.MinValue;
            }
        }


        protected DateTime valueDateTo
        {
            get
            {
                DateTime? dateTimeValue = DateTimeHelper.GetDateTimeValue(this.TimeValueToDate, this.TimeValueToTime);
                return dateTimeValue.HasValue ? dateTimeValue.Value : DateTime.MaxValue;
            }
        }

        protected override List<CriteriaOperator> BuildExtraCriteria()
        {
            List<CriteriaOperator> list = base.BuildExtraCriteria();

            
            CriteriaOperator criteriaOfItem = null;

            // Αλλαγή της βασικής τιμής του είδους - αν ισχύει
            CriteriaOperator baseValueCriteria =
                            CriteriaOperator.And(
                            new BetweenOperator("ValueChangedOn", dateFrom.Ticks, dateTo.Ticks),
                            new NotOperator(
                                new ContainsOperator("TimeValues",
                                    CriteriaOperator.And(
                                        new BinaryOperator("TimeValueValidFrom", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual),
                                        new BinaryOperator("TimeValueValidUntil", DateTime.Now.Ticks, BinaryOperatorType.GreaterOrEqual),
                                        new BinaryOperator("IsActive", true, BinaryOperatorType.Equal)
                        )
                    )                    
                ));


            //Αλλαγή του είδους
            if (WithValueChangeOnly == false)
            {
                criteriaOfItem = CriteriaOperator.Or(new BetweenOperator("Item.UpdatedOnTicks", dateFrom.Ticks, dateTo.Ticks),
                                                     new ContainsOperator("Item.ItemExtraInfos",
                                                                           new BetweenOperator("UpdatedOnTicks", dateFrom.Ticks, dateTo.Ticks)
                                                                         ),
                                                     baseValueCriteria
                                                    );
            }
            else
            {
                criteriaOfItem = baseValueCriteria;
            }


            //Αλλαγή της τιμής χρονικής ισχύος - που ήδη ισχύει
            CriteriaOperator timeValueChangeCriteria =
                                                    new ContainsOperator("TimeValues",
                                                        CriteriaOperator.And(
                                                            new BetweenOperator("TimeValueChangedOn", dateFrom.Ticks, dateTo.Ticks),
                                                            CriteriaOperator.And(
                                                                new BinaryOperator("TimeValueValidFrom", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual),
                                                                new BinaryOperator("TimeValueValidUntil", DateTime.Now.Ticks, BinaryOperatorType.GreaterOrEqual)
                                                            )
                                                        ));

            //Αλλαγή λόγω εναρξης ή λήξης χρονικής ισχύος
            CriteriaOperator timeValueStartCriteria = new ContainsOperator("TimeValues", new BetweenOperator("TimeValueValidFrom", dateFrom.Ticks, dateTo.Ticks));
            CriteriaOperator timeValueEndCriteria = new ContainsOperator("TimeValues", new BetweenOperator("TimeValueValidUntil", dateFrom.Ticks, dateTo.Ticks));

            if (WithTimeValueFilter)
            {
                list.Add(CriteriaOperator.Or(//timeValueChangeCriteria, 
                                             criteriaOfItem,
                                              timeValueStartCriteria,
                                             timeValueEndCriteria
                                             ));
            }
            else
            {
                list.Add(CriteriaOperator.Or(timeValueChangeCriteria, criteriaOfItem));
            }

            if (!string.IsNullOrEmpty(this.Barcode))
            {
                string barcode = this.Barcode.Replace("*", "%");
                barcode = (barcode.Contains('%')) ? barcode : String.Format("%{0}%", barcode);
                list.Add(new BinaryOperator("Barcode.Code", barcode, BinaryOperatorType.Like));
            }

            return list;
        }
    }
}
