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
    [CreateOrUpdaterOrder(Order = 450, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemAnalyticTree: BaseObj,IItemAnalyticTree
    {
        public ItemAnalyticTree()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ItemAnalyticTree(Session session)
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
        
        public Guid Root { get; set; }
        
       // public Guid ObjectOid { get; set; }
        
        public Guid Object { get; set; }

        public Guid Node { get; set; }

    }
}