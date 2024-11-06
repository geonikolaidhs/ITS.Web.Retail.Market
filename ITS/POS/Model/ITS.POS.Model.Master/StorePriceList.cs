using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    public class StorePriceList : BaseObj
    {
        public StorePriceList()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StorePriceList(Session session)
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

        private Guid _Store;
        [Indexed("GCRecord", Unique=false)]
        public Guid Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        private Guid _PriceList;
        public Guid PriceList
        {
            get
            {
                return _PriceList;
            }
            set
            {
                SetPropertyValue("PriceList", ref _PriceList, value);
            }
        }

        private bool _IsDefault;
        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }



    }
}
