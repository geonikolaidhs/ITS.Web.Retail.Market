//-----------------------------------------------------------------------
// <copyright file="Address.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 100, 
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class DeletedObject : BaseObj
    {
        public DeletedObject()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DeletedObject(Session session)
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
        /*
		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, Supplier supplier, Store store)
		{
			CriteriaOperator crop = null;
			

			return crop;
		}*/

        // Fields...
        private Guid _DeletedObjectOid;

        public Guid DeletedObjectOid
        {
            get
            {
                return _DeletedObjectOid;
            }
            set
            {
                SetPropertyValue("DeletedObjectOid", ref _DeletedObjectOid, value);
            }
        }
       
    }

}