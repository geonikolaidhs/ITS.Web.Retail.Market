using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
     [Updater(Order = 195,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
     [EntityDisplayName("DiscountType", typeof(ResourcesLib.Resources))]
    public class DiscountType : Lookup2Fields, IDiscountType
    {
        public DiscountType()
        {
            
        }
        public DiscountType(Session session)
            : base(session)
        {
            
        }
        public DiscountType(string code, string description)
            : base(code, description)
        {
            
        }
        public DiscountType(Session session, string code, string description)
            : base(session, code, description)
        {
            
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("DiscountType.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        // Fields...
        private bool _DiscardsOtherDiscounts;
        private bool _IsUnique;
        private eDiscountType _EDiscountType;
        private int _Priority;
        private bool _IsHeaderDiscount;
        [DisplayOrder (Order=4)]
        public eDiscountType eDiscountType
        {
            get
            {
                return _EDiscountType;
            }
            set
            {
                SetPropertyValue("eDiscountType", ref _EDiscountType, value);
            }
        }

        [DisplayOrder(Order = 3)]
        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public bool IsUnique
        {
            get
            {
                return _IsUnique;
            }
            set
            {
                SetPropertyValue("IsUnique", ref _IsUnique, value);
            }
        }

        [DisplayOrder(Order = 7)]
        public bool IsHeaderDiscount
        {
            get
            {
                return _IsHeaderDiscount;
            }
            set
            {
                SetPropertyValue("IsHeaderDiscount", ref _IsHeaderDiscount, value);
            }
        }

        [DisplayOrder(Order = 6)]
        public bool DiscardsOtherDiscounts
        {
            get
            {
                return _DiscardsOtherDiscounts;
            }
            set
            {
                SetPropertyValue("DiscardsOtherDiscounts", ref _DiscardsOtherDiscounts, value);
            }
        }


        [Aggregated, Association("DiscountType-DiscountTypeFields")]
        public XPCollection<DiscountTypeField> DiscountTypeFields
        {
            get
            {
                return GetCollection<DiscountTypeField>("DiscountTypeFields");
            }
        }

    }
}
