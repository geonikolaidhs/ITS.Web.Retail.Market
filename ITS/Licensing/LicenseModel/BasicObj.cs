using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    [NonPersistent]
    public abstract class BasicObj : XPCustomObject, IXPObject
    {
        public BasicObj()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BasicObj(Session session)
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
            CreatedOnTicks = DateTime.Now.Ticks;
            UpdatedOnTicks = DateTime.Now.Ticks;
            Version = DateTime.MinValue.Ticks;
        }


        protected override void OnSaving()
        {
            _UpdatedOnTicks = DateTime.Now.Ticks;
            _UpdatedOn = DateTime.Now;
            //UpdatedBy = GlobalSettings.User;
            base.OnSaving();
        }

        private Guid _Oid;
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

        public DateTime CreatedOn
        {
            get
            {
                return new DateTime(_CreatedOnTicks);
            }
        }

        private long _CreatedOnTicks;
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

        private DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get
            {
                return new DateTime(_UpdatedOnTicks);
            }
        }

        private long _UpdatedOnTicks;
        public long UpdatedOnTicks
        {
            get
            {
                return _UpdatedOnTicks;
            }
            set
            {
                SetPropertyValue("UpdatedOnTicks", ref _UpdatedOnTicks, value);
            }
        }

        private User _CreatedBy;
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

        private User _UpdatedBy;
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

        private long _Version;
        public long Version
        {
            get
            {
                return _Version;
            }
            set
            {
                SetPropertyValue("Version", ref _Version, value);
            }
        }
    }
}