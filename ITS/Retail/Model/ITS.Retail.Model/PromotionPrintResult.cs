using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1080,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionPrintResult", typeof(ResourcesLib.Resources))]

    public class PromotionPrintResult : PromotionResult
    {
        private string _Message;

         public PromotionPrintResult()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public PromotionPrintResult(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PromotionPrintResult.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetPropertyValue("Message", ref _Message, value);
            }
        }

        public override string Description
        {
            get
            {
                return ResourcesLib.Resources.Print + "(" + this.ExecutionPlan + "): \"" + this.Message+"\"";
            }
        }
    }
}
