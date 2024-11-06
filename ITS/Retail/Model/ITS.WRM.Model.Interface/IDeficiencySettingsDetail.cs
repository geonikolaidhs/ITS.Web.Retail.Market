using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDeficiencySettingsDetail: IBasicObj
    {
        IDocumentType DocumentType { get; set; }
        IDeficiencySettings DeficiencySettings { get; set; }
    }
}
