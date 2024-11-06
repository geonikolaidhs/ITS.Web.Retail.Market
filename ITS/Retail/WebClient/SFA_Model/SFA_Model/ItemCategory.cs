using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{

    [CreateOrUpdaterOrder(Order = 440, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemCategory : LookupField, IOwner, IItemCategory
    {
        public ItemCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ItemCategory(Session session)
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
        public string FullDescription { get; set; }
        public string Code { get; set; }
        public decimal Points { get; set; }

        public Guid? ParentOid { get; set; }
        private int _AssosietedItems;

        public Guid Owner
        { get; set; }

        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        string IOwner.Description
        {
            get;
            
        }        
    }
}