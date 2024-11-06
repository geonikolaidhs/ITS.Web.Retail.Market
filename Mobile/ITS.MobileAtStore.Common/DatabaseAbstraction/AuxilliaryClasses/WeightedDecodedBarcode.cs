using System;
using System.Collections.Generic;
using System.Text;
using ITS.Common.Logging;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction.AuxilliaryClasses
{
    /// <summary>
    /// Weighted Barcode class
    /// </summary>
    public class WeightedDecodedBarcode : DecodedBarcode
    {
        public decimal Weight { get; set; }
        /// <summary>
        /// Decode an input string to the Weighted Barcode
        /// </summary>
        /// <param name="code">The encoded barcode</param>
        /// <returns>Decoded information</returns>
        public static WeightedDecodedBarcode Decode(string code, DecodingPattern pattern)
        {
            WeightedDecodedBarcode decodedBarcode = new WeightedDecodedBarcode();
            string cc = "", pp = "";
            //if (code.Length != pattern.DecodingRule.Length-pattern.StartIndex)
            //{
            //    Logger.Info("WeightedDecodedBarcode", "Decode",
            //        String.Format("The code {0} fullfils the pattern prefix {1} but the code length is {2} which is different from the expected length {3} (Decoding pattern: {4}). Decoding is not performed", code, pattern.Prefix, code.Length, pattern.DecodingRule.Length, pattern.DecodingRule));
            //    return null;
            //}
            for (int i = 0; i < code.Length; i++)
            {
                if (pattern.DecodingRule[i] == 'W')
                {
                    pp += code[i];
                }
                else if (pattern.DecodingRule[i + pattern.StartIndex] == 'C')
                {
                    cc += code[i];
                }
            }
            decodedBarcode.Code = AbstractDataLayer.PadCode(cc, pattern.PaddingSettings);
            decimal dc;
            try
            {
                dc = decimal.Parse(pp);
                int div = MathUtils.Pow(10, Settings.DecodingSettings.WeightDecimalDigits);
                decodedBarcode.Weight = dc / div;
                return decodedBarcode;
            }
            catch (Exception ex)
            {
                Logger.Info("WeightedDecodedBarcode", "Decode",
                    String.Format("The decoding of weight {0} failed. Decoding is not performed. Exception Message:{1}", ex.Message));
            }

            return null;
        }
    }
}
