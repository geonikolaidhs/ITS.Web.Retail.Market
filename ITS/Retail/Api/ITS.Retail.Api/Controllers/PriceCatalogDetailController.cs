﻿using ITS.Retail.Common.ViewModel;
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
    public class PriceCatalogDetailController : BaseODataController<PriceCatalogDetail>
    {
        public PriceCatalogDetailController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}