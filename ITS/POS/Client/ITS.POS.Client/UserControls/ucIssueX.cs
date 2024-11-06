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
using ITS.POS.Client.Actions;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Issues X
    /// </summary>
    public partial class ucIssueX : ucActionButton
    {
        public ucIssueX()
        {
            Action = eActions.ISSUE_X;
            this.ButtonText = POSClientResources.ISSUE_X;
            InitializeComponent();
        }

        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }


    }
}
