using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace ITS.Retail.Api.Controllers
{
    [EnableQuery(MaxExpansionDepth = 4)]
    public class CompanyNewController : BaseODataController<CompanyNew>
    {
        public CompanyNewController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}
