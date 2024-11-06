//-----------------------------------------------------------------------
// <copyright file="VatCategory.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 70,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("VatCategory", typeof(ResourcesLib.Resources))]

    public class VatCategory : Lookup2Fields
    {
       
        public VatCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatCategory(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatCategory(string code, string description)
            : base(code, description)
        {
            
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
                        throw new Exception("VatCategory.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Association("VatCategory-Items")]
        public XPCollection<Item> Items
        {
            get
            {
                return GetCollection<Item>("Items");
            }
        }

        [Association("VatCategory-VatFactors")]
        public XPCollection<VatFactor> VatFactors
        {
            get
            {
                return GetCollection<VatFactor>("VatFactors");
            }
        }

        private eMinistryVatCategoryCode _MinistryVatCategoryCode;
        public eMinistryVatCategoryCode MinistryVatCategoryCode
        {
            get
            {
                return _MinistryVatCategoryCode;
            }
            set
            {
                SetPropertyValue("MinistryVatCategoryCode", ref _MinistryVatCategoryCode, value);
            }

        }

        public XPCollection<Item> GetItems()
        {
            return new XPCollection<Item>(new BinaryOperator("VatCategory.Oid", Oid, BinaryOperatorType.Equal), new SortProperty("VatCategory.Code", SortingDirection.Ascending));
        }

    }

}