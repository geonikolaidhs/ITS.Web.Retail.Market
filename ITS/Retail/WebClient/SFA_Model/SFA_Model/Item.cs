using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface;
using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 420, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Item : BasicObj
    {
        public Item()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public Item(Session session)
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

        public bool AcceptsCustomDescription { get; set; }

        public bool IsTax { get; set; }
        public bool DoesNotAllowDiscounts { get; set; }

        public Guid Buyer { get; set; }

        public string Code { get; set; }

        public decimal ContentUnit { get; set; }

        public Guid DefaultBarcode { get; set; }

        public Guid DefaultSupplier { get; set; }

        public string Description
        {
            get;
        }

        public DateTime InsertedDate { get; set; }

        public bool IsGeneralItem { get; set; }
        public Guid ItemExtraInfo { get; set; }

        public double MaxOrderQty { get; set; }

        public double MinOrderQty { get; set; }

        public Guid? MotherCodeOid { get; set; }

        public string Name { get; set; }

        public double OrderQty { get; set; }

        public Guid Owner
        {
            get; set;
        }

        public Guid PackingMeasurementUnit { get; set; }

        public double Stock { get; set; }

        public double PackingQty { get; set; }

        public decimal Points { get; set; }

        public decimal ReferenceUnit { get; set; }

        public string Remarks { get; set; }

        public Guid VatCategory { get; set; }

        //[NonPersistent]
        //ICompanyNew IOwner.Owner
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //}
        //[NonPersistent]
        //IMeasurementUnit IItem.PackingMeasurementUnit
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //[NonPersistent]
        //IBarcode IItem.DefaultBarcode
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //[NonPersistent]
        //IVatCategory IItem.VatCategory
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //[NonPersistent]
        //IBuyer IItem.Buyer
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //[NonPersistent]
        //ISupplierNew IItem.DefaultSupplier
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //[NonPersistent]
        //IItemExtraInfo IItem.ItemExtraInfo
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
