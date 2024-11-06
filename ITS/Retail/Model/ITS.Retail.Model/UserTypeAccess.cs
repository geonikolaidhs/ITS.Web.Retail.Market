//-----------------------------------------------------------------------
// <copyright file="UserTypeAccess.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Linq;
using System.Collections.Generic;

namespace ITS.Retail.Model
{
    [Updater(Order = 140,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("UserTypeAccess", typeof(ResourcesLib.Resources))]

    public class UserTypeAccess : BaseObj
    {
        public UserTypeAccess()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserTypeAccess(Session session)
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

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    {
                        if (owner == null || store == null)
                        {
                            throw new Exception("UserTypeAccess.GetUpdaterCriteria(); Error: Owner or Store is null");
                        }

                        CriteriaOperator userTypeAccessOwnerCriteria = User.GetUpdaterCriteria(direction, owner, store, deviceID);

                        IEnumerable<Guid> userOids = owner.Session.Query<User>().AppendWhere(new CriteriaToExpressionConverter(), userTypeAccessOwnerCriteria).Cast<User>().Select(usr=>usr.Oid);

                        crop = CriteriaOperator.And(
                                                    new InOperator("User", userOids),
                                                    CriteriaOperator.Or(
                                                                        CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName),
                                                                                             new BinaryOperator("EntityOid", owner.Oid)
                                                                                             ),
                                                                        CriteriaOperator.And(new BinaryOperator("EntityType", typeof(Store).FullName),
                                                                                             new BinaryOperator("EntityOid", store.Oid)
                                                                                             )
                                                                        )
                                                   );
                    }
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    {
                        if (owner == null || store == null)
                        {
                            throw new Exception("UserTypeAccess.GetUpdaterCriteria(); Error: Supplier or Store is null");
                        }
                        CriteriaOperator crop1 = CriteriaOperator.And(
                                    CriteriaOperator.Or(
                                        new NotOperator(new AggregateOperand("Role.RoleEntityAccessPermisions", Aggregate.Exists)), //admins
                                        new ContainsOperator("UserTypeAccesses", CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName), new BinaryOperator("EntityOid", owner.Oid)))
                                    ),
                                    new NotOperator(new NullOperator("POSUserName"))
                                );

                        
                        XPCollection<User> users = new XPCollection<User>(owner.Session, crop1);
                        crop = CriteriaOperator.And(
                            new InOperator("User", users.Select(g => g.Oid)),
                            CriteriaOperator.Or(CriteriaOperator.And(new BinaryOperator("EntityType", typeof(CompanyNew).FullName), new BinaryOperator("EntityOid", owner.Oid)),
                                                   CriteriaOperator.And(new BinaryOperator("EntityType", typeof(Store).FullName), new BinaryOperator("EntityOid", store.Oid)))
                                                   );
                    }
                    break;  
			}

			return crop;
		}

        private User _User;
        [Association("User-UserTypeAccesses")]
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

        private Guid _EntityOid;
        [Indexed("User;GCRecord", Unique = true)]  // Ο συγκεκριμένος user για το συγκεκριμένο entity θα πρέπει να είναι μοναδικό
        public Guid EntityOid
        {
            get
            {
                return _EntityOid;
            }
            set
            {
                SetPropertyValue("EntityOid", ref _EntityOid, value);
            }
        }

        private string _EntityType;
        [vRequired()]
        [Indexed("GCRecord;User;EntityOid",Unique=false)]
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
    }
}