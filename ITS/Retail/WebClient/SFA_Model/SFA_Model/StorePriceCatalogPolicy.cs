using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 60, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class StorePriceCatalogPolicy: BaseObj
    {
        public StorePriceCatalogPolicy()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StorePriceCatalogPolicy(Session session)
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
        public Guid Store { get; set; }
        public Guid PriceCatalogPolicy { get; set; }
        public bool IsDefault { get; set; }

        [NonPersistent]
        Store StoreOId
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
        PriceCatalogPolicy PriceCatalogPolicyStoreOId
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