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
using ITS.Retail.WRM.Kernel;

namespace ITS.Retail.Api.Controllers
{
    /// <summary>
    /// Base Odata API Controller
    /// </summary>    
    /// <typeparam name="ClassModel"></typeparam>
    [CustomAuthorize]
    public class BaseODataController<ClassModel> : CustomODataController, IWrmApiController<ClassModel>
                                                        where ClassModel : BasicObj
    {
        /// <summary>
        /// Access to Database
        /// </summary>
        protected WRMUserDbModule wrmUserDbModule
        {
            get;
            private set;
        }

        /// <summary>
        /// The Currently Loggedin User
        /// </summary>
        protected IUser CurrentUser { get; set; }


        /// <summary>
        /// Creates a new instance of Base OData Controller
        /// </summary>
        /// <param name="wrmDbModule">Database Access Layer</param>
        public BaseODataController(IWRMUserDbModule wrmDbModule)
        {
            WRMDbModule db = new WRMDbModule();
            WRMUserModule um = new WRMUserModule(db);
            this.wrmUserDbModule = new WRMUserDbModule(db, um);
            //Claim claim = (this.User as System.Security.Claims.ClaimsPrincipal).Claims.FirstOrDefault(x => x.Type == "AspNet.Identity.SecurityStamp");
            //CurrentUser = db.Query<User>().FirstOrDefault(x => x.AuthToken == claim.Value && x.UserName == this.User.Identity.Name);

        }



        internal override void AfterAuthorization()
        {
            base.AfterAuthorization();
            Claim claim = (this.User as System.Security.Claims.ClaimsPrincipal).Claims.FirstOrDefault(x => x.Type == "AspNet.Identity.SecurityStamp");
            CurrentUser = wrmUserDbModule.Query<User>().FirstOrDefault(x => x.AuthToken == claim.Value && x.UserName == this.User.Identity.Name);
        }

        /// <summary>
        /// Disposes IDisposable objects used in Controller
        /// </summary>
        /// <param name="disposing">true if should dispose in this call</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && wrmUserDbModule is IDisposable)
            {
                (wrmUserDbModule as IDisposable).Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Generic Get. User permissions are applied here. External filters, ordering, top, skip etc are applied at the end.
        /// </summary>
        /// <param name="queryOptions">Contains the parameters of the external filters, ordering, top, skip etc</param>
        /// <returns></returns>
        [EnableQuery]
        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<ClassModel> queryOptions)
        {
            try
            {
                IQueryable<ClassModel> wrmUserDbModuleQueryable = wrmUserDbModule.Query<ClassModel>(this.CurrentUser);
                return Ok<IQueryable<ClassModel>>(wrmUserDbModuleQueryable);
            }
            catch (Exception e)
            {
                throw;
            }
            //finally
            //{
            //    wrmUserDbModule.Dispose();
            //}
        }

        /// <summary>
        /// Returns the requested by key object
        /// </summary>
        /// <param name="key">the key of the object to return</param>
        /// <param name="queryOptions">extra query parameters (e.g. expand)</param>
        /// <returns></returns>
        [EnableQuery]
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
        }

        /// <summary>
        /// Put (update) action for an object (by key). The provided properties are replaced, while the rest remain unaffected
        /// </summary>
        /// <param name="key">The key of the object to retunr</param>
        /// <param name="postedObject">The posted object in the request</param>
        /// <returns></returns>
        public virtual async Task<IHttpActionResult> Put([FromODataUri] Guid key, [ModelBinder(BinderType = typeof(WrmModelBinder))] ClassModel postedObject)
        {
            if (postedObject == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != postedObject.Oid)
            {
                return BadRequest();
            }

            try
            {
                ClassModel ptype = wrmUserDbModule.GetObjectByKey<ClassModel>(this.CurrentUser, key);
                if (ptype == null)
                {
                    return NotFound();
                }
                if (wrmUserDbModule.Access<ClassModel>(ptype, this.CurrentUser).HasFlag(ePermition.Update))
                {
                    ptype.GetData(postedObject);
                    ptype.UpdatedBy = wrmUserDbModule.GetObjectByKey<User>(this.CurrentUser.Oid);
                    if (postedObject is IOwner)
                    {
                        IOwner ownerObj = postedObject as IOwner;
                        wrmUserDbModule.AssignOwner(ptype, this.CurrentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
                    }
                    wrmUserDbModule.CommitChanges();
                    return Updated(ptype);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                WebApiConfig.ApiLogger.Log(ex, " ,BaseODataController, Put", KernelLogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Post (insert) action for an object (by key). The provided properties are set, while the rest take the default value
        /// </summary>
        /// <param name="requestModel">The posted object in the request</param>
        /// <returns></returns>
        public virtual async Task<IHttpActionResult> Post([ModelBinder(BinderType = typeof(WrmModelBinder))]ClassModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (wrmUserDbModule.Access<ClassModel>(null, this.CurrentUser).HasFlag(ePermition.Insert) == false)
            {
                return Unauthorized();
            }

            ClassModel dbObject = wrmUserDbModule.GetObjectByKey<ClassModel>(requestModel.Oid);
            if (dbObject != null)
            {
                return BadRequest("Duplicate Primary Key");
            }
            try
            {
                dbObject = wrmUserDbModule.CreateObject<ClassModel>(this.CurrentUser);
                // TODO Owner handling, createdon, updatedon, createdby, updatedby
                dbObject.GetData(requestModel);
                dbObject.GetDataFromDetailObjects(requestModel, dbObject.IgnorePropertiesOnJsonDeserialise());
                dbObject.UpdatedBy = wrmUserDbModule.GetObjectByKey<User>(this.CurrentUser.Oid);
                if (requestModel is IOwner)
                {
                    IOwner ownerObj = requestModel as IOwner;
                    wrmUserDbModule.AssignOwner(dbObject, this.CurrentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
                }
                dbObject.Save();
                wrmUserDbModule.CommitChanges();
                return Created(dbObject);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete  action for an object (by key).
        /// </summary>
        /// <param name="key">The key of the object to delete</param>
        /// <param name="queryOptions">extra query parameters</param>
        /// <returns></returns>
        [EnableQuery]
        public IHttpActionResult Delete([FromODataUri] Guid key, ODataQueryOptions<ClassModel> queryOptions)
        {
            ClassModel deletedObj = wrmUserDbModule.GetObjectByKey<ClassModel>(key);
            if (deletedObj == null)
            {
                return NotFound();
            }
            else if (wrmUserDbModule.Access(deletedObj, this.CurrentUser).HasFlag(ePermition.Delete))
            {
                try
                {
                    deletedObj.Delete();
                    wrmUserDbModule.CommitChanges();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Unauthorized();
        }


    }
}
