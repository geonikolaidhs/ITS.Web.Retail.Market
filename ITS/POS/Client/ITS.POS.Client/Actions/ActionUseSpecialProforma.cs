using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.POS.Client.Actions
{
    public class ActionUseSpecialProforma : Action
    {
        public ActionUseSpecialProforma(IPosKernel kernel) : base(kernel)
        {

        }
        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();

            if (AppContext.GetMachineStatus() == eMachineStatus.SALE
             || AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT
               )
            {
                if (AppContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(false), dontCheckPermissions: true);
                }
                AppContext.CurrentDocument.DocumentType = config.SpecialProformaDocumentTypeOid;
                AppContext.CurrentDocument.DocumentSeries = config.SpecialProformaDocumentSeriesOid;
                StoreDocumentSeriesType specialProformaStoreDocumentSeriesTypes = sessionManager.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                                                                                                        new BinaryOperator("DocumentType", config.SpecialProformaDocumentTypeOid),
                                                                                                        new BinaryOperator("DocumentSeries", config.SpecialProformaDocumentSeriesOid)
                                                                                                        ));
                if (specialProformaStoreDocumentSeriesTypes == null)
                {
                    throw new POSUserVisibleException(POSClientResources.PLEASE_REVISE_SPECIAL_PROFORMA_SETTINGS);
                }

                if (specialProformaStoreDocumentSeriesTypes.DefaultCustomer != Guid.Empty
                    && (AppContext.CurrentCustomer == null ||
                            (AppContext.CurrentCustomer != null
                             && AppContext.CurrentCustomer.Oid == config.DefaultCustomerOid
                            )
                       )
                   )
                {
                    Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(specialProformaStoreDocumentSeriesTypes.DefaultCustomer);
                    if (defaultCustomer != null)
                    {
                        Address defaultAddress = sessionManager.GetObjectByKey<Address>(defaultCustomer.Oid);
                        actionManager.GetAction(eActions.ADD_CUSTOMER_INTERNAL).Execute(new ActionAddCustomerInternalParams(defaultCustomer, defaultCustomer.Code, defaultAddress));
                    }
                }
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT; }
        }


        public override eActions ActionCode
        {
            get { return eActions.USE_SPECIAL_PROFORMA; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}