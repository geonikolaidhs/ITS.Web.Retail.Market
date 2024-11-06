using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Sets or restores the remote ADHME (DiSign) on error. 
    /// </summary>
    public partial class ucSetFiscalError : ucButton
    {
        /// <summary>
        /// If true the remote ADHME will be set on error, else it will be restored.
        /// </summary>
        public bool SetOnError { get; set; }

        public ucSetFiscalError()
        {
            InitializeComponent();
            this._button.Text = POSClientResources.SET_FISCAL_ON_ERROR;
        }

        private void SetOnErrorButton_Button_Click(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                actionManager.GetAction(eActions.SET_FISCAL_ON_ERROR).Execute(new ActionSetFiscalOnErrorParams(SetOnError));
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "SetOnErrorButton:OnButtonClick,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }

    }
}
