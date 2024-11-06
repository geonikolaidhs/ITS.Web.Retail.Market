//-----------------------------------------------------------------------
// <copyright file="VatLevel.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 35,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("VatLevel", typeof(ResourcesLib.Resources))]

    public class VatLevel : Lookup2Fields
    {
        public VatLevel()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatLevel(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatLevel(string code, string description)
            : base(code, description)
        {
            
        }
        public VatLevel(Session session, string code, string description)
            : base(session, code, description)
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
                        throw new Exception("VatLevel.GetUpdaterCriteria(); Error: Owner is null");
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

        private bool _IsDefaultLevel;

        public bool IsDefaultLevel
        {
            get
            {
                return _IsDefaultLevel;
            }
            set
            {
                SetPropertyValue("IsDefaultLevel", ref _IsDefaultLevel, value);
            }
        }

        [Association("VatLevel-Customer")]
        public XPCollection<Customer> Customers
        {
            get
            {
                return GetCollection<Customer>("Customers");
            }
        }

        [Association("VatLevel-VatFactors")]
        public XPCollection<VatFactor> VatFactors
        {
            get
            {
                return GetCollection<VatFactor>("VatFactors");
            }
        }        
    }

}