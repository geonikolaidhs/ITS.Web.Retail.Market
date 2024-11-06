using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 230, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentSeries : LookUp2Fields, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner, IDocumentSeries
    {
        public DocumentSeries()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSeries(Session session)
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
        public DocumentSeries(string code, string description) : base()
        {

        }

        public DocumentSeries(Session session, string code, string description)
            : base(session, code, description)
        {

        }
        public Guid DocumentSequence { get; set; }

        public eModule eModule { get; set; }

        public bool HasAutomaticNumbering { get; set; }

        public Guid? IsCanceledByOid { get; set; }

        public bool IsCancelingSeries { get; set; }
        public Guid POS { get; set; }

        public string PrintedCode { get; set; }

        public string Remarks { get; set; }

        public bool ShouldResetMenu { get; set; }

        public Guid Store { get; set; }
        [NonPersistent]
        IStore IDocumentSeries.Store
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
        IDocumentSequence IDocumentSeries.DocumentSequence
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
        IPOS IDocumentSeries.POS
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