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
    public class PhoneController : BaseODataController< Phone>
    {
        public PhoneController (IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        ///// <summary>
        ///// Search Phone by any number inside 
        ///// </summary>
        ///// <param name="AnyNumber"></param>
        ///// <returns></returns>
        //public IQueryable<PhoneViewModel> GetAnyNumber(string AnyNumber)
        //{
        //    var Ws = db.Query<Phone>().Where( x=> x.Number.Contains(AnyNumber) )
        //                                .Select(s => new PhoneViewModel(s));
        //    return Ws;
        //}
        ///// <summary>
        ///// Search Phone by Phone Number ( exact search )
        ///// </summary>
        ///// <param name="Number"></param>
        ///// <returns></returns>
        //public IQueryable<PhoneViewModel> GetNumber(string Number)
        //{
        //    var Ws = db.Query<Phone>().Where(x => x.Number == Number)
        //                                .Select(s => new PhoneViewModel(s));
        //    return Ws;
        //}


    }
}
    

