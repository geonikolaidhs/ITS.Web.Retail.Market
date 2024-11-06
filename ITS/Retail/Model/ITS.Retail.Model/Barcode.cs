//-----------------------------------------------------------------------
// <copyright file="Barcode.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 270,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Barcode", typeof(Resources))]
    public class Barcode : BaseObj
    {
        public Barcode()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Barcode(Session session)
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
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("Barcode.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new ContainsOperator( "ItemBarcodes",new BinaryOperator("Owner.Oid", owner.Oid));
                    break;
            }

            return crop;
        }

        private string _Code;
        [Indexed("GCRecord",Unique = true)]
        [DescriptionField]
        public string Code   // item barcode
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        public BarcodeType Type(CompanyNew owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(owner);
            return itemBarcode == null ? null : itemBarcode.Type;
        }

        [Association("Barcode-ItemBarcodes"), Aggregated]
        public XPCollection<ItemBarcode> ItemBarcodes
        {
            get
            {
                return GetCollection<ItemBarcode>("ItemBarcodes");
            }
        }

        public MeasurementUnit MeasurementUnit(CompanyNew owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(owner);
            return itemBarcode==null ? null : itemBarcode.MeasurementUnit;
        }

        public ItemBarcode ItemBarcode(CompanyNew Owner)
        {
            if (Owner == null)
            {
                throw new Exception("Barcode Owner Cannot be null");
            }

            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Barcode.Oid", this.Oid), new BinaryOperator("Owner.Oid", Owner.Oid));
            return this.Session.FindObject<ItemBarcode>(crop);
        }

        [Association("Barcode-PriceCatalogDetails")]
        public XPCollection<PriceCatalogDetail> PriceCatalogDetails
        {
            get
            {
                return GetCollection<PriceCatalogDetail>("PriceCatalogDetails");
            }
        }

        [Association("Barcode-LeafletDetails")]
        public XPCollection<LeafletDetail> LeafletDetails
        {
            get
            {
                return GetCollection<LeafletDetail>("LeafletDetails");
            }
        }
    }

}