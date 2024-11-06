using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class ItemBarcodeController : BaseObjController<ItemBarcode>
    {

        UnitOfWork uow;

        protected void GenerateUnitOfWork()
        {

            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }


        public override ActionResult Dialog(List<string> arguments)
        {
            if (arguments != null)
            {
                if (arguments[0].ToUpper() == "SEASONALITY")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.Seasonality;

                    //-- The name of the partial to render in the Dialog
                    this.DialogOptions.BodyPartialView = "../Seasonality/SeasonalityEditForm";
                    this.DialogOptions.OKButton.OnClick = "SeasonalityDialogOkButton_OnClick";
                }
                else if (arguments[0].ToUpper() == "BUYER")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.Buyer;

                    this.DialogOptions.BodyPartialView = "../Buyer/BuyerEditForm";
                    this.DialogOptions.OKButton.OnClick = "BuyerDialogOkButton_OnClick";
                }
            }
            return PartialView();
        }

        CriteriaOperator filter = null;

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            Session["ItemBarcodeFilter"] = filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.OnClick = "ExportSelectedItems";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "ItemID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdItemBarcodes");
            this.CustomJSProperties.AddJSProperty("editController", "Item");
            this.ToolbarOptions.OptionsButton.Visible = MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER || UserHelper.IsCustomer(CurrentUser) == true ? false : true;
            if (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                List<PrintLabelSettings> printsettings = GetList<PrintLabelSettings>(XpoHelper.GetNewUnitOfWork(),
                                                         new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStore.Oid)).ToList();
                ViewBag.Labels = printsettings.OrderByDescending(criteria => criteria.IsDefault);
            }
            return View("Index", new XPQuery<ItemBarcode>(XpoHelper.GetNewUnitOfWork()).Where(g=>g.Oid==Guid.Empty));
        }


        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            Guid ItemID = Request["ItemID"] == null || Request["ItemID"] == "null" || Request["ItemID"] == "-1" ? Guid.Empty : Guid.Parse(Request["ItemID"]);

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                ViewData["CallbackMode"] = "SEARCH";

                if (Request.HttpMethod == "POST")
                {
                    string Fcode = Request["Fcode"] == null || Request["Fcode"] == "null" ? "" : Request["Fcode"];
                    string Fname = Request["Fname"] == null || Request["Fname"] == "null" ? "" : Request["Fname"];
                    string FBarcode = Request["Fbarcode"] == null || Request["Fbarcode"] == "null" ? "" : Request["Fbarcode"];
                    string Factive = Request["Factive"] == null || Request["Factive"] == "null" ? "" : Request["Factive"];
                    string Fcategory = Request["Fcategory"] == null || Request["Fcategory"] == "null" ? "" : Request["Fcategory"];
                    string FcreatedOn = Request["FcreatedOn"] == null || Request["FcreatedOn"] == "null" ? "" : Request["FcreatedOn"];
                    string FupdatedOn = Request["FupdatedOn"] == null || Request["FupdatedOn"] == "null" ? "" : Request["FupdatedOn"];
                    string FitemSupplier = Request["FitemSupplier"] == null || Request["FitemSupplier"] == "null" ? "" : Request["FitemSupplier"];
                    string Fbuyer = Request["Fbuyer"] == null || Request["Fbuyer"] == "null" ? "" : Request["Fbuyer"];
                    string Fseasonality = Request["Fseasonality"] == null || Request["Fseasonality"] == "null" ? "" : Request["Fseasonality"];
                    string Fmothercode = Request["Fmothercode"] == null || Request["Fmothercode"] == "null" ? "" : Request["Fmothercode"];

                    if (OwnerApplicationSettings != null)
                    {
                        if (OwnerApplicationSettings.PadBarcodes)
                        {
                            FBarcode = FBarcode != "" && !FBarcode.Contains("*") && !FBarcode.Contains("%") ? FBarcode.PadLeft(OwnerApplicationSettings.BarcodeLength, OwnerApplicationSettings.BarcodePaddingCharacter[0]) : FBarcode;
                        }

                        if (OwnerApplicationSettings.PadItemCodes)
                        {
                            Fcode = Fcode != "" && !Fcode.Contains("*") && !Fcode.Contains("%") ? Fcode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fcode;
                            Fmothercode = Fmothercode != "" && !Fmothercode.Contains("*") && !Fmothercode.Contains("%") ? Fmothercode.PadLeft(OwnerApplicationSettings.ItemCodeLength, OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : Fmothercode;
                        }

                        if (OwnerApplicationSettings.TrimBarcodeOnDisplay)
                        {
                            FBarcode = String.IsNullOrWhiteSpace(FBarcode) ? FBarcode : "%" + FBarcode.TrimStart(OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                            Fcode = String.IsNullOrWhiteSpace(Fcode) ? Fcode : "%" + Fcode.TrimStart(OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                            Fmothercode = String.IsNullOrWhiteSpace(Fmothercode) ? Fmothercode : "%" + Fmothercode.TrimStart(OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                        }
                    }


                    //if (ItemID != Guid.Empty)
                    //{
                    //    ViewBag.BarcodeComboBox = GetList<Barcode>(uow, CriteriaOperator.Parse("Item='" + ItemID.ToString() + "'", ""));
                    //    ViewData["PriceCatalogDetail"] = GetList<PriceCatalogDetail>(uow, CriteriaOperator.Parse("Item='" + ItemID.ToString() + "'", ""));
                    //    ViewData["ItemsOfMotherCode"] = GetList<Item>(uow, CriteriaOperator.Parse("MotherCode='" + ItemID.ToString() + "'", ""));
                    //}
                    //else
                    //{
                    //    ViewBag.BarcodeComboBox = GetList<Barcode>(uow, new ContainsOperator("ItemBarcodes",new BinaryOperator("Item.Oid", ItemID)));
                    //    ViewData["PriceCatalogDetail"] = GetList<PriceCatalogDetail>(uow, CriteriaOperator.Parse("Item='" + Guid.Empty + "'", ""));
                    //    ViewData["ItemsOfMotherCode"] = GetList<Item>(uow, CriteriaOperator.Parse("MotherCode='" + Guid.Empty + "'", ""));
                    //}


                    CriteriaOperator codeFilter = null;
                    if (Fcode != null && Fcode.Trim() != "")
                    {
                        if (Fcode.Replace('%', '*').Contains("*"))
                        {
                            codeFilter = new BinaryOperator("Item.Code", Fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            codeFilter = new BinaryOperator("Item.Code", Fcode, BinaryOperatorType.Equal);
                        }
                    }

                    CriteriaOperator nameFilter = null;
                    if (Fname != null && Fname.Trim() != "")
                    {
                        nameFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Name"), Fname);
                                                          // new BinaryOperator("Item.Name", "%" + Fname + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator barfilter = null;
                    if (FBarcode != null && FBarcode.Trim() != "")
                    {
                        if (FBarcode.Replace('%', '*').Contains("*"))
                        {
                           barfilter = new BinaryOperator("Barcode.Code", FBarcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            barfilter = new BinaryOperator("Barcode.Code", FBarcode);
                        }
                    }


                    CriteriaOperator activefilter = null;
                    if (Factive == "0" || Factive == "1")
                    {
                        activefilter = new BinaryOperator("Item.IsActive", Factive == "1");
                    }

                    CriteriaOperator categoryfilter = null;
                    if (Fcategory != "" && Fcategory != "-1")
                    {
                        Guid Fcategory_guid = Guid.Parse(Fcategory);
                        ItemCategory ic = uow.FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + Fcategory_guid + "'", ""));
                        categoryfilter = ic.GetAllNodeTreeFilter("Item.ItemAnalyticTrees");
                    }

                    CriteriaOperator createdOnFilter = null;
                    if (FcreatedOn != "")
                    {
                        createdOnFilter = CriteriaOperator.Or(new BinaryOperator("Item.InsertedDate", DateTime.Parse(FcreatedOn), BinaryOperatorType.GreaterOrEqual));
                    }

                    CriteriaOperator updatedOnFilter = null;
                    if (FupdatedOn != "")
                    {
                        updatedOnFilter = CriteriaOperator.Or(new BinaryOperator("Item.UpdatedOnTicks", DateTime.Parse(FupdatedOn).Ticks, BinaryOperatorType.GreaterOrEqual));
                    }

                    CriteriaOperator itemSupplierFilter = null;
                    if (FitemSupplier != null && FitemSupplier.Trim() != "")
                    {
                        itemSupplierFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.DefaultSupplier.CompanyName"), FitemSupplier);
                                                                  // new BinaryOperator("Item.DefaultSupplier.CompanyName", "%" + FitemSupplier + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator buyerFilter = null;
                    if (Fbuyer != null && Fbuyer.Trim() != "")
                    {
                        buyerFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Buyer.Description"), Fbuyer);
                                                                // new BinaryOperator("Item.Buyer.Description", "%" + Fbuyer + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator seasonalityFilter = null;
                    if (Fseasonality != null && Fseasonality.Trim() != "")
                    {
                        seasonalityFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Item.Seasonality.Description"), Fseasonality);
                                                                  // new BinaryOperator("Item.Seasonality.Description", "%" + Fseasonality + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator mothercodeFilter = null;
                    if (Fmothercode != null && Fmothercode.Trim() != "")
                    {
                        if (Fmothercode.Replace('%', '*').Contains("*"))
                        {
                            mothercodeFilter = new BinaryOperator("Item.MotherCode.Code", Fmothercode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            mothercodeFilter = new BinaryOperator("Item.MotherCode.Code", Fmothercode);
                        }
                    }

                    filter = ApplyOwnerCriteria(CriteriaOperator.And(codeFilter, nameFilter, barfilter, activefilter, categoryfilter, createdOnFilter, updatedOnFilter, itemSupplierFilter, buyerFilter, seasonalityFilter, mothercodeFilter), typeof(Item));
                    Session["ItemBarcodeFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["ItemBarcodeFilter"] = filter;
                }
            }
            else if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
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
                            DeleteAll(uow, oids);
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
            }
            else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
            {
                ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
            }
            else
            {
                //Session["ItemBarcodeFilter"] = Session["ItemBarcodeFilter"] == null ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "") : (CriteriaOperator)Session["ItemBarcodeFilter"];
            }

            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();            
            return PartialView("Grid",new XPQuery<ItemBarcode>(uow).AppendWhere(conv, (CriteriaOperator)Session["ItemBarcodeFilter"]));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return ExportToFile<ItemBarcode>(Session["ItemBarcodeGridSettings"] as GridViewSettings, (CriteriaOperator)Session["ItemBarcodeFilter"]);
        }

        public ActionResult PrintLabelPopUp()
        {
            GenerateUnitOfWork();
            Customer defaultCustomer = uow.GetObjectByKey<Customer>(StoreControllerAppiSettings.DefaultCustomerOid);
            if (defaultCustomer == null)
            {
                Session["Error"] = Resources.DefaultCustomerNotFound;
                return View("Error");
            }
            else
            {
                ViewBag.Labels = GetList<Label>(XpoSession);
                return PartialView();
            }
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                Guid ItemGuid = Guid.Empty;

                ItemBarcode ibc = XpoHelper.GetNewUnitOfWork().GetObjectByKey<ItemBarcode>(Guid.Parse(ViewData["ID"].ToString()));
                ItemGuid = ibc.Item.Oid;
                
                ViewData["ID"] = ibc.Item.Oid.ToString();

                if (ItemGuid != Guid.Empty)
                {
                    Item item = XpoHelper.GetNewUnitOfWork().FindObject<Item>(new BinaryOperator("Oid", ItemGuid));
                    ViewData["Code"] = item.Code;
                    ViewData["Name"] = item.Name;
                    ViewData["DefaultBarcode"] = item.DefaultBarcode == null ? "" : item.DefaultBarcode.Code;
                    ViewData["MotherCode"] = item.MotherCode == null ? "" : item.MotherCode.Code;
                    ViewData["ItemSupplier"] = item.DefaultSupplier == null ? "" : item.DefaultSupplier.CompanyName;
                    ViewData["VatCategory"] = item.VatCategory == null ? "" : item.VatCategory.Description;
                    ViewData["Buyer"] = item.Buyer == null ? "" : item.Buyer.Description;
                    ViewData["IsActive"] = item.IsActive;
                    ViewData["IsCentralStored"] = item.IsCentralStored;
                    ViewData["PackingQty"] = item.PackingQty;
                    ViewData["OrderQty"] = item.OrderQty;
                    ViewData["PackingMeasurementUnit"] = item.PackingMeasurementUnit == null ? "" : item.PackingMeasurementUnit.Description;
                    ViewData["MaxOrderQty"] = item.MaxOrderQty;
                    ViewData["InsertedOn"] = item.InsertedDate.ToString();
                    ViewData["ExtraDescription"] = item.ExtraDescription;
                    ViewData["Points"] = item.Points;
                    ViewData["ref_unit"] = item.ReferenceUnit;
                    ViewData["content_unit"] = item.ContentUnit;
                    ViewData["MinOrderQty"] = item.MinOrderQty;
                    ViewData["Remarks"] = item.Remarks;
                    ViewData["UpdatedOnTicks"] = item.UpdatedOnTicks;
                    ViewData["AcceptsCustomDescription"] = item.AcceptsCustomDescription;
                    ViewData["AcceptsCustomPrice"] = item.AcceptsCustomPrice;
                    ViewData["CustomPriceOptions"] = item.CustomPriceOptions.ToString();
                    ViewData["Seasonality"] = item.Seasonality == null ? "" : item.Seasonality.ToString();
                    ViewData["ExtraFilename"] = item.ExtraFilename == null ? "" : item.ExtraFilename.ToString();
                    ViewBag.OwnerApplicationSettings = item.Owner.OwnerApplicationSettings;
                }
                else
                {
                    ViewData["Code"] = "";
                    ViewData["Name"] = "";
                    ViewData["DefaultBarcode"] = "";
                    ViewData["MotherCode"] = "";
                    ViewData["ItemSupplier"] = "";
                    ViewData["VatCategory"] = "";
                    ViewData["Buyer"] = "";
                    ViewData["IsActive"] = false;
                    ViewData["IsCentralStored"] = false;
                    ViewData["PackingQty"] = "";
                    ViewData["OrderQty"] = "";
                    ViewData["PackingMeasurementUnit"] = "";
                    ViewData["MaxOrderQty"] = "";
                    ViewData["InsertedOn"] = "";
                    ViewData["ExtraDescription"] = "";
                    ViewData["Points"] = "";
                    ViewData["ref_unit"] = "";
                    ViewData["content_unit"] = "";
                    ViewData["MinOrderQty"] = "";
                    ViewData["Remarks"] = "";
                    ViewData["AcceptsCustomDescription"] = false;
                    ViewData["AcceptsCustomPrice"] = false;
                    ViewData["CustomPriceOptions"] = " ";
                    ViewData["Seasonality"] = " ";
                    ViewData["ExtraFilename"] = " ";
                    ViewData["UpdatedOnTicks"] = 0;
                }
            }

            ActionResult rt = PartialView("../Item/LoadViewPopup");
            return rt;
        }

        [Security(OverrideSecurity = true, ReturnsPartial = false)]
        public ActionResult ExportLabels()
        {
            if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
            {
                Session["Error"] = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description);
                return View("Index");
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string allOids = Request["ItemGuids"];
                string type = Request["Type"];

                if (!String.IsNullOrWhiteSpace(allOids) && !String.IsNullOrWhiteSpace(type))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();
                    foreach (string strOid in strOids)
                    {
                        Guid oid;
                        if(Guid.TryParse(strOid,out oid))
                        {
                            ItemBarcode itembarcode = uow.GetObjectByKey<ItemBarcode>(oid);
                            oids.Add(itembarcode.Barcode.Oid);
                        }

                    }

                    IEnumerable<PriceCatalogDetail> priceCatalogDetails =
                        PriceCatalogHelper.GetAllSortedPriceCatalogDetails(StoreControllerAppiSettings.CurrentStore, new InOperator("Barcode.Oid", oids));
                        //PriceCatalogHelper.GetTreePriceCatalogDetails(StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalog, new InOperator("Barcode.Oid", oids));

                    using (MemoryStream ms = new MemoryStream())
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> lines = GridExportHelper.GetLabelExportLines(priceCatalogDetails, type, EffectiveOwner);
                        if (lines == null)
                        {
                            Session["Error"] = Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                            return null;
                        }
                        if (lines == null)
                        {
                            Session["Error"] = Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                            return null;
                        }
                        foreach (string line in lines)
                        {
                            sr.WriteLine(line);
                        }
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "labels_export.txt");
                    }
                }
                else
                {
                    return null;
                }
            }

        }
    }
}
