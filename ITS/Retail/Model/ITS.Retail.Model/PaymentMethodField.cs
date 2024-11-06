using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;

namespace ITS.Retail.Model
{
    [Updater(Order = 530,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PaymentMethodField", typeof(ResourcesLib.Resources))]

    public class PaymentMethodField : CustomField
    {
        public PaymentMethodField()
        {
            
        }

        public PaymentMethodField(Session session)
            : base(session)
        {
            
        }

        // Fields...
        private PaymentMethod _PaymentMethod;
        [Association("PaymentMethod-PaymentMethodFields"), Indexed(Unique = false)]
        public PaymentMethod PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("PaymentMethodField.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("PaymentMethod.Owner.Oid", owner.Oid), new NullOperator("PaymentMethod.Owner"));
                    break;
            }

            return crop;
        }


    }
}
