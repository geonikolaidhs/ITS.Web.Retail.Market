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
    public class MinistryDocumentTypeController : BaseODataController< MinistryDocumentType>
    {
        public MinistryDocumentTypeController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule)
        {
        }

        ///// <summary>
        ///// Get MinistryDocumentType searching my ministry code  ( string ) 
        ///// exact search
        ///// </summary>
        ///// <param name="MinistryCode"></param>
        ///// <returns></returns>
        //public IQueryable<MinistryDocumentTypeViewModel> GetMinistryCode(String MinistryCode )
        //{
        //    var Ws = db.Query<MinistryDocumentType>().Where(x => x.Code == MinistryCode)
        //                                .Select(s => new MinistryDocumentTypeViewModel(s));
        //    return Ws;
        //}

    }
}
