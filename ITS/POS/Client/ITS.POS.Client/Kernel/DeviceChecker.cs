using ITS.POS.Client.Forms;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides device validation logic.
    /// </summary>
    public class DeviceChecker : IDeviceChecker
    {
        internal class DeviceCheckerRunner : IActionProgress
        {
            public DeviceCheckerRunner(Logger logger, IDeviceManager deviceManager, IFormManager formManager)
            {

                this.FormManager = formManager;
                this.DeviceManager = deviceManager;
                this.Logger = logger;
            }

            private IFormManager FormManager { get; set; }
            private IDeviceManager DeviceManager { get; set; }
            private Logger Logger { get; set; }

            public frmAsyncMessageBox MessageBox { get; set; }

            public event ActionProgressStartedEventHandler ProgressStarted;

            public event ActionProgressChangedEventHandler ProgressChanged;

            public event ActionProgressCompletedEventHandler ProgressCompleted;

            public bool? Result { get; set; }

            public void DoWork()
            {
                ProgressStarted?.Invoke(this, 1, DeviceManager.Devices.Count, "Checking Devices..." + Environment.NewLine);
                bool continueApplication = true, windowAutoClose = true;
                int counter = 1;
                foreach (IGrouping<Type, Device> deviceGroup in DeviceManager.Devices.GroupBy(x => x.GetType()))
                {
                    string message, progressMessage;
                    if (deviceGroup.Key.GetCustomAttributes(typeof(RequireOnlyOneSuccessfulDeviceTypeAttribute), true) == null || deviceGroup.Count() == 1)
                    {
                        foreach (Device device in deviceGroup)
                        {
                            eDeviceCheckResult result = eDeviceCheckResult.INFO;
                            message = "";
                            if (device.ShouldRunOnMainThread)
                            {
                                this.MessageBox.Invoke((MethodInvoker)delegate ()
                                {
                                    result = device.CheckDevice(out message);
                                });
                            }
                            else
                            {
                                result = device.CheckDevice(out message);
                            }
                            KeyValuePair<bool, bool> handleDeviceCheckResult = HandleDeviceCheckResult(device, result, message, out progressMessage);
                            continueApplication &= handleDeviceCheckResult.Key;
                            windowAutoClose &= handleDeviceCheckResult.Value;
                            if (ProgressChanged != null) ProgressChanged(this, counter, progressMessage);
                            counter++;
                        }
                    }
                    else
                    {
                        List<eDeviceCheckResult> results = new List<eDeviceCheckResult>();
                        bool temporaryContinueApplication = false, temporaryWindowAutoClose = false;
                        foreach (Device device in deviceGroup)
                        {
                            eDeviceCheckResult result = device.CheckDevice(out message);
                            KeyValuePair<bool, bool> handleDeviceCheckResult = HandleDeviceCheckResult(device, result, message, out progressMessage);
                            temporaryContinueApplication |= handleDeviceCheckResult.Key;
                            temporaryWindowAutoClose |= handleDeviceCheckResult.Value;
                            if (ProgressChanged != null) ProgressChanged(this, counter, progressMessage);
                            counter++;
                        }
                        continueApplication &= temporaryContinueApplication;

                    }
                }
                if (windowAutoClose)
                {
                    if (MessageBox.InvokeRequired)
                    {
                        MessageBox.Invoke((MethodInvoker)delegate () { MessageBox.Close(); });
                    }
                    else
                    {
                        MessageBox.Close();
                    }
                }
                Result = continueApplication;
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(this, "", Result);
                }
            }


            private KeyValuePair<bool, bool>
                HandleDeviceCheckResult(Device device, eDeviceCheckResult result, string message, out string progressMessage)
            {
                Application.DoEvents();
                //Thread.Sleep(1000);
                Logger.Info(String.Format("Device {0}: Check Result: {1}\nMessage: {2}", device.DeviceName, result, message));
                progressMessage = device.DeviceName + ": ";
                switch (result)
                {
                    case eDeviceCheckResult.SUCCESS:
                        progressMessage += " OK!" + Environment.NewLine;
                        return new KeyValuePair<bool, bool>(true, true);
                    case eDeviceCheckResult.INFO:
                        progressMessage += " Recoverable Error (" + message + ")" + Environment.NewLine;
                        return new KeyValuePair<bool, bool>(true, true);
                    case eDeviceCheckResult.WARNING:
                        progressMessage += " Recoverable Error (" + message + ")" + Environment.NewLine;
                        return new KeyValuePair<bool, bool>(true, false);
                    case eDeviceCheckResult.FAILURE:
                    default:
                        progressMessage += " Fatal Error (" + message + ")" + Environment.NewLine;
                        return new KeyValuePair<bool, bool>(false, false);
                }
            }
        }


        private IFormManager FormManager { get; set; }
        private IDeviceManager DeviceManager { get; set; }
        private Logger Logger { get; set; }
        private IPosKernel Kernel { get; set; }

        public DeviceChecker(IPosKernel kernel, Logger logger, IDeviceManager deviceManager, IFormManager formManager)
        {
            this.FormManager = formManager;
            this.DeviceManager = deviceManager;
            this.Logger = logger;
            this.Kernel = kernel;
        }

        //Async Version
        public bool CheckDevices()
        {
            DeviceCheckerRunner runner = new DeviceCheckerRunner(Logger, DeviceManager, FormManager);
            using (frmAsyncMessageBox frm = new frmAsyncMessageBox(Kernel, runner))
            {
                frm.btnRetry.Visible = false;
                frm.btnOK.Visible = false;
                runner.MessageBox = frm;
                Task asyncTask = new Task(() => { runner.DoWork(); });
                frm.Shown += (sender, arg) =>
                {
                    asyncTask.Start();
                };
                frm.ShowDialog();
                if (runner.Result.HasValue && runner.Result.Value == false)
                {
                    return false;
                }
                return true;
            }



        }
        /*

        /// <summary>
        /// Checks if any device has any problem, and returns a boolean that determines if the application should continue.
        /// </summary>
        /// <returns></returns>
        public bool CheckDevices()
        {
            using (frmMessageBox msgBox = FormManager.CreateMessageBox("Checking Devices..." + Environment.NewLine))
            {
                msgBox.btnCancel.Visible = false;
                msgBox.btnRetry.Visible = false;
                msgBox.btnOK.Visible = false;
                msgBox.AcceptButton = null;
                msgBox.CancelButton = null;
                msgBox.CanBeClosedByUser = false;
                string message;
                bool continueApplication = true;
                msgBox.Shown += (sender, e) =>
                {
                    foreach (IGrouping<Type, Device> deviceGroup in DeviceManager.Devices.GroupBy(x=>x.GetType()))
                    {
                        
                        if(deviceGroup.Key.GetCustomAttributes(typeof(RequireOnlyOneSuccessfulDeviceTypeAttribute), true)==null || deviceGroup.Count()==1)
                        {
                            foreach (Device device in deviceGroup)
                            {
                                eDeviceCheckResult result = device.CheckDevice(out message);
                                continueApplication &= HandleDeviceCheckResult(msgBox, device, result, message);
                                Application.DoEvents();
                                Application.DoEvents();
                            }
                        }
                        else
                        {
                            List<eDeviceCheckResult> results = new List<eDeviceCheckResult>();
                            bool temporaryContinueApplication = false;
                            foreach (Device device in deviceGroup)
                            {
                                eDeviceCheckResult result = device.CheckDevice(out message);
                                temporaryContinueApplication |= HandleDeviceCheckResult(msgBox, device, result, message);
                                Application.DoEvents();
                                Application.DoEvents();
                            }
                            continueApplication &= temporaryContinueApplication;
                        }
                        Application.DoEvents();
                    }
                    if (msgBox.btnCancel.Visible)
                    {
                        msgBox.btnCancel.Enabled = true;
                        msgBox.CanBeClosedByUser = true;
                        msgBox.Focus();
                    }
                    else
                    {
                        msgBox.DialogResult = DialogResult.Cancel;
                    }
                };
                msgBox.ShowDialog();
                
                return continueApplication;
            }
        }

        private bool HandleDeviceCheckResult(frmMessageBox msgBox, Device device, eDeviceCheckResult result, string message)
        {
            Logger.Info(String.Format("Device {0}: Check Result: {1}\nMessage: {2}", device.DeviceName, result, message));
            msgBox.Message += device.DeviceName + ": ";
            switch (result)
            {
                case eDeviceCheckResult.SUCCESS:
                    msgBox.Message += " OK!" + Environment.NewLine;
                    return true;
                case eDeviceCheckResult.INFO:
                    msgBox.Message += " Recoverable Error (" + message + ")" + Environment.NewLine;
                    return true;
                case eDeviceCheckResult.WARNING:
                    msgBox.Message += " Recoverable Error (" + message + ")" + Environment.NewLine;
                    msgBox.btnCancel.Visible = true;
                    msgBox.btnCancel.Enabled = false;
                    return true;
                case eDeviceCheckResult.FAILURE:
                default:
                    msgBox.Message += " Fatal Error (" + message + ")" + Environment.NewLine;
                    msgBox.btnCancel.Visible = true;
                    msgBox.btnCancel.Enabled = false;
                    return false;
            }
        }*/
    }
}
