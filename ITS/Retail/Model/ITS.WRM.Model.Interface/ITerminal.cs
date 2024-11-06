using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ITerminal: ILookUpFields
    {
        string Name { get; set; }
        string IPAddress { get; set; }
        string Remarks { get; set; }
        IStore Store { get; set; }
    }
}
