//-----------------------------------------------------------------------
// <copyright file="PriceCatalogDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITS.Retail.Model
{
    [Updater(Order = 660,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalogDetail", typeof(ResourcesLib.Resources))]

    [Indices("PriceCatalog;Item;Barcode;GCRecord;IsActive;Oid",
        "PriceCatalog;Barcode;GCRecord;IsActive;Oid",
        "PriceCatalog;Item;GCRecord;IsActive;Oid",
        "GCRecord;IsActive;PriceCatalog;Item")]
    public class PriceCatalogDetail : BaseObj, IPriceCatalogDetail
    {
        public PriceCatalogDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.'
            ValueChangedOn = CreatedOnTicks;
            MarkUp = 0;
            long yesterday = DateTime.Now.AddDays(-1).Ticks;
            TimeValueValidFrom = yesterday;
            TimeValueValidUntil = yesterday;
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PriceCatalogDetail.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("PriceCatalog.Owner.Oid", supplier.Oid);
                    break;
            }
            return crop;
        }

        private decimal _OldTimeValue;
        private decimal _TimeValue;
        private long _TimeValueValidUntil;
        private long _TimeValueValidFrom;
        private long _LabelPrintedOn;
        private decimal _MarkUp;
        private long _ValueChangedOn;
        private decimal _OldValue;
        private PriceCatalog _PriceCatalog;
        private decimal _DatabaseValue;
        private bool _VATIncluded;
        private decimal _UnitValue;
        private long _OldTimeValueValidFrom;
        private long _OldTimeValueValidUntil;
        private bool _LabelPrinted;
        private long _TimeValueChangedOn;
        private Item _Item;
        private Barcode _Barcode;
        private decimal _Discount;
        private decimal _RetailValue;
        [Indexed("Item;Barcode;GCRecord", Unique = true),
         Association("PriceCatalog-PriceCatalogDetails")]
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


        [Indexed(Unique = false), Association("Item-PriceCatalogs")]
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


        [Indexed(Unique = false), Association("Barcode-PriceCatalogDetails")]
        public Barcode Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }



        public decimal Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public decimal Value
        {
            get
            {
                long now = DateTime.Now.Ticks;
                PriceCatalogDetailTimeValue effectiveTimeValueObject = TimeValues
                                                                       .Where(x => x.IsActive && x.TimeValueValidFrom <= now && x.TimeValueValidUntil >= now && x.TimeValue > 0)
                                                                       .OrderBy(x => x.TimeValueRange).FirstOrDefault();
                if (effectiveTimeValueObject != null)
                {
                    return effectiveTimeValueObject.TimeValue;
                }
                return DatabaseValue;
            }
        }

        [Persistent("Value")]
        public decimal DatabaseValue
        {
            get
            {
                return _DatabaseValue;
            }
            set
            {
                SetPropertyValue("DatabaseValue", ref _DatabaseValue, value);
            }
        }

        public decimal TimeValue
        {
            get
            {
                return _TimeValue;
            }
            set
            {
                SetPropertyValue("TimeValue", ref _TimeValue, value);
            }
        }

        public bool VATIncluded
        {
            get
            {
                return _VATIncluded;
            }
            set
            {
                SetPropertyValue("VATIncluded", ref _VATIncluded, value);
            }
        }

        [NonPersistent]
        public DateTime TimeValueChangedOnDate
        {
            get
            {
                return new DateTime(TimeValueChangedOn);
            }
        }

        public long TimeValueChangedOn
        {
            get
            {
                return _TimeValueChangedOn;
            }
            set
            {
                SetPropertyValue("TimeValueChangedOn", ref _TimeValueChangedOn, value);
            }
        }

        [NonPersistent]
        public DateTime ValueChangedOnDate
        {
            get
            {
                return new DateTime(ValueChangedOn);
            }
        }

        public long ValueChangedOn
        {
            get
            {
                return _ValueChangedOn;
            }
            set
            {
                SetPropertyValue("ValueChangedOn", ref _ValueChangedOn, value);
            }
        }

        public decimal OldValue
        {
            get
            {
                return _OldValue;
            }
            set
            {
                SetPropertyValue("OldValue", ref _OldValue, value);
            }
        }

        [NonPersistent]
        public decimal RetailValue
        {
            get
            {
                return RetailValueWithoutDiscount * (1 - Discount);
            }
        }

        [NonPersistent]
        public decimal WholesaleValue
        {
            get
            {
                if (!VATIncluded || Item == null || Item.VatCategory == null)
                    return RetailValue;
                else
                {
                    VatLevel vatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                    if (vatLevel == null)
                    {
                        return RetailValue;
                    }
                    else
                    {
                        VatFactor vatFactor = this.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory.Oid", Item.VatCategory.Oid, BinaryOperatorType.Equal), new BinaryOperator("VatLevel.Oid", vatLevel.Oid, BinaryOperatorType.Equal)));
                        if ((vatFactor == null) || (vatFactor.Factor == 0))
                        {
                            return RetailValue;
                        }
                        else
                        {
                            return RetailValue / (1 + Math.Abs(vatFactor.Factor));
                        }
                    }
                }
            }
        }

        [NonPersistent]
        public decimal RetailValueWithoutDiscount
        {
            get
            {
                decimal _UnitPrice;
                if (VATIncluded || Item == null || Item.VatCategory == null)
                {
                    _UnitPrice = Value;
                }
                else
                {
                    VatLevel vatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                    if (vatLevel == null)
                    {
                        _UnitPrice = Value;
                    }
                    else
                    {
                        VatFactor vatFactor = this.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory.Oid", Item.VatCategory.Oid, BinaryOperatorType.Equal), new BinaryOperator("VatLevel.Oid", vatLevel.Oid, BinaryOperatorType.Equal)));
                        if ((vatFactor == null) || (vatFactor.Factor == 0))
                        {
                            _UnitPrice = Value;
                        }
                        else
                        {
                            _UnitPrice = Value * (1 + Math.Abs(vatFactor.Factor));
                        }
                    }
                }
                return _UnitPrice;
            }
        }


        [NonPersistent]
        public decimal OldRetailValue
        {
            get
            {
                decimal _UnitPrice;
                if (VATIncluded || Item == null || Item.VatCategory == null)
                {
                    _UnitPrice = OldValue;
                }
                else
                {
                    VatLevel vatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                    if (vatLevel == null)
                    {
                        _UnitPrice = OldValue;
                    }
                    else
                    {
                        VatFactor vatFactor = this.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory.Oid", Item.VatCategory.Oid, BinaryOperatorType.Equal), new BinaryOperator("VatLevel.Oid", vatLevel.Oid, BinaryOperatorType.Equal)));
                        if ((vatFactor == null) || (vatFactor.Factor == 0))
                        {
                            _UnitPrice = OldValue;
                        }
                        else
                        {
                            _UnitPrice = OldValue * (1 + Math.Abs(vatFactor.Factor));
                        }
                    }
                }
                return _UnitPrice * (1 - Discount);
            }
        }
        // Unit price are item price with out Vat

        public decimal GetUnitPrice()
        {
            decimal _UnitPrice;
            if (!VATIncluded)
            {
                _UnitPrice = Value;
            }
            else
            {
                if (Item == null)
                {
                    _UnitPrice = Value;
                }
                else
                {
                    if (Item.VatCategory == null)
                    {
                        _UnitPrice = Value;
                    }
                    else
                    {
                        VatLevel vatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                        if (vatLevel == null)
                        {
                            _UnitPrice = Value;
                        }
                        else
                        {
                            VatFactor vatFactor = this.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory.Oid", Item.VatCategory.Oid, BinaryOperatorType.Equal), new BinaryOperator("VatLevel.Oid", vatLevel.Oid, BinaryOperatorType.Equal)));
                            if ((vatFactor == null) || (vatFactor.Factor == 0))
                            {
                                _UnitPrice = Value;
                            }
                            else
                            {
                                _UnitPrice = Value / (1 + Math.Abs(vatFactor.Factor));
                            }
                        }
                    }
                }
            }
            return _UnitPrice;
        }
        public decimal GetUnitPriceWithVat()
        {
            decimal _UnitPrice;
            if (VATIncluded)
            {
                _UnitPrice = Value;
            }
            else
            {
                if (Item == null)
                {
                    _UnitPrice = Value;
                }
                else
                {
                    if (Item.VatCategory == null)
                    {
                        _UnitPrice = Value;
                    }
                    else
                    {
                        VatLevel vatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
                        if (vatLevel == null)
                        {
                            _UnitPrice = Value;
                        }
                        else
                        {
                            VatFactor vatFactor = this.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory.Oid", Item.VatCategory.Oid, BinaryOperatorType.Equal), new BinaryOperator("VatLevel.Oid", vatLevel.Oid, BinaryOperatorType.Equal)));
                            if ((vatFactor == null) || (vatFactor.Factor == 0))
                            {
                                _UnitPrice = Value;
                            }
                            else
                            {
                                _UnitPrice = Value * (1 + Math.Abs(vatFactor.Factor));
                            }
                        }
                    }
                }
            }
            return _UnitPrice;
        }

        public decimal MarkUp
        {
            get
            {
                return _MarkUp;
            }
            set
            {
                SetPropertyValue("MarkUp", ref _MarkUp, value);
            }
        }

        [NonPersistent]
        public decimal Margin
        {
            get
            {
                return MarkUp / (1 + MarkUp);
            }
            set
            {
                MarkUp = value / (1 - value);
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "LabelPrintedOn")
            {
                this.LabelPrinted = true;
            }
            else if (propertyName == "DatabaseValue" && (decimal)oldValue != (decimal)newValue)
            {
                this.OldValue = (decimal)oldValue;
                this.ValueChangedOn = DateTime.Now.Ticks;
            }
            else if (propertyName == "IsActive" && (bool)oldValue != (bool)newValue) //Quick Fix TODO
            {
                if ((bool)newValue)
                {
                    this.ValueChangedOn = DateTime.Now.Ticks;
                }
                else if (this.PriceCatalog != null && this.PriceCatalog.ParentCatalog != null && this.Item != null)
                {
                    PriceCatalogDetail pcd = this.Item.GetPriceCatalogDetail(this.PriceCatalog.ParentCatalog, this.Barcode);
                    if (pcd != null)
                    {
                        pcd.ValueChangedOn = DateTime.Now.Ticks;
                    }
                }
            }
            else if (propertyName == "TimeValue" && (decimal)oldValue != (decimal)newValue)
            {
                this.OldTimeValue = (decimal)oldValue;
                this.TimeValueChangedOn = DateTime.Now.Ticks;
            }
            else if (propertyName == "TimeValueValidFrom" && (long)oldValue != (long)newValue)
            {
                this.OldTimeValueValidFrom = (long)oldValue;
            }
            else if (propertyName == "TimeValueValidUntil" && (long)oldValue != (long)newValue)
            {
                this.OldTimeValueValidUntil = (long)oldValue;
            }

        }

        [NonPersistent]
        public decimal UnitValue
        {
            get
            {
                if (this.Item == null)
                {
                    return 0;
                }

                if (this.Item.ReferenceUnit > 0)
                {
                    _UnitValue = (this.Item.ContentUnit / this.Item.ReferenceUnit) * this.RetailValue;
                }
                else
                {
                    _UnitValue = 0;
                }
                return _UnitValue;
            }
        }

        [NonPersistent]
        public DateTime LabelPrintedOnDate
        {
            get
            {
                return new DateTime(LabelPrintedOn);
            }
        }

        public long LabelPrintedOn
        {
            get
            {
                return _LabelPrintedOn;
            }
            set
            {
                SetPropertyValue("LabelPrintedOn", ref _LabelPrintedOn, value);
            }
        }

        public bool LabelPrinted
        {
            get
            {
                return _LabelPrinted;
            }
            set
            {
                SetPropertyValue("LabelPrinted", ref _LabelPrinted, value);
            }
        }

        [NonPersistent]
        public DateTime TimeValueValidFromDate
        {
            get
            {
                return new DateTime(TimeValueValidFrom);
            }
            set
            {
                TimeValueValidFrom = value.Ticks;
            }
        }

        public long TimeValueValidFrom
        {
            get
            {
                return _TimeValueValidFrom;
            }
            set
            {
                SetPropertyValue("TimeValueValidFrom", ref _TimeValueValidFrom, value);
            }
        }


        [NonPersistent]
        public DateTime TimeValueValidUntilDate
        {
            get
            {
                return new DateTime(TimeValueValidUntil);
            }
            set
            {
                TimeValueValidUntil = value.Ticks;
            }
        }

        public long TimeValueValidUntil
        {
            get
            {
                return _TimeValueValidUntil;
            }
            set
            {
                SetPropertyValue("TimeValueValidUntil", ref _TimeValueValidUntil, value);
            }
        }

        /// <summary>
        /// Returns true if properties TimeValue, TimeValueValidFrom and TimeValueValidUntil have been set correctly.Otherwise false.
        /// </summary>
        public bool TimeValueIsValid
        {
            get
            {
                long now = DateTime.Now.Ticks;
                if (TimeValueValidFrom <= now && now <= TimeValueValidUntil && TimeValue <= 0)
                {
                    return false;
                }

                if (TimeValueValidFrom > TimeValueValidUntil)
                {
                    return false;
                }
                return true;
            }
        }

        public decimal OldTimeValue
        {
            get
            {
                return _OldTimeValue;
            }
            set
            {
                SetPropertyValue("OldTimeValue", ref _OldTimeValue, value);
            }
        }

        public long OldTimeValueValidFrom
        {
            get
            {
                return _OldTimeValueValidFrom;
            }
            set
            {
                SetPropertyValue("OldTimeValueValidFrom", ref _OldTimeValueValidFrom, value);
            }
        }

        public long OldTimeValueValidUntil
        {
            get
            {
                return _OldTimeValueValidUntil;
            }
            set
            {
                SetPropertyValue("OldTimeValueValidUntil", ref _OldTimeValueValidUntil, value);
            }
        }

        [Aggregated, Association("PriceCatalogDetail-TimeValues")]
        public XPCollection<PriceCatalogDetailTimeValue> TimeValues
        {
            get
            {
                return GetCollection<PriceCatalogDetailTimeValue>("TimeValues");
            }
        }

        IEnumerable<IPriceCatalogDetailTimeValue> IPriceCatalogDetail.TimeValues
        {
            get
            {
                return this.TimeValues;
            }
        }

        public string JsonWithDetails(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, object> dict = this.GetDict(settings, includeType, true);
            if (dict.ContainsKey("TimeValues") == false)
            {
                dict.Add("TimeValues", TimeValues.Select(g => g.GetDict(settings, includeType, true)).ToList());
            }
            else
            {
                dict["TimeValues"] = TimeValues.Select(g => g.GetDict(settings, includeType, true)).ToList();
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }

        public bool HasChangedOrHasTimeValueChanges
        {
            get
            {
                if (this.Session.IsNewObject(this)
                      || this.Session.IsObjectToDelete(this)
                      || this.Session.IsObjectToSave(this)
                       )
                {
                    return true;
                }

                foreach (PriceCatalogDetailTimeValue timeValue in this.TimeValues)
                {
                    if (timeValue.Session.IsNewObject(timeValue)
                      || timeValue.Session.IsObjectToDelete(timeValue)
                      || timeValue.Session.IsObjectToSave(timeValue)
                       )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [NonPersistent]
        public ItemBarcode ItemBarcode
        {
            get
            {
                return this.Item != null ? this.Item.ItemBarcodes.FirstOrDefault(x => x.Barcode == this.Barcode) : null;
            }
            set
            {
                if (value != null && value.Owner != null && this.PriceCatalog != null && this.PriceCatalog.Owner != null
                    && value.Owner.Oid == this.PriceCatalog.Owner.Oid)
                {
                    this.Item = value.Item;
                    this.Barcode = value.Barcode;
                }
            }
        }

        private PriceCatalogDetailTimeValue EffectivePriceCatalogDetailTimeValue
        {
            get
            {
                long nowTicks = DateTime.Now.Ticks;
                return this.TimeValues.FirstOrDefault(timeValue => timeValue.TimeValueValidFrom <= nowTicks && nowTicks <= timeValue.TimeValueValidUntil && timeValue.IsActive == true);
            }
        }


        [NonPersistent]
        public PriceCatalogDetailTimeValue FirstTimeValue { get; set; }
        [NonPersistent]
        public PriceCatalogDetailTimeValue SecondTimeValue { get; set; }
    }

}
