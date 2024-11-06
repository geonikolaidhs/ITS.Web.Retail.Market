using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Common.ViewModel
{
    public class BaseSearchFilter: BasicViewModel
    {
        public Guid? LoggedInUser { get; set; }

        protected virtual List<CriteriaOperator> BuildExtraCriteria()
        {
            return new List<CriteriaOperator>();
        }

        public CriteriaOperator BuildCriteria(string criteriaPrefix ="")
        {

            List<CriteriaOperator> list = new List<CriteriaOperator>(), extra = BuildExtraCriteria();
            if (extra != null)
            {
                list.AddRange(extra);
            }

            foreach (PropertyInfo propInfo in GetType().GetProperties().Where(prop => prop.GetCustomAttributes(typeof(CriteriaFieldAttribute), true).Count() > 0))
            {
                CriteriaFieldAttribute attr = propInfo.GetCustomAttributes(typeof(CriteriaFieldAttribute), true).Cast<CriteriaFieldAttribute>().First();
                object value = propInfo.GetValue(this, null);
                if (value != null && value.Equals(attr.NullValue) == false)
                {
                    if (attr.PreProcessorType != null)
                    {
                        value = PreprocessValue(attr.PreProcessorType, value, attr);
                    }

                    CriteriaOperator criteriaOperator;
                    if (value.GetType().IsArray == false || (value as object[]).Length == 1)
                    {
                        value = value.GetType().IsArray ? (value as object[]).First() : value;
                        if(attr.OperatorType == CustomBinaryOperatorType.Like && value is string)
                        {
                            criteriaOperator = BuildInternalCriteria(attr, value as string, criteriaPrefix);
                        }
                        else
                        {
                            criteriaOperator = new BinaryOperator(criteriaPrefix+attr.FieldName, value, attr.BinaryOperatorType);
                        }
                        //criteriaOperator = new BinaryOperator(attr.FieldName, value, attr.OperatorType);
                        //if (String.IsNullOrWhiteSpace(attr.ValuePrefix) == false || String.IsNullOrWhiteSpace(attr.ValueSufix) == false)
                        //{
                        //    string modifiedRightOperand = ((BinaryOperator)criteriaOperator).RightOperand.ToString().Replace("'", string.Empty);
                        //    modifiedRightOperand = attr.ValuePrefix + modifiedRightOperand + attr.ValueSufix;

                        //    string criteriaString = criteriaOperator.ToString().Replace(((BinaryOperator)criteriaOperator).RightOperand.ToString().Replace("'", string.Empty), modifiedRightOperand);

                        //    criteriaOperator = CriteriaOperator.Parse(criteriaString);
                        //}
                        //else if (attr.OperatorType == BinaryOperatorType.Like && attr.SearchExactPhrase == false && value is string && ((string)value).Contains('%') == false)
                        //{
                        //    criteriaOperator = new BinaryOperator(attr.FieldName, string.Format("%{0}%",value), attr.OperatorType);
                        //}
                    }
                    else
                    {
                        object[] arrValue = value as object[];
                        List<BinaryOperator> operators = new List<BinaryOperator>();
                        foreach( var svalue in arrValue)
                        {
                            operators.Add(new BinaryOperator(attr.FieldName, svalue, attr.BinaryOperatorType));
                        }
                        criteriaOperator = (attr.GroupOperator == GroupOperatorType.Or) ? CriteriaOperator.Or(operators) : CriteriaOperator.And(operators);
                    }
                    list.Add(criteriaOperator);
                }
            }
            return CriteriaOperator.And(list);
        }

        private static CriteriaOperator BuildInternalCriteria(CriteriaFieldAttribute criteriaFieldAttribute, string value, string criteriaPrefix="")
        {
            CriteriaOperator criteriaOperator;
            value = value.Replace('*', '%');
            if (value.Contains('%'))
            {
                List<string> values = value.Split('%').ToList();
                List<CriteriaOperator> valueCriteria = new List<CriteriaOperator>();
                values.Where(val => String.IsNullOrEmpty(val) == false
                                 && String.IsNullOrWhiteSpace(val) == false
                             )
                       .ToList()
                       .ForEach(val =>
                       {
                           valueCriteria.Add(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(criteriaPrefix+criteriaFieldAttribute.FieldName), val));
                       });
                criteriaOperator = CriteriaOperator.And(valueCriteria);
            }
            else
            {
                criteriaOperator = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(criteriaPrefix+criteriaFieldAttribute.FieldName), value);
            }

            return criteriaOperator;
        }

        private object PreprocessValue(Type type, object value, CriteriaFieldAttribute attribute)
        {

            if (typeof(ICriteriaPreprocessor).IsAssignableFrom(type) && type.IsInterface == false && type.IsAbstract == false)
            {
                try
                {
                    MethodInfo mInfo = GetType().GetMethod("PreprocessValueGeneric",  BindingFlags.Public);
                    if (mInfo != null)
                    {
                        MethodInfo genericMethodInfo = mInfo.MakeGenericMethod(value.GetType());
                        value = genericMethodInfo.Invoke(null, new object[] { Activator.CreateInstance(type), value, attribute });
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetFullMessage();
                }
            }
            return value;

        }

        public object PreprocessValueGeneric<T>(ICriteriaPreprocessor<T> obj, T value, CriteriaFieldAttribute attribute)
        {
            return obj.Preprocess(value, attribute);
        }

        public void Reset()
        {
            BaseSearchFilter filter =  Newtonsoft.Json.JsonConvert.DeserializeObject(this.ResetModel, this.GetType()) as BaseSearchFilter;
            Copy(filter);
        }   
        
        private void Copy(BaseSearchFilter other)
        {
            foreach(PropertyInfo pInfo in this.GetType().GetProperties().Where(x=>x.CanWrite))
            {
                pInfo.SetValue(this, pInfo.GetValue(other, null), null);
            }
        }

        public void Set()
        {
            ResetModel = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        private string ResetModel;
    }
    
}
