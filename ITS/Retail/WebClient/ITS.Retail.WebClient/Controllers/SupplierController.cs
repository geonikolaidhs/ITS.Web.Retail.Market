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
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using ITS.Retail.Common.Helpers;
using ITS.Retail.WebClient.Attributes;
using System.Reflection;

namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    public class SupplierController : BaseObjController<SupplierNew>
    {

        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return new Dictionary<PropertyInfo, string>()
                {
                    { typeof(SupplierNew).GetProperty("VatLevel"), "VatLevel_VI" }
                };
            }
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                TraderHelper.LoadViewPopup<SupplierNew>(ViewData["ID"].ToString(), ViewData);
                FillLookupComboBoxes();
            }
            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }

        public override ActionResult Grid()
        {
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
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
                        catch (ConstraintViolationException)
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
                return PartialView(new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, 
                    ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));
            }


            if (Request["DXCallbackArgument"].Contains("SEARCH") == false)
            {

                if (Session["SupplierFilter"] == null) Session["SupplierFilter"] = new BinaryOperator("Oid", Guid.Empty);
                return PartialView("Grid", new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"], typeof(SupplierNew))));
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
                if (String.IsNullOrWhiteSpace(code))
                    codeFilter = null;
                else if (code.Replace('%', '*').Contains("*"))
                    codeFilter = new BinaryOperator("Code", code.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                else
                    codeFilter = CreateCriteria(code, "Code");

                if (String.IsNullOrWhiteSpace(name))
                {
                    nameFilter = lastNameFilter = companyNameFilter=null;
                }
                else
                {
                    nameFilter = new BinaryOperator("Trader.FirstName", "%" + name + "%", BinaryOperatorType.Like);
                    lastNameFilter = new BinaryOperator("Trader.LastName", "%" + name + "%", BinaryOperatorType.Like);
                    companyNameFilter = new BinaryOperator("CompanyName", "%" + name + "%", BinaryOperatorType.Like);
                }
                if (String.IsNullOrWhiteSpace(taxn))
                {
                    taxnFilter = null;
                }
                else
                {
                    taxnFilter = new BinaryOperator("Trader.TaxCode", "%" + taxn + "%", BinaryOperatorType.Like);
                }
                int j;
                activeFilter = null;
                if (Int32.TryParse(Request["is_active"].ToString(), out j))
                {
                    activeFilter = new BinaryOperator("IsActive", j==1);
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
            Session["SupplierFilter"] = criteria;
            Session["CurrentElement"] = ViewData["CurrentElement"];
            FillLookupComboBoxes();
            return PartialView(new XPQuery<SupplierNew>(XpoHelper.GetNewUnitOfWork()).AppendWhere(conv, ApplyOwnerCriteria((CriteriaOperator)Session["SupplierFilter"],typeof(SupplierNew))));

        }
        [Security(OverrideSecurity = true)]
        public ActionResult GetTraderDescription(string SupplierID)
        {
            TraderHelper.GetTraderDescription<SupplierNew>(SupplierID, ViewData, XpoSession);
            return PartialView("GetTraderDescription", XpoSession.FindObject<SupplierNew>(new BinaryOperator("Oid", ViewData["SupplierID"])));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.FilterButton.Visible = true;
            ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.VariableValuesButton.Visible = this.GetType().GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null;
            this.ToolbarOptions.VariableValuesButton.OnClick = "VariableValuesDisplay.ShowVariableValuesPopUp";

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "SupplierID");
            CustomJSProperties.AddJSProperty("gridName", "grdSupplier");


            FillLookupComboBoxes();
            Session["SupplierFilter"] = new BinaryOperator("Oid", Guid.Empty);
            return View(new List<SupplierNew>().AsQueryable());
        }


        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            if (Session["IsNewSupplier"] != null && (bool)Session["IsNewSupplier"])
            {

                ViewBag.Title = Resources.NewSupplier;
            }
            else
            {

                ViewBag.Title = Resources.EditSupplierDetails;
            }

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
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

            return PartialView();
        }

        public ActionResult PartialEditGrid(bool displayCommands, string SupplierID, bool editMode)
        {
            ViewData["displayCommands"] = displayCommands;
            ViewBag.EditMode = editMode;
            SupplierNew supplier;
            Guid SupplerGuID;
            if (!Guid.TryParse(SupplierID, out SupplerGuID))
            {
                Session["Error"] = Resources.AnErrorOccurred;
                return PartialView("../SharedTrader/PartialEditGrid", null);
            }

            if (!displayCommands)
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                supplier = uow.GetObjectByKey<SupplierNew>(SupplerGuID);
            }
            else
            {
                supplier = (SupplierNew)Session["Supplier"];
            }
            ViewData["SupplierID"] = SupplerGuID;
            ViewData["TraderID"] = supplier.Trader.Oid;
            return PartialView("../SharedTrader/PartialEditGrid", supplier);
        }
                
        public ActionResult Edit(string Oid)
        {

            if (!this.TableCanInsert && !this.TableCanUpdate)
            {
                return new RedirectResult("~/Login");
            }
            Guid sguid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);
            ViewData["OwnEdit"] = false;
            if (Oid == null)
            {
                ViewData["OwnEdit"] = true;
                XPCollection<SupplierNew> cust = BOApplicationHelper.GetUserEntities<SupplierNew>(XpoHelper.GetNewUnitOfWork(), CurrentUser);
                sguid = cust[0].Oid;
            }


            SupplierNew supplier;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            if (sguid != Guid.Empty)
            {
                if (!this.TableCanUpdate)
                {
                    return new RedirectResult("~/Login");
                }
                supplier = uow.GetObjectByKey<SupplierNew>(sguid);
                if (supplier != null)
                {
                    Session["IsNewSupplier"] = false;
                }
                else
                {
                    Customer customerFoundByCodeOrTaxCode = uow.GetObjectByKey<Customer>(sguid);
                    if (customerFoundByCodeOrTaxCode == null )
                    {
                        Session["Error"] = Resources.TraderNotFound;
                        return null;
                    }

                    supplier = new SupplierNew(uow);
                    supplier.Trader = customerFoundByCodeOrTaxCode.Trader;
                    supplier.CompanyName = customerFoundByCodeOrTaxCode.CompanyName;
                    supplier.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);

                    Session["IsNewSupplier"] = true;
                }
            }
            else
            {
                if (!this.TableCanInsert)
                {
                    return new RedirectResult("~/Login");
                }

                supplier = new SupplierNew(uow);

                CriteriaOperator cropVatLevel = CriteriaOperator.And(
                                                                new BinaryOperator("IsDefault", true),
                                                                new BinaryOperator("Owner", EffectiveOwner.Oid)
                                                            );
                supplier.VatLevel = supplier.Session.FindObject<VatLevel>(cropVatLevel);
                supplier.Trader = new Trader(uow);
                supplier.Trader.Suppliers.Add(supplier);
                Session["IsNewSupplier"] = true;
            }
            Session["NewTrader"] = true;
            Session["TraderType"] = "Supplier";
            Session["TraderUow"] = uow;
            Session["Supplier"] = supplier;
            Session["Trader"] = supplier.Trader;

            FillLookupComboBoxes();
            ViewData["SupplierID"] = supplier.Oid.ToString();
            ViewData["TraderID"] = supplier.Trader.Oid.ToString();
            return PartialView("Edit", supplier);
        }
        
        public JsonResult Save()
        {
            SupplierNew ct = null;

            try
            {
                Guid supplierGuid = Guid.Empty;
                if (Guid.TryParse(Request["Supplier_ID"], out supplierGuid))
                {
                    ct = XpoHelper.GetNewUnitOfWork().FindObject<SupplierNew>(new BinaryOperator("Oid", supplierGuid));
                    if (ct == null)
                    {
                        if (!this.TableCanInsert)
                        {
                            return Json(new { error = Resources.AnErrorOccurred });
                        }
                    }
                    else
                    {
                        if (!this.TableCanUpdate)
                        {
                            return Json(new { error = Resources.AnErrorOccurred });
                        }
                    }
                    ct = (SupplierNew)Session["Supplier"];

                    TryUpdateModel<SupplierNew>(ct);
                    TryUpdateModel<Trader>(ct.Trader);
                    UpdateLookupObjects(ct);
                    Guid defAddress;
                    if (Guid.TryParse(Request.Params["DefaultAddress_VI"], out defAddress))
                    {
                        ct.DefaultAddress = ct.Trader.Addresses.FirstOrDefault(x => x.Oid == defAddress);
                    }

                    if (ct.Trader.Addresses.Count == 0)
                    {
                        Session["Error"] = Resources.NoAddressInserted;
                        ViewData["SupplierID"] = ct.Oid.ToString();
                        ViewData["TraderID"] = ct.Trader.Oid.ToString();
                        ViewData["Customer_ID"] = Request["Customer_ID"];
                        FillLookupComboBoxes();
                        return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
                    }

                    AssignOwner(ct);
                    ct.Save();
                    XpoHelper.CommitTransaction(ct.Session);
                    Session["NewTrader"] = null;
                    Session["TraderType"] = null;
                    Session["TraderUow"] = null;
                    Session["Supplier"] = null;
                    Session["Trader"] = null;
                }
                SessionHelper.ReloadSessionCommonItems();
                return Json(new { });
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.GetFullMessage();
                if (CheckIfCodeExists(ct))
                {
                    Session["Error"] = Resources.CodeAlreadyExists;
                }
                ViewData["OwnEdit"] = false;
                ViewData["SupplierID"] = ct.Oid;
                ViewData["TraderID"] = ct.Trader == null ? Guid.Empty : ct.Trader.Oid;
                FillLookupComboBoxes();
                return Json(new { reloadEdit = true, data = ct, error = Session["Error"] });
            }

        }

        private bool CheckIfCodeExists(SupplierNew supplier)
        {
            SupplierNew suppl = supplier.Session.FindObject<SupplierNew>(CriteriaOperator.And(
                                                                            new BinaryOperator("Code", supplier.Code),
                                                                            new BinaryOperator("Oid",supplier.Oid,BinaryOperatorType.NotEqual)
                                                                         ));
            return suppl != null;
        }


        
        public ActionResult AssociateNewSupplierWithTrader(string TraderID)
        {
            if (!this.TableCanInsert)
                return new RedirectResult("~/Login");

            SupplierNew ct;

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Guid TraderGuid;
            if (!Guid.TryParse(TraderID, out TraderGuid))
                return View("Edit", Session["Customer"] as Customer);
            Trader t = uow.FindObject<Trader>(new BinaryOperator("Oid", TraderGuid));
            if (t == null)
                return View("Edit", Session["Customer"] as Customer);
            ct = new SupplierNew(uow);
            if (t.Customers.Count > 0)
            {
                ct.CompanyName = t.Customers.First().CompanyName;
                ct.Profession = t.Customers.First().Profession;
                ct.DefaultAddress = t.Customers.First().DefaultAddress;
            }
            t.Suppliers.Add(ct);
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
            return PartialView("UpdateAddressComboBox", ((SupplierNew)Session["Supplier"]));
        }

        public ActionResult CancelEdit()
        {
            Session["NewTrader"] = null;
            Session["Trader"] = null;
            Session["Supplier"] = null;
            return null;
        }

        public JsonResult UpdateTraderData(string sender, string Code, string TaxCode)
        {
            SupplierNew supplier = null;
            Customer customer = null;
            Trader trader = null;
            Trader currentObjectTrader = (Session["Trader"] as Trader);
            try
            {
                UnitOfWork uow = currentObjectTrader.Session as UnitOfWork;
                if (sender == "TaxCode")
                {
                    trader = uow.FindObject<Trader>(new BinaryOperator("TaxCode", TaxCode));
                    if (trader != null)
                    {
                        supplier = trader.Suppliers.FirstOrDefault();
                        if (supplier == null)
                        {
                            customer = trader.Customers.Where(custome => custome.Owner.Oid == EffectiveOwner.Oid).FirstOrDefault();
                        }
                    }
                }
                else if (sender == "Code")
                {
                    supplier = uow.FindObject<SupplierNew>(new BinaryOperator("Code", Code));
                    if (supplier != null)
                    {
                        trader = supplier.Trader;
                    }
                    else
                    {
                        customer = uow.FindObject<Customer>(new BinaryOperator("Code", Code));
                        if ( customer != null)
                        {
                            trader = customer.Trader;
                        }
                    }
                }

                if (trader == null)
                {
                    return Json(new { Error = "", NoDuplicateFound = true });
                }
                else if (trader.Oid == currentObjectTrader.Oid)
                {
                    return Json(new {Error = "", NoDuplicateFound = true });
                }
                else {
                string confirm_message = Resources.TraderFoundWithSameCode + Environment.NewLine + trader.Code + Environment.NewLine
                                            + String.Format(Resources.WouldYouLikeToOpenTheCurrentTrader, Resources.Trader);
                    return Json(new
                    {
                        TraderID = trader.Oid,
                        TraderFirstName = trader.FirstName,
                        TraderLastName = trader.LastName,
                        confirm_message = confirm_message,
                        triggered_by = sender,
                        supplier_id = supplier == null ? string.Empty : supplier.Oid.ToString(),
                        customer_id = customer == null ? string.Empty : customer.Oid.ToString(),
                        controller = "supplier"
                    });
                }
            }
            catch(Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
                return Json(new { Error = "", NoDuplicateFound = true });
            }          
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.AddressComboBox = Session["Trader"] != null ? ((Trader)Session["Trader"]).Addresses : null;
            ViewBag.SupplierComboBox = GetList<SupplierNew>(XpoSession).OrderBy(supplier => supplier.CompanyName);
            ViewBag.VatLevelComboBox = GetList<VatLevel>(XpoSession);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return base.ExportToFile<Customer>(Session["SupplierGridSettings"] as GridViewSettings, (CriteriaOperator)Session["SupplierFilter"]);
        }

        public ActionResult SelectTaxOffice()
        {
            return PartialView();
        }

        public static object TaxOfficeRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            if (e.Filter == "") { return null; }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            SupplierNew supplier = (System.Web.HttpContext.Current.Session["Supplier"] as SupplierNew);
            if (supplier != null)
            {
                UnitOfWork uow = supplier.Session as UnitOfWork;
                CriteriaOperator crop = CriteriaOperator.Or(new BinaryOperator("Description", proccessed_filter, BinaryOperatorType.Like),
                                                            new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like));

                XPCollection<TaxOffice> searched_item_taxoffices = GetList<TaxOffice>(uow, crop, "Code");

                searched_item_taxoffices.SkipReturnedObjects = e.BeginIndex;
                searched_item_taxoffices.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

                return searched_item_taxoffices;
            }
            return null;
        }
    }
}
