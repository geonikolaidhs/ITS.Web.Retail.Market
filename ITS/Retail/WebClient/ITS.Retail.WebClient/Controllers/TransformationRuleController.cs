using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    public class TransformationRuleController : BaseObjController<TransformationRule>
    {

        public override ActionResult Grid()
        {
            if (Session["currentDocumentType"] == null)
            {
                throw new Exception(Resources.AnErrorOccurred);
            }

            if (Request["DXCallbackArgument"].Contains("STARTEDIT") || Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                GetApplicableTransfromableDocumentType(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"])); ;
            }
            return PartialView((Session["currentDocumentType"] as DocumentType).TransformsTo);
        }

        private void GetApplicableTransfromableDocumentType(Guid guid)
        {
            DocumentType currentDocumentType = (Session["currentDocumentType"] as DocumentType);
            List<DocumentType> transformationDocumentTypes = GetList<DocumentType>(XpoSession, new BinaryOperator("Oid", currentDocumentType.Oid, BinaryOperatorType.NotEqual)).ToList();
            IEnumerable<DocumentType> excludeDocumentTypes = currentDocumentType.TransformsTo.Where(rule => rule.Oid != guid).Select(rule => rule.DerrivedType);
            transformationDocumentTypes.RemoveAll(docType => excludeDocumentTypes.Select(type => type.Oid).Contains(docType.Oid));
            ViewBag.DocumentType = transformationDocumentTypes;
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] TransformationRule ct)
        {
            DocumentType docType = Session["currentDocumentType"] as DocumentType;
            TransformationRule rule = docType.TransformsTo.FirstOrDefault(g => g.Oid == ct.Oid);
            if (rule == null)
            {
                rule = new TransformationRule(docType.Session);
                rule.InitialType = docType;
            }

            //ct.InitialType = ct.InitialType;
            UpdateLookupObjects(ct);
            if (ct.InitialType == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    DocumentType dtype = ct.Session.GetObjectByKey<DocumentType>(docType.Oid);
                    ct.InitialType = dtype;
                }
            }
            SetObjectFactorValues(ref ct);
            rule.GetData(ct, new List<string>() { "Session" });
            if (ct.IsDefault && docType.TransformsTo.FirstOrDefault(rul => rul.IsDefault && rul.Oid != ct.Oid) != null)
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
            }
            if (ModelState.IsValid)
            {
                rule.Save();
                docType.TransformsTo.Add(rule);
            }
            else
            {
                GetApplicableTransfromableDocumentType(ct.Oid);
            }
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).TransformsTo);
        }

        [HttpPost]
        public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] TransformationRule ct)
        {
            (Session["currentDocumentType"] as DocumentType).TransformsTo.First(g => g.Oid == ct.Oid).Delete();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).TransformsTo);
        }


        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
        }


        protected override void UpdateLookupObjects(TransformationRule a)
        {
            base.UpdateLookupObjects(a);
            a.InitialType = GetObjectByArgument<DocumentType>(a.Session, "InitialDocumentType_VI") as DocumentType;
            a.DerrivedType = GetObjectByArgument<DocumentType>(a.Session, "DerrivedDocumentType_VI") as DocumentType;
        }

        protected void SetObjectFactorValues(ref TransformationRule transformationRule)
        {
            transformationRule.QtyTransformationFactor = (Request["rdBLQtyTransformationFactor"] == "1") ? -1.0 : +1.0;
            transformationRule.ValueTransformationFactor = (Request["rdBLValueTransformationFactor"] == "1") ? -1.0 : +1.0;
        }
    }
}

