using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Common.Interfaces;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace ITS.Retail.Api.Controllers
{
    public class DivisionController : BaseODataController<Division>
    {
        public DivisionController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }

    }
}
