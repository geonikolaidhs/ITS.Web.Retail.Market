using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;



namespace ITS.Retail.Api.Controllers
{
    /// <summary>
    /// A wrapper of ODataController to support Custom Authorization
    /// </summary>
    public class CustomODataController : ODataController
    {
        internal virtual void AfterAuthorization()
        {

        }
    }
}
