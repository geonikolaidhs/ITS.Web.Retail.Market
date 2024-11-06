using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;

namespace ITS.POS.Model.Master
{
    public class SupplierNew : BaseObj
    {
        public SupplierNew()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SupplierNew(Session session)
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


        public string Description
        {
            get
            {
                return CompanyName;
            }
        }

        private Guid _Trader;
        public Guid Trader
        {
            get
            {
                return _Trader;
            }
            set
            {
                SetPropertyValue("Trader", ref _Trader, value);
            }
        }

        private string _Code;
        [Indexed(Unique = true)]
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

        private string _CompanyName;
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref _CompanyName, value);
            }
        }
        private Guid _DefaultAddress;
        public Guid DefaultAddress
        {
            get
            {
                return _DefaultAddress;
            }
            set
            {
                SetPropertyValue("DefaultAddress", ref _DefaultAddress, value);
            }
        }
        private string _Profession;
        public string Profession
        {
            get
            {
                return _Profession;
            }
            set
            {
                SetPropertyValue("Profession", ref _Profession, value);
            }
        }
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

        private Guid _VatLevel;
        public Guid VatLevel
        {
            get
            {
                return _VatLevel;
            }
            set
            {
                SetPropertyValue("VatLevel", ref _VatLevel, value);
            }
        }

    }
}
