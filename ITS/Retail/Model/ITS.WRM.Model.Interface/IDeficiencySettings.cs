﻿using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDeficiencySettings: ILookUp2Fields, IRequiredOwner
    {
        IDocumentType DeficiencyDocumentType { get; set; }
    }
}