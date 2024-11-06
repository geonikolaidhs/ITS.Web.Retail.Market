using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum DaysOfWeek
    {
        [Display(Name = "NoneDay", ResourceType = typeof(Resources))]
        None = 0 ,
        [Display(Name = "Sunday", ResourceType = typeof(Resources))]
        Sunday = 1,
        [Display(Name = "Monday", ResourceType = typeof(Resources))]
        Monday = 2,
        [Display(Name = "Tuesday", ResourceType = typeof(Resources))]
        Tuesday = 4 ,
        [Display(Name = "Wednesday", ResourceType = typeof(Resources))]
        Wednesday = 8 ,
        [Display(Name = "Thursday", ResourceType = typeof(Resources))]
        Thursday = 16,
        [Display(Name = "Friday", ResourceType = typeof(Resources))]
        Friday = 32,
        [Display(Name = "Saturday", ResourceType = typeof(Resources))]
        Saturday = 64,
        
    }
}
