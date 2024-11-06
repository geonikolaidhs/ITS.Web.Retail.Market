using ITS.Retail.Api.FileStreaming;
using ITS.Retail.Api.Providers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;


using ITS.Retail.Api.Providers;
using ITS.Retail.Model;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;

namespace ITS.Retail.Api.Controllers
{
    public class SimpleFilesController<ClassModel> : CustomODataController//, IWrmApiController<ClassModel>
                                                        where ClassModel : BasicObj
    {
        public IFileProvider FileProvider { get; set; }

        protected IWRMUserDbModule wrmUserDbModule
        {
            get;
            private set;
        }

        /// <summary>
        /// The Currently Loggedin User
        /// </summary>
        protected IUser CurrentUser { get; set; }

        public SimpleFilesController()
        {
            FileProvider = new FileProvider();
        }


        [EnableQuery]
        public async Task<IHttpActionResult> Get(string fileName, ODataQueryOptions<ClassModel> queryOptions)
        {
            if (!FileProvider.Exists(fileName))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (wrmUserDbModule.Access(FileProvider.Exists(fileName), this.CurrentUser).HasFlag(ePermition.Visible))
            {
                FileStream fileStream = FileProvider.Open(fileName);
                var response = new HttpResponseMessage();
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentDisposition
                    = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                response.Content.Headers.ContentType
                    = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentLength
                    = FileProvider.GetLength(fileName);

                return Ok(response);
            }
            return Unauthorized();

        }

        /* [EnableQuery]
         public async Task<IHttpActionResult> Get([FromODataUri] Guid key, ODataQueryOptions<ClassModel> queryOptions)
         {
             ClassModel foundObject = wrmUserDbModule.GetObjectByKey<ClassModel>(key);
             if (foundObject == null || foundObject.IsDeleted)
             {
                 return NotFound();
             }
             if (wrmUserDbModule.Access(foundObject, this.CurrentUser).HasFlag(ePermition.Visible))
             {
                 return Ok<ClassModel>(foundObject);
             }
             return Unauthorized();
         }*/

        public Task<IHttpActionResult> Get(ODataQueryOptions<ClassModel> queryOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete(Guid key, ODataQueryOptions<ClassModel> queryOptions)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpActionResult> Post(ClassModel requestModel)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpActionResult> Put(Guid key, ClassModel postedObject)
        {
            throw new NotImplementedException();
        }
    }
}
