using DevExpress.XtraEditors;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ITS.POS.Client.Kernel;
namespace ITS.POS.Client.Forms
{
    public partial class frmDialogBase : XtraForm, IObserverKeyListener, IPOSForm
    {
        protected frmDialogBase()
        {
            InitializeComponent();
            frmDialogBaseKeyDownHandler = new KeyEventHandler(this.frmDialogBase_KeyDown);
            this.KeyDown += frmDialogBaseKeyDownHandler;
        }
        public KeyEventHandler frmDialogBaseKeyDownHandler { get; set; }
        public IPosKernel Kernel { get; set; }

        public void Initialize(IPosKernel kernel)
        {
            Kernel = kernel;
        }

        public frmDialogBase(IPosKernel kernel)
        {
            Initialize(kernel);
            InitializeComponent();
            frmDialogBaseKeyDownHandler = new KeyEventHandler(this.frmDialogBase_KeyDown);
            this.KeyDown += frmDialogBaseKeyDownHandler;
            ValidActions = new List<eActions>();
            ValidActions.Add(eActions.CANCEL);
            ValidActions.Add(eActions.BACKSPACE);
            ValidActions.Add(eActions.OPEN_DRAWER);

            this.CanBeClosedByUser = true;

        }

        protected virtual bool UsesCustomBackgroundImage
        {
            get
            {
                return false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Kernel != null)
            {
                IControlLocalizer controlLocalizer = Kernel.GetModule<IControlLocalizer>();
                controlLocalizer.LocalizeControl(this);
                if (UsesCustomBackgroundImage == false)
                {
                    IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                    IFormManager formManager = Kernel.GetModule<IFormManager>();
                    formManager.HandleBackGroundImage(this, config.EnableLowEndMode);
                }
            }
        }

        public bool CanBeClosedByUser { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected List<eActions> ValidActions { get; set; }

        Type[] paramsTypes = new Type[] { typeof(KeyListenerParams) };

        public virtual Type[] GetParamsTypes()
        {
            return paramsTypes;
        }


        public virtual void Update(KeyListenerParams parameters)
        {
            if (parameters.NotificationType == eNotificationsTypes.ACTION && ValidActions.Contains(parameters.ActionCode))
            {
                IActionExecutor executor = Kernel.GetModule<IActionExecutor>();
                executor.ExecuteAction(parameters.ActionCode, parameters.ActionParameters);
                return;
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY && parameters.RedirectTo != Keys.None)
            {
                string keyText = UtilsHelper.KeyboardDecode(parameters.RedirectTo);
                this.Invoke((MethodInvoker)delegate()
                {
                    SendKeys.Send(keyText);
                });
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY)
            {
                MethodInfo callMethod = this.GetType().GetMethod(parameters.KeyData.ToString() + "Key");
                if (callMethod != null)
                {
                    callMethod.Invoke(this, null);
                }
                return;
            }
        }

        public virtual void EscapeKey()
        {
            if (CanBeClosedByUser)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                frmDialogBase_KeyDown(this, new KeyEventArgs(keyData));
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void frmDialogBase_KeyDown(object sender, KeyEventArgs e)
        {
            ISynchronizationManager sync = Kernel.GetModule<ISynchronizationManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (sync.UpdateKeyMappings)
                {
                    sync.UpdateKeyMappings = false;
                    UtilsHelper.InitKeyMaps(config.CurrentTerminalOid, sessionManager);
                }

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
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    POSKeyMapping mapping = UtilsHelper.hKeyMap[e.KeyData.ToString()];
                    Update(new KeyListenerParams(mapping.ActionCode, mapping.NotificationType, mapping.RedirectTo, mapping.KeyData, mapping.ActionParameters));
                }
                else
                {
                    Update(new KeyListenerParams(eActions.NONE, eNotificationsTypes.KEY, Keys.None, e.KeyData, ""));
                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmMainBase:KeyDown,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }

        private void frmDialogBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.HIDE_TOUCH_PAD).Execute();
        }

        public void InitializeActionSubscriptions()
        {

        }

        public void DropActionSubscriptions()
        {

        }
    }
}
