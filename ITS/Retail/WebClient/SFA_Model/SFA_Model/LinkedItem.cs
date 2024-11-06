using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 510, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class LinkedItem : BaseObj, ILinkedItem
    {
        public LinkedItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LinkedItem(Session session)
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

        public Guid Item { get; set; }
        
        public double QtyFactor { get; set; }

        public Guid SubItem { get; set; }
        [NonPersistent]
        IItem ILinkedItem.Item
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
        IItem ILinkedItem.SubItem
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