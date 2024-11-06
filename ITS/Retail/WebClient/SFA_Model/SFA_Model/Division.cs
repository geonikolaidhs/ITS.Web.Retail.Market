using ITS.WRM.Model.Interface;
using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using DevExpress.Xpo;


namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 170, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Division : LookupField, IDivision
    {

        public Division()
           : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Division(Session session)
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

        [NonPersistent]
        public List<IDocumentType> DocumentTypes { get; set; }
        
        public eDivision Section { get; set; }

        public bool Update { get; set; }

        string ILookUpFields.Description { get; set; }
       
    }
}