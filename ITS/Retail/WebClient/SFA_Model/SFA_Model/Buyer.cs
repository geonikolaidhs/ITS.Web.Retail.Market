using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 280, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Buyer : LookUp2Fields, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner
    {
        public Buyer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Buyer(Session session)
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
        
        public Guid Owner
        {
            get;set;
        }
    }
}