using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 192, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CustomEnumerationDefinition:LookUp2Fields
    {
        public CustomEnumerationDefinition()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomEnumerationDefinition(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

    }
}