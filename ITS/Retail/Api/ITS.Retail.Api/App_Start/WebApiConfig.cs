using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using ITS.Retail.Api.Controllers;
using System.Reflection;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.WRM.Kernel;
using System.Web.Http.Dispatcher;
using ITS.Retail.Api.Providers;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using System.IO;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using System.Xml.Serialization;
using ITS.Retail.Common;
using Microsoft.AspNet.OData.Batch;
using ITS.Retail.WRM.Kernel.Interface;


namespace ITS.Retail.Api
{
    public static class WebApiConfig
    {
        private static void ReadWRMDatabaseConfiguration()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WRMDatabaseConfiguration));

            using (TextReader textReader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(Api.WRM_DATABASE_CONFIGURATION_FILE_PATH)))
            {
                WRMDatabaseConfiguration wrmDatabaseConfiguration = xmlSerializer.Deserialize(textReader) as WRMDatabaseConfiguration;
                XpoHelper.databasetype = wrmDatabaseConfiguration.DataBaseType;
                XpoHelper.sqlserver = wrmDatabaseConfiguration.Server;
                XpoHelper.username = wrmDatabaseConfiguration.Username;
                XpoHelper.pass = wrmDatabaseConfiguration.Password;
                XpoHelper.database = wrmDatabaseConfiguration.DataBase;
            }
        }

        private static IWRMKernel Kernel { get; set; }
        public static IWRMLogModule ApiLogger { get; set; }

        public static string API_ROOT_PATH { get; set; }

        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
#if DEBUG
            // System.Diagnostics.Debugger.Launch();
#endif
            //Web API routes
            config.MapHttpAttributeRoutes();


            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);


            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            ODataModelBuilder builder = new ODataModelBuilder();

            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);

            //Add all persistent model classes to edm
            Dictionary<Type, EntityTypeConfiguration> modelMapping = new Dictionary<Type, EntityTypeConfiguration>();
            List<Type> modelTypes = typeof(Item).Assembly.GetTypes().Where(mc => typeof(BasicObj).IsAssignableFrom(mc)).ToList();
            List<Type> types = new List<Type>(modelTypes.Count);

            foreach (Type modelClass in modelTypes)
            {
                if (modelClass.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() > 0)
                {
                    continue;
                }
                types.Add(modelClass);
                EntityTypeConfiguration entityTypeConfig = builder.AddEntityType(modelClass);

                entityTypeConfig.HasKey(modelClass.GetProperty("Oid"));

                foreach (PropertyInfo propertyInfo in modelClass.GetProperties())
                {
                    if (propertyInfo.Name == "Oid")
                    {
                        continue;
                    }
                    if (typeof(BaseObj).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        entityTypeConfig.AddNavigationProperty(propertyInfo, Microsoft.OData.Edm.EdmMultiplicity.One);
                    }
                    else if (!(propertyInfo.PropertyType.IsValueType == true || propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(DateTime)))
                    {

                        entityTypeConfig.RemoveProperty(propertyInfo);
                    }
                    else if (propertyInfo.Name == "MLValues" || propertyInfo.Name == "SkipOnSavingProcess" || propertyInfo.Name == "Loading" || propertyInfo.Name == "IsLoading" || propertyInfo.Name == "TempObjExists"
                        || propertyInfo.Name == "ObjectSignature" || propertyInfo.Name == "IsDeleted" || propertyInfo.Name == "MasterObjOid"
                        || propertyInfo.Name == "CreatedOn" || propertyInfo.Name == "UpdatedOn")
                    {

                    }
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        entityTypeConfig.AddEnumProperty(propertyInfo);
                        builder.AddEnumType(propertyInfo.PropertyType);
                    }
                    else if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && Nullable.GetUnderlyingType(propertyInfo.PropertyType).IsEnum)
                    {
                        //TODO
                    }
                    else if (typeof(BaseObj).IsAssignableFrom(propertyInfo.PropertyType) == false)
                    {
                        entityTypeConfig.AddProperty(propertyInfo);
                    }
                }
                //entityTypeConfig.AddProperty(modelClass.GetProperty("UpdatedOn"));
                //entityTypeConfig.AddProperty(modelClass.GetProperty("CreatedOn"));
                //entityTypeConfig.AddNavigationProperty(modelClass.GetProperty("CreatedBy"), EdmMultiplicity.One);
                //entityTypeConfig.AddNavigationProperty(modelClass.GetProperty("UpdatedBy"), EdmMultiplicity.One);

                IEnumerable<PropertyInfo> propertiesForNavigation = modelClass.GetProperties().Where(prInfo => typeof(XPBaseCollection).IsAssignableFrom(prInfo.PropertyType));
                foreach (PropertyInfo pInfo in propertiesForNavigation)
                {
                    entityTypeConfig.AddNavigationProperty(pInfo, Microsoft.OData.Edm.EdmMultiplicity.Many);
                }
                modelMapping[modelClass] = entityTypeConfig;

            }
            IEnumerable<Type> allBaseOdataControllers = typeof(WebApiConfig).Assembly.GetTypes().Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                t.BaseType.GetGenericTypeDefinition() == typeof(BaseODataController<>));
            foreach (Type controllerType in allBaseOdataControllers)
            {
                Type modelClass = controllerType.BaseType.GenericTypeArguments[0];
                if (modelMapping.ContainsKey(modelClass))
                {
                    SelfLinkBuilder<Uri> editLink = new SelfLinkBuilder<Uri>(p => new Uri(API_ROOT_PATH + modelClass.Name), false);
                    builder.AddEntitySet(controllerType.Name.Replace("Controller", ""), modelMapping[modelClass]).HasEditLink(editLink);
                    types.Remove(modelClass);
                }
            }
            foreach (Type modelType in types)
            {
                if (modelMapping.ContainsKey(modelType))
                {
                    SelfLinkBuilder<Uri> editLink = new SelfLinkBuilder<Uri>(p => new Uri(API_ROOT_PATH + modelType.Name), false);
                    builder.AddEntitySet(modelType.Name, modelMapping[modelType]).HasEditLink(editLink);
                }
            }
            string result = String.Empty;
            builder.EntitySets.ToList().ForEach(t => { result += t.Name + ": " + t.ClrType.Name + System.Environment.NewLine; });
            Microsoft.OData.Edm.IEdmModel mmodel = builder.GetEdmModel();
            ReadWRMDatabaseConfiguration();
            var a = config.MapODataServiceRoute("ODataRoute", "api",
                mmodel, new DefaultODataPathHandler(),
                ODataRoutingConventions.CreateDefaultWithAttributeRouting("ODataRoute", config),
                new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));

            config.EnableDependencyInjection();
            foreach (var c in ODataRoutingConventions.CreateDefaultWithAttributeRouting("ODataRoute", config).ToList())
            {
                try
                {
                    config.Services.Add(typeof(IODataRoutingConvention), c);
                }
                catch (Exception ex) { continue; }
            }
            config.Expand(QueryOptionSetting.Allowed);
            config.Services.Replace(typeof(IHttpControllerSelector), new WrmODataControllerSelector(config, types));


        }


    }
}
