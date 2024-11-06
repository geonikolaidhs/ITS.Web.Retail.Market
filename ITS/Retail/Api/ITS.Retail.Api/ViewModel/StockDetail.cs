using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Api.ViewModel
{
    public class StockDetail
    {
        public Guid Oid { get; set; }
        public Guid ItemOid { get; set; }

        public Guid StockHeaderOid { get; set; }

        public decimal Qty { get; set; }
        public string Description { get; set; }

        public string Code { get; set; }
    }
}
