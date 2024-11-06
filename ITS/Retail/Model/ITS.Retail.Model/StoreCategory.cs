//-----------------------------------------------------------------------
// <copyright file="StoreCategory.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;

namespace ITS.Retail.Model
{
	//[Updater(Order = 240,
	//    Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class StoreCategory : CategoryNode
    {
        public StoreCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public StoreCategory(Session session)
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
        private string _FullDescription;
        [NonPersistent]
        public string FullCode
        {
            get
            {
                return GetFullCode();
            }
        }
        public string FullDescription
        {
            get
            {
                return _FullDescription;
            }
            set
            {
                SetPropertyValue("FullDescription", ref _FullDescription, value);
            }
        }
        public string GetFullCode()
        {
            if (this.Parent != null) 
            {
                return String.Format("{0}:{1}",Code,(Parent as ItemCategory).GetFullCode());
            }
            return Code;
        }
        public string GetFullDescription()
        {
            if (this.Parent != null)
            {
                return String.Format("{0}>{1}",Description,(Parent as ItemCategory).GetFullDescription());
            }
            return Description;
        }
        public XPCollection<T> GetAllNodeTreeStore<T>(CriteriaOperator crop = null)  //<-  Πρέπει να αλλαχθεί
        {

            return null;
        }
    }

}