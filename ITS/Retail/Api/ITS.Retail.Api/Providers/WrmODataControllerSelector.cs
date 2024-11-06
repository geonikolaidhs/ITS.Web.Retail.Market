using ITS.Retail.Api.Controllers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;


namespace ITS.Retail.Api.Providers
{
    public class WrmODataControllerSelector : DefaultHttpControllerSelector, IDisposable
    {
        private HttpConfiguration Configuration { get; set; }

        readonly List<Type> _types;
        public WrmODataControllerSelector(HttpConfiguration configuration, List<Type> types) : base(configuration)
        {
            this._types = types;
            this.Configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
        {
            var controllerName = base.GetControllerName(request);
            if (controllerName == "ODataMetadata")
            {
                return base.SelectController(request);
            }

            try
            {
                Type valueType = _types.FirstOrDefault(x => string.Compare(x.Name, controllerName, true) == 0);
                if (valueType != null)
                {
                    Type controllerType = typeof(BaseODataController<>).MakeGenericType(valueType);
                    return new HttpControllerDescriptor(Configuration, controllerName, controllerType);
                }

            }
            catch (Exception ex)
            {

            }
            return base.SelectController(request);
        }


        public void Dispose()
        {

        }
    }
}