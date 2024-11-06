using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Web;
using ITS.Retail.Model.NonPersistant;
using DevExpress.Web.Mvc;

namespace ITS.Retail.WebClient.Helpers
{
    public static class TreeHelper
    {
        public static XPCollection<ItemAnalyticTree> GetAllNodeTreeItemsOfCatalog(ItemCategory itemCategory, PriceCatalogPolicy policy)
        {
            IEnumerable<Guid> allChilds = itemCategory.GetNodeIDs();

            List<CriteriaOperator> categoryfilterList = new List<CriteriaOperator>();

            categoryfilterList.Add(new BinaryOperator("Node.Oid", itemCategory.Oid));
            foreach (Guid node in allChilds)
            {
                categoryfilterList.Add(new BinaryOperator("Node.Oid", node));
            }

            List<CriteriaOperator> PriceCatalogfilterList = new List<CriteriaOperator>();

            //Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();
            //pcQueue.Enqueue(pc);
            //while (pcQueue.Count != 0)
            //{
            //    PriceCatalog currPC = pcQueue.Dequeue();
            //    PriceCatalogfilterList.Add(new BinaryOperator("PriceCatalog", currPC.Oid));
            //    if (currPC.ParentCatalog != null)
            //    {
            //        pcQueue.Enqueue(currPC.ParentCatalog);
            //    }

            //}

            foreach (Guid priceCatalogGuid in PriceCatalogHelper.GetPriceCatalogsFromPolicy(new EffectivePriceCatalogPolicy(policy)))
            {
                PriceCatalogfilterList.Add(new BinaryOperator("PriceCatalog", priceCatalogGuid));
            }

            CriteriaOperator crop = CriteriaOperator.And(new ContainsOperator("Object.PriceCatalogs", CriteriaOperator.Or(PriceCatalogfilterList)), CriteriaOperator.Or(categoryfilterList));
            XPCollection<ItemAnalyticTree> iats = new XPCollection<ItemAnalyticTree>(itemCategory.Session, crop);
            return iats;
        }

        public static XPCollection<CustomerAnalyticTree> GetAllNodeTreeCustomersOfCatalog(CustomerCategory customerCategory)
        {
            IEnumerable<Guid> allChilds = customerCategory.GetNodeIDs();

            List<CriteriaOperator> categoryfilterList = new List<CriteriaOperator>();

            categoryfilterList.Add(new BinaryOperator("Node.Oid", customerCategory.Oid));
            foreach (Guid node in allChilds)
            {
                categoryfilterList.Add(new BinaryOperator("Node.Oid", node));
            }
            XPCollection<CustomerAnalyticTree> iats = new XPCollection<CustomerAnalyticTree>(customerCategory.Session, categoryfilterList);
            return iats;
        }

        public static XPCollection<ItemAnalyticTree> GetAllNodeTreeItemsOfCatalogs(ItemCategory itemCategory, XPCollection<PriceCatalog> priceCatalogs)
        {
            IEnumerable<Guid> allChilds = itemCategory.GetNodeIDs();
            var categoryfilter = itemCategory.GetAllNodeTreeFilter("");
            List<CriteriaOperator> PriceCatalogfilterList = new List<CriteriaOperator>();
            Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();
            foreach (PriceCatalog pc in priceCatalogs)
            {
                pcQueue.Enqueue(pc);
            }
            while (pcQueue.Count != 0)
            {
                PriceCatalog currPC = pcQueue.Dequeue();
                PriceCatalogfilterList.Add(new BinaryOperator("PriceCatalog", currPC.Oid));
                foreach (PriceCatalog pc in currPC.PriceCatalogs)
                {
                    pcQueue.Enqueue(pc);
                }

            }

            CriteriaOperator crop = CriteriaOperator.And(new ContainsOperator("Object.PriceCatalogs", CriteriaOperator.Or(PriceCatalogfilterList)), categoryfilter);
            XPCollection<ItemAnalyticTree> iats = new XPCollection<ItemAnalyticTree>(itemCategory.Session, crop);
            return iats;
        }


        public static MVCxTreeViewNodeCollection CreateItemCategoryTreeNodes()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                IEnumerable<ItemCategory> collection = new XPCollection<ItemCategory>(uow, new NullOperator("ParentOid")).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);

                Queue<ItemCategory> icQueue = new Queue<ItemCategory>();
                MVCxTreeViewNodeCollection nodes = new MVCxTreeViewNodeCollection();
                Queue<MVCxTreeViewNode> nodeQueue = new Queue<MVCxTreeViewNode>();

                foreach (ItemCategory ic in collection)
                {

                    nodeQueue.Enqueue(nodes.Add(ic.Code + " - " + ic.Description, ic.Oid.ToString()));
                    icQueue.Enqueue(ic);


                }
                MVCxTreeViewNode parentNode, newNode;
                while (icQueue.Count != 0)
                {
                    ItemCategory ic2 = icQueue.Dequeue();
                    parentNode = nodeQueue.Dequeue();

                    IEnumerable<ItemCategory> collection2 = new XPCollection<ItemCategory>(uow, new BinaryOperator("ParentOid", ic2.Oid, BinaryOperatorType.Equal)).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                    foreach (ItemCategory ic in collection2)
                    {
                        newNode = new MVCxTreeViewNode(ic.Code + " - " + ic.Description, ic.Oid.ToString());
                        parentNode.Nodes.Add(newNode);
                        icQueue.Enqueue(ic);
                        nodeQueue.Enqueue(newNode);

                    }
                }

                return nodes;
            }
        }

        /// <summary>
        /// Δημιουργεί το δένδρο των τιμοκαταλόγων με τους τιμοκαταλόγους που μπορεί να δεί ο user (ως admin ή company, OXI CUSTOMER)
        /// </summary>
        /// <returns></returns>
        public static MVCxTreeViewNodeCollection CreatePriceCatalogTreeNodes(CompanyNew owner, CriteriaOperator filter = null)
        {
            if (owner == null)
            {
                return new MVCxTreeViewNodeCollection();
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                User currentUser = (User)HttpContext.Current.Session["currentUser"];
                CriteriaOperator visiblePriceCatalogs = new BinaryOperator("Owner.Oid", owner.Oid, BinaryOperatorType.Equal);
                if (filter != null)
                {
                    visiblePriceCatalogs = CriteriaOperator.And(visiblePriceCatalogs, filter);
                }


                IEnumerable<PriceCatalog> collection = new XPCollection<PriceCatalog>(uow, CriteriaOperator.And(visiblePriceCatalogs,
                                                                                       new NullOperator("ParentCatalog"))).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();
                MVCxTreeViewNodeCollection nodes = new MVCxTreeViewNodeCollection();
                Queue<MVCxTreeViewNode> nodeQueue = new Queue<MVCxTreeViewNode>();

                foreach (PriceCatalog pc in collection)
                {

                    nodeQueue.Enqueue(nodes.Add(pc.Code + " - " + pc.Description, pc.Oid.ToString()));
                    pcQueue.Enqueue(pc);


                }
                MVCxTreeViewNode parentNode, newNode;
                while (pcQueue.Count != 0)
                {
                    PriceCatalog pc2 = pcQueue.Dequeue();
                    parentNode = nodeQueue.Dequeue();

                    IEnumerable<PriceCatalog> collection2 = new XPCollection<PriceCatalog>(uow, new BinaryOperator("ParentCatalog", pc2))
                        .OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                    foreach (PriceCatalog pc in collection2)
                    {
                        newNode = new MVCxTreeViewNode(pc.Code + " - " + pc.Description, pc.Oid.ToString());
                        parentNode.Nodes.Add(newNode);
                        pcQueue.Enqueue(pc);
                        nodeQueue.Enqueue(newNode);

                    }
                }

                return nodes;
            }

        }

        public static MVCxTreeViewNodeCollection CreateCustomersCategoryTreeNodes()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                IEnumerable<CustomerCategory> collection = new XPCollection<CustomerCategory>(uow, new NullOperator("ParentOid")).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);

                Queue<CustomerCategory> icQueue = new Queue<CustomerCategory>();
                MVCxTreeViewNodeCollection nodes = new MVCxTreeViewNodeCollection();
                Queue<MVCxTreeViewNode> nodeQueue = new Queue<MVCxTreeViewNode>();

                foreach (CustomerCategory ic in collection)
                {

                    nodeQueue.Enqueue(nodes.Add(ic.Code + " - " + ic.Description, ic.Oid.ToString()));
                    icQueue.Enqueue(ic);


                }
                MVCxTreeViewNode parentNode, newNode;
                while (icQueue.Count != 0)
                {
                    CustomerCategory ic2 = icQueue.Dequeue();
                    parentNode = nodeQueue.Dequeue();

                    IEnumerable<CustomerCategory> collection2 = new XPCollection<CustomerCategory>(uow, new BinaryOperator("ParentOid", ic2.Oid)).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                    foreach (CustomerCategory ic in collection2)
                    {
                        newNode = new MVCxTreeViewNode(ic.Code + " - " + ic.Description, ic.Oid.ToString());
                        parentNode.Nodes.Add(newNode);
                        icQueue.Enqueue(ic);
                        nodeQueue.Enqueue(newNode);

                    }
                }

                return nodes;
            }
        }


        /// <summary>
        /// Επιστρέφει την συνάρτηση που χτίζει δυναμικά το δένδρο των τιμοκαταλόγων με τους τιμοκαταλόγους που μπορεί να δεί ο user (ως admin ή company, OXI CUSTOMER)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="filter">Extra φίλτρο που θα εφαρμοστεί στην αναζήτηση των Ριζικών Κόμβων</param>
        public static TreeViewVirtualModeCreateChildrenMethod GetPriceCatalogTreeViewVirtualMethod(CompanyNew owner, CriteriaOperator filter = null)
        {

            return (TreeViewVirtualModeCreateChildrenEventArgs e) =>
                   {
                       if (owner == null)
                       {
                           throw new ArgumentNullException("owner");
                       }

                       List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();

                       using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                       {

                           IEnumerable<PriceCatalog> pcs = null;
                           if (e.NodeName == null)
                           {
                               //root
                               User currentUser = (User)HttpContext.Current.Session["currentUser"];
                               CriteriaOperator visiblePriceCatalogs = new BinaryOperator("Owner.Oid", owner.Oid);
                               if (!ReferenceEquals(filter, null))
                               {
                                   visiblePriceCatalogs = CriteriaOperator.And(visiblePriceCatalogs, filter);
                               }

                               IEnumerable<PriceCatalog> parentCatalogs = new XPCollection<PriceCatalog>(uow, CriteriaOperator.And(visiblePriceCatalogs,
                                                                                           new NullOperator("ParentCatalog"))).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                               pcs = parentCatalogs;
                           }
                           else
                           {
                               //child
                               String nodeOidString = e.NodeName;
                               Guid nodeOid;
                               if (Guid.TryParse(nodeOidString, out nodeOid))
                               {
                                   pcs = new XPCollection<PriceCatalog>(uow, new BinaryOperator("ParentCatalogOid", nodeOid)).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                               }
                               else
                               {
                                   throw new Exception("Invalid parent Oid: " + nodeOid);
                               }
                           }

                           foreach (PriceCatalog pc in pcs)
                           {
                               TreeViewVirtualNode tvvn = new TreeViewVirtualNode(pc.Oid.ToString(), pc.Code + " - " + pc.Description);
                               tvvn.IsLeaf = pc.PriceCatalogs.Count == 0;
                               children.Add(tvvn);
                           }

                       }
                       e.Children = children;
                   };
        }

        public static TreeViewVirtualModeCreateChildrenMethod VirtualModeCreateChildren<T>(CompanyNew owner) where T : CategoryNode
        {
            return (TreeViewVirtualModeCreateChildrenEventArgs e) =>
            {
                List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    IEnumerable<T> itcs = null;
                    if (e.NodeName == null)
                    {
                        //root
                        itcs = new XPCollection<T>(uow, RetailHelper.ApplyOwnerCriteria(new NullOperator("ParentOid"), typeof(T), owner)).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                    }
                    else
                    {
                        //child
                        string nodeOidString = e.NodeName;
                        Guid nodeOid;
                        if (Guid.TryParse(nodeOidString, out nodeOid))
                        {
                            itcs = new XPCollection<T>(uow, RetailHelper.ApplyOwnerCriteria(new BinaryOperator("ParentOid", nodeOid), typeof(T), owner)).OrderBy(g => g.Code).OrderBy(g => g.Code.Length);
                        }
                        else
                        {
                            //todo
                        }
                    }
                    foreach (T ic in itcs)
                    {
                        TreeViewVirtualNode tvvn = new TreeViewVirtualNode(ic.Oid.ToString(), ic.Code + " - " + ic.Description);
                        tvvn.IsLeaf = ic.ChildCategories.Count == 0;
                        children.Add(tvvn);
                    }
                }
                e.Children = children;
            };
        }
    }
}
