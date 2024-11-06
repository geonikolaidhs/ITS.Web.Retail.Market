using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using DevExpress.Xpo;

using ITS.Retail.Platform.Enumerations;

namespace SFA_Model
{
    //[CreateOrUpdaterOrder(Order = 910, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentSequence : LookupField, IOwner, IDocumentSequence
    {
        public DocumentSequence()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSequence(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public DocumentSequence(Session session, DocumentSeries series)
            : base(session)
        {

            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public int DocumentNumber { get; set; }

        public Guid DocumentSeries { get; set; }

        public CompanyNew Owner
        {
            get; set;
        }

        public bool Update { get; set; }

        string ILookUpFields.Description { get; set; }

        string IOwner.Description
        {
            get;
        }
        [NonPersistent]
        IDocumentSeries IDocumentSequence.DocumentSeries
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
        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }

        }
    }
}