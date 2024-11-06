using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System.IO;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo.DB.Exceptions;
using Ionic.Zip;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Providers;
using System.Text;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class CustomReportController : BaseObjController<CustomReport>
    {

        UnitOfWork uow;
        CriteriaOperator filter = null;

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

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.PropertiesToIgnore.Add("IsDefault");
            ruleset.DetailsToIgnore.Add("DocumentTypeCustomReports");
            ruleset.DetailPropertiesToIgnore.Add(typeof(ReportRole), new List<string>() { "IsActive" });
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        // GET: /CustomReport/

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.Edit;

            return PartialView("LoadEditPopup");
        }

        [Security(ReturnsPartial = false)]
        public FileContentResult Download()
        {
            if (Request["Oids"] != null)
            {
                string allOids = Request["Oids"].Trim(';');
                string[] unparsed = allOids.Split(',');
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.AlternateEncoding = Encoding.GetEncoding(737);
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        foreach (String stringOid in unparsed)
                        {
                            Guid oid;
                            if (Guid.TryParse(stringOid, out oid))
                            {
                                CustomReport report = uow.GetObjectByKey<CustomReport>(oid);
                                if (report != null)
                                {
                                    ZipEntry z = zip.AddEntry(report.FileName, report.ReportFile);
                                }
                            }
                        }
                    }
                    using (MemoryStream mem = new MemoryStream())
                    {
                        zip.Save(mem);
                        return File(mem.ToArray(), "application/zip", "reports.zip");
                    }
                }

            }
            return null;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Categories()
        {
            GenerateUnitOfWork();
            //this.ToolbarOptions.Visible = true;
            //this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("gridName", "grdReportCategories");

            return View(GetList<ReportCategory>(uow));
        }

        public ActionResult CategoriesGrid()
        {
            GenerateUnitOfWork();
            if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                ViewData["CallbackMode"] = "DELETESELECTED";
                if (TableCanDelete)
                {
                    using (UnitOfWork localUow = XpoHelper.GetNewUnitOfWork())
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
                                DeleteAllT<ReportCategory>(localUow, oids);
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
            }
            return PartialView(GetList<ReportCategory>(uow));
        }

        public ActionResult CategoryInlineAdd([ModelBinder(typeof(RetailModelBinder))] ReportCategory ct)
        {
            if (!TableCanInsert) return null;
            if (String.IsNullOrWhiteSpace(ct.Description))
            {
                ModelState.AddModelError("Description", Resources.DescriptionIsEmpty);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (SaveT<ReportCategory>(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        Session["Error"] = Resources.CodeAlreadyExists;
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
                ViewBag.CurrentItem = ct;
            }

            return PartialView("CategoriesGrid", GetList<ReportCategory>(XpoHelper.GetNewUnitOfWork()));
        }

        public ActionResult CategoryInlineUpdate([ModelBinder(typeof(RetailModelBinder))] ReportCategory ct)
        {
            if (!TableCanUpdate) return null;
            if (String.IsNullOrWhiteSpace(ct.Description))
            {
                ModelState.AddModelError("Description", Resources.DescriptionIsEmpty);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SaveT<ReportCategory>(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
                ViewBag.CurrentItem = ct;
            }
            Session["Notice"] = Resources.SavedSuccesfully;

            return PartialView("CategoriesGrid", GetList<ReportCategory>(XpoHelper.GetNewUnitOfWork()));
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.FilterButton.Visible = true;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.ExportButton.Visible = true;
            ToolbarOptions.ExportButton.OnClick = "ExportReport";
            ToolbarOptions.PrintButton.Visible = true;
            ToolbarOptions.PrintButton.OnClick = "PrintSelectedReports";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.OptionsButton.Visible = true;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "CustomReportGuid");
            CustomJSProperties.AddJSProperty("gridName", "grdCustomReport");

            GenerateUnitOfWork();

            Session["CustomReportFilter"] = filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");

            return View("Index", GetList<CustomReport>(XpoHelper.GetNewUnitOfWork(), filter, "Code").AsEnumerable<CustomReport>());
        }

        public override ActionResult Grid()
        {
            GenerateUnitOfWork();

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                ViewData["CallbackMode"] = "SEARCH";

                if (Request.HttpMethod == "POST")
                {
                    string Fcode = Request["Fcode"] == null || Request["Fcode"] == "null" ? "" : Request["Fcode"];
                    string Ftitle = Request["Ftitle"] == null || Request["Ftitle"] == "null" ? "" : Request["Ftitle"];
                    string Fdescription = Request["Fdescription"] == null || Request["Fdescription"] == "null" ? "" : Request["Fdescription"];
                    string FcultureInfo = Request["FcultureInfo"] == null || Request["FcultureInfo"] == "null" ? "" : Request["FcultureInfo"];
                    string Fsupplier = Request["Fsupplier"] == null || Request["Fsupplier"] == "null" ? "" : Request["Fsupplier"];

                    CriteriaOperator codeFilter = null;
                    if (Fcode != null && Fcode.Trim() != "")
                    {
                        if (Fcode.Replace('%', '*').Contains("*"))
                        {
                            codeFilter = new BinaryOperator("Code", Fcode.Replace('*', '%'), BinaryOperatorType.Like);
                        }
                        else
                        {
                            codeFilter = new BinaryOperator("Code", Fcode, BinaryOperatorType.Equal);
                        }
                    }

                    CriteriaOperator titleFilter = null;
                    if (Ftitle != null && Ftitle.Trim() != "")
                    {
                        titleFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Title"), Ftitle);
                        //new BinaryOperator("Title", "%" + Ftitle + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator descriptionFilter = null;
                    if (Fdescription != null && Fdescription.Trim() != "")
                    {
                        descriptionFilter = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Title"), Fdescription);
                        // new BinaryOperator("Title", "%" + Fdescription + "%", BinaryOperatorType.Like);
                    }


                    CriteriaOperator culturefilter = null;
                    if (!String.IsNullOrWhiteSpace(FcultureInfo))
                    {
                        eCultureInfo culture = (eCultureInfo)Enum.Parse(typeof(eCultureInfo), FcultureInfo);
                        culturefilter = new BinaryOperator("CultureInfo", culture);
                    }

                    CriteriaOperator visibleReports = null;


                    if (!Boolean.Parse(Session["IsAdministrator"].ToString()))
                    {
                        try
                        {

                            User currentUser = CurrentUser;

                            ////customer should not see this table
                            ////--------------------
                            if (UserHelper.IsCustomer(currentUser))
                            {
                                visibleReports = new BinaryOperator("Oid", Guid.Empty);

                            }

                            ////Supplier reports                  
                            ////----------------------
                            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);
                            if (userSupplier != null)
                            {
                                visibleReports = new BinaryOperator("Owner.Oid", userSupplier.Oid);
                            }
                        }
                        catch (Exception ex)
                        {
                            visibleReports = new BinaryOperator("Oid", Guid.Empty);
                            Session["Error"] = Resources.AnErrorOccurred + ": " + ex.Message;
                        }
                    }
                    else //Admin
                    {
                        Guid supplierOid;
                        if (Fsupplier != null && Guid.TryParse(Fsupplier, out supplierOid))
                        {
                            visibleReports = new BinaryOperator("Owner.Oid", supplierOid);
                        }


                    }
                    filter = CriteriaOperator.And(codeFilter, titleFilter, descriptionFilter, culturefilter, visibleReports);
                    Session["CustomReportFilter"] = filter;
                }
                else
                {
                    filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
                    Session["CustomReportFilter"] = filter;
                }
            }
            GridFilter = (CriteriaOperator)Session["CustomReportFilter"];
            GridSortingField = "Code";
            return base.Grid();
        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid customReportGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);
            User currentUser = CurrentUser;
            ViewData["IsAdministrator"] = UserHelper.IsSystemAdmin(currentUser);
            CompanyNew userSupplier = UserHelper.GetCompany(currentUser);
            ViewData["IsSupplier"] = userSupplier != null;
            ViewData["IsCustomer"] = UserHelper.GetCustomer(currentUser) != null;
            ViewBag.Categories = GetList<ReportCategory>(this.uow);

            if (customReportGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (customReportGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            CustomReport customReport;
            if (Session["UnsavedCustomReport"] == null)
            {
                if (customReportGuid != Guid.Empty)
                {
                    customReport = uow.FindObject<CustomReport>(new BinaryOperator("Oid", customReportGuid, BinaryOperatorType.Equal));
                    Session["IsNewCustomReport"] = false;
                }
                else
                {
                    customReport = new CustomReport(uow);
                    Session["IsNewCustomReport"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (customReportGuid != Guid.Empty && (Session["UnsavedCustomReport"] as CustomReport).Oid == customReportGuid)
                {
                    Session["IsRefreshed"] = true;
                    customReport = (CustomReport)Session["UnsavedCustomReport"];
                }
                else if (customReportGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    customReport = (CustomReport)Session["UnsavedCustomReport"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    customReport = uow.FindObject<CustomReport>(new BinaryOperator("Oid", customReportGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["CustomReportID"] = customReportGuid.ToString();
            Session["UnsavedCustomReport"] = customReport;

            if (bool.Parse(ViewData["IsSupplier"].ToString()))
            {
                Session["currentSupplier"] = userSupplier;
            }
            else if (bool.Parse(ViewData["IsAdministrator"].ToString()))
            {
                if (Session["currentSupplier"] == null)
                {
                    //TODO
                    if (this.CurrentStore != null)
                    {
                        Session["currentSupplier"] = EffectiveOwner;
                    }
                }
            }

            if ((bool)Session["IsNewCustomReport"] == true)
            {
                ViewData["UserSupplier"] = Session["currentSupplier"]; //Used for combobox binding
                (Session["UnsavedCustomReport"] as CustomReport).Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner == null ? Guid.Empty : EffectiveOwner.Oid);
            }
            else
            {
                ViewData["UserSupplier"] = (Session["UnsavedCustomReport"] as CustomReport).Owner; //Used for combobox binding
                ViewData["EnableOwnersComboBox"] = false;
            }


            return PartialView("Edit", customReport);
        }

        public JsonResult jsonSelectSupplier()
        {
            if (Request["SupplierID"] != null)
            {
                CompanyNew supplier = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CompanyNew>(Guid.Parse(Request["SupplierID"]));
                Session["currentSupplier"] = supplier;
            }
            return Json(new { });
        }


        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid customReportGuid = Guid.Empty, reportCategoryGuid = Guid.Empty;

            bool correctCustomReportGuid = Request["CustomReportID"] != null && Guid.TryParse(Request["CustomReportID"].ToString(), out customReportGuid);
            Guid.TryParse(Request["ReportCategory_VI"].ToString(), out reportCategoryGuid);

            if (correctCustomReportGuid)
            {
                CustomReport customReport = (Session["UnsavedCustomReport"] as CustomReport);
                if (customReport != null)
                {
                    if (customReport.ReportFile == null || customReport.ReportFile.Length == 0)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ": Report File is not defined.";
                        return Json(new { error = Session["Error"] });
                    }
                    customReport.Code = Request["Code"];
                    customReport.Description = Request["Description"];
                    customReport.Title = Request["Title"];
                    customReport.CultureInfo = (eCultureInfo)Enum.Parse(typeof(eCultureInfo), Request["CultureInfo"]);
                    customReport.ReportCategory = customReport.Session.GetObjectByKey<ReportCategory>(reportCategoryGuid);
                    if (customReport.Owner == null)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ": Owner is not defined.";
                        return Json(new { error = Session["Error"] });
                    }
                    try
                    {
                        customReport.Save();
                        //uow.CommitTransaction();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                        return Json(new { error = Session["Error"] });
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewCustomReport"] = null;
                        Session["uow"] = null;
                        Session["UnsavedCustomReport"] = null;
                        Session["IsRefreshed"] = null;
                    }
                }
            }
            return Json(new { });
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] CustomReport ct)
        //{
        //    if (!TableCanDelete)
        //        return null;

        //    GenerateUnitOfWork();
        //    try
        //    {
        //        base.Delete(ct);
        //    }
        //    catch (Exception e)
        //    {
        //        Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
        //    }

        //    FillLookupComboBoxes();
        //    return PartialView("Grid", GetList<CustomReport>(uow).AsEnumerable<CustomReport>());
        //}

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (Session["IsRefreshed"] != null && !Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewCustomReport"] = null;
                Session["UnsavedCustomReport"] = null;
            }
            return null;
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".itsreport" },
            MaxFileSize = 200971520
        };


        public ActionResult CustomReportUploadPopup()
        {
            return PartialView();
        }


        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult CustomReportUploadForm()
        {
            return PartialView("CustomReportEdit");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, FileUpload_FileUploadComplete);
            return null;
        }

        public JsonResult jsonDeleteReportFile()
        {
            if (Session["UnsavedCustomReport"] != null)
            {
                CustomReport customReport = (Session["UnsavedCustomReport"] as CustomReport);

                customReport.ReportFile = null;
                customReport.FileName = null;
                Session["UnsavedCustomReport"] = customReport;
            }
            return Json(new { success = true });
        }

        public static void FileUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {

                CustomReport currentCustomReport = (System.Web.HttpContext.Current.Session["UnsavedCustomReport"] as CustomReport);
                if (currentCustomReport != null)
                {
                    //Save the dll
                    //currentPosLayout.LayoutFileSize = e.UploadedFile.ContentLength;
                    currentCustomReport.ReportFile = new Byte[e.UploadedFile.FileBytes.Length];
                    Buffer.BlockCopy(e.UploadedFile.FileBytes, 0, currentCustomReport.ReportFile, 0, e.UploadedFile.FileBytes.Length);
                    currentCustomReport.FileName = e.UploadedFile.FileName;
                    Type reportType = XtraReportBaseExtension.GetReportTypeFromFile(currentCustomReport.ReportFile);
                    Type singleObjectReportType = XtraReportBaseExtension.GetSingleObjectTypeFromFile(currentCustomReport.ReportFile);
                    //currentCustomReport.ReportType = reportType != null ? reportType.Name : "";
                    currentCustomReport.ReportType = reportType == typeof(XtraReportExtension) ? "General Report" : "Single Object Report";
                    currentCustomReport.ObjectType = singleObjectReportType != null ? singleObjectReportType.Name : "";
                }
                e.CallbackData = e.UploadedFile.FileName + "|" + currentCustomReport.ReportType + "|" + currentCustomReport.ObjectType;
            }
        }


        public ActionResult ReportRoleGrid()
        {
            ViewBag.Roles = GetList<Role>((Session["UnsavedCustomReport"] as CustomReport).Session as UnitOfWork).Except((Session["UnsavedCustomReport"] as CustomReport).ReportRoles.Select(reprole => reprole.Role));
            return PartialView((Session["UnsavedCustomReport"] as CustomReport).ReportRoles);
        }


        public ActionResult ReportRoleInlineAdd([ModelBinder(typeof(RetailModelBinder))] ReportRole ct)
        {
            if (ModelState.IsValid)
            {
                Guid roleGuid;
                if (Request["Role!Key_VI"] != null && Guid.TryParse(Request["Role!Key_VI"].ToString(), out roleGuid))
                {
                    CustomReport rep = Session["UnsavedCustomReport"] as CustomReport;
                    ReportRole rr = new ReportRole(rep.Session);
                    rr.Report = rep;
                    rr.Role = rep.Session.GetObjectByKey<Role>(roleGuid);
                }
            }
            return PartialView("ReportRoleGrid", (Session["UnsavedCustomReport"] as CustomReport).ReportRoles);
        }

        public ActionResult ReportRoleInlineDelete([ModelBinder(typeof(RetailModelBinder))] ReportRole ct)
        {
            if (ModelState.IsValid)
            {
                CustomReport rep = Session["UnsavedCustomReport"] as CustomReport;
                ReportRole rr = rep.ReportRoles.First(g => g.Oid == ct.Oid);
                if (rr != null)
                {
                    rr.Delete();
                }
            }
            return PartialView("ReportRoleGrid", (Session["UnsavedCustomReport"] as CustomReport).ReportRoles);
        }

    }
}