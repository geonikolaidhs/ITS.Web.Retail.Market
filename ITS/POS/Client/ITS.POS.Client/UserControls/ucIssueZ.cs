using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Issues Z
    /// </summary>
    public partial class ucIssueZ : ucActionButton
    {
        public ucIssueZ()
        {
            InitializeComponent();
            this.Action = eActions.ISSUE_Z;
            this.ButtonText = POSClientResources.ISSUE_Z;//"ISSUE Z";
        }

        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }


    }
}
