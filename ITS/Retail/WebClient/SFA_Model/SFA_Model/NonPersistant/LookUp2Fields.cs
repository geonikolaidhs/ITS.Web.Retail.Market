using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using DevExpress.Xpo;


namespace SFA_Model.NonPersistant
{
    [NonPersistent]
    public class LookUp2Fields : LookupField, ILookUp2Fields
    {

        public LookUp2Fields()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LookUp2Fields(Session session)
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
        public LookUp2Fields(Session session, string code, string description)
            : base(session, description)
        {
            _Code = code;
        }
        private string _Code;
        public string Code { get; set; }
        
        public Guid Owner { get; set; }

        public string ReferenceCode { get; set; }

        public bool Update { get; set; }

        string IOwner.Description { get;}

        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}