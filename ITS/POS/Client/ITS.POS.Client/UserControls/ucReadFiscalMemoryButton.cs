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
    /// For fiscal printer only. Sends commands to the printer to reprint Z reports
    /// </summary>
    public partial class ucReadFiscalMemoryButton : ucButton
    {
        /// <summary>
        /// If set to true, user is prompted for a date range, else a Z number range is used
        /// </summary>
        public bool UseDateFilter { get; set; }

        /// <summary>
        /// Determines the report mode
        /// </summary>
        public eReprintZReportsMode Mode { get; set; }

        public ucReadFiscalMemoryButton()
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
                Kernel.LogFile.Info(ex, "ReadFiscalMemoryButton:OnButtonClick,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }

    }
}
