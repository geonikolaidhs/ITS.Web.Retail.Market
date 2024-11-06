using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Helpers;
using System.IO;
using Ionic.Zip;
using System.Text;
using ITS.Retail.Common.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    public class InformationSheetController : BaseObjController<ItemCategory>
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
            this.ToolbarOptions.ForceVisible = false;
            CriteriaOperator filter = new BinaryOperator("Oid", Guid.Empty);

            return View();
        }

        public ActionResult TreeViewPartial()
        {
            return PartialView("TreeViewPartial");
        }

        public ActionResult ItemsOfNode()
        {
            GenerateUnitOfWork();
            string categoryid = Request["categoryid"] == null || Request["categoryid"] == "null" ? "" : Request["categoryid"];
            if (categoryid != "")
            {
                Guid category_guid = categoryid != "" ? Guid.Parse(categoryid) : Guid.Empty;

                ItemCategory cat = XpoHelper.GetNewSession().FindObject<ItemCategory>(CriteriaOperator.Parse("Oid='" + category_guid + "'", ""));
                if (cat != null)
                {
                    ViewData["categoryName"] = cat.Description;
                    ItemCategory root = (ItemCategory)cat.GetRoot(XpoHelper.GetNewSession());

                    CriteriaOperator filter = (categoryid == "" ? CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "") : CriteriaOperator.Parse("Node='" + Guid.Parse(categoryid) + "'" + " and Root='" + root.Oid + "'"));

                    ViewData["categoryid"] = categoryid;
                    User currentUser = CurrentUser;
                    XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(currentUser.Session, currentUser);
                    XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(currentUser.Session, currentUser);
                    XPCollection<ItemAnalyticTree> itemTrees;
                    if (Boolean.Parse(Session["IsAdministrator"].ToString()))
                    {
                        itemTrees = cat.GetAllNodeTreeItems<ItemAnalyticTree>();
                        ViewData["ItemAnalyticTree"] = itemTrees.Where(tree => !String.IsNullOrEmpty(tree.Object.ExtraFilename)).ToList();
                    }
                    else
                    {
                        if (userSuppliers.Count != 0)
                        {
                            CompanyNew owner = userSuppliers.First();
                            XPCollection<PriceCatalog> supplierCatalogs = GetList<PriceCatalog>(uow, new ContainsOperator("StorePriceLists", new BinaryOperator("Store.Owner.Oid", owner.Oid)));
                            itemTrees = TreeHelper.GetAllNodeTreeItemsOfCatalogs(cat, supplierCatalogs);
                            foreach (ItemAnalyticTree tree in itemTrees.ToList())
                            {
                                if (String.IsNullOrEmpty(tree.Object.ExtraFilename))
                                {
                                    itemTrees.ToList().Remove(tree);
                                }
                            }
                            ViewData["ItemAnalyticTree"] = itemTrees;

                        }
                        else if (userCustomers.Count != 0)
                        {
                            Customer cust = userCustomers.First();
                            Store store = cust.Session.GetObjectByKey<Store>(this.CurrentStore.Oid);

                            itemTrees = TreeHelper.GetAllNodeTreeItemsOfCatalog(cat, PriceCatalogHelper.GetPriceCatalogPolicy(store, cust));//cust.GetPriceCatalog(store));
                            foreach (ItemAnalyticTree tree in itemTrees.ToList())
                            {
                                if (String.IsNullOrEmpty(tree.Object.ExtraFilename))
                                {
                                    itemTrees.ToList().Remove(tree);
                                }
                            }
                            ViewData["ItemAnalyticTree"] = itemTrees;
                        }
                    }
                    ViewData["rootid"] = root.Oid;
                }


            }
            return PartialView();
        }

        [Security(ReturnsPartial = false)]
        public FileContentResult DownloadExtraFile()
        {
            if (!String.IsNullOrEmpty(Request["Oids"]))
            {
                Item item;
                ItemAnalyticTree iat = null;
                Guid itemAnalyticTreeGuid;
                string[] itemAnalyticTreeOids = Request["Oids"].Trim(';').Split(',');

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (itemAnalyticTreeOids.Count() == 1)
                    {
                        if (Guid.TryParse(itemAnalyticTreeOids[0], out itemAnalyticTreeGuid))
                        {
                            iat = uow.GetObjectByKey<ItemAnalyticTree>(itemAnalyticTreeGuid);
                            item = iat != null ? iat.Object : null;
                            if (item != null)
                            {
                                return File(item.ExtraFile, item.ExtraMimeType, "Item_"+item.Code+"_InformationSheet."+item.ExtraFilename.Split('.').Last<String>());
                            }
                        }
                    }
                    else
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AlternateEncodingUsage = ZipOption.Always;
                            zip.AlternateEncoding = Encoding.GetEncoding(737);
                            foreach (String itemAnalyticTreeOid in itemAnalyticTreeOids)
                            {

                                if (Guid.TryParse(itemAnalyticTreeOid, out itemAnalyticTreeGuid))
                                {
                                    iat = uow.GetObjectByKey<ItemAnalyticTree>(itemAnalyticTreeGuid);
                                    item = iat != null ? iat.Object : null;
                                    if (item != null)
                                    {
                                        string filename = item.ExtraFilename;
                                        int numberOfTimesFilenameFound = zip.Entries.Where(entry => entry.FileName == filename).Count();
                                        if (numberOfTimesFilenameFound > 0)
                                        {
                                            while (zip.Entries.Where(entry => entry.FileName == filename).Count() > 0)
                                            {
                                                numberOfTimesFilenameFound++;
                                                filename = GetNewFileName(item.ExtraFilename, numberOfTimesFilenameFound);
                                            }
                                        }
                                        ZipEntry z = zip.AddEntry(filename, item.ExtraFile);
                                    }
                                }
                            }
                            using (MemoryStream mem = new MemoryStream())
                            {
                                zip.Save(mem);
                                return File(mem.ToArray(), "application/zip", "InformationSheets.zip");
                            }
                        }              
                    }
                }
            }
            return null;
        }

        private string GetNewFileName(string filename,int numberOfTimesFilenameFound)
        {
            string fileNameOnly = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            return fileNameOnly + numberOfTimesFilenameFound + extension;            
        }
    }
}
