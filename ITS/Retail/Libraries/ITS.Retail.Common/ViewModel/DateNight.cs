using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Common.ViewModel
{
    public class DateNight : ICriteriaPreprocessor<DateTime>
    {

        public DateTime[] Preprocess(DateTime input, CriteriaFieldAttribute attribute)
        {
            return new DateTime[] { input.Date.AddDays(1).AddMilliseconds(-1) };
        }
    }
}