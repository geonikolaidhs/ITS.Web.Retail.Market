using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Common.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.WebClient.Helpers
{
    public static class MaskHelper
    {
        /// <summary>
        /// Get relevant BarcodeTypes matching given code for type basicObjectType (FullName).Use this ONLY for Web, OM and everything BESIDES POS!!!
        /// </summary>
        /// <param name="code">Given Code</param>
        /// <param name="basicObjectType">Given Model Type.The table to search into. If this is string.Empty then this is not used to filter BarcodeTypes</param>
        /// <returns></returns>
        public static List<BarcodeType> GetMatchingMasks(string code, string basicObjectType)
        {
            CriteriaOperator criteria = string.IsNullOrEmpty(basicObjectType) ? null : new BinaryOperator("EntityType", basicObjectType);

            return new XPCollection<BarcodeType>(XpoHelper.GetNewUnitOfWork(), criteria)
                      .Where(barType => barType.Length == code.Length
                              && code.StartsWith(barType.Prefix)
                              && CustomBarcodeHelper.NonSpecialCharactersMatch(code, barType.Prefix + barType.Mask))
                      .ToList();
        }
    }
}
