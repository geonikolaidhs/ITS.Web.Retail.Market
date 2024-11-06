using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface;
using DevExpress.Xpo;

using ITS.Retail.Platform.Enumerations;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 460, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemBarcode : BaseObj, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner, IItemBarcode
    {

        public ItemBarcode()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemBarcode(Session session)
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

        public Guid Barcode { get; set; }

        public string Description
        {
            get;
        }

        public Guid Item { get; set; }

        public Guid MeasurementUnit { get; set; }


        public Guid Owner
        {
            get; set;
        }

        public string PluCode { get; set; }

        public string PluPrefix { get; set; }

        public double RelationFactor { get; set; }

        public Guid Type { get; set; }

        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IMeasurementUnit IItemBarcode.MeasurementUnit
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
        IBarcodeType IItemBarcode.Type
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
        IBarcode IItemBarcode.Barcode
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

        IItem IItemBarcode.Item
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