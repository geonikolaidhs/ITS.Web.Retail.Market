using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class SupplierImportFileRecordField : ImportFileRecordField
    {
         public SupplierImportFileRecordField()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public SupplierImportFileRecordField(Session session)
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


        private SupplierImportMappingHeader _SupplierImportMappingHeader;
        private SupplierImportFileRecordHeader _SupplierImportFileRecordHeader;
        [Association("SupplierImportFileRecordHeader-SupplierImportFileRecordFields")]
        public SupplierImportFileRecordHeader SupplierImportFileRecordHeader
        {
            get
            {
                return _SupplierImportFileRecordHeader;
            }
            set
            {
                SetPropertyValue("SupplierImportFileRecordHeader", ref _SupplierImportFileRecordHeader, value);
            }
        }



        public SupplierImportMappingHeader SupplierImportMappingHeader
        {
            get
            {
                return _SupplierImportMappingHeader;
            }
            set
            {
                SetPropertyValue("SupplierImportMappingHeader", ref _SupplierImportMappingHeader, value);
            }
        }


    }
}
