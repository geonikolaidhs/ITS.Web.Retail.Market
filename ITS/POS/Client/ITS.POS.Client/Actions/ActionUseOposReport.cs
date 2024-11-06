using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    public class ActionUseOposReport : Action
    {
        public ActionUseOposReport(IPosKernel kernel) : base(kernel)
        {

        }


        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            IFormManager formManager = this.Kernel.GetModule<IFormManager>();


            ActionUseOposReportParams castedParams = parameters as ActionUseOposReportParams;

            PosReport posReport = null;
            PosOposReportSettings reportSettings = null;
            if (string.IsNullOrWhiteSpace(castedParams.PosReportCode))
            {
                XPCollection<PosOposReportSettings> posReportsettings = new XPCollection<PosOposReportSettings>(sessionManager.GetSession<PosOposReportSettings>());

                CriteriaOperator criteria = CriteriaOperator.And(new InOperator("Oid", posReportsettings.Select(x => x.PrintFormat)));
                XPCollection<PosReport> posReports = new XPCollection<PosReport>(sessionManager.GetSession<PosReport>(), criteria);

                using (frmSelectLookup frm = new frmSelectLookup(POSClientResources.USE_OPOS_REPORT, posReports, "Description", "This", new List<string>() { "Code", "Description" }, posReports.FirstOrDefault(), this.Kernel))
                {
                    DialogResult result = frm.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }
                    posReport = frm.SelectedObject as PosReport;
                    reportSettings = sessionManager.FindObject<PosOposReportSettings>(new BinaryOperator("PrintFormat", posReport.Oid));
                }

            }
            else
            {
                posReport = sessionManager.FindObject<PosReport>(new BinaryOperator("Code", castedParams.PosReportCode));
                reportSettings = sessionManager.FindObject<PosOposReportSettings>(new BinaryOperator("PrintFormat", posReport.Oid));
            }

            if (posReport == null || reportSettings == null)
            {
                throw new POSUserVisibleException(POSClientResources.USE_OPOS_REPORT + "(" + castedParams.PosReportCode + "): " + POSClientResources.NOT_FOUND);
            }
            else
            {
                actionManager.GetAction(eActions.PRINT_REPORT_TO_OPOS_PRINTER).Execute(new ActionPrintReportToOposPrinterInernalParams(posReport, reportSettings));
            }

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT | eMachineStatus.UNKNOWN; }
        }


        public override eActions ActionCode
        {
            get { return eActions.USE_OPOS_REPORT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }



    }
}
