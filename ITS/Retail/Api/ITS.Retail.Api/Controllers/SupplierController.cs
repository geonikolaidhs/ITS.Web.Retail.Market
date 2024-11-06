using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.Api.Controllers
{
    public class SupplierController : BaseODataController<SupplierNew>
    {
        public SupplierController (IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
    }
}