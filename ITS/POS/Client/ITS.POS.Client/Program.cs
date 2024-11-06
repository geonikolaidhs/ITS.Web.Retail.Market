using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using ITS.POS.Client.Helpers;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Synchronization;
using System.IO;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Kernel;
using NLog;
using ITS.Retail.Platform.Kernel;
using DevExpress.Data.Filtering;
using System.Threading.Tasks;
using System.Reflection;

namespace ITS.POS.Client
{

    static class Program
    {

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int BM_CLICK = 0x00F5;
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(HandleRef hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        //public static IAppContext AppContext { get; private set; }

        public static void SetActiveWindow(Form frm)
        {
            if (frm != null && frm.IsDisposed == false)
            {
                IntPtr hWnd = frm.Handle;
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                    ShowWindow(hWnd, 1);
                }
            }
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        public static bool TaskManagerIsRunning()
        {
            string name = "taskmgr";
            IEnumerable<Process> processesWithTheSameName = Process.GetProcesses().Where(g => g.ProcessName.ToUpper() == name.ToUpper());
            int count = processesWithTheSameName.Count();
            return count >= 1;
        }


        static bool ApplicationAlreadyOpen(out Process p)
        {
            string name = Process.GetCurrentProcess().ProcessName;
            IEnumerable<Process> processesWithTheSameName = Process.GetProcesses().Where(g => g.ProcessName == name);
            int count = processesWithTheSameName.Count();

            p = processesWithTheSameName.FirstOrDefault(x => x != Process.GetCurrentProcess());

            return count > 1;
        }

        public static void BringApplicationToFront()
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (ApplicationIsActivated() == false && TaskManagerIsRunning() == false)
            {
                uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
                uint appThread = GetCurrentThreadId();
                const uint SW_SHOW = 5;
                ////Get the last opened focusable form
                Form form = Application.OpenForms.Cast<Form>()
                            .Where(x => typeof(frmMainBase).IsAssignableFrom(x.GetType()) ||
                                       typeof(frmDialogBase).IsAssignableFrom(x.GetType()) ||
                                       typeof(frmSplashScreen).IsAssignableFrom(x.GetType()))
                            .LastOrDefault();
                if (form != null)
                {
                    if (foreThread != appThread)
                    {
                        AttachThreadInput(foreThread, appThread, true);
                        BringWindowToTop(form.Handle);
                        ShowWindow(form.Handle, SW_SHOW);
                        AttachThreadInput(foreThread, appThread, false);
                    }
                    else
                    {
                        BringWindowToTop(form.Handle);
                        ShowWindow(form.Handle, SW_SHOW);
                    }
                    form.Activate();
                }

            }
        }

        private static IPosKernel Kernel { get; set; }


        [STAThread]
        static void Main()
        {
#if DEBUG
            Debugger.Launch();
#endif
            //Debug.Assert(Debugger.IsAttached == false, "Should be no debugger");
            //if (!Debugger.IsAttached)
            //{
            //    Debug.Assert(Debugger.Launch(), "Debugger not launched");
            //}
            //Debugger.Break();
            //Debug.Assert(Debugger.IsAttached == true, "Debugger should be attached");

            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs eventArgs) =>
                {
                    eventArgs.SetObserved();
                    ((AggregateException)eventArgs.Exception).Handle(ex =>
                    {
                        Kernel.LogFile.Error("Exception type: {0}", ex.GetType());
                        Kernel.LogFile.Error("fullmessage " + ex.GetFullMessage());
                        return true;
                    });
                };

                Process p = null;
                if (ApplicationAlreadyOpen(out p))
                {
                    try
                    {
                        SetForegroundWindow(p.MainWindowHandle);
                        ShowWindow(p.MainWindowHandle, 1);
                        return;
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.GetFullMessage();
                        MessageBox.Show("Switch to application failed. Please try again in a few moments.");
                        return;
                    }
                }

                System.IO.DirectoryInfo posDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                string sourceDirectory = posDir.FullName.ToString();
                if (File.Exists(sourceDirectory + "//" + "Update.bat"))
                {
                    File.Delete(sourceDirectory + "//" + "Update.bat");
                }

                if (File.Exists(sourceDirectory + "//" + "RestartApp.bat"))
                {
                    File.Delete(sourceDirectory + "//" + "RestartApp.bat");
                }

                DevExpress.Skins.SkinManager.EnableFormSkinsIfNotVista();
                DevExpress.UserSkins.BonusSkins.Register();
                UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

                SessionManager sessionManager = new SessionManager();

                UnitOfWork mas = sessionManager.GetSession<Item>();
                UnitOfWork set = sessionManager.GetSession<BarcodeType>();
                UnitOfWork tra = sessionManager.GetSession<DocumentHeader>();
                UnitOfWork mem = sessionManager.MemorySettings;
                sessionManager.UpdateDatabase();


                string globalsXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\Globals.xml";
                string receiptFormatXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\ReceiptFormat.xml";
                string xReportFormatXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\XReportFormat.xml";
                string zReportFormatXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\ZReportFormat.xml";
                string devicesXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\Devices.xml";
                string entityUpdaterModesXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\EntityUpdaterModes.xml";
                string actionLevelsXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\ActionLevels.xml";
                string customActionCodesXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\CustomActionCodes.xml";
                string reportSettingsXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\ReportSettings.xml";
                string extraSettingsXmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration\\ExtraReportSettings.xml";

                ////Kernel composition
                Kernel = new Kernel.PosKernel();
                Kernel.LogFile = LogManager.GetLogger("POSClient");
                PlatformDocumentDiscountService platformDocumentDiscountService = new PlatformDocumentDiscountService();
                PlatformRoundingHandler platformRoundingHandler = new PlatformRoundingHandler();
                PersistentObjectMap persistentObjectMap = new PersistentObjectMap();
                ActionManager actionManager = new ActionManager(ActionFactory.CreateActions(Kernel), actionLevelsXmlPath, customActionCodesXmlPath);
                ConfigurationManager config = new ConfigurationManager(sessionManager, platformRoundingHandler, Kernel);
                platformRoundingHandler.SetOwnerApplicationSettings(config.GetAppSettings());
                config.LoadConfiguration(globalsXmlPath, receiptFormatXmlPath, xReportFormatXmlPath, zReportFormatXmlPath, reportSettingsXmlPath, Kernel.LogFile, extraSettingsXmlPath);
                DeviceManager deviceManager = new DeviceManager();
                deviceManager.LoadDevices(devicesXmlPath, Kernel.LogFile, config.TerminalID);
                AppContext appContext = new AppContext(actionManager, deviceManager, config);
                SynchronizationManager synchronizationManager = new SynchronizationManager(config, appContext, sessionManager, actionManager, Kernel.LogFile, entityUpdaterModesXmlPath);
                FormManager formManager = new FormManager(Kernel, sessionManager, config, appContext);
                DeviceChecker deviceChecker = new DeviceChecker(Kernel, Kernel.LogFile, deviceManager, formManager);
                CustomerService customerService = new CustomerService(sessionManager);
                ItemService itemService = new ItemService(sessionManager, config);
                TotalizersService totalizersService = new TotalizersService(sessionManager, platformRoundingHandler);
                IntermediateModelManager intermediateModelManager = new IntermediateModelManager(sessionManager, persistentObjectMap);
                DocumentService documentService = new DocumentService(sessionManager, config, itemService, platformDocumentDiscountService, platformRoundingHandler);
                ReceiptBuilder receiptBuilder = new ReceiptBuilder(sessionManager, config, documentService, platformRoundingHandler, Kernel.LogFile, Kernel);
                PlatformPromotionService platformPromotionService = new PlatformPromotionService(documentService, intermediateModelManager, platformRoundingHandler, platformDocumentDiscountService, config.DefaultCustomerOid);
                PromotionService promotionService = new PromotionService(sessionManager, config, documentService, receiptBuilder, platformPromotionService);
                ActionExecutor actionExecutor = new ActionExecutor(appContext, sessionManager, config, deviceManager, actionManager);
                ScannedCodeHandler scannedCodeHandler = new ScannedCodeHandler(itemService, sessionManager, actionManager, formManager, appContext, customerService, deviceManager);
                VatFactorService vatFactorChecker = new VatFactorService(sessionManager, config);
                PriceCreator priceCreator = new PriceCreator(itemService, config, Kernel.LogFile);
                ApplicationDesignModeProvider applicationDesignModeProvider = new ApplicationDesignModeProvider();
                LocalizationResolver localizationResolver = new LocalizationResolver(Kernel.LogFile, applicationDesignModeProvider);
                ControlLocalizer controlLocalizer = new ControlLocalizer(localizationResolver, formManager);
                OposReportService oposReportService = new OposReportService(sessionManager, config, Kernel);

                Kernel.RegisterModule<IDeviceManager, DeviceManager>(deviceManager);
                Kernel.RegisterModule<ISynchronizationManager, SynchronizationManager>(synchronizationManager);
                Kernel.RegisterModule<ISessionManager, SessionManager>(sessionManager);
                Kernel.RegisterModule<IConfigurationManager, ConfigurationManager>(config);
                Kernel.RegisterModule<IAppContext, AppContext>(appContext);
                Kernel.RegisterModule<IFormManager, FormManager>(formManager);
                Kernel.RegisterModule<IItemService, ItemService>(itemService);
                Kernel.RegisterModule<IDocumentService, DocumentService>(documentService);
                Kernel.RegisterModule<IPromotionService, PromotionService>(promotionService);
                Kernel.RegisterModule<IReceiptBuilder, ReceiptBuilder>(receiptBuilder);
                Kernel.RegisterModule<ITotalizersService, TotalizersService>(totalizersService);
                Kernel.RegisterModule<IActionManager, ActionManager>(actionManager);
                Kernel.RegisterModule<IActionExecutor, ActionExecutor>(actionExecutor);
                Kernel.RegisterModule<IScannedCodeHandler, ScannedCodeHandler>(scannedCodeHandler);
                Kernel.RegisterModule<ICustomerService, CustomerService>(customerService);
                Kernel.RegisterModule<IPlatformRoundingHandler, PlatformRoundingHandler>(platformRoundingHandler);
                Kernel.RegisterModule<IPlatformDocumentDiscountService, PlatformDocumentDiscountService>(platformDocumentDiscountService);
                Kernel.RegisterModule<IPlatformPersistentObjectMap, PlatformPersistentObjectMap>(persistentObjectMap);
                Kernel.RegisterModule<IIntermediateModelManager, IntermediateModelManager>(intermediateModelManager);
                Kernel.RegisterModule<IPlatformPromotionService, PlatformPromotionService>(platformPromotionService);
                Kernel.RegisterModule<IVatFactorService, VatFactorService>(vatFactorChecker);
                Kernel.RegisterModule<IPriceCreator, PriceCreator>(priceCreator);
                Kernel.RegisterModule<IDeviceChecker, DeviceChecker>(deviceChecker);
                Kernel.RegisterModule<ILocalizationResolver, LocalizationResolver>(localizationResolver);
                Kernel.RegisterModule<IControlLocalizer, ControlLocalizer>(controlLocalizer);
                Kernel.RegisterModule<IOposReportService, OposReportService>(oposReportService);

                (appContext as AppContext).Initialize(Kernel);
                UtilsHelper.InitKeyMaps(config.CurrentTerminalOid, sessionManager);
                UtilsHelper.InitKeyCharacters();


                if (config.CurrentTerminalOid == Guid.Empty)
                {
                    throw new Exception(POSClientResources.INVALID_TERMINAL_OID);
                }

                try
                {
                    actionManager.GetAction(eActions.OPEN_SCANNERS).Execute();
                }
                catch (Exception e)
                {
                    Kernel.LogFile.Info(e, "Program:Main,Exception catched");
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
                }

                if (deviceChecker.CheckDevices() == false)
                {
                    ApplicationExit();
                    return;
                }

                if (appContext.SupportingForm != null)
                {
                    appContext.SupportingForm.Show();
                }

                appContext.MainForm.Show();
                appContext.MainForm.Hide();

                appContext.SplashForm.Show();

                Application.DoEvents();
                appContext.SplashForm.Focus();
                appContext.SplashForm.BringToFront();
                SetActiveWindow(appContext.SplashForm);
                Application.DoEvents();
                synchronizationManager.Initialize();
                synchronizationManager.GetUpdatesThread.Start();

                if (!config.DemoMode)
                {
                    synchronizationManager.PostTransactionsThread.Start();
                }
                synchronizationManager.PublishStatusThread.Start();
                synchronizationManager.UpdateStatusThread.Start();
                if (config.AutoFocus)
                {
                    synchronizationManager.AutoFocusThread.Start();
                }

                synchronizationManager.PostSynchronizationInfoThread.Start();


                if (!config.DemoMode)
                {
                    synchronizationManager.GetDocumentSequences(documentService);
                    synchronizationManager.GetZReportSequences(documentService);
                }
                else
                {
                    //Demo mode, create sequences if they do not exist
                    try
                    {
                        List<Guid> documentSeriesList = synchronizationManager.DocumentSeriesList;
                        foreach (Guid documentSeries in documentSeriesList)
                        {
                            if (documentService.SequenceExists(documentSeries) == false)
                            {
                                DocumentSequence sequence = new DocumentSequence(sessionManager.GetSession<DocumentSequence>());
                                sequence.DocumentSeries = documentSeries;
                                sequence.Save();
                                sequence.Session.CommitTransaction();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetFullMessage());
                        Kernel.LogFile.Info(ex, "Program:Main,Exception catched:" + ex.GetFullMessage() + Environment.NewLine + ex.GetFullStackTrace());
                        if (MessageBox.Show(POSClientResources.INIT_ERROR + ": " + ex.Message, POSClientResources.INIT_ERROR, MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            ApplicationExit();
                        }
                    }
                }

                actionManager.GetAction(eActions.LOAD_EXISTING_DOCUMENT).Execute();
                actionManager.GetAction(eActions.LOAD_EXISTING_DOCUMENTS_ON_HOLD).Execute();
                if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    actionManager.GetAction(eActions.CHECK_STATUS_WITH_FISCAL_PRINTER).Execute();
                }
                Item item = sessionManager.FindObject<Item>(new BinaryOperator("Oid", Guid.Empty));
                Application.Run();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullMessage());
                Kernel.LogFile.Info(ex, "Program:Main,Exception catched:" + ex.GetFullMessage() + Environment.NewLine +
                ex.GetFullStackTrace());
                if (MessageBox.Show(POSClientResources.INIT_ERROR + ": " + ex.Message, POSClientResources.INIT_ERROR, MessageBoxButtons.OK) == DialogResult.OK)
                {
                    ApplicationExit();
                }
            }
        }

        public static readonly int ExitTimeout = 120000; // Wait 2 mins before killing all threads

        public static void ApplicationExit()
        {
            ISynchronizationManager Synchronizer = Kernel.GetModule<ISynchronizationManager>();
            List<CustomThread> applicationBackgroundThreads = new List<CustomThread>(10);
            applicationBackgroundThreads.Add(Synchronizer.GetUpdatesThread);
            applicationBackgroundThreads.Add(Synchronizer.PostTransactionsThread);
            applicationBackgroundThreads.Add(Synchronizer.PublishStatusThread);
            applicationBackgroundThreads.Add(Synchronizer.UpdateStatusThread);
            applicationBackgroundThreads.Add(Synchronizer.AutoFocusThread);
            applicationBackgroundThreads.Add(Synchronizer.PostSynchronizationInfoThread);

            applicationBackgroundThreads.ForEach(thread => thread.Abort());

            IFormManager formManager = Kernel.GetModule<IFormManager>();
            frmMessageBox dialog = formManager.CreateMessageBox(POSClientResources.PLEASE_WAIT + "...");
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            try
            {
                appContext.MainForm.Invoke((MethodInvoker)delegate ()
                {
                    dialog.btnOK.Visible = false;
                    dialog.btnCancel.Visible = false;
                    dialog.btnRetry.Visible = false;
                    dialog.CanBeClosedByUser = false;
                    dialog.Show();
                    dialog.BringToFront();
                });
            }
            catch
            {
            }

            Application.DoEvents();

            int eachThreadTimeout = ExitTimeout / applicationBackgroundThreads.Count;
            foreach (CustomThread thread in applicationBackgroundThreads)
            {
                if (thread.IsAlive)
                {
                    thread.Join(eachThreadTimeout);
                }
            }

            Environment.Exit(0);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).GetFullMessage());
            Kernel.LogFile.Info((Exception)e.ExceptionObject, "Program:Main,Exception catched:" + ((Exception)e.ExceptionObject).Message + Environment.NewLine +
               ((Exception)e.ExceptionObject).StackTrace);
        }
    }
}