using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionSlipPrintParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.SLIP_PRINT; }
        }

        public Device Printer { get; protected set; }
        public string[] Lines { get; protected set; }

        public ActionSlipPrintParams(Device printer, string[] lines)
        {
            this.Printer = printer;
            this.Lines = lines;

        }
    }
}
