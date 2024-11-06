using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;

namespace ITS.Retail.Api.Controllers
{
    [EnableQuery(MaxExpansionDepth = 5)]
    public class ItemBarcodeController : BaseODataController<ItemBarcode>
    {
        public ItemBarcodeController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}
