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
    public class ActionDatabaseMaintenance : Action
    {
        internal class DatabaseMaintenanceRunner : IActionProgress
        {
            private ISessionManager sessionManager;
            public DatabaseMaintenanceRunner(ISessionManager sessionManager)
            {
                this.sessionManager = sessionManager;
            }

            public event ActionProgressStartedEventHandler ProgressStarted;

            public event ActionProgressChangedEventHandler ProgressChanged;

            public event ActionProgressCompletedEventHandler ProgressCompleted;

            public void DoWork()
            {
                if (ProgressStarted != null) { ProgressStarted(this, 1, 4, "Recreating Databases " + Environment.NewLine); }
                sessionManager.ClearAllSessions();
                if (ProgressChanged != null) { ProgressChanged(this, 1, "Clearing existing sessions" + Environment.NewLine); }
                sessionManager.GetSession<Item>().ExecuteNonQuery("VACUUM");
                if (ProgressChanged != null) { ProgressChanged(this, 2, "Master Database Completed" + Environment.NewLine); }
                sessionManager.GetSession<VatCategory>().ExecuteNonQuery("VACUUM");
                if (ProgressChanged != null) { ProgressChanged(this, 3, "Settings Database Completed" + Environment.NewLine); }
                sessionManager.GetSession<TableVersions>().ExecuteNonQuery("VACUUM");
                if (ProgressChanged != null) { ProgressChanged(this, 4, "Version Database Completed" + Environment.NewLine); }
                if (ProgressCompleted != null) { ProgressCompleted(this, "", null); ;}
            }
        }

        public ActionDatabaseMaintenance(IPosKernel kernel)
            : base(kernel)
        {

        }


        public override bool RequiresParameters
        {
            get { return false; }
        }


        public override eActions ActionCode
        {
            get { return eActions.DATABASE_MAINTENANCE; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION4;
            }
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
                DatabaseMaintenanceRunner runner = new DatabaseMaintenanceRunner(sessionManager);

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
                //sessionManager.ClearAllSessions();
                //sessionManager.GetSession<Item>().ExecuteNonQuery("VACUUM");
                //sessionManager.GetSession<VatCategory>().ExecuteNonQuery("VACUUM");
            }
        }
    }
}
