using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class ImportFileRecordField : BaseObj
    {
         public ImportFileRecordField()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public ImportFileRecordField(Session session)
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
        
        private string _ConstantValue;
        private string _DefaultValue;
        private bool _Trim;
        private string _PaddingCharacter;
        private bool _Padding;
        private int _Length;
        private int _Position;
        private string _PropertyName;
        private double _Multiplier;
        private bool _AllowNull;
        private bool _UseThirdPartNum;

        [DescriptionField]
        [DisplayOrder(Order = 1)]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                SetPropertyValue("PropertyName", ref _PropertyName, value);
            }
        }

        [DisplayOrder(Order = 2)]
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }
        [DisplayOrder(Order = 6)]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                SetPropertyValue("Length", ref _Length, value);
            }
        }

        [DisplayOrder(Order = 4)]
        public bool Padding
        {
            get
            {
                return _Padding;
            }
            set
            {
                SetPropertyValue("Padding", ref _Padding, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public string PaddingCharacter
        {
            get
            {
                return _PaddingCharacter;
            }
            set
            {
                SetPropertyValue("PaddingCharacter", ref _PaddingCharacter, value);
            }
        }

        [DisplayOrder(Order = 9)]
        public bool Trim
        {
            get
            {
                return _Trim;
            }
            set
            {
                SetPropertyValue("Trim", ref _Trim, value);
            }
        }

        [DisplayOrder(Order = 3)]
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                SetPropertyValue("DefaultValue", ref _DefaultValue, value);
            }
        }

        [DisplayOrder(Order = 7)]
        public string ConstantValue
        {
            get
            {
                return _ConstantValue;
            }
            set
            {
                SetPropertyValue("ConstantValue", ref _ConstantValue, value);
            }
        }
        [DisplayOrder(Order = 8)]
        public double Multiplier
        {
            get
            {
                return _Multiplier;
            }
            set
            {
                SetPropertyValue("Multiplier", ref _Multiplier, value);
            }
        }
        [DisplayOrder(Order = 10)]
        public bool AllowNull
        {
            get
            {
                return _AllowNull;
            }
            set
            {
                SetPropertyValue("AllowNull", ref _AllowNull, value);
            }
        }
        [DisplayOrder(Order = 11)]
        public bool UseThirdPartNum
        {
            get
            {
                return _UseThirdPartNum;
            }
            set
            {
                SetPropertyValue("UseThirdPartNum", ref _UseThirdPartNum, value);
            }
        }
    }
}
