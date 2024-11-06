using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;


using System;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 340, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DeliveryType : LookupField, IDeliveryType
    {
        public DeliveryType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DeliveryType(Session session)
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

        
        //[Persistent("Owner")]
        //public Guid OwnerOid { get; set;} 
        public bool Update { get; set; }

        
        string ILookUpFields.Description { get; set; }




        public Guid Owner
        {
            get;
            set;
        }

        ICompanyNew ITS.WRM.Model.Interface.Model.SupportingClasses.IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ITS.WRM.Model.Interface.Model.SupportingClasses.IOwner.Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}