using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    public class ScannedDocumentHeaderController : BaseObjController<ScannedDocumentHeader>

    {
        [Security(ReturnsPartial=false)]
        public ActionResult Index()
        {
        
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "PreviewImage";
            ToolbarOptions.EditButton.Visible = false;           
            ToolbarOptions.EditButton.Text = ResourcesLib.Resources.Name;

            if (UserHelper.IsSystemAdmin(CurrentUser))
            {
                ToolbarOptions.DeleteButton.Visible = true;
                ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            }
            else
            {
                ToolbarOptions.DeleteButton.Visible = false;
            }          
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ExportButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("gridName", "grdScannedDocumentHeader");
            return View(GetList<ScannedDocumentHeader>(XpoHelper.GetNewUnitOfWork()));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Save()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid oid;
                ScannedDocumentHeader scannedDocumentHeader = null;
                if (Guid.TryParse(Request["Oid"], out oid))
                {
                    scannedDocumentHeader = uow.GetObjectByKey<ScannedDocumentHeader>(oid);
                    if (scannedDocumentHeader != null)
                    {
                        scannedDocumentHeader.Inserted = Request["Inserted"].ToString() == "C";
                        scannedDocumentHeader.Save();
                        XpoHelper.CommitChanges(uow);                        
                    }
                }
            }
            return View("../Home/CloseWindow");
        }

        [Security(ReturnsPartial=false)]
        public ActionResult Edit(String itemID)
        {
            this.ToolbarOptions.ForceVisible = false;

            Guid oid;
            if (Guid.TryParse(itemID, out oid))
            {
                ScannedDocumentHeader scannedDocumentHeader = null;
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                scannedDocumentHeader = uow.GetObjectByKey<ScannedDocumentHeader>(oid);
                if ((scannedDocumentHeader != null && scannedDocumentHeader.Inserted == false) || UserHelper.IsSystemAdmin(CurrentUser))
                {
                    scannedDocumentHeader.EditingUser = uow.GetObjectByKey<User>(CurrentUser.Oid);
                    scannedDocumentHeader.Save();
                    XpoHelper.CommitChanges(uow);     
                    ViewBag.DocumentHeader = scannedDocumentHeader;
                    return View("Edit", scannedDocumentHeader);
                }
            }
            return View("../Home/CloseWindow");
        }

        public ActionResult LoadViewPopup(String ItemID)
        {
            ScannedDocumentHeader item = null;
            Guid oid;
            if (Guid.TryParse(ItemID, out oid))
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                item = uow.GetObjectByKey<ScannedDocumentHeader>(oid);
            }
            ViewBag.DocumentHeader = item;
            
            return PartialView();
        }


        [Security(ReturnsPartial = false, DontLogAction = true)]
        public FileContentResult ShowImageId(String Id)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Guid itemGuid;

            if (Guid.TryParse(Id, out itemGuid))
            {
                ScannedDocumentHeader it = uow.GetObjectByKey<ScannedDocumentHeader>(itemGuid);
                if (it != null && it.ScannedImage != null)
                {
                    ImageConverter converter = new ImageConverter();

                    byte[] imageBytes = (byte[])converter.ConvertTo(it.ScannedImage, typeof(byte[]));
                    string format = "";

                    if (it.ScannedImage.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        format = "jpeg";
                    }
                    else if ((it.ScannedImage.RawFormat.Equals(ImageFormat.Gif)))
                    {
                        format = "gif";
                    }
                    else if ((it.ScannedImage.RawFormat.Equals(ImageFormat.Png)))
                    {
                        format = "png";
                    }

                    return new FileContentResult(imageBytes, "image/" + format);
                }
            }
            {
                Image defaultImage = Image.FromFile(Server.MapPath("~/Content/img/no_image.png"));
                ImageConverter converter = new ImageConverter();

                byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                return new FileContentResult(imageBytes, "image/gif");
            }

        }

    }
}
