using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// Constrains the document lines' quantity or value, for the given item category.
    /// </summary>
    public class PromotionItemCategoryConstrain : IPromotionConstrain
    {
        /// <summary>
        /// The item category of the constrain.
        /// </summary>
        public DenormalizedCategory DenormalizedCategory { get; set; }

        /// <summary>
        /// The total value to constrain.
        /// </summary>
        public decimal ItemCategoryValue { get; set; }

        /// <summary>
        /// The total quantity to constrain.
        /// </summary>
        public decimal ItemCategoryQuantity { get; set; }

        public void InsertOrUpdateToList(List<IPromotionConstrain> list)
        {
            IEnumerable<IPromotionConstrain> sameTypeResults = list.Where(x => x.GetType() == this.GetType());
            PromotionItemCategoryConstrain existing = sameTypeResults.Cast<PromotionItemCategoryConstrain>().Where(x => x.DenormalizedCategory.CategoryID == this.DenormalizedCategory.CategoryID).FirstOrDefault();

            if (existing != null)
            {
                if (this.ItemCategoryValue > existing.ItemCategoryValue)
                {
                    existing.ItemCategoryValue = this.ItemCategoryValue;
                }
                if (this.ItemCategoryQuantity > existing.ItemCategoryQuantity)
                {
                    existing.ItemCategoryQuantity = this.ItemCategoryQuantity;
                }
            }
            else
            {
                list.Add(this);
            }
        }

        /// <summary>
        /// Subtracts the ItemCategoryValue and the ItemCategoryQuantity from the denormalized document data 
        /// that match the item category of the constrain.
        /// </summary>
        /// <param name="listToApply"></param>
        /// <returns></returns>
        public List<DenormalizedDocumentDataLine> GetLinesAfterConstrain(List<DenormalizedDocumentDataLine> listToApply)
        {
            var validLines = listToApply.Where(x => (x.ItemCategories.Contains(this.DenormalizedCategory.CategoryID) || x.ItemCategories.Intersect(this.DenormalizedCategory.AllChildCategoryIDs).Count() > 0));
            if (validLines.Count() > 0)
            {


                if (this.ItemCategoryQuantity > 0)
                {
                    decimal totalValidLinesQty = validLines.Sum(x => x.ItemTotalQuantity);
                    bool qtyGoesNegative = (totalValidLinesQty - this.ItemCategoryQuantity) < 0;

                    decimal remainingQty = this.ItemCategoryQuantity;
                    foreach (var line in validLines)
                    {
                        decimal remainingQtyResult = remainingQty - line.ItemTotalQuantity;
                        if (remainingQtyResult >= 0 && qtyGoesNegative == false)
                        {
                            line.ItemTotalQuantity = 0;
                        }
                        else
                        {
                            line.ItemTotalQuantity -= remainingQty;
                        }
                        remainingQty = remainingQtyResult;
                        if (remainingQty <= 0)
                        {
                            break;
                        }
                    }
                }

                if (this.ItemCategoryValue > 0)
                {
                    decimal totalValidLinesValue = validLines.Sum(x => x.ItemTotalValue);
                    bool valueGoesNegative = (totalValidLinesValue - this.ItemCategoryValue) < 0;

                    decimal remainingValue = this.ItemCategoryValue;
                    foreach (var line in validLines)
                    {
                        decimal remainingValueResult = remainingValue - line.ItemTotalValue;
                        if (remainingValueResult >= 0 && valueGoesNegative == false)
                        {
                            line.ItemTotalValue = 0;
                        }
                        else
                        {
                            line.ItemTotalValue -= remainingValue;
                        }
                        remainingValue = remainingValueResult;
                        if (remainingValue <= 0)
                        {
                            break;
                        }
                    }
                }
            }

            return listToApply;

        }
    }
}
