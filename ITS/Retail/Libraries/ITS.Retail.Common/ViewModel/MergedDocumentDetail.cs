using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class MergedDocumentDetail
    {

        public MergedDocumentDetail(DocumentDetail docDetail, Guid docHeaderOid, int docTypeFactor)
        {
            Oid = Guid.NewGuid();
            ItemOid = docDetail.ItemOid;
            Description = docDetail.CustomDescription;
            ItemCode = docDetail.ItemCode;
            BarcodeCode = docDetail.BarcodeCode;
            Remarks = docDetail.Remarks;
            VatFactor = docDetail.VatFactor;
            IsLinkedLine = docDetail.IsLinkedLine;
            Qty = docDetail.Qty * docTypeFactor;
            DocumentHeaderOid = docHeaderOid;
        }
        public Guid Oid { get; set; }

        public Guid ItemOid { get; set; }

        public string Description { get; set; }

        public string ItemCode { get; set; }

        public string BarcodeCode { get; set; }

        public string Remarks { get; set; }

        public string MeasurementUnit { get; set; }

        public decimal VatFactor { get; set; }

        public bool IsLinkedLine { get; set; }

        public decimal Qty { get; set; }

        public Guid DocumentHeaderOid { get; set; }

    }
}
