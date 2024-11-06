using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class OwnerCategories : LookupField
    {
        public OwnerCategories()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public OwnerCategories(Session session)
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

        // Fields...
        private CategoryNode _CategoryNode;
        private CompanyNew _Owner;

        [Indexed("CategoryNode;GCRecord", Unique = true), Association("Supplier-OwnerCategories")]
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

        [Association("CategoryNode-OwnerCategories")]
        public CategoryNode CategoryNode
        {
        	get
        	{
        		return _CategoryNode;
        	}
        	set
        	{
        	  SetPropertyValue("CategoryNode", ref _CategoryNode, value);
        	}
        }
    }
}
