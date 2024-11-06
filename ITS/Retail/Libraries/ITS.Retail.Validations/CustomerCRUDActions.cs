using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Validations
{
    public static class CustomerCRUDActions
    {
        public static Customer CreateCustomer(UnitOfWork uow, Customer requestModel, IUser CurrentUser, IWRMUserDbModule wrmUserDbModule)
        {

            Guid customerOid = requestModel.Oid;
            Trader dbTrader = CreateTrader(requestModel, CurrentUser, uow);
            //Customer dbCustomer = wrmUserDbModule.CreateObject<Customer>(CurrentUser);
            Customer dbCustomer = new Customer(uow);
            dbCustomer.GetData(requestModel);
            dbCustomer.Trader = dbTrader;
            dbCustomer.Oid = customerOid;
            if (requestModel is IOwner)
            {
                IOwner ownerObj = requestModel as IOwner;
                wrmUserDbModule.AssignOwner(dbCustomer, CurrentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
            }
            uow.CommitChanges();

            return dbCustomer;
        }

        private static Address CreateAddress(Customer requestModel, IUser currentUser, IWRMUserDbModule wrmUserDbModule)
        {
            Address newAddress = wrmUserDbModule.CreateObject<Address>(currentUser);
            newAddress.GetData(requestModel.DefaultAddress);
            if (requestModel.DefaultAddress is IOwner)
            {
                IOwner ownerObj = requestModel.DefaultAddress as IOwner;
                wrmUserDbModule.AssignOwner(newAddress, currentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
            }
            return newAddress;
        }

        private static void UpdateAddress(Trader requestModel, IUser currentUser, UnitOfWork uow)
        {
            IList<Address> existingAddressses = new XPCollection<Address>(CriteriaOperator.And(new BinaryOperator("Trader.Oid", requestModel.Oid))).ToList();
            //wrmUserDbModule.GetList<Address>(CriteriaOperator.And(new BinaryOperator("Trader.Oid", requestModel.Oid)));

            foreach (Address requestAddress in requestModel?.Addresses)
            {
                foreach (Address traderAddress in existingAddressses)
                {
                    if (requestAddress.Oid == traderAddress.Oid)
                    {
                        traderAddress.GetData(requestAddress);
                    }
                }
            }
            uow.CommitChanges();
        }

        private static Trader CreateTrader(Customer requestModel, IUser currentUser, UnitOfWork uow)
        {
            Trader newTrader = new Trader(uow);
            newTrader.GetData(requestModel.Trader);
            return newTrader;
        }

        public static void UpdateTrader(Customer requestModel, IUser currentUser, UnitOfWork uow, IWRMUserDbModule wrmUserDbModule)
        {
            if (requestModel.Trader != null && requestModel.Trader.Oid != Guid.Empty)
            {
                Trader ptype = uow.GetObjectByKey<Trader>(requestModel.Trader.Oid);
                if (ptype != null)
                {
                    if (requestModel.DefaultAddress != null)
                    {
                        Address dbAddress = uow.GetObjectByKey<Address>(requestModel.DefaultAddress.Oid);
                        if (dbAddress != null)
                        {
                            if (ptype.Addresses == null)
                            {
                                ptype.Addresses.Add(dbAddress);
                            }
                            else if (ptype.Addresses.IndexOf(dbAddress) == -1)
                            {
                                ptype.Addresses.Add(dbAddress);
                            }
                            uow.CommitChanges();
                        }
                    }
                    if (wrmUserDbModule.Access<Trader>(ptype, currentUser).HasFlag(ePermition.Update))
                    {
                        ptype.GetData(requestModel.Trader);
                        ptype.UpdatedBy = uow.GetObjectByKey<User>(currentUser.Oid);
                        if (requestModel is IOwner)
                        {
                            IOwner ownerObj = requestModel as IOwner;
                            wrmUserDbModule.AssignOwner(ptype, currentUser, ownerObj.Owner == null ? Guid.Empty : ownerObj.Owner.Oid);
                        }
                        UpdateAddress(requestModel.Trader, currentUser, uow);
                    }

                }
            }

        }
    }
}
