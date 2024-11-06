//-----------------------------------------------------------------------
// <copyright file="VatFactor.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 90,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("VatFactor", typeof(ResourcesLib.Resources))]

    public class VatFactor : Lookup2Fields
    {
        public VatFactor()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatFactor(Session session)
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
                        throw new Exception("VatFactor.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        private decimal _Factor;
        public decimal Factor
        {
            get
            {
                return _Factor;
            }
            set
            {
                SetPropertyValue("Factor", ref _Factor, value);
            }
        }

        private VatLevel _VatLevel;
        [Association("VatLevel-VatFactors")]
        public VatLevel VatLevel
        {
            get
            {
                return _VatLevel;
            }
            set
            {
                SetPropertyValue("VatLevel", ref _VatLevel, value);
            }
        }

        private VatCategory _VatCategory;
        [Association("VatCategory-VatFactors")]
        public VatCategory VatCategory
        {
            get
            {
                return _VatCategory;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatCategory, value);
            }
        }

        [Association("VatFactor-UserDailyTotalsDetails")]
        public XPCollection<UserDailyTotalsDetail> UserDailyTotalsDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsDetail>("UserDailyTotalsDetails");
            }
        }

        [Association("VatFactor-DailyTotalsDetails")]
        public XPCollection<DailyTotalsDetail> DailyTotalsDetails
        {
            get
            {
                return GetCollection<DailyTotalsDetail>("DailyTotalsDetails");
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if (this.IsDeleted == false && VatLevel.Owner != null && VatCategory.Owner != null && VatLevel.Owner.Oid != VatCategory.Owner.Oid)
            {
                throw new Exception("VatLevel and VatCategory must have the same Owner");
            }
            base.OnSaving();
        }

        public decimal Value
        {
            get
            {
                return this.Factor * 100m;
            }
        }

        public string ValueString
        {
            get
            {
                return Value.ToString("N2") + " %";
            }
        }


    }

}