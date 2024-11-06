using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
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
    public class BarcodeController : BaseODataController< Barcode>
    {
        public BarcodeController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /*public IQueryable<BarcodeViewModel> GetCode(string Code)
        {
            var Ws = db.Query<Barcode>().Where(x=>x.Code==Code)
                                        .Select(s=> new BarcodeViewModel(s));
            return Ws;
        }*/

    }
}
