using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;

namespace ITS.POS.Model.Settings
{
    public class CustomReport : LookupField
    {
        public CustomReport()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public CustomReport(Session session)
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

        private string _Code;
        private string _Title;
        private byte[] _ReportFile;
        private string _FileName;
        private eCultureInfo _CultureInfo;
        private string _ObjectType;
        private string _ReportType;
        private Guid _Owner;

        public Guid Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        public eCultureInfo CultureInfo
        {
            get
            {
                return _CultureInfo;
            }
            set
            {
                SetPropertyValue("CultureInfo", ref _CultureInfo, value);
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                SetPropertyValue("FileName", ref _FileName, value);
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                SetPropertyValue("Title", ref _Title, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public Byte[] ReportFile
        {
            get
            {
                return _ReportFile;
            }
            set
            {
                SetPropertyValue("ReportFile", ref _ReportFile, value);
            }
        }

        [Indexed("Owner;GCRecord", Unique = true)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        public string ReportType
        {
            get
            {
                return _ReportType;
            }
            set
            {
                SetPropertyValue("ReportType", ref _ReportType, value);
            }
        }

        public string ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                SetPropertyValue("ObjectType", ref _ObjectType, value);
            }
        }

    }
}
