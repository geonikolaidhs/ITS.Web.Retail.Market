using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Helpers
{
    public static class DocumentTypeHelper
    {

        public static object CopyDocumentType(Guid documentTypeGuid)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            DocumentType originalDocumentType = uow.GetObjectByKey<DocumentType>(documentTypeGuid);
            if (originalDocumentType == null)
            {
                HttpContext.Current.Session["Error"] = Resources.AnErrorOccurred;
            }

            DocumentType copy = new DocumentType(uow);

            copy.Division = originalDocumentType.Division;
            copy.DefaultDocumentStatus = originalDocumentType.DefaultDocumentStatus;
            copy.DefaultPaymentMethod = originalDocumentType.DefaultPaymentMethod;
            copy.MinistryDocumentType = originalDocumentType.MinistryDocumentType;

            foreach (DocumentTypeCustomReport docTypeCustomReport in originalDocumentType.DocumentTypeCustomReports)
            {
                DocumentTypeCustomReport dtcr = new DocumentTypeCustomReport(uow);
                dtcr.DocumentType = copy;
                dtcr.Report = docTypeCustomReport.Report;
                dtcr.UserType = docTypeCustomReport.UserType;
                dtcr.Save();
            }
            foreach (StoreDocumentSeriesType storeDocumentSeriesType in originalDocumentType.StoreDocumentSeriesTypes)
            {
                StoreDocumentSeriesType sdst = new StoreDocumentSeriesType(uow);
                sdst.DocumentSeries = storeDocumentSeriesType.DocumentSeries;
                sdst.DocumentType = copy;
                sdst.Save();
            }
            
            copy.Owner = copy.Session.GetObjectByKey<CompanyNew>(originalDocumentType.Owner.Oid);
            copy.TakesDigitalSignature = originalDocumentType.TakesDigitalSignature;
            
            foreach(TransformationRule transRule in originalDocumentType.TransformsFrom)
            {
                TransformationRule transformationRule = new TransformationRule(uow);
                transformationRule.InitialType = transRule.InitialType;
                transformationRule.DerrivedType = copy;
                transformationRule.Save();
            }

            foreach (TransformationRule transRule in originalDocumentType.TransformsTo)
            {
                TransformationRule transformationRule = new TransformationRule(uow);
                transformationRule.InitialType = copy;
                transformationRule.DerrivedType = transRule.DerrivedType;
                transformationRule.Save();
            }

            foreach (DocumentTypeRole documentTypeRole in originalDocumentType.DocumentTypeRoles)
            {
                DocumentTypeRole docTypeRole = new DocumentTypeRole(uow);
                docTypeRole.DocumentType = copy;
                docTypeRole.Role = documentTypeRole.Role;
                docTypeRole.DocumentView = documentTypeRole.DocumentView;
                docTypeRole.IsActive = documentTypeRole.IsActive;
                docTypeRole.Save();
            }

            copy.UsesPrices = originalDocumentType.UsesPrices;
            copy.ValueFactor = originalDocumentType.ValueFactor;
            copy.QuantityFactor = originalDocumentType.QuantityFactor;
            copy.ReferenceCode = originalDocumentType.ReferenceCode;
            copy.FormDescription = originalDocumentType.FormDescription;
            copy.MergedSameDocumentLines = originalDocumentType.MergedSameDocumentLines;
            copy.AllowItemZeroPrices = originalDocumentType.AllowItemZeroPrices;
            copy.IsDefault = originalDocumentType.IsDefault;
            copy.SupportLoyalty = originalDocumentType.SupportLoyalty;
            copy.UsesPrices = originalDocumentType.UsesPrices;
            copy.TakesDigitalSignature = originalDocumentType.TakesDigitalSignature;
            copy.IsForWholesale = originalDocumentType.IsForWholesale;
            copy.UsesPaymentMethods = originalDocumentType.UsesPaymentMethods;
            //copy.IsQuantitative = originalDocumentType.IsQuantitative;
            //copy.IsOfValues = originalDocumentType.IsOfValues;            
            copy.UsesMarkUp = originalDocumentType.UsesMarkUp;            
            copy.UsesMarkUpForm = originalDocumentType.UsesMarkUpForm;
            copy.DocumentHeaderCanBeCopied = originalDocumentType.DocumentHeaderCanBeCopied;
            copy.RecalculatePricesOnTraderChange = originalDocumentType.RecalculatePricesOnTraderChange;

            return copy;
        }

        //public static void AddCategoryToDocType<T>(DocumentType docType, string categoryOid, Type typeCategory) where T : BasicObj
        //{
        //    Guid SelectedNodeID = Guid.Empty;
        //    if (Guid.TryParse(categoryOid, out SelectedNodeID))
        //    {
        //        IEnumerable<Guid> docItemCategories = docType.DocumentTypeItemCategories.SelectMany(doctypeitemcat => doctypeitemcat.ItemCategory.GetNodeIDs());
        //        T docTypeItemCategory = docType.Session.GetObjectByKey<T>(ct.Oid) ??
        //                                                      new T(docType.Session);

        //        if (!docItemCategories.Contains(SelectedNodeID))
        //        {
        //            docTypeItemCategory.GetData(ct, new List<string>() { "Session" });
        //            docTypeItemCategory.ItemCategory = docTypeItemCategory.Session.GetObjectByKey<ItemCategory>(SelectedNodeID); ;
        //            foreach (DocumentTypeItemCategory docItemCat in docType.DocumentTypeItemCategories.ToList())
        //            {
        //                if (docTypeItemCategory.ItemCategory.GetNodeIDs().Contains(docItemCat.ItemCategory.Oid) && docTypeItemCategory.ItemCategory.Oid != docItemCat.ItemCategory.Oid)
        //                {
        //                    docType.DocumentTypeItemCategories.Remove(docItemCat);
        //                    docType.Session.Delete(docItemCat);
        //                }
        //            }
        //            docTypeItemCategory.DocumentType = docType;
        //            docType.DocumentTypeItemCategories.Add(docTypeItemCategory);
        //        }
        //        else
        //        {
        //            Session["Error"] = Resources.DuplicateItemCategory;
        //        }
        //    }
        //    else
        //    {
        //        Session["Error"] = Resources.AnErrorOccurred;
        //    }
        //}

        //public static List<eDocumentTraderType> GetAvailableTraderTypes(eDivision edivision, Division division)
        //{
            
        //}



    }
}
