using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionDeleteItemParams : ActionParams
    {
        public DocumentDetail Detail { get; set; }
        public bool ShowDialog { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.DELETE_ITEM; }
        }

        public ActionDeleteItemParams(DocumentDetail detail, bool showDialog)
        {
            Detail = detail;
            ShowDialog = showDialog;
        }
    }
}
