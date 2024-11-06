    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class UnreferencedDocumentDetail
    {
        public Barcode Barcode { get; set; }
        public decimal Quantity { get; set; }
        public decimal FinalUnitPrice { get; set; }
        public decimal Discount { get; set; }
        public Guid LinkedLine { get; set; }

        public UnreferencedDocumentDetail(Barcode barcode, decimal finalUnitPrice, decimal quantity, Guid linkedLine, decimal discount)
        {
            Barcode = barcode;
            FinalUnitPrice = finalUnitPrice;
            Quantity = quantity;
            LinkedLine = linkedLine;
            Discount = discount;
        }
    }
}
