using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common
{
    public static class VatValidator
    {
        public static bool CheckVAT(String VatNo)
        {
	        
            long vat;
            if (VatNo.Length == 9 && Int64.TryParse(VatNo, out vat))
            {
                long totalval = 0;
                for (int i = 7; i >= 0; i--)
                {
                    int v = VatNo[i] - 48;
                    totalval += (long)(v * Math.Pow(2, 8 - i));
                }
                int c = VatNo[8] - 48;
                return c == totalval % 11;
            }
            return false;
        }
    }
}
