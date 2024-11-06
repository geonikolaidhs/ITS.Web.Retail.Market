//-----------------------------------------------------------------------
// <copyright file="EntityAccessPermision.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 110,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("EntityAccessPermision", typeof(ResourcesLib.Resources))]
    public class EntityAccessPermision : Permission
    {
        public EntityAccessPermision()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public EntityAccessPermision(Session session)
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

        private string _EntityType;
        [DescriptionField]
        public string EntityType
        {
            get
            {
                return _EntityType;
            }
            set
            {
                SetPropertyValue("EntityType", ref _EntityType, value);
            }
        }

        [Association("EntityAccessPermision-RoleEntityAccessPermisions")]
        public XPCollection<RoleEntityAccessPermision> RoleEntityAccessPermisions
        {
            get
            {
                return GetCollection<RoleEntityAccessPermision>("RoleEntityAccessPermisions");
            }
        }
    }
}