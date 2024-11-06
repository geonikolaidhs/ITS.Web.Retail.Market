using DevExpress.Xpo;
using ITS.Retail.Api.Providers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Validations;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace ITS.Retail.Api.Controllers
{
    public class CustomerController : BaseODataController<Customer>
    {
        public CustomerController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }


        public override async Task<IHttpActionResult> Post([ModelBinder(BinderType = typeof(WrmModelBinder))]Customer requestModel)
        {
            using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
            {
                if (wrmUserDbModule.Access<Customer>(null, this.CurrentUser).HasFlag(ePermition.Insert) == false)
                {
                    return Unauthorized();
                }
                Customer dbObject = uow.GetObjectByKey<Customer>(requestModel.Oid);
                if (dbObject != null)
                {
                    return BadRequest("Duplicate Primary Key");
                }
                try
                {
                    dbObject = CustomerCRUDActions.CreateCustomer(uow, requestModel, this.CurrentUser, wrmUserDbModule);
                    return Created(dbObject);
                }
                catch (Exception ex)
                {
                    WebApiConfig.ApiLogger.Log(ex, ex.Message + " ," + "CustomerController ,Post )", KernelLogLevel.Error);
                    return BadRequest(ex.Message);
                }
            }

        }

        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, [ModelBinder(BinderType = typeof(WrmModelBinder))] Customer requestModel)
        {

            using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
            {
                try
                {
                    if (wrmUserDbModule.Access<Customer>(null, this.CurrentUser).HasFlag(ePermition.Update) == false)
                    {
                        return Unauthorized();
                    }
                    Customer dbCustomer = uow.GetObjectByKey<Customer>(requestModel.Oid);
                    if (dbCustomer == null)
                    {
                        throw new Exception("Customer Not Found");
                    }
                    if (requestModel.Trader == null)
                    {
                        throw new Exception("Customer Not Found");
                    }
                    Trader dbTrader = uow.GetObjectByKey<Trader>(requestModel.Trader.Oid);
                    if (dbTrader == null)
                    {
                        throw new Exception("Trader Not Found");
                    }
                    Address defaultAddress = dbCustomer.DefaultAddress;
                    dbCustomer.GetData(requestModel);
                    dbTrader.GetData(requestModel.Trader);
                    dbCustomer.DefaultAddress = defaultAddress;
                    uow.CommitChanges();
                    return Updated<Customer>(dbCustomer);
                }
                catch (Exception ex)
                {
                    WebApiConfig.ApiLogger.Log(ex, ex.Message + " ," + "CustomerController ,Put )", KernelLogLevel.Error);
                    return BadRequest(ex.Message);
                }
            }
        }


    }
}
