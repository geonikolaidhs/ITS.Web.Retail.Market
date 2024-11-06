//-----------------------------------------------------------------------
// <copyright file="Country.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 310,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class Country : LookupField
    {
        public Country()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Country(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public Country(string description)
            : base(description)
        {
            
        }
        public Country(Session session, string description)
            : base(session, description)
        {
            
        }
         

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }
    }

}