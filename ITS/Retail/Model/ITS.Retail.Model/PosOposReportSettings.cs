using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 67,
  Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class PosOposReportSettings : BaseObj
    {
        [EntityDisplayName("PosOposReportSettings", typeof(ResourcesLib.Resources))]
        public PosOposReportSettings() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PosOposReportSettings(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            Guid posId = Guid.Empty;
            Guid.TryParse(deviceID, out posId);
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop = CriteriaOperator.And(new BinaryOperator("POS.Oid", posId));
                    break;
            }
            return crop;
        }




        private POS _POS;
        [UpdaterIgnoreField]
        [Association("POS-PosOposReportSettings")]
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

        private PosReport _PrintFormat;
        [Association("PosReport-PosOposReportSettings")]
        public PosReport PrintFormat
        {
            get
            {
                return _PrintFormat;
            }
            set
            {
                SetPropertyValue("PrintFormat", ref _PrintFormat, value);
            }
        }



        private POSDevice _Printer;
        [UpdaterIgnoreField]
        public POSDevice Printer
        {
            get
            {
                return _Printer;
            }
            set
            {
                SetPropertyValue("Printer", ref _Printer, value);
            }
        }


        private String _PrinterName;
        public String PrinterName
        {
            get
            {
                return _PrinterName;
            }
            set
            {
                SetPropertyValue("_PrinterName", ref _PrinterName, value);
            }
        }
    }
}
