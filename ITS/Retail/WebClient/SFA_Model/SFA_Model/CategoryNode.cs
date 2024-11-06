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
    [CreateOrUpdaterOrder(Order = 290, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CategoryNode : Category, ICategoryNode
    {
        public CategoryNode()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public CategoryNode(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            AssosietedItems = 0;
        }
        public Guid? ParentOid { get; set; }
        private int _AssosietedItems;
        public int AssosietedItems
        {
            get
            {
                return _AssosietedItems;
            }
            set
            {
                SetPropertyValue("AssosietedItems", ref _AssosietedItems, value);
            }
        }

    }
}