using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Common;
using DevExpress.Xpo.DB.Exceptions;

namespace ITS.Retail.WebClient.Controllers
{
    public class SupplierImportFilesSetController : BaseObjController<SupplierImportFilesSet>
    {
        protected SupplierImportFilesSetViewModel CurrentModelInEdit
        {
            get
            {
                return Session["CurrentSupplierImportFilesSetViewModel"] as SupplierImportFilesSetViewModel;
            }
            set
            {
                Session["CurrentSupplierImportFilesSetViewModel"] = value;
            }
        }

        protected IEnumerable<string> GetEntitiesDataSource()
        {
            return typeof(IImportableViewModel).Assembly.GetTypes()
                .Where(x => typeof(IImportableViewModel).IsAssignableFrom(x) && x.IsAbstract == false)
                .Select(x => Activator.CreateInstance(x) as IImportableViewModel).Select(x => x.EntityName).ToList();
        }

        protected IEnumerable<DocumentType> GetDocumentTypesDataSource()
        {
            XPCollection<DocumentType> documentTypes = GetList<DocumentType>(XpoSession);
            return documentTypes;
        }

        protected IEnumerable<DocumentStatus> GetDocumentStatusesDataSource()
        {
            XPCollection<DocumentStatus> documentStatuses = GetList<DocumentStatus>(XpoSession);
            return documentStatuses;
        }

        protected List<SupplierImportFilesSetViewModel> GetModel()
        {
            XPCollection<SupplierImportFilesSet> xpoModel = GetList<SupplierImportFilesSet>(XpoSession);
            List<SupplierImportFilesSetViewModel> model = new List<SupplierImportFilesSetViewModel>(xpoModel.Count);
            foreach (SupplierImportFilesSet xpoItem in xpoModel)
            {
                SupplierImportFilesSetViewModel modelItem = new SupplierImportFilesSetViewModel();
                modelItem.LoadPersistent(xpoItem);
                model.Add(modelItem);
            }
            return model;
        }

        protected void FillSupplierImportFileRecordHeaderLookupComboBoxes(SupplierImportFileRecordHeaderViewModel currentHeader)
        {
            ViewData["EntityNames"] = GetEntitiesDataSource();
            ViewData["DocumentTypes"] = GetDocumentTypesDataSource();
            ViewData["DocumentStatuses"] = GetDocumentStatusesDataSource();
            if (currentHeader != null)
            {
                ViewData["SupplierImportFileRecordHeaders"] = CurrentModelInEdit.SupplierImportFileRecordHeaders.Where(x => x.Oid != currentHeader.Oid && x.IsDeleted == false);                
            }
        }

        protected void FillSupplierImportFileRecordFieldLookupComboBoxes(string entityName)
        {
            Dictionary<string, Type> importableTypes = typeof(IImportableViewModel).Assembly.GetTypes()
                .Where(x => typeof(IImportableViewModel).IsAssignableFrom(x) && x.IsAbstract == false)
                .Select(x => Activator.CreateInstance(x) as IImportableViewModel).ToDictionary(x => x.EntityName, x => x.GetType());

            if (importableTypes.ContainsKey(entityName))
            {
                ViewData["PropertyNames"] = importableTypes[entityName].GetProperties().Where(x => x.CanWrite).Select(x => x.Name).ToList();
            }
            ViewData["SupplierImportMappingHeaders"] = CurrentModelInEdit.SupplierImportMappingHeaders.Where(x => x.IsDeleted == false);

        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.OptionsButton.Visible = false;


            this.CustomJSProperties.AddJSProperty("gridName", "grdSupplierImportFilesSets");
            List<SupplierImportFilesSetViewModel> model = GetModel();
            return View("Index", model);
        }

        public ActionResult SupplierComboBox()
        {
            return PartialView(CurrentModelInEdit);
        }

        public override ActionResult Grid()
        {
            string dxCallbackArgument = Request["DXCallbackArgument"];
            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW && TableCanInsert == false ||
               RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.STARTEDIT && TableCanUpdate == false)
            {
                Session["Error"] = Resources.YouCannotEditThisElement;
                return null;
            }

            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.DELETESELECTED && TableCanDelete == false)
            {
                Session["Error"] = Resources.CannotDeleteObject;
                return null;
            }


            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW)
            {
                this.CurrentModelInEdit = new SupplierImportFilesSetViewModel();
                ViewBag.CurrentItem = this.CurrentModelInEdit;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.STARTEDIT)
            {
                Guid oid = RetailHelper.GetOidToEditFromDxCallbackArgument(dxCallbackArgument);
                SupplierImportFilesSet supplierImportFilesSet = XpoSession.GetObjectByKey<SupplierImportFilesSet>(oid);
                this.CurrentModelInEdit = new SupplierImportFilesSetViewModel();
                if (supplierImportFilesSet != null)
                {
                    this.CurrentModelInEdit.LoadPersistent(supplierImportFilesSet);
                }
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.CANCELEDIT)
            {
                this.CurrentModelInEdit = null;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.DELETESELECTED)
            {
                if (TableCanDelete)
                {
                    List<Guid> oids = RetailHelper.GetOidsToDeleteFromDxCallbackArgument(Request["DXCallbackArgument"]);
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
            }

            List<SupplierImportFilesSetViewModel> model = GetModel();
            return PartialView("Grid", model);
        }

        public ActionResult AddOrUpdateSupplierImportFilesSet()
        {

            if (TryUpdateModel<SupplierImportFilesSetViewModel>(CurrentModelInEdit))
            {
                using (var uow = XpoHelper.GetNewUnitOfWork())
                {
                    SupplierImportFilesSet supplierImportFilesSet = null;
                    if (Request["Oid"] == null)
                    {
                        supplierImportFilesSet = new SupplierImportFilesSet(uow);
                    }
                    else
                    {
                        Guid oid;
                        Guid.TryParse(Request["Oid"], out oid);
                        supplierImportFilesSet = uow.GetObjectByKey<SupplierImportFilesSet>(oid);
                    }

                    CurrentModelInEdit.UpdateModel(uow);
                    CurrentModelInEdit.Persist(supplierImportFilesSet);
                    AssignOwner(supplierImportFilesSet);
                    XpoHelper.CommitChanges(uow);
                    CurrentModelInEdit = null;
                }
            }
            else
            {
                ViewBag.CurrentItem = CurrentModelInEdit;
            }

            List<SupplierImportFilesSetViewModel> model = GetModel();
            return PartialView("Grid", model);
        }

        public ActionResult SupplierImportFileRecordHeaderGrid()
        {
            string dxCallbackArgument = Request["DXCallbackArgument"];
            SupplierImportFileRecordHeaderViewModel currentHeader = null;

            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW)
            {
                currentHeader = new SupplierImportFileRecordHeaderViewModel();
                currentHeader.IsNew = true;
                this.CurrentModelInEdit.SupplierImportFileRecordHeaders.Add(currentHeader);
                ViewBag.CurrentItem = currentHeader;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.CANCELEDIT)
            {
                Guid oid;
                Guid.TryParse(Request["SupplierImportFileRecordHeaderOid"], out oid);
                currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == oid);
                if (currentHeader.IsNew)
                {
                    CurrentModelInEdit.SupplierImportFileRecordHeaders.Remove(currentHeader);
                }
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.STARTEDIT)
            {
                Guid oid = RetailHelper.GetOidToEditFromDxCallbackArgument(dxCallbackArgument);
                currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == oid);
            }

            FillSupplierImportFileRecordHeaderLookupComboBoxes(currentHeader);
            return PartialView("SupplierImportFileRecordHeader/Grid", this.CurrentModelInEdit.SupplierImportFileRecordHeaders.Where(x => x.IsDeleted == false));
        }

        public ActionResult AddOrUpdateSupplierImportFileRecordHeader()
        {
            Guid oid;
            Guid.TryParse(Request["SupplierImportFileRecordHeaderOid"], out oid);
            SupplierImportFileRecordHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == oid);
            if (TryUpdateModel<SupplierImportFileRecordHeaderViewModel>(currentHeader))
            {
                currentHeader.UpdateModel(XpoSession);
                currentHeader.IsNew = false;
            }
            else
            {
                ViewBag.CurrentItem = currentHeader;
            }

            FillSupplierImportFileRecordHeaderLookupComboBoxes(currentHeader);
            return PartialView("SupplierImportFileRecordHeader/Grid", this.CurrentModelInEdit.SupplierImportFileRecordHeaders.Where(x => x.IsDeleted == false));
        }

        public ActionResult DeleteSupplierImportFileRecordHeader()
        {
            Guid oid;
            Guid.TryParse(Request["Oid"], out oid);
            SupplierImportFileRecordHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == oid);
            currentHeader.IsNew = false;
            currentHeader.IsDeleted = true;
            return PartialView("SupplierImportFileRecordHeader/Grid", this.CurrentModelInEdit.SupplierImportFileRecordHeaders.Where(x => x.IsDeleted == false));
        }

        public ActionResult SupplierImportFileRecordFieldGrid()
        {
            string dxCallbackArgument = Request["DXCallbackArgument"];
            Guid currentHeaderOid;
            Guid.TryParse(Request["SupplierImportFileRecordHeaderOid"], out currentHeaderOid);
            string currentEntityName = Request["CurrentEntityName"];
            SupplierImportFileRecordHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);

            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW)
            {
                SupplierImportFileRecordFieldViewModel currentField = new SupplierImportFileRecordFieldViewModel();
                currentField.IsNew = true;
                currentHeader.SupplierImportFileRecordFields.Add(currentField);
                ViewBag.CurrentItem = currentField;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.CANCELEDIT)
            {
                Guid fieldOid;
                Guid.TryParse(Request["SupplierImportFileRecordFieldOid"], out fieldOid);
                SupplierImportFileRecordFieldViewModel currentField = currentHeader.SupplierImportFileRecordFields.FirstOrDefault(x => x.Oid == fieldOid);
                if (currentField.IsNew)
                {
                    currentHeader.SupplierImportFileRecordFields.Remove(currentField);
                }
            }

            FillSupplierImportFileRecordFieldLookupComboBoxes(currentEntityName);
            return PartialView("SupplierImportFileRecordHeader/SupplierImportFileRecordField/Grid", currentHeader.SupplierImportFileRecordFields.Where(x => x.IsDeleted == false));
        }

        public ActionResult AddOrUpdateSupplierImportFileRecordField()
        {
            string currentEntityName = Request["CurrentEntityName"];
            Guid currentHeaderOid;
            Guid.TryParse(Request["SupplierImportFileRecordHeaderOid"], out currentHeaderOid);
            SupplierImportFileRecordHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);
            Guid fieldOid;
            Guid.TryParse(Request["SupplierImportFileRecordFieldOid"], out fieldOid);
            SupplierImportFileRecordFieldViewModel currentField = currentHeader.SupplierImportFileRecordFields.FirstOrDefault(x => x.Oid == fieldOid);
            if (TryUpdateModel<SupplierImportFileRecordFieldViewModel>(currentField))
            {
                currentField.UpdateModel(XpoSession);
                currentField.IsNew = false;
            }
            else
            {
                ViewBag.CurrentItem = currentField;
            }

            FillSupplierImportFileRecordFieldLookupComboBoxes(currentEntityName);
            return PartialView("SupplierImportFileRecordHeader/SupplierImportFileRecordField/Grid", currentHeader.SupplierImportFileRecordFields.Where(x => x.IsDeleted == false));
        }

        public ActionResult DeleteSupplierImportFileRecordField()
        {
            Guid currentHeaderOid;
            Guid.TryParse(Request["SupplierImportFileRecordHeaderOid"], out currentHeaderOid);
            SupplierImportFileRecordHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportFileRecordHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);
            Guid fieldOid;
            Guid.TryParse(Request["SupplierImportFileRecordFieldOid"], out fieldOid);
            SupplierImportFileRecordFieldViewModel currentField = currentHeader.SupplierImportFileRecordFields.Where(res => res.Oid == fieldOid).FirstOrDefault();
            if (currentField != null)
            {
                currentField.IsDeleted = true;
            }

            return PartialView("SupplierImportFileRecordHeader/SupplierImportFileRecordField/Grid", currentHeader.SupplierImportFileRecordFields.Where(x => x.IsDeleted == false));
        }
        ///////

        public ActionResult SupplierImportMappingHeaderGrid()
        {
            string dxCallbackArgument = Request["DXCallbackArgument"];
            SupplierImportMappingHeaderViewModel currentHeader = null;

            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW)
            {
                currentHeader = new SupplierImportMappingHeaderViewModel();
                currentHeader.IsNew = true;
                this.CurrentModelInEdit.SupplierImportMappingHeaders.Add(currentHeader);
                ViewBag.CurrentItem = currentHeader;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.CANCELEDIT)
            {
                Guid mappingHeaderOid;
                Guid.TryParse(Request["SupplierImportMappingHeaderOid"], out mappingHeaderOid);
                currentHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == mappingHeaderOid);
                if (currentHeader.IsNew)
                {
                    CurrentModelInEdit.SupplierImportMappingHeaders.Remove(currentHeader);
                }
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.STARTEDIT)
            {
                Guid currentHeaderOid = RetailHelper.GetOidToEditFromDxCallbackArgument(dxCallbackArgument);
                currentHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);
            }

            return PartialView("SupplierImportMappingHeader/Grid", this.CurrentModelInEdit.SupplierImportMappingHeaders.Where(x => x.IsDeleted == false));
        }

        public ActionResult AddOrUpdateSupplierImportMappingHeader()
        {
            Guid oid;
            Guid.TryParse(Request["SupplierImportMappingHeaderOid"], out oid);
            SupplierImportMappingHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == oid);
            if (TryUpdateModel<SupplierImportMappingHeaderViewModel>(currentHeader))
            {
                currentHeader.UpdateModel(XpoSession);
                currentHeader.IsNew = false;
            }
            else
            {
                ViewBag.CurrentItem = currentHeader;
            }

            return PartialView("SupplierImportMappingHeader/Grid", this.CurrentModelInEdit.SupplierImportMappingHeaders.Where(x => x.IsDeleted == false));
        }

        public ActionResult DeleteSupplierImportMappingHeader()
        {
            Guid oid;
            Guid.TryParse(Request["Oid"], out oid);
            SupplierImportMappingHeaderViewModel currentImportMappingHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == oid);
            currentImportMappingHeader.IsNew = false;
            currentImportMappingHeader.IsDeleted = true;
            return PartialView("SupplierImportMappingHeader/Grid", this.CurrentModelInEdit.SupplierImportMappingHeaders.Where(x => x.IsDeleted == false));
        }


        public ActionResult SupplierImportMappingDetailGrid()
        {
            string dxCallbackArgument = Request["DXCallbackArgument"];
            Guid currentHeaderOid;
            Guid.TryParse(Request["SupplierImportMappingHeaderOid"], out currentHeaderOid);
            string currentEntityName = Request["CurrentEntityName"];
            SupplierImportMappingHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);

            if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.ADDNEWROW)
            {
                SupplierImportMappingDetailViewModel currentField = new SupplierImportMappingDetailViewModel();
                currentField.IsNew = true;
                currentHeader.SupplierImportMappingDetails.Add(currentField);
                ViewBag.CurrentItem = currentField;
            }
            else if (RetailHelper.GetGridCallbackType(dxCallbackArgument) == eDXCallbackArgument.CANCELEDIT)
            {
                Guid oid;
                Guid.TryParse(Request["SupplierImportMappingDetailOid"], out oid);
                SupplierImportMappingDetailViewModel currentField = currentHeader.SupplierImportMappingDetails.FirstOrDefault(x => x.Oid == oid);
                if (currentField.IsNew)
                {
                    currentHeader.SupplierImportMappingDetails.Remove(currentField);
                }
            }
            return PartialView("SupplierImportMappingHeader/SupplierImportMappingDetail/Grid", currentHeader.SupplierImportMappingDetails.Where(x => x.IsDeleted == false));
        }

        public ActionResult AddOrUpdateSupplierImportMappingDetail()
        {
            string currentEntityName = Request["CurrentEntityName"];
            Guid currentHeaderOid;
            Guid.TryParse(Request["SupplierImportMappingHeaderOid"], out currentHeaderOid);
            SupplierImportMappingHeaderViewModel currentHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == currentHeaderOid);
            Guid detailOid;
            Guid.TryParse(Request["SupplierImportMappingDetailOid"], out detailOid);
            SupplierImportMappingDetailViewModel currentField = currentHeader.SupplierImportMappingDetails.FirstOrDefault(x => x.Oid == detailOid);
            if (TryUpdateModel<SupplierImportMappingDetailViewModel>(currentField))
            {
                currentField.UpdateModel(XpoSession);
                currentField.IsNew = false;
            }
            else
            {
                ViewBag.CurrentItem = currentField;
            }

            return PartialView("SupplierImportMappingHeader/SupplierImportMappingDetail/Grid", currentHeader.SupplierImportMappingDetails.Where(x => x.IsDeleted == false));
        }

        public ActionResult DeleteSupplierImportMappingDetail()
        {
            Guid importMappingHeaderOid;
            Guid.TryParse(Request["SupplierImportMappingHeaderOid"], out importMappingHeaderOid);
            SupplierImportMappingHeaderViewModel currentMappingHeader = CurrentModelInEdit.SupplierImportMappingHeaders.FirstOrDefault(x => x.Oid == importMappingHeaderOid);
            Guid importMappingDetailOid;
            Guid.TryParse(Request["SupplierImportMappingDetailOid"], out importMappingDetailOid);
            SupplierImportMappingDetailViewModel currentMappingDetail = currentMappingHeader.SupplierImportMappingDetails.FirstOrDefault(x => x.Oid == importMappingDetailOid);
            if (currentMappingDetail != null)
            {
                currentMappingDetail.IsDeleted = true;
            }

            return PartialView("SupplierImportMappingHeader/SupplierImportMappingDetail/Grid", currentMappingHeader.SupplierImportMappingDetails.Where(x => x.IsDeleted == false));
        }

    }
}
