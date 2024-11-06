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
    public class ItemCategoryController : BaseODataController< ItemCategory>
    {
        public ItemCategoryController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /*        public override IQueryable<ItemCategoryViewModel> GetAll()
                {
                    var Ws = from b in db.Query<ItemCategory>().Where(x=> x.ParentOid == Guid.Empty || x.ParentOid == null).OrderBy(x => x.Code).ToList().AsQueryable()
                             select new ItemCategoryViewModel(b);
                    return Ws;
                }
                */
    }
    
}
