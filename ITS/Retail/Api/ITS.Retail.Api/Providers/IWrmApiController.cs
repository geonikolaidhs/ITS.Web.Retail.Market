using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ITS.Retail.Api.Providers
{
    public interface IWrmApiController<ClassModel> : IHttpController
    {
        Task<IHttpActionResult> Get(ODataQueryOptions<ClassModel> queryOptions);
        Task<IHttpActionResult> Get(Guid key, ODataQueryOptions<ClassModel> queryOptions);
        IHttpActionResult Delete(Guid key, ODataQueryOptions<ClassModel> queryOptions);
        Task<IHttpActionResult> Post(ClassModel requestModel);

        Task<IHttpActionResult> Put(Guid key, ClassModel postedObject);


    }
}