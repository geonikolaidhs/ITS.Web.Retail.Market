using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 270,
    Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class Label : Lookup2Fields
    {
        public Label()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Label(Session session)
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
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("Label.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }
            return crop;
        }       

        public byte[] LabelFile
        {
            get
            {
                return this._LabelFile;
            }
            set
            {
                SetPropertyValue("LabelFile", ref this._LabelFile, value);
            }
        }

        [DisplayOrder (Order=3)]
        public string LabelFileName
        {
            get
            {
                return this._LabelFileName;
            }
            set
            {
                SetPropertyValue("LabelFileName", ref this._LabelFileName, value);
            }
        }

        // Fields...
        private string _LabelFileName;
        private byte[] _LabelFile;
        private bool _UseDirectSQL;
        private string _DirectSQL;
        private int _PrinterEncoding;
        private LabelPrintServiceSettings _PrintServiceSettings;

        public bool UseDirectSQL
        {
            get
            {
                return this._UseDirectSQL;
            }
            set
            {
                SetPropertyValue("UseDirectSQL", ref this._UseDirectSQL, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string DirectSQL
        {
            get
            {
                return this._DirectSQL;
            }
            set
            {
                SetPropertyValue("DirectSQL", ref this._DirectSQL, value);
            }
        }

        public int PrinterEncoding
        {
            get
            {
                return this._PrinterEncoding;
            }
            set
            {
                SetPropertyValue("PrinterEncoding", ref this._PrinterEncoding, value);
            }
        }

        [UpdaterIgnoreField]
        public LabelPrintServiceSettings PrintServiceSettings
        {
            get
            {
                return _PrintServiceSettings;
            }
            set
            {
                if (_PrintServiceSettings == value)
                {
                    return;
                }

                LabelPrintServiceSettings previousPrintServiceSettings = _PrintServiceSettings;
                _PrintServiceSettings = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousPrintServiceSettings != null && previousPrintServiceSettings.Label == this)
                {
                    previousPrintServiceSettings.Label = null;
                }


                if (_PrintServiceSettings != null)
                {
                    _PrintServiceSettings.Label = this;
                }

                OnChanged("PrintServiceSettings");
            }
        }
    }
}
