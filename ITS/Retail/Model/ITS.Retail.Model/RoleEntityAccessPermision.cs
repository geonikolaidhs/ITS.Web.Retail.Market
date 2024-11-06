//-----------------------------------------------------------------------
// <copyright file="RoleEntityAccessPermision.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 570,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class RoleEntityAccessPermision : BaseObj
    {
        public RoleEntityAccessPermision()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public RoleEntityAccessPermision(Session session)
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

        private Role _Role;
        [Association("Role-RoleEntityAccessPermisions")]
        public Role Role
        {
            get
            {
                return _Role;
            }
            set
            {
                SetPropertyValue("Role", ref _Role, value);
            }
        }

        private EntityAccessPermision _EnityAccessPermision;
        [Association("EntityAccessPermision-RoleEntityAccessPermisions")]
        public EntityAccessPermision EnityAccessPermision
        {
            get
            {
                return _EnityAccessPermision;
            }
            set
            {
                SetPropertyValue("EnityAccessPermision", ref _EnityAccessPermision, value);
            }
        }
    }

}