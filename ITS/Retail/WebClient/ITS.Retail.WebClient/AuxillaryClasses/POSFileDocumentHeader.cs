using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.POS.Model.Transactions;
using DevExpress.Xpo;
using Newtonsoft.Json;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class POSFileDocumentHeader
    {
        public POSFileDocumentHeader(DocumentHeader posDocument,UnitOfWork ITSRetailModelUow)
        {
            this.DocumentHeaderOid = posDocument.Oid;
            this.GrossTotal = posDocument.GrossTotal;
            this.DocumentNumber = posDocument.DocumentNumber;
            this.FiscalDate = posDocument.FiscalDate;
            this.DocumentSeries = posDocument.DocumentSeriesCode;
            this.DocumentType = posDocument.DocumentTypeCode;
            this.Customer = posDocument.CustomerDisplayName;
            //this.Supplier = 

            ITS.Retail.Model.DocumentHeader documentHeader = ITSRetailModelUow.GetObjectByKey<ITS.Retail.Model.DocumentHeader>(posDocument.Oid);
            if (documentHeader == null)
            {
                this.POSFileDocumentHeaderStatus = POSFileDocumentHeaderStatus.New;
                this.ExistingGrossTotal = 0;
            }
            else
            {
                this.ExistingGrossTotal = documentHeader.GrossTotal;
                if (this.Difference == 0)
                {
                    this.POSFileDocumentHeaderStatus = POSFileDocumentHeaderStatus.Existing;
                }
                else
                {
                    this.POSFileDocumentHeaderStatus = POSFileDocumentHeaderStatus.Modified;
                }
            }
        }

        public Guid DocumentHeaderOid { get; set; }

        [JsonIgnore]
        public POSFileDocumentHeaderStatus POSFileDocumentHeaderStatus { get; set; }

        public string Status
        {
            get
            {
                return this.POSFileDocumentHeaderStatus.ToString();
            }
        }

        public decimal GrossTotal { get; set; }
        public decimal ExistingGrossTotal { get; set; }
        public decimal Difference
        {
            get
            {
                return this.GrossTotal - this.ExistingGrossTotal;
            }
        }
        public string DocumentType { get; set; }
        public string DocumentSeries { get; set; }
        public string Customer { get; set; }
        //public string Supplier { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime FiscalDate { get; set; }
    }
}