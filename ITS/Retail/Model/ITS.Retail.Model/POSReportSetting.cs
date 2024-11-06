using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class POSReportSetting : LookupField
    {
        [EntityDisplayName("POSReportSetting", typeof(ResourcesLib.Resources))]
        public POSReportSetting() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSReportSetting(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private POS _POS;

        [Association("POS-POSReportSettings")]
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

        private POSPrintFormat _PrintFormat;
        public POSPrintFormat PrintFormat
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

        private DocumentType _DocumentType;
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        private CustomReport _Report;
        public CustomReport Report
        {
            get
            {
                return _Report;
            }
            set
            {
                SetPropertyValue("Report", ref _Report, value);
            }
        }

        private POSDevice _Printer;
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
    }
}
