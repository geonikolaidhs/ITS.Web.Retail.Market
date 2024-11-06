using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Promotions
{
    /// <summary>
    /// A categoryID with the IDs of all it's child categories.
    /// </summary>
    public class DenormalizedCategory
    {

        public DenormalizedCategory(Guid categoryID, IEnumerable<Guid> allChildCategoryIDs)
        {
            this.CategoryID = categoryID;
            this.AllChildCategoryIDs = allChildCategoryIDs;
        }

        public Guid CategoryID { get; set; }
        public IEnumerable<Guid> AllChildCategoryIDs { get; set; }
    }
}
