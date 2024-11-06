using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model
{
    public interface ILabel:ILookUp2Fields 
    {
        string LabelFileName {get; set;}
        byte[] LabelFile { get; set; }
        bool UseDirectSQL { get; set; }
        string DirectSQL { get; set; }
        int PrinterEncoding { get; set; }
    }
}
