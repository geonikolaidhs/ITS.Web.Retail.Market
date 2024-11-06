using ITS.Retail.Platform.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Promotions
{
    public class PromotionPriceCatalogConstrain : IPromotionConstrain
    {
        protected Guid [] PriceCatalogs { get;  set; }
        public List<DenormalizedDocumentDataLine> GetLinesAfterConstrain(List<DenormalizedDocumentDataLine> listToApply)
        {
            return listToApply;
        }

        public void InsertOrUpdateToList(List<IPromotionConstrain> list)
        {
            //TODO
            //throw new NotImplementedException();
        }
    }
}
