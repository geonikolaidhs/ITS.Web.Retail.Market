using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    [EntityDisplayName("ResendObjectRequest", typeof(ResourcesLib.Resources))]
    public class ResendObjectRequest : XPBaseObject //TODO May need modification
    {
        private Guid _Oid;
        private string _EntityName;
        private long _RequestedOnTicks;
        private eUpdateDirection _UpdateDirection;
        private Guid _EntityOid;
        private Guid _TargetDeviceOid;

        public ResendObjectRequest()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ResendObjectRequest(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
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

        [Indexed("EntityName;UpdateDirection", Unique = false)]
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

        public Guid TargetDeviceOid
        {
            get
            {
                return _TargetDeviceOid;
            }
            set
            {
                SetPropertyValue("TargetDeviceOid", ref _TargetDeviceOid, value);
            }
        }

        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }

        public long RequestedOnTicks
        {
            get
            {
                return _RequestedOnTicks;
            }
            set
            {
                SetPropertyValue("RequestedOnTicks", ref _RequestedOnTicks, value);
            }
        }

        public eUpdateDirection UpdateDirection
        {
            get
            {
                return _UpdateDirection;
            }
            set
            {
                SetPropertyValue("UpdateDirection", ref _UpdateDirection, value);
            }
        }
    }
}
