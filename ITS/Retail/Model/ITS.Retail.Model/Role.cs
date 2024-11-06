//-----------------------------------------------------------------------
// <copyright file="Role.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 120,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Role", typeof(ResourcesLib.Resources))]

    public class Role : LookupField, IOwner, IRole
    {
        public Role()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Role(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //GDPRActions = false;
            //GDPREnabled = false;
            // Place here your initialization code.
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
                        throw new Exception("Role.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        private CompanyNew _Owner;
        private eRoleType _Type;


        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        [Association("Role-Users")]
        public XPCollection<User> Users
        {
            get
            {
                return GetCollection<User>("Users");
            }
        }


        [Aggregated, Association("Role-RoleEntityAccessPermisions")]
        public XPCollection<RoleEntityAccessPermision> RoleEntityAccessPermisions
        {
            get
            {
                return GetCollection<RoleEntityAccessPermision>("RoleEntityAccessPermisions");
            }
        }

        [Association("Role-DocumentTypeRoles"), Aggregated]
        public XPCollection<DocumentTypeRole> DocumentTypeRoles
        {
            get
            {
                return GetCollection<DocumentTypeRole>("DocumentTypeRoles");
            }
        }

        [Association("CustomDataViews-Roles")]
        public XPCollection<CustomDataView> CustomDataViews
        {
            get
            {
                return GetCollection<CustomDataView>("CustomDataViews");
            }
        }

        public eRoleType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }
        #region "GDPREnabled"
        private bool _GDPREnabled;
        public bool GDPREnabled
        {
            get { return _GDPREnabled; }
            set { SetPropertyValue("GDPREnabled", ref _GDPREnabled, value); }
        }
        #endregion
        #region "GDPRActions"
        private bool _GDPRActions;
        public bool GDPRActions
        {
            get { return _GDPRActions; }
            set { SetPropertyValue("GDPRActions", ref _GDPRActions, value); }
        }
        #endregion
    }

}
