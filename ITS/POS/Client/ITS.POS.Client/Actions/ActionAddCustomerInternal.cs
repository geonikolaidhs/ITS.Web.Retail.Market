using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using Newtonsoft.Json;
using ITS.Retail.Platform;
using System.Collections.Generic;
using ITS.POS.Client.Forms;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations.ViewModel;
using System;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Adds the given customer to the current document. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionAddCustomerInternal : Action
    {
        public ActionAddCustomerInternal(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.GetMachineStatus() == eMachineStatus.SALE)
            {
                actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(true), dontCheckPermissions: true);
            }
            DocumentHeader header = appContext.CurrentDocument;
            Customer customer = (parameters as ActionAddCustomerInternalParams).Customer;
            Address address = (parameters as ActionAddCustomerInternalParams).Address;
            string customerLookupCode = (parameters as ActionAddCustomerInternalParams).CustomerLookupCode;

            if (customer != null)
            {
                if ((parameters as ActionAddCustomerInternalParams).CustomerViewModel == null || !(parameters as ActionAddCustomerInternalParams).CustomerViewModel.IsNew)
                {
                    if (documentService.CheckIfCustomerIsValidForDocumentType(header.DocumentType, customer.Oid) == false)
                    {
                        throw new POSUserVisibleException(POSClientResources.CUSTOMER_NOT_ALLOWED_TO_THIS_DOCUMENT_TYPE);
                    }
                }

                if (address != null)
                {
                    header.BillingAddress = address.Oid;
                    header.AddressProfession = address.Profession;
                    header.DeliveryAddress = address.Description;
                }
                else
                {
                    InsertedCustomerViewModel customerViewModel = (parameters as ActionAddCustomerInternalParams).CustomerViewModel;
                    if (customerViewModel != null && customerViewModel.CustomerOid == customer.Oid)
                    {
                        header.DenormalizedCustomer = JsonConvert.SerializeObject(customerViewModel, PlatformConstants.JSON_SERIALIZER_SETTINGS);
                        header.DenormalisedAddress = customerViewModel.AddressOid;
                        header.DeliveryAddress = customerViewModel.Street + ", " + customerViewModel.PostalCode + ", " + customerViewModel.City;
                    }
                }

                header.CustomerNotFound = false;
                header.Remarks = null;
            }
            else
            {
                customer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
                InsertedCustomerViewModel customerViewModel = (parameters as ActionAddCustomerInternalParams).CustomerViewModel;
                if (customerViewModel != null)
                {
                    header.DenormalizedCustomer = JsonConvert.SerializeObject(customerViewModel, PlatformConstants.JSON_SERIALIZER_SETTINGS);
                    header.CustomerLookUpTaxCode = customerViewModel.TaxCode;
                    header.DenormalisedAddress = customerViewModel.AddressOid;
                    header.DeliveryAddress = customerViewModel.Street + ", " + customerViewModel.PostalCode + ", " + customerViewModel.City;
                }
                header.CustomerNotFound = true;
                header.Remarks = POSClientResources.CUSTOMER + ": " + customerLookupCode;
            }

            //PriceCatalog pc = customerService.GetPriceCatalog(customer.Oid, config.CurrentStoreOid);
            PriceCatalogPolicy priceCatalogPolicy = customerService.GetPriceCatalogPolicy(customer.Oid, config.CurrentStoreOid);
            if (priceCatalogPolicy == null)
            {
                throw new POSException(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
            }


            appContext.CurrentCustomer = customer;
            header.Customer = customer.Oid;
            header.CustomerName = customer.CompanyName;
            //header.PriceCatalog = pc.Oid;
            header.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            header.CustomerLookupCode = customerLookupCode;
            documentService.RecalculateDocumentCosts(header, true);
            sessionManager.FillDenormalizedFields(appContext.CurrentDocument);
            try
            {
                Trader trader = sessionManager.GetObjectByKey<Trader>(customer.Trader);
                if (string.IsNullOrEmpty(trader.GDPRProtocolNumber))
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.CUSTOMER_HAS_NOT_SIGN_THE_GDPR_CONTRACT));
                else
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(""));
            }
            catch { }

            appContext.CurrentDocument.Save();
            sessionManager.CommitTransactionsChanges();
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_CUSTOMER_INTERNAL; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
