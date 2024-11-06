using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DevExpress.Xpo;
using ITS.Retail.WRM.Kernel.Interface;

namespace ITS.Retail.Api.Controllers
{
    public class ItemController : BaseODataController<Item>
    {
        public ItemController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }



    }

}
