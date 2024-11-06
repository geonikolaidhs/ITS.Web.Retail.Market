using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IItemStock: IBaseObj
    {
        IStore Store { get; set; }
        IItem Item { get; set; }
        IBarcode Barcode { get; set; }
        double Value { get; set; }
    }
}
