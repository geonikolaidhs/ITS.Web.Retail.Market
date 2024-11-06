using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class DataViewCategory : Lookup2Fields
    {
        public DataViewCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataViewCategory(Session session)
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

        [Association("DataViewCategory-DataViews")]
        public XPCollection<DataView> DataViews
        {
            get
            {
                return GetCollection<DataView>("DataViews");
            }
        }
    }
}
