using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class PosOposReportSettings : BaseObj
    {
        public PosOposReportSettings()
         : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PosOposReportSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

        }


        private Guid _PrintFormat;
        public Guid PrintFormat
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


        private String _PrinterName;
        public String PrinterName
        {
            get
            {
                return _PrinterName;
            }
            set
            {
                SetPropertyValue("PrinterName", ref _PrinterName, value);
            }
        }
    }
}
