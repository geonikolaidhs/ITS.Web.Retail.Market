using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class RoleEntityAccessPermision : BaseObj
    {
        public RoleEntityAccessPermision()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public RoleEntityAccessPermision(Session session)
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

        private Guid _Role;
        public Guid Role
        {
            get
            {
                return _Role;
            }
            set
            {
                SetPropertyValue("Role", ref _Role, value);
            }
        }

        private Guid _EnityAccessPermision;
        public Guid EnityAccessPermision
        {
            get
            {
                return _EnityAccessPermision;
            }
            set
            {
                SetPropertyValue("EnityAccessPermision", ref _EnityAccessPermision, value);
            }
        }

        public EntityAccessPermision EnityAccessPermisionObject
        {
            get
            {
                return this.Session.GetObjectByKey<EntityAccessPermision>(this.EnityAccessPermision);
            }
        }

    }
}
