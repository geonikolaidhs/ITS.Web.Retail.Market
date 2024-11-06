using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class POSActionLevelsSet : Lookup2Fields
    {
        public POSActionLevelsSet()
			: base()
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

        public POSActionLevelsSet(Session session)
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

        [Association("POSs-POSActionLevelsSet")]
		public XPCollection<POS> POSs
		{
			get
			{
				return GetCollection<POS>("POSs");
			}
		}

        [Association("POSActionLevelsSet-POSActionLevels"), Aggregated]
        public XPCollection<POSActionLevel> POSActionLevels
		{
			get
			{
                return GetCollection<POSActionLevel>("POSActionLevels");
			}
		}
    }
}
