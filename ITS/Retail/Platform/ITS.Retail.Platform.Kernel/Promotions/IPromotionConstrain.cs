using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// Represents a contrain that is applied by a promotion execution to the document data, to prevent conflicts between promotions.
    /// </summary>
    public interface IPromotionConstrain
    {
        /// <summary>
        /// Adds or updates the contrain in the given list of constrains. If the same type of constrain exists, then the constrain should be updated.
        /// </summary>
        /// <param name="list"></param>
        void InsertOrUpdateToList(List<IPromotionConstrain> list);

        /// <summary>
        /// Gets the document denormalized data after applying the current constrain. 
        /// If any values of the DenormalizedDocumentDataLine become negative, then a constrain violation is detected.
        /// </summary>
        /// <param name="listToApply"></param>
        /// <returns></returns>
        List<DenormalizedDocumentDataLine> GetLinesAfterConstrain(List<DenormalizedDocumentDataLine> listToApply);
    }

   
}
