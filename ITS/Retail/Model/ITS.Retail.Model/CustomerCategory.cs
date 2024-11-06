using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
//-----------------------------------------------------------------------
// <copyright file="CustomerCategory.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ITS.Retail.Model
{
    [Updater(Order = 320,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CustomerCategory", typeof(ResourcesLib.Resources))]
    public class CustomerCategory : CategoryNode, ICustomerCategory
    {
        public CustomerCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerCategory(Session session)
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
                        throw new Exception("CustomerCategory.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        private bool _IsLoyalty;

        public bool IsLoyalty
        {
            get
            {
                return _IsLoyalty;
            }
            set
            {
                SetPropertyValue("IsLoyalty", ref _IsLoyalty, value);
            }
        }

        [Association("CustomerCategory-CustomerCategoryDiscounts")]
        public XPCollection<CustomerCategoryDiscount> CustomerCategoryDiscounts
        {
            get
            {
                return GetCollection<CustomerCategoryDiscount>("CustomerCategoryDiscounts");
            }
        }

        [Association("CustomerCategory-PromotionCustomerCategoryApplicationRules")]
        public XPCollection<PromotionCustomerCategoryApplicationRule> PromotionCustomerCategoryApplicationRules
        {
            get
            {
                return GetCollection<PromotionCustomerCategoryApplicationRule>("PromotionCustomerCategoryApplicationRules");
            }
        }

        public XPCollection<T> GetAllNodeTreeItems<T>(CriteriaOperator crop = null)
        {
            if (typeof(T) != typeof(CustomerAnalyticTree))
            {
                return GetAllNodeTreeItems_old<T>(crop);
            }
            List<Guid> nodeIds = this.GetNodeIDs();
            String nodeCriteriaStr = "";

            foreach (Guid nodeId in nodeIds)
            {
                nodeCriteriaStr += "or Node.Oid = '" + nodeId.ToString() + "' ";
            }
            nodeCriteriaStr = nodeCriteriaStr.Substring(2);

            CriteriaOperator finalCrop = CriteriaOperator.Or(crop, CriteriaOperator.Parse(nodeCriteriaStr));
            XPCollection<T> tcol = new XPCollection<T>(this.Session, finalCrop);
            return tcol;
        }

        public XPCollection<T> GetAllNodeTreeItems_old<T>(CriteriaOperator crop = null)
        {
            CustomerAnalyticTree iat = new CustomerAnalyticTree(this.Session);
            IEnumerable<Guid> lstIDs = iat.GetNodeObjectIDs<T>(this as CategoryNode);
            XPCursor cursor = new XPCursor(this.Session, typeof(T), lstIDs.ToList());
            XPCollection<T> tcol = new XPCollection<T>(this.Session, false);
            tcol.AddRange(cursor.OfType<T>());
            return tcol;
        }

        public override CriteriaOperator GetAllNodeTreeFilter(string collectionProperty = "CustomerAnalyticTrees")
        {
            return base.GetAllNodeTreeFilter(collectionProperty);
        }
    }

}