using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.WebClient.Providers;
using System.Collections.Generic;

namespace ITS.Retail.WebClient.Controllers
{
    public class DocumentTypeRoleController : BaseObjController<DocumentTypeRole>
    {
        private void GetApplicableRoles(String includeGuid = null)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(includeGuid, out guid);

            XPCollection<Role> roles = (XPCollection<Role>)ViewBag.Roles;
            List<Guid> rolesToBeRemovedGuids = (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles.Where(docTypeRole => docTypeRole.Role != null && docTypeRole.Oid != guid).Select(documentTypeRole => documentTypeRole.Role.Oid).ToList();
            ViewBag.Roles = roles.Where( role => rolesToBeRemovedGuids.Contains(role.Oid) == false ).ToList();
        }

        public override ActionResult Grid()
        {
            FillLookupComboBoxes();
            if(Request["DXCallbackArgument"]!=null && !String.IsNullOrEmpty(Request["DXCallbackArgument"]))
            {
                if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    GetApplicableRoles();
                }
                
                if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    String oid = Request["DXCallbackArgument"].ToString().Split('|').Last().Split(';').First();                   
                    GetApplicableRoles(oid); ;
                }
            }

            return PartialView((Session["currentDocumentType"] as DocumentType).DocumentTypeRoles);
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.DocumentTypes = GetList<DocumentType>(XpoSession);
            ViewBag.Roles = GetList<Role>(XpoSession);
        }

        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] DocumentTypeRole documentTypeRole)
        {
            UpdateLookupObjects(documentTypeRole);
            DocumentTypeRole newObj = new DocumentTypeRole((Session["currentDocumentType"] as DocumentType).Session);
            newObj.GetData(documentTypeRole);
            newObj.DocumentType = (Session["currentDocumentType"] as DocumentType);
            (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles.Add(newObj);
            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles);
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] DocumentTypeRole documentTypeRole)
        {
            UpdateLookupObjects(documentTypeRole);
            DocumentTypeRole dtcr = (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles.First(g => g.Oid == documentTypeRole.Oid);
            dtcr.GetData(documentTypeRole);
            (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles.Add(dtcr);
            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles);
        }

        [HttpPost]
        public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] DocumentTypeRole documentTypeRole)
        {
            (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles.First(g => g.Oid == documentTypeRole.Oid).Delete();
            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeRoles);
        }

        protected override void UpdateLookupObjects(DocumentTypeRole documentTypeRole)
        {
            base.UpdateLookupObjects(documentTypeRole);
            documentTypeRole.DocumentType = GetObjectByArgument<DocumentType>(documentTypeRole.Session, "DocumentTypeKey_VI");
            documentTypeRole.Role = GetObjectByArgument<Role>(documentTypeRole.Session, "RoleKey_VI");
        }
    }
}