using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public abstract class ActionParams
    {
        public abstract eActions ActionCode
        {
            get;
        }
    }
}
