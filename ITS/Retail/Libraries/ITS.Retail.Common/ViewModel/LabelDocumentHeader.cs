using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Common.ViewModel
{
    public class LabelDocument
    {
        public Guid Oid { get; set; }
        public PriceCatalogDetail PriceCatalogDetail { get; set; }
        public DocumentDetail DocumentDetail { get; set; }
        public int DocumentDetailLineNumber
        {
            get
            {
                if (DocumentDetail == null)
                {
                    return 0;
                }
                return DocumentDetail.LineNumber;
            }
        }
    }
}