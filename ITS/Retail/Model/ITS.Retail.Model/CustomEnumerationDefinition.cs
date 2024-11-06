using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 192,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CustomEnumerationDefinition", typeof(ResourcesLib.Resources))]
    public class CustomEnumerationDefinition : Lookup2Fields
    {
        public CustomEnumerationDefinition()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomEnumerationDefinition(Session session)
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
                        throw new Exception("CustomEnumerationDefinition.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        [Association("CustomEnumerationDefinition-CustomEnumerationValues"),Grid(HideFromGrid=true), Aggregated]
        public XPCollection<CustomEnumerationValue> CustomEnumerationValues
        {
            get
            {
                return GetCollection<CustomEnumerationValue>("CustomEnumerationValues");
            }
        }
        
    }
}
