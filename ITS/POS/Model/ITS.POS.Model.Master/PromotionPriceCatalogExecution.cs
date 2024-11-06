using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    public class PromotionPriceCatalogExecution : PromotionExecution, IPromotionPriceCatalogExecution
    {
        public PromotionPriceCatalogExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionPriceCatalogExecution(Session session)
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

        private string _PriceCatalogs;
        private bool _OncePerItem;
        private eItemExecutionMode _ExecutionMode;

        [Size(SizeAttribute.Unlimited)]
        public string PriceCatalogs
        {
            get
            {
                return _PriceCatalogs;
            }
            set
            {
                SetPropertyValue("PriceCatalogs", ref _PriceCatalogs, value);
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
    }
}
