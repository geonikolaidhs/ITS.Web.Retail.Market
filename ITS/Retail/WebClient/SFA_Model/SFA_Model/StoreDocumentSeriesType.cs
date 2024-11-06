using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 240, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class StoreDocumentSeriesType : BasicObj, IStoreDocumentSeriesType
    {
        public StoreDocumentSeriesType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDocumentSeriesType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public Guid DefaultCustomer { get; set; }
        [NonPersistent]
        public ICustomReport DefaultCustomReport { get; set; }

        public Guid DefaultSupplier { get; set; }

        public Guid DocumentSeries { get; set; }

        public Guid DocumentType { get; set; }

        public int Duplicates { get; set; }

        public string MenuDescription { get; set; }

        public eStoreDocumentType StoreDocumentType { get; set; }

        public UserType UserType { get; set; }
        [NonPersistent]
        IDocumentSeries IStoreDocumentSeriesType.DocumentSeries
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ICustomer IStoreDocumentSeriesType.DefaultCustomer
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ISupplierNew IStoreDocumentSeriesType.DefaultSupplier
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentType IStoreDocumentSeriesType.DocumentType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        UserType IStoreDocumentSeriesType.UserType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}