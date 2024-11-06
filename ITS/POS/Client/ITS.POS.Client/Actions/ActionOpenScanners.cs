using System;
using System.Collections.Generic;
using System.Linq;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Used at application startup. Initializes the scanner devices and attaches the apropriate listening events. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionOpenScanners : Action
    {
        public ActionOpenScanners(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.OPEN_SCANNERS; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        /// <summary>
        /// Searches in Globals.SystemDevices and opens any scanners found attaching them to the Read Event.
        /// When the OnRead event is triggered, the observers of this action are notified with the read data.
        /// </summary>
        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            foreach (Device device in deviceManager.Devices)
            {
                if (device.GetType() == typeof(Scanner))
                {
                    (device as Scanner).ReadEvent += device_ReadEvent;
                    switch ((device as Scanner).StartListening())
                    {
                        case DeviceResult.FAILURE:
                            throw new Exception(POSClientResources.FAIL_OPEN_SCANNER);
                        case DeviceResult.DEVICENOTREADY:
                            throw new Exception(POSClientResources.FAIL_OPEN_SCANNER+": "+POSClientResources.DEVICE_NOT_READY);
                        case DeviceResult.UNAUTHORIZEDACCESS:
                            throw new Exception(POSClientResources.FAIL_OPEN_SCANNER+": "+POSClientResources.SYSTEM_IO_UNAUTHORIZED_EXCEPTION);
                        case DeviceResult.INVALIDPROPERTY:
                            throw new Exception(POSClientResources.FAIL_OPEN_SCANNER+": "+POSClientResources.INVALID_PROPERTY_DETECTED);
                        default :
                            break;
                    }
                    
                }

            }

        }

        void device_ReadEvent(Scanner sender, ScannerReadEventArgs e)
        {
            if (sender.Paused)
            {
                sender.DiscardSerialPortData();
                return;
            }

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            string data = e.Data;
            if(String.IsNullOrEmpty(sender.Settings.NewLine) == false)
            {
                if (data != null && data.EndsWith(sender.Settings.NewLine) == false)
                {
                    data = data + Environment.NewLine;
                }
            }
            
            if (appContext.CurrentFocusedScannerInput != null)
            {
                try
                {
                    string finalData = data.TrimEnd('\r', '\n');
                    appContext.MainForm.GetMainInput().FromScanner = true;
                    appContext.CurrentFocusedScannerInput.SetText(finalData);
                    appContext.CurrentFocusedScannerInput.SendEnter();
                }
                catch(Exception ex)
                {
                    Kernel.LogFile.Error(ex, "Error trying to handle scanner read data");
                }
            }
            else
            {
                Kernel.LogFile.Info("Unhandled scanned data: " + data);
            }
        }

    }
}
