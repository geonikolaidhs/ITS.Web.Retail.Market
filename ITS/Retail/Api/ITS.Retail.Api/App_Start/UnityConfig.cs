using ITS.Retail.WRM.Kernel;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace ITS.Retail.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            UnityContainer container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            WRMLogModule logModule = new WRMLogModule("ITS.Retail.Api", false);
            WebApiConfig.ApiLogger = logModule;
            container.RegisterType<IWRMDbModule, WRMDbModule>(new TransientLifetimeManager());
            container.RegisterType<IWRMUserModule, WRMUserModule>(new TransientLifetimeManager());
            container.RegisterType<IWRMUserDbModule, WRMUserDbModule>(new TransientLifetimeManager());
            container.RegisterType<IWRMLogModule, WRMLogModule>();
            GC.KeepAlive(WebApiConfig.ApiLogger);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }

}
