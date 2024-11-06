using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// Constrains the document's total value to prevent conflicts with other promotions that affect the total.
    /// </summary>
    public class PromotionDocumentConstrain : IPromotionConstrain
    {
        /// <summary>
        /// The amount to constrain.
        /// </summary>
        public decimal DocumentValue { get; set; }

        public void InsertOrUpdateToList(List<IPromotionConstrain> list)
        {
            IEnumerable<IPromotionConstrain> sameTypeResults = list.Where(x => x.GetType() == this.GetType());
            PromotionDocumentConstrain existing = sameTypeResults.Cast<PromotionDocumentConstrain>().FirstOrDefault();

            if (existing != null)
            {
                if (this.DocumentValue > existing.DocumentValue)
                {
                    existing.DocumentValue = this.DocumentValue;
                }
            }
            else
            {
                list.Add(this);
            }
        }

        /// <summary>
        /// Subtracts the document value from the denormalized document data.
        /// </summary>
        /// <param name="listToApply"></param>
        /// <returns></returns>
        public List<DenormalizedDocumentDataLine> GetLinesAfterConstrain(List<DenormalizedDocumentDataLine> listToApply)
        {
            foreach(DenormalizedDocumentDataLine line in listToApply)
            {
                line.DocumentGrossTotalBeforeDocumentDiscount -= this.DocumentValue;
            }

            return listToApply;
        }

    }
}
