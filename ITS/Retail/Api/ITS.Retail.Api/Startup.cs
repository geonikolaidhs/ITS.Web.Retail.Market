using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection;
using ITS.Retail.Api.Providers;
using System.Web.Mvc;
using System.Diagnostics;

[assembly: OwinStartup(typeof(ITS.Retail.Api.Startup))]

namespace ITS.Retail.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);

        }

    }
}
