using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class POSPrintFormat : LookupField
    {
        public POSPrintFormat()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSPrintFormat(Session session)
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

        // Fields...
        private string _Format;
        private eFormatType _FormatType;
        private Guid _DocumentType;

        [Size(SizeAttribute.Unlimited)]
        public string Format
        {
            get
            {
                return _Format;
            }
            set
            {
                SetPropertyValue("Format", ref _Format, value);
            }
        }

        public eFormatType FormatType
        {
            get
            {
                return _FormatType;
            }
            set
            {
                SetPropertyValue("FormatType", ref _FormatType, value);
            }
        }

        public Guid DocumentType
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

    }
}
