using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// A converted form of a document detail, used to apply promotion filters.
    /// </summary>
    public class DenormalizedDocumentDataLine : ICloneable
    {
        public IDocumentHeader DocumentHeader {get; protected set;}
        public decimal DocumentGrossTotalBeforeDocumentDiscount { get; set; }
        public ICustomer Customer { get; protected set; }
        public Guid Item { get; protected set; }
        public decimal ItemTotalQuantity { get; set; }
        public decimal ItemTotalValue { get; set; }
        public IEnumerable<Guid> ItemCategories { get; protected set; }
        public IEnumerable<Guid> CustomerCategories { get; protected set; }

        public Guid PriceCatalog { get; protected set; }

        public DenormalizedDocumentDataLine(IDocumentHeader header, ICustomer customer, Guid item, IEnumerable<Guid> itemCategories, decimal itemTotalQuantity, decimal itemTotalValue, IEnumerable<Guid> customerCategories, Guid priceCatalog)
        {
            this.DocumentHeader = header;
            this.DocumentGrossTotalBeforeDocumentDiscount = header.GrossTotalBeforeDocumentDiscount;
            this.Customer = customer;
            this.Item = item;
            this.ItemTotalQuantity = itemTotalQuantity;
            this.ItemTotalValue = itemTotalValue;
            this.ItemCategories = itemCategories;
            this.CustomerCategories = customerCategories;
            this.PriceCatalog = priceCatalog;
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}
