using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Master
{
    public class Customer : BaseObj, ICustomer
    {
        public Customer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Customer(Session session)
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
        private Guid _Trader;
        [Indexed("GCRecord", Unique =false)]
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

        public decimal CollectedPoints
        {
            get
            {
                return _CollectedPoints;
            }
            set
            {
                SetPropertyValue("CollectedPoints", ref _CollectedPoints, value);
            }
        }

        public Guid PriceCatalogPolicy
        {
            get
            {
                return _PriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
            }
        }

        private string _Code;
        [Indexed("GCRecord", Unique = false)]
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

        private string _Loyalty;
        public string Loyalty
        {
            get
            {
                return _Loyalty;
            }
            set
            {
                SetPropertyValue("Loyalty", ref _Loyalty, value);
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

        private double _Discount;
        private decimal _CollectedPoints;
        private string _CardID;
        private Guid _RefundStore;
        private Guid _DefaultAddress;
        private Guid _PriceCatalogPolicy;

        public double Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }

        [Indexed("GCRecord", Unique = false)]
        public string CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                SetPropertyValue("CardID", ref _CardID, value);
            }
        }

        public Guid RefundStore
        {
            get
            {
                return _RefundStore;
            }
            set
            {
                SetPropertyValue("RefundStore", ref _RefundStore, value);
            }
        }

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

        protected override bool UpdateTicksOnSaving
        {
            get { return false; }
        }
    }
}
