using ITS.WRM.Model.Interface.Model.SupportingClasses;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.WRM.Model.Interface
{
    public interface ITransformationRule: IOwner, IBaseObj
    {
        bool IsDefault { get; set; }
        eTransformationLevel TransformationLevel { get; set; }
        double ValueTransformationFactor { get; set; }
        IDocumentType InitialType { get; set; }
        IDocumentType DerrivedType { get; set; }
        double QtyTransformationFactor { get; set; }
    }
}
