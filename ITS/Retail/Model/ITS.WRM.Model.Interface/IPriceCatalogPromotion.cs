﻿using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPriceCatalogPromotion: IBaseObj
    {
        IPromotion Promotion { get; set; }
        IPriceCatalog PriceCatalog { get; set; }
    }
}