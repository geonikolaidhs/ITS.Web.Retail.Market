using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// Constrains the document lines' quantity or value, for the given item.
    /// </summary>
    public class PromotionItemConstrain : IPromotionConstrain
    {
        /// <summary>
        /// The item of the constrain.
        /// </summary>
        public Guid Item { get; set; }

        /// <summary>
        /// The total value to constrain.
        /// </summary>
        public decimal ItemValue { get; set; }

        /// <summary>
        /// The total quantity to constrain.
        /// </summary>
        public decimal ItemQuantity { get; set; }

        public void InsertOrUpdateToList(List<IPromotionConstrain> list)
        {
            IEnumerable<IPromotionConstrain> sameTypeResults = list.Where(x => x.GetType() == this.GetType());
            PromotionItemConstrain existing = sameTypeResults.Cast<PromotionItemConstrain>().Where(x => x.Item == this.Item).FirstOrDefault();

            if (existing != null)
            {
                if (this.ItemValue > existing.ItemValue)
                {
                    existing.ItemValue = this.ItemValue;
                }
                if (this.ItemQuantity > existing.ItemQuantity)
                {
                    existing.ItemQuantity = this.ItemQuantity;
                }
            }
            else
            {
                list.Add(this);
            }
        }

        /// <summary>
        /// Subtracts the ItemValue and the ItemQuantity from the denormalized document data 
        /// that match the item of the constrain.
        /// </summary>
        /// <param name="listToApply"></param>
        /// <returns></returns>
        public List<DenormalizedDocumentDataLine> GetLinesAfterConstrain(List<DenormalizedDocumentDataLine> listToApply)
        {
            var lineToApply = listToApply.Where(x => x.Item == this.Item).FirstOrDefault();
            if (lineToApply != null)
            {
                lineToApply.ItemTotalQuantity -= this.ItemQuantity;
                lineToApply.ItemTotalValue -= this.ItemValue;
            }
            return listToApply;

        }
    }
}
