using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Performs a stress test by printing receipts automatically with random items.
    /// </summary>
    public class ActionStressTest : Action
    {
        public ActionStressTest(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.STRESS_TEST; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ActionStressTestParams castedParams = parameters as ActionStressTestParams;
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            DialogResult result = formManager.ShowMessageBox("Stress test will run." + Environment.NewLine + castedParams.NumberOfReceipts +
                                             " Receipts" + Environment.NewLine + castedParams.ItemsPerReceipt +
                                             " Items per receipt" + Environment.NewLine +
                                             "Random Customer: " + castedParams.RandomCustomer + Environment.NewLine +
                                             "Random Payment: " + castedParams.RandomPayment + Environment.NewLine +
                                             "Random Proforma: " + castedParams.RandomProforma + Environment.NewLine +
                                             "Random Cancel Document: " + castedParams.RandomCancelDocument + Environment.NewLine +
                                             "Random Cancel Lines: " + castedParams.RandomCancelLines , MessageBoxButtons.OKCancel);

            PriceCatalogPolicy priceCatalogPolicy = customerService.GetPriceCatalogPolicy(config.DefaultCustomerOid, config.CurrentStoreOid);
            if (priceCatalogPolicy == null)
            {
                throw new POSException(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
            }
            Guid priceCatalogPolicyOid = priceCatalogPolicy.Oid;


            if (result == DialogResult.OK)
            {
                for (int i = 0; i < castedParams.NumberOfReceipts; i++)
                {
                    actionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(true), dontCheckPermissions: true);
                    if (castedParams.RandomProforma && new Random().Next(1, 5) == 1) //20% chance to start proforma
                    {
                        actionManager.GetAction(eActions.USE_PROFORMA).Execute(dontCheckPermissions: true);
                    }

                    if (castedParams.RandomCustomer)
                    {
                        try
                        {
                            Customer cust = GetRandomCustomer(sessionManager);
                            if (cust != null)
                            {
                                actionManager.GetAction(eActions.ADD_CUSTOMER_INTERNAL).Execute(new ActionAddCustomerInternalParams(cust, cust.Code, null));
                            }
                        }
                        catch (Exception ex)
                        {
                            Kernel.LogFile.Error("Stress Test Error getting customer: " + ex.GetFullMessage());
                        }
                    }
                    for (int j = 0; j < castedParams.ItemsPerReceipt; j++)
                    {
                        Random rnd = new Random();
                        decimal qty = 1 + rnd.Next(10);
                        string itemCode = GetRandomItemFromDatabase(sessionManager, priceCatalogPolicyOid);
                        Item item = sessionManager.FindObject<Item>(new BinaryOperator("Code", itemCode));
                        if (item != null)
                        {
                            Barcode barcode = sessionManager.GetObjectByKey<Barcode>(item.DefaultBarcode);

                            if (barcode != null)
                            {
                                MeasurementUnit mu = sessionManager.GetObjectByKey<MeasurementUnit>(barcode.MeasurementUnit(item.Owner));
                                if (mu != null && mu.SupportDecimal)
                                {
                                    qty = (1 + rnd.Next(10000)) / 1000m;
                                    if (qty < 0.001m)
                                    {
                                        qty = 1;
                                    }
                                }
                            }
                        }
                        actionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(itemCode, qty));

                        if (castedParams.RandomCancelLines && appContext.CurrentDocument.DocumentDetails.Count(detail => detail.IsCanceled == false) > 1 &&
                            rnd.Next(1, 10) == 1) //10% chance to cancel item
                        {
                            actionManager.GetAction(eActions.DELETE_ITEM).Execute(new ActionDeleteItemParams(appContext.CurrentDocumentLine, false));
                        }
                    }

                    actionManager.GetAction(eActions.DOCUMENT_TOTAL).Execute();
                    if (castedParams.RandomPayment)
                    {
                        PaymentMethod randomPayment = GetRandomPaymentMethod(sessionManager);
                        actionManager.GetAction(eActions.ADD_TOTAL_PAYMENT).Execute(new ActionAddTotalPaymentParams(randomPayment, null));
                    }
                    else
                    {
                        PaymentMethod defaultPaymentMethod = sessionManager.GetObjectByKey<PaymentMethod>(config.DefaultPaymentMethodOid);
                        actionManager.GetAction(eActions.ADD_TOTAL_PAYMENT).Execute(new ActionAddTotalPaymentParams(defaultPaymentMethod, null));
                    }

                    Application.DoEvents();
                    if (castedParams.RandomCancelDocument && new Random().Next(1, 10) == 1) //10% chance to cancel document
                    {
                        actionManager.GetAction(eActions.CANCEL_DOCUMENT).Execute(new ActionCancelDocumentParams(false));
                    }
                    else
                    {
                        actionManager.GetAction(eActions.DOCUMENT_TOTAL).Execute();
                    }
                    Application.DoEvents();
                }
            }
        }

        private void GetPriceCatalogIDS(ISessionManager sessionManager, PriceCatalog pc, List<Guid> oids)
        {
            oids.Add(pc.Oid);

            if (pc.ParentCatalogOid != Guid.Empty && pc.ParentCatalogOid != null)
            {
                PriceCatalog parent = sessionManager.GetSession<PriceCatalog>().GetObjectByKey<PriceCatalog>(pc.ParentCatalogOid);
                GetPriceCatalogIDS(sessionManager, parent, oids);
            }
        }

        public List<Guid> GetPriceCatalogsFromPolicy(ISessionManager sessionManager, Guid priceCatalogPolicyOid)
        {

            List<Guid> pcOids = new List<Guid>();
            PriceCatalogPolicy priceCatalogPolicy = sessionManager.GetObjectByKey<PriceCatalogPolicy>(priceCatalogPolicyOid);
            if (priceCatalogPolicy != null)
            {
                foreach (PriceCatalogPolicyDetail priceCatalogPolicyDetail in priceCatalogPolicy.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort))
                {
                    GetPriceCatalogIDS(sessionManager, priceCatalogPolicyDetail.GetPriceCatalog, pcOids);
                }
            }
            return pcOids;
        }

        private Customer GetRandomCustomer(ISessionManager sessionManager)
        {
            XPQuery<Customer> customers = new XPQuery<Customer>(sessionManager.GetSession<Customer>());
            int count = (int)sessionManager.GetSession<Customer>().Evaluate(typeof(Customer), CriteriaOperator.Parse("Count"), null);
            int skip = new Random().Next(0, count - 1);
            return customers.OrderBy(cust => cust.Code).Skip(skip).FirstOrDefault();
        }

        private PaymentMethod GetRandomPaymentMethod(ISessionManager SessionManager)
        {
            XPQuery<PaymentMethod> paymentMethods = new XPQuery<PaymentMethod>(SessionManager.GetSession<PaymentMethod>());
            int count = (int)SessionManager.GetSession<PaymentMethod>().Evaluate(typeof(PaymentMethod), CriteriaOperator.Parse("Count"), null);
            int skip = new Random().Next(0, count - 1);
            return paymentMethods.OrderBy(payment => payment.Code).Skip(skip).FirstOrDefault();
        }

        private string GetRandomItemFromDatabase(ISessionManager sessionManager, Guid policy) //PriceCatalog pc)
        {

            List<Guid> priceCatalogids = new List<Guid>();
            //GetPriceCatalogIDS(sessionManager, pc, priceCatalogids);
            priceCatalogids = GetPriceCatalogsFromPolicy(sessionManager, policy);
            string oids = "";
            if (priceCatalogids.Count == 1)
            {
                oids = "'" + priceCatalogids.First().ToString() + "'";
            }
            else
            {
                oids = priceCatalogids.Select(x => "'" + x.ToString()+"'").Aggregate((x, y) =>  x + "," + y);
            }

            string randomItemCode = sessionManager.GetSession<Item>().ExecuteScalar(String.Format(@"SELECT Item.Code FROM Item inner join pricecatalogdetail 
on Item.Oid =  PriceCatalogDetail.Item where PriceCatalogDetail.PriceCatalog in ({0}) and Item.CustomPriceOptions = 0 and 
Item.GCRecord is null and PriceCatalogDetail.GCRecord is null
and PriceCatalogDetail.IsActive = 1 and Item.IsActive = 1 and PriceCatalogDetail.Value > 0 ORDER BY RANDOM() LIMIT 1", oids)).ToString();

            return randomItemCode;
        }
    }
}
