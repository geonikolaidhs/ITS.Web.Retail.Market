//-----------------------------------------------------------------------
// <copyright file="TablePermission.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
	//[Updater(Order = 690,
	//    Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class TablePermission : Permission
    {
        public TablePermission()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TablePermission(Session session)
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

        private string _TableName;
        [Indexed("GCRecord", Unique = true)]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                SetPropertyValue("TableName", ref _TableName, value);
            }
        }

        [Association("TablePermission-FieldPermissions")]
        public XPCollection<FieldPermission> FieldPermissions
        {
            get
            {
                return GetCollection<FieldPermission>("FieldPermissions");
            }
        }

        private User _User;
        [Association("User-TablePermissions")]
        public User User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        //public override void GetData(Session myses, object item)
        //{
        //    base.GetData(myses, item);
        //    TablePermission tp = item as TablePermission;
        //    TableName = tp.TableName;
        //}


    }

}