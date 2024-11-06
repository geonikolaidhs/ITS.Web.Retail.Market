//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using System.Drawing.Imaging;
using ITS.Retail.ResourcesLib;
using Ionic.Zip;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSLayoutController : BaseObjController<POSLayout>
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

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.POSLayout;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {

            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.OptionsButton.Visible = true;
            ToolbarOptions.ExportButton.Visible = true;
            ToolbarOptions.ExportButton.OnClick = "POSLayout.ExportLayout";

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "POSLayoutGuid");
            CustomJSProperties.AddJSProperty("gridName", "grdPOSLayout");

            GenerateUnitOfWork();
            XPCollection<POSLayout> all = GetList<POSLayout>(uow);
            return View(all);
        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid posLayoutGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            if (posLayoutGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (posLayoutGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            POSLayout posLayout;
            if (Session["UnsavedPOSLayout"] == null)
            {
                if (posLayoutGuid != Guid.Empty)
                {
                    posLayout = uow.FindObject<POSLayout>(new BinaryOperator("Oid", posLayoutGuid, BinaryOperatorType.Equal));
                    Session["IsNewPOSLayout"] = false;
                }
                else
                {
                    posLayout = new POSLayout(uow);
                    Session["IsNewPOSLayout"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (posLayoutGuid != Guid.Empty && (Session["UnsavedPOSLayout"] as POSLayout).Oid == posLayoutGuid)
                {
                    Session["IsRefreshed"] = true;
                    posLayout = (POSLayout)Session["UnsavedPOSLayout"];
                }
                else if (posLayoutGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    posLayout = (POSLayout)Session["UnsavedPOSLayout"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    posLayout = uow.FindObject<POSLayout>(new BinaryOperator("Oid", posLayoutGuid, BinaryOperatorType.Equal));
                }
            }
            FillLookupComboBoxes();
            ViewData["POSLayoutID"] = posLayoutGuid.ToString();
            Session["UnsavedPOSLayout"] = posLayout;

            return PartialView("Edit", posLayout);
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid posLayoutGuid = Guid.Empty;

            bool correctPOSLayoutGuid = Request["POSLayoutID"] != null && Guid.TryParse(Request["POSLayoutID"].ToString(), out posLayoutGuid);
            if (correctPOSLayoutGuid)
            {
                POSLayout posLayout = (Session["UnsavedPOSLayout"] as POSLayout);
                if (posLayout != null)
                {
                    posLayout.Code = Request["Code"];
                    posLayout.Description = Request["Description"];
                    try
                    {
                        AssignOwner<POSLayout>(posLayout);
                        UpdateLookupObjects(posLayout);
                        posLayout.Save();
                        posLayout.Save();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                        return Json(new { error = Session["Error"]});
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewPOSLayout"] = null;
                        Session["uow"] = null;
                        Session["UnsavedPOSLayout"] = null;
                        Session["IsRefreshed"] = null;
                        if (Session["MainLayoutImageStream"] != null)
                        {
                            ((FileStream)Session["MainLayoutImageStream"]).Dispose();
                            ((FileStream)Session["MainLayoutImageStream"]).Close();
                            Session["MainLayoutImageStream"] = null;
                        }

                        if (Session["SecondaryLayoutImageStream"] != null)
                        {
                            ((FileStream)Session["SecondaryLayoutImageStream"]).Dispose();
                            ((FileStream)Session["SecondaryLayoutImageStream"]).Close();
                            Session["SecondaryLayoutImageStream"] = null;
                        }
                    }
                }
            }
            return Json(new { });
        }
        /*
        [HttpPost, ValidateInput(false)]
        public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] POSLayout ct)
        {
            if (!TableCanDelete)
                return null;

            GenerateUnitOfWork();
            try
            {
                base.Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("Grid", GetList<POSLayout>(uow).AsEnumerable<POSLayout>());
        }*/

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
                Session["IsNewPOSLayout"] = null;
                Session["UnsavedPOSLayout"] = null;
                if (Session["MainLayoutImageStream"] != null)
                {
                    ((FileStream)Session["MainLayoutImageStream"]).Dispose();
                    ((FileStream)Session["MainLayoutImageStream"]).Close();
                    Session["MainLayoutImageStream"] = null;
                }

                if (Session["SecondaryLayoutImageStream"] != null)
                {
                    ((FileStream)Session["SecondaryLayoutImageStream"]).Dispose();
                    ((FileStream)Session["SecondaryLayoutImageStream"]).Close();
                    Session["SecondaryLayoutImageStream"] = null;
                }
            }
            return null;
        }

        [Security(ReturnsPartial = false, DontLogAction = true)]
        public FileContentResult ShowImage(int type)
        {
            GenerateUnitOfWork();
            POSLayout posLayout = (Session["UnsavedPOSLayout"] as POSLayout);
            Image image = null;
            if (type == 0)
            {
                image = posLayout == null ? null : posLayout.MainLayoutImage;
            }
            else
            {
                image = posLayout == null ? null : posLayout.SecondaryLayoutImage;
            }


            if (image != null)
            {
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
                string format = "";

                if (image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    format = "jpeg";
                }
                else if ((image.RawFormat.Equals(ImageFormat.Gif)))
                {
                    format = "gif";
                }
                else if ((image.RawFormat.Equals(ImageFormat.Png)))
                {
                    format = "png";
                }

                return new FileContentResult(imageBytes, "image/" + format);
            }
            else
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/img/no_image.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }
        }

        public static readonly UploadControlValidationSettings MainLayoutValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".itsform" },
            MaxFileSize = 200971520
        };

        public static readonly UploadControlValidationSettings SecondaryLayoutValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".itssform" },
            MaxFileSize = 200971520
        };

        [Security(ReturnsPartial = false)]
        public ActionResult MainLayoutUploadControl()
        {
            UploadControlExtension.GetUploadedFiles("MainLayoutUploadControl", MainLayoutValidationSettings, MainLayoutFileUpload_FileUploadComplete);
            return null;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult SecondaryLayoutUploadControl()
        {
            UploadControlExtension.GetUploadedFiles("SecondaryLayoutUploadControl", SecondaryLayoutValidationSettings, SecondaryLayoutFileUpload_FileUploadComplete);
            return null;
        }

        public JsonResult jsonDeleteMainLayoutFile()
        {
            if (Session["UnsavedPOSLayout"] != null)
            {
                POSLayout posLayout = (Session["UnsavedPOSLayout"] as POSLayout);
                if (posLayout.MainLayoutImage != null)
                {
                    posLayout.MainLayoutImage.Dispose();
                    posLayout.MainLayoutImage = null;
                }
                posLayout.MainLayout = null;
                posLayout.MainLayoutFileName = null;
                Session["UnsavedPOSLayout"] = posLayout;
            }
            return Json(new { success = true });
        }

        public JsonResult jsonDeleteSecondaryLayoutFile()
        {
            if (Session["UnsavedPOSLayout"] != null)
            {
                POSLayout posLayout = (Session["UnsavedPOSLayout"] as POSLayout);
                if (posLayout.SecondaryLayoutImage != null)
                {
                    posLayout.SecondaryLayoutImage.Dispose();
                    posLayout.SecondaryLayoutImage = null;
                }
                posLayout.SecondaryLayout = null;
                posLayout.SecondaryLayoutFileName = null;
                Session["UnsavedPOSLayout"] = posLayout;
            }
            return Json(new { success = true });
        }

        public static void MainLayoutFileUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {

            if (e.UploadedFile.IsValid)
            {
                List<string> tempFiles = new List<string>();
                e.CallbackData = "success";
                POSLayout currentPosLayout = (System.Web.HttpContext.Current.Session["UnsavedPOSLayout"] as POSLayout);
                if (currentPosLayout != null)
                {
                    if (System.Web.HttpContext.Current.Session["MainLayoutImageStream"] != null)
                    {
                        ((FileStream)System.Web.HttpContext.Current.Session["MainLayoutImageStream"]).Dispose();
                        ((FileStream)System.Web.HttpContext.Current.Session["MainLayoutImageStream"]).Close();
                    }

                    currentPosLayout.MainLayout = new Byte[e.UploadedFile.FileBytes.Length];
                    Buffer.BlockCopy(e.UploadedFile.FileBytes, 0, currentPosLayout.MainLayout, 0, e.UploadedFile.FileBytes.Length);

                    if (!Directory.Exists(MvcApplication.TEMP_FOLDER))
                    {
                        Directory.CreateDirectory(MvcApplication.TEMP_FOLDER);
                    }

                    string tempFormFile = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".itsform";
                    string bitmapFilename = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".bmp";
                    e.UploadedFile.SaveAs(tempFormFile);
                    string tempDLL = POSHelper.BuildForm(tempFormFile, MvcApplication.TEMP_FOLDER, System.Web.HttpContext.Current.Server);

                    FileStream outFS;
                    Image img = POSHelper.GetFormPreview(tempDLL, System.Web.HttpContext.Current.Server, bitmapFilename, out outFS);

                    System.Web.HttpContext.Current.Session["MainLayoutImageStream"] = outFS; //must keep alive until image is saved
                    currentPosLayout.MainLayoutImage = img;
                    currentPosLayout.MainLayoutFileName = e.UploadedFile.FileName;

                    tempFiles.Add(tempFormFile);
                    tempFiles.Add(tempDLL);
                    DeleteTemporaryFiles(tempFiles);
                }
            }
        }

        public static void SecondaryLayoutFileUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {

            if (e.UploadedFile.IsValid)
            {
                List<string> tempFiles = new List<string>();
                e.CallbackData = "success";
                POSLayout currentPosLayout = (System.Web.HttpContext.Current.Session["UnsavedPOSLayout"] as POSLayout);
                if (currentPosLayout != null)
                {
                    if (System.Web.HttpContext.Current.Session["SecondaryLayoutImageStream"] != null)
                    {
                        ((FileStream)System.Web.HttpContext.Current.Session["SecondaryLayoutImageStream"]).Dispose();
                        ((FileStream)System.Web.HttpContext.Current.Session["SecondaryLayoutImageStream"]).Close();
                    }

                    currentPosLayout.SecondaryLayout = new Byte[e.UploadedFile.FileBytes.Length];
                    Buffer.BlockCopy(e.UploadedFile.FileBytes, 0, currentPosLayout.SecondaryLayout, 0, e.UploadedFile.FileBytes.Length);

                    if (!Directory.Exists(MvcApplication.TEMP_FOLDER))
                    {
                        Directory.CreateDirectory(MvcApplication.TEMP_FOLDER);
                    }

                    string tempFormFile = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".itssform";
                    string bitmapFilename = MvcApplication.TEMP_FOLDER + "\\" + Guid.NewGuid().ToString().Replace("-", "") + ".bmp";
                    e.UploadedFile.SaveAs(tempFormFile);
                    string tempDLL = POSHelper.BuildForm(tempFormFile, MvcApplication.TEMP_FOLDER, System.Web.HttpContext.Current.Server);

                    FileStream outFS;
                    Image img = POSHelper.GetFormPreview(tempDLL, System.Web.HttpContext.Current.Server, bitmapFilename, out outFS);

                    System.Web.HttpContext.Current.Session["SecondaryLayoutImageStream"] = outFS; //must keep alive until image is saved
                    currentPosLayout.SecondaryLayoutImage = img;
                    currentPosLayout.SecondaryLayoutFileName = e.UploadedFile.FileName;

                    tempFiles.Add(tempFormFile);
                    tempFiles.Add(tempDLL);
                    DeleteTemporaryFiles(tempFiles);
                }
            }
        }


        private static void DeleteTemporaryFiles(List<string> files)
        {
            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

            }

        }

        public FileContentResult DownloadPOSLayout(Guid? LayoutOid)
        {
            string message = string.Empty;
            if (LayoutOid.HasValue)
            {
                using (UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork())
                {
                    POSLayout posLayout = unitOfWork.GetObjectByKey<POSLayout>(LayoutOid);
                    if (posLayout != null)
                    {
                        string mainFormFilePath = string.IsNullOrEmpty(posLayout.MainLayoutFileName) || string.IsNullOrWhiteSpace(posLayout.MainLayoutFileName)
                                                     ? string.Empty
                                                     : MvcApplication.TEMP_FOLDER + posLayout.MainLayoutFileName;
                        string secondaryFormFilePath = string.IsNullOrEmpty(posLayout.SecondaryLayoutFileName) || string.IsNullOrWhiteSpace(posLayout.SecondaryLayoutFileName)
                                                     ? string.Empty
                                                     : MvcApplication.TEMP_FOLDER + posLayout.SecondaryLayoutFileName;
                        if (string.IsNullOrEmpty(mainFormFilePath) && string.IsNullOrEmpty(secondaryFormFilePath))
                        {
                            message = Resources.InvalidValue;
                            return null;
                        }
                        using (ZipFile zip = new ZipFile())
                        {
                            if (!string.IsNullOrEmpty(posLayout.MainLayoutFileName) && !string.IsNullOrWhiteSpace(posLayout.MainLayoutFileName))
                            {
                                zip.AddEntry(posLayout.MainLayoutFileName, posLayout.MainLayout);
                            }
                            if (!string.IsNullOrEmpty(posLayout.SecondaryLayoutFileName) && !string.IsNullOrWhiteSpace(posLayout.SecondaryLayoutFileName))
                            {
                                zip.AddEntry(posLayout.SecondaryLayoutFileName, posLayout.SecondaryLayout);
                            }
                            using (MemoryStream mem = new MemoryStream())
                            {
                                zip.Save(mem);
                                return File(mem.ToArray(), "application/zip", "PosLayout.zip");
                            }
                        }
                    }
                    else
                    {
                        message = Resources.InvalidValue;
                    }
                }
            }
            else
            {
                message = Resources.InvalidValue;
            }
            return null;
        }
    }
}
//#endif