using DevExpress.Data.Filtering;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class SynchronizationInfoViewModel : BasePersistableViewModel
    {
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

        public string SyncInfoEntityDirectionText
        {
            get
            {
                return SyncInfoEntityDirection.ToString();
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

        private long _ExpectedEntityCount;
        public long ExpectedEntityCount
        {
            get
            {
                return _ExpectedEntityCount;
            }
            set
            {
                SetPropertyValue("DeviceEntityCount", ref _ExpectedEntityCount, value);
            }
        }

        public bool IsEntityCountSynchronized
        {
            get
            {
                return DeviceEntityCount == ExpectedEntityCount;
            }
        }

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

        private long _ExpectedVersion;
        public long ExpectedVersion
        {
            get
            {
                return _ExpectedVersion;
            }
            set
            {
                SetPropertyValue("ExpectedVersion", ref _ExpectedVersion, value);
            }
        }

        public bool IsEntityVersionSynchronized
        {
            get
            {
                return DeviceVersion == ExpectedVersion;
            }
        }

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

        public override Type PersistedType
        {
            get { return typeof(SynchronizationInfo); }
        }

        public override void UpdateModel(DevExpress.Xpo.Session uow)
        {
            base.UpdateModel(uow);

            if (this.DeviceType == eIdentifier.STORECONTROLLER && MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
            {

                SynchronizationInfo masterSyncInfo = uow.FindObject<SynchronizationInfo>(CriteriaOperator.And(new BinaryOperator("EntityName", this.EntityName), new BinaryOperator("DeviceType", eIdentifier.MASTER)));
                ExpectedEntityCount = masterSyncInfo == null ? 0 : masterSyncInfo.DeviceEntityCount;
                ExpectedVersion = masterSyncInfo == null ? 0 : masterSyncInfo.DeviceVersion;
            }
            else
            {
                ExpectedEntityCount = SynchronizationInfoHelper.GetEntityExpectedCountOfRemoteDevice(MvcApplication.ApplicationInstance, EntityName, DeviceMinVersion, DeviceOid, DeviceType, SyncInfoEntityDirection);
                ExpectedVersion = SynchronizationInfoHelper.GetEntityExpectedVersionOfRemoteDevice(MvcApplication.ApplicationInstance, EntityName, DeviceOid, DeviceType, SyncInfoEntityDirection);
            }      
        }
    }
}