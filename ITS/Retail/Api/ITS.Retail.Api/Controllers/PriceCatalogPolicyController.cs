using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITS.Retail.Api.Controllers
{
    public class PriceCatalogPolicyController : BaseODataController<PriceCatalogPolicy>
    {
        public PriceCatalogPolicyController (IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}
