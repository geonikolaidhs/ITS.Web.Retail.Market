//-----------------------------------------------------------------------
// <copyright file="MesurmentUnits.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 265,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("MeasurementUnit", typeof(ResourcesLib.Resources))]
    public class MeasurementUnit : Lookup2Fields, IRequiredOwner
    {
        public MeasurementUnit()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MeasurementUnit(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public MeasurementUnit(string code, string description)
            : base(code, description)
        {
            
        }
        public MeasurementUnit(Session session, string code, string description)
            : base(session, code, description)
        {
            
        }


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("MeasurementUnit.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.MeasurementUnitType = eMeasurementUnitType.ORDER;
        }

        [Association("MesurmentUnits-ItemBarcodes")]
        public XPCollection<ItemBarcode> ItemBarcodes
        {
            get
            {
                return GetCollection<ItemBarcode>("ItemBarcodes");
            }
        }

        [Association("MeasurementUnits-Items")]
        public XPCollection<Item> Items
        {
            get
            {
                return GetCollection<Item>("Items");
            }
        }

        //// Fields...

        private eMeasurementUnitType _MeasurementUnitType;
        private bool _SupportDecimal;


        public bool SupportDecimal {
            get {
                return _SupportDecimal;
            }
            set {
                SetPropertyValue("SupportDecimal", ref _SupportDecimal, value);
            }
        }


        public eMeasurementUnitType MeasurementUnitType
        {
            get
            {
                return _MeasurementUnitType;
            }
            set
            {
                SetPropertyValue("MeasurementUnitType", ref _MeasurementUnitType, value);
            }
        }
	}

}