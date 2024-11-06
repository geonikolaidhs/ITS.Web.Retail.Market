using System;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.Data.Filtering;
using System.Data;
using System.Collections.Generic;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Master
{
    public class CustomerAnalyticTree : BaseObj, ICustomerAnalyticTree
    {
        public CustomerAnalyticTree()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerAnalyticTree(Session session)
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
        
        private Guid _Root;
        [Indexed("Node;Object", Unique = false)]
        public Guid Root
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

        private Guid _Node;
        [Indexed(Unique = false)]
        public Guid Node
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

        private Guid _Object;
        public Guid Object
        {
            get
            {
                return _Object;
            }
            set
            {
                SetPropertyValue("Object", ref _Object, value);
            }
        }



        // Get all Items which 
        public XPCollection<CustomerAnalyticTree> GetTreeItem(CategoryNode root)
        {
            return new XPCollection<CustomerAnalyticTree>(this.Session, root != null ? new BinaryOperator("Root", root, BinaryOperatorType.Equal) : null);
        }

        public XPCollection<Customer> GetNonTreeObjects(CategoryNode root)
        {
            XPQuery<CustomerAnalyticTree> tree = new XPQuery<CustomerAnalyticTree>(this.Session);
            var treeobject = from ot in tree
                             where ot.Root.Equals(root)
                             select ot.Object;
            return new XPCollection<Customer>(this.Session, new NotOperator(new InOperator("Oid", treeobject.ToList())));
        }


        // Επιστρέφει όλα τα είδη του τρέχοντα κόμβου
        private List<Guid> GetItemIds<T>(CategoryNode node)
        {
            CategoryNode root = node.GetRoot(this.Session);

            XPQuery<CustomerAnalyticTree> tree = new XPQuery<CustomerAnalyticTree>(this.Session);
            var treeobject = from ot in tree
                             where ot.Root.Equals(root) && ot.Node.Equals(node)
                             select typeof(T).Name == "Item" ? ot.Object: ot.Oid;


            return treeobject.ToList();
        }

        // επιτρέγει λίστα με τα Oid όλων των ειδών που υπάρχουν από τον 
        // συγκεκριμένο κόμβο και κάτω.
        public List<Guid> GetNodeObjectIDs<T>(CategoryNode node)
        {
            List<Guid> Ids = new List<Guid>();
            foreach (CategoryNode child in node.ChildCategories)
            {
                Ids.AddRange(GetNodeObjectIDs<T>(child)); // get from childs
            }
            Ids.AddRange(GetItemIds<T>(node)); // get current node Ids
            return Ids;
        }

        public XPCollection<CustomerAnalyticTree> GetAllNodeObjects(CategoryNode node)
        {
            return new XPCollection<CustomerAnalyticTree>(this.Session, new InOperator("Oid", GetItemIds<ItemAnalyticTree>(node)));
        }

        public bool ItemExistInTree(Session session, CategoryNode node, Item item)
        {
            CategoryNode root = node.GetRoot(session);
            CriteriaOperator cop = CriteriaOperator.And(
            (root != null) ? new BinaryOperator("Root", root, BinaryOperatorType.Equal) : null,
            (item != null) ? new BinaryOperator("Object", item.Oid, BinaryOperatorType.Equal) : null);
            CustomerAnalyticTree foundIt = session.FindObject<CustomerAnalyticTree>(cop);
            return (foundIt != null) ? true : false;
        }

        public void UpdateNode(Session session, CategoryNode newNode)
        {
            if (Root == newNode.GetRoot(session).Oid)
            {
                Node = newNode.Oid;
            }
            else
                throw new DBConcurrencyException("Πρέπει να έχει την ίδια ρίζα");
        }

        public Guid NodeOid
        {
            get { return Node; }
        }

        public Guid ObjectOid
        {
            get { return Object; }
        }
    }
}
