using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{
    public class DocumentDetailAssociation
    {
        public Guid UniqueKey { get { return documentDetail.Oid; } }
        public DocumentDetail documentDetail { get; set; }
        public decimal Quantity { get; set; }
        public decimal RetrievedQuantity { get; set; }
        public bool IsSelected { get; set; }

        public string DocumentHeaderInfo
        {
            get
            {
                return documentDetail.DocumentHeader.DocumentType.Code + " " + documentDetail.DocumentHeader.DocumentNumber + " " + documentDetail.DocumentHeader.FinalizedDate.ToShortDateString();
            }
        }

        public bool SupportsDecimal
        {
            get
            {
                CompanyNew owner = documentDetail.DocumentHeader.Owner;
                return documentDetail.Barcode.MeasurementUnit(owner) == null ? false : documentDetail.Barcode.MeasurementUnit(owner).SupportDecimal;
            }
        }

        public DocumentDetailAssociation(DocumentDetail documentDetailValue, decimal quantity)
        {
            documentDetail = documentDetailValue;
            Quantity = quantity;
            RetrievedQuantity = Quantity;
            IsSelected = true;
        }

        public void RetrieveAllQuantity()
        {
            RetrievedQuantity = Quantity;
            IsSelected = true;
        }

        public void RetrieveNothing()
        {
            RetrievedQuantity = 0.0m;
            IsSelected = false;
        }
    }
}
