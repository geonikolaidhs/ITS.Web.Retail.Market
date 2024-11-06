//-----------------------------------------------------------------------
// <copyright file="ItemCategory.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [DataViewParameter]
    [Updater(Order = 440,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("ItemCategory", typeof(ResourcesLib.Resources))]
    public class ItemCategory : CategoryNode, IRequiredOwner, IItemCategory
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("ItemCategory.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }



        private string _FullDescription;
        private decimal _Points;
        [NonPersistent]
        public string FullCode
        {
            get
            {
                return GetFullCode();
            }
        }

        public string FullDescription
        {
            get
            {
                return _FullDescription;
            }
            set
            {
                SetPropertyValue("FullDescription", ref _FullDescription, value);
            }
        }

        public decimal Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Points", ref _Points, value);
            }
        }

        public string GetFullCode()
        {
            if (this.Parent != null)
            {
                return String.Format("{0}:{1}", Code, (Parent as ItemCategory).GetFullCode());
            }
            return Code;
        }
        public string GetFullDescription()
        {
            if (this.Parent != null)
            {
                return String.Format("{0}>{1}", Description, (Parent as ItemCategory).GetFullDescription());
            }
            return Description;
        }

        public XPCollection<T> GetAllNodeTreeItems_old<T>(CriteriaOperator crop = null)
        {
            ItemAnalyticTree iat = new ItemAnalyticTree(this.Session);
            IEnumerable<Guid> lstIDs = iat.GetNodeObjectIDs<T>(this as CategoryNode);
            XPCursor cursor = new XPCursor(this.Session, typeof(T), lstIDs.ToList());
            XPCollection<T> tcol = new XPCollection<T>(this.Session, false);
            tcol.AddRange(cursor.OfType<T>());
            return tcol;
        }


        public XPCollection<T> GetAllNodeTreeItems<T>(CriteriaOperator crop = null)
        {
            if (typeof(T) != typeof(ItemAnalyticTree))
            {
                return GetAllNodeTreeItems_old<T>(crop);
            }
            List<Guid> nodeIds = this.GetNodeIDs();
            string nodeCriteriaStr = "";

            foreach (Guid nodeId in nodeIds)
            {
                nodeCriteriaStr += "or Node.Oid = '" + nodeId.ToString() + "' ";
            }
            nodeCriteriaStr = nodeCriteriaStr.Substring(2);

            CriteriaOperator finalCrop = CriteriaOperator.Or(crop, CriteriaOperator.Parse(nodeCriteriaStr));
            XPCollection<T> tcol = new XPCollection<T>(this.Session, finalCrop);
            return tcol;
        }

        [Association("ItemCategory-CustomerCategoryDiscounts")]
        public XPCollection<CustomerCategoryDiscount> CustomerCategoryDiscounts
        {
            get
            {
                return GetCollection<CustomerCategoryDiscount>("CustomerCategoryDiscounts");
            }
        }
        public override CriteriaOperator GetAllNodeTreeFilter(string collectionProperty = "ItemAnalyticTrees")
        {
            return base.GetAllNodeTreeFilter(collectionProperty);
        }
    }

}