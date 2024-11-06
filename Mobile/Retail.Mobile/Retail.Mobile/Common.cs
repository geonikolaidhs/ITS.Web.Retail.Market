using System;

using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Retail.Mobile
{
    public static class Common
    {
        public static readonly string fname_trans = "trans.db";
        public static readonly string fname_items = "items.db";
        public static readonly string fname_tmpProducts = "tmpProducts.db";
        public static string SelectedCulture = "";

        /// <summary>
        /// Initializes the common information needed by other applications such as CultureInfo etc
        /// </summary>
        static Common()
        {
            switch (SelectedCulture)
            {
                case "el-gr":
                    try
                    {
                        _cultureInfo = new CultureInfo("el-gr");
                    }
                    catch (PlatformNotSupportedException)
                    {
                        _cultureInfo = new CultureInfo("");
                        CultureInfo.NumberFormat = System.Globalization.CultureInfo.GetCultureInfo("de-DE").NumberFormat;//*/

                        _cultureInfo.DateTimeFormat.AbbreviatedDayNames = new string[] { "Δευ", "Τρί", "Τετ", "Πέμ", "Παρ", "Σάβ", "Κυρ" };
                        _cultureInfo.DateTimeFormat.DayNames = new string[] { "Δευτέρα", "Τρίτη", "Τετάρτη", "Πέμπτη", "Παρασκευή", "Σάββατο", "Κυριακή" };
                        _cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Ιαν", "Φεβ", "Μαρ", "Απρ", "Μαι", "Ιουν", "Ιουλ", "Αυγ", "Σεπ", "Οκτ", "Νοε", "Δεκ", "" };
                        _cultureInfo.DateTimeFormat.MonthNames = new string[] { "Ιανουάριος", "Φεβρουάριος", "Μάρτιος", "Απρίλιος", "Μάϊος", "Ιούνιος", "Ιούλιος", "Αύγουστος", "Σεπτέμβριος", "Οκτώβριος", "Νοέμβριος", "Δεκέμβριος", "" };
                        _cultureInfo.DateTimeFormat.DateSeparator = "/";
                        _cultureInfo.DateTimeFormat.FullDateTimePattern = "dddd d MMMM yyyy";
                        _cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                        _cultureInfo.DateTimeFormat.YearMonthPattern = "MMMM yyyy";
                        _cultureInfo.DateTimeFormat.MonthDayPattern = "d MMMM";

                        _cultureInfo.DateTimeFormat.TimeSeparator = ":";
                        _cultureInfo.DateTimeFormat.AMDesignator = "π.μ.";
                        _cultureInfo.DateTimeFormat.PMDesignator = "μ.μ.";
                        _cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
                        _cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
                    }
                    break;
                case "nb-NO":
                    try
                    {
                        _cultureInfo = new CultureInfo("nb-NO");
                    }
                    catch (PlatformNotSupportedException)
                    {
                        _cultureInfo = new CultureInfo("en-US");
                        //TODO
                        //_cultureInfo = new CultureInfo("");
                        //CultureInfo.NumberFormat = System.Globalization.CultureInfo.GetCultureInfo("de-DE").NumberFormat;//*/

                        //_cultureInfo.DateTimeFormat.AbbreviatedDayNames = new string[] { "Δευ", "Τρί", "Τετ", "Πέμ", "Παρ", "Σάβ", "Κυρ" };
                        //_cultureInfo.DateTimeFormat.DayNames = new string[] { "Δευτέρα", "Τρίτη", "Τετάρτη", "Πέμπτη", "Παρασκευή", "Σάββατο", "Κυριακή" };
                        //_cultureInfo.DateTimeFormat.AbbreviatedMonthNames = new string[] { "Ιαν", "Φεβ", "Μαρ", "Απρ", "Μαι", "Ιουν", "Ιουλ", "Αυγ", "Σεπ", "Οκτ", "Νοε", "Δεκ", "" };
                        //_cultureInfo.DateTimeFormat.MonthNames = new string[] { "Ιανουάριος", "Φεβρουάριος", "Μάρτιος", "Απρίλιος", "Μάϊος", "Ιούνιος", "Ιούλιος", "Αύγουστος", "Σεπτέμβριος", "Οκτώβριος", "Νοέμβριος", "Δεκέμβριος", "" };
                        //_cultureInfo.DateTimeFormat.DateSeparator = "/";
                        //_cultureInfo.DateTimeFormat.FullDateTimePattern = "dddd d MMMM yyyy";
                        //_cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                        //_cultureInfo.DateTimeFormat.YearMonthPattern = "MMMM yyyy";
                        //_cultureInfo.DateTimeFormat.MonthDayPattern = "d MMMM";

                        //_cultureInfo.DateTimeFormat.TimeSeparator = ":";
                        //_cultureInfo.DateTimeFormat.AMDesignator = "π.μ.";
                        //_cultureInfo.DateTimeFormat.PMDesignator = "μ.μ.";
                        //_cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
                        //_cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
                    }
                    break;
                case "en-US":
                default:
                    _cultureInfo = new CultureInfo("en-US");
                    break;               
            }
        }

        private static CultureInfo _cultureInfo;
        public static CultureInfo CultureInfo
        {
            get
            {
                return _cultureInfo;
            }
            set
            {
                _cultureInfo = value;
            }
        }
    }

}
