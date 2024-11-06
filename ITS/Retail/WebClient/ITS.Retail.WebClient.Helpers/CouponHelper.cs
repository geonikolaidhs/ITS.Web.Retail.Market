using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class CouponHelper
    {
        public static string GetMaskChunk(string code, string mask, char selector)
        {
            //Chunk mask on selector
            int[] selectorPositions = mask.ToArray().Select((ch, index) => new { Character = ch, Index = index })
                                                    .Where(x => x.Character == selector)
                                                    .Select(x => x.Index).ToArray();

            if (selectorPositions.Length == 0)
            {
                return "";
            }

            //Concatenate code based on previous chunk
            return string.Concat(
                        code.ToArray().Select((ch, index) => new { Character = ch, Index = index })
                            .Where(x => selectorPositions.Contains(x.Index))
                            .Select(x => x.Character))
                        ;

        }

        public static decimal GetMaskAmount(string code, CouponMask couponMask)
        {
            string integralValue = GetMaskChunk(code, couponMask.Prefix + couponMask.Mask, Platform.Common.RetailConstants.CouponMaskSettings.VALUE_INTEGRAL_PART_DIGIT);
            string decimalValue = GetMaskChunk(code, couponMask.Prefix + couponMask.Mask, Platform.Common.RetailConstants.CouponMaskSettings.VALUE_DECIMAL_PART_DIGIT);

            return Decimal.Parse(integralValue + "." + decimalValue, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
