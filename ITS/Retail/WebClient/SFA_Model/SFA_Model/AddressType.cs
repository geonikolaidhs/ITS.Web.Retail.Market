using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 30, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class AddressType:LookUp2Fields
    {
        public AddressType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public AddressType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public AddressType(Session session, string code, string description)
           : base(session, code, description)
        {

        }
    }
}