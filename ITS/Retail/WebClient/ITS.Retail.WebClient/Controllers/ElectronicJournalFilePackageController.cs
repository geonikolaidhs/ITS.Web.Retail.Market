using DevExpress.Xpo;
using Ionic.Zip;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.IO;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ElectronicJournalFilePackageController : BaseObjController<ElectronicJournalFilePackage>
    {
        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.ExportButton.Visible = true;
            ToolbarOptions.ExportButton.OnClick = "DownloadPackage";
            ToolbarOptions.ExportToButton.Visible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdElectronicJournalFilePackage");
            return View(GetList<ElectronicJournalFilePackage>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("PackageData");
            ruleset.PropertiesToIgnore.Add("IsActive");
            return ruleset;
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
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        foreach (String stringOid in unparsed)
                        {
                            Guid oid;
                            if (Guid.TryParse(stringOid, out oid))
                            {
                                ElectronicJournalFilePackage ejPack = uow.GetObjectByKey<ElectronicJournalFilePackage>(oid);
                                if (ejPack != null)
                                {
                                    ZipEntry z = zip.AddEntry(ejPack.Description + ".zip", ejPack.PackageData);
                                }
                            }
                        }
                    }
                    using (MemoryStream mem = new MemoryStream())
                    {
                        zip.Save(mem);
                        return File(mem.ToArray(), "application/zip", "ejFiles.zip");
                    }
                }

            }
            return null;
        }

    }
}
