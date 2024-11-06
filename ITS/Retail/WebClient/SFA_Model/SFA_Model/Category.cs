using System;
using DevExpress.Xpo;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
   [NonPersistent]
    public class Category : LookupField, IOwner
    {
        public Category()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Category(Session session)
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

        public Guid Owner
        { get; set; }

        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IOwner.Description
        {
            get;
            
        }
    }
}