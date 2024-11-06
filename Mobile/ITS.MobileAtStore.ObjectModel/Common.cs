using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ITS.MobileAtStore.ObjectModel
{
    public static class Common
    {
        public static readonly string fname_trans = "trans.db";
        public static readonly string fname_items = "items.db";
        public static readonly string fname_tmpProducts = "tmpProducts.db";

        /// <summary>
        /// Initializes the common information needed by other applications such as CultureInfo etc
        /// </summary>
        static Common()
        {
            try
            {
                _cultureInfo = new CultureInfo("el-gr");
            }
            catch (PlatformNotSupportedException)
            {
                _cultureInfo = new CultureInfo("");

                _cultureInfo.NumberFormat.CurrencySymbol = "EUR ";
                _cultureInfo.NumberFormat.CurrencyDecimalDigits = 2;
                _cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
                _cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
                _cultureInfo.NumberFormat.CurrencyGroupSizes[0] = 3;

                _cultureInfo.NumberFormat.NumberDecimalDigits = 3;
                _cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
                _cultureInfo.NumberFormat.NumberGroupSeparator = ".";
                _cultureInfo.NumberFormat.NumberGroupSizes = new int[] { 6, 3 };

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
