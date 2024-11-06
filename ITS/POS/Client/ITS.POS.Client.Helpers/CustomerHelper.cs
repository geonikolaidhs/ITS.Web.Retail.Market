using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Master;
using DevExpress.Xpo;


namespace ITS.POS.Client.Helpers
{
    public class CustomerHelper
    {

        public static PriceCatalog GetPriceCatalog(Guid customer, Guid store)
        {
            var storePriceList = new XPCollection<StorePriceList>(SessionHelper.GetSession<StorePriceList>(), new BinaryOperator("Store", store));
            ///.Where(x=>x.Store == store);
            var customerPriceList = new XPCollection<CustomerStorePriceList>(SessionHelper.GetSession<CustomerStorePriceList>(), new BinaryOperator("Customer", customer));
            //.Where(x => x.Customer == customer);

            Guid pc = (from stpl in storePriceList
                       join cstpl in customerPriceList on stpl.Oid equals cstpl.StorePriceList
                      // where stpl.Store == store && cstpl.Customer == customer
                       select stpl.PriceList).FirstOrDefault();

            //String Query = "select PriceList from storepricelist inner join CustomerStorePriceList on storepricelist.Oid=CustomerStorePriceList.storepricelist where Customer = '8cb654db-c48e-4f6b-9fb1-ed1359008947' and Store = 'b55fd5e1-a3ac-40d0-bd83-38f246574e17'";
            PriceCatalog pcobj =  pc != null ? SessionHelper.GetObjectByKey<PriceCatalog>(pc) : null;
            if(pcobj == null)
            {
                Store storeobj = SessionHelper.GetObjectByKey<Store>(store);
                if (storeobj != null)
                {
                    pcobj = SessionHelper.GetObjectByKey<PriceCatalog>(storeobj.DefaultPriceCatalog);
                }
            }
            return pcobj;
        }

        public static Customer SearchCustomer(string lookupString)
        {
            ////Search by CardID
            Customer cust = SessionHelper.FindObject<Customer>(new BinaryOperator("CardID", lookupString));

            ////Search by Tax Code
            if (cust == null)
            {
                Trader trader = SessionHelper.FindObject<Trader>(new BinaryOperator("TaxCode", lookupString));
                if (trader != null)
                {
                    cust = SessionHelper.FindObject<Customer>(new BinaryOperator("Trader", trader.Oid));
                }
            }
            ////Search by code
            if (cust == null)
            {
                cust = SessionHelper.FindObject<Customer>(new BinaryOperator("Code", lookupString));
            }


            return cust;
        }
    }
}
