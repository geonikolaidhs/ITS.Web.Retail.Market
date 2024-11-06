using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;

namespace ITS.POS.Client.UserControls
{
    public partial class ucUseReportTypeButton : ucButton
    {
        public string ReportTypeCode { get; set; }

        public ucUseReportTypeButton()
        {
            InitializeComponent();
        }


        private void ucUseReportType_Button_Click(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                eMachineStatus status = appContext.GetMachineStatus();

               
                actionManager.GetAction(eActions.USE_OPOS_REPORT).
                    Execute(new ActionUseOposReportParams(ReportTypeCode));
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "ucUseReportType:Button_Click,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }
    }
}
