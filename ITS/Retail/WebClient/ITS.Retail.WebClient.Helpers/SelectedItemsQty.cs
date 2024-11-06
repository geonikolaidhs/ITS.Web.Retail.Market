using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;
using System.Threading;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Helpers
{
    public class SelectedItemsQty
    {

        public Item item{get;set;}
        public PriceCatalogDetail PriceCatalogDetail(Store store, Customer customer = null)
        {
            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(store, item.Code, customer);
            return priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
        }
        public decimal qty { get; set; }
        public decimal order_qty { get; set; }

        public SelectedItemsQty() { }

        public SelectedItemsQty(Item it, decimal qt, decimal or_qty)
        {
            item = it;
            qty = qt;
            order_qty = or_qty;            
        }
    }
}