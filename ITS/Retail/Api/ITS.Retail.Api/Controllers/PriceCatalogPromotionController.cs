using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITS.Retail.Api.Controllers
{
    public class PriceCatalogPromotionController : BaseODataController< PriceCatalogPromotion>
    {
        public PriceCatalogPromotionController (IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /// <summary>
        /// Returns all price catalogs of specific promotion ( guid )
        /// </summary>
        /// <param name="Promotion"> Promotion guid </param>
        /// <returns></returns>
       /* public IQueryable<PriceCatalogPromotionViewModel> GetPriceCatalogsByPromotions(Guid Promotion)
        {
            var Ws = db.Query<PriceCatalogPromotion>().Where(x => x.Promotion.Oid == Promotion)
                                        .Select(s => new PriceCatalogPromotionViewModel(s));
            return Ws;
        }


        /// <summary>
        /// returns all promotion of specific priceCatalog ( guid )
        /// </summary>
        /// <param name="PriceCatalog">price catalog guid</param>
        /// <returns></returns>
        public IQueryable<PriceCatalogPromotionViewModel> GetPromotionsByPriceCatalogs(Guid PriceCatalog)
        {
            var Ws = db.Query<PriceCatalogPromotion>().Where(x => x.PriceCatalog.Oid == PriceCatalog)
                                        .Select(s => new PriceCatalogPromotionViewModel(s));
            return Ws;
        }*/
    }
}
