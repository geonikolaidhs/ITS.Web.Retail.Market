using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class DocTypeCustomerCategory : BaseObj
    {
        private Guid _DocumentType;
        private Guid _CustomerCategory;
        public DocTypeCustomerCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocTypeCustomerCategory(Session session)
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

        [Association("DocumentType-DocTypeCustomerCategories")]
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

        public Guid CustomerCategory
        {
            get
            {
                return _CustomerCategory;
            }
            set
            {
                SetPropertyValue("CustomerCategory", ref _CustomerCategory, value);
            }
        }
    }
}
