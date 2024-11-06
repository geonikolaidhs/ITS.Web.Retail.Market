using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eCultureInfo
    {
        [Description("el")]
        [Display(ResourceType = typeof(Resources), Name = "el")]
        Ελληνικά,

        [Description("en")]
        [Display(ResourceType = typeof(Resources), Name = "en")]
        English,

        [Description("de")]
        [Display(ResourceType = typeof(Resources), Name = "de")]
        Deutch,

        [Description("nb-no")]
        [Display(ResourceType = typeof(Resources), Name = "nb_no")]
        Norsk
    }



}
