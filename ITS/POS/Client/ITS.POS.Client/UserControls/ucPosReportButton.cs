using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;

namespace ITS.POS.Client.UserControls
{
    public partial class ucPosReportButton : ucGroupButton
    {
        public ucPosReportButton()
        {
            this.InitializeComponent();
            this.ReportsButtonProperties = new ButtonProperties()
            {
                BackColor = this.BackColor,
                Font = this.Font,
                ForeColor = this.ForeColor
            };
        }


        public PosReport posReport { get; set; }

        private bool FormRemainsOpenAfterBtnClick;





        public ButtonProperties ReportsButtonProperties { get; set; }

        protected override void Button_Click(object sender, EventArgs e)
        {

            frmReportGroupButton form = (frmReportGroupButton)this.FindForm();

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {

                actionManager.GetAction(eActions.USE_OPOS_REPORT).Execute(new ActionUseOposReportParams(posReport.Code));

            }
            catch (Exception ex)
            {
                Kernel.LogFile.InfoException("USE_OPOS_REPORT:Button_Click,Exception catched", ex);
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }


            //form.DialogResult = System.Windows.Forms.DialogResult.OK;
            //form.SelectedObject = posReport;
            if (this.RemainOpenAfterClick == false)
            {
                form.Close();
            }

        }

    }




}
