using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Model
{
    [Updater(Order = 21, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("TaxOffice", typeof(ResourcesLib.Resources))]

    public class TaxOffice: Lookup2Fields
    {
        private string _Street;
        private string _PostCode;
        private string _Municipality;
        public TaxOffice()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TaxOffice(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null || store == null)
                    {
                        throw new Exception("TaxOffice.GetUpdaterCriteria(); Error: Supplier or Store is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid),new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        [DisplayOrder (Order= 3)]
        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                SetPropertyValue("Street", ref _Street, value);
            }
        }

        [DisplayOrder(Order = 4)]
        public string PostCode
        {
            get
            {
                return _PostCode;
            }
            set
            {
                SetPropertyValue("PostCode", ref _PostCode, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public string Municipality
        {
            get
            {
                return _Municipality;
            }
            set
            {
                SetPropertyValue("Municipality", ref _Municipality, value);
            }
        }
    }
}
