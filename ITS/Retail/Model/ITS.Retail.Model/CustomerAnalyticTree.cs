//-----------------------------------------------------------------------
// <copyright file="CustomerAnalyticTree.cs" company="ITS">
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
    [Updater(Order = 330,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("CustomerAnalyticTree", typeof(ResourcesLib.Resources))]
    public class CustomerAnalyticTree : absAnalyticTree<Customer>, ICustomerAnalyticTree
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("CustomerAnalyticTree.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Object.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        [Association("Customer-CustomerAnalyticTrees")]
        public Customer Object
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

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    CustomerAnalyticTree sat = item as CustomerAnalyticTree;
        //    Object = GetLookupObject<Customer>(myses, sat.Object) as Customer;
        //}

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
    }

}