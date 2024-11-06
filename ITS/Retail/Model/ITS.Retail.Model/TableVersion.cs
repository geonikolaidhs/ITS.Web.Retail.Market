using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model {

    public class TableVersion : BaseObj {
        public TableVersion()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TableVersion(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...
        
        private long _Number;
        private string _EntityName;
        private bool _ForceReload;

        public string EntityName {
            get {
                return _EntityName;
            }
            set {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }

        public long Number
        {
            get {
                return _Number;
            }
            set {
                SetPropertyValue("Number", ref _Number, value);
            }
        }

        public bool ForceReload
        {
            get
            {
                return _ForceReload;
            }
            set
            {
                SetPropertyValue("ForceReload", ref _ForceReload, value);
            }
        }

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    TableVersion tbv = item as TableVersion;
        //    EntityName = tbv.EntityName;
        //    Number = tbv.Number;
        //}

    }

}