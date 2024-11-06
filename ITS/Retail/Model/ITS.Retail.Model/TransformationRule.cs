//-----------------------------------------------------------------------
// <copyright file="DocumentHeader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 690,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class TransformationRule : BaseObj, IOwner
    {
        public TransformationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TransformationRule(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.TransformationLevel = eTransformationLevel.DEFAULT;
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
                        throw new Exception("TransformationRule.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("InitialType.Owner.Oid", owner.Oid), new NullOperator("InitialType.Owner"));
                    break;
            }

            return crop;
        }

        // Fields
        private bool _IsDefault;
        private eTransformationLevel _TransformationLevel;
        private double _ValueTransformationFactor;

        private DocumentType _InitialType;

        private DocumentType _DerrivedType;
        private double _QtyTransformationFactor;

        [DescriptionField]
        public String Description
        {
            get
            {
                return String.Format("Transformation between {0}-{1}", InitialType.Description, DerrivedType.Description);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return InitialType.Owner;
            }
        }

        [Association("DocumentType-TransformationRulesTo")]
        public DocumentType InitialType
        {
            get
            {
                return _InitialType;
            }
            set
            {
                SetPropertyValue("InitialType", ref _InitialType, value);
            }
        }

        [Association("DocumentType-TransformationRulesFrom")]
        public DocumentType DerrivedType
        {
            get
            {
                return _DerrivedType;
            }
            set
            {
                SetPropertyValue("DerrivedType", ref _DerrivedType, value);
            }
        }

        public double QtyTransformationFactor
        {
            get
            {
                return _QtyTransformationFactor;
            }
            set
            {
                SetPropertyValue("TransformationFactor", ref _QtyTransformationFactor, value);
            }
        }


        public double ValueTransformationFactor
        {
            get
            {
                return _ValueTransformationFactor;
            }
            set
            {
                SetPropertyValue("ValueTransformationFactor", ref _ValueTransformationFactor, value);
            }
        }


        public eTransformationLevel TransformationLevel
        {
            get
            {
                return _TransformationLevel;
            }
            set
            {
                SetPropertyValue("TransformationLevel", ref _TransformationLevel, value);
            }
        }


        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if (this.IsDeleted == false && InitialType.Owner.Oid != DerrivedType.Owner.Oid)
            {
                throw new Exception("Initial Type and Derrived Type must have the same Owner");
            }
            base.OnSaving();
        }
    }
}
