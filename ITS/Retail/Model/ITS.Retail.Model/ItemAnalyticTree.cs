//-----------------------------------------------------------------------
// <copyright file="ItemAnalyticTree.cs" company="ITS">
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
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.OwnTable)]
    [Updater(Order = 450,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("ItemAnalyticTree", typeof(ResourcesLib.Resources))]
    [Indices("GCRecord;Root", "GCRecord;Node;Object", "GCRecord;Node;Object;Root", "Object;GCRecord;Oid;Root;Node")]
    public class ItemAnalyticTree : absAnalyticTree<Item>, IItemAnalyticTree
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("ItemAnalyticTree.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Object.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }


        [Association("Item-ItemAnalyticTrees"), Indexed(Unique = false)]
        public Item Object
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

        public Guid ObjectOid
        {
            get
            {
                if (this.Object == null)
                {
                    return Guid.Empty;
                }
                return this.Object.Oid;
            }
        }

        public Guid NodeOid
        {
            get
            {
                if (this.Node == null)
                {
                    return Guid.Empty;
                }
                return this.Node.Oid;
            }
        }


        private int _ShowOrder;
        public int ShowOrder
        {
            get
            {
                return _ShowOrder;
            }
            set
            {
                SetPropertyValue("ShowOrder", ref _ShowOrder, value);
            }
        }


    }
}