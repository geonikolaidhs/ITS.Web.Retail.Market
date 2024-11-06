using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
   // [Updater(Order = 1170, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class CustomDataViewParameter : BaseObj, IRequiredOwner
    {
        private CustomDataView _CustomDataView;
        private string _Description;
        private string _Value;
        private string _Name;
        private string _ParameterType;
        private CompanyNew _Owner;

        public CustomDataViewParameter()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomDataViewParameter(Session session)
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
         
        [Association("CustomDataView-Parameters")]
        public CustomDataView CustomDataView
        {
            get
            {
                return _CustomDataView;
            }
            set
            {
                SetPropertyValue("CustomDataView", ref _CustomDataView, value);
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

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        public string ParameterType
        {
            get
            {
                return _ParameterType;
            }
            set
            {
                SetPropertyValue("ParameterType", ref _ParameterType, value);
            }
        }

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:

                    Type thisType = typeof(CustomDataViewParameter);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }
    }
}
