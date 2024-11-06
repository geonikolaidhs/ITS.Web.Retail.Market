using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IItem : IBasicObj, IRequiredOwner
    {
        IMeasurementUnit PackingMeasurementUnit { get; set; }
        bool AcceptsCustomDescription { get; set; }
        decimal ReferenceUnit { get; set; }
        string Remarks { get; set; }
        decimal Points { get; set; }
        double MinOrderQty { get; set; }
        double PackingQty { get; set; }
        double OrderQty { get; set; }
        double MaxOrderQty { get; set; }
        string Code { get; set; }
        IBarcode DefaultBarcode { get; set; }
        IVatCategory VatCategory { get; set; }
        Guid? MotherCodeOid { get; set; }
        DateTime InsertedDate { get; set; }
        IBuyer Buyer { get; set; }
        ISupplierNew DefaultSupplier { get; set; }
        string Name { get; set; }
        bool IsTax { get; set; }
        bool DoesNotAllowDiscounts { get; set; }
        decimal ContentUnit { get; set; }
        bool IsGeneralItem { get; set; }
        IItemExtraInfo ItemExtraInfo { get; set; }

    }
}
