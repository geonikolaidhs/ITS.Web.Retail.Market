using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDocumentTypeCustomReport: IBaseObj
    {
        IDocumentType DocumentType { get; set; }
        ICustomReport Report { get; set; }
        UserType UserType { get; set; }
        int Duplicates { get; set; }
    }
}
