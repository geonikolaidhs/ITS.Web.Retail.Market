using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class EnumTranslationHelper
    {
        public static string Translate(Enum eNum)
        {            
            switch (eNum.GetType().ToString())
            {
                case "ITS.Retail.Platform.Enumerations.eItemCustomPriceOptions":
                    return TranslateEItemCustomPriceOption((eItemCustomPriceOptions)eNum);                    
                default:
                    return "";                    
            }
            //return "";
        }

        private static string TranslateEItemCustomPriceOption(eItemCustomPriceOptions itemCustomPriceOption)
        {
            switch(itemCustomPriceOption.ToString()){
                case "ONPRICE":
                    return ITS.Retail.ResourcesLib.Resources.OnPrice;
                    //break;
                case "ONDICSOUNT":
                    return ITS.Retail.ResourcesLib.Resources.OnDiscount;
                    //break;
                default:
                    return "";
                    //break;
            }
            //return "";
        }
    }

    
}
