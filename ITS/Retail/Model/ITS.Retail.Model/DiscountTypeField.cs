using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.NonPersistant;

namespace ITS.Retail.Model
{
     [Updater(Order = 199,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
     [EntityDisplayName("DiscountTypeField", typeof(ResourcesLib.Resources))]
    public class DiscountTypeField : CustomField
    {
        public DiscountTypeField()
        {
            
        }
        public DiscountTypeField(Session session)
            : base(session)
        {
            
        }


        // Fields...
        private DiscountType _DiscountType;

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("DiscountTypeField.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("DiscountType.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }
        
        [Association("DiscountType-DiscountTypeFields"), Indexed(Unique = false),Grid(HideFromGrid=true)]
        public DiscountType DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }

    }
}
