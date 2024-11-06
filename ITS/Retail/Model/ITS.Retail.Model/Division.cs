//-----------------------------------------------------------------------
// <copyright file="Division.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;


namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 170,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class Division : LookupField, ITS.Retail.Platform.Kernel.Model.IDivision
    {
        public Division()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Division(Session session)
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

        [Association("Division-DocumentTypes")]
        public XPCollection<DocumentType> DocumentTypes
        {
            get
            {
                return GetCollection<DocumentType>("DocumentTypes");
            }
        }

        private eDivision _Section;
        public eDivision Section
        {
            get
            {
                return _Section;
            }
            set
            {
                SetPropertyValue("Section", ref _Section, value);
            }
        }
    }

}
