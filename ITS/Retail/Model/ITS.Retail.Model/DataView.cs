using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class DataView : BaseObj, IRequiredOwner
    {
        private string _Description;
        private string _Query;
        private DataViewCategory _Category;
        private CompanyNew _Owner;
        public DataView()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataView(Session session)
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

        public string Query
        {
            get
            {
                return _Query;
            }
            set
            {
                SetPropertyValue("Query", ref _Query, value);
            }
        }

        [Association("DataViewCategory-DataViews")]
        public DataViewCategory Category
        {
            get
            {
                return _Category;
            }
            set
            {
                SetPropertyValue("Category", ref _Category, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }
    }
}
