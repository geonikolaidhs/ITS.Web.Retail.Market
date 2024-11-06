using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Api.ViewModel
{
    public class StockDocumentHeader
    {
        public StockDocumentHeader(DocumentHeader header)
        {
            this.Oid = header.Oid;
            this.CreatedOnTicks = header.CreatedOnTicks;
            this.UpdatedOnTicks = header.UpdatedOnTicks;
            this.Description = header.Description;
            this.Number = header.DocumentNumber;
            Details = new List<StockDetail>();
            this.TotalQty = 0;
            foreach (DocumentDetail dtl in header.DocumentDetails)
            {
                StockDetail sdtl = new StockDetail();
                sdtl.ItemOid = dtl.ItemOid;
                sdtl.Oid = dtl.Oid;
                sdtl.Qty = dtl.Qty;
                sdtl.Description = dtl.CustomDescription;
                sdtl.StockHeaderOid = header.Oid;
                sdtl.Code = dtl.ItemCode;
                this.TotalQty = this.TotalQty + dtl.Qty;
                Details.Add(sdtl);
            }
        }

        public Guid Oid { get; set; }

        public long CreatedOnTicks { get; set; }

        public long UpdatedOnTicks { get; set; }

        public string Description { get; set; }

        public int Number { get; set; }

        public decimal TotalQty { get; set; }


        public List<StockDetail> Details;
    }
}
