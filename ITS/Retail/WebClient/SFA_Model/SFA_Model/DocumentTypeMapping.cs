using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Mobile.AuxilliaryClasses;
using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 199, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentTypeMapping : BaseObj, IDocumentTypeMapping
    {
        public DocumentTypeMapping()
        {

        }
        public DocumentTypeMapping(Session session)
            : base(session)
        {

        }

        public Guid DocumentType { get; set; }
       
        public eDocumentType EDocumentType { get; set; }
        [NonPersistent]
        IDocumentType IDocumentTypeMapping.DocumentType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}