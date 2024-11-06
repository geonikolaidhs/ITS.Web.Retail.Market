using ITS.Retail.Mobile.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDocumentTypeMapping
    {
        IDocumentType DocumentType { get; set; }
        eDocumentType EDocumentType { get; set; }
    }
}
