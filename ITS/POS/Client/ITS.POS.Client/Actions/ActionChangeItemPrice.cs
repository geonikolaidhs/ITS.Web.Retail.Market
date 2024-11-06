using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Changes the current line's unit price, if the item supports price changing.
    /// A GetItemPrice form is displayed so that the user can input the new unit price of the line.
    /// 
    /// If the setting "POSCanChangePrices" is true, then a PriceCatalogDetail is also created.
    /// </summary>
    public class ActionChangeItemPrice : Action
    {

        public ActionChangeItemPrice(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.CHANGE_ITEM_PRICE; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION2;
            }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ActionChangeItemPriceParams castedParams = parameters as ActionChangeItemPriceParams;
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            IPriceCreator priceCreator = Kernel.GetModule<IPriceCreator>();

            Item item = sessionManager.GetObjectByKey<Item>(castedParams.DocumentDetail.Item);
            if (item.CustomPriceOptions == eItemCustomPriceOptions.NONE && castedParams.DocumentDetail.Qty > 0)
            {
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ITEM_DOES_NOT_SUPPORT_PRICE_CHANGE));
                return;
            }
            using (frmGetItemPrice form = new frmGetItemPrice(this.Kernel))
            {
                form.Item = item;
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    decimal price = form.Price;
                    documentService.ChangeDocumentLinePrice(appContext.CurrentDocumentLine, price);

                    if(config.GetAppSettings().POSCanChangePrices)
                    {
                        Guid defaultPriceCatalogPolicyGuid = sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid).DefaultPriceCatalogPolicy;
                        PriceCatalogPolicy defaultPriceCatalogPolicy = sessionManager.GetObjectByKey<PriceCatalogPolicy>(defaultPriceCatalogPolicyGuid);

                        if (defaultPriceCatalogPolicy == null)
                        {
                            throw new POSException(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
                        }

                        string updatedPcdJson= priceCreator.CreateOrUpdatePrice(defaultPriceCatalogPolicy, item, price, appContext.CurrentDocumentLine.PriceCatalogValueVatIncluded);
                        appContext.CurrentDocumentLine.POSGeneratedPriceCatalogDetailSerialized = updatedPcdJson;
                        appContext.CurrentDocumentLine.Save();
                        appContext.CurrentDocumentLine.Session.CommitTransaction();
                    }


                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(appContext.CurrentDocumentLine,true,true));
                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(appContext.CurrentDocument));
                }
            }
        }
    }
}
