using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IMeasurementUnit: ILookUp2Fields, IRequiredOwner
    {
         eMeasurementUnitType MeasurementUnitType { get; set; }
         bool SupportDecimal { get; set; }
    }
}
