using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class Store : BaseObj
    {
        public Store()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Store(Session session)
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
        private Guid _DefaultPriceCatalog;
        private Guid _Owner;
        private Guid _Address;
        private Guid _DefaultPriceCatalogPolicy;
        private Guid _ReferenceCompany;
        private string _Code;
        private string _Name;

        [Indexed("GCRecord", Unique = true)]
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

        
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

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



        public Guid DefaultPriceCatalog
        {
            get
            {
                return _DefaultPriceCatalog;
            }
            set
            {
                SetPropertyValue("DefaultPriceCatalog", ref _DefaultPriceCatalog, value);
            }
        }

        public Guid DefaultPriceCatalogPolicy
        {
            get
            {
                return _DefaultPriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("DefaultPriceCatalogPolicy", ref _DefaultPriceCatalogPolicy, value);
            }
        }


        public Guid Address
        {
            get
            {
                return _Address;
            }
            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

        public Guid ReferenceCompany
        {
            get
            {
                return _ReferenceCompany;
            }
            set
            {
                SetPropertyValue("ReferenceCompany", ref _ReferenceCompany, value);
            }
        }
    }
}
