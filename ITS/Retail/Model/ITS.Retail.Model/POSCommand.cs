using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Xpo.DB;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{

    public class POSCommand : XPBaseObject
    {
        public POSCommand()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public POSCommand(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Oid = Guid.NewGuid();
            CreatedOnTicks = DateTime.Now.Ticks;
        }


        public POS POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }


        public ePosCommand Command
        {
            get
            {
                return _Command;
            }
            set
            {
                SetPropertyValue("Command", ref _Command, value);
            }
        }


        [Size(SizeAttribute.Unlimited)]
        public string CommandParameters
        {
            get
            {
                return _CommandParameters;
            }
            set
            {
                SetPropertyValue("CommandParameters", ref _CommandParameters, value);
            }
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


        public int Timeout
        {
            get
            {
                return _Timeout;
            }
            set
            {
                SetPropertyValue("Timeout", ref _Timeout, value);
            }
        }

        // Fields...
        private string _CommandParameters;
        private ePosCommand _Command;
        private POS _POS;
        private long _UpdatedOnTicks;
        private long _CreatedOnTicks;
        private User _CreatedBy;
        private Guid _Oid;
        private User _UpdatedBy;
        private int _Timeout;

        protected override void OnSaving()
        {
            base.OnSaving();
            UpdatedOnTicks = DateTime.Now.Ticks;
        }


    }
}
