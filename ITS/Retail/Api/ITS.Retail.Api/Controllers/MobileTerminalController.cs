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
    public class MobileTerminalController : BaseODataController<MobileTerminal>
    {
        public MobileTerminalController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule)
        {
        }


        ///// <summary>
        ///// Retruns all mobile terminal of specific store
        ///// </summary>
        ///// <param name="Store"></param>
        ///// <returns></returns>
        //public IQueryable<MobileTerminalViewModel> GetMobileTerminalsByStore(Guid Store)
        //{
        //    var Ws = db.Query<MobileTerminal>().Where(x => x.Store.Oid == Store)
        //                                .Select(s => new MobileTerminalViewModel(s));
        //    return Ws;
        //}

        ///// <summary>
        ///// Return mobile terminal with spcific IP Address
        ///// </summary>
        ///// <param name="IPAddress"></param>
        ///// <returns></returns>
        //public IQueryable<MobileTerminalViewModel> GetMobileTerminalByIP(String IPAddress)
        //{
        //    var Ws = db.Query<MobileTerminal>().Where(x => x.IPAddress == IPAddress)
        //                                .Select(s => new MobileTerminalViewModel(s));
        //    return Ws;
        //}

        ///// <summary>
        ///// Return mobile terminal with specific name ( exact search )
        ///// </summary>
        ///// <param name="TerminalName"></param>
        ///// <returns></returns>
        //public IQueryable<MobileTerminalViewModel> GetMobileTerminalByName(String TerminalName)
        //{
        //    var Ws = db.Query<MobileTerminal>().Where(x => x.Name == TerminalName )
        //                                .Select(s => new MobileTerminalViewModel(s));
        //    return Ws;
        //}

        ///// <summary>
        ///// Returns mobile terminal with specific TerminalID
        ///// </summary>
        ///// <param name="TerminalID"></param>
        ///// <returns></returns>
        //public IQueryable<MobileTerminalViewModel> GetMobileTerminalByID(int TerminalID)
        //{
        //    var Ws = db.Query<MobileTerminal>().Where(x => x.ID == TerminalID)
        //                                .Select(s => new MobileTerminalViewModel(s));
        //    return Ws;
        //}

        ///// <summary>
        ///// Returns mobile terminal with specific store &&amp; ID ( guid, int ) 
        ///// </summary>
        ///// <param name="Store"></param>
        ///// <param name="TerminalID"></param>
        ///// <returns></returns>
        //public IQueryable<MobileTerminalViewModel> GetMobileTerminalByStoreAndID(Guid Store, int TerminalID)
        //{
        //    var Ws = db.Query<MobileTerminal>().Where(x => x.Store.Oid==Store && x.ID == TerminalID)
        //                                .Select(s => new MobileTerminalViewModel(s));
        //    return Ws;
        //}

    }
}
