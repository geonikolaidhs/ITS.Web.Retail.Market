using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;

using System;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 420, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogDetailTimeValue : BasicObj
    {
        public PriceCatalogDetailTimeValue()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogDetailTimeValue(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.'
        }

        public long TimeValueValidUntil { get; set; }

        public long TimeValueValidFrom { get; set; }

        //public DateTime TimeValueValidFromDate { get; set; }
        //public DateTime TimeValueValidUntilDate { get; set; }

        public decimal TimeValue { get; set; }

        public long TimeValueRange { get; set; }

        public Guid PriceCatalogDetail { get; set; }
    }
}
