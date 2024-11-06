//-----------------------------------------------------------------------
// <copyright file="Item.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using System.Drawing;
using DevExpress.Xpo.Metadata;
using ITS.Retail.ResourcesLib;
using System.Collections.Generic;

namespace ITS.Retail.Model
{


    [EntityDisplayName("ItemDeviceIndex", typeof(Resources))]
    public class ItemDeviceIndex : BaseObj
    {
        public ItemDeviceIndex()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ItemDeviceIndex(Session session)
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

        private ItemCategory _ItemCategory;
        private PriceCatalog _PriceCatalog;
        private Item _Item;
        private BarcodeType _BarcodeType;
        private int _DeviceIndex;
        public int DeviceIndex
        {
            get
            {
                return _DeviceIndex;
            }
            set
            {
                SetPropertyValue("DeviceIndex", ref _DeviceIndex, value);
            }
        }
        [DescriptionField]
        public string Description
        {
            get
            {
                return Oid.ToString();
            }
        }
        public ItemCategory ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("ItemCategory", ref _ItemCategory, value);
            }
        }
        public Item Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }
        public PriceCatalog PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }
        public BarcodeType BarcodeType
        {
            get
            {
                return _BarcodeType;
            }
            set
            {
                SetPropertyValue("BarcodeType", ref _BarcodeType, value);
            }
        }

    }
}


