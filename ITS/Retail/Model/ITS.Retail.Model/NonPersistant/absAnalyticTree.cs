//-----------------------------------------------------------------------
// <copyright file="absAnalyticTree.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using System.Data;
using System.Collections.Generic;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{


    [Indices("Root;GCRecord", "Root;Node;Object;GCRecord")]
    public class absAnalyticTree<T> : BaseObj where T : BaseObj
    {
        public absAnalyticTree()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public absAnalyticTree(Session session)
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

        private CategoryNode _Root;
        [Indexed("Node;Object;GCRecord", Unique = false)]
        public CategoryNode Root
        {
            get
            {
                return _Root;
            }
            set
            {
                SetPropertyValue("Root", ref _Root, value);
            }
        }

        private CategoryNode _Node;
        [Indexed(Unique = false)]
        public CategoryNode Node
        {
            get
            {
                return _Node;
            }
            set
            {
                SetPropertyValue("Node", ref _Node, value);                
            }
        }
        protected T _Object;
        //public virtual T Object {
        //    get {
        //        return _Object;
        //    }
        //    set {
        //        SetPropertyValue("Object", ref _Object, value);
        //    }
        //}

        [NonPersistent]
        [DescriptionField]
        public string CategoryPath
        {
            get
            {
                string path = "";
                if (this.Node != null && this.Node.Parent != null)
                {
                    CalculateCategoryPath(this.Node.Parent, ref path);
                }
                else
                {
                    path = this.Node == null ? "" : this.Node.Description;
                }
                return path;
            }
        }


        private void CalculateCategoryPath(CategoryNode node, ref string path)
        {
            if (node.Parent != null)
            {
                path = String.Format(", {0}{1}", node.Description, path);
                CalculateCategoryPath(node.Parent, ref path);
            }
            else
            {
                path = node.Description + path;
            }
        }
        // Get all Items which 
        public XPCollection<absAnalyticTree<T>> GetTreeItem(CategoryNode root)
        {
            return new XPCollection<absAnalyticTree<T>>(this.Session, root != null ? new BinaryOperator("Root.Oid", root.Oid, BinaryOperatorType.Equal) : null);
        }
        public XPCollection<Item> GetNonTreeObjects(CategoryNode root)
        {
            XPQuery<absAnalyticTree<T>> tree = new XPQuery<absAnalyticTree<T>>(this.Session);
            var treeobject = from ot in tree
                             where ot.Root.Oid == root.Oid
                             select ot._Object.Oid;
            return new XPCollection<Item>(this.Session, new NotOperator(new InOperator("Oid", treeobject.ToList())));
        }
        // Επιστρέφει όλα τα είδη του τρέχοντα κόμβου
        private List<Guid> GetItemIds<U>(CategoryNode node)
        {
            CategoryNode root = node.GetRoot(this.Session);
            var quertType = typeof(XPQuery<>);
            Type[] typeArgs = { this.GetType() };
            var makeClass = quertType.MakeGenericType(typeArgs);
            object qry = Activator.CreateInstance(makeClass, this.Session);
            IEnumerable<absAnalyticTree<T>> qryData = qry as IEnumerable<absAnalyticTree<T>>;
            var treeobject = from ot in qryData
                             where ot.Root.Oid == root.Oid && ot.Node.Oid == node.Oid
                             select typeof(U).Name == "Item" ? ot._Object.Oid : ot.Oid;
            return treeobject.ToList();
        }
        // επιτρέπει λίστα με τα Oid όλων των ειδών που υπάρχουν από τον 
        // συγκεκριμένο κόμβο και κάτω.
        public List<Guid> GetNodeObjectIDs<U>(CategoryNode node)
        {
            List<Guid> Ids = new List<Guid>();
            foreach (CategoryNode child in node.ChildCategories)
            {
                Ids.AddRange(GetNodeObjectIDs<U>(child));  // get from childs
            }
            Ids.AddRange(GetItemIds<U>(node));  // get current node Ids
            return Ids;
        }

        public XPCollection<absAnalyticTree<T>> GetAllNodeObjects(CategoryNode node)
        {
            return new XPCollection<absAnalyticTree<T>>(this.Session, new InOperator("Oid", GetItemIds<absAnalyticTree<T>>(node)));
        }

        public bool ItemExistInTree(Session session, CategoryNode node, Item item)
        {
            CategoryNode root = node.GetRoot(session);
            CriteriaOperator cop = CriteriaOperator.And(
                (root != null) ? new BinaryOperator("Root.Oid", root.Oid, BinaryOperatorType.Equal) : null,
                (item != null) ? new BinaryOperator("Object.Oid", item.Oid, BinaryOperatorType.Equal) : null);
            absAnalyticTree<T> foundIt = session.FindObject<absAnalyticTree<T>>(cop);
            return (foundIt != null) ? true : false;
        }

        public void UpdateNode(Session session, CategoryNode newNode)
        {
            if (Root.Oid == newNode.GetRoot(session).Oid)
            {
                Node = newNode;
            }
            else
                throw new DBConcurrencyException("Πρέπει να έχει την ίδια ρίζα");
        }

        protected override void OnSaving()
        {
            if (this.Session.IsNewObject(this) && this.Node != null)
            {
                this.Node.AssosietedItems++;
            }
            base.OnSaving();
        }

        protected override void OnDeleting()
        {
            if (this.Node != null)
            {
                this.Node.AssosietedItems--;
            }
            base.OnDeleting();
        }

    }
}