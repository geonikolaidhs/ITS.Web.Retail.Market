using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ITS.Retail.Api.Controllers
{
    public class BaseApiController<T, W> : ApiController where T : BasePersistableViewModel
                                                        where W : BasicObj
    {
        protected UnitOfWork db
        {
            get;
            private set;
        }

        public BaseApiController()
        {
            db = XpoHelper.GetNewUnitOfWork();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// GetAll Return all records
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll()
        {
            var Ws = from b in db.Query<W>().ToList().AsQueryable()
                     select (T)Activator.CreateInstance(typeof(T),  b );
            
//            var Ws = db.Query<W>().Select(s=> new )

            return Ws;
        }


        /// <summary>
        /// Get specific object with Oid ( guid )
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IHttpActionResult> Get(Guid id)
        {
            var W = db.GetObjectByKey<W>(id);
            if (W == null)
            {
                return NotFound();
            }

            return Ok((T)Activator.CreateInstance(typeof(T), new object[] { W }));
        }


        /// <summary>
        /// Put specific object with key Oid  ( guid )
        /// </summary>
        /// <param name="id"></param>
        /// <param name="W"></param>
        /// <returns></returns>
        // PUT: api/Ws/5
        [ResponseType(typeof(void))]
        public virtual async Task<IHttpActionResult> Put(Guid id, T W)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != W.Oid)
            {
                return BadRequest();
            }
            try
            {
                W ptype = db.GetObjectByKey<W>(id);
                W.Persist(ptype);
                db.CommitChanges();
            }
            catch (Exception ex)
            {
                if (!Exists(id))
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

        // POST: api/Ws
        public virtual async Task<IHttpActionResult> Post(T vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            W pt;

            try
            {
                pt = (W)Activator.CreateInstance(typeof(W), new object[] { db });
                vm.Persist(pt);
                db.CommitChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            T WRetrived = (T)Activator.CreateInstance(typeof(T), new object[] { pt }); ;
            return CreatedAtRoute("DefaultApi", new { id = vm.Oid }, WRetrived);
        }

        // DELETE: api/Ws/5
        //        [ResponseType(typeof(T))]
        public virtual async Task<IHttpActionResult> Delete(Guid id)
        {
            W deletedObj = db.GetObjectByKey<W>(id);
            if (deletedObj == null)
            {
                return NotFound();
            }

            T vm = (T)Activator.CreateInstance(typeof(T), new object[] { deletedObj });
            deletedObj.Delete();
            db.CommitChanges();
            return Ok(vm);
        }

        private bool Exists(Guid id)
        {
            return db.Query<W>().Count(e => e.Oid == id) > 0;
        }



    }
}
