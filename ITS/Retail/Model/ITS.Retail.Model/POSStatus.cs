using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class POSStatus : LookupField
    {
        public POSStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public POSStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            MachineStatusTicks = 0;
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            //switch (direction)
            //{
            //    case eUpdateDirection.MASTER_TO_STORECONTROLLER:
            //        if (supplier == null)
            //        {
            //            throw new Exception("ItemImage.GetUpdaterCriteria(); Error: Supplier is null");
            //        }
            //        crop = CriteriaOperator.Parse("[<Item>][Oid = ^.ItemOid AND Owner.Oid = '" + supplier.Oid + "']"); //inner join 
            //        break;
            //}

            return crop;
        }
       

        // Fields...
        private string _LastKnownIP;
        private long _MachineStatusTicks;
        private eMachineStatus _MachineStatus;
        private Guid _POSOid;

        public Guid POSOid
        {
            get
            {
                return _POSOid;
            }
            set
            {
                SetPropertyValue("POSOid", ref _POSOid, value);
            }
        }

        [NonPersistent]
        public POS POS{
            get
            {
                if(POSOid  == Guid.Empty){
                    return null;//TOCHECK....
                }
                return Session.GetObjectByKey<POS>(POSOid);
            }
        }


        public long MachineStatusTicks
        {
            get
            {
                return _MachineStatusTicks;
            }
            set
            {
                SetPropertyValue("MachineStatusTicks", ref _MachineStatusTicks, value);
            }
        }

        public DateTime MachineStatusDate
        {
            get
            {
                return new DateTime(MachineStatusTicks);
            }
        }

        public eMachineStatus MachineStatus
        {
            get
            {
                return _MachineStatus;
            }
            set
            {
                SetPropertyValue("MachineStatus", ref _MachineStatus, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string LastKnownIP
        {
            get
            {
                return _LastKnownIP;
            }
            set
            {
                SetPropertyValue("LastKnownIP", ref _LastKnownIP, value);
            }
        }

        //public override void GetData(Session myses, object item)
        //{
        //    base.GetData(myses, item);

        //}
    }
}
