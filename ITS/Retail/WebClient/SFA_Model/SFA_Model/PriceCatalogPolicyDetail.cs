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
    public class PriceCatalogPolicyDetail : BasicObj, IPriceCatalogPolicyDetail
    {
        public PriceCatalogPolicyDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPolicyDetail(Session session)
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
        public bool IsDefault { get; set; }

        public Guid PriceCatalog { get; set; }
        
        public Guid PriceCatalogPolicy { get; set; }
        
        public PriceCatalogSearchMethod PriceCatalogSearchMethod { get; set; }
        
        public int Sort { get; set; }
        [NonPersistent]
        IPriceCatalog IPriceCatalogPolicyDetail.PriceCatalog
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
        IPriceCatalogPolicy IPriceCatalogPolicyDetail.PriceCatalogPolicy
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
        PriceCatalogSearchMethod IPriceCatalogPolicyDetail.PriceCatalogSearchMethod
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