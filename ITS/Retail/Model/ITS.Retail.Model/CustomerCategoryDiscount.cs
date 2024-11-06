//-----------------------------------------------------------------------
// <copyright file="CustomerCategoryDiscount.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{

    public class CustomerCategoryDiscount : BaseObj
    {
        public CustomerCategoryDiscount()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerCategoryDiscount(Session session)
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

        private PriceCatalog _PriceCatalog;
        [Association("PriceCatalog-CustomerCategoryDiscounts")]
        public PriceCatalog PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }
        
        private CustomerCategory _CustomerCategory;
        [Association("CustomerCategory-CustomerCategoryDiscounts")]
        public CustomerCategory CustomerCategory
        {
            get
            {
                return _CustomerCategory;
            }
            set
            {
                SetPropertyValue("CustomerCategory", ref _CustomerCategory, value);
            }
        }

        private ItemCategory _ItemCategory;
        [Association("ItemCategory-CustomerCategoryDiscounts")]
        public ItemCategory ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("ItemCategory", ref _ItemCategory, value);
            }
        }

        private double _Discount;
        [DescriptionField]
        public double Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }
    }

}