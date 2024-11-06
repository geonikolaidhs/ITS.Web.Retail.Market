using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 3,
        Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("POSKeysLayout", typeof(ResourcesLib.Resources))]

	public class POSKeysLayout : Lookup2Fields
	{
		public POSKeysLayout()
			: base()
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

		public POSKeysLayout(Session session)
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
                        throw new Exception("POSKeysLayout.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

		[Association("POS-POSKeysLayouts")]
		public XPCollection<POS> POSs
		{
			get
			{
				return GetCollection<POS>("POSs");
			}
		}

		[Association("POSKeysLayout-POSKeyMappings"),Aggregated]
		public XPCollection<POSKeyMapping> POSKeyMappings
		{
			get
			{
				return GetCollection<POSKeyMapping>("POSKeyMappings");
			}
		}
	}
}
