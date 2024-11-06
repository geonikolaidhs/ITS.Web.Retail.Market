//-----------------------------------------------------------------------
// <copyright file="Buyer.cs" company="ITS">
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
    [Updater(Order = 280,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class Buyer : Lookup2Fields, IRequiredOwner
    {
        public Buyer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Buyer(Session session)
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
				    Type thisType = typeof(Buyer);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }
                    crop =  new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        [Association("Buyer-Items")]
        public XPCollection<Item> Items
        {
            get
            {
                return GetCollection<Item>("Items");
            }
        }
    }
}
