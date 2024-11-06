using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel.Model;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Master
{
    [Indices("ParentOid;GCRecord")]
    [SyncInfoIgnore]
    public class CategoryNode : Category , ICategoryNode
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
        }
        private Guid? _ParentOid;
        [Indexed("GCRecord;ObjectType", Unique=false)]
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

        public XPCollection<CategoryNode> ChildCategories
        {
            get 
            {
                return new XPCollection<CategoryNode>(this.Session, new BinaryOperator("ParentOid", Oid));
            }
        }

        public bool HasChild
        {
            get
            {
                return ChildCategories.Count == 0 ? false : true;
            }
        }


        public CategoryNode GetRoot(Session session)
        {
            if (this.ParentOid != null)
            {
                CategoryNode cnParent = this.Session.GetObjectByKey<CategoryNode>(this.ParentOid);
                return cnParent.GetRoot(this.Session);
            }
            return this;

        }

        public void MoveTo(CategoryNode newParent)
        {
            ParentOid = newParent.Oid;
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

        //public XPCollection<CategoryNode> GetAllNodes()
        //{
        //    CriteriaOperator coper = new InOperator("Oid", this.GetNodeIDs());
        //    return new XPCollection<CategoryNode>(this.Session, coper, new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
        //}


        protected override void OnDeleting()
        {
            if (HasChild)
                throw new Exception("This node has childs");
            else
                base.OnDeleting();
        }
    }
}
