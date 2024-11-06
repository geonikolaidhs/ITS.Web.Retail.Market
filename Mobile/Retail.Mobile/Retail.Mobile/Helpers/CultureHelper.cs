using System;

using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Retail.Mobile.Helpers
{
    public static class CultureHelper
    {
        public static void SetCulture(CultureInfo new_culture)
        {
            new_culture.NumberFormat = System.Globalization.CultureInfo.GetCultureInfo("de-DE").NumberFormat;
            Common.CultureInfo = new_culture;
            Resources.Resources.Culture = new_culture;
        }
    }
}
