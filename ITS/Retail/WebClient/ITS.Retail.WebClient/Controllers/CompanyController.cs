using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Xpo.DB.Exceptions;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.Helpers;
using System.Diagnostics;

namespace ITS.Retail.WebClient.Controllers
{
    public class CompanyController : BaseObjController<CompanyNew>
    {
        public override ActionResult Grid()
        {
            ViewData["CallbackMode"] = "";
            if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                ViewData["CallbackMode"] = "DELETESELECTED";
                if (TableCanDelete)
                {
                    List<Guid> oids = new List<Guid>();
                    string allOids = Request["DXCallbackArgument"].Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
                    string[] unparsed = allOids.Split(',');
                    foreach (string unparsedOid in unparsed)
                    {
                        oids.Add(Guid.Parse(unparsedOid));
                    }
                    if (oids.Count > 0)
                    {
                        try
                        {
                            DeleteAll(XpoSession, oids);
                        }
                        catch (ConstraintViolationException )
                        {
                            Session["Error"] = Resources.CannotDeleteObject;
                        }
                        catch (Exception e)
                        {
                            Session["Error"] = e.Message;
                        }
                    }
                }
                else
                {
                    Session["Error"] = Resources.AnErrorOccurred;
                }
                return PartialView("Grid", GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["CompanyFilter"]));
            }


            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {

                if (Session["CompanyFilter"] == null)
                {
                    Session["CompanyFilter"] = new BinaryOperator("Oid", Guid.Empty);
                }
                return PartialView("Grid", GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["CompanyFilter"]));
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }

            CriteriaOperator criteria;
            if (Request.HttpMethod == "POST")
            {
                ViewData["CallbackMode"] = "SEARCH";
                string code = (Request["supplier_code"] == null || Request["supplier_code"] == "null") ? "" : Request["supplier_code"];
                string name = (Request["supplier_name"] == null || Request["supplier_name"] == "null") ? "" : Request["supplier_name"];
                string taxn = (Request["supplier_tax_number"] == null || Request["supplier_tax_number"] == "null") ? "" : Request["supplier_tax_number"];
                string lcod = (Request["loyalty_code"] == null || Request["loyalty_code"] == "null") ? "" : Request["loyalty_code"];

                CriteriaOperator codeFilter, nameFilter, lastNameFilter, taxnFilter, activeFilter, companyNameFilter;
                if (code.Replace('%', '*').Contains("*"))
                {
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%'), BinaryOperatorType.Like);
                }
                else
                {
                    codeFilter = CreateCriteria(code, "Code");
                }

                nameFilter = String.IsNullOrWhiteSpace(name)
                             ? null
                             : new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.FirstName"), name);
                                //new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                lastNameFilter = String.IsNullOrWhiteSpace(name)
                                 ? null
                                 : new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.LastName"), name);
                                //new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                companyNameFilter = String.IsNullOrWhiteSpace(name)
                                    ? null
                                    : new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), name);
                                //new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                taxnFilter = String.IsNullOrWhiteSpace(taxn)
                             ? null
                             : new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Trader.TaxCode"), taxn);
                                // new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);

                int is_active;
                activeFilter = null;
                if (Int32.TryParse(Request["is_active"].ToString(), out is_active))
                {
                    activeFilter = new BinaryOperator("IsActive", is_active);
                }

                criteria = CriteriaOperator.And(codeFilter,
                                                CriteriaOperator.Or(nameFilter, lastNameFilter, companyNameFilter),
                                                taxnFilter,
                                                CreateCriteria(lcod, "Loyalty"),
                                                activeFilter
                                                );
                if (ReferenceEquals(criteria, null))
                {
                    criteria = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.NotEqual);
                }


            }
            else
            {
                criteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");

            }
            String CompanyID = Request.Params["CompanyID"];

            Guid CompanyGuid;
            if (CompanyID == "-1")
            {
                CompanyGuid = Guid.Empty;
            }
            else
            {
                CompanyGuid = CompanyID != null && CompanyID != "null" ? Guid.Parse(CompanyID) : Guid.Empty;
            }

            Session["CompanyFilter"] = criteria;
            Session["CurrentElement"] = ViewData["CurrentElement"];
            FillLookupComboBoxes();
            XPCollection<CompanyNew> tmp = GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["CompanyFilter"]);
            return PartialView("Grid", tmp);

        }
        
        [Security(OverrideSecurity=true)]
        public ActionResult GetTraderDescription(string CompanyID)
        {
            TraderHelper.GetTraderDescription<CompanyNew>(CompanyID, ViewData, XpoSession);
            return PartialView("GetTraderDescription", XpoSession.FindObject<CompanyNew>(new BinaryOperator("Oid", ViewData["CompanyID"])));
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                TraderHelper.LoadViewPopup<CompanyNew>(ViewData["ID"].ToString(), ViewData);
            }

            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }


        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditCompany;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            if (EffectiveOwner == null)
            {
                Session["Error"] = ResourcesLib.Resources.SelectCompany;
                return new RedirectResult("~/Home/Index");
            }

            this.ToolbarOptions.NewButton.Visible = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL;
            ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.FilterButton.Visible = true;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "Component.ExternalAdd";
            this.ToolbarOptions.EditButton.OnClick = "Component.ExternalEdit";

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "CompanyID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdCompany");

            FillLookupComboBoxes();
            Session["CompanyFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
            return View("Index", GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["CompanyFilter"]).AsEnumerable<CompanyNew>());
        }

        
        public ActionResult Edit(string Oid)
        {
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.FilterButton.Visible = false;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;

            if (!this.TableCanInsert && !this.TableCanUpdate)  return new RedirectResult("~/Login");
            Guid sguid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);
            ViewData["OwnEdit"] = false;
            if (Oid == null)
            {
                ViewData["OwnEdit"] = true;
                XPCollection<CompanyNew> cust = BOApplicationHelper.GetUserEntities<CompanyNew>(XpoSession, CurrentUser);
                sguid = cust[0].Oid;
            }


            CompanyNew ct;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            if (sguid != Guid.Empty)
            {
                if (!this.TableCanUpdate)
                    return new RedirectResult("~/Login");
                ct = uow.FindObject<CompanyNew>(new BinaryOperator("Oid", sguid, BinaryOperatorType.Equal));
                Session["IsNewSupplier"] = false;
            }
            else
            {
                if (!this.TableCanInsert)
                    return new RedirectResult("~/Login");

                ct = new CompanyNew(uow);
                ct.Trader = new Trader(uow);
                ct.Trader.Companies.Add(ct);
                OwnerApplicationSettings settings = new OwnerApplicationSettings(uow);
                settings.Owner = ct;
                Session["IsNewSupplier"] = true;
            }
            Session["NewTrader"] = true;
            Session["TraderType"] = "Supplier";
            Session["TraderUow"] = uow;
            Session["Company"] = ct;
            Session["Trader"] = ct.Trader;
            Session["UnsavedOwnerImage"] = ct==null || ct.OwnerApplicationSettings == null ? null : uow.FindObject<OwnerImage>(new BinaryOperator("Oid", ct.OwnerApplicationSettings.OwnerImageOid));
            
            FillLookupComboBoxes();
            ViewData["CompanyID"] = ct.Oid.ToString();
            ViewData["TraderID"] = ct.Trader.Oid.ToString();
            return View("Edit", ct);

        }


        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult UpdateProfile()
        {
            this.ToolbarOptions.ForceVisible = false;

            CompanyNew sup = UserHelper.GetCompany(CurrentUser);
            ViewData["CompanyID"] = sup == null ? Guid.Empty : sup.Oid;
            ViewData["TraderID"] = sup == null ? Guid.Empty : sup.Trader.Oid;
            return View();
        }

        public ActionResult LoadAssosiatedSupplierEditPopup()
        {
            Guid TraderID = Guid.Empty;

            if (Guid.TryParse(Request["TraderID"], out TraderID))
            {
                ViewData["TraderID"] = TraderID;
            }

            ViewBag.Title = Resources.EditSupplierDetails;

            ActionResult rt = PartialView("../Supplier/LoadAssosiatedSupplierEditPopup");
            return rt;
        }

        public ActionResult PopupAssosiatedEditCallbackPanel()
        {
            Guid TraderID = Guid.Empty;

            if (Guid.TryParse(Request["TraderID"], out TraderID))
            {
                ViewData["TraderID"] = TraderID;
            }

            return PartialView("../Supplier/PopupAssosiatedEditCallbackPanel");
        }
        
        public ActionResult Save()
        {
            bool isOwnEdit = false;
            Guid cguid = Guid.Empty;

            bool correctCustomerGuid = Request["Supplier_ID"] != null && Guid.TryParse(Request["Supplier_ID"].ToString(), out cguid);
            ViewData["OwnEdit"] = Request["isOwnEdit"];
            Boolean.TryParse(Request["isOwnEdit"], out isOwnEdit);

            if (correctCustomerGuid)
            {
                CompanyNew company = XpoHelper.GetNewUnitOfWork().FindObject<CompanyNew>(new BinaryOperator("Oid", cguid));
                if (company == null)
                {
                    if (!this.TableCanInsert) return Json(new { error = Resources.AnErrorOccurred });
                        
                }
                else
                {
                    if (!this.TableCanUpdate) return Json(new { error = Resources.AnErrorOccurred });
                }
                UnitOfWork uow2 = Session["TraderUow"] as UnitOfWork;
                company = (CompanyNew)Session["Company"];


                if (!isOwnEdit)
                {
                    company.Code = Request["Code"];
                    company.IsActive = !(Request["IsActive"].ToString().ToUpper() == "U");
                }
                company.Trader.FirstName = Request["FirstName"];
                company.Trader.LastName = Request["LastName"];
                company.Trader.TaxCode = Request["TaxCode"];
                company.CompanyName = Request["CompanyName"];
                Guid taxofficeGuid = Guid.Empty;
                if (Guid.TryParse(Request["TaxOfficeLookUpOid_VI"], out taxofficeGuid))
                {
                    company.Trader.TaxOfficeLookUpOid = taxofficeGuid;
                }
                company.Profession = Request["Profession"];
                company.B2CURL = Request["B2CURL"];
                Guid defAddress = (Request.Params["DefaultAddress_VI"] == null || Request.Params["DefaultAddress_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["DefaultAddress_VI"]);
                company.DefaultAddress = null;
                foreach (Address ad in company.Trader.Addresses)
                {
                    if (ad.Oid == defAddress)
                    {
                        company.DefaultAddress = ad;
                        break;
                    }
                }
                OwnerApplicationSettings ownerApplicationSettings;
                if (company.OwnerApplicationSettings == null)
                {
                    ownerApplicationSettings = new OwnerApplicationSettings(uow2);
                    ownerApplicationSettings.Owner = company;
                }
                else
                {
                    ownerApplicationSettings = company.OwnerApplicationSettings;
                }

                string errorMessage = string.Empty;
                bool ownerApplicationSettingsHaveBeenFilled = OwnerApplicationSettingsController.FillOwnerApplicationSettings(ownerApplicationSettings, Request,out errorMessage);
                if (!ownerApplicationSettingsHaveBeenFilled)
                {
                    Session["Error"] = errorMessage;
                    return new RedirectResult("~/Company");
                }
                
                OwnerImage ownerImage = (OwnerImage)Session["UnsavedOwnerImage"];
                if (ownerImage != null)
                {
                    ownerImage.OwnerApplicationSettingsOid = ownerApplicationSettings.Oid;
                    ownerApplicationSettings.OwnerImageOid = ownerImage.Oid;
                    ownerImage.Save();
                }
                               
                ownerApplicationSettings.Save();
                company.Save();
                XpoHelper.CommitTransaction(uow2);
                Session["NewTrader"] = null;
                Session["TraderType"] = null;
                Session["TraderUow"] = null;
                Session["Company"] = null;
                Session["Trader"] = null;
                PrepareLoggedInUserVariables();
            }

            SessionHelper.ReloadSessionCommonItems();
            UpdateCurrentUserSettings();            

            if (isOwnEdit)
            {               
                return Json(new { error = Resources.AnErrorOccurred });
            }

            return new RedirectResult("~/Company");
        }

        
        public ActionResult AssociateNewSupplierWithTrader(string TraderID)
        {
            if (!this.TableCanInsert)
                return new RedirectResult("~/Login");

            CompanyNew ct;

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Guid TraderGuid;
            if (!Guid.TryParse(TraderID, out TraderGuid))
                return View("Edit", Session["Customer"] as Customer);
            Trader t = uow.FindObject<Trader>(new BinaryOperator("Oid", TraderGuid));
            if (t == null)
                return View("Edit", Session["Customer"] as Customer);
            ct = new CompanyNew(uow);
            if(t.Customers.Count>0){
                ct.CompanyName = t.Customers.First<Customer>().CompanyName;
                ct.Profession = t.Customers.First<Customer>().Profession;
                ct.DefaultAddress = t.Customers.First<Customer>().DefaultAddress;
            }
            t.Companies.Add(ct);
            Session["NewTrader"] = true;
            Session["TraderType"] = "Customer";
            Session["TraderUow"] = uow;
            Session["Supplier"] = ct;
            Session["Trader"] = ct.Trader;
            ViewData["SupplierID"] = ct.Oid;
            ViewData["TraderID"] = t.Oid;
            FillLookupComboBoxes();
            ViewData["OwnEdit"] = false;
            return PartialView("Edit", ct);
        }

        public ActionResult UpdateAddressComboBox(string TraderID)
        {
            FillLookupComboBoxes();
            Trader trader = Session["NewTrader"] == null || (bool)Session["NewTrader"] == false ? null : Session["Trader"] as Trader;
            ViewData["TraderID"] = TraderID;
            return PartialView("UpdateAddressComboBox", Session["Company"]);
        }


        public ActionResult GetSupplierStores()
        {
            string splid = Request["SupplierGUID"];
            Guid splguid;
            if (Guid.TryParse(splid, out splguid) == false)
                splguid = Guid.Empty;
            CompanyNew sup = this.XpoSession.FindObject<CompanyNew>(new BinaryOperator("Oid", splguid));
            ViewBag.StoreComboBox = (sup == null) ? null : sup.Stores.OrderBy(store => store.Name);
            ViewData["OwnEdit"] = false;
            return PartialView();
        }


        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] CompanyNew ct)
        {
            if (!this.TableCanDelete)
                return null;
            try
            {
                Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("Grid", GetList<CompanyNew>(XpoHelper.GetNewUnitOfWork(), Session["SupplierFilter"] as CriteriaOperator).AsEnumerable<CompanyNew>());
        }


        public ActionResult CancelEdit()
        {
            Session["NewTrader"] = null;
            Session["Trader"] = null;
            Session["Supplier"] = null;

            return new RedirectResult("~/Company");
        }

        public JsonResult UpdateTraderData(string TaxCode)
        {
            Trader trader = XpoSession.FindObject<Trader>(new BinaryOperator("TaxCode", TaxCode));
            if (trader == null)
            {
                return Json(new { Error = "" });  
            }
            else
            {
                CompanyNew company = trader.Companies.FirstOrDefault();
                if (company != null)
                {
                    String confirm_message = Resources.CustomerFoundWithSameCode + "\n" + company.CompanyName + "\n"
                     + String.Format(Resources.WouldYouLikeToOpenTheCurrentTrader, Resources.Supplier);
                    return Json(new
                    {
                        TraderID = company.Trader.Oid,
                        TraderFirstName = company.Trader.FirstName,
                        TraderLastName = company.Trader.LastName,
                        confirm_message = confirm_message,
                        supplier_id = company.Oid,
                    });
                }
                else
                {
                    return Json(new { Error = "" });
                }

            }
        }

        public ActionResult PartialEditGrid(bool displayCommands, String CompanyID, bool editMode)
        {
            ViewData["displayCommands"] = displayCommands;
            ViewBag.EditMode = editMode;
            CompanyNew company;
            Guid CompanyGuID;
            if (!Guid.TryParse(CompanyID, out CompanyGuID))
            {
                Session["Error"] = Resources.AnErrorOccurred;
                return PartialView("../SharedTrader/PartialEditGrid", null);
            }

            if (!displayCommands)
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                company = uow.GetObjectByKey<CompanyNew>(CompanyGuID);
            }
            else
            {
                company = (CompanyNew)Session["Company"];
            }
            ViewData["CompanyID"] = CompanyGuID;
            ViewData["TraderID"] = company.Trader.Oid;
            return PartialView("../SharedTrader/PartialEditGrid", company);
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.AddressComboBox = Session["Trader"] != null ? ((Trader)Session["Trader"]).Addresses : null;
            ViewBag.SupplierComboBox = GetList<CompanyNew>(XpoSession).OrderBy(supplier => supplier.CompanyName);
            ViewBag.Fonts = RetailHelper.GetAvailableFonts(Server.MapPath("~/Content/B2C/customize/fonts"));

            ViewBag.PointsDocumentTypes = GetList<DocumentType>(XpoSession, new BinaryOperator("Division.Section", eDivision.Other));
            ViewBag.PointsDocumentSeries = GetList<DocumentSeries>(XpoSession, new ContainsOperator("StoreDocumentSeriesTypes",
                new BinaryOperator("DocumentType.Division.Section", eDivision.Other)));
            ViewBag.PointsDocumentStatus = GetList<DocumentStatus>(XpoSession);
            ViewBag.GDPRReports = GetList<CustomReport>(XpoSession);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return ExportToFile<Customer>(Session["SupplierGridSettings"] as GridViewSettings, (CriteriaOperator)Session["SupplierFilter"]);
        }

        public ActionResult ReportsComboBox()
        {
           ViewData["Name"] = Request["name"];
            return PartialView();
        }
        public ActionResult SelectTaxOffice()
        {
            return PartialView();
        }
        public static object TaxOfficeRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (e.Filter == "")
            {
                return null;
            }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            CompanyNew company = (System.Web.HttpContext.Current.Session["Company"] as CompanyNew);
            if (company != null)
            {
                UnitOfWork uow = company.Session as UnitOfWork;
                CriteriaOperator crop = CriteriaOperator.Or(new BinaryOperator("Description", proccessed_filter, BinaryOperatorType.Like),
                                                            new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like));

                XPCollection<TaxOffice> searched_item_taxoffices = GetList<TaxOffice>(uow, crop, "Code");

                searched_item_taxoffices.SkipReturnedObjects = e.BeginIndex;
                searched_item_taxoffices.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

                return searched_item_taxoffices;
            }
            return null;
        }
        public static object CustomReportMethodRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<CustomReport> collection = GetList<CustomReport>(XpoHelper.GetNewUnitOfWork(),
                                                     CriteriaOperator.Or(new BinaryOperator("Title", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                         new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)),
                                                     "Title");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public static object CustomReportMethodRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(Guid))
            {
                CustomReport obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CustomReport>(e.Value);
                return obj;
            }
            return null;
        }
    }
}
