using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 950, Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class ElectronicJournalFilePackage : BaseObj , IOwner
    {
         public ElectronicJournalFilePackage()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public ElectronicJournalFilePackage(Session session)
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

        private Store _Store;
        [DisplayOrder(Order = 1)]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        private POS _POS;
        [DisplayOrder(Order = 2)]
        [Association("POS-ElectronicJournalFilePackages")]
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

        private int _ZReportNumber;
        [DisplayOrder(Order = 3)]
        public int ZReportNumber
        {
            get
            {
                return _ZReportNumber;
            }
            set
            {
                SetPropertyValue("ZReportNumber", ref _ZReportNumber, value);
            }
        }

        private byte[] _PackageData;
        [Size(SizeAttribute.Unlimited)]
        public byte[] PackageData
        {
            get
            {
                return _PackageData;
            }
            set
            {
                SetPropertyValue("PackageData", ref _PackageData, value);
            }
        }

        private DateTime _Date;
        [DisplayOrder(Order = 4)]
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                SetPropertyValue("Date", ref _Date, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public string Description
        {
            get { return (POS != null ? POS.Name : "") + " - Z " +ZReportNumber; }
        }

        [DisplayOrder (Order = 6)]
        public CompanyNew Owner
        {
            get { return Store == null ? null : Store.Owner; }
        }
    }
}
