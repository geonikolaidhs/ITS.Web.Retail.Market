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
    [CreateOrUpdaterOrder(Order = 540, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PhoneType: LookUp2Fields,IPhoneType
    {
        public PhoneType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PhoneType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public PhoneType(string code, string description)
            : base()
        {

        }
        public PhoneType(Session session, string code, string description)
            : base(session, code, description)
        {

        }
    }
}