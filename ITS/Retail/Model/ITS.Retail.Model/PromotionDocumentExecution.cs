using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1050,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionDocumentExecution", typeof(ResourcesLib.Resources))]

    public class PromotionDocumentExecution : PromotionExecution, IPromotionDocumentExecution
    {
        private decimal _Points;
        private bool _KeepOnlyPoints;
        public PromotionDocumentExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionDocumentExecution(Session session)
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
                        throw new Exception("PromotionDocumentExecution.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        public bool KeepOnlyPoints
        {
            get
            {
                return _KeepOnlyPoints;
            }
            set
            {
                SetPropertyValue("KeepOnlyPoints", ref _KeepOnlyPoints, value);
            }
        }

        public decimal Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Points", ref _Points, value);
            }
        }

        public override string Description
        {
            get
            {
                if (KeepOnlyPoints )
                {
                    return ResourcesLib.Resources.Points + ": " + this.Points;
                }
                else if (DiscountType != null)
                {
                    return (DiscountType.eDiscountType == eDiscountType.PERCENTAGE)
                        ? String.Format("{0}({1}): {2:P}", ResourcesLib.Resources.Discount, DiscountType.Description, Percentage)
                        : String.Format("{0}({1}): {2}", ResourcesLib.Resources.Discount, DiscountType.Description, Value);
                }

                return base.Description;
            }
        }
         
    }
}
