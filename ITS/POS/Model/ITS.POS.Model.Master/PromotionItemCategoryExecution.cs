using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PromotionItemCategoryExecution : PromotionExecution, IPromotionItemCategoryExecution
    {
        public PromotionItemCategoryExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionItemCategoryExecution(Session session)
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

        private Guid _ItemCategory;
        private decimal _Quantity;
        private bool _OncePerItem;
        private eItemExecutionMode _ExecutionMode;
        private decimal _FinalUnitPrice;

        public Guid ItemCategory
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

        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        public bool OncePerItem
        {
            get
            {
                return _OncePerItem;
            }
            set
            {
                SetPropertyValue("OncePerItem", ref _OncePerItem, value);
            }
        }


        public eItemExecutionMode ExecutionMode
        {
            get
            {
                return _ExecutionMode;
            }
            set
            {
                SetPropertyValue("ExecutionMode", ref _ExecutionMode, value);
            }

        }

        public decimal FinalUnitPrice
        {
            get
            {
                return _FinalUnitPrice;
            }
            set
            {
                SetPropertyValue("FinalUnitPrice", ref _FinalUnitPrice, value);
            }
        }


        public Guid ItemCategoryOid
        {
            get { return ItemCategory; }
        }
    }
}
