using System;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using System.Collections.Generic;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Helpers;
using System.Linq;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;
using System.Windows.Forms;
using ITS.POS.Client.Actions.Permission;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Actions;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// An abstract class that is the base for all actions.
    /// All actions are IObservable and all the UI components are listening to the apropriate actions.
    /// This allows the seperation of the UI from business logic, which is required since the UI is customizable from the end-users
    /// </summary>
    public abstract class Action : IAction
    {
        public IPosKernel Kernel { get; protected set; }

        protected Action(IPosKernel kernel)
        {
            Kernel = kernel;
            _Observers = new ThreadSafeList<IObserver>();
        }

        protected eKeyStatus _KeyStatusRequirement;

        /// <summary>
        /// Gets the minimum key status required for the action execution.
        /// </summary>
        public eKeyStatus KeyStatusRequirement
        {
            get
            {
                return _KeyStatusRequirement;
            }
        }

        /// <summary>
        /// Gets the fiscal methods that the action is valid for execution.
        /// Default: All fiscal methods are valid
        /// </summary>
        public virtual eFiscalMethod ValidFiscalMethods
        {
            get
            {
                return eFiscalMethod.ADHME | eFiscalMethod.EAFDSS | eFiscalMethod.UNKNOWN;
            }
        }

        /// <summary>
        /// Returns if the drawer must be closed for the action to be able to execute.
        /// </summary>
        public virtual bool NeedsDrawerClosed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a flaged eMachineStatus with all the statuses that are valid for the execution of this action.
        /// </summary>
        public abstract eMachineStatus ValidMachineStatuses { get; }

        /// <summary>
        /// Default: Position0
        /// </summary>
        protected virtual eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION0;
            }
        }

        public abstract eActions ActionCode
        {
            get;
        }

        protected bool CustomPermissionLoaded { get; set; }

        /// <summary>
        /// Validates the passed parameters, checks if action can be executed and then executes it.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dontCheckPermissions"></param>
        /// <param name="validateMachineStatus"></param>
        public void Execute(ActionParams parameters = null, bool dontCheckPermissions = false, bool validateMachineStatus = true)
        {
            if (CustomPermissionLoaded == false)
            {
                LoadCustomPermission();
                CustomPermissionLoaded = true;
            }

            Kernel.LogFile.Trace("Starting action " + this.ActionCode.ToString());
            this.CheckFiscalMethod();
            if (validateMachineStatus)
            {
                this.CheckMachineStatus();
            }
            this.ValidateParameters(parameters);
            if (!dontCheckPermissions)
            {
                this.CheckPermission();
            }
            ExecuteCore(parameters, dontCheckPermissions);
            Kernel.LogFile.Trace("End action " + this.ActionCode.ToString());
        }

        /// <summary>
        /// The execution core of the action.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dontCheckPermissions"></param>
        protected abstract void ExecuteCore(ActionParams parameters, bool dontCheckPermissions);

        private void CheckMachineStatus()
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (!this.ValidMachineStatuses.HasFlag(appContext.GetMachineStatus()))
            {
                throw new InvalidMachineStatusException(appContext.GetMachineStatus(), this.ValidMachineStatuses);
            }

            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            if (this.NeedsDrawerClosed && deviceManager.IsDrawerOpen)
            {
                throw new POSException(POSClientResources.PLEASE_CLOSE_DRAWER);
            }
        }

        private void CheckFiscalMethod()
        {
            IConfigurationManager configManager = Kernel.GetModule<IConfigurationManager>();
            if (!this.ValidFiscalMethods.HasFlag(configManager.FiscalMethod))
            {
                throw new InvalidFiscalMethodException(configManager.FiscalMethod, this.ValidFiscalMethods);
            }
        }

        private void CheckPermission()
        {
            bool mustShowForm = false;
            IConfigurationManager configManager = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (configManager.UsesKeyLock)
            {
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                if (this.KeyStatusRequirement != deviceManager.KeyStatus && this.KeyStatusRequirement != eKeyStatus.UNKNOWN)
                {
                    mustShowForm = true;
                }
            }
            else
            {
                if ((appContext.CurrentUser != null && (int)appContext.CurrentUser.POSUserLevel < (int)this.KeyStatusRequirement))
                {
                    mustShowForm = true;
                }
            }

            if (mustShowForm)
            {
                using (frmElevatePrivileges form = new frmElevatePrivileges(this.KeyStatusRequirement,Kernel))
                {
                    DialogResult result = form.ShowDialog();
                    appContext.MainForm.Invoke(new MethodInvoker(delegate { appContext.MainForm.Focus(); }));
                    if (result == DialogResult.Cancel)
                    {
                        throw new POSException(POSClientResources.ACTION_CANCELED);
                    }
                }
            }
        }

        private void ValidateParameters(ActionParams parameters)
        {
            if (this.RequiresParameters)
            {
                if (parameters == null || parameters.ActionCode != this.ActionCode)
                {
                    throw new InvalidActionParametersException(this.ActionCode, parameters);
                }
            }
        }

        protected void LoadCustomPermission()
        {
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            IEnumerable<ActionPermission> effective = actionManager.ActionPermissions.Where(g => g.ActionCode == this.ActionCode);
            if (effective.Count() > 0)
            {
                this._KeyStatusRequirement = effective.Max(g => g.KeyStatus);
            }
            else
            {
                this._KeyStatusRequirement = this.DefaultKeyStatusRequirement;
            }
        }

        protected ThreadSafeList<IObserver> _Observers;
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

        public abstract bool RequiresParameters
        {
            get;
        }

        public bool IsInternal
        {
            get
            {
                return (int)this.ActionCode > 100000;
            }
        }

        public void AttachObservers(List<IObserver> Observers)
        {
            foreach (IObserver item in Observers)
            {
                Attach(item);
            }
        }

        public void Attach(IObserver os)
        {
            if (_Observers == null)
                _Observers = new ThreadSafeList<IObserver>();
            _Observers.Add(os);
        }
        public void Dettach(IObserver os)
        {
            if (_Observers != null)
                _Observers.Remove(os);
        }

        public virtual void Notify(ObserverParams parameters)
        {
            ObserverHelper.NotifyObservers(this._Observers, parameters, Kernel.LogFile);
        }



    }
}
