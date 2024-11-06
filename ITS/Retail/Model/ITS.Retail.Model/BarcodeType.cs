//-----------------------------------------------------------------------
// <copyright file="BarcodeType.cs" company="ITS">
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
    [Updater(Order = 260, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("BarcodeType", typeof(ResourcesLib.Resources))]
    public class BarcodeType : Lookup2Fields, IRequiredOwner
    {
        public BarcodeType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BarcodeType(Session session)
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

        private bool _NonSpecialCharactersIncluded;
        private bool _PrefixIncluded;
        private bool _IsWeighed;
        private string _Prefix;
        private string _Mask;
        private bool _HasMixInformation;
        private string _EntityType;

        [DisplayOrder(Order = 6)]
        public bool IsWeighed     // αφορά barcode ζυγιζόμενου
        {
            get
            {
                return _IsWeighed;
            }
            set
            {
                SetPropertyValue("IsWeighed", ref _IsWeighed, value);
            }
        }


        [Indexed(Unique = false)]
        [DisplayOrder(Order = 4)]
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                SetPropertyValue("Prefix", ref _Prefix, value);
            }
        }


        [DisplayOrder(Order = 5)]
        public string Mask
        {
            get
            {
                return _Mask;
            }
            set
            {
                SetPropertyValue("Mask", ref _Mask, value);
            }
        }


        [DisplayOrder(Order = 7)]
        public bool HasMixInformation
        {
            get
            {
                return _HasMixInformation;
            }
            set
            {
                SetPropertyValue("HasMixInformation", ref _HasMixInformation, value);
            }
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
                        throw new Exception("BarcodeType.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        [Association("BarcodeType-StoreBarcodeTypes"), Indexed(Unique = false)]
        public XPCollection<StoreBarcodeType> Stores
        {
            get
            {
                return GetCollection<StoreBarcodeType>("Stores");
            }
        }

        [Association("BarcodeType-BarcodeTypeBarcodeTypes"), Aggregated]
        public XPCollection<DocumentTypeBarcodeType> DocumentTypeBarcodeTypes
        {
            get
            {
                return GetCollection<DocumentTypeBarcodeType>("DocumentTypeBarcodeTypes");
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }
            if (this.IsWeighed == false && this.DocumentTypeBarcodeTypes.Count > 0)
            {
                this.Session.Delete(this.DocumentTypeBarcodeTypes);//TODO verify safe deletion
            }
            base.OnSaving();
        }

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

        public int Length
        {
            get
            {
                int prefixLength = !string.IsNullOrEmpty(Prefix) ? Prefix.Length : 0;
                int maskLength = !string.IsNullOrEmpty(Mask) ? Mask.Length : 0;
                return prefixLength + maskLength;
            }
        }


        public bool PrefixIncluded
        {
            get
            {
                return _PrefixIncluded;
            }
            set
            {
                SetPropertyValue("PrefixIncluded", ref _PrefixIncluded, value);
            }
        }


        public bool NonSpecialCharactersIncluded
        {
            get
            {
                return _NonSpecialCharactersIncluded;
            }
            set
            {
                SetPropertyValue("NonSpecialCharactersIncluded", ref _NonSpecialCharactersIncluded, value);
            }
        }
    }
}