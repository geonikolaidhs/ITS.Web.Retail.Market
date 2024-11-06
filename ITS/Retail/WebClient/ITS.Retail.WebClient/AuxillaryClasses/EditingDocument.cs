using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.WebClient.ViewModel;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class EditingDocument
    {
        public DocumentHeaderViewModel DocumentHeader { get; set; }
        
        public DocumentDetailViewModel DocumentDetail { get; set; }

        public Store Store { get; protected set; }
        public CompanyNew Owner { get; protected set; }
        public PriceCatalog PriceCatalog { get; protected set; }
        public DocumentType DocumentType { get; protected set; }
        public DocumentSeries DocumentSeries { get; protected set; }
        public DocumentStatus DocumentStatus { get; protected set; }


        public void UpdateRefernces(UnitOfWork XpoSession)
        {
            if (this.DocumentHeader != null)
            {
                this.DocumentStatus = (this.DocumentHeader.Status.HasValue) ? XpoSession.GetObjectByKey<DocumentStatus>(this.DocumentHeader.Status.Value) : null;
                this.DocumentSeries = (this.DocumentHeader.DocumentSeries.HasValue) ? XpoSession.GetObjectByKey<DocumentSeries>(this.DocumentHeader.DocumentSeries.Value) : null;
                this.DocumentType = (this.DocumentHeader.DocumentType.HasValue) ? XpoSession.GetObjectByKey<DocumentType>(this.DocumentHeader.DocumentType.Value) : null;
                this.PriceCatalog = (this.DocumentHeader.PriceCatalog.HasValue) ? XpoSession.GetObjectByKey<PriceCatalog>(this.DocumentHeader.PriceCatalog.Value) : null;
                this.Store = (this.DocumentHeader.Store.HasValue) ? XpoSession.GetObjectByKey<Store>(this.DocumentHeader.Store.Value) : null;
                this.Owner = (this.Store != null) ? Store.Owner : null;
            }
        }
    }
}