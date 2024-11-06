using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using System.Globalization;
using ITS.POS.Resources;

namespace ITS.POS.Client.Helpers
{
    public static class LocaleHelper
    {
        public static eLocale GetLanguage(string language)
        {
            eLocale currentLocale = eLocale.English;
            switch (language)
            {
                case "el-GR":
                case "el":
                    currentLocale = eLocale.Ελληνικά;
                    break;
                case "en-US":
                default:
                    currentLocale = eLocale.English;
                    break;
            }

            return currentLocale;
        }

        public static string GetLanguageCode(eLocale locale)
        {
            string language = "en-US";
            if (locale == eLocale.Ελληνικά)
            {
                language = "el-GR";
            }
            if (locale == eLocale.English)
            {
                language = "en-US";
            }
            return language;
        }

        public static void SetLocale(string language, string currencySymbol, eCurrencyPattern currencyPattern)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol = currencySymbol;
            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyPositivePattern = (int)currencyPattern;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }
    }
}