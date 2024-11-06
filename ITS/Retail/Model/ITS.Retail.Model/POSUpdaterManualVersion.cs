//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class POSUpdaterManualVersion : BaseObj
    {

        public POSUpdaterManualVersion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSUpdaterManualVersion(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        // Fields...
        private string _EntityName;
        private long _MaxUpdatedOnTicks;
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


        public long MaxUpdatedOnTicks
        {
            get
            {
                return _MaxUpdatedOnTicks;
            }
            set
            {
                SetPropertyValue("MaxUpdatedOnTicks", ref _MaxUpdatedOnTicks, value);
            }
        }

       
        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }

    }
}
