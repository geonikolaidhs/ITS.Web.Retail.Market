using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class CompanyNew : BaseObj
    {
        public CompanyNew()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CompanyNew(Session session)
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
        public XPCollection<Store> Stores
        {
            get
            {
                return new XPCollection<Store>(this.Session, new BinaryOperator("Supplier", Oid, BinaryOperatorType.Equal)); 
                    //GetCollection<Store>("Stores");
            }
        }

    }
}
