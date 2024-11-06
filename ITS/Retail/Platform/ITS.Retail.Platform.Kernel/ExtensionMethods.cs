using ITS.Retail.Platform.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class KernelExtensionMethods
    {

        public static DaysOfWeek ToDaysOfWeek(this DayOfWeek take)
        {
            return (DaysOfWeek)(1 << (int)take);
        }


        public static bool IsInDays(this DateTime time, DaysOfWeek range)
        {
            return range.HasFlag(time.DayOfWeek.ToDaysOfWeek());
        }

        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }


    }
}
