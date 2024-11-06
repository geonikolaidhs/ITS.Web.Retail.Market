//-----------------------------------------------------------------------
// <copyright file="Seasonality.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
/// <summary>
/// Εποχικότητα ειδών είναι lookup Field στα είδη
///
/// </summary>
    [Updater(Order = 400,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class Seasonality : Lookup2Fields, IRequiredOwner
    {
        public Seasonality()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Seasonality(Session session)
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
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("Seasonality.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }


        [Association("Seasonality-Items")]
        public XPCollection<Item> Items
        {
            get
            {
                return GetCollection<Item>("Items");
            }
        }
        
    }

}