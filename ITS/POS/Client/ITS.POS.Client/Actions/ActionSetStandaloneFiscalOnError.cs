using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Sets or restores the current ADHME POS on error. The pos will print "Fiscal On Error" receipts
    /// </summary>
    class ActionSetStandaloneFiscalOnError : Action
    {
        public ActionSetStandaloneFiscalOnError(IPosKernel kernel): base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED |
                    eMachineStatus.CLOSED | eMachineStatus.UNKNOWN;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.SET_STANDALONE_FISCAL_ON_ERROR; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        public override eFiscalMethod ValidFiscalMethods
        {
            get
            {
                return eFiscalMethod.EAFDSS;
            }
        }


        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IConfigurationManager configrationManager = Kernel.GetModule<IConfigurationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            ActionSetStandaloneFiscalOnErrorParams castedParams = parameters as ActionSetStandaloneFiscalOnErrorParams;
            Model.Settings.POS currentTerminal = sessionManager.GetSession<Model.Settings.POS>().GetObjectByKey<Model.Settings.POS>(configrationManager.CurrentTerminalOid);

            if (currentTerminal == null)
            {
                throw new POSException(Resources.POSClientResources.INVALID_TERMINAL_OID);
            }

            string message = castedParams.SetOnError ? POSClientResources.SET_FISCAL_ON_ERROR : POSClientResources.REMOVE_FISCAL_ON_ERROR;
            DialogResult dialogResult = formManager.ShowMessageBox(message, System.Windows.Forms.MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {

                currentTerminal.IsStandaloneFiscalOnError = castedParams.SetOnError;
                currentTerminal.Save();
                currentTerminal.Session.CommitTransaction();
            }
        }
    }
}
