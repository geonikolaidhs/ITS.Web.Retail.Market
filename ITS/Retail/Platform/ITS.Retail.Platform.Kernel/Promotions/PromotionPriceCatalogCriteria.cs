using ITS.Retail.Platform.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Platform.Kernel.Promotions
{
    public class PromotionPriceCatalogCriteria : IPromotionCriteria
    {
        public string PriceCatalogs { get; protected set; }

        protected Guid[] PriceCatalogOids { get; private set; }

        public PromotionPriceCatalogCriteria(string priceCatalogs)
        {
            this.PriceCatalogs = priceCatalogs;
            try
            {
                PriceCatalogOids = PriceCatalogs.Split(',').Select(oidString => Guid.Parse(oidString)).ToArray();
            }
            catch (Exception ex)
            {
                PriceCatalogOids = new Guid[0];
            }
        }

        public List<IPromotionConstrain> GetConstrains(List<DenormalizedDocumentDataLine> denormalizedDocument, ePromotionExecutionPriority priority)
        {
            return new List<IPromotionConstrain>();
        }

        public bool MeetCriteria(List<DenormalizedDocumentDataLine> denormalizedDocument, ePromotionExecutionPriority priority)
        {
            if (PriceCatalogOids.Length == 0)
            {
                return false;
            }
            //throw new NotImplementedException();
            return denormalizedDocument.Any(detail => PriceCatalogOids.Contains(detail.PriceCatalog));
        }
    }
}
