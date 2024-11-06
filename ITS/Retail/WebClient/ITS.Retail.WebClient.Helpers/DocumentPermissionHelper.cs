using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ITS.Retail.WebClient.Helpers
{
    public static class DocumentPermissionHelper
    {
        public static bool CanEditDocument(string documentOid)
        {
            Guid documentGuid = Guid.Empty;
            if (Guid.TryParse(documentOid, out documentGuid) == false)
            {
                throw new Exception(" Invalid Document Oid " + documentOid);
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                if (documentHeader == null)
                {
                    throw new Exception(" Invalid Document Oid " + documentOid);
                }

                if (documentHeader.IsCanceled
                    || documentHeader.IsCancelingAnotherDocument
                    || (documentHeader.DocumentType.TakesDigitalSignature && documentHeader.DocumentNumber > 0)
                    || documentHeader.HasBeenExecuted
                    || (documentHeader.HasBeenExecuted && documentHeader.InvoicingDate != null && documentHeader.InvoicingDate > DateTime.MinValue))
                {
                    return false;
                }
                return true;
            }
        }


        public static List<eDocumentFields> GetDisallowedFieldsForDocument(DocumentHeader documentHeader)
        {            

            bool hasAutomaticNumbering = DocumentHasAutomaticNumbering(documentHeader);
            #region Invalid Document States
            if (documentHeader.Status.TakeSequence == false && hasAutomaticNumbering == true && documentHeader.DocumentNumber > 0)
            {
                throw new Exception(" Invalid Document State for document" + documentHeader.Oid.ToString());
            }

            if (documentHeader.Status.TakeSequence == true && hasAutomaticNumbering == true && documentHeader.DocumentNumber == 0)
            {
                throw new Exception(" Invalid Document State for document" + documentHeader.Oid.ToString());
            }
            #endregion

            if (documentHeader.DocumentNumber > 0)
            {
                if (documentHeader.Status.TakeSequence == false && hasAutomaticNumbering == false)
                {
                    return new List<eDocumentFields>() { eDocumentFields.DocumentSeries, eDocumentFields.DocumentType };
                }
                else if (documentHeader.Status.TakeSequence == true && hasAutomaticNumbering == false)
                {
                    return new List<eDocumentFields>() { eDocumentFields.DocumentSeries, eDocumentFields.DocumentType };
                }
                else if (documentHeader.Status.TakeSequence == true && hasAutomaticNumbering == true)
                {
                    return new List<eDocumentFields>() { eDocumentFields.DocumentSeries, eDocumentFields.DocumentType, eDocumentFields.StatusesWhereTakesSequenceIsFalse };
                }
            }
            else if (documentHeader.DocumentNumber == 0)
            {
                if (documentHeader.Status.TakeSequence == false && hasAutomaticNumbering == false)
                {
                    return new List<eDocumentFields>();
                }
                else if (documentHeader.Status.TakeSequence == false && hasAutomaticNumbering == true)
                {
                    return new List<eDocumentFields>();
                }
                else if (documentHeader.Status.TakeSequence == true && hasAutomaticNumbering == false)
                {
                    return new List<eDocumentFields>() { eDocumentFields.DocumentSeries, eDocumentFields.DocumentType, eDocumentFields.StatusesWhereTakesSequenceIsFalse };
                }
            }
            throw new Exception(" Unrecognized State reached for Document "+documentHeader.Oid.ToString());
        }

        public static List<eDocumentFields> GetDisalowedFields(string documentOid)
        {
            Guid documentGuid = Guid.Empty;
            if (Guid.TryParse(documentOid, out documentGuid))
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentGuid);
                    if (documentHeader == null)
                    {
                        throw new Exception(" Invalid Document Oid " + documentOid);
                    }
                    return GetDisallowedFieldsForDocument(documentHeader);
                }
            }
            throw new Exception(" Invalid Document Oid " + documentOid);
        }

        public static bool DocumentHasAutomaticNumbering(DocumentHeader documentHeader)
        {
            if (documentHeader.Status != null && documentHeader.DocumentType != null && documentHeader.DocumentSeries != null)
            {

                return documentHeader.DocumentSeries.HasAutomaticNumbering;

            }
            else
            {
                return true;
            }
        }

        public static bool IsDisallowedField(List<eDocumentFields> disallowed, eDocumentFields field)
        {
            return disallowed.Contains(field);
        }
    }
}
