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
    //[Updater(Order = 450,
    //    Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
	public class UpdaterMode : BaseObj
	{
		
		public UpdaterMode()
			: base()
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

		public UpdaterMode(Session session)
			: base(session)
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

        // Fields...
        private eUpdaterMode _Mode;
        private string _EntityName;

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


        public eUpdaterMode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                SetPropertyValue("Mode", ref _Mode, value);
            }
        }
    }
}
