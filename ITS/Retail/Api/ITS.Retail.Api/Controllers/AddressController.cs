using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Api.Providers;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Validations;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace ITS.Retail.Api.Controllers
{

    public class AddressController : BaseODataController<Address>
    {
        public AddressController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }

        public override async Task<IHttpActionResult> Post([ModelBinder(BinderType = typeof(WrmModelBinder))]Address requestModel)
        {

            using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
            {
                try
                {
                    if (wrmUserDbModule.Access<Address>(null, this.CurrentUser).HasFlag(ePermition.Insert) == false)
                    {
                        return Unauthorized();
                    }
                    await Task.Delay(1);
                    Address newAddress = wrmUserDbModule.CreateObject<Address>(this.CurrentUser);
                    newAddress.GetData(requestModel);
                    if (newAddress is IOwner)
                    {
                        IOwner ownerObj = requestModel as IOwner;
                        wrmUserDbModule.AssignOwner(newAddress, this.CurrentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
                    }
                    if (newAddress.Trader == null)
                    {
                        throw new Exception("Address Without Trader");
                    }
                    CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("Trader.Oid", newAddress.Trader.Oid, BinaryOperatorType.Equal));
                    Customer customer = uow.FindObject<Customer>(crit);
                    if (customer != null)
                    {
                        if (customer.DefaultAddress == null)
                        {
                            customer.DefaultAddress = newAddress;
                        }
                        else
                        {
                            customer.DefaultAddress = customer.DefaultAddress;
                        }
                    }
                    uow.CommitChanges();
                    return Created<Address>(newAddress);
                }
                catch (Exception ex)
                {
                    WebApiConfig.ApiLogger.Log(ex, " ,AddressController , Post", KernelLogLevel.Error);
                    uow.RollbackTransaction();
                    return BadRequest(ex.Message);
                }
            }
        }
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, [ModelBinder(BinderType = typeof(WrmModelBinder))] Address requestModel)
        {

            using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
            {
                try
                {
                    if (wrmUserDbModule.Access<Address>(null, this.CurrentUser).HasFlag(ePermition.Update) == false)
                    {
                        return Unauthorized();
                    }
                    Address dbAddress = uow.GetObjectByKey<Address>(requestModel.Oid);
                    if (dbAddress == null)
                    {
                        throw new Exception("Address Not Found");
                    }
                    Trader dbTrader = uow.GetObjectByKey<Trader>(requestModel.Trader.Oid);
                    if (requestModel.Trader == null)
                    {
                        throw new Exception("Trader is NULL");
                    }
                    Phone defaultPhone = dbAddress.DefaultPhone;
                    Guid? defaultPhoneOid = dbAddress.DefaultPhoneOid;
                    dbAddress.GetData(requestModel);
                    dbAddress.DefaultPhone = defaultPhone;
                    dbAddress.DefaultPhoneOid = defaultPhoneOid;
                    uow.CommitChanges();
                    return Updated<Address>(dbAddress);
                }
                catch (Exception ex)
                {
                    WebApiConfig.ApiLogger.Log(ex, ex.Message + " ," + "AddressController ,Put )", KernelLogLevel.Error);
                    return BadRequest(ex.Message);
                }
            }
        }


    }
}
