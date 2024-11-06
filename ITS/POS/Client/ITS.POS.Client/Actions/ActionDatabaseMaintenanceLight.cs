using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Versions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    public class ActionDatabaseMaintenanceLight : Action
    {
        internal class DatabaseMaintenanceLightRunner : IActionProgress
        {
            private ISessionManager sessionManager;
            public DatabaseMaintenanceLightRunner(ISessionManager sessionManager)
            {
                this.sessionManager = sessionManager;
            }

            public event ActionProgressStartedEventHandler ProgressStarted;

            public event ActionProgressChangedEventHandler ProgressChanged;

            public event ActionProgressCompletedEventHandler ProgressCompleted;

            public void DoWork()
            {
                if (ProgressStarted != null) { ProgressStarted(this, 1, 3, "Creating Database Statistics" + Environment.NewLine); }
                sessionManager.GetSession<Item>().ExecuteNonQuery("ANALYZE");
                if (ProgressChanged != null) { ProgressChanged(this, 1, "Master Database Completed" + Environment.NewLine); }
                sessionManager.GetSession<VatCategory>().ExecuteNonQuery("ANALYZE");
                if (ProgressChanged != null) { ProgressChanged(this, 2, "Settings Database Completed" + Environment.NewLine); }
                sessionManager.GetSession<TableVersions>().ExecuteNonQuery("ANALYZE");
                if (ProgressChanged != null) { ProgressChanged(this, 3, "Version Database Completed" + Environment.NewLine); }
                if (ProgressCompleted != null) { ProgressCompleted(this, "", null); }
            }
        }

        public ActionDatabaseMaintenanceLight(IPosKernel kernel)
            : base(kernel)
        {

        }


        public override bool RequiresParameters
        {
            get { return false; }
        }


        public override eActions ActionCode
        {
            get { return eActions.DATABASE_MAINTENANCE_LIGHT; }
        }


        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.SALE; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            if (formManager.ShowMessageBox(POSClientResources.DO_NOT_PROCEED_IF_YOU_DONT_KNOW, MessageBoxButtons.OKCancel) == DialogResult.OK)//MessageBox.Show(POSClientResources.DO_YOU_WANT_TO_CANCEL_LINE, POSClientResources.CANCEL_LINE, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                DatabaseMaintenanceLightRunner runner = new DatabaseMaintenanceLightRunner(sessionManager);

                using (frmAsyncMessageBox frm = new frmAsyncMessageBox(Kernel, runner))
                {
                    Task task = new Task(() => { runner.DoWork(); });
                    frm.Shown += (sender, evt) =>
                    {
                        task.Start();
                    };
                    frm.btnOK.Visible = true;
                    frm.btnRetry.Visible = false;
                    frm.btnCancel.Visible = true;
                    frm.btnOK.Click += (sender, evt) => { frm.Close(); }; 
                    frm.ShowDialog();                    
                }
            }
        }
    }
}
