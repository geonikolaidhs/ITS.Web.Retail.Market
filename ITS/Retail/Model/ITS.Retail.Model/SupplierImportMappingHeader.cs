using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class SupplierImportMappingHeader: LookupField
    {
        public SupplierImportMappingHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public SupplierImportMappingHeader(Session session)
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

        [Association("SupplierImportMappingHeader-SupplierImportMappingDetails"), Aggregated]
        public XPCollection<SupplierImportMappingDetail> SupplierImportMappingDetails
        {
            get
            {
                return GetCollection<SupplierImportMappingDetail>("SupplierImportMappingDetails");
            }
        }

        // Fields...
        private SupplierImportFilesSet _SupplierImportFilesSet;

        [Association("SupplierImportFilesSet-SupplierImportMappingHeaders")]
        public SupplierImportFilesSet SupplierImportFilesSet
        {
            get
            {
                return _SupplierImportFilesSet;
            }
            set
            {
                SetPropertyValue("SupplierImportFilesSet", ref _SupplierImportFilesSet, value);
            }
        }

    }
}
