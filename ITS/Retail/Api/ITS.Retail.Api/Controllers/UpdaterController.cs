using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITS.Retail.Api.Authentication;
using System.Net;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Api.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http;
using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using ITS.Retail.Api.Helpers;

namespace ITS.Retail.Api.Controllers
{

    [RoutePrefix("api/Updater")]
    [EnableQuery(MaxExpansionDepth = 4)]
    [CustomAuthorize]
    public class UpdaterController : ApiController
    {
        UpdaterServiceHelper UpdaterServiceHelper = null;
        SyncCriteriaHelper SyncCriteriaHelper = null;
        public UpdaterController(IWRMUserDbModule wrmDbModule) : base()
        {
            UpdaterServiceHelper = new UpdaterServiceHelper();
            SyncCriteriaHelper = new SyncCriteriaHelper();
            this.wrmUserDbModule = wrmDbModule;
        }
        /// <summary>
        /// Access to Database
        /// </summary>
        protected IWRMUserDbModule wrmUserDbModule
        {
            get;
            private set;
        }

        /// <summary>
        /// The Currently Loggedin User
        /// </summary>
        protected IUser CurrentUser { get; set; }



        internal void AfterAuthorization()
        {
            Claim claim = (this.User as System.Security.Claims.ClaimsPrincipal).Claims.FirstOrDefault(x => x.Type == "AspNet.Identity.SecurityStamp");
            CurrentUser = wrmUserDbModule.Query<User>().FirstOrDefault(x => x.AuthToken == claim.Value && x.UserName == this.User.Identity.Name);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.Route("GetMaxVersion/{ClassName}")]
        public async Task<IHttpActionResult> GetMaxVersion(String ClassName)
        {
            long version = DateTime.MinValue.Ticks;
            try
            {
                Type[] argsType = UpdaterServiceHelper.GetGenericMethodArgsRetailModel(ClassName);
                object[] parameters = new object[] { };
                version = UpdaterServiceHelper.InvokeGeneric<long, UpdaterServiceHelper>("GetMaxTableVersion", parameters, argsType, UpdaterServiceHelper);
            }
            catch (Exception ex)
            {
                WebApiConfig.ApiLogger.Log(ex, ex.Message + " ," + ClassName + " ,UpdaterController ,GetMaxVersion " + "(" + ClassName + ")", KernelLogLevel.Error);
            }

            return Ok<long>(version);

        }


    }
}
