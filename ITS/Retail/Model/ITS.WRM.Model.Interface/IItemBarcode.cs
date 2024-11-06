using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IItemBarcode: IBaseObj, IRequiredOwner
    {
        string PluPrefix { get; set; }
        string PluCode { get; set; }
        IMeasurementUnit MeasurementUnit { get; set; }
        double RelationFactor { get; set; }
        IBarcodeType Type { get; set; }
        IBarcode Barcode { get; set; }
        IItem Item { get; set; }
    }
}
