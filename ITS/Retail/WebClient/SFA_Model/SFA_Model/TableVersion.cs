using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    public class TableVersion : XPBaseObject
    {
        [Key]
        public Guid Oid { get; set; }
        public string TableName { get; set; }
        public long Version { get; set; }
        public long UpdatedOnTicks { get; set; }

        public TableVersion()
          : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TableVersion(Session session)
           : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Oid = Guid.NewGuid();
        }
    }
}