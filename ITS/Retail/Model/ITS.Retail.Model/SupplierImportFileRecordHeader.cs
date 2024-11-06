using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class SupplierImportFileRecordHeader : ImportFileRecordHeader
    {
        public SupplierImportFileRecordHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SupplierImportFileRecordHeader(Session session)
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

        private SupplierImportFilesSet _SupplierImportSettingsSet;
        [Association("SupplierImportFilesSet-SupplierImportFileRecordHeaders")]
        public SupplierImportFilesSet SupplierImportSettingsSet
        {
            get
            {
                return _SupplierImportSettingsSet;
            }
            set
            {
                SetPropertyValue("SupplierImportSettingsSet", ref _SupplierImportSettingsSet, value);
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

        private DocumentStatus _DocumentStatus;
        private SupplierImportFileRecordHeader _MasterSupplierImportFileRecordHeader;
        public DocumentStatus DocumentStatus
        {
            get
            {
                return _DocumentStatus;
            }
            set
            {
                SetPropertyValue("DocumentStatus", ref _DocumentStatus, value);
            }
        }

        [Association("SupplierImportFileRecordHeader-SupplierImportFileRecordFields"), Aggregated]
        public XPCollection<SupplierImportFileRecordField> SupplierImportFileRecordFields
        {
            get
            {
                return GetCollection<SupplierImportFileRecordField>("SupplierImportFileRecordFields");
            }
        }


        [Association("SupplierImportFileRecordHeader-DetailSupplierImportFileRecordHeaders")]
        public XPCollection<SupplierImportFileRecordHeader> DetailSupplierImportFileRecordHeaders
        {
            get
            {
                return GetCollection<SupplierImportFileRecordHeader>("DetailSupplierImportFileRecordHeaders");
            }
        }

        
        [Association("SupplierImportFileRecordHeader-DetailSupplierImportFileRecordHeaders")]
        public SupplierImportFileRecordHeader MasterSupplierImportFileRecordHeader
        {
            get
            {
                return _MasterSupplierImportFileRecordHeader;
            }
            set
            {
                SetPropertyValue("MasterSupplierImportFileRecordHeader", ref _MasterSupplierImportFileRecordHeader, value);
            }
        }

        public bool IsMasterFileRecord
        {
            get
            {
                return this.MasterSupplierImportFileRecordHeader == null;
            }
        }

        public bool HasFileRecordDetails
        {
            get
            {
                return this.DetailSupplierImportFileRecordHeaders.Count > 0;
            }
        }

        public bool IsDetailFileRecord
        {
            get
            {
                return this.MasterSupplierImportFileRecordHeader != null;
            }
        }
    }
}
