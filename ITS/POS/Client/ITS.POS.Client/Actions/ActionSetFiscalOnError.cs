using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Sets or restores the remote ADHME (DiSign) on error. All the POSes that listen to that ADHME will print "Fiscal On Error" receipts
    /// </summary>
    public class ActionSetFiscalOnError : Action
    {
        public ActionSetFiscalOnError(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | 
                eMachineStatus.CLOSED | eMachineStatus.UNKNOWN; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SET_FISCAL_ON_ERROR; }
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
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();

            ActionSetFiscalOnErrorParams castedParams = parameters as ActionSetFiscalOnErrorParams;

            DiSign found = deviceManager.Devices.Where(x => x.GetType() == typeof(DiSign)).FirstOrDefault() as DiSign;
            DiSign defaultITSFiscal = deviceManager.GetPrimaryDevice<DiSign>() ?? found;
            if (defaultITSFiscal == null)
            {
                throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
            }

            string message = castedParams.SetOnError ? POSClientResources.SET_FISCAL_ON_ERROR : POSClientResources.REMOVE_FISCAL_ON_ERROR;
            DialogResult dialogResult = formManager.ShowMessageBox(message, System.Windows.Forms.MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                var result = defaultITSFiscal.SetFiscalOnError(castedParams.SetOnError, out message);
                if (result == false)
                {
                    string finalMessage = "Setting Fiscal On Error = \"" + castedParams.SetOnError + "\" failed";
                    if (!String.IsNullOrWhiteSpace(message))
                    {
                        finalMessage += ".Error: " + message;
                    }

                    throw new Exception(finalMessage);
                }
            }
        }

    }
}
