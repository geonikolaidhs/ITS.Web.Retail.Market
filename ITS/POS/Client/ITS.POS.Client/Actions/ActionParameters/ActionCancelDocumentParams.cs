using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionCancelDocumentParams : ActionParams
    {
		public bool ShowDialog { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.CANCEL_DOCUMENT; }
        }

        public ActionCancelDocumentParams(bool showDialog)
        {
            this.ShowDialog = showDialog;
        }

    }
}
