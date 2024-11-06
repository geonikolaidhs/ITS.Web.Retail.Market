using System;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class OfferController : BaseObjController<Offer>
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

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {

            GenerateUnitOfWork();

            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
			Session["OfferFilter"] = filter;

            //this.ToolbarOptions.Visible = true;
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "Component.ShowPopup";
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.PrintButton.Visible = this.TableCanExport;

            if (UserHelper.IsCustomer(CurrentUser))
            {
                this.ToolbarOptions.OptionsButton.Visible = false;
            }
            this.ToolbarOptions.PrintButton.OnClick = "ExportSelectedOffers";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "OfferID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdOffers");

            return View("Index", GetList<Offer>(uow, filter).AsEnumerable<Offer>());


        }


        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            CriteriaOperator filter = null;

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                ViewData["CallbackMode"] = "SEARCH";
                if (Request.HttpMethod == "POST")
                {
                    string fcode = Request["fcode"] == null || Request["fcode"] == "null" ? "" : Request["fcode"];
                    string fdescription = Request["fdescription"] == null || Request["fdescription"] == "null" ? "" : Request["fdescription"];
                    string fpriceCatalog = Request["fpriceCatalog"] == null || Request["fpriceCatalog"] == "null" ? "" : Request["fpriceCatalog"];
                    string fstartDate = Request["fstartDate"] == null || Request["fstartDate"] == "null" ? "" : Request["fstartDate"];
                    string fendDate = Request["fendDate"] == null || Request["fendDate"] == "null" ? "" : Request["fendDate"];

                    CriteriaOperator codeFilter = null;
                    if (fcode != null && fcode.Trim() != "")
                    {
                        if (fcode.Replace('%', '*').Contains("*"))
                        {
                            codeFilter = new BinaryOperator("Code", fcode.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            codeFilter = new BinaryOperator("Code", fcode, BinaryOperatorType.Equal);
                        }
                    }

                    CriteriaOperator descriptionFilter = null;
                    if (fdescription != null && fdescription.Trim() != "")
                    {
                        descriptionFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), fdescription);
                                                                    // new BinaryOperator("Description", "%" + fdescription + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator description2Filter = null;
                    if (fdescription != null && fdescription.Trim() != "")
                    {
                        description2Filter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description2"), fdescription);
                                                                    // new BinaryOperator("Description2", "%" + fdescription + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator priceCatalogFilter = null;
                    if (fpriceCatalog != null && fpriceCatalog.Trim() != "")
                    {
                        priceCatalogFilter = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("PriceCatalog.Code"), fpriceCatalog),
                                                                 new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("PriceCatalog.Description"), fpriceCatalog)
                                                                 //new BinaryOperator("PriceCatalog.Code", "%" + fpriceCatalog + "%", BinaryOperatorType.Like),
                                                                 //new BinaryOperator("PriceCatalog.Description", "%" + fpriceCatalog + "%", BinaryOperatorType.Like)
                                                                );
                    }


                    CriteriaOperator startDateFilter = null;
                    if (fstartDate != "")
                    {
                        startDateFilter = new BinaryOperator("EndDate", DateTime.Parse(fstartDate), BinaryOperatorType.GreaterOrEqual);
                    }

                    CriteriaOperator endDateFilter = null;
                    if (fendDate != "")
                    {
                        endDateFilter = new BinaryOperator("StartDate", DateTime.Parse(fendDate), BinaryOperatorType.LessOrEqual);
                    }


                     

                    CriteriaOperator dateFilter = null;
                    if (UserHelper.IsCustomer(CurrentUser) || UserHelper.IsCompanyUser(CurrentUser))
                    {
                        DateTime now = DateTime.Now;

                        dateFilter = CriteriaOperator.And(new BinaryOperator("StartDate", now, BinaryOperatorType.LessOrEqual),
                                                                     new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual));
                    }

                    filter = CriteriaOperator.And(codeFilter, descriptionFilter, description2Filter, startDateFilter, endDateFilter, priceCatalogFilter,dateFilter);
                    Session["OfferFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["OfferFilter"] = filter;
                }
            }
            this.GridFilter = (CriteriaOperator)Session["OfferFilter"];
            return base.Grid();
            
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditOffer;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }
                        
        public ActionResult EditView(string Oid)
        {
            GenerateUnitOfWork();
            Guid offerGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            if (offerGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (offerGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            Offer offer;
            ViewData["EditMode"] = true;
            if (Session["UnsavedOffer"] == null)
            {
                if (offerGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditOffer;
                    offer = uow.FindObject<Offer>(new BinaryOperator("Oid", offerGuid, BinaryOperatorType.Equal));
                    Session["IsNewOffer"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewOffer;
                    offer = new Offer(uow);
                    Session["IsNewOffer"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (offerGuid != Guid.Empty && (Session["UnsavedOffer"] as Offer).Oid == offerGuid)
                {
                    Session["IsRefreshed"] = true;
                    offer = (Offer)Session["UnsavedOffer"];
                }
                else if (offerGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    offer = (Offer)Session["UnsavedOffer"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    offer = uow.FindObject<Offer>(new BinaryOperator("Oid", offerGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["OfferID"] = offer.Oid.ToString();
            Session["UnsavedOffer"] = offer;
            //ViewData["OfferDetail"] = offer.OfferDetails;
            //ViewData["TraderID"] = ct.Trader.Oid.ToString();
            return PartialView("EditView", offer);
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid offerGuid = Guid.Empty;

            bool correctOfferGuid = Request["OfferID"] != null && Guid.TryParse(Request["OfferID"].ToString(), out offerGuid);
            if (correctOfferGuid)
            {
                Offer offer = (Offer)Session["UnsavedOffer"];
                if (offer != null)
                {
                    if ((bool)Session["IsNewOffer"])
                    {
                        AssignOwner(offer);
                    }

                    if (String.IsNullOrWhiteSpace(Request["Code"]))
                    {
                        Session["Error"] = String.Format(Resources.RequiredFieldError, Resources.Code);
                        return Json(new { error = Session["Error"].ToString() });
                    }

                    if (String.IsNullOrWhiteSpace(Request["Description"]))
                    {
                        Session["Error"] = String.Format(Resources.RequiredFieldError, Resources.Description);
                        return Json(new { error = Session["Error"].ToString() });
                    }


                    if (String.IsNullOrWhiteSpace(Request.Params["PriceCatalog_VI"]))
                    {
                        Session["Error"] = String.Format(Resources.RequiredFieldError, Resources.PriceCatalog);
                        return Json(new { error = Session["Error"].ToString() });
                    }

                    offer.Code = Request["Code"];
                    offer.Description = Request["Description"];
                    offer.PriceCatalog = uow.FindObject<PriceCatalog>(new BinaryOperator("Oid", (Request.Params["PriceCatalog_VI"] == null || Request.Params["PriceCatalog_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["PriceCatalog_VI"])));

                    offer.Description2 = Request["Description2"];
                    if (Request["StartDate"] != null && Request["StartDate"] != "")
                    {
                        offer.StartDate = DateTime.Parse(Request["StartDate"]);
                    }
                    if (Request["EndDate"] != null && Request["EndDate"] != "")
                    {
                        offer.EndDate = DateTime.Parse(Request["EndDate"]);
                    }
                    
                    offer.IsActive = Request["IsActive"] != null && !String.IsNullOrEmpty(Request["IsActive"]) && Request["IsActive"] == "C";
                    offer.Save();
                    //uow.CommitTransaction();
                    XpoHelper.CommitTransaction(uow);
                    Session["IsNewOffer"] = null;
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                    Session["UnsavedOffer"] = null;
                    Session["IsRefreshed"] = null;

                }
            }
            return Json(new { });

        }

        public JsonResult CancelEdit()
        {
            try
            {
                if (!Boolean.Parse(Session["IsRefreshed"].ToString()))
                {
                    if (Session["uow"] != null)
                    {
                        ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                        ((UnitOfWork)Session["uow"]).RollbackTransaction();
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["uow"] = null;
                    }
                    Session["IsRefreshed"] = null;
                    Session["IsNewOffer"] = null;
                    Session["UnsavedOffer"] = null;
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult OfferDetailGrid(string OfferID, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (/*editMode == null || */editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedOfferDetail"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Guid OfferDetailID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    Offer offer = (Offer)Session["UnsavedOffer"];
                    foreach (OfferDetail offerDetail in offer.OfferDetails)
                    {
                        if (offerDetail.Oid == OfferDetailID)
                        {
                            Session["UnsavedOfferDetail"] = offerDetail;
                            break;
                        }
                    }
                }
                //ViewData["OfferDetail"] = ((Offer)Session["UnsavedOffer"]).OfferDetails;
                return PartialView("OfferDetailGrid", ((Offer)Session["UnsavedOffer"]).OfferDetails);
            }
            else  //view mode
            {
                Guid OfferGuid = Guid.Parse(OfferID);
                Offer offer = XpoHelper.GetNewUnitOfWork().FindObject<Offer>(new BinaryOperator("Oid", OfferGuid, BinaryOperatorType.Equal));
                ViewData["OfferID"] = OfferID;

                return PartialView("OfferDetailGrid", offer.OfferDetails);
            }
        }


        public ActionResult OfferDetailAddNew([ModelBinder(typeof(RetailModelBinder))] OfferDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    Offer offer = (Offer)Session["UnsavedOffer"];
                    Guid itemID = Guid.Parse(Request["ItemComboBoxID"]);
                    OfferDetail offerDetail = new OfferDetail(uow);
                    Item item = offer.Session.FindObject<Item>(new BinaryOperator("Oid", itemID));
                    offerDetail.Item = item;
                    offer.OfferDetails.Add(offerDetail);
                    Session["UnsavedOffer"] = offer;
                    Session["UnsavedOfferDetail"] = null;
                    //ViewData["OfferDetail"] = ((Offer)Session["UnsavedOffer"]).OfferDetails;
                }
                catch (Exception e)
                {
                    Session["Error"]  = e.Message+Environment.NewLine+e.StackTrace;
                }
            }
            else
                Session["Error"]  = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("OfferDetailGrid", ((Offer)Session["UnsavedOffer"]).OfferDetails);
        }

        [HttpPost]
        public ActionResult OfferDetailUpdate([ModelBinder(typeof(RetailModelBinder))] OfferDetail ct)
        {
            GenerateUnitOfWork();
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {

                    Offer offer = (Offer)Session["UnsavedOffer"];
                    Guid itemID = Guid.Parse(Request["ItemComboBoxID"]);

                    OfferDetail offerDetail = (OfferDetail)Session["UnsavedOfferDetail"];
                    Item item = offer.Session.FindObject<Item>(new BinaryOperator("Oid", itemID));
                    offerDetail.Item = item;
                    offer.OfferDetails.Remove(offerDetail);
                    offer.OfferDetails.Add(offerDetail);
                    Session["UnsavedOffer"] = offer;
                    Session["UnsavedOfferDetail"] = null;
                }
                catch (Exception e)
                {
                    Session["Error"]  = e.Message+Environment.NewLine+e.StackTrace;
                }
            }
            else
                Session["Error"]  = Resources.AnErrorOccurred;

            FillLookupComboBoxes();
            return PartialView("OfferDetailGrid", ((Offer)Session["UnsavedOffer"]).OfferDetails);
        }

        [HttpPost]
        public ActionResult OfferDetailDelete([ModelBinder(typeof(RetailModelBinder))] OfferDetail ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                Offer offer = (Offer)Session["UnsavedOffer"];
                foreach (OfferDetail offerDetail in offer.OfferDetails)
                {
                    if (offerDetail.Oid == ct.Oid)
                    {
                        offer.OfferDetails.Remove(offerDetail);
                        offerDetail.Delete();
                        break;
                    }
                }
                //ViewData["OfferDetail"] = ((Offer)Session["UnsavedOffer"]).OfferDetails;
            }
            catch (Exception e)
            {
                Session["Error"]  = e.Message+Environment.NewLine+e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("OfferDetailGrid", ((Offer)Session["UnsavedOffer"]).OfferDetails);
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();

            if (ViewData["ID"] != null)
            {
                GenerateUnitOfWork();
                ViewData["EditMode"] = true;

                Offer offer = uow.FindObject<Offer>(new BinaryOperator("Oid", ViewData["ID"]));
                ViewData["Code"] = offer.Code;
                ViewData["Description"] = offer.Description;
                ViewData["Description2"] = offer.Description2;
                ViewData["StartDate"] = offer.StartDate.ToShortDateString();
                ViewData["EndDate"] = offer.EndDate.ToShortDateString();
                ViewData["PriceCatalog"] = offer.PriceCatalog != null ? offer.PriceCatalog.Description : "";
                ViewData["IsActive"] = offer.IsActive;
            }
            ActionResult rt = PartialView("LoadViewPopup");
            return rt;
        }

        //public ActionResult OfferViewPopup(string OfferID)
        //{
        //    GenerateUnitOfWork();
        //    ViewData["EditMode"] = true;
        //    ViewData["OfferID"] = OfferID;
        //    Guid OfferGuid = (OfferID == null || OfferID == "null" || OfferID == "-1") ? Guid.Empty : Guid.Parse(OfferID);
        //    if (OfferGuid != Guid.Empty)
        //    {
        //        Offer offer = uow.FindObject<Offer>(new BinaryOperator("Oid", OfferGuid));
        //        ViewData["Code"] = offer.Code;
        //        ViewData["Description"] = offer.Description;
        //        ViewData["Description2"] = offer.Description2;
        //        ViewData["StartDate"] = offer.StartDate.ToShortDateString();
        //        ViewData["EndDate"] = offer.EndDate.ToShortDateString();
        //        ViewData["PriceCatalog"] = offer.PriceCatalog != null ? offer.PriceCatalog.Description : "";
        //        ViewData["IsActive"] = offer.IsActive;
        //    }
        //    else
        //    {
        //        ViewData["Code"] = "";
        //        ViewData["Description"] = "";
        //        ViewData["Description2"] = "";
        //        ViewData["StartDate"] = "";
        //        ViewData["EndDate"] = "";
        //        ViewData["PriceCatalog"] = "";
        //        ViewData["IsActive"] = "";
        //    }

        //    return PartialView();
        //}

        public static object ItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                Item obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Item>(e.Value);
                return obj;
            }
            return null;

        }

        public static object GetItemByValue(object value)
        {
            return GetObjectByValue<Item>(value);
        }

        public static object ItemsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            Offer offer = (Offer)System.Web.HttpContext.Current.Session["UnsavedOffer"];
            OwnerApplicationSettings settings = (bool)System.Web.HttpContext.Current.Session["IsNewOffer"] == true ? OwnerApplicationSettings : offer.Owner.OwnerApplicationSettings;
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            if (settings.PadItemCodes && !codefilter.Contains('%'))
            {
                codefilter = codefilter.PadLeft(settings.ItemCodeLength, settings.ItemCodePaddingCharacter[0]);
            }
            
            XPCollection<Item> collection = GetList<Item>(XpoHelper.GetNewUnitOfWork(),
                                                          CriteriaOperator.Or(new BinaryOperator("Name", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                              new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)),
                                                          "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult ItemsComboBox()
        {
            return PartialView();
        }

        public static object PriceCatalogRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                PriceCatalog obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PriceCatalog>(e.Value);
                return obj;
            }
            return null;

        }

        public static object GetPriceCatalogByValue(object value)
        {
            return GetObjectByValue<PriceCatalog>(value);
        }

        public static object PriceCatalogsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<PriceCatalog> collection = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(),
                                                                        CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                                                            new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                                                            //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                            //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                                                           ),
                                                                        "Description");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult ExportTo()
        {
            return ExportToFile<Offer>(Session["OfferGridSettings"] as GridViewSettings, (CriteriaOperator)Session["OfferFilter"]);
        }

        public ActionResult PriceCatalogsComboBox()
        {
            return PartialView();
        }
    }
}
