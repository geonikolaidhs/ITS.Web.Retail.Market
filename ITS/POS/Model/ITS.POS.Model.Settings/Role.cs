using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Settings
{
    public class Role : LookupField
    {
        public Role()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Role(Session session)
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

        public XPCollection<User> Users
        {
            get
            {
                return GetCollection<User>("Users");
            }
        }


        //public XPCollection<RoleEntityAccessPermision> RoleEntityAccessPermisions
        //{
        //    get
        //    {
        //        return new XPCollection<RoleEntityAccessPermision>(this.Session, new BinaryOperator("Role", Oid, BinaryOperatorType.Equal));
        //    }
        //}
    }
}
