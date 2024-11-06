using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ITS.Retail.WRM.Kernel;
using ITS.Retail.WRM.Kernel.Interface;
using System.Xml.Serialization;
using System.IO;
using System;

namespace ITS.Retail.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IWRMLogModule WRMLogModule { get; internal set; }

        private const string WRM_DATABASE_CONFIGURATION_FILE_PATH = "~/Configuration/wrm.xml";

        IWRMKernel Kernel { get; set; }

        void PrepareDBConnection()
        {
            ReadWRMDatabaseConfiguration();

            //dummy command, just to initialize the connection with database
            Task.Run(() =>
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    VatCategory vc = uow.FindObject<VatCategory>(null);
                }
            });
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            WebApiConfig.API_ROOT_PATH = Server.MapPath("~/api");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            PrepareDBConnection();
            GlobalConfiguration.Configuration.EnsureInitialized();
            GC.KeepAlive(WebApiConfig.ApiLogger);
        }

        protected void ReadWRMDatabaseConfiguration()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WRMDatabaseConfiguration));

            using (TextReader textReader = new StreamReader(Server.MapPath(WRM_DATABASE_CONFIGURATION_FILE_PATH)))
            {
                WRMDatabaseConfiguration wrmDatabaseConfiguration = xmlSerializer.Deserialize(textReader) as WRMDatabaseConfiguration;
                XpoHelper.databasetype = wrmDatabaseConfiguration.DataBaseType;
                XpoHelper.sqlserver = wrmDatabaseConfiguration.Server;
                XpoHelper.username = wrmDatabaseConfiguration.Username;
                XpoHelper.pass = wrmDatabaseConfiguration.Password;
                XpoHelper.database = wrmDatabaseConfiguration.DataBase;
            }

        }
    }
}
