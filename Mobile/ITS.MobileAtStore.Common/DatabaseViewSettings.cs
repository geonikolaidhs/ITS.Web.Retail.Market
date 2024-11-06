using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{

    public class DatabaseViewSettings : IXmlSubitems
    {
        public String ProductsView { get; set; }
        public String SuppliersView { get; set; }
        public String CustomersView { get; set; }
        public String OffersView { get; set; }
        public String PriceListsView { get; set; }
        public String WarehousesView { get; set; }
    }

}
