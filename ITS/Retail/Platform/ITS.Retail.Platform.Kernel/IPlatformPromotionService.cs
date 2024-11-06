using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public interface IPlatformPromotionService : IKernelModule
    {
        void ClearDocumentPromotions(IDocumentHeader header);

        void ExecutePromotions(IDocumentHeader header, IPriceCatalogPolicy priceCatalogPolicy, IOwnerApplicationSettings ownerApplicationSettings, DateTime activeAtDate, bool includeTestPromotions);

        

    }
}
