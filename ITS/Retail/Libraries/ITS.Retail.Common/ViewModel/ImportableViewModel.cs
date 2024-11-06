using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel.Importable;
using ITS.Retail.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public interface IImportableViewModel
    {
        string EntityName { get; }

        bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store);

        ImportResult Import(string textToParse, SupplierImportFileRecordHeader header);

        bool UpdateChildren(List<IImportableViewModel> updateList, Type importableType, string property = null);

        void CheckWithDatabase(UnitOfWork uow, Guid owner);

        BaseObj GetPersistangObject(UnitOfWork uow, Guid owner);

        Guid? PersistantObjectOid { get; }

        bool IsNew { get; }

        bool HasNewChildren { get; }

        Type PeristentType { get; }

        string ImportObjectUniqueKey { get; }
    }

    public abstract class ImportableViewModel<Q> : BasicViewModel, IImportableViewModel
        where Q : BaseObj
    {
        [Display(AutoGenerateField = false)]
        public abstract string ImportObjectUniqueKey { get; }
        [Display(AutoGenerateField = false)]
        public abstract string EntityName { get; }

        [Display(AutoGenerateField = false)]
        public Guid? PersistantObjectOid { get; set; }

        [Display(Order = 99)]
        public bool IsNew { get; set; }

        [Display(Order = 100)]
        public abstract bool HasNewChildren { get; }

        [Display(AutoGenerateField = false)]
        public Type PeristentType
        {
            get
            {
                return typeof(Q);
            }
        }

        public BaseObj GetPersistangObject(UnitOfWork uow, Guid owner)
        {
            return GetPersistentObjectGeneric(uow, owner);
        }

        public virtual Q GetPersistentObjectGeneric(UnitOfWork uow, Guid owner)
        {
            PropertyInfo keyInfo = this.GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(KeyImportableAttribute), true).Length > 0);
            if (keyInfo == null)
            {
                return null;
            }
            KeyImportableAttribute keyAttribute = keyInfo.GetCustomAttributes(typeof(KeyImportableAttribute), true)[0] as KeyImportableAttribute;
            if (keyAttribute == null)
            {
                return null;
            }
            CriteriaOperator crop = new BinaryOperator(keyAttribute.PersistentLookupProperty, keyInfo.GetValue(this, null));
            if (typeof(IOwner).IsAssignableFrom(typeof(Q)))
            {
                crop = CriteriaOperator.And(new BinaryOperator(keyAttribute.PersistentLookupProperty, keyInfo.GetValue(this, null)), new BinaryOperator("Owner.Oid", owner));
            }
            return uow.FindObject<Q>(crop);
        }

        public abstract bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store);

        public bool UpdateChildren(List<IImportableViewModel> updateList, Type importableType, string property = null)
        {
            PropertyInfo listProperty = null;
            if (property != null)
            {
                listProperty = this.GetType().GetProperty(property);
            }
            if (listProperty == null)
            {
                Type genericListType = typeof(List<>).MakeGenericType(importableType);
                IEnumerable<PropertyInfo> applicableProperties = this.GetType().GetProperties().Where(x => x.PropertyType == genericListType);
                if (applicableProperties.Count() != 1)
                {
                    return false;
                }
                listProperty = applicableProperties.First();
            }
            IList objectList = listProperty.GetValue(this, null) as IList;
            if (objectList == null)
            {
                throw new Exception("ImportableViewModel.Update<" + importableType.Name + "> List in the master object has not been initialized.");
            }
            MasterDetailImportableAttribute impAttribute = listProperty.GetCustomAttributes(typeof(MasterDetailImportableAttribute), true).Cast<MasterDetailImportableAttribute>().FirstOrDefault();
            if (impAttribute == null)
            {
                throw new Exception(this.GetType().Name + "." + listProperty.Name + " is not supplied with an appropriate ImportableAttribute.");
            }
            PropertyInfo detailObjectProperty = (importableType).GetProperty(impAttribute.DetailProperty);
            PropertyInfo masterObjectProperty = GetType().GetProperty(impAttribute.MasterProperty);
            if (masterObjectProperty == null)
            {
                throw new Exception("Property " + this.GetType().Name + "." + impAttribute.MasterProperty + " cannot be found.");
            }
            if (detailObjectProperty == null)
            {
                throw new Exception("Property " + (importableType).Name + "." + impAttribute.DetailProperty + " cannot be found.");
            }
            object masterObjectValue = masterObjectProperty.GetValue(this, null);
            if (masterObjectValue == null)
            {
                return false;
            }
            foreach (IImportableViewModel obj in updateList.Where(x => masterObjectValue.Equals(detailObjectProperty.GetValue(x, null))))
            {
                objectList.Add(obj);
            }
            return true;
        }

        public virtual ImportResult Import(string textToParse, SupplierImportFileRecordHeader header)
        {
            ImportResult result = new ImportResult() { Applicable = false, Successful = false };
            if (header.EntityName == this.EntityName)
            {
                string recordType;
                string[] array = null;
                if (header.IsTabDelimited)
                {
                    array = textToParse.Split(new string[] { header.TabDelimitedString }, StringSplitOptions.None);
                    recordType = array.Length < header.Position ? array[header.Position] : string.Empty;
                }
                else
                {
                    recordType = textToParse.Length < header.Position + header.Length ? string.Empty : textToParse.Substring(header.Position, header.Length);
                }
                if (recordType != header.HeaderCode)
                {
                    return result;
                }
                result.Applicable = true;
                Dictionary<string, string> fieldResults = header.SupplierImportFileRecordFields.ToDictionary(x => x.PropertyName, x => ImportField(x, textToParse, array).Value);
                result.FieldsSuccessful = fieldResults.Count(x => string.IsNullOrWhiteSpace(x.Value));
                result.Errors = fieldResults.Where(x => string.IsNullOrWhiteSpace(x.Value) == false).ToDictionary(x => x.Key, x => x.Value);
                result.Successful = result.Errors.Count == 0;
            }
            return result;
        }

        public virtual void CheckWithDatabase(UnitOfWork uow, Guid owner)
        {
            Q persistentObject = this.GetPersistentObjectGeneric(uow, owner);
            if (persistentObject == null)
            {
                this.IsNew = true;
            }
            else
            {
                this.PersistantObjectOid = persistentObject.Oid;
            }

            //Update Lookups
            Dictionary<PropertyInfo, LookupImportableAttribute> lookupdictionary = this.GetType().GetProperties()
                .ToDictionary(x => x, x => x.GetCustomAttributes(typeof(LookupImportableAttribute), true).Cast<LookupImportableAttribute>().FirstOrDefault());
            lookupdictionary = lookupdictionary.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<PropertyInfo, LookupImportableAttribute> pair in lookupdictionary)
            {
                object propertyValue = pair.Key.GetValue(this, null);
                BaseObj persistentLookupObject = uow.FindObject(pair.Value.PersistentType, new BinaryOperator(pair.Value.PersistentProperty, propertyValue)) as BaseObj; ;

                if (persistentLookupObject != null)
                {
                    PropertyInfo pInfo = this.GetType().GetProperty(pair.Value.LocalProperty);
                    pInfo.SetValue(this, persistentLookupObject.Oid, null);
                }
            }
        }

        protected KeyValuePair<bool, string> ImportField(SupplierImportFileRecordField field, string textToParse, string[] values)
        {
            PropertyInfo fieldProperty = this.GetType().GetProperty(field.PropertyName);
            if (fieldProperty == null)
            {
                return new KeyValuePair<bool, string>(false, "Field not found");
            }
            string valueToApply;
            if (string.IsNullOrWhiteSpace(field.ConstantValue))
            {
                if (field.SupplierImportFileRecordHeader.IsTabDelimited)
                {
                    if (field.Position > values.Length)
                    {
                        return new KeyValuePair<bool, string>(false, "Field Position out of range");
                    }
                    valueToApply = values[field.Position];
                }
                else
                {
                    if (field.Position + field.Length >= textToParse.Length)
                    {
                        return new KeyValuePair<bool, string>(false, "Field Position out of range");
                    }
                    valueToApply = textToParse.Substring(field.Position, field.Length);
                }
            }
            else
            {
                valueToApply = field.ConstantValue;
            }
            if (field.Trim)
            {
                valueToApply = valueToApply.Trim();
            }
            if(field.SupplierImportMappingHeader != null)
            {
                SupplierImportMappingDetail mappingRule = 
                field.SupplierImportMappingHeader.SupplierImportMappingDetails.FirstOrDefault(x => x.InitialString == valueToApply);
                if(mappingRule !=null)
                {
                    valueToApply = mappingRule.ReplacedString;
                }
            }
            try
            {
                Type underlyingType = Nullable.GetUnderlyingType(fieldProperty.PropertyType) ?? fieldProperty.PropertyType;
                if (field.Multiplier != 0 && field.Multiplier != 1 && numericTypes.Contains(underlyingType))
                {
                    decimal numericValue = (decimal)Convert.ChangeType(valueToApply, typeof(decimal));
                    numericValue = numericValue * (decimal)field.Multiplier;
                    fieldProperty.SetValue(this, Convert.ChangeType(numericValue, underlyingType), null);
                }
                else
                {
                    fieldProperty.SetValue(this, Convert.ChangeType(valueToApply, underlyingType), null);
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, "Error during conversion: " + ex.Message);
            }
            return new KeyValuePair<bool, string>(true, string.Empty);
        }


        static List<Type> numericTypes = new List<Type>() {
             typeof(byte) ,
             typeof(SByte) ,
             typeof(UInt16) ,
             typeof(UInt32) ,
             typeof(UInt64) ,
             typeof(Int16) ,
             typeof(Int32) ,
             typeof(Int64) ,
             typeof(Decimal) ,
             typeof(Double) ,
             typeof(Single)
        };


    }
}
