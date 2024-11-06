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
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class ReadFiscalMemoryButton : ucitsButton
    {
        public bool UseDateFilter { get; set; }
        public eReprintZReportsMode Mode { get; set; }

        public ReadFiscalMemoryButton()
        {
            InitializeComponent();
            this._button.Text = POSClientResources.REPRINT_Z_REPORTS;

        }

        private void ReadFiscalMemoryButton_Click(object sender, EventArgs e)
        {
            ReadFiscalMemoryButton_Button_Click(_button, e);
        }

        private void ReadFiscalMemoryButton_Button_Click(object sender, EventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                actionManager.GetAction(eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS).Execute(new ActionFiscalPrinterReprintZReportsParams(UseDateFilter, Mode));
            }
            catch (Exception ex)
            {
                Kernel.LogFile.InfoException("ReadFiscalMemoryButton:OnButtonClick,Exception catched", ex);
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }

    }
}
