//-----------------------------------------------------------------------
// <copyright file="StoreAnalyticTree.cs" company="ITS">
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

namespace ITS.Retail.Model
{
	//[Updater(Order = 460,
	//    Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class StoreAnalyticTree : absAnalyticTree<Store>
    {
        public StoreAnalyticTree()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreAnalyticTree(Session session)
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

        [Association("Store-StoreAnalyticTrees")]
        public Store Object {
            get {
                return _Object;
            }
            set {
                SetPropertyValue("Object", ref _Object, value);
            }
        }

        //public override void GetData(Session myses, object item) {
        //    base.GetData(myses, item);
        //    StoreAnalyticTree sat = item as StoreAnalyticTree;
        //    Object = GetLookupObject<Store>(myses, sat.Object) as Store;
        //}
    }
}