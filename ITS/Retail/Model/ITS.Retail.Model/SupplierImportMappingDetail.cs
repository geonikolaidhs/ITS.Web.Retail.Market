using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ITS.Retail.Model
{
    public class SupplierImportMappingDetail: BaseObj
    {
        public SupplierImportMappingDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public SupplierImportMappingDetail(Session session)
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
        private SupplierImportMappingHeader _SupplierImportMappingHeader;
        private string _ReplacedString;
        private string _InitialString;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string InitialString
        {
            get
            {
                return _InitialString;
            }
            set
            {
                SetPropertyValue("InitialString", ref _InitialString, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ReplacedString
        {
            get
            {                
                return _ReplacedString;
            }
            set
            {
                SetPropertyValue("ReplacedString", ref _ReplacedString, value);
            }
        }


        [Association("SupplierImportMappingHeader-SupplierImportMappingDetails")]
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
