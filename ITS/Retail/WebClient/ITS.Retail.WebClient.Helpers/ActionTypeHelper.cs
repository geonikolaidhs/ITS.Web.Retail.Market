using DevExpress.Data.Linq;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.Text.RegularExpressions;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ActionTypeHelper
    {
        private static List<BasicObj> GetDetailObjects(BasicObj masterObject, ActionTypeInfoAttribute actionTypeInfoAttribute)
        {
            List<object> currentLevelObjects = new List<object>() { masterObject };
            CriteriaToExpressionConverter criteriaConverter = new CriteriaToExpressionConverter();
            foreach (string entityName in actionTypeInfoAttribute.Path)
            {
                List<object> propertyValueList = new List<object>();
                foreach (object currentPropertyObject in currentLevelObjects)
                {
                    PropertyInfo property = currentPropertyObject.GetType().GetProperty(entityName);
                    if (property == null)
                    {
                        throw new Exception("Property " + entityName + " not found for Object of Type " + currentPropertyObject.GetType().FullName);
                    }
                    object propertyValue = property.GetValue(currentPropertyObject, null);
                    if (propertyValue == null)
                    {
                        throw new Exception("Property " + entityName + " has null value for Object of Type " + currentPropertyObject.GetType().FullName);
                    }
                    else if (propertyValue is IEnumerable)
                    {
                        IQueryable queryable = ((IEnumerable)propertyValue).AsQueryable();
                        if(entityName == actionTypeInfoAttribute.Path.Last())
                        {
                            queryable = queryable.AppendWhere(criteriaConverter, actionTypeInfoAttribute.Criteria);
                            if(!String.IsNullOrEmpty(actionTypeInfoAttribute.BasicObjCriteria))
                            {
                                queryable = queryable.AppendWhere(criteriaConverter, new BinaryOperator(actionTypeInfoAttribute.BasicObjCriteria,
                                    masterObject.GetType().GetProperty(actionTypeInfoAttribute.BasicObjCriteria).GetValue(masterObject,null)));
                            }
                        }
                        propertyValueList.AddRange(queryable.Cast<object>());
                    }
                    else
                    {
                        propertyValueList.Add(propertyValue);
                    }
                }
                currentLevelObjects = propertyValueList;
            }
            return currentLevelObjects.ConvertAll(val => (BasicObj)val);
        }

        public static List<BasicObj> GetActivityObjects(BasicObj masterObject, ActionEntityCategory? actionCategory)
        {
            ActionTypeInfoAttribute actionTypeInfoAttribute = EnumerationHelper.GetAttribute<ActionTypeInfoAttribute>(actionCategory);
            if (actionTypeInfoAttribute == null || string.IsNullOrEmpty(actionTypeInfoAttribute.DetailEntity))
            {
                return new List<BasicObj>(){ masterObject };
            }
            else
            {
                return GetDetailObjects(masterObject, actionTypeInfoAttribute);
            }
        }

        public static void RecalculateActionType(ActionType actionType, CriteriaOperator criteria, CompanyNew owner)
        {
            ActionTypeInfoAttribute actionTypeInfoAttribute = EnumerationHelper.GetAttribute<ActionTypeInfoAttribute>(actionType.Category);
            Type basicObjectType = typeof(BasicObj);
            Type masterObjectType = basicObjectType.Assembly.GetType(basicObjectType.FullName.Replace(basicObjectType.Name,actionTypeInfoAttribute.MasterEntity));
            CriteriaOperator appliedCriteria = RetailHelper.ApplyOwnerCriteria(criteria,masterObjectType, owner);
            using( UnitOfWork uow = XpoHelper.GetNewUnitOfWork() )
            {
                XPCollection masterObjects = new XPCollection(uow, masterObjectType, appliedCriteria, null);
                foreach(BasicObj masterObj in masterObjects)
                {
                    PropertyInfo actionTypeEntitiesProperty = masterObjectType.GetProperty("ActionTypeEntities");
                    XPCollection<ActionTypeEntity> actionTypeEntitiesValue = (XPCollection<ActionTypeEntity>)(actionTypeEntitiesProperty.GetValue(masterObj, null));

                    IEnumerable<ActionTypeEntity> actionTypeEntities = actionTypeEntitiesValue.Where(ate => ate.ActionType.Oid == actionType.Oid);
                    foreach(ActionTypeEntity actionTypeEntity in actionTypeEntities)
                    {
                        List<BasicObj> detailObjects = GetActivityObjects(masterObj, actionType.Category);
                        foreach (BasicObj detailObject in detailObjects)
                        {
                            VariableValues variableValues = actionTypeEntity.Execute(detailObject, uow);
                            variableValues.Save();
                        }   
                    }
                }
                XpoHelper.CommitTransaction(uow);
            }
        }

        /// <summary>
        /// Returns the result set of an executed query as a dynamic sized 2-dimension string array
        /// </summary>
        /// <param name="selectedData">The result set as a SelectedStatementResult array</param>
        /// <returns>The string array</returns>
        public static List<Dictionary<string, object>> ShowDataViewDataWeb(SelectStatementResult[] selectedData)
        {
            if (selectedData.Length > 0)
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                int rowsLength = selectedData[1].Rows.Length + 1;
                int columnsLength = selectedData[0].Rows.Length;
                string[,] dataView = new string[rowsLength, columnsLength];


                for (int column = 0; column < columnsLength; column++)
                {
                    object val = selectedData[0].Rows[column].Values[0];
                    string stringValue = val == null ? string.Empty : val.ToString();
                    dataView[0, column] = stringValue;
                }


                for (int row = 1; row < rowsLength; row++)
                {
                    for (int column = 0; column < columnsLength; column++)
                    {
                        object val = selectedData[1].Rows[row - 1].Values[column];
                        string stringValue = val == null ? string.Empty : val.ToString();
                        dataView[row, column] = stringValue;
                    }
                }

                for (int rowIndex = 1; rowIndex < dataView.GetLength(0); rowIndex++)
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int columnIndex = 0; columnIndex < dataView.GetLength(1); columnIndex++)
                    {
                        row.Add(dataView[0, columnIndex].ToString(), dataView[rowIndex, columnIndex]);
                    }
                    result.Add(row);
                }
                return result;
            }
            return null;
        }

        public static bool AllQueryParametersAreDefined(CustomDataView view)
        {
            if (view.Query == null) { return true; }
            Regex parameterExpression = new Regex("\\{(\\w+)\\}", RegexOptions.Multiline);
            MatchCollection paramMatchCollection = parameterExpression.Matches(view.Query);
            foreach(Match match in paramMatchCollection)
            {
                if(match.Value != "{OIDS}" && !view.Parameters.Select(param => param.Name).Contains(match.Value))
                {
                    return false;
                }
            }
            return true;
        }

        public static void VariableUpdateDataField(BasicObj basicObject, VariableActionType variableActType, VariableValues variableValues)
        {
            try
            {
                List<BasicObj> updatingEntities = GetActivityObjects(basicObject, variableActType.Variable.TargetCategory);
                foreach (BasicObj updEntity in updatingEntities)
                {
                    object newValue = variableValues.GetType().GetProperty(variableActType.Variable.FieldName).GetValue(variableValues, null);
                    object oldValue = updEntity.GetType().GetProperty(variableActType.Variable.TargetFieldName).GetValue(updEntity, null);
                    Type[] numericTypes = { typeof(double), typeof(decimal), typeof(int), typeof(long) };

                    if(numericTypes.Contains(newValue.GetType()) && numericTypes.Contains(oldValue.GetType()))
                    {
                        switch (variableActType.VariableAction)
                        {
                            case VariableMethods.INCREASE:
                                updEntity.GetType().GetProperty(variableActType.Variable.TargetFieldName).SetValue(updEntity, Convert.ChangeType(Convert.ToDecimal(newValue) + Convert.ToDecimal(oldValue), oldValue.GetType()), null);
                                break;
                            case VariableMethods.DECREASE:
                                updEntity.GetType().GetProperty(variableActType.Variable.TargetFieldName).SetValue(updEntity, Convert.ChangeType((-1) * Convert.ToDecimal(newValue) + Convert.ToDecimal(oldValue), oldValue.GetType()), null);
                                break;
                            case VariableMethods.NONE:
                                break;
                            default:
                                break;
                        }
                    }
                    else if(newValue.GetType() == oldValue.GetType())
                    {
                        updEntity.GetType().GetProperty(variableActType.Variable.TargetFieldName).SetValue(updEntity, newValue, null);
                    }
                }
            }
            catch(Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }
        }

        /// <summary>
        /// Returns the update mode corresponding to current MvcApplication
        /// </summary>
        /// <returns>Enumeration</returns>
        public static eTotalizersUpdateMode GetUpdaterMode(eApplicationInstance applicationInstance)
        {
            eTotalizersUpdateMode mode = eTotalizersUpdateMode.GLOBAL;
            switch (applicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                case eApplicationInstance.RETAIL:
                    mode = eTotalizersUpdateMode.GLOBAL;
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    mode = eTotalizersUpdateMode.STORE;
                    break;
                default:
                    break;
            }
            return mode;
        }
    }
}
