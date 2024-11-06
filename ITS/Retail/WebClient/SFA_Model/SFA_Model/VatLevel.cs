using DevExpress.Xpo;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ITS.Retail.Platform.Enumerations;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 35, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class VatLevel : LookUp2Fields, IVatLevel
    {
        public VatLevel()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatLevel(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatLevel(string code, string description)
            : base()
        {

        }
        public VatLevel(Session session, string code, string description)
            : base(session, code, description)
        {

        }
        public bool IsDefaultLevel { get; set; }
    }
}