using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDocumentStatus: ILookUp2Fields, IRequiredOwner
    {
         bool TakeSequence { get; set; }
         bool ReadOnly { get; set; }
         List<IDocumentHeader> DocumentHeaders { get; set; }
         
    }
}
