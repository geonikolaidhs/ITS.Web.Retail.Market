//-----------------------------------------------------------------------
// <copyright file="DocumentSequence.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 920,
        Permissions = eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER)]
    public class ZReportSequence : LookupField, IOwner
    {
        public ZReportSequence()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ZReportSequence(Session session)
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
        private int _ZReportNumber;
        private POS _POS;

        public POS POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }

        [DescriptionField]
        public int ZReportNumber
        {
            get
            {
                return _ZReportNumber;
            }
            set
            {
                SetPropertyValue("ZReportNumber", ref _ZReportNumber, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return POS.Store.Owner;
            }
        }
        
    }

}
