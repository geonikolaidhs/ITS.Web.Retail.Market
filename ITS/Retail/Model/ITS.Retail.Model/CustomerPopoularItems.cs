//-----------------------------------------------------------------------
// <copyright file="CustomerPopoularItems.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.Drawing;
using System.Collections.Generic;

namespace ITS.Retail.Model
{

    public class CustomerPopoularItems : BaseObj
    {
        public CustomerPopoularItems()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerPopoularItems(Session session)
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


        // Fields...
        private double _Qty60;
        private double _Qty30;
        private double _Qty15;
        private double _Qty7;
        private double _Qty3;
        private double _QtyWeight;
        private Item _Item;
        private Customer _Customer;

        //[Association("Customer-CustomerPopoularItemss"),Indexed(Unique=false)]
        public Customer Customer {
            get {
                return _Customer;
            }
            set {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        //[Association("Item-CustomerPopoularItemss")]
        public Item Item {
            get {
                return _Item;
            }
            set {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        public double QtyWeight {
            get {
                return _QtyWeight;
            }
            set {
                SetPropertyValue("QtyWeight", ref _QtyWeight, value);
            }
        }

        public double Qty3{
            get {
                return _Qty3;
            }
            set {
                SetPropertyValue("Qty3", ref _Qty3, value);
            }
        }

        public double Qty7 {
            get {
                return _Qty7;
            }
            set {
                SetPropertyValue("Qty7", ref _Qty7, value);
            }
        }

        public double Qty15 {
            get {
                return _Qty15;
            }
            set {
                SetPropertyValue("Qty15", ref _Qty15, value);
            }
        }

        public double Qty30 {
            get {
                return _Qty30;
            }
            set {
                SetPropertyValue("Qty30", ref _Qty30, value);
            }
        }



        public double Qty60 {
            get {
                return _Qty60;
            }
            set {
                SetPropertyValue("Qty60", ref _Qty60, value);
            }
        }

        public void ComputeQtys() {

            XPCollection<DocumentDetail> OrdersDetail = new XPCollection<DocumentDetail>(this.Session);
            if (Customer == null || Item == null) {
                return;
            }
            IEnumerable<DocumentDetail> od60 = OrdersDetail.Where(g => g.DocumentHeader.Status.TakeSequence == true && 
                                                                        g.DocumentHeader.DocumentNumber > 0 &&
                                                                        g.DocumentHeader.Customer.Oid == this.Customer.Oid && 
                                                                        g.Item.Oid == this.Item.Oid).OrderByDescending(g => g.DocumentHeader.DocumentNumber).Take(60);
            if (od60.Count() == 0) return;
            Qty60 = od60.Count();
            Qty30 = od60.Take(30).Count();
            Qty15 = od60.Take(15).Count();
            Qty7 = od60.Take(7).Count();
            Qty3 = od60.Take(3).Count();
            QtyWeight = Qty60;
        }

    }
}
