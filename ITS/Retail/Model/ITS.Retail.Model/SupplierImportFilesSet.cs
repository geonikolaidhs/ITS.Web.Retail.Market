using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class SupplierImportFilesSet : Lookup2Fields, IRequiredOwner
    {
     
        public SupplierImportFilesSet()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SupplierImportFilesSet(Session session)
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

        private SupplierNew _Supplier;
        [Association("Supplier-SupplierImportFilesSets")]
        public SupplierNew Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
            }
        }

        private int _CodePage;
        public int CodePage
        {
            get
            {
                return _CodePage;
            }
            set
            {
                SetPropertyValue("CodePage", ref _CodePage, value);
            }
        }


        [Association("SupplierImportFilesSet-SupplierImportFileRecordHeaders"),Aggregated]
        public XPCollection<SupplierImportFileRecordHeader> SupplierImportFileRecordHeaders
        {
            get
            {
                return GetCollection<SupplierImportFileRecordHeader>("SupplierImportFileRecordHeaders");
            }
        }

        [Association("SupplierImportFilesSet-SupplierImportMappingHeaders"), Aggregated]
        public XPCollection<SupplierImportMappingHeader> SupplierImportMappingHeaders
        {
            get
            {
                return GetCollection<SupplierImportMappingHeader>("SupplierImportMappingHeaders");
            }
        }
    }
}
