using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Xpo;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using System.Linq;
using DevExpress.Xpo.DB;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern;
using System.Reflection;

namespace ITS.POS.Client.Forms
{
    public partial class frmSplashScreen : frmDialogBase, IObserverContainer, IObserverStatusDisplayer
    {
        public frmSplashScreen(IPosKernel kernel)
            : base(kernel)
        {
            InitializeComponent();
            this.ActionsToObserve = new List<eActions>();
            this.ActionsToObserve.Add(eActions.PUBLISH_MACHINE_STATUS);

            ValidActions.Add(eActions.START_FISCAL_DAY);
            ValidActions.Add(eActions.START_SHIFT);
            ValidActions.Add(eActions.RESTART);
            ValidActions.Add(eActions.SHUTDOWN);
            ValidActions.Add(eActions.CALL_OTHER_ACTION);
            ValidActions.Add(eActions.APPLICATION_EXIT);
            ValidActions.Add(eActions.ISSUE_Z);
            this.CanBeClosedByUser = false;

        }

        private bool showSpecialCommandBar;
        public bool ShowSpecialCommandBar
        {
            get
            {
                return showSpecialCommandBar;
            }
            set
            {
                showSpecialCommandBar = value;
                cmdTextBox.Visible = showSpecialCommandBar;
            }
        }

        public override void EscapeKey()
        {
            base.EscapeKey();
            if (ShowSpecialCommandBar)
            {
                ShowSpecialCommandBar = false;
            }
        }

        public void BackKey()
        {
            if (cmdTextBox.Visible)
            {
                cmdTextBox.Text = cmdTextBox.Text.Length != 0 ? cmdTextBox.Text.Remove(cmdTextBox.Text.Length - 1, 1) : "";
            }
        }

        public override void Update(KeyListenerParams parameters)
        {
            base.Update(parameters);
            if (parameters.NotificationType == eNotificationsTypes.KEY)
            {
                MethodInfo callMethod = this.GetType().GetMethod(parameters.KeyData.ToString() + "Key");
                if (callMethod == null)
                {
                    cmdTextBox.Text += UtilsHelper.KeyboardDecode(parameters.KeyData);
                }
            }
        }

        public void ReturnKey()
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (ShowSpecialCommandBar)
                {
                    ShowSpecialCommandBar = false;
                    actionManager.GetAction(eActions.CALL_OTHER_ACTION).Execute(new ActionCallOtherActionParams(cmdTextBox.Text));
                }
                cmdTextBox.Text = "";
            }
            catch (Exception e)
            {
                Kernel.LogFile.Trace(e, "frmSplashScreen:ReturnKey,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
                cmdTextBox.Text = "";
            }
        }

        public bool ShouldSerializeActionsToObserve()
        {
            return false;
        }

        [Browsable(false)]
        public List<eActions> ActionsToObserve
        {
            get;
            set;
        }



        private bool _subscriptionsAreInitialized;
        public bool SubscriptionsAreInitialized
        {
            get
            {
                return _subscriptionsAreInitialized;
            }
        }

        public void IntitializeSubscriptions()
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (!SubscriptionsAreInitialized)
            {
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (typeof(IObserver).IsAssignableFrom(control.GetType()))
                    {
                        ((IObserver)control).InitializeActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = true;

                foreach (eActions act in ActionsToObserve)
                {
                    actionManager.GetAction(act).Attach(this);
                }
            }
        }

        public void DropSubscriptions()
        {
            if (SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (control is ucObserver)
                    {
                        (control as ucObserver).DropActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = false;

                foreach (eActions act in ActionsToObserve)
                {
                    actionManager.GetAction(act).Dettach(this);
                }
            }
        }

        protected void SetUIButtons(eMachineStatus status)
        {
            if (this.Visible == false)
            {
                return;
            }
            this.Invoke((MethodInvoker)delegate
            {
                switch (status)
                {
                    case eMachineStatus.CLOSED:
                        ucStartDay.Enabled = true;
                        ucStartShift.Enabled = false;
                        ucCloseDay.Enabled = !ucStartDay.Enabled;
                        break;
                    case eMachineStatus.DAYSTARTED:
                        ucStartDay.Enabled = false;
                        ucStartShift.Enabled = true;
                        ucCloseDay.Enabled = !ucStartDay.Enabled;
                        break;
                }
            });

        }

        protected override bool UsesCustomBackgroundImage
        {
            get
            {
                return true;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Visible = false;
            MessageDisplay.Text = "";
            MessageDisplay.ForeColor = Color.Black;

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            SetUIButtons(appContext.GetMachineStatus());

            DailyTotals latestZ = new XPCollection<DailyTotals>(sessionManager.GetSession<DailyTotals>(), null, new SortProperty("FiscalDate", SortingDirection.Descending)).FirstOrDefault();

            if (latestZ == null || latestZ.FiscalDateOpen == false)
            {
                MessageDisplay.Text = POSClientResources.YOU_SHOULD_OPEN_FISCAL_DAY;//"Πρέπει να κάνετε \"Έναρξη Ημέρας\"";
                MessageDisplay.ForeColor = Color.Red;
                ucStartShift.Enabled = false;
                ucCloseDay.Enabled = false;
                Visible = true;
                appContext.SetMachineStatus(eMachineStatus.CLOSED);
            }
            else
            {
                DateTime fiscalDate;
                try
                {
                    appContext.CurrentDailyTotals = totalizersService.CreateDailyTotals(config.CurrentTerminalOid, config.CurrentStoreOid, latestZ == null ? Guid.Empty : latestZ.Oid, out fiscalDate);
                    appContext.FiscalDate = fiscalDate;
                    appContext.CurrentDailyTotals.UserDailyTotalss.Sorting = new SortingCollection(new SortProperty("CreatedOn", SortingDirection.Ascending));
                    appContext.CurrentUserDailyTotals = appContext.CurrentDailyTotals.UserDailyTotalss.Where(g => g.IsOpen == true).FirstOrDefault();
                    if (appContext.CurrentUserDailyTotals == null)
                    {
                        appContext.SetMachineStatus(eMachineStatus.DAYSTARTED);
                        MessageDisplay.Text = POSClientResources.YOU_SHOULD_START_SHIFT;//"Πρέπει να κάνετε \"Έναρξη Βάρδιας\"";
                        MessageDisplay.ForeColor = Color.Red;
                        ucStartDay.Enabled = false;
                        ucCloseDay.Enabled = true;
                        Visible = true;
                        //FormHelper.ShowForm<frmLogin>(this, true);
                    }
                    else
                    {
                        appContext.CurrentUser = sessionManager.GetObjectByKey<User>(appContext.CurrentUserDailyTotals.User);
                        if (appContext.CurrentUser != null)
                        {
                            appContext.SetMachineStatus(eMachineStatus.SALE);
                            formManager.ShowForm<frmMainBase>(this, true);
                            //Globals.MainForm.Show();
                        }
                        else
                        {
                            ucStartDay.Enabled = false;
                            ucCloseDay.Enabled = true;
                            Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Kernel.LogFile.Info(ex, "frmSplashScreen:OnShown,Exception catched");
                    appContext.SetMachineStatus(eMachineStatus.DAYSTARTED);
                    MessageDisplay.Text = ex.GetFullMessage();//POSClientResources.YOU_SHOULD_ISSUE_Z;// "Πρέπει να κάνετε \"Κλείσιμο Ημέρας\"";
                    MessageDisplay.ForeColor = Color.Red;
                    ucStartDay.Enabled = false;
                    ucStartShift.Enabled = false;
                    ucCloseDay.Enabled = true;
                    Visible = true;
                }
            }

        }

        private void ucStartDay_Click(object sender, EventArgs e)
        {
            try
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

                actionManager.GetAction(eActions.START_FISCAL_DAY).Execute();
                if (appContext.CurrentDailyTotals != null)
                {
                    this.ucStartDay.Enabled = false;
                    ucStartShift.Enabled = true;
                    ucCloseDay.Enabled = true;
                    MessageDisplay.Text = POSClientResources.YOU_SHOULD_START_SHIFT;
                    ucStartShift_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmSplashScreen:StartDayButtonClick,Exception catched");
                MessageDisplay.Text = POSClientResources.ERROR + ": " + ex.GetFullMessage();
                MessageDisplay.ForeColor = Color.Red;
            }
        }

        private void ucStartShift_Click(object sender, EventArgs e)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                actionManager.GetAction(eActions.START_SHIFT).Execute();
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmSplashScreen:StartShiftbutton_Click,Exception catched");
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
            }
        }

        private void ucCloseDay_Click(object sender, EventArgs e)
        {
            try
            {
                CloseDay();
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmSplashScreen:CloseDaybutton_Click,Exception catched");
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
            }
        }

        private void CloseDay()
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.ISSUE_Z).Execute(new ActionIssueZReportParams(false, false));
            ucStartDay.Enabled = true;
            ucStartShift.Enabled = false;
            ucCloseDay.Enabled = false;
            MessageDisplay.Text = POSClientResources.SUCCESSFUL_ISSUE_Z;//"Επιτυχές κλείσιμο ημέρας!";
            MessageDisplay.ForeColor = Color.YellowGreen;

        }

        private void usitsKeyMappingButton1_Load(object sender, EventArgs e)
        {

        }

        public void Update(StatusDisplayerParams parameters)
        {
            SetUIButtons(parameters.MachineStatus);
        }


        Type[] paramsTypes = new Type[] { typeof(StatusDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }
    }
}