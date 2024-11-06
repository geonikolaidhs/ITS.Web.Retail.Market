using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 421, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemExtraInfo : BasicObj, IItemExtraInfo
    {
        public ItemExtraInfo()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemExtraInfo(Session session)
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
        public string Description { get; set; }
        
        public DateTime ExpiresAt { get; set; }
        
        public string Ingredients { get; set; }
        
        public Guid Item { get; set; }
        
        public Guid Owner { get; set; }
        
        public DateTime PackedAt { get; set; }
        [NonPersistent]
        IItem IItemExtraInfo.Item
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
                throw new NotImplementedException();
            }
            
        }
    }
}