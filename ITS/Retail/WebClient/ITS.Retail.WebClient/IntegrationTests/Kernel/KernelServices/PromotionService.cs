using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.WebClient.Kernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.Kernel.KernelServices
{
    public class PromotionService : IPromotionService
    {
        protected UnitOfWork ItemUnitOfWork { get; set; }

        IPlatformPromotionService PlatformPromotionService { get; set; }

        public void ClearDocumentPromotions(Platform.Kernel.Model.IDocumentHeader header)
        {
            PlatformPromotionService.ClearDocumentPromotions(header);
        }

        public void ExecutePromotions(Platform.Kernel.Model.IDocumentHeader header, Platform.Kernel.Model.IPriceCatalogPolicy priceCatalogPolicy, Platform.Kernel.Model.IOwnerApplicationSettings ownerApplicationSettings, DateTime activeAtDate, bool includeTestPromotions)
        {
            
            PlatformPromotionService.ExecutePromotions(header, priceCatalogPolicy, ownerApplicationSettings, activeAtDate, includeTestPromotions);
        }

        public void ExecutePromotionResults()
        {
            throw new NotImplementedException();
        }


        public void SetUnitOfWork(UnitOfWork itemUnitOfWork)
        {
            this.ItemUnitOfWork = itemUnitOfWork;
        }
    }
}