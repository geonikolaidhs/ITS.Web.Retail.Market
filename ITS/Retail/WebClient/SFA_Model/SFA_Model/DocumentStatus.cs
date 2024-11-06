using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 150, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentStatus : LookUp2Fields, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner, IDocumentStatus
    {
        public DocumentStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            TakeSequence = false;
            ReadOnly = false;
            IsDefault = false;
        }

        [NonPersistent]
        public List<IDocumentHeader> DocumentHeaders { get; set; }

        public bool ReadOnly { get; set; }

        public bool TakeSequence { get; set; }

    }
}