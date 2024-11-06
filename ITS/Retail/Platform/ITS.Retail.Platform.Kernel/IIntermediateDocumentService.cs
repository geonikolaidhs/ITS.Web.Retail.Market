using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform
{
    /// <summary>
    /// Exposes platform common document service functions.
    /// </summary>
    public interface IIntermediateDocumentService
    {
        void RecalculateDocumentCosts(IDocumentHeader documentHeader, bool recompute_document_lines = true, bool findValuesFromDatabase = true);//, bool skipHeaderDiscounts = false);
        void RecalculateDocumentDetail(IDocumentDetail detail, IDocumentHeader header);
        void FixPromotionsDocumentDiscountDeviations(IDocumentHeader header);
    }
}
