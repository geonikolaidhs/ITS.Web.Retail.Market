using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class UserTypeAccess : BaseObj
    {
        public UserTypeAccess()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserTypeAccess(Session session)
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

        private Guid _User;
        public Guid User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        private Guid _EntityOid;
        [Indexed("User;GCRecord", Unique = true)]
        // Ο συγκεκριμένος user για το συγκεκριμένο entity θα πρέπει να είναι μοναδικό
        public Guid EntityOid
        {
            get
            {
                return _EntityOid;
            }
            set
            {
                SetPropertyValue("EntityOid", ref _EntityOid, value);
            }
        }

        private string _EntityType;
        public string EntityType
        {
            get
            {
                return _EntityType;
            }
            set
            {
                SetPropertyValue("EntityType", ref _EntityType, value);
            }
        }



    }
}
