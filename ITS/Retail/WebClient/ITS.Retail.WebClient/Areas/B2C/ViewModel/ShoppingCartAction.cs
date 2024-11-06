using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.AuxillaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.Areas.B2C.ViewModel
{
    public enum eShoppingCartAction
    {
        REFRESH, ADD, UPDATE, DELETE, ADDFROMWISHLIST
    }
    public class ShoppingCartAction
    {
        [Binding("rs-action")]
        public eShoppingCartAction Action { get; set; }

        [Binding("rs-PriceCatalogDetailOid")]
        public Guid? PriceCatalogDetailGuid { get; set; }

        [Binding("rs-Qty")]
        public decimal? Qty { get; set; }

        [Binding("rs-DocumentDetailOid")]
        public Guid? DocumentDetailOid { get; set; }

    }

}