using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace ITS.Retail.Model
{
    public class TemporaryObject : XPBaseObject
    {
        public TemporaryObject()
        {

        }
        public TemporaryObject(Session session)
            : base(session)
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Oid = Guid.NewGuid();
        }

        [Key]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        public User CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                SetPropertyValue("CreatedBy", ref _CreatedBy, value);
            }
        }

        public User UpdatedBy
        {
            get
            {
                return _UpdatedBy;
            }
            set
            {
                SetPropertyValue("UpdatedBy", ref _UpdatedBy, value);
            }
        }

        public long CreatedOnTicks
        {
            get
            {
                return _CreatedOnTicks;
            }
            set
            {
                SetPropertyValue("CreatedOnTicks", ref _CreatedOnTicks, value);
            }
        }

        public long UpdateOnTicks
        {
            get
            {
                return _UpdateOnTicks;
            }
            set
            {
                SetPropertyValue("UpdateOnTicks", ref _UpdateOnTicks, value);
            }
        }

        [Indexed("EntityOid")]
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

        [Indexed(Unique=false)]
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

        [Size(SizeAttribute.Unlimited)]
        public string SerializedData
        {
            get
            {
                return _SerializedData;
            }
            set
            {
                SetPropertyValue("SerializedData", ref _SerializedData, value);
            }
        }

        public DateTime CreatedOn
        {
            get
            {
                return new DateTime(this.CreatedOnTicks);
            }
        }

        public DateTime UpdatedOn
        {
            get
            {
                return new DateTime(this.UpdateOnTicks);
            }
        }

        // Fields...
        private Guid _EntityOid;
        private string _SerializedData;
        private string _EntityType;
        private long _UpdateOnTicks;
        private long _CreatedOnTicks;
        private User _CreatedBy;
        private Guid _Oid;
        private User _UpdatedBy;
    }
}
