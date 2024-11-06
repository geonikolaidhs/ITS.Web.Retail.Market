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
    [Updater(Order = 160,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("SpecialItem", typeof(ResourcesLib.Resources))]

    public class SpecialItem : Lookup2Fields, IRequiredOwner
    {
        public SpecialItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SpecialItem(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public SpecialItem(string code, string description)
            : base(code, description)
        {
            
        }
        public SpecialItem(Session session, string code, string description)
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
                        throw new Exception("SpecialItem.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }
         
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        


        
    }

}