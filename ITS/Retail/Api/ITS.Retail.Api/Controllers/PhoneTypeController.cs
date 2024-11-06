using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using ITS.Retail.WRM.Kernel.Interface;

namespace ITS.Retail.Api.Controllers
{
    public class PhoneTypeController : BaseODataController<PhoneType>
    {
        public PhoneTypeController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /*
         * 
                private UnitOfWork db = XpoHelper.GetNewUnitOfWork();
                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        db.Dispose();
                    }
                    base.Dispose(disposing);
                }

                public IQueryable<PhoneTypeViewModel> GetPhoneTypes()
                {
                    var PhoneTypes = from b in db.Query<PhoneType>()
                                     select new PhoneTypeViewModel(b);
                    return PhoneTypes;
                }

                [ResponseType(typeof(PhoneTypeViewModel))]
                public async Task<IHttpActionResult> GetPhoneType(Guid id)
                {
                    var phoneType = db.GetObjectByKey<PhoneType>(id);
                    if (phoneType == null)
                    {
                        return NotFound();
                    }

                    return Ok(new PhoneTypeViewModel(phoneType));
                }


                // PUT: api/PhoneTypes/5
                [ResponseType(typeof(void))]
                public async Task<IHttpActionResult> PutPhoneType(Guid id, PhoneTypeViewModel phoneType)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != phoneType.Oid)
                    {
                        return BadRequest();
                    }


                    try
                    {
                        PhoneType ptype = db.GetObjectByKey<PhoneType>(id);
                        phoneType.Persist(ptype);
                        db.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        if (!PhoneTypeExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return StatusCode(HttpStatusCode.NoContent);
                }

                // POST: api/PhoneTypes
                [ResponseType(typeof(PhoneTypeViewModel))]
                public async Task<IHttpActionResult> PostPhoneType(PhoneTypeViewModel phoneType)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    PhoneType pt;

                    try
                    {
                        pt = new PhoneType(db);
                        phoneType.Persist(pt);
                        db.CommitChanges();
                    }
                    catch (Exception ex)
                    {

                        throw;

                    }
                    PhoneTypeViewModel phoneTypeRetrived = new PhoneTypeViewModel(pt);
                    return CreatedAtRoute("DefaultApi", new { id = phoneType.Oid }, phoneTypeRetrived);
                }

                // DELETE: api/PhoneTypes/5
                [ResponseType(typeof(PhoneTypeViewModel))]
                public async Task<IHttpActionResult> DeletePhoneType(Guid id)
                {
                    PhoneType PhoneType = db.GetObjectByKey<PhoneType>(id);
                    if (PhoneType == null)
                    {
                        return NotFound();
                    }

                    PhoneTypeViewModel phoneTypeViewModel = new PhoneTypeViewModel(PhoneType);
                    PhoneType.Delete();
                    db.CommitChanges();
                    return Ok(phoneTypeViewModel);
                }

                private bool PhoneTypeExists(Guid id)
                {
                    return db.Query<PhoneType>().Count(e => e.Oid == id) > 0;
                }
                */


    }


}

