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
    public class OwnerCategoriesController : BaseODataController< OwnerCategories>
    {
        public OwnerCategoriesController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule)
        {
        }

        ///// <summary>
        ///// Get all OwnerCategories with specific Owner ( guid )
        ///// </summary>
        ///// <param name="Owner"></param>
        ///// <returns></returns>
        //public IQueryable<OwnerCategoriesViewModel> GetByOwner(Guid Owner)
        //{
        //    var Ws = db.Query<OwnerCategories>().Where(x => x.Owner.Oid == Owner)
        //                                .Select(s => new OwnerCategoriesViewModel(s));
        //    return Ws;
        //}


    }
}
