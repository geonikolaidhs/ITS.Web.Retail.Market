﻿using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Kernel;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// Promotion criteria for the document lines' item categories.
    /// </summary>
    public class PromotionItemCategoryCriteria : IPromotionCriteria
    {
        /// <summary>
        /// The minimum of the total quantity that the document lines of the given category must have.
        /// </summary>
        public decimal TotalQuantity { get; protected set; }

        /// <summary>
        /// The minimum of the total value that the document lines of the given category must have.
        /// </summary>
        public decimal TotalValue { get; protected set; }

        /// <summary>
        /// The category to filter with.
        /// </summary>
        public DenormalizedCategory DenormalizedCategory { get; protected set; }//List<Guid> ChildItemCategories { get; protected set; }

        public PromotionItemCategoryCriteria(DenormalizedCategory denormalizedCategory, decimal quantity, decimal value = 0)
        {
            this.DenormalizedCategory = denormalizedCategory;
            this.TotalQuantity = quantity;
            this.TotalValue = value;            
        }

        /// <summary>
        /// Returns true if the document lines of the given category have a 
        /// total quantity or total value greater than the TotalQuantity or TotalValue properties respectively.
        /// </summary>
        /// <param name="denormalizedDocument"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public bool MeetCriteria(List<DenormalizedDocumentDataLine> denormalizedDocument, ePromotionExecutionPriority priority)
        {
            List<DenormalizedDocumentDataLine> result = new List<DenormalizedDocumentDataLine>();

            if (this.TotalQuantity > 0) //Check for quantity 
            {
                var validLines = denormalizedDocument.Where(x => (x.ItemCategories.Contains(this.DenormalizedCategory.CategoryID) || x.ItemCategories.Intersect(this.DenormalizedCategory.AllChildCategoryIDs).Count() > 0));
                var totalQty = validLines.Sum(x=>x.ItemTotalQuantity);
                if(totalQty >=  this.TotalQuantity)
                {
                    result.AddRange(validLines);
                }
            }
            else if (this.TotalValue > 0) // Else check for value
            {
                var validLines = denormalizedDocument.Where(x => (x.ItemCategories.Contains(this.DenormalizedCategory.CategoryID) || x.ItemCategories.Intersect(this.DenormalizedCategory.AllChildCategoryIDs).Count() > 0));
                var totalValue = validLines.Sum(x => x.ItemTotalValue);
                if (totalValue >= this.TotalValue)
                {
                    
                    result.AddRange(validLines);
                }
            }
            else
            {
                throw new Exception("Quantity and Value cannot be both 0 or less at the same time");
            }

            return result.Count > 0;
        }

        /// <summary>
        /// Gets a list of constrains that are generated by applying this criteria.
        /// The total value or total quantity of the matching lines is constrained for use in other promotions.
        /// </summary>
        /// <param name="denormalizedDocument"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public List<IPromotionConstrain> GetConstrains(List<DenormalizedDocumentDataLine> denormalizedDocument, ePromotionExecutionPriority priority)
        {
            List<IPromotionConstrain> constrains = new List<IPromotionConstrain>();
            if (this.TotalQuantity > 0) //Check for quantity 
            {
                constrains.Add(new PromotionItemCategoryConstrain() { DenormalizedCategory = this.DenormalizedCategory, ItemCategoryQuantity = this.TotalQuantity });
            }
            else if (this.TotalValue > 0) // Else check for value
            {
                constrains.Add(new PromotionItemCategoryConstrain() { DenormalizedCategory = this.DenormalizedCategory, ItemCategoryValue = this.TotalValue });
            }
            else
            {
                throw new Exception("Quantity and Value cannot be both 0 or less at the same time");
            }

            return constrains;
        }
    }
}