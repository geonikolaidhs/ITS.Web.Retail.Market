using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.Model
{
    [Updater(Order = 1160, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class VariableValues : BaseObj, IRequiredOwner
    {
        private Guid _EntityOid;
        private Guid _ActionTypeOid;
        private decimal _DecimalField1;
        private decimal _DecimalField2;
        private decimal _DecimalField3;
        private decimal _DecimalField4;
        private decimal _DecimalField5;
        private decimal _DecimalField6;
        private decimal _DecimalField7;
        private decimal _DecimalField8;
        private decimal _DecimalField9;
        private decimal _DecimalField10;
        private decimal _ITSDecimalField11;
        private decimal _ITSDecimalField12;
        private decimal _ITSDecimalField13;
        private decimal _ITSDecimalField14;
        private decimal _ITSDecimalField15;
        private DateTime? _DateTimeField1;
        private DateTime? _DateTimeField2;
        private DateTime? _DateTimeField3;
        private DateTime? _DateTimeField4;
        private DateTime? _DateTimeField5;
        private DateTime? _DateTimeField6;
        private DateTime? _DateTimeField7;
        private DateTime? _DateTimeField8;
        private DateTime? _DateTimeField9;
        private DateTime? _DateTimeField10;
        private DateTime? _ITSDateTimeField11;
        private DateTime? _ITSDateTimeField12;
        private DateTime? _ITSDateTimeField13;
        private DateTime? _ITSDateTimeField14;
        private DateTime? _ITSDateTimeField15;
        private string _StringField1;
        private string _StringField2;
        private string _StringField3;
        private string _StringField4;
        private string _StringField5;
        private string _StringField6;
        private string _StringField7;
        private string _StringField8;
        private string _StringField9;
        private string _StringField10;
        private string _ITSStringField11;
        private string _ITSStringField12;
        private string _ITSStringField13;
        private string _ITSStringField14;
        private string _ITSStringField15;
        private Guid? _GuidField1;
        private Guid? _GuidField2;
        private Guid? _GuidField3;
        private Guid? _GuidField4;
        private Guid? _GuidField5;
        private Guid? _GuidField6;
        private Guid? _GuidField7;
        private Guid? _GuidField8;
        private Guid? _GuidField9;
        private Guid? _GuidField10;
        private Guid? _ITSGuidField11;
        private Guid? _ITSGuidField12;
        private Guid? _ITSGuidField13;
        private Guid? _ITSGuidField14;
        private Guid? _ITSGuidField15;
        private bool _BooleanField1;
        private bool _BooleanField2;
        private bool _BooleanField3;
        private bool _BooleanField4;
        private bool _BooleanField5;
        private bool _BooleanField6;
        private bool _BooleanField7;
        private bool _BooleanField8;
        private bool _BooleanField9;
        private bool _BooleanField10;
        private bool _ITSBooleanField11;
        private bool _ITSBooleanField12;
        private bool _ITSBooleanField13;
        private bool _ITSBooleanField14;
        private bool _ITSBooleanField15;
        private CompanyNew _Owner;
        private string _Description;
        private eTotalizersUpdateMode _UpdateMode;
        private Store _Store;

        public VariableValues()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VariableValues(Session session)
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
            Type thisType = typeof(VariableValues);            
            if (owner == null)
            {
                throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
            }
            CriteriaOperator crop = new BinaryOperator("Owner.Oid", owner.Oid);
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception(thisType.Name+".GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = CriteriaOperator.And(crop, new BinaryOperator("UpdateMode", eTotalizersUpdateMode.GLOBAL));
                    break;
                    //case eUpdateDirection.STORECONTROLLER_TO_MASTER:
                    //    if (store == null)
                    //    {
                    //        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Error: Store is null");
                    //    }
                    //    crop = CriteriaOperator.And(crop, new BinaryOperator("UpdateMode", eTotalizersUpdateMode.STORE));
                    //    break;
            }
            return crop;
        }

        public Guid ActionTypeOid
        {
            get
            {
                return _ActionTypeOid;
            }
            set
            {
                SetPropertyValue("ActionType", ref _ActionTypeOid, value);
            }
        }

        public Guid EntityOid
        {
            get
            {
                return _EntityOid;
            }
            set
            {
                SetPropertyValue("EntityOid", ref _EntityOid, value);
            }
        }

        public decimal DecimalField1
        {
            get
            {
                return _DecimalField1;
            }
            set
            {
                SetPropertyValue("DecimalField1", ref _DecimalField1, value);
            }
        }

        public decimal DecimalField2
        {
            get
            {
                return _DecimalField2;
            }
            set
            {
                SetPropertyValue("DecimalField2", ref _DecimalField2, value);
            }
        }
        public decimal DecimalField3
        {
            get
            {
                return _DecimalField3;
            }
            set
            {
                SetPropertyValue("DecimalField3", ref _DecimalField3, value);
            }
        }

        public decimal DecimalField4
        {
            get
            {
                return _DecimalField4;
            }
            set
            {
                SetPropertyValue("DecimalField4", ref _DecimalField4, value);
            }
        }

        public decimal DecimalField5
        {
            get
            {
                return _DecimalField5;
            }
            set
            {
                SetPropertyValue("DecimalField5", ref _DecimalField5, value);
            }
        }

        public decimal DecimalField6
        {
            get
            {
                return _DecimalField6;
            }
            set
            {
                SetPropertyValue("DecimalField6", ref _DecimalField6, value);
            }
        }

        public decimal DecimalField7
        {
            get
            {
                return _DecimalField7;
            }
            set
            {
                SetPropertyValue("DecimalField7", ref _DecimalField7, value);
            }
        }

        public decimal DecimalField8
        {
            get
            {
                return _DecimalField8;
            }
            set
            {
                SetPropertyValue("DecimalField1", ref _DecimalField8, value);
            }
        }

        public decimal DecimalField9
        {
            get
            {
                return _DecimalField9;
            }
            set
            {
                SetPropertyValue("DecimalField1", ref _DecimalField9, value);
            }
        }

        public decimal DecimalField10
        {
            get
            {
                return _DecimalField10;
            }
            set
            {
                SetPropertyValue("DecimalField10", ref _DecimalField10, value);
            }
        }

        public decimal ITSDecimalField11
        {
            get
            {
                return _ITSDecimalField11;
            }
            set
            {
                SetPropertyValue("ITSDecimalField11", ref _ITSDecimalField11, value);
            }
        }

        public decimal ITSDecimalField12
        {
            get
            {
                return _ITSDecimalField12;
            }
            set
            {
                SetPropertyValue("ITSDecimalField12", ref _ITSDecimalField12, value);
            }
        }

        public decimal ITSDecimalField13
        {
            get
            {
                return _ITSDecimalField13;
            }
            set
            {
                SetPropertyValue("ITSDecimalField13", ref _ITSDecimalField13, value);
            }
        }

        public decimal ITSDecimalField14
        {
            get
            {
                return _ITSDecimalField14;
            }
            set
            {
                SetPropertyValue("ITSDecimalField14", ref _ITSDecimalField14, value);
            }
        }

        public decimal ITSDecimalField15
        {
            get
            {
                return _ITSDecimalField15;
            }
            set
            {
                SetPropertyValue("ITSDecimalField15", ref _ITSDecimalField15, value);
            }
        }

        public DateTime? DateTimeField1
        {
            get
            {
                return _DateTimeField1;
            }
            set
            {
                SetPropertyValue("DateTimeField1", ref _DateTimeField1, value);
            }
        }

        public DateTime? DateTimeField2
        {
            get
            {
                return _DateTimeField2;
            }
            set
            {
                SetPropertyValue("DateTimeField2", ref _DateTimeField2, value);
            }
        }
        public DateTime? DateTimeField3
        {
            get
            {
                return _DateTimeField3;
            }
            set
            {
                SetPropertyValue("DateTimeField3", ref _DateTimeField3, value);
            }
        }

        public DateTime? DateTimeField4
        {
            get
            {
                return _DateTimeField4;
            }
            set
            {
                SetPropertyValue("DateTimeField4", ref _DateTimeField4, value);
            }
        }

        public DateTime? DateTimeField5
        {
            get
            {
                return _DateTimeField5;
            }
            set
            {
                SetPropertyValue("DateTimeField5", ref _DateTimeField5, value);
            }
        }

        public DateTime? DateTimeField6
        {
            get
            {
                return _DateTimeField6;
            }
            set
            {
                SetPropertyValue("DateTimeField6", ref _DateTimeField6, value);
            }
        }

        public DateTime? DateTimeField7
        {
            get
            {
                return _DateTimeField7;
            }
            set
            {
                SetPropertyValue("DateTimeField7", ref _DateTimeField7, value);
            }
        }

        public DateTime? DateTimeField8
        {
            get
            {
                return _DateTimeField8;
            }
            set
            {
                SetPropertyValue("DateTimeField1", ref _DateTimeField8, value);
            }
        }

        public DateTime? DateTimeField9
        {
            get
            {
                return _DateTimeField9;
            }
            set
            {
                SetPropertyValue("DateTimeField1", ref _DateTimeField9, value);
            }
        }

        public DateTime? DateTimeField10
        {
            get
            {
                return _DateTimeField10;
            }
            set
            {
                SetPropertyValue("DateTimeField10", ref _DateTimeField10, value);
            }
        }

        public DateTime? ITSDateTimeField11
        {
            get
            {
                return _ITSDateTimeField11;
            }
            set
            {
                SetPropertyValue("ITSDateTimeField11", ref _ITSDateTimeField11, value);
            }
        }

        public DateTime? ITSDateTimeField12
        {
            get
            {
                return _ITSDateTimeField12;
            }
            set
            {
                SetPropertyValue("ITSDateTimeField12", ref _ITSDateTimeField12, value);
            }
        }

        public DateTime? ITSDateTimeField13
        {
            get
            {
                return _ITSDateTimeField13;
            }
            set
            {
                SetPropertyValue("ITSDateTimeField13", ref _ITSDateTimeField13, value);
            }
        }

        public DateTime? ITSDateTimeField14
        {
            get
            {
                return _ITSDateTimeField14;
            }
            set
            {
                SetPropertyValue("ITSDateTimeField14", ref _ITSDateTimeField14, value);
            }
        }

        public DateTime? ITSDateTimeField15
        {
            get
            {
                return _ITSDateTimeField15;
            }
            set
            {
                SetPropertyValue("ITSDateTimeField15", ref _ITSDateTimeField15, value);
            }
        }

        public string StringField1
        {
            get
            {
                return _StringField1;
            }
            set
            {
                SetPropertyValue("StringField1", ref _StringField1, value);
            }
        }

        public string StringField2
        {
            get
            {
                return _StringField2;
            }
            set
            {
                SetPropertyValue("StringField2", ref _StringField2, value);
            }
        }
        public string StringField3
        {
            get
            {
                return _StringField3;
            }
            set
            {
                SetPropertyValue("StringField3", ref _StringField3, value);
            }
        }

        public string StringField4
        {
            get
            {
                return _StringField4;
            }
            set
            {
                SetPropertyValue("StringField4", ref _StringField4, value);
            }
        }

        public string StringField5
        {
            get
            {
                return _StringField5;
            }
            set
            {
                SetPropertyValue("StringField5", ref _StringField5, value);
            }
        }

        public string StringField6
        {
            get
            {
                return _StringField6;
            }
            set
            {
                SetPropertyValue("StringField6", ref _StringField6, value);
            }
        }

        public string StringField7
        {
            get
            {
                return _StringField7;
            }
            set
            {
                SetPropertyValue("StringField7", ref _StringField7, value);
            }
        }

        public string StringField8
        {
            get
            {
                return _StringField8;
            }
            set
            {
                SetPropertyValue("StringField1", ref _StringField8, value);
            }
        }

        public string StringField9
        {
            get
            {
                return _StringField9;
            }
            set
            {
                SetPropertyValue("StringField1", ref _StringField9, value);
            }
        }

        public string StringField10
        {
            get
            {
                return _StringField10;
            }
            set
            {
                SetPropertyValue("StringField10", ref _StringField10, value);
            }
        }

        public string ITSStringField11
        {
            get
            {
                return _ITSStringField11;
            }
            set
            {
                SetPropertyValue("ITSStringField11", ref _ITSStringField11, value);
            }
        }

        public string ITSStringField12
        {
            get
            {
                return _ITSStringField12;
            }
            set
            {
                SetPropertyValue("ITSStringField12", ref _ITSStringField12, value);
            }
        }

        public string ITSStringField13
        {
            get
            {
                return _ITSStringField13;
            }
            set
            {
                SetPropertyValue("ITSStringField13", ref _ITSStringField13, value);
            }
        }

        public string ITSStringField14
        {
            get
            {
                return _ITSStringField14;
            }
            set
            {
                SetPropertyValue("ITSStringField14", ref _ITSStringField14, value);
            }
        }

        public string ITSStringField15
        {
            get
            {
                return _ITSStringField15;
            }
            set
            {
                SetPropertyValue("ITSStringField15", ref _ITSStringField15, value);
            }
        }

        public Guid? GuidField1
        {
            get
            {
                return _GuidField1;
            }
            set
            {
                SetPropertyValue("GuidField1", ref _GuidField1, value);
            }
        }

        public Guid? GuidField2
        {
            get
            {
                return _GuidField2;
            }
            set
            {
                SetPropertyValue("GuidField2", ref _GuidField2, value);
            }
        }
        public Guid? GuidField3
        {
            get
            {
                return _GuidField3;
            }
            set
            {
                SetPropertyValue("GuidField3", ref _GuidField3, value);
            }
        }

        public Guid? GuidField4
        {
            get
            {
                return _GuidField4;
            }
            set
            {
                SetPropertyValue("GuidField4", ref _GuidField4, value);
            }
        }

        public Guid? GuidField5
        {
            get
            {
                return _GuidField5;
            }
            set
            {
                SetPropertyValue("GuidField5", ref _GuidField5, value);
            }
        }

        public Guid? GuidField6
        {
            get
            {
                return _GuidField6;
            }
            set
            {
                SetPropertyValue("GuidField6", ref _GuidField6, value);
            }
        }

        public Guid? GuidField7
        {
            get
            {
                return _GuidField7;
            }
            set
            {
                SetPropertyValue("GuidField7", ref _GuidField7, value);
            }
        }

        public Guid? GuidField8
        {
            get
            {
                return _GuidField8;
            }
            set
            {
                SetPropertyValue("GuidField1", ref _GuidField8, value);
            }
        }

        public Guid? GuidField9
        {
            get
            {
                return _GuidField9;
            }
            set
            {
                SetPropertyValue("GuidField1", ref _GuidField9, value);
            }
        }

        public Guid? GuidField10
        {
            get
            {
                return _GuidField10;
            }
            set
            {
                SetPropertyValue("GuidField10", ref _GuidField10, value);
            }
        }

        public Guid? ITSGuidField11
        {
            get
            {
                return _ITSGuidField11;
            }
            set
            {
                SetPropertyValue("ITSGuidField11", ref _ITSGuidField11, value);
            }
        }

        public Guid? ITSGuidField12
        {
            get
            {
                return _ITSGuidField12;
            }
            set
            {
                SetPropertyValue("ITSGuidField12", ref _ITSGuidField12, value);
            }
        }

        public Guid? ITSGuidField13
        {
            get
            {
                return _ITSGuidField13;
            }
            set
            {
                SetPropertyValue("ITSGuidField13", ref _ITSGuidField13, value);
            }
        }

        public Guid? ITSGuidField14
        {
            get
            {
                return _ITSGuidField14;
            }
            set
            {
                SetPropertyValue("ITSGuidField14", ref _ITSGuidField14, value);
            }
        }

        public Guid? ITSGuidField15
        {
            get
            {
                return _ITSGuidField15;
            }
            set
            {
                SetPropertyValue("ITSGuidField15", ref _ITSGuidField15, value);
            }
        }

        public bool BooleanField1
        {
            get
            {
                return _BooleanField1;
            }
            set
            {
                SetPropertyValue("BooleanField1", ref _BooleanField1, value);
            }
        }

        public bool BooleanField2
        {
            get
            {
                return _BooleanField2;
            }
            set
            {
                SetPropertyValue("BooleanField2", ref _BooleanField2, value);
            }
        }
        public bool BooleanField3
        {
            get
            {
                return _BooleanField3;
            }
            set
            {
                SetPropertyValue("BooleanField3", ref _BooleanField3, value);
            }
        }

        public bool BooleanField4
        {
            get
            {
                return _BooleanField4;
            }
            set
            {
                SetPropertyValue("BooleanField4", ref _BooleanField4, value);
            }
        }

        public bool BooleanField5
        {
            get
            {
                return _BooleanField5;
            }
            set
            {
                SetPropertyValue("BooleanField5", ref _BooleanField5, value);
            }
        }

        public bool BooleanField6
        {
            get
            {
                return _BooleanField6;
            }
            set
            {
                SetPropertyValue("BooleanField6", ref _BooleanField6, value);
            }
        }

        public bool BooleanField7
        {
            get
            {
                return _BooleanField7;
            }
            set
            {
                SetPropertyValue("BooleanField7", ref _BooleanField7, value);
            }
        }

        public bool BooleanField8
        {
            get
            {
                return _BooleanField8;
            }
            set
            {
                SetPropertyValue("BooleanField1", ref _BooleanField8, value);
            }
        }

        public bool BooleanField9
        {
            get
            {
                return _BooleanField9;
            }
            set
            {
                SetPropertyValue("BooleanField1", ref _BooleanField9, value);
            }
        }

        public bool BooleanField10
        {
            get
            {
                return _BooleanField10;
            }
            set
            {
                SetPropertyValue("BooleanField10", ref _BooleanField10, value);
            }
        }

        public bool ITSBooleanField11
        {
            get
            {
                return _ITSBooleanField11;
            }
            set
            {
                SetPropertyValue("ITSBooleanField11", ref _ITSBooleanField11, value);
            }
        }

        public bool ITSBooleanField12
        {
            get
            {
                return _ITSBooleanField12;
            }
            set
            {
                SetPropertyValue("ITSBooleanField12", ref _ITSBooleanField12, value);
            }
        }

        public bool ITSBooleanField13
        {
            get
            {
                return _ITSBooleanField13;
            }
            set
            {
                SetPropertyValue("ITSBooleanField13", ref _ITSBooleanField13, value);
            }
        }

        public bool ITSBooleanField14
        {
            get
            {
                return _ITSBooleanField14;
            }
            set
            {
                SetPropertyValue("ITSBooleanField14", ref _ITSBooleanField14, value);
            }
        }

        public bool ITSBooleanField15
        {
            get
            {
                return _ITSBooleanField15;
            }
            set
            {
                SetPropertyValue("ITSBooleanField15", ref _ITSBooleanField15, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
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

        public eTotalizersUpdateMode UpdateMode
        {
            get
            {
                return _UpdateMode;
            }
            set
            {
                SetPropertyValue("UpdateMode", ref _UpdateMode, value);
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

        public void SetVariableValue(string propertyName, object propertyValue)
        {
            PropertyInfo variableValuesPropertyInfo = this.GetType().GetProperty(propertyName);
            variableValuesPropertyInfo.SetValue(this, propertyValue, null);
        }

        public object GetVariableValue(string propertyName)
        {
            PropertyInfo variableValuesPropertyInfo = this.GetType().GetProperty(propertyName);
            return variableValuesPropertyInfo.GetValue(this, null);
        }


        internal void ResetValues()
        {
            foreach (PropertyInfo property in typeof(VariableValues).GetProperties().Where(propertyInfo => propertyInfo.Name.Contains("Field")))
            {
                object currentPropertyValue = property.GetValue(this, null);
                if (currentPropertyValue == null || currentPropertyValue == DBNull.Value)
                {
                    continue;
                }


                if (property.PropertyType == typeof(Nullable<>))
                {
                    property.SetValue(this, null, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(this, Guid.Empty, null);
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    property.SetValue(this, 0m, null);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(this, string.Empty, null);
                }
                else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool))
                {
                    property.SetValue(this, false, null);
                }
            }
        }
    }
}
