using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using System.Threading.Tasks;
using System.Diagnostics;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;

namespace ITS.Retail.DesktopClient.StoreControllerClient
{
    public static class Program
    {

        public static string UserPassword { get; set; }
        public const string CONFIGURATION_FOLDER = "\\ITS\\StoreController";
        public const string CONFIGURATION_FILE = "\\settings.xml";

        /// <summary>
        /// Gets the Global application Logger
        /// </summary>
        public static Logger Logger { get; private set; }

        public static StoreControllerClientSettings Settings { get; set; }

        public static string SettingsFilePath { get; private set; }

        private static void LoadReferencedAssembly(Assembly assembly, int level = 0)
        {
#if DEBUG
            Logger.Trace(new string('-', level) + assembly.FullName);
#endif
            foreach (AssemblyName name in assembly.GetReferencedAssemblies())
            {
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName == name.FullName))
                {
                    try
                    {
                        LoadReferencedAssembly(Assembly.Load(name), level + 1);
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(ex, "Failed to load assembly " + name);
                    }
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("Metropolis");
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


#if DEBUG
            Debugger.Launch();
#endif

            try
            {
                SplashScreenManager.ShowForm(typeof(ITSSplashScreen));
                SplashScreenManager.Default.SetWaitFormDescription(ResourcesLib.Resources.StartingApp);
                Logger = LogManager.GetLogger("StoreControllerClient");
                Task.Factory.StartNew(() =>
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        LoadReferencedAssembly(assembly);
                    }
                    using (var gridLevelNode = new DevExpress.XtraGrid.GridLevelNode())
                    {
                    }
                    using (var gridView = new DevExpress.XtraGrid.Views.Grid.GridView())
                    {
                    }
                    using (var gridColumn = new DevExpress.XtraGrid.Columns.GridColumn())
                    {
                    }
                    using (var ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl())
                    {
                    }
                    using (var repositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit())
                    {
                    }
                });
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + CONFIGURATION_FOLDER;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                SettingsFilePath = path + CONFIGURATION_FILE;
                Settings = new StoreControllerClientSettings(SettingsFilePath, true);
                Settings.Load();
                SetCulture();
                if (Settings.StoreControllerURL == null)
                {
                    ShowSettings();
                    Settings.Load();
                    if (Settings.StoreControllerURL == null)
                    {
                        return;
                    }
                    SplashScreenManager.ShowForm(typeof(ITSSplashScreen));
                }
                //#if !DEBUG                
                SplashScreenManager.Default.SetWaitFormDescription(ResourcesLib.Resources.Connecting);
                //#endif
                XpoDefault.DataLayer = null;// XpoDefault.GetDataLayer(Settings.WebServiceURI, AutoCreateOption.None);
                XpoDefault.Session = null;
                XpoHelper.databasetype = DBType.Remote;
                XpoHelper.WEBServiceURI = Settings.WebServiceURI;
                //XpoHelper.HQURI = Settings.ITSWebServiceHQURI;

                DialogResult dialogResult = DialogResult.None;
                using (LoginForm loginForm = new LoginForm(false))
                {
                    dialogResult = loginForm.ShowDialog();
                }

                if (dialogResult == DialogResult.OK)
                {
                    SplashScreenManager.CloseForm(false, true);
                    SplashScreenManager.ShowForm(typeof(ITSSplashScreen));
                    SplashScreenManager.Default.SetWaitFormDescription(string.Format(ResourcesLib.Resources.LoadingP0, "Application"));
                    using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                    {
                        if (itsService.GetDbType() == DBType.postgres)
                        {
                            Common.SingleObjectXtraReport.RemoteDBType = Platform.Enumerations.DataBaseConnectionType.PostGreSql;
                        }
                        else if (itsService.GetDbType() == DBType.SQLServer)
                        {
                            Common.SingleObjectXtraReport.RemoteDBType = Platform.Enumerations.DataBaseConnectionType.MSSql;
                        }
                        else if (itsService.GetDbType() == DBType.Oracle)
                        {
                            Common.SingleObjectXtraReport.RemoteDBType = Platform.Enumerations.DataBaseConnectionType.Oracle;
                        }
                        else
                        {
                            throw new Exception("Unsupported Database Type");
                        }

                    }
                    MainForm mainForm = new MainForm();
                    mainForm.Shown += MainForm_Shown;
                    mainForm.Show();
                    mainForm.Focus();
                    Application.Run();
                }
            }
            catch (UnableToOpenDatabaseException exc)
            {
                Logger.Error(exc, "UnableToOpenDatabaseException");
                XtraMessageBox.Show(ResourcesLib.Resources.ConnectionErrorReviseSettings + " " + exc.Message);
                ShowSettings();

            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Unhandled");
                using (Form messageForm = new Form() { TopMost = true })
                {
                    XtraMessageBox.Show(messageForm, exception.Message, ResourcesLib.Resources.Error);
                }
            }
        }

        private static void MainForm_Shown(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                PreloadReports();
            });
            (sender as Form).Shown -= MainForm_Shown;
        }

        private static void PreloadReports()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection<CustomReport> reports = new XPCollection<CustomReport>(uow, new BinaryOperator("ObjectType", "DocumentHeader"));
                CompanyNew owner = uow.GetObjectByKey<CompanyNew>(Settings.StoreControllerSettings.Store.Owner.Oid);
                User user = uow.GetObjectByKey<User>(Settings.CurrentUser.Oid);
                string title, descr;
                Dictionary<XtraReportBaseExtension, string> xreportList = reports.AsParallel()
                    .Select(customReport => new { Report = ReportsHelper.GetXtraReport(customReport.Oid, owner, user, null, out title, out descr), Description = customReport.Description })
                    .Where(x => x.Report != null)
                    .ToDictionary(x => x.Report, x => x.Description);
                KeyValuePair<XtraReportBaseExtension, string> pair = xreportList.FirstOrDefault();
            }
        }

        static void frm_Shown(object sender, EventArgs e)
        {
            Form form = sender as Form;
            if (form != null)
            {
                form.Close();
            }
        }

        public static void ShowSettings()
        {
            using (SettingsForm form = new SettingsForm(SettingsFilePath))
            {
                form.ShowDialog();
            }
        }

        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = Settings.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = Settings.CultureInfo;
        }
    }
}
