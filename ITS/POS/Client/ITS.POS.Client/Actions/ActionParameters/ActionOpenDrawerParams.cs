using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionOpenDrawerParams : ActionParams
    {
        public Drawer Drawer { get; set; }
        public bool IncreaseTotalizor { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.OPEN_DRAWER; }
        }

        public ActionOpenDrawerParams(Drawer drawer,bool increaseTotalizor)
        {
            Drawer = drawer;
            IncreaseTotalizor = increaseTotalizor;
        }

    }
}
