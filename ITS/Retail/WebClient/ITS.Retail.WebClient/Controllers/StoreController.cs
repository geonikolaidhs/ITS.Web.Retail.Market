using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Helpers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.SessionState;

namespace ITS.Retail.WebClient.Controllers
{
    public class StoreController : BaseObjController<Store>
    {
        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return new Dictionary<PropertyInfo, string>()
                {
                    { typeof(StorePriceList).GetProperty("PriceList"), "PriceCatalogCb_VI" },
                    { typeof(StorePriceCatalogPolicy).GetProperty("PriceCatalogPolicy"), "PriceCatalogPolicyCb_VI" }
                };
            }
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsActive", "ImageOid", "CentralOid","Name" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(StorePriceList), new List<string>() { "IsActive", "IsDefault" });
            ruleset.DetailsToIgnore.AddRange(new List<string>() {
                                                                    "BarcodeTypes",
                                                                    "DocumentSeries",
                                                                    "DocumentHeaders",
                                                                    "Customers",
                                                                    "UserDailyTotalss",
                                                                    "DailyTotalss",
                                                                    "ItemStocks",
                                                                    "Terminals",
                                                                    "Items",
                                                                    "Users",
                                                                    "StoreAnalyticTrees"
                                                                }
                                            );


            ruleset.DetailedPropertiesToShow.Add("StoreControllerSettings");
            ruleset.DetailedPropertiesPropToIgnore.Add(typeof(StoreControllerSettings), new List<string>() {"IsActive", "IsDefault", "CultureInfo", "Store"});
            ruleset.DetailedPropertiesNumOfCol.Add(typeof(StoreControllerSettings), 3);

