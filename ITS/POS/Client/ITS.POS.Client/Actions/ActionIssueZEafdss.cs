using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware.Common;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Handles the Issue of Z in EAFDSS device
    /// </summary>
    public class ActionIssueZEafdss : Action
    {
        public ActionIssueZEafdss(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.ISSUE_Z_EAFDSS; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();


            List<Device> fiscalDevices = deviceManager.GetEAFDSSDevicesByPriority(config.FiscalDevice); //
            if (fiscalDevices == null || fiscalDevices.Count == 0)
            {
                return;
            }
            string uploadZErrorMessage = string.Empty;
            List<string> exceptionsThrown = new List<string>();
            Dictionary<Device, DeviceIssueZStatus> dictionaryDevice = new Dictionary<Device, DeviceIssueZStatus>();
            foreach (Device fiscalDevice in fiscalDevices)
            {
                DeviceIssueZStatus deviceIssueZStatus = new DeviceIssueZStatus();
                Kernel.LogFile.Trace("Trying to sign using " + fiscalDevice.DeviceName);
                try
                {

                    IssueEAFDSSSZ(config.ABCDirectory, fiscalDevice, ref uploadZErrorMessage);
                    deviceIssueZStatus.IsIssueZ = true;
                }
                catch (Exception ex)
                {
                    fiscalDevice.FailureCount++;
                    string message = POSClientResources.FAILURE_ISSUING_EAFDSS_Z + Environment.NewLine + fiscalDevice.DeviceName + Environment.NewLine + ex.GetFullMessage();
                    Kernel.LogFile.Error(ex, message);
                    exceptionsThrown.Add(fiscalDevice.DeviceName + " : " + ex.GetFullMessage());
                    deviceIssueZStatus.IsIssueZ = false;
                    deviceIssueZStatus.ExceptionMessage = ex.GetFullMessage();
                }
                dictionaryDevice.Add(fiscalDevice, deviceIssueZStatus);
            }
            if (dictionaryDevice.Where(x => x.Value.IsIssueZ == true).Count() > 0 && dictionaryDevice.Where(x => x.Value.IsIssueZ == false).Count() <= 0)
            {
                if (!string.IsNullOrEmpty(uploadZErrorMessage) && uploadZErrorMessage != "SUCCESS")
                {
                    throw new POSUploadZException(uploadZErrorMessage);
                }
                return;
            }
            else if (dictionaryDevice.Where(x => x.Value.IsIssueZ == true).Count() > 0 && dictionaryDevice.Where(x => x.Value.IsIssueZ == false).Count() > 0)
            {
                string fiscalDevicesErrors = string.Empty;
                foreach (KeyValuePair<Device, DeviceIssueZStatus> keyPair in dictionaryDevice.Where(x => x.Value.IsIssueZ == false).ToList())
                {
                    fiscalDevicesErrors += String.Format("Z Issue printing succesfull but not for device {0} error {1} \n", keyPair.Key.DeviceName, keyPair.Value.ExceptionMessage);
                }
                throw new POSException(string.Format("Issue Z printing succesfull but not sign the receipt using after trying with all fiscal devices. {0}", fiscalDevicesErrors));
            }
            else
            {
                string fiscalDevicesErrors = string.Empty;
                foreach (KeyValuePair<Device, DeviceIssueZStatus> keyPair in dictionaryDevice.ToList())
                {
                    fiscalDevicesErrors += String.Format("Cannot sign the receipt using after trying with {0} error {1} \n", keyPair.Key.DeviceName, keyPair.Value.ExceptionMessage);
                }
                throw new Exception(string.Format("Cannot issue z after trying with all fiscal devices. {0}", fiscalDevicesErrors));
            }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        private static void IssueEAFDSSSZ(string ABCDirectory, Device fiscalDevice, ref string uploadZErrorMessage)
        {
            string exResult = null;
            if (fiscalDevice is DiSign)
            {
                uploadZErrorMessage = string.Empty;
                int result = (fiscalDevice as DiSign).IssueZ(ref exResult, ref uploadZErrorMessage);
                if (result == -2)
                {
                    return;
                }
                if (result != 0)
                {
                    throw new POSException("Error Code " + result + "\nResult " + exResult + "\n " + uploadZErrorMessage);
                }
            }
            else if (fiscalDevice is AlgoboxNetESD)
            {
                AlgoboxNetESD.AlgoboxNetResult result = (fiscalDevice as AlgoboxNetESD).IssueZreport(ref exResult);
                if (result != AlgoboxNetESD.AlgoboxNetResult.SUCCESS)
                {
                    throw new POSException("Error Code " + result + "\nResult " + exResult);
                }
            }
            else if (fiscalDevice is DataSignESD)
            {
                DataSignESD.DataSignResult result = (fiscalDevice as DataSignESD).IssueZreport(ABCDirectory);
                if (result != DataSignESD.DataSignResult.ERR_SUCCESS)
                {
                    throw new POSException("Error Code " + (int)result + "\nResult " + result);
                }
            }
        }
    }
}
