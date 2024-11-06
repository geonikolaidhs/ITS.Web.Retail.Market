using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Settings
{
    public class VatCategory : Lookup2Fields
    {
        public VatCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatCategory(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatCategory(string code, string description)
            : base(code, description)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private eMinistryVatCategoryCode _MinistryVatCategoryCode;
        public eMinistryVatCategoryCode MinistryVatCategoryCode
        {
            get
            {
                return _MinistryVatCategoryCode;
            }
            set
            {
                SetPropertyValue("MinistryVatCategoryCode", ref _MinistryVatCategoryCode, value);
            }

        }
    }
}
