
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Indices("EntityName;DeviceOid")]
    public class SynchronizationInfo : BaseObj
    {

        public SynchronizationInfo()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SynchronizationInfo(Session session)
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

        private string _EntityName;
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

        private eSyncInfoEntityDirection _SyncInfoEntityDirection;
        public eSyncInfoEntityDirection SyncInfoEntityDirection
        {
            get
            {
                return _SyncInfoEntityDirection;
            }
            set
            {
                SetPropertyValue("SyncInfoEntityDirection", ref _SyncInfoEntityDirection, value);
            }
        }

        private eIdentifier _DeviceType;
        public eIdentifier DeviceType
        {
            get
            {
                return _DeviceType;
            }
            set
            {
                SetPropertyValue("DeviceOid", ref _DeviceType, value);
            }
        }

        private Guid _DeviceOid;
        [Indexed]
        public Guid DeviceOid
        {
            get
            {
                return _DeviceOid;
            }
            set
            {
                SetPropertyValue("DeviceOid", ref _DeviceOid, value);
            }
        }

        private string _DeviceName;
        public string DeviceName
        {
            get
            {
                return _DeviceName;
            }
            set
            {
                SetPropertyValue("DeviceName", ref _DeviceName, value);
            }
        }

        private long _DeviceEntityCount;
        public long DeviceEntityCount
        {
            get
            {
                return _DeviceEntityCount;
            }
            set
            {
                SetPropertyValue("DeviceEntityCount", ref _DeviceEntityCount, value);
            }
        }

        //private long _ExpectedEntityCount;
        //public long ExpectedEntityCount
        //{
        //    get
        //    {
        //        return _ExpectedEntityCount;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DeviceEntityCount", ref _ExpectedEntityCount, value);
        //    }
        //}

        //[NonPersistent]
        //public bool IsEntityCountSynchronized
        //{
        //    get
        //    {
        //        return DeviceEntityCount == ExpectedEntityCount;
        //    }
        //}

        private long _DeviceMinVersion;
        public long DeviceMinVersion
        {
            get
            {
                return _DeviceMinVersion;
            }
            set
            {
                SetPropertyValue("DeviceMinVersion", ref _DeviceMinVersion, value);
            }
        }

        private long _DeviceVersion;
        public long DeviceVersion
        {
            get
            {
                return _DeviceVersion;
            }
            set
            {
                SetPropertyValue("DeviceVersion", ref _DeviceVersion, value);
            }
        }

        //private long _ExpectedVersion;
        //public long ExpectedVersion
        //{
        //    get
        //    {
        //        return _ExpectedVersion;
        //    }
        //    set
        //    {
        //        SetPropertyValue("ExpectedVersion", ref _ExpectedVersion, value);
        //    }
        //}

        //[NonPersistent]
        //public bool IsEntityVersionSynchronized
        //{
        //    get
        //    {
        //        return DeviceVersion == ExpectedVersion;
        //    }
        //}

        private DateTime _LastUpdate;
        public DateTime LastUpdate
        {
            get
            {
                return _LastUpdate;
            }
            set
            {
                SetPropertyValue("LastUpdate", ref _LastUpdate, value);
            }
        }

    }
}
