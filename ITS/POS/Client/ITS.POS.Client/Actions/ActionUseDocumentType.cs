using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    public class ActionUseDocumentType : Action
    {
        public ActionUseDocumentType(IPosKernel kernel) : base(kernel)
        {

        }


        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            IDocumentService documentService = this.Kernel.GetModule<IDocumentService>();


            ActionUserDocumentTypeParams castedParams = parameters as ActionUserDocumentTypeParams;

            DocumentType dType = null;
            if (string.IsNullOrWhiteSpace(castedParams.DocumentTypeCode))
            {
                List<Guid> standardPOSDocumentTypes = new List<Guid>()
                {
                    configurationManager.DefaultDocumentTypeOid,
                    configurationManager.ProFormaInvoiceDocumentTypeOid,
                    configurationManager.SpecialProformaDocumentTypeOid
                };
                //CriteriaOperator documentTypeCriteria = CriteriaOperator.Or(new InOperator("Oid", standardPOSDocumentTypes),
                //                                                              new BinaryOperator("IsPrintedOnStoreController", true)
                //                                                            );

                // XPCollection<DocumentType> documentTypes = new XPCollection<DocumentType>(sessionManager.GetSession<DocumentType>(), documentTypeCriteria);//

                XPCollection<DocumentType> documentTypes = documentService.GetAllValidPosDocumentTypes(configurationManager, sessionManager);

                using (frmSelectLookup frm = new frmSelectLookup(POSClientResources.DOCUMENT_TYPE, documentTypes, "Description", "This", new List<string>() { "Code", "Description" }, documentTypes.FirstOrDefault(), this.Kernel))
                {
                    DialogResult result = frm.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }
                    dType = frm.SelectedObject as DocumentType;
                }
            }
            else
            {
                dType = sessionManager.FindObject<DocumentType>(new BinaryOperator("Code", castedParams.DocumentTypeCode));
            }

            if (dType == null)
            {
                throw new POSUserVisibleException(POSClientResources.DOCUMENT_TYPE + "(" + castedParams.DocumentTypeCode + "): " + POSClientResources.NOT_FOUND);
            }

            if (dType.Oid == configurationManager.DefaultDocumentTypeOid)
            {
                actionManager.GetAction(eActions.USE_DEFAULT_DOCUMENT_TYPE).Execute();
                return;
            }

            if (dType.Oid == configurationManager.ProFormaInvoiceDocumentTypeOid)
            {
                actionManager.GetAction(eActions.USE_PROFORMA).Execute();
                return;
            }


            XPCollection<StoreDocumentSeriesType> storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>();

            if (dType.IsPrintedOnStoreController)
            {
                storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>(sessionManager.GetSession<StoreDocumentSeriesType>(),
                                                                                     new BinaryOperator("DocumentType", dType.Oid)
                                                                                 );
            }
            else
            {
                XPCollection<DocumentSeries> series = new XPCollection<DocumentSeries>(sessionManager.GetSession<DocumentSeries>(),
           CriteriaOperator.And(new BinaryOperator("POS", configurationManager.CurrentTerminalOid)));

                storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>(sessionManager.GetSession<StoreDocumentSeriesType>(),
                    CriteriaOperator.And(new BinaryOperator("DocumentType", dType.Oid), new InOperator("DocumentSeries", series.Select(x => x.Oid))));

            }



            if (storeDocumentSeriesTypes.Count == 0)
            {
                throw new POSUserVisibleException(POSClientResources.NO_DOCUMENT_SERIES_DEFINED_FOR + " " + dType.Code + " - " + dType.Description);
            }
            if (storeDocumentSeriesTypes.Count > 1)
            {
                throw new POSUserVisibleException(POSClientResources.MULTIPLE_DOCUMENT_SERIES_DEFINED_FOR + " " + dType.Code + " - " + dType.Description);
            }

            if (AppContext.GetMachineStatus() == eMachineStatus.SALE || AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
            {
                if (AppContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(false), dontCheckPermissions: true);
                }
                AppContext.CurrentDocument.DocumentType = dType.Oid;
                AppContext.CurrentDocument.DocumentSeries = storeDocumentSeriesTypes[0].DocumentSeries;
                if (storeDocumentSeriesTypes[0].DefaultCustomer != Guid.Empty &&
                   (AppContext.CurrentCustomer == null ||
                   (AppContext.CurrentCustomer != null &&
                   AppContext.CurrentCustomer.Oid == configurationManager.DefaultCustomerOid)))
                {
                    Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(storeDocumentSeriesTypes[0].DefaultCustomer);
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
            get { return eActions.USE_DOCUMENT_TYPE; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }



    }
}
