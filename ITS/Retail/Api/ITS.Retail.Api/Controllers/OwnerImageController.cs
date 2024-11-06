using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ITS.Retail.WRM.Kernel.Interface;

namespace ITS.Retail.Api.Controllers
{
    public class OwnerImageController : BaseODataController<OwnerImage>
    {
        public OwnerImageController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule)
        {
        }
    }
}
