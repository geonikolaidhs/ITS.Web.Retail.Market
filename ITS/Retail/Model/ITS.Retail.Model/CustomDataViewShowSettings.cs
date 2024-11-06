using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    //[Updater(Order = 1190, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class CustomDataViewShowSettings : BaseObj, IRequiredOwner
    {
        private string _EntityType;
        private VariableDisplayValuesMode _DisplayValuesMode;
        private CustomDataView _CustomDataView;
        private CompanyNew _Owner;
        private string _Description;
        private bool _IsDefault;

        public CustomDataViewShowSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomDataViewShowSettings(Session session)
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

        /// <summary>
        /// The Type (FullName) of the entity the View refers to
        /// </summary>
        [DisplayOrder(Order = 1)]
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

        /// <summary>
        /// Defines where the View can be shown
        /// </summary>
        [DisplayOrder(Order = 2)]
        public VariableDisplayValuesMode DisplayValuesMode
        {
            get
            {
                return _DisplayValuesMode;
            }
            set
            {
                SetPropertyValue("DisplayValuesMode", ref _DisplayValuesMode, value);
            }
        }

        /// <summary>
        /// The Master View
        /// </summary>
        [Association("CustomDataView-ShowSettings")]
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

        /// <summary>
        /// The Owner
        /// </summary>
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

        /// <summary>
        /// The Description
        /// </summary>
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

        /// <summary>
        /// Defines if current CustomDataView is default choice for current controller
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("Description", ref _IsDefault, value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:

                    Type thisType = typeof(CustomDataViewShowSettings);
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
