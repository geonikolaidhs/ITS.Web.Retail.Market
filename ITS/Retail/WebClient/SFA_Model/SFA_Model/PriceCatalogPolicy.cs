using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 46, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogPolicy: LookUp2Fields, IRequiredOwner, IPriceCatalogPolicy
    {
        public PriceCatalogPolicy()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogPolicy(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

    }
}