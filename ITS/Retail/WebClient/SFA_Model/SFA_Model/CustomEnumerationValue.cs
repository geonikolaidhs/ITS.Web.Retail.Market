using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 194, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CustomEnumerationValue : BaseObj
    {
        public CustomEnumerationValue()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomEnumerationValue(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Guid CustomEnumerationDefinition { get; set; }
        
        //public CustomEnumerationDefinition CustomEnumerationDefinition { get; set; }
        public string Description { get; set; }
        public int Ordering { get; set; }
    }
}
    
