using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IOwnerImage: ILookUpFields
    {
        int ImageWidth { get; set; }
        int ImageHeight { get; set; }
        string Info { get; set; }
        Guid OwnerApplicationSettingsOid { get; set; }
        Image Image { get; set; }
    }           
}
