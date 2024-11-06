using DevExpress.Xpo;
using ITS.Retail.Api.Controllers;
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
    public class ItemImageController : BaseODataController<ItemImage>
    {
        public ItemImageController (IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /*
        /// <summary>
        /// Returns ItemImage from specific Item Oid ( guid ) 
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        public IQueryable<ItemImageViewModel> GetOwnerDocumentSequence(Guid Item )
        {
            var Ws = db.Query<ItemImage>().Where(x => x.ItemOid == Item)
                                        .Select(s => new ItemImageViewModel(s));
            return Ws;
        }*/

    }
}
