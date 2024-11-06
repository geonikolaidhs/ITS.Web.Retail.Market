using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{
    [Updater(Order = 1155, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class ActionType : BaseObj, IRequiredOwner
    {
        private string _Description;
        private ActionEntityCategory _Category;
        private CompanyNew _Owner;
        private eTotalizersUpdateMode _UpdateMode;
        private Store _Store;

        public ActionType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ActionType(Session session)
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
            Type thisType = typeof(ActionType);            
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
                    crop = CriteriaOperator.And(crop, CriteriaOperator.And(new BinaryOperator("UpdateMode", eTotalizersUpdateMode.STORE),
                                                                           new BinaryOperator("Store.Oid", store.Oid)));
                    break;
            }
            return crop;
        }

        [Aggregated, Association("ActionType-VariableActionTypes")]
        public XPCollection<VariableActionType> VariableActionTypes
        {
            get
            {
                return GetCollection<VariableActionType>("VariableActionTypes");
            }
        }

        [Aggregated, Association("ActionType-ActionTypeEntities")]
        public XPCollection<ActionTypeEntity> ActionTypeEntities
        {
            get
            {
                return GetCollection<ActionTypeEntity>("ActionTypeEntities");
            }
        }

        [DescriptionField]
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

        public ActionEntityCategory Category
        {
            get
            {
                return _Category;
            }
            set
            {
                SetPropertyValue("Category", ref _Category, value);
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

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("ActionTypeEntities", ActionTypeEntities.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("VariableActionTypes", VariableActionTypes.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }
    }
}