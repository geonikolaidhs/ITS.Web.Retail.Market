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
    }
}
