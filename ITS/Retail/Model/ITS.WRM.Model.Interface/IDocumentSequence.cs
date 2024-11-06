using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDocumentSequence:ILookUpFields ,IOwner
    {
        IDocumentSeries DocumentSeries { get; set; }
        int DocumentNumber { get; set; }
        
    }
}
