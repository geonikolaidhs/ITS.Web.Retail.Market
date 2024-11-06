using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;
using ITS.Retail.WebClient;

namespace ITS.Retail.WebClient.Helpers
{
    public class BusinessLogic
    {
        public static Decimal Round(decimal value, CompanyNew owner)
        {
            double ownerOwnerApplicationSettingsDisplayDigits = owner!=null && owner.OwnerApplicationSettings!=null ? owner.OwnerApplicationSettings.DisplayDigits : 2;
            return Decimal.Round(Convert.ToDecimal(value), Convert.ToInt32(ownerOwnerApplicationSettingsDisplayDigits), MidpointRounding.AwayFromZero);
        }

        public static String RoundAndStringify(decimal value, CompanyNew owner)
        {
            double ownerOwnerApplicationSettingsDisplayDigits = owner != null && owner.OwnerApplicationSettings != null ? owner.OwnerApplicationSettings.DisplayDigits : 2;
            return Round(value, owner).ToString("N" + ownerOwnerApplicationSettingsDisplayDigits.ToString());
        }

        public static Decimal RoundOnTwoDigits(decimal value)
        {
            double ownerOwnerApplicationSettingsDisplayDigits = 2;
            return Decimal.Round(Convert.ToDecimal(value), Convert.ToInt32(ownerOwnerApplicationSettingsDisplayDigits), MidpointRounding.AwayFromZero);
        }

        public static String RoundAndStringifyOnTwoDigits(decimal value)
        {
            double ownerOwnerApplicationSettingsDisplayDigits =  2;
            return RoundOnTwoDigits(value).ToString("N" + ownerOwnerApplicationSettingsDisplayDigits.ToString());
        }
    }
}