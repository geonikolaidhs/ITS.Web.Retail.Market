using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;
using System.Linq;
using DevExpress.XtraEditors;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using System.Reflection;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Kernel;

namespace ITS.POS.Client.Forms
{
    public partial class frmMainBase : XtraForm, IObservable, IObserverContainer, IPoleDisplayInputContainer, IPOSForm
    {
        private bool _subscriptionsAreInitialized;
        public bool SubscriptionsAreInitialized
        {
            get
            {
                return _subscriptionsAreInitialized;
            }
        }

        public frmMainBase()
        {
            InitializeComponent();
            Attach(ucInput);

            FocusedPoleDisplayInputChanged += OnFocusedPoleDisplayInputChanged;
        }

        public string InputText
        {
            get
            {
                return this.ucInput.Text;
            }
        }

        public void AppendInputText(string text)
        {
            this.ucInput.Text += text;
            this.ucInput.MoveCursorToEnd();
        }

        public decimal SelectedQty
        {
            get
            {
                return this.ucInput.SelectedQty;
            }
            set
            {
                this.ucInput.SelectedQty = value;
            }
        }

        public void ResetInputText()
        {
            ucInput.Text = "";
        }

        public void SetInputBackground(Color color)
        {

            this.ucInput.edtInput.Properties.Appearance.BackColor = color;
        }

        private ThreadSafeList<IObserver> _Observers;
        public ThreadSafeList<IObserver> Observers
        {
            get
            {
                return _Observers;
            }
            set
            {
                _Observers = value;
            }
        }

        public void Attach(IObserver os)
        {
            if (Observers == null)
                Observers = new ThreadSafeList<IObserver>();
            Observers.Add(os);
        }

        public void Dettach(IObserver os)
        {
            if (Observers != null)
                Observers.Remove(os);
        }


        public void frmMainBase_KeyDown(object sender, KeyEventArgs e)
        {
            ISynchronizationManager sync = Kernel.GetModule<ISynchronizationManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            try
            {
                if (sync.UpdateKeyMappings)
                {
                    sync.UpdateKeyMappings = false;
                    UtilsHelper.InitKeyMaps(config.CurrentTerminalOid, sessionManager);
                }
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                IEnumerable<KeyLock> indirectKeylocks = deviceManager.Devices.Where(x => typeof(KeyLock).IsAssignableFrom(x.GetType()) && x.ConType == ConnectionType.INDIRECT).Cast<KeyLock>();

                bool keyHandled = false;
                foreach (KeyLock keylock in indirectKeylocks)
                {
                    keyHandled = keylock.IndirectConnectionHandleKey(e);
                    if (keyHandled)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }

                if (e.Handled)
                {
                    return;
                }

                if (UtilsHelper.hKeyMap.ContainsKey(e.KeyData.ToString()))
                {
                    POSKeyMapping mapping = UtilsHelper.hKeyMap[e.KeyData.ToString()];
                    Notify(new KeyListenerParams(mapping.ActionCode, mapping.NotificationType, mapping.RedirectTo, mapping.KeyData, mapping.ActionParameters));
                }
                else
                {
                    Notify(new KeyListenerParams(eActions.NONE, eNotificationsTypes.KEY, Keys.None, e.KeyData, ""));
                }
            }
            catch (InvalidMachineStatusException)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
            }
            catch (POSUserVisibleException puvxception)
            {
                Kernel.LogFile.Debug(puvxception,"frmMainBase:KeyDown,POSUserVisibleException catched");                
                formManager.ShowCancelOnlyMessageBox(puvxception.Message);
            }
            catch (DocumentNegativeTotalException docNegTotalException)
            {
                Kernel.LogFile.Debug(docNegTotalException, "frmMainBase:KeyDown,DocumentNegativeTotalException catched");
                formManager.ShowCancelOnlyMessageBox(POSClientResources.NEGATIVE_TOTAL_NOT_PERMITED);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is DocumentNegativeTotalException)
                {
                    Kernel.LogFile.Debug(ex, "frmMainBase:KeyDown,DocumentNegativeTotalException catched");
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.NEGATIVE_TOTAL_NOT_PERMITED);
                }
                else if (ex.InnerException is InvalidMachineStatusException)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
                }
                else if (ex.InnerException is POSUserVisibleException)
                {
                    Kernel.LogFile.Debug(ex.InnerException, "frmMainBase:KeyDown,POSUserVisibleException catched");
                    formManager.ShowCancelOnlyMessageBox(ex.InnerException.Message);
                }
                else
                {
                    Kernel.LogFile.Info(ex, "frmMainBase:KeyDown,Exception catched");
                    string exmessage = ex.GetFullMessage();
                    if (ex is TargetInvocationException && ex.InnerException != null)
                    {
                        exmessage = ex.InnerException.GetFullMessage();
                    }
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(exmessage));
                }
            }
        }

        public void Notify(ObserverParams parameters)
        {
            ObserverHelper.NotifyObservers(this._Observers, parameters, Kernel.LogFile);
        }

        private void frmMainBase_Shown(object sender, EventArgs e)
        {
            this.ucInput.Focus();
        }

        private void frmMainBase_Activated(object sender, EventArgs e)
        {
            this.ucInput.Focus();
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            AppContext.CurrentFocusedScannerInput = this.ucInput;
        }

        public void IntitializeSubscriptions()
        {
            if (!SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (typeof(IObserver).IsAssignableFrom(control.GetType()))
                    {
                        ((IObserver)control).InitializeActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = true;
            }
        }

        public void DropSubscriptions()
        {
            if (SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (control is ucObserver)
                    {
                        (control as ucObserver).DropActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = false;
            }
        }

        public void Update(MessengerParams msgParams)
        {
            this.ucInput.Update(msgParams);
        }

        public void Update(KeyListenerParams keyParams)
        {
            this.ucInput.Update(keyParams);
        }


        public ucPOSInput GetMainInput()
        {
            return this.ucInput;
        }

        public bool waitSpecialCommand
        {
            get
            {
                return ucInput.waitSpecialCommand;
            }
            set
            {
                ucInput.waitSpecialCommand = value;
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public bool ShowTitle
        {
            get { return false; }
        }

        public bool ShowInputName
        {
            get { return false; }
        }

        public string Title
        {
            get { return ""; }
        }


        public event FocusedPoleDisplayInputChangedEventHandler FocusedPoleDisplayInputChanged;
        //public event FocusedScannerInputChangedEventHandler FocusedScannerInputChanged;

        IPoleDisplayInput previousPoleDisplayControl = null;
        /// <summary>
        /// This method is called whenever the ActiveControl property changes
        /// </summary>
        protected override void UpdateDefaultButton()
        {
            base.UpdateDefaultButton();
            if (this.ActiveControl != null)
            {
                IPoleDisplayInput actualPoleDisplayControl = this.ActiveControl is IPoleDisplayInput ? this.ActiveControl as IPoleDisplayInput : this.ActiveControl.Parent as IPoleDisplayInput;
                if (actualPoleDisplayControl != null && actualPoleDisplayControl != previousPoleDisplayControl)
                {
                    FocusedPoleDisplayInputChanged(this, new FocusedPoleDisplayInputChangedArgs(previousPoleDisplayControl, actualPoleDisplayControl));
                    previousPoleDisplayControl = actualPoleDisplayControl;
                }
            }
        }

        public void OnFocusedPoleDisplayInputChanged(object sender, FocusedPoleDisplayInputChangedArgs args)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            deviceManager.HandleFocusedPoleDisplayInputChanged(this as IPoleDisplayInputContainer, args.PreviousControlWithFocus, args.CurrentControlWithFocus, deviceManager.GetCashierPoleDisplay());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Kernel != null)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> scannerInputs = formManager.GetAllChildControls(this, typeof(IScannerInput));
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IControlLocalizer controlLocalizer = Kernel.GetModule<IControlLocalizer>();
                controlLocalizer.LocalizeControl(this);
                foreach (Control scannerInput in scannerInputs)
                {
                    scannerInput.Enter += appContext.ScannerInputOnEnterHandler(appContext);
                    scannerInput.Leave += appContext.ScannerInputOnLeaveHandler(appContext);
                }
            }
        }


        public IPosKernel Kernel { get; set; }

        public void Initialize(IPosKernel kernel)
        {
            this.Kernel = kernel;
        }
    }
}