using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions.ActionParameters
{
	public enum eDisplayTextMode
	{
		NORMAL,
		BLINK
	}


	public class ActionCashierPoleDisplayMessageParams : ActionParameters.ActionParams
	{
		public string[] Lines { get; set; }
		public eDisplayTextMode Mode { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.CASHIER_POLE_DISPLAY_MESSAGE; }
        }

        public ActionCashierPoleDisplayMessageParams(string[] lines, eDisplayTextMode mode = eDisplayTextMode.NORMAL)
		{
			this.Lines = lines;
			this.Mode = mode;
		}
	}
}
