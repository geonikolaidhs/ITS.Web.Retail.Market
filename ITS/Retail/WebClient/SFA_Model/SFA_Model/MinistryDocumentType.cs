using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 180, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class MinistryDocumentType : LookupField, IMinistryDocumentType
    {
        public MinistryDocumentType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public MinistryDocumentType(Session session)
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
        public string Code { get; set; }
        
        public eDocumentValueFactor DocumentValueFactor { get; set; }

        public bool IsSupported { get; set; }

        public string ShortTitle { get; set; }

        public string Title { get; set; }

        public bool Update { get; set; }

        string ILookUpFields.Description { get; set; }

    }
}