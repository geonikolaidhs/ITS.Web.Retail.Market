using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.Model
{
    [Updater(Order = 1160, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class ActionTypeEntity : BaseObj
    {
        private Guid _EntityOid;
        private string _EntityType;
        private ActionType _ActionType;
        private List<string> _OrderedVariables;
        private string _EntityCode;
        private CompanyNew _Owner;
        private Store _Store;
        private eTotalizersUpdateMode _UpdateMode;
        public ActionTypeEntity()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ActionTypeEntity(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this._OrderedVariables = new List<string>();
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

        public string EntityCode
        {
            get
            {
                return _EntityCode;
            }
            set
            {
                SetPropertyValue("EntityCode", ref _EntityCode, value);
            }
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

        [Indexed("EntityType;EntityOid;GCRecord", Unique = true),
         Association("ActionType-ActionTypeEntities")]
        public ActionType ActionType
        {
            get
            {
                return _ActionType;
            }
            set
            {
                SetPropertyValue("ActionType", ref _ActionType, value);
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

        public IEnumerable<VariableActionType> ValidVariableActionTypes
        {
            get
            {
                return this.ActionType.VariableActionTypes.Where(variableATE => variableATE.Variable.IsActive);
            }
        }

        /// <summary>
        /// Calculates relative Variables
        /// </summary>
        /// <param name="basicObject">The object to get value from</param>
        /// <param name="uow">The UnitOfWork in wich the calculations take place</param>
        /// <returns>A VariableValues in the provided UnitOfWork uow containing the calculated values</returns>
        public VariableValues Execute(BasicObj basicObject, UnitOfWork uow)
        {
            VariableValues variableValues;
            variableValues = uow.FindObject<VariableValues>(CriteriaOperator.And(new BinaryOperator("ActionTypeOid", this.ActionType.Oid),
                                                                                 new BinaryOperator("EntityOid", basicObject.Oid)));
            if (variableValues == null)
            {
                variableValues = new VariableValues(uow);
                variableValues.ActionTypeOid = this.ActionType.Oid;
                variableValues.EntityOid = basicObject.Oid;
                variableValues.Owner = this.ActionType.Owner;
                variableValues.UpdateMode = this.ActionType.UpdateMode;
                if(variableValues.UpdateMode == eTotalizersUpdateMode.STORE)
                {
                    PropertyInfo storeProperty = basicObject.GetType().GetProperty("Store");
                    variableValues.Store = storeProperty != null ? storeProperty.GetValue(basicObject, null) as Store : null;
                }
            }
            else
            {
                variableValues.ResetValues();
            }

            foreach (VariableActionType variableActionType in this.OrderedVariableActionTypes)
            {
                object newValue = variableActionType.Variable.ExecuteExpression(basicObject, variableValues);
                Type[] numericTypes = { typeof(double), typeof(decimal), typeof(int), typeof(long) };

                if (numericTypes.Contains(newValue.GetType()))
                {
                    switch (variableActionType.VariableAction)
                    {
                        case VariableMethods.INCREASE:
                            variableValues.SetVariableValue(variableActionType.Variable.FieldName, Convert.ToDecimal(newValue));
                            break;
                        case VariableMethods.DECREASE:
                            variableValues.SetVariableValue(variableActionType.Variable.FieldName, (-1) * Convert.ToDecimal(newValue));
                            break;
                        case VariableMethods.NONE:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    variableValues.SetVariableValue(variableActionType.Variable.FieldName, newValue);
                }
            }

            return variableValues;
        }


        public List<string> OrderedVariables
        {
            get
            {
                if (_OrderedVariables == null)
                {
                    _OrderedVariables = new List<string>();
                }
                if (_OrderedVariables.Count == 0 && this.ValidVariableActionTypes.Count() > 0)
                {
                    OrderVariables();
                }
                return _OrderedVariables;
            }
        }

        private void OrderVariables()
        {
            this._OrderedVariables = this.ValidVariableActionTypes.Where(variableActionType => variableActionType.Variable.Source == VariableSource.FIELD).Select(varActType => varActType.Variable.FieldName).ToList();
                        
            IEnumerable<Variable> formulaVariables = this.ValidVariableActionTypes.Where(variableActionType => variableActionType.Variable.Source == VariableSource.FORMULA).Select(varActionType => varActionType.Variable);
            List<string> formulaVariablesIDs = formulaVariables.Select(var => var.FieldName).ToList();
            List<VariableDependency> formulaVariablesDependencies = formulaVariables.Select(variable => variable.GetVariableDependency(null)).ToList();

            List<string> independentVariables = new List<string>();
            IEnumerable<VariableDependency> independentVariablesDependencies = new List<VariableDependency>();

            while(formulaVariablesDependencies.Count > 0)
            {
                independentVariablesDependencies = formulaVariablesDependencies.Where(dep => dep.DependsOnVariables.Where(varID => formulaVariablesIDs.Contains(varID)).Count() == 0);
                independentVariables = independentVariablesDependencies.Select(dep => dep.VariableID).ToList();
                this._OrderedVariables.AddRange(independentVariables);
                formulaVariablesDependencies.RemoveAll(x => independentVariablesDependencies.Contains(x));
                formulaVariablesIDs.RemoveAll(x => independentVariables.Contains(x));
                foreach(VariableDependency currentDependency in formulaVariablesDependencies)
                {
                     currentDependency.RemoveDependencies(independentVariables);
                }
            }
        }

        public List<VariableActionType> OrderedVariableActionTypes
        {
            get
            {
                List<VariableActionType> orderedVariableActionTypes = new List<VariableActionType>();

                foreach(string variableID in this.OrderedVariables)
                {
                    VariableActionType variableActionType = this.ValidVariableActionTypes.Where(varActionType => varActionType.Variable.FieldName == variableID).FirstOrDefault();
                    orderedVariableActionTypes.Add(variableActionType);
                }

                return orderedVariableActionTypes;
            }
        }

        /// <summary>
        /// A list of DocumentStatus objects which can trigger the Action
        /// </summary>
        [Aggregated,Association("ActionTypeEntity-ActionTypeDocStatuses")]
        public XPCollection<ActionTypeDocStatus> ActionTypeDocStatuses
        {
            get
            {
                return GetCollection<ActionTypeDocStatus>("ActionTypeDocStatuses");
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            Type thisType = typeof(ActionTypeEntity);            
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

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("ActionTypeDocStatuses", ActionTypeDocStatuses.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }
    }
}
