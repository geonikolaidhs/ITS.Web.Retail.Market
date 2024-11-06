using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ITS.POS.Hardware;
using ITS.POS.Model.Transactions;
using System.IO.Ports;
using ITS.POS.Client.Receipt;
using ITS.POS.Client.Actions.ActionParameters;
using System.Reflection;
using System.Threading;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Synchronization;
using ITS.POS.Client.Actions;
using NLog;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Actions.Permission;
using ITS.Retail.Platform;
using System.Collections.Concurrent;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.POS.Hardware.Wincor.Fiscal;
using System.Threading.Tasks;
using ITS.POS.Client.UserControls;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Manages and contains info about the application's runtime status (Current Document, Current Document Line etc).
    /// </summary>
    public class AppContext : IAppContext
    {
        private IActionManager ActionManager { get; set; }
        private IDeviceManager DeviceManager { get; set; }
        private IConfigurationManager ConfigurationManager { get; set; }

        public AppContext(IActionManager actionManager, IDeviceManager deviceManager, IConfigurationManager config)
        {
            this.ActionManager = actionManager;
            this.DeviceManager = deviceManager;
            this.ConfigurationManager = config;
        }

        /// <summary>
        /// Gets or sets the application's splash form.
        /// </summary>
        public frmSplashScreen SplashForm { get; set; }

        /// <summary>
        /// Gets or sets the current Fiscal Date.
        /// </summary>
        public DateTime FiscalDate { get; set; }

        /// <summary>
        /// Gets or sets the current open document.
        /// </summary>
        public DocumentHeader CurrentDocument { get; set; }


        private DocumentDetail _currentDocumentLine;

        /// <summary>
        /// Gets or sets the current document line.
        /// </summary>
        public DocumentDetail CurrentDocumentLine
        {
            get
            {
                return _currentDocumentLine;
            }
            set
            {
                DocumentDetail previousLine = _currentDocumentLine;
                _currentDocumentLine = value;
                if (previousLine != _currentDocumentLine && value != null)
                {
                    CurrentDocumentLineChanged(previousLine, _currentDocumentLine);
                }
            }
        }

        public DocumentPayment _currentDocumentPayment;

        /// <summary>
        /// Gets or sets the current document payment.
        /// </summary>
        public DocumentPayment CurrentDocumentPayment
        {
            get
            {
                return _currentDocumentPayment;
            }
            set
            {
                DocumentPayment previousPayment = _currentDocumentPayment;
                _currentDocumentPayment = value;
                if (previousPayment != _currentDocumentPayment && value != null)
                {
                    CurrentPaymentChanged(previousPayment, _currentDocumentPayment);
                }
            }
        }

        private void CurrentDocumentLineChanged(DocumentDetail previousLine, DocumentDetail currentLine)
        {
            if (currentLine != null && currentLine.CustomDescription != null)
            {
                ActionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(currentLine, true, true));
            }
        }

        private void CurrentPaymentChanged(DocumentPayment previousPayment, DocumentPayment currentPayment)
        {
            if (currentPayment != null)
            {
                PoleDisplay customerDisplay = DeviceManager.GetCustomerPoleDisplay();
                string[] customerLines = DeviceManager.GetPaymentPoleDisplayLines(currentPayment.DocumentHeader, currentPayment, customerDisplay, false);
                ActionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));

                PoleDisplay cashierDisplay = DeviceManager.GetCustomerPoleDisplay();
                string[] cashierLines = DeviceManager.GetPaymentPoleDisplayLines(currentPayment.DocumentHeader, currentPayment, cashierDisplay, true);
                ActionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
            }
        }

        private eMachineStatus _machineStatus;

        /// <summary>
        /// Gets or sets the touch pad form.
        /// </summary>
        public frmTouchPad TouchPadPopup { get; set; }

        /// <summary>
        /// Gets or sets the current open document's customer.
        /// </summary>
        public Customer CurrentCustomer { get; set; }

        /// <summary>
        /// Gets or sets the current open daily totals.
        /// </summary>
        public DailyTotals CurrentDailyTotals { get; set; }

        /// <summary>
        /// Gets or sets the current open user daily totals.
        /// </summary>
        public UserDailyTotals CurrentUserDailyTotals { get; set; }

        /// <summary>
        /// Gets or sets the currently logged in user.
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the application's main form.
        /// </summary>
        public frmMainBase MainForm { get; set; }

        /// <summary>
        /// Gets or sets the application's supporting form.
        /// </summary>
        public frmSupportingBase SupportingForm { get; set; }

        /// <summary>
        /// Gets or sets the currenlty focused input that can receive scanner data.
        /// </summary>
        public IScannerInput CurrentFocusedScannerInput { get; set; }

        /// <summary>
        /// Gets or sets the current shift's documents that are on hold.
        /// </summary>
        public List<DocumentHeader> DocumentsOnHold { get; set; }

        public void Initialize(IPosKernel kernel)
        {
            DocumentsOnHold = new List<DocumentHeader>();

            DynamicModules.AssembliesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\modules";
            DynamicModules.LoadAssemblies();
            TouchPadPopup = new frmTouchPad();

            List<Type> types = DynamicModules.GetTypes();
            IEnumerable<Type> filteredTypes = types.Where(g => g.IsSubclassOf(typeof(frmMainBase)));
            if (filteredTypes.Count() == 0)
            {
                this.MainForm = new frmMain();
            }
            else
            {
                this.MainForm = Activator.CreateInstance(filteredTypes.First()) as frmMainBase;
            }
            this.MainForm.Initialize(kernel);
            this.MainForm.IntitializeSubscriptions();



            SplashForm = new frmSplashScreen(kernel);
            SplashForm.IntitializeSubscriptions();

            filteredTypes = types.Where(g => g.IsSubclassOf(typeof(frmSupportingBase)));
            if (filteredTypes.Count() > 0 && Screen.AllScreens.Length > 1)
            {

                this.SupportingForm = Activator.CreateInstance(filteredTypes.First()) as frmSupportingBase;
                this.SupportingForm.Initialize(kernel);
                this.SupportingForm.IntitializeSubscriptions();
                this.SupportingForm.StartPosition = FormStartPosition.Manual;

                System.Drawing.Rectangle bounds = Screen.AllScreens[1].Bounds;
                this.SupportingForm.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// Gets the application's status.
        /// </summary>
        /// <returns></returns>
        public eMachineStatus GetMachineStatus()
        {
            return _machineStatus;
        }

        /// <summary>
        /// Sets the application's machine status and optionally displays an appropriate message to the pole displays.
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="showMessage"></param>
        /// <param name="messageDelay"></param>
        public void SetMachineStatus(eMachineStatus newStatus, bool showMessage = true, int messageDelay = -1)
        {
            LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            _machineStatus = newStatus;
            if (showMessage)
            {
                string[] customerLines = new string[0];
                string[] cashierLines = new string[0];
                switch (newStatus)
                {
                    case eMachineStatus.PAUSE:
                        customerLines = new string[] { POSClientResources.CASH_REGISTRY_CLOSED.ToUpperGR() };
                        cashierLines = new string[] { POSClientResources.PAUSED.ToUpperGR() };
                        break;
                    case eMachineStatus.SALE:
                        customerLines = new string[] { POSClientResources.NEXT_CUSTOMER.ToUpperGR() };
                        cashierLines = new string[] { POSClientResources.NEXT_CUSTOMER.ToUpperGR() };
                        break;
                    case eMachineStatus.CLOSED:
                        customerLines = new string[] { POSClientResources.CASH_REGISTRY_CLOSED.ToUpperGR() };
                        cashierLines = new string[] { POSClientResources.CASH_REGISTRY_CLOSED.ToUpperGR() };
                        break;
                    case eMachineStatus.OPENDOCUMENT_PAYMENT:
                        {
                            PoleDisplay customerDisplay = DeviceManager.GetCustomerPoleDisplay();
                            customerLines = DeviceManager.GetPaymentPoleDisplayLines(this.CurrentDocument,
                                                     this.CurrentDocumentPayment,
                                                     customerDisplay,
                                                     false);
                            PoleDisplay cashierDisplay = DeviceManager.GetCashierPoleDisplay();
                            cashierLines = DeviceManager.GetPaymentPoleDisplayLines(this.CurrentDocument,
                                                     this.CurrentDocumentPayment,
                                                     cashierDisplay,
                                                     true);
                            break;
                        }
                    case eMachineStatus.OPENDOCUMENT:
                        {
                            if (this.CurrentDocumentLine != null && this.CurrentDocumentLine.CustomDescription != null)
                            {
                                customerLines = DeviceManager.GetDocumentDetailPoleDisplayLines(this.CurrentDocumentLine, false);
                                cashierLines = DeviceManager.GetDocumentDetailPoleDisplayLines(this.CurrentDocumentLine, true);
                            }
                            break;
                        }
                    case eMachineStatus.DAYSTARTED:
                        customerLines = new string[] { POSClientResources.CASH_REGISTRY_CLOSED.ToUpperGR() };
                        cashierLines = new string[] { POSClientResources.DAY_START.ToUpperGR(), "v " + DeviceManager.GetVisibleVersion(ConfigurationManager.FiscalMethod) };
                        break;
                }

                if (messageDelay > 0)
                {
                    Thread showDisplayThread = new Thread(() =>
                    {
                        Thread.Sleep(messageDelay);
                        if (this.GetMachineStatus() == newStatus) //check if after the wait, the message is still valid
                        {
                            ActionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));
                            ActionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
                        }
                    });
                    showDisplayThread.Start();
                }
                else
                {
                    ActionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));
                    ActionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
                }

            }

            ActionManager.GetAction(eActions.PUBLISH_MACHINE_STATUS).Execute(new ActionPublishMachineStatusParams(newStatus));
        }

        /// <summary>
        /// Application handler that is used to determine what is the CurrentFocusedScannerInput.
        /// </summary>
        /// <param name="appContext"></param>
        /// <returns></returns>
        public EventHandler ScannerInputOnEnterHandler(IAppContext appContext)
        {
            return (object sender, EventArgs e) =>
            {
                appContext.CurrentFocusedScannerInput = sender as IScannerInput;
            };
        }

        /// <summary>
        /// Application handler that is used to determine what is the CurrentFocusedScannerInput.
        /// </summary>
        /// <param name="appContext"></param>
        /// <returns></returns>
        public EventHandler ScannerInputOnLeaveHandler(IAppContext appContext)
        {
            return (object sender, EventArgs e) =>
            {
                appContext.CurrentFocusedScannerInput = appContext.MainForm.GetMainInput();
            };
        }
    }
}
