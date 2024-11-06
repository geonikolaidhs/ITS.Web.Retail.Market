using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 660, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogDetail : BaseObj, IPriceCatalogDetail
    {
        public PriceCatalogDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.'
            ValueChangedOn = CreatedOnTicks;
            MarkUp = 0;
            long yesterday = DateTime.Now.AddDays(-1).Ticks;
            TimeValueValidFrom = yesterday;
            TimeValueValidUntil = yesterday;
        }

        public Guid Barcode { get; set; }
        [Persistent("Value")]
        public decimal DatabaseValue { get; set; }

        public decimal Discount { get; set; }

        public Guid Item { get; set; }
        
        public bool LabelPrinted { get; set; }

        public long LabelPrintedOn { get; set; }

        public decimal MarkUp { get; set; }

        public decimal OldTimeValue { get; set; }

        public long OldTimeValueValidFrom { get; set; }

        public long OldTimeValueValidUntil { get; set; }

        public decimal OldValue { get; set; }

        public Guid PriceCatalog { get; set; }
        
        public decimal TimeValue { get; set; }

        public long TimeValueChangedOn { get; set; }

        public long TimeValueValidFrom { get; set; }

        public long TimeValueValidUntil { get; set; }

        public decimal UnitValue { get; set; }

        public long ValueChangedOn { get; set; }

        public bool VATIncluded { get; set; }
        [NonPersistent]
        IPriceCatalog IPriceCatalogDetail.PriceCatalog
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IItem IPriceCatalogDetail.Item
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IBarcode IPriceCatalogDetail.Barcode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}