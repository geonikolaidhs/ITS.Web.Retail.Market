using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Combines the date and time in one variable
        /// </summary>
        /// <param name="date">Date value</param>
        /// <param name="time">Time value</param>
        /// <returns>The combined value</returns>
        public static DateTime? GetDateTimeValue(DateTime? date, DateTime? time)
        {
            if (!date.HasValue)
            {
                return null;
            }
            if (!time.HasValue)
            {
                return date;
            }
            return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day,
                time.Value.Hour, time.Value.Minute, time.Value.Second);
        }
    }
}
