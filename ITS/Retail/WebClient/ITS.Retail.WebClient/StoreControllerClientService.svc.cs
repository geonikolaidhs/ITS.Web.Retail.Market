using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ITS.Retail.WebClient
{
#if !_RETAIL_WEBCLIENT
    public class StoreControllerClientService : CachedDataStoreService
    {
        public StoreControllerClientService()
            : base(XpoHelper.CacheRoot)
        {

        }

    }
#endif
}