            ruleset.NumberOfColumns = 3;
            return ruleset;
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();
            ViewBag.Title = Resources.EditStore;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.Visible = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL;
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "StoreID");
            CustomJSProperties.AddJSProperty("gridName", "grdStore");

            return View(GetList<Store>(XpoSession));
        }

        protected int GetNextStoreControllerSettingsID(UnitOfWork uow, CompanyNew owner)
        {
            StoreControllerSettings settingsWithMaxID = GetList<StoreControllerSettings>(uow, sortingField:"ID", direction: SortingDirection.Descending).FirstOrDefault();

            int newID = settingsWithMaxID== null ? 1 : settingsWithMaxID.ID + 1;
            return newID;
        }

        protected bool CheckIfIdExists(CompanyNew owner,int storeControllerID,Guid storeGuid)
        {
            StoreControllerSettings existing = owner.Session.FindObject<StoreControllerSettings>(
                CriteriaOperator.And(
                                        new BinaryOperator("Owner.Oid", owner.Oid),
                                        new BinaryOperator("ID",storeControllerID),
                                        new NotOperator(new BinaryOperator("Store.Oid",storeGuid))
                                    ));

            return existing != null;
        }

        public ActionResult Edit(string Oid)
        {
            if(this.ToolbarOptions != null)
            {
                this.ToolbarOptions.ForceVisible = false;
            }

            Guid StoreGuid;
            if (Guid.TryParse(Oid, out StoreGuid) == false)
            {
                Session["Error"] = Resources.StoreNotFound;
            }
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            StoreControllerSettings storeSettings;
            Store store = uow.GetObjectByKey<Store>(StoreGuid);
            if (store == null)
            {
                store = new Store(uow);
                store.Address = new Address(uow);
                store.Owner = uow.GetObjectByKey<CompanyNew>(CurrentOwner.Oid);
                store.Address.Trader = store.Owner.Trader;
                storeSettings = new StoreControllerSettings(uow);
                storeSettings.Store = store;
                storeSettings.Owner = store.Owner;
                storeSettings.ID = GetNextStoreControllerSettingsID(uow, store.Owner);
            }
            else
            {
                if (store.StoreControllerSettings == null)
                {
                    storeSettings = new StoreControllerSettings(uow);
                    storeSettings.Store = store;
                    storeSettings.Owner = store.Owner;
                    storeSettings.ID = GetNextStoreControllerSettingsID(uow, store.Owner);
                }
                else
                {
                    if(store.StoreControllerSettings.Owner == null)
                {
                    store.StoreControllerSettings.Owner = store.Owner;
                }
            }
            }

            Session["Trader"] = store.Owner.Trader;
            Session["NewTrader"] = true;
            Session["EditingStore"] = store;
            FillLookupComboBoxes();
            ViewBag.DefaultPhoneComboBox = store.Address.Phones.OrderBy(phone => phone.PhoneType);
            return PartialView(Session["EditingStore"]);
        }

        public JsonResult Save()
        {
            Store store = (Store)Session["EditingStore"];
            try
            {
                if (CheckIfIdExists(store.Owner, Int32.Parse(Request["ID"]),store.Oid))
                {
                    Session["Error"] = Resources.StoreControllerIdAlreadyExists;
                    throw new Exception(Resources.StoreControllerIdAlreadyExists);
                }

                bool isCentral = Request["IsCentralStore"] == "C";
                if (isCentral)
                {
                    CriteriaOperator counterExpression = CriteriaOperator.Parse("Count()");
                    CriteriaOperator storeCriteria = CriteriaOperator.And(new BinaryOperator("Oid", store.Oid, BinaryOperatorType.NotEqual),
                                                                              new BinaryOperator("IsCentralStore", true)
                                                                            );
                    int centralStoresFound = (int)store.Session.Evaluate<Store>(counterExpression, ApplyOwnerCriteria(storeCriteria, typeof(Store), EffectiveOwner));
                    if (centralStoresFound > 0)
                    {
                        string errorMessage = Resources.CentralStoreAlreadyExists;
                        Session["Error"] = errorMessage;
                        throw new Exception(errorMessage);
                    }
                }
                if (Session["StoreOwnerImage"] is OwnerImage)
                {
                    OwnerImage ownerImage = (OwnerImage)Session["StoreOwnerImage"];
                    ownerImage.OwnerApplicationSettingsOid = store.Oid;
                    store.ImageOid = ownerImage.Oid;
                    ownerImage.Save();
                }
                UpdateLookupObjects(store);
                string storeCode = Request["StoreCode"];                
                if (StoreCodeExists(store, storeCode))
                {
                    Session["Error"] = Resources.CodeAlreadyExists;
                    this.ToolbarOptions.ForceVisible = false;
                    FillLookupComboBoxes();
                    ViewBag.DefaultPhoneComboBox = store.Address.Phones.OrderBy(phone => phone.PhoneType);
                    return Json(new { error = Session["Error"] });
                }

                if (!(MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER))
                {
                    Guid referenceCompanyGuid = Guid.Empty;
                    CompanyNew referenceCompany = null;
                    if (Guid.TryParse(Request["ReferenceCompany_VI"], out referenceCompanyGuid))
                    {
                        referenceCompany = store.Session.GetObjectByKey<CompanyNew>(referenceCompanyGuid);
                    }
                    store.ReferenceCompany = referenceCompany;
                }
                

                XpoHelper.CommitChanges((UnitOfWork)store.Session);
                CreateOrUpdateRequiredElements(store.StoreControllerSettings);
                UpdateCurrentUserSettings();
                
            }
            catch (Exception ex)
            {
                Session["Error"] = Resources.AnErrorOccurred + ":" + ex.Message;
                return Json(new { error = Session["Error"] });
            }
            if (!String.IsNullOrEmpty(Session["Error"].ToString()))
            {
                UpdateLookupObjects(store);
                FillLookupComboBoxes();
                ViewBag.DefaultPhoneComboBox = store.Address.Phones.OrderBy(phone => phone.PhoneType);
                return Json(new {error = Session["Error"]});
            }
            return Json(new { });
        }

        private bool StoreCodeExists(Store store,string storeCode)
        {
          Store str =  store.Session.FindObject<Store>( CriteriaOperator.And(
                                            new BinaryOperator("Owner", store.Owner.Oid),
                                            new BinaryOperator("Code",storeCode),
                                            new NotOperator(new BinaryOperator("Oid",store.Oid))
                                            ));

            return str!=null;
        }

        protected override void UpdateLookupObjects(Store store)
        {
            base.UpdateLookupObjects(store);
            store.IsCentralStore = Request["IsCentralStore"] == "C";
            store.Address.AddressType = GetObjectByArgument<AddressType>(store.Session, "AddressTypeCb_VI");
            store.Address.VatLevel = GetObjectByArgument<VatLevel>(store.Session, "VatLevelCb_VI");
            store.Address.City = Request["City"];
            store.Address.DefaultPhone = GetObjectByArgument(store.Session, "DefaultPhoneCb_VI", store.Address.Phones);
            store.Address.email = Request["Email"];
            store.Address.POBox = Request["POBox"];
            store.Address.PostCode = Request["PostCode"];
            store.Address.Street = Request["Street"];
            store.Central = GetObjectByArgument<Store>(store.Session, "Central_VI");
            store.Code = Request["StoreCode"];
            store.Name = Request["StoreName"];
            store.DefaultPriceCatalogPolicy = GetObjectByArgument(store.Session, "DefaultPriceCatalogPolicy_VI", store.StorePriceCatalogPolicies.Select(g => g.PriceCatalogPolicy));
            store.StoreControllerSettings.ID = Int32.Parse(Request["ID"]);
            store.StoreControllerSettings.MaximumNumberOfPOS = Int32.Parse(Request["MaximumNumberOfPOS"]);
            store.StoreControllerSettings.ServerName = Request["ServerName"];
            store.StoreControllerSettings.ServerUrl = Request["ServerUrl"];
            store.StoreControllerSettings.DefaultCustomer = GetObjectByArgument<Customer>(store.Session, "DefaultCustomerComboBox_VI");
            store.StoreControllerSettings.DepositDocumentType = GetObjectByArgument<DocumentType>(store.Session, "DepositDocumentType_VI");
            store.StoreControllerSettings.WithdrawalDocumentType = GetObjectByArgument<DocumentType>(store.Session, "WithdrawalDocumentType_VI");
            store.StoreControllerSettings.ReceiptDocumentType = GetObjectByArgument<DocumentType>(store.Session, "ReceiptDocumentType_VI");
            store.StoreControllerSettings.ProformaDocumentType = GetObjectByArgument<DocumentType>(store.Session, "ProformaDocumentType_VI");
            store.StoreControllerSettings.WithdrawalItem = GetObjectByArgument<SpecialItem>(store.Session, "WithdrawalItemComboBox_VI");
            store.StoreControllerSettings.SpecialProformaDocumentType = GetObjectByArgument<DocumentType>(store.Session, "SpecialProformaDocumentType_VI");
            store.StoreControllerSettings.DepositItem = GetObjectByArgument<SpecialItem>(store.Session, "DepositItemComboBox_VI");
            store.StoreControllerSettings.DefaultDocumentStatus = GetObjectByArgument<DocumentStatus>(store.Session, "DefaultDocumentStatus_VI");
            store.StoreControllerSettings.POSSellsInactiveItems = Request["POSSellsInactiveItems"] == "C";

            try
            {
                if (this.IsValidPath(Request["TransactionFilesFolder"]))
                {
                    store.StoreControllerSettings.TransactionFilesFolder = Request["TransactionFilesFolder"];
                }     
            }
            catch(Exception exc)
            {
                string exceptionMessage = exc.GetFullMessage();
                Session["Error"] = Resources.PathIsInvalid;
            }
        }

        private bool IsValidPath(string path)
        {
            Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheck.IsMatch(path.Substring(0, 3)))
            {
                return false;
            }
            string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
            strTheseAreInvalidFileNameChars += @":/?*" + "\"";
            Regex containsABadCharacter = new Regex(string.Format("[{0}]", Regex.Escape(strTheseAreInvalidFileNameChars)));
            if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
            {
                return false;
            }

            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(path));
            if (!dir.Exists)
            {
               dir.Create();
            }
            return true;
        }

        protected void CreateOrUpdateRequiredElements(StoreControllerSettings scs)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection<DocumentSeries> allDocumentSeries = GetList<DocumentSeries>(uow);
                Store currentStore = uow.GetObjectByKey<Store>(scs.Store.Oid);
                DocumentType ReceiptDocumentType = scs==null || scs.ReceiptDocumentType==null ? null :  uow.GetObjectByKey<DocumentType>(scs.ReceiptDocumentType.Oid);
                DocumentType ProformaDocumentType = scs == null || scs.ProformaDocumentType == null ? null : uow.GetObjectByKey<DocumentType>(scs.ProformaDocumentType.Oid);
                DocumentType DepositDocumentType = scs == null || scs.DepositDocumentType == null ? null : uow.GetObjectByKey<DocumentType>(scs.DepositDocumentType.Oid);
                DocumentType WithdrawalDocumentType = scs == null || scs.WithdrawalDocumentType == null ? null : uow.GetObjectByKey<DocumentType>(scs.WithdrawalDocumentType.Oid);
                DocumentType SpecialProformaDocumentType = scs == null || scs.SpecialProformaDocumentType == null ? null : uow.GetObjectByKey<DocumentType>(scs.SpecialProformaDocumentType.Oid);
                string prefix = scs.Store.Code + "_";
                string receiptSufix = "_Receipt";
                string proformaSufix = "_Proforma";
                string depositSufix = "_Deposit";
                string withdrawSufix = "_Withdraw";
                string specialProformaSufix = "_SpecialProforma";

                for (int i = 1; i <= scs.MaximumNumberOfPOS; i++)
                {
                    string posNameWithPrefix = prefix + "POS" + i;

                    //receipt
                    DocumentSeries receiptSeries;
                    string seriesName = posNameWithPrefix + receiptSufix;
                    IEnumerable<DocumentSeries> documentSeriesEnum = allDocumentSeries.Where(g => g.Description == seriesName);
                    if (documentSeriesEnum.Count() == 0)
                    {
                        receiptSeries = new DocumentSeries(uow);
                        receiptSeries.Code = seriesName;
                        receiptSeries.Description = seriesName;
                        receiptSeries.Owner = uow.GetObjectByKey<CompanyNew>(scs.Owner.Oid);
                        receiptSeries.Store = currentStore;
                        receiptSeries.HasAutomaticNumbering = true;
                        receiptSeries.eModule = Platform.Enumerations.eModule.POS;
                        receiptSeries.Save();
                    }
                    else if (documentSeriesEnum.Count() > 1)
                    {
                        throw new Exception(Resources.AnErrorOccurred);
                    }
                    else
                    {
                        receiptSeries = documentSeriesEnum.First();
                    }

                    //proforma
                    DocumentSeries proformaSeries;
                    seriesName = posNameWithPrefix + proformaSufix;
                    documentSeriesEnum = allDocumentSeries.Where(g => g.Description == seriesName);
                    if (documentSeriesEnum.Count() == 0)
                    {
                        proformaSeries = new DocumentSeries(uow);
                        proformaSeries.Code = seriesName;
                        proformaSeries.Description = seriesName;
                        proformaSeries.Owner = uow.GetObjectByKey<CompanyNew>(scs.Owner.Oid);
                        proformaSeries.Store = currentStore;
                        proformaSeries.HasAutomaticNumbering = true;
                        proformaSeries.eModule = Platform.Enumerations.eModule.POS;
                        proformaSeries.Save();
                    }
                    else if (documentSeriesEnum.Count() > 1)
                    {
                        throw new Exception(Resources.AnErrorOccurred);
                    }
                    else
                    {
                        proformaSeries = documentSeriesEnum.First();
                    }

                    //deposit
                    DocumentSeries depositSeries;
                    seriesName = posNameWithPrefix + depositSufix;
                    documentSeriesEnum = allDocumentSeries.Where(g => g.Description == seriesName);
                    if (documentSeriesEnum.Count() == 0)
                    {
                        depositSeries = new DocumentSeries(uow);
                        depositSeries.Code = seriesName;
                        depositSeries.Description = seriesName;
                        depositSeries.Owner = uow.GetObjectByKey<CompanyNew>(scs.Owner.Oid);
                        depositSeries.Store = currentStore;
                        depositSeries.HasAutomaticNumbering = true;
                        depositSeries.eModule = Platform.Enumerations.eModule.POS;
                        depositSeries.Save();
                    }
                    else if (documentSeriesEnum.Count() > 1)
                    {
                        throw new Exception(Resources.AnErrorOccurred);
                    }
                    else
                    {
                        depositSeries = documentSeriesEnum.First();
                    }

                    //withdraw
                    DocumentSeries withdrawSeries;
                    seriesName = posNameWithPrefix + withdrawSufix;
                    documentSeriesEnum = allDocumentSeries.Where(g => g.Description == seriesName);
                    if (documentSeriesEnum.Count() == 0)
                    {
                        withdrawSeries = new DocumentSeries(uow);
                        withdrawSeries.Code = seriesName;
                        withdrawSeries.Description = seriesName;
                        withdrawSeries.Owner = uow.GetObjectByKey<CompanyNew>(scs.Owner.Oid);
                        withdrawSeries.Store = currentStore;
                        withdrawSeries.HasAutomaticNumbering = true;
                        withdrawSeries.eModule = Platform.Enumerations.eModule.POS;
                        withdrawSeries.Save();
                    }
                    else if (documentSeriesEnum.Count() > 1)
                    {
                        throw new Exception(Resources.AnErrorOccurred);
                    }
                    else
                    {
                        withdrawSeries = documentSeriesEnum.First();
                    }

                    //special proforma
                    DocumentSeries specialProformaSeries;
                    seriesName = posNameWithPrefix + specialProformaSufix;
                    documentSeriesEnum = allDocumentSeries.Where(g => g.Description == seriesName);
                    if (documentSeriesEnum.Count() == 0)
                    {
                        specialProformaSeries = new DocumentSeries(uow);
                        specialProformaSeries.Code = seriesName;
                        specialProformaSeries.Description = seriesName;
                        specialProformaSeries.Owner = uow.GetObjectByKey<CompanyNew>(scs.Owner.Oid);
                        specialProformaSeries.Store = currentStore;
                        specialProformaSeries.HasAutomaticNumbering = true;
                        specialProformaSeries.eModule = Platform.Enumerations.eModule.POS;
                        specialProformaSeries.Save();
                    }
                    else if (documentSeriesEnum.Count() > 1)
                    {
                        throw new Exception(Resources.AnErrorOccurred);
                    }
                    else
                    {
                        specialProformaSeries = documentSeriesEnum.First();
                    }

                    StoreDocumentSeriesType receipt, proforma, deposit, withdraw , specialProformaStoreDocumentSeriesType;
                    receipt = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                            new BinaryOperator("DocumentSeries", receiptSeries), new BinaryOperator("DocumentType.Oid", ReceiptDocumentType.Oid)
                        ));

                    proforma = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                            new BinaryOperator("DocumentSeries", proformaSeries), new BinaryOperator("DocumentType.Oid", ProformaDocumentType.Oid)
                        ));

                    deposit = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                            new BinaryOperator("DocumentSeries", depositSeries), new BinaryOperator("DocumentType.Oid", DepositDocumentType.Oid)
                        ));

                    withdraw = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                            new BinaryOperator("DocumentSeries", withdrawSeries), new BinaryOperator("DocumentType.Oid", WithdrawalDocumentType.Oid)
                        ));
                    specialProformaStoreDocumentSeriesType = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(
                            new BinaryOperator("DocumentSeries", withdrawSeries), new BinaryOperator("DocumentType.Oid", WithdrawalDocumentType.Oid)
                        ));

                    if (receipt == null)
                    {
                        receipt = new StoreDocumentSeriesType(uow);
                        receipt.DocumentType = ReceiptDocumentType;
                        receipt.DocumentSeries = receiptSeries;
                        receipt.Save();
                    }
                    if (proforma == null)
                    {
                        proforma = new StoreDocumentSeriesType(uow);
                        proforma.DocumentType = ProformaDocumentType;
                        proforma.DocumentSeries = proformaSeries;
                        proforma.Save();
                    }
                    if (deposit == null)
                    {
                        deposit = new StoreDocumentSeriesType(uow);
                        deposit.DocumentType = DepositDocumentType;
                        deposit.DocumentSeries = depositSeries;
                        deposit.Save();
                    }
                    if (withdraw == null)
                    {
                        withdraw = new StoreDocumentSeriesType(uow);
                        withdraw.DocumentType = WithdrawalDocumentType;
                        withdraw.DocumentSeries = withdrawSeries;
                        withdraw.Save();
                    }
                    if (specialProformaStoreDocumentSeriesType == null)
                    {
                        specialProformaStoreDocumentSeriesType = new StoreDocumentSeriesType(uow);
                        specialProformaStoreDocumentSeriesType.DocumentType = SpecialProformaDocumentType;
                        specialProformaStoreDocumentSeriesType.DocumentSeries = specialProformaSeries;
                        specialProformaStoreDocumentSeriesType.Save();
                    }
                }
                XpoHelper.CommitChanges(uow);
                if(this.CurrentStore != null && this.CurrentStore.Oid == currentStore.Oid)
                {
                    this.CurrentStore.LoadPersistent(currentStore);
                }
            }
        }

        public ActionResult CustomersComboBox()
        {
            return PartialView();
        }

        public static object CustomersRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            CriteriaOperator visibleCustomers = null;
            Store store = System.Web.HttpContext.Current.Session["EditingStore"] as Store;
            if (store != null)
            {
                visibleCustomers = new BinaryOperator("Owner.Oid", store.Owner.Oid);
            }
            else
            {
                visibleCustomers = new BinaryOperator("Oid", Guid.Empty);
            }

            XPCollection<Customer> collection = GetList<Customer>(XpoHelper.GetNewUnitOfWork(),
                            CriteriaOperator.And(visibleCustomers, CriteriaOperator.Or(new BinaryOperator("CompanyName", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                       new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like))),"Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }


        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.AddressTypeComboBox = GetList<AddressType>(XpoSession).OrderBy(address => address.Description);
            ViewBag.VatLevelComboBox = GetList<VatLevel>(XpoSession);
            if (Session["EditingStore"] is Store)
            {
                ViewBag.CentralStoreList = ((Store)Session["EditingStore"]).Owner.Stores.Where(g => g.IsCentralStore);
                ViewBag.PriceCatalogs = ((Store)Session["EditingStore"]).StorePriceLists.Where(spl => spl.PriceList != null).Select(g => g.PriceList);
                ViewBag.PriceCatalogPolicies = ((Store)Session["EditingStore"]).StorePriceCatalogPolicies.Where(spl => spl.PriceCatalogPolicy != null).Select(g => g.PriceCatalogPolicy);
            }
            ViewBag.DocumentTypes = GetList<DocumentType>(XpoSession);
            ViewBag.DocumentStatuses = GetList<DocumentStatus>(XpoSession);
            ViewBag.OwnersPriceCatalogs = GetList<PriceCatalog>(XpoSession);
            ViewBag.Labels = GetList<Label>(XpoSession);

            ViewBag.Companies = GetList<CompanyNew>(XpoSession);
        }

        public static object DepositItemRequestedByValue(ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                SpecialItem obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<SpecialItem>(e.Value);
                return obj;
            }
            return null;

        }

        public static object GetDepositItemByValue(object value)
        {
            return GetObjectByValue<SpecialItem>(value);
        }

        public static object DepositItemsRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');

            XPCollection<SpecialItem> collection = GetList<SpecialItem>(XpoHelper.GetNewUnitOfWork(),
                                                                    CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                        new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)),
                                                                    "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult WithdrawalItemsComboBox()
        {
            return PartialView();
        }

        public ActionResult DepositItemsComboBox()
        {
            return PartialView();
        }

        public ActionResult StorePriceListGrid()
        {
            Store store = Session["EditingStore"] as Store;
            if (store != null)
            {
                FillLookupComboBoxes();
                return PartialView(store.StorePriceLists);
            }
            return PartialView();
        }

        public ActionResult StorePriceCatalogPoliciesGrid()
        {
            Store store = Session["EditingStore"] as Store;
            if (store != null)
            {
                ViewBag.PriceCatalogPolicies = GetList<PriceCatalogPolicy>(XpoSession);
                return PartialView(store.StorePriceCatalogPolicies);
            }
            return PartialView();
        }

        public ActionResult StorePricelistUpdate([ModelBinder(typeof(RetailModelBinder))] StorePriceList ct)
        {
            Store store = Session["EditingStore"] as Store;
            UpdateLookupObjects(ct);

            if (store != null && ct.PriceList != null)
            {
                string modelKey;
                string modelError = StoreHelper.CheckedStorePriceLists(store, ct, out modelKey);
                if (String.IsNullOrWhiteSpace(modelError))
                {
                    StorePriceList storePriceList = store.StorePriceLists.FirstOrDefault(spl => spl.Oid == ct.Oid) ?? new StorePriceList(store.Session);
                    storePriceList.GetData(ct, new List<string>() { "Session" });
                    storePriceList.Store = store;
                }
                else
                {
                    ModelState.AddModelError(modelKey, modelError);
                    ViewBag.CurrentItem = ct;
                    ViewBag.OwnersPriceCatalogs = GetList<PriceCatalog>(XpoSession);
                }
            }
            return PartialView("StorePriceListGrid", store.StorePriceLists);
        }

        public ActionResult StorePricelistDelete([ModelBinder(typeof(RetailModelBinder))] StorePriceList ct)
        {
            Store store = Session["EditingStore"] as Store;
            if (store != null)
            {
                StorePriceList spl = store.StorePriceLists.FirstOrDefault(g => g.Oid == ct.Oid);
                if (spl != null)
                {
                    spl.Delete();
                }
                //FillLookupComboBoxes();              
            }
            return PartialView("StorePriceListGrid", store.StorePriceLists);
        }

        public ActionResult StorePriceCatalogPolicyUpdate([ModelBinder(typeof(RetailModelBinder))] StorePriceCatalogPolicy model)
        {
            Store store = Session["EditingStore"] as Store;
            UpdateLookupObjects(model);

            if (store != null && model.PriceCatalogPolicy != null)
            {
                string modelKey;
                string modelError = StoreHelper.CheckedStorePriceCatalogPolicies(store, model, out modelKey);
                if (String.IsNullOrWhiteSpace(modelError))
                {
                    StorePriceCatalogPolicy storePriceCatalogPolicy = store.StorePriceCatalogPolicies.FirstOrDefault(storePolicy => storePolicy.Oid == model.Oid) ?? new StorePriceCatalogPolicy(store.Session);
                    storePriceCatalogPolicy.GetData(model, new List<string>() { "Session" });
                    storePriceCatalogPolicy.Store = store;
                }
                else
                {
                    ModelState.AddModelError(modelKey, modelError);
                    ViewBag.CurrentItem = model;
                    ViewBag.PriceCatalogPolicies = GetList<PriceCatalogPolicy>(XpoSession);
                }
            }
            return PartialView("StorePriceCatalogPoliciesGrid", store.StorePriceCatalogPolicies);
        }

        public ActionResult StorePriceCatalogPolicyDelete([ModelBinder(typeof(RetailModelBinder))] StorePriceCatalogPolicy model)
        {
            Store store = Session["EditingStore"] as Store;
            if (store != null)
            {
                StorePriceCatalogPolicy storePolicy = store.StorePriceCatalogPolicies.FirstOrDefault(storepolicy => storepolicy.Oid == model.Oid);
                if (storePolicy != null)
                {
                    storePolicy.Delete();
                }             
            }
            return PartialView("StorePriceCatalogPoliciesGrid", store.StorePriceCatalogPolicies);
        }

        public ActionResult UpdatePriceLists()
        {
            Store store = Session["EditingStore"] as Store;
            FillLookupComboBoxes();
            if (!(store.StorePriceCatalogPolicies.Select(stprcatalogpolicy => stprcatalogpolicy.PriceCatalogPolicy)).Contains(store.DefaultPriceCatalogPolicy))
            {
                store.DefaultPriceCatalogPolicy = null;               
            }
            return PartialView(store.DefaultPriceCatalogPolicy);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", ItemController.UploadControlValidationSettings, ImageUpload_FileUploadComplete);
            return null;
        }

        public static void ImageUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                HttpSessionState currentSession = System.Web.HttpContext.Current.Session;
                Store store = currentSession["EditingStore"] as Store;
                e.CallbackData = "success";

                if (System.Web.HttpContext.Current.Session["ImageStream"] != null)
                {
                    ((MemoryStream)currentSession["ImageStream"]).Dispose();
                    ((MemoryStream)currentSession["ImageStream"]).Close();
                }
                MemoryStream outMs = new MemoryStream(e.UploadedFile.FileBytes);
                currentSession["ImageStream"] = outMs; 
                Image uploadedImage = Image.FromStream(outMs);
                             
                if (store != null)
                {              
                    currentSession["StoreOwnerImage"]  = currentSession["StoreOwnerImage"] ?? new OwnerImage(store.Session);                  
                    (currentSession["StoreOwnerImage"] as OwnerImage).Image = uploadedImage;
                }
            }
        }

        public JsonResult jsonDeleteOwnerImage()
        {
            Store store = Session["EditingStore"] as Store;
            Session["StoreOwnerImage"] = null;
            store.ImageOid = Guid.Empty;
            return Json(new { success = true, StoreId = store == null ? Guid.Empty.ToString() : store.Oid.ToString() });
        }

        [Security(ReturnsPartial = false, DontLogAction = true)]
        [AllowAnonymous]
        public FileContentResult ShowImageId()
        {
            Store store = Session["EditingStore"] as Store;
            OwnerImage im = (store != null) ? Session["StoreOwnerImage"] as OwnerImage ?? store.Session.FindObject<OwnerImage>(new BinaryOperator("Oid", store.ImageOid)) : null;
            
            if (im != null)
            {
                int[] dimensions = new int[2];
                dimensions = ImageUtilities.calculateAspectRatioFit(im.Image.Width, im.Image.Height, 150, 150);
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(ImageUtilities.ResizeImage(im.Image, dimensions[0], dimensions[1]), typeof(byte[]));
                string format = "";

                if (im.Image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    format = "jpeg";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Gif)))
                {
                    format = "gif";
                }
                else if ((im.Image.RawFormat.Equals(ImageFormat.Png)))
                {
                    format = "png";
                }

                return new FileContentResult(imageBytes, "image/" + format);
            }
            return null;
        }

        public ActionResult CancelEdit()
        {
            Session["EditingStore"] = null;
            Session["StoreOwnerImage"] = null;
            return null;
        }
    }
}
