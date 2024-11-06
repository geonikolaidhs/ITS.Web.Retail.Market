//-----------------------------------------------------------------------
// <copyright file="DocumentStatus.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 150,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("DocumentStatus", typeof(ResourcesLib.Resources))]
    public class DocumentStatus : Lookup2Fields, IRequiredOwner
    {
        public DocumentStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            TakeSequence = false;
            ReadOnly = false;
            IsDefault = false;
        }


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:

                    Type thisType = typeof(DocumentStatus);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        private bool _TakeSequence;
        [DisplayOrder(Order = 5)]
        public bool TakeSequence
        {
            get
            {
                return _TakeSequence;
            }
            set
            {
                SetPropertyValue("TakeSequence", ref _TakeSequence, value);
            }
        }
        private bool _ReadOnly;
        [DisplayOrder (Order = 4)]
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                SetPropertyValue("ReadOnly", ref _ReadOnly, value);
            }
        }

        [Association("DocumentStatus-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

        [Aggregated,Association("DocumentStatus-ActionTypeDocStatuses")]
        public XPCollection<ActionTypeDocStatus> ActionTypeDocStatuses
        {
            get
            {
                return GetCollection<ActionTypeDocStatus>("ActionTypeDocStatuses");
            }
        }
    }
}