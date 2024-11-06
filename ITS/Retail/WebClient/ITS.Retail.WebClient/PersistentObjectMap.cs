using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient
{
    public class PersistentObjectMap : PlatformPersistentObjectMap
    {
        public PersistentObjectMap()
        {
            this.RegisterType<IOwnerApplicationSettings, OwnerApplicationSettings>();
            this.RegisterType<ICustomer, Customer>();
            this.RegisterType<ICustomerAnalyticTree, CustomerAnalyticTree>();
            this.RegisterType<ICategoryNode, CategoryNode>();
            this.RegisterType<ICustomerCategory, CustomerCategory>();
            this.RegisterType<IDiscountType, DiscountType>();
            this.RegisterType<IDocumentDetail, DocumentDetail>();
            this.RegisterType<IDocumentDetailDiscount, DocumentDetailDiscount>();
            this.RegisterType<IDocumentHeader, DocumentHeader>();
            this.RegisterType<IDocumentPromotion, DocumentPromotion>();
            this.RegisterType<IItemAnalyticTree, ItemAnalyticTree>();
            this.RegisterType<IItemCategory, ItemCategory>();
            this.RegisterType<IPriceCatalog, PriceCatalog>();
            this.RegisterType<IPriceCatalogPromotion, PriceCatalogPromotion>();
            this.RegisterType<IPromotion, Promotion>();
            this.RegisterType<IPromotionApplicationRule, PromotionApplicationRule>();
            this.RegisterType<IPromotionApplicationRuleGroup, PromotionApplicationRuleGroup>();
            this.RegisterType<IPromotionCustomerApplicationRule, PromotionCustomerApplicationRule>();
            this.RegisterType<IPromotionCustomerCategoryApplicationRule, PromotionCustomerCategoryApplicationRule>();
            this.RegisterType<IPromotionDocumentApplicationRule, PromotionDocumentApplicationRule>();
            this.RegisterType<IPromotionExecution, PromotionExecution>();
            this.RegisterType<IPromotionDocumentExecution, PromotionDocumentExecution>();
            this.RegisterType<IPromotionItemApplicationRule, PromotionItemApplicationRule>();
            this.RegisterType<IPromotionItemCategoryApplicationRule, PromotionItemCategoryApplicationRule>();
            this.RegisterType<IPromotionItemCategoryExecution, PromotionItemCategoryExecution>();
            this.RegisterType<IPromotionItemExecution, PromotionItemExecution>();
        }
    }
}