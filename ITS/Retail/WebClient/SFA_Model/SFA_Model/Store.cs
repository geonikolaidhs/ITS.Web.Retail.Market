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
    [CreateOrUpdaterOrder(Order = 52, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Store : BasicObj, IStore
    {
        public Store() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Store(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }
        public Guid Address { get; set; }
        //public IAddress Address { get; set; }
        public Guid? CentralOid { get; set; }
        public string Code { get; set; }
        public Guid DefaultPriceCatalogPolicy { get; set; }
        public Guid ImageOid { get; set; }

        public bool IsCentralStore { get; set; }

        public string Name { get; set; }
        public Guid Owner
        {
            get;set;
        }

        public Guid ReferenceCompanyOid { get; set; }
        [NonPersistent]
        ICompanyNew IStore.Owner
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
        IAddress IStore.Address
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
        public IPriceCatalog DefaultPriceCatalog
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