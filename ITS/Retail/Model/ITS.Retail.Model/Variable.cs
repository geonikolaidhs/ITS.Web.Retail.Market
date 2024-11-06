using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using Jace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ITS.Retail.Model
{
    [Updater(Order = 1160, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class Variable : BaseObj, IRequiredOwner
    {
        private string _Description;
        private string _Expression;
        private ActionEntityCategory _Category;
        private string _FieldName;
        private CompanyNew _Owner;
        private VariableSource _Source;
        private VariableDependency _VariableDependency;
        private ActionEntityCategory? _TargetCategory;
        private string _TargetFieldName;

        public static string FIELD_VARIABLE_START_CHARACTER = "$";
        public static string FIELD_VARIABLE_END_CHARACTER = "$";
        public static string FIELD_VARIABLE_REGULAR_EXPRESSION = "\\$((\\w|\\.)+)\\$";
        public static string VALUE_VARIABLE_REGULAR_EXPRESSION = "\\[([^(\\[\\])]+)\\]";
        private static string VALUE_VARIABLE_START_CHARACTER = "[";
        private static string VALUE_VARIABLE_END_CHARACTER = "]";
        private static string INNER_DEPENENDENCY_SEPERATOR = "->";
        private static string CURRENT_LEVEL_DEPENENDENCY_SEPERATOR = ",";
        private static string LEVEL_SEPERATOR = "|";
        public static string PROPERTY_SEPERATOR = ".";

        public Variable()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Variable(Session session)
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
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:

                    Type thisType = typeof(Variable);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }


        [DisplayOrder(Order = 1)]
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

        [DisplayOrder(Order = 2)]
        [Indexed("GCRecord", Unique = true)]
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                SetPropertyValue("FieldName", ref _FieldName, value);
            }
        }

        [DisplayOrder(Order = 5)]
        [Size(SizeAttribute.Unlimited)]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                SetPropertyValue("Expression", ref _Expression, value);
            }
        }

        [DisplayOrder(Order = 3)]
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

        [DisplayOrder(Order = 4)]
        public VariableSource Source
        {
            get
            {
                return _Source;
            }
            set
            {
                SetPropertyValue("Source", ref _Source, value);
            }
        }

        public ActionEntityCategory? TargetCategory
        {
            get
            {
                return _TargetCategory;
            }
            set
            {
                SetPropertyValue("TargetCategory", ref _TargetCategory, value);
            }
        }

        public string TargetFieldName
        {
            get
            {
                return _TargetFieldName;
            }
            set
            {
                SetPropertyValue("TargetFieldName", ref _TargetFieldName, value);
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

        [Aggregated, Association("Variable-VariableActionTypes")]
        public XPCollection<VariableActionType> VariableActionTypes
        {
            get
            {
                return GetCollection<VariableActionType>("VariableActionTypes");
            }
        }

        /// <summary>
        /// Calculates and sets the value computed by according to the this.Expression
        /// </summary>
        /// <param name="basicObject">The object that triggered the ActionType which also contains the values</param>
        /// <param name="variableValues">The VariableValues that contains all the computed values, and where the result will be saved</param>
        public object ExecuteExpression(BasicObj basicObject, VariableValues variableValues)
        {
            if (this.HasCircularReference)
            {
                throw new Exception(string.Format(Resources.VariableHasCircularReference, this.Description, this.GetVariableDependency(null).Info));
            }

            Regex fieldRegularExpression = new Regex(FIELD_VARIABLE_REGULAR_EXPRESSION, RegexOptions.Multiline);
            Regex valueRegularExpression = new Regex(VALUE_VARIABLE_REGULAR_EXPRESSION, RegexOptions.Multiline);

            switch (this.Source)
            {
                case VariableSource.FIELD:
                    Match match = fieldRegularExpression.Match(this.Expression);
                    string propertyName = match.Groups[1].Value;
                    object propertyValue = GetBasicObjectPropertyValue(basicObject, propertyName);
                    return propertyValue;
                case VariableSource.FORMULA:
                    MatchCollection fieldMatchCollection = fieldRegularExpression.Matches(this.Expression);
                    MatchCollection valueMatchCollection = valueRegularExpression.Matches(this.Expression);

                    Dictionary<string, double> expressionVariables = new Dictionary<string, double>();

                    foreach (Match fieldMatch in fieldMatchCollection)
                    {
                        string fieldPropertyName = fieldMatch.Groups[1].Value;
                        object fieldPropertyValue = GetBasicObjectPropertyValue(basicObject, fieldPropertyName);
                        string fieldPropertyValueString = fieldPropertyValue == null ? string.Empty : fieldPropertyValue.ToString();

                        expressionVariables.Add(fieldPropertyName.Replace('.', '_'), Convert.ToDouble(fieldPropertyValue));

                    }

                    Variable innerVariable;
                    foreach (Match valueMatch in valueMatchCollection)
                    {
                        string valuePropertyName = valueMatch.Groups[1].Value;
                        innerVariable = this.Session.FindObject<Variable>(new BinaryOperator("FieldName", valuePropertyName));
                        variableValues.GetData(variableValues, new List<string>() { "Oid" });
                        object valuePropertyValue = innerVariable.ExecuteExpression(basicObject, variableValues);
                        string valuePropertyValueString = valuePropertyValue == null ? string.Empty : valuePropertyValue.ToString();
                        if (expressionVariables.ContainsKey(valuePropertyName.Replace('.', '_')) == false)
                        {
                            expressionVariables.Add(valuePropertyName.Replace('.', '_'), Convert.ToDouble(valuePropertyValue));
                        }
                    }
                    #region Execute expression
                    CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);
                    engine.AddFunction("Month", new Func<double, double>((inp) => new DateTime((long)inp).Month));
                    engine.AddFunction("Year", new Func<double, double>((inp) => new DateTime((long)inp).Year));
                    engine.AddFunction("Day", new Func<double, double>((inp) => new DateTime((long)inp).Day));
                    engine.AddFunction("Hour", new Func<double, double>((inp) => new DateTime((long)inp).Hour));
                    engine.AddFunction("Minute", new Func<double, double>((inp) => new DateTime((long)inp).Minute));
                    engine.AddFunction("Second", new Func<double, double>((inp) => new DateTime((long)inp).Second));
                    List<string> specialCharacters = new List<string>()
                    {
                        FIELD_VARIABLE_START_CHARACTER,
                        FIELD_VARIABLE_END_CHARACTER,
                        VALUE_VARIABLE_START_CHARACTER,
                        VALUE_VARIABLE_END_CHARACTER
                    };
                    string finalExpression = this.Expression.Replace('.', '_');

                    foreach (string specialCharacter in specialCharacters)
                    {
                        finalExpression = finalExpression.Replace(specialCharacter, "");
                    }

                    object engineResult = engine.Calculate(finalExpression, expressionVariables);

                    return engineResult;
                #endregion
                default:
                    throw new NotSupportedException();
            }
        }

        private object GetInnerValue(BasicObj basicObject, string propertyName)
        {
            if (propertyName.Contains(PROPERTY_SEPERATOR))
            {
                string innerBasicObjectPropertyName = propertyName.Split(PROPERTY_SEPERATOR.ToCharArray()).FirstOrDefault();
                string innerPropertyName = propertyName.Substring(innerBasicObjectPropertyName.Length + PROPERTY_SEPERATOR.Length);
                BasicObj innerBasicObject = basicObject.GetType().GetProperty(innerBasicObjectPropertyName).GetValue(basicObject, null) as BasicObj;
                if (innerBasicObject == null)
                {
                    return null;
                }
                return GetInnerValue(innerBasicObject, innerPropertyName);
            }
            return basicObject.GetType().GetProperty(propertyName).GetValue(basicObject, null);
        }

        private object GetBasicObjectPropertyValue(BasicObj basicObject, string propertyName)
        {
            if (propertyName.Contains(PROPERTY_SEPERATOR))
            {
                return GetInnerValue(basicObject, propertyName);
            }            
            object value = basicObject.GetType().GetProperty(propertyName).GetValue(basicObject, null);
            if(value is DateTime)
            {
                return ((DateTime)value).Ticks;
            }
            return value;
        }

        public List<string> GetInnerVariables
        {
            get
            {
                List<string> innerVariableIDs = new List<string>();
                if (_VariableDependency == null && string.IsNullOrEmpty(this.Expression) == false)
                {
                    Regex valueRegularExpression = new Regex(VALUE_VARIABLE_REGULAR_EXPRESSION, RegexOptions.Multiline);
                    MatchCollection valueMatchCollection = valueRegularExpression.Matches(this.Expression);

                    foreach (Match valueMatch in valueMatchCollection)
                    {
                        IEnumerator groupEnumerator = valueMatch.Groups.GetEnumerator();
                        groupEnumerator.MoveNext();
                        while (groupEnumerator.MoveNext())
                        {
                            Group currentGroup = (Group)groupEnumerator.Current;
                            innerVariableIDs.Add(currentGroup.Value);
                        }
                    }
                }
                return innerVariableIDs;
            }
        }

        /// <summary>
        /// Gets the dependencies of this Variable
        /// </summary>
        /// <param name="ancestors">A list of all the ancestors of Variables dependecies already found.
        /// Default value is null thus signaling the dependecy search from this variable and onward</param>
        /// <returns>Returns a VariableDependency object containing either all the variable dependencies
        /// either all till the point where a circular reference has been found</returns>
        public VariableDependency GetVariableDependency(Dictionary<string, string> ancestors)
        {
            if (ancestors == null)
            {
                ancestors = new Dictionary<string, string>();
            }

            if (ancestors.Keys.Contains(this.FieldName) == false)
            {
                ancestors.Add(this.FieldName, "");
            }

            if (this._VariableDependency == null && string.IsNullOrEmpty(this.Expression) == false)
            {
                List<string> innerVariableIDs = this.GetInnerVariables;
                ancestors[this.FieldName] += string.Join(CURRENT_LEVEL_DEPENENDENCY_SEPERATOR, innerVariableIDs);
                bool circularReferenceFound = false;
                string circularReference = this.FieldName;
                foreach (string currentLevelVariables in innerVariableIDs)
                {
                    if (ancestors.Keys.Contains(currentLevelVariables))
                    {
                        circularReferenceFound = true;
                        circularReference = currentLevelVariables;
                    }
                }

                if (innerVariableIDs.Contains(this.FieldName) || circularReferenceFound)
                {
                    VariableDependency circularDependency = new VariableDependency(circularReference, new List<VariableDependency>());
                    this._VariableDependency = new VariableDependency(this.FieldName, new List<VariableDependency>() { circularDependency });
                }

                List<VariableDependency> innerVariableDependencies = new List<VariableDependency>();
                if (this._VariableDependency == null)
                {
                    XPCollection<Variable> innerVariables = new XPCollection<Variable>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, new InOperator("FieldName", innerVariableIDs));

                    if (innerVariables.Count > 0)
                    {
                        ancestors[this.FieldName] += LEVEL_SEPERATOR;
                    }

                    foreach (Variable innerVariable in innerVariables)
                    {
                        VariableDependency innerVariableDependency = innerVariable.GetVariableDependency(ancestors);
                        innerVariableDependencies.Add(innerVariableDependency);
                        ancestors[this.FieldName] += innerVariable.FieldName + INNER_DEPENENDENCY_SEPERATOR + ancestors[innerVariable.FieldName];
                        if (ancestors[this.FieldName].Contains(this.FieldName))
                        {
                            break;
                        }
                    }
                    this._VariableDependency = new VariableDependency(this.FieldName, innerVariableDependencies);
                }
            }
            return this._VariableDependency;
        }

        public bool HasCircularReference
        {
            get
            {
                return this._VariableDependency == null ?
                       (this.GetVariableDependency(null) == null ?
                            false : this._VariableDependency.HasCircularReference) :
                       this._VariableDependency.HasCircularReference;
            }
        }

        public bool IsUsedByOtherVariables
        {
            get
            {
                //CriteriaOperator criteria = new BinaryOperator("Expression", VALUE_VARIABLE_START_CHARACTER + this.FieldName + VALUE_VARIABLE_END_CHARACTER,
                //                                                BinaryOperatorType.Like);
                CriteriaOperator criteria = new FunctionOperator(FunctionOperatorType.Contains,
                                                                 new OperandProperty("Expression"),
                                                                 VALUE_VARIABLE_START_CHARACTER + this.FieldName + VALUE_VARIABLE_END_CHARACTER);

                int usedByOtherVariables = (int)this.Session.Evaluate(typeof(Variable), CriteriaOperator.Parse("Count"), criteria);
                return usedByOtherVariables != 0;
            }
        }
    }
}
