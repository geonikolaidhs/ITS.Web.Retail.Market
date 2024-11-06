//-----------------------------------------------------------------------
// <copyright file="CategoryNode.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 290,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CategoryNode", typeof(ResourcesLib.Resources))]
    [Indices("ParentOid;GCRecord;Oid;Owner", "ParentOid;GCRecord;Oid", "GCRecord;Oid")]
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("CategoryNode.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        private int _AssosietedItems;

        private Guid? _ParentOid;
        [Persistent("Parent"),Indexed("GCRecord", Unique=false)]
        public Guid? ParentOid
        {
            get
            {
                return _ParentOid;
            }
            set
            {
                SetPropertyValue("ParentOid", ref _ParentOid, value);
            }
        }

        [NonPersistent, UpdaterIgnoreField]
        public CategoryNode Parent
        {
            get
            {                
                return this.Session.FindObject<CategoryNode>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.ParentOid));
            }
            set
            {
                if (value == null)
                {
                    ParentOid = null;
                }
                else
                {
                    ParentOid = value.Oid;
                }
            }
        }

        public XPCollection<CategoryNode> ChildCategories
        {
            get
            {
                return new XPCollection<CategoryNode>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, new BinaryOperator("ParentOid", this.Oid));
            }
        }

        public bool HasChild
        {
            get
            {
                return ChildCategories.FirstOrDefault() != null ;
            }
        }

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

        public CategoryNode GetRoot(Session session)
        {
            return (this.Parent != null) ? Parent.GetRoot(session) : this;
        }

        public void MoveTo(CategoryNode newParent)
        {
            Parent = newParent;
        }

        public List<Guid> GetNodeIDs()
        {
            List<Guid> Ids = new List<Guid>();
            foreach (CategoryNode obj in this.ChildCategories)
            {
                Ids.AddRange(obj.GetNodeIDs());
            }
            Ids.Add(this.Oid);
            return Ids;
        }

        [Aggregated, Association("CategoryNode-OwnerCategories")]
        public XPCollection<OwnerCategories> OwnerCategories
        {
            get
            {
                return GetCollection<OwnerCategories>("OwnerCategories");
            }
        }

        public IEnumerable<CategoryNode> GetAllNodes()
        {
            List<Guid> nodeIds = this.GetNodeIDs();
            string nodeCriteriaStr = "";

            foreach (Guid nodeId in nodeIds)
            {
                nodeCriteriaStr += "or Oid = '" + nodeId.ToString() + "' ";
            }
            nodeCriteriaStr = nodeCriteriaStr.Substring(2);
            CriteriaOperator coper = CriteriaOperator.Parse(nodeCriteriaStr);
            return new XPCollection<CategoryNode>(this.Session, coper, new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
        }


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "AssosietedItems" && Parent != null)
            {
                Parent.AssosietedItems += (int)newValue - (int)oldValue;
            }
            base.OnChanged(propertyName, oldValue, newValue);
        }


        protected override void OnDeleting()
        {
            if (this.HasChild)
            {
                this.Session.Delete(ChildCategories);
            }
            // CaseID 3882 
            MethodInfo callMethod = this.GetType().GetMethod("DeleteChild");
            if (callMethod != null)
            {
                Type[] args = new Type[] { this.GetType() };
                MethodInfo generic = callMethod.MakeGenericMethod(args);
                generic.Invoke(this, new object[] { });
            }
            base.OnDeleting();
        }
        public void DeleteChilds<T>()
        {
            XPCollection<T> ChildCollection = new XPCollection<T>(this.Session, new BinaryOperator("Node", this, BinaryOperatorType.Equal));
            if (ChildCollection.Count != 0)
                this.Session.Delete(ChildCollection);
        }

        public virtual CriteriaOperator GetAllNodeTreeFilter(string collectionProperty = "") 
        {
            List<Guid> allChilds = this.GetNodeIDs();
            string criteriaStr = "Node.Oid = '" + this.Oid.ToString() + "'";
            foreach (Guid node in allChilds)
            {
                criteriaStr += " or Node.Oid ='" + node.ToString() + "'";
            }

            CriteriaOperator finalCrop = CriteriaOperator.Parse(criteriaStr);
            if (String.IsNullOrWhiteSpace(collectionProperty))
            {
                return finalCrop;
            }
            else
            {
                return new ContainsOperator(collectionProperty, finalCrop);
            }
        }
    }
}