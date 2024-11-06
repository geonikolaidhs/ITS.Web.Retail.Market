using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using System;
using ITS.POS.Model.Master;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Changes the current document's type to the default type
    /// </summary>
    public class ActionUseDefaultDocumentType: Action
    {
        public ActionUseDefaultDocumentType(IPosKernel kernel)
            : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IDocumentService documentService = this.Kernel.GetModule<IDocumentService>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();

            if (AppContext.GetMachineStatus() == eMachineStatus.SALE || AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
			{
                if (AppContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(false), dontCheckPermissions: true);
                }
                AppContext.CurrentDocument.DocumentType = config.DefaultDocumentTypeOid;
                AppContext.CurrentDocument.DocumentSeries = config.DefaultDocumentSeriesOid;
                StoreDocumentSeriesType defaultStoreDocumentSeriesTypes = sessionManager.FindObject<StoreDocumentSeriesType>
                    (CriteriaOperator.And(new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                     new BinaryOperator("DocumentSeries", config.DefaultDocumentSeriesOid)));

                if (defaultStoreDocumentSeriesTypes.DefaultCustomer != Guid.Empty &&
                   (AppContext.CurrentCustomer == null ||
                   (AppContext.CurrentCustomer != null &&
                   AppContext.CurrentCustomer.Oid == config.DefaultCustomerOid)))
                {
                    Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(defaultStoreDocumentSeriesTypes.DefaultCustomer);
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
            get { return eMachineStatus.OPENDOCUMENT ; }
        }


        public override eActions ActionCode
        {
            get { return eActions.USE_DEFAULT_DOCUMENT_TYPE; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
