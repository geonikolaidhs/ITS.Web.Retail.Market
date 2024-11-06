using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
        [Updater(Order = 194,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
        [EntityDisplayName("CustomEnumerationValue", typeof(ResourcesLib.Resources))]
    public class CustomEnumerationValue: BaseObj
    {
        public CustomEnumerationValue()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        public CustomEnumerationValue(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
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
                        throw new Exception("CustomEnumerationValue.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("CustomEnumerationDefinition.Owner.Oid", owner.Oid), new NullOperator("CustomEnumerationDefinition.Owner"));
                    break;
            }

            return crop;
        }

        // Fields...
        private CustomEnumerationDefinition _CustomEnumerationDefinition;
        private string _Description;
        private int _Ordering;
        
        [DescriptionField]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public int Ordering
        {
            get
            {
                return _Ordering;
            }
            set
            {
                SetPropertyValue("Ordering", ref _Ordering, value);
            }
        }

        [Association("CustomEnumerationDefinition-CustomEnumerationValues"), Grid(HideFromGrid=true)]
        public CustomEnumerationDefinition CustomEnumerationDefinition
        {
            get
            {
                return _CustomEnumerationDefinition;
            }
            set
            {
                SetPropertyValue("CustomEnumerationDefinition", ref _CustomEnumerationDefinition, value);
            }
        }

    }
}
