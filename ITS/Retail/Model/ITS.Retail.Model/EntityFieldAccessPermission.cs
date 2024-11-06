using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model {

    public class EntityFieldAccessPermission : Permission {
        public EntityFieldAccessPermission()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public EntityFieldAccessPermission(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...

        // Fields...
        private bool _IsCollection;
        private EntityAccessPermision _EntityAccessPermission;

        //[Association("EntityAccessPermision-EntityFieldAccessPermissions")]
        public EntityAccessPermision EntityAccessPermission {
            get {
                return _EntityAccessPermission;
            }
            set {
                SetPropertyValue("EntityAccessPermission", ref _EntityAccessPermission, value);
            }
        }


        public bool IsCollection {
            get {
                return _IsCollection;
            }
            set {
                SetPropertyValue("IsCollection", ref _IsCollection, value);
            }
        }


    }

}