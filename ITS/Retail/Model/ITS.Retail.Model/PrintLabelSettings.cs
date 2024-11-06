using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 280)]
    [IsDefaultUniqueFields(UniqueFields = new string[] {"Store"})]
    public class PrintLabelSettings : BaseObj
    {
        private Label _Label;
        private int _Copies;
        private string _PortName;
        private string _PrintingType;
        private Model.Store _Store;
        private string _Code;
        private bool _IsDefault;
        private string _Description;
        private int? _PrinterEncoding;

        public PrintLabelSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PrintLabelSettings(Session session)
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
                    if (store == null)
                    {
                        throw new Exception("Label.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = new BinaryOperator("Store.Oid", store.Oid);
                        //CriteriaOperator.Or(new BinaryOperator("Label.Owner.Oid", owner.Oid), new NullOperator("Label.Owner"));
                    break;
            }

            return crop;
        }

        public Label Label
        {
            get
            {
                return _Label;
            }
            set
            {
                SetPropertyValue("Label", ref this._Label, value);
            }
        }

        public int Copies
        {
            get
            {
                return _Copies;
            }
            set
            {
                SetPropertyValue("Copies", ref _Copies, value);
            }
        }

        public string PortName
        {
            get
            {
                return _PortName;
            }
            set
            {
                SetPropertyValue("PortName", ref _PortName, value);
            }
        }

        public string PrintingType
        {
            get
            {
                return _PrintingType;
            }
            set
            {
                SetPropertyValue("PrintingType", ref _PrintingType, value);
            }
        }

        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        [Indexed("Store;GCRecord", Unique = true)]
        public string Code
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

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public int? PrinterEncoding
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
    }
}
