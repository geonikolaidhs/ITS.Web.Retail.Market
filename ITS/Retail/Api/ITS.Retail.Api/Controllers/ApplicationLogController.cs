using ITS.Retail.Model;
using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ITS.Retail.WRM.Kernel.Interface;

namespace ITS.Retail.Api.Controllers
{
    public class ApplicationLogController : BaseODataController<ApplicationLog>       
    {
        public ApplicationLogController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}
