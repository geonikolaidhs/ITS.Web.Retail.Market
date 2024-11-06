using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model;
using System.Reflection;
using System.Collections;

namespace ITS.Retail.Common.ViewModel
{
    public static class PersistentHelper
    {
        public static void Persist<T>(this IPersistableViewModel viewModel, T persistant, bool ignoreValidation = false) where T : BasicObj
        {
            string message;
            if(ignoreValidation == false &&viewModel.Validate(out message) == false)
            {                
                throw new Exception("Invalid data:" + message);
            }
            if(viewModel.IsDeleted)
            {
                persistant.Delete();
                return;
            }
            foreach (PropertyInfo pInfo in viewModel.GetType().GetProperties())
            {
                string pInfoName = pInfo.Name;
                PersistableViewModelAttribute attribute = pInfo.GetCustomAttributes(typeof(PersistableViewModelAttribute), true).Cast<PersistableViewModelAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    if (attribute.NotPersistant)
                    {
                        continue;
                    }
                    else if (String.IsNullOrWhiteSpace(attribute.PersistantObjectPropertyName) == false)
                    {
                        pInfoName = attribute.PersistantObjectPropertyName;
                    }
                }
                PropertyInfo persistantPropertyInfo = persistant.GetType().GetProperty(pInfoName);
                if (persistantPropertyInfo != null)
                {
                    object nonPersistantValue = pInfo.GetValue(viewModel, null);
                    if (nonPersistantValue == null)
                    {
                        persistantPropertyInfo.SetValue(persistant, null, null);
                        continue;
                    }
                    if (typeof(BasicObj).IsAssignableFrom(persistantPropertyInfo.PropertyType))
                    {
                        if (nonPersistantValue is IPersistableViewModel)
                        {
                            IPersistableViewModel cvo = nonPersistantValue as IPersistableViewModel;
                            BasicObj currentObject = persistantPropertyInfo.GetValue(persistant, null) as BasicObj;
                            if (currentObject == null)
                            {
                                currentObject = persistant.Session.GetObjectByKey(persistantPropertyInfo.PropertyType, cvo.Oid) as BasicObj;
                                if (currentObject == null)
                                {
                                    currentObject = Activator.CreateInstance(cvo.PersistedType, new object[] { persistant.Session }) as BasicObj;
                                }
                            }
                            cvo.Persist(currentObject, ignoreValidation);
                            persistantPropertyInfo.SetValue(persistant, currentObject, null);
                        }
                        else
                        {
                            object value =  persistant.Session.GetObjectByKey(persistantPropertyInfo.PropertyType, nonPersistantValue,false);
                            if(value == null)
                            {
                                value = persistant.Session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,persistantPropertyInfo.PropertyType, new BinaryOperator("Oid",nonPersistantValue));
                            }
                            persistantPropertyInfo.SetValue(persistant,value, null);
                        }
                    }
                    else if (typeof(XPBaseCollection).IsAssignableFrom(persistantPropertyInfo.PropertyType) && typeof(IList).IsAssignableFrom(pInfo.PropertyType))
                    {
                        IList list = (nonPersistantValue as IList);
                        if (list.Count > 0 && pInfo.PropertyType.IsGenericType)
                        {
                            XPBaseCollection baseCollection = persistantPropertyInfo.GetValue(persistant, null) as XPBaseCollection;
                            Type generic = pInfo.PropertyType.GetGenericArguments()[0];
                            Type persistantGenericType = persistantPropertyInfo.PropertyType.GetGenericArguments()[0];
                            if (generic == typeof(Guid))
                            {

                                foreach (Guid g in list as List<Guid>)
                                {
                                    BaseObj found = persistant.Session.GetObjectByKey(persistantGenericType, g) as BaseObj;
                                    if (found != null)
                                    {
                                        baseCollection.BaseAdd(found);
                                    }
                                }
                            }
                            else if (typeof(IPersistableViewModel).IsAssignableFrom(generic))
                            {
                                foreach (var childViewObject in list)
                                {
                                    if (childViewObject is IPersistableViewModel)
                                    {
                                        IPersistableViewModel cvo = (IPersistableViewModel)childViewObject;
                                        object childPersistantObject = persistant.Session.GetObjectByKey(persistantGenericType, cvo.Oid);
                                        if (childPersistantObject == null)
                                        {
                                            childPersistantObject = Activator.CreateInstance(cvo.PersistedType, new object[] { persistant.Session });
                                        }
                                        cvo.Persist(childPersistantObject as BasicObj, ignoreValidation);
                                        baseCollection.BaseAdd(childPersistantObject);
                                    }
                                }
                            }
                        }
                    }
                    else if (persistantPropertyInfo.CanWrite)
                    {
                        persistantPropertyInfo.SetValue(persistant, nonPersistantValue, null);
                    }
                }
            }
        }



        public static IPersistableViewModel LoadPersistent<T>(this IPersistableViewModel viewModel, T persistant) where T : BasicObj
        {
            foreach (PropertyInfo pInfo in viewModel.GetType().GetProperties())
            {
                string pInfoName = pInfo.Name;
                PersistableViewModelAttribute attribute = pInfo.GetCustomAttributes(typeof(PersistableViewModelAttribute), true).Cast<PersistableViewModelAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    if (attribute.NotPersistant)
                    {
                        continue;
                    }
                    else if (String.IsNullOrWhiteSpace(attribute.PersistantObjectPropertyName) == false)
                    {
                        pInfoName = attribute.PersistantObjectPropertyName;
                    }
                }
                PropertyInfo persistantPropertyInfo = persistant.GetType().GetProperty(pInfoName);
                if (persistantPropertyInfo != null)
                {
                    object persistantValue = persistantPropertyInfo.GetValue(persistant, null);

                    if (persistantValue == null)
                    {
                        continue;
                    }


                    if (persistantValue is BasicObj)
                    {
                        //Lookup Property
                        BasicObj value = (BasicObj)persistantValue;
                        if (pInfo.PropertyType == typeof(Guid) || pInfo.PropertyType == typeof(Guid?))
                        {
                            pInfo.SetValue(viewModel, value.Oid, null);
                        }
                        else if (typeof(IPersistableViewModel).IsAssignableFrom(pInfo.PropertyType))
                        {
                            IPersistableViewModel obj = (IPersistableViewModel)Activator.CreateInstance(pInfo.PropertyType);
                            obj.LoadPersistent(value);
                            obj.UpdateModel(persistant.Session);
                            pInfo.SetValue(viewModel, obj, null);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else if (persistantValue is XPBaseCollection && typeof(IList).IsAssignableFrom(pInfo.PropertyType))
                    {
                        //Collection  
                        IList list = (IList)pInfo.GetValue(viewModel, null);
                        Type childViewModelType = pInfo.PropertyType.GetGenericArguments()[0];
                        if (childViewModelType == typeof(Guid) || childViewModelType == typeof(Guid?))
                        {
                            foreach (var obj in (XPBaseCollection)persistantValue)
                            {
                                BasicObj value = (BasicObj)obj;
                                list.Add(value.Oid);
                            }
                        }
                        else
                        {
                            IEnumerable<Type> types = childViewModelType.Assembly.GetTypes().Where(x => childViewModelType.IsAssignableFrom(x) && x.IsAbstract == false);
                            Dictionary<Type, IPersistableViewModel> objectDictionary = types.ToDictionary(x => x, x => (IPersistableViewModel)Activator.CreateInstance(x));
                            foreach (var obj in (XPBaseCollection)persistantValue)
                            {
                                BasicObj value = (BasicObj)obj;
                                IPersistableViewModel cvm;
                                var pair = objectDictionary.Where(x => x.Value.PersistedType == value.GetType());
                                if (pair.Count() > 0)
                                {
                                    cvm = (IPersistableViewModel)Activator.CreateInstance(pair.First().Key);
                                    cvm.LoadPersistent(value);
                                    cvm.UpdateModel(persistant.Session);
                                    list.Add(cvm);
                                }
                            }
                        }
                    }
                    else if(pInfo.PropertyType.IsEnum)
                    {
                        int pval = (int)persistantValue;
                        pInfo.SetValue(viewModel, pval, null);
                    }
                    else if(pInfo.CanWrite)
                    {
                        //Simple Property
                        pInfo.SetValue(viewModel, persistantValue, null);
                    }
                }
            }
            viewModel.UpdateModel(persistant.Session);
            return viewModel;
        }

        public static void UpdateProperties(this IPersistableViewModel viewModel, Session uow)
        {
            IEnumerable<PropertyInfo> propertiesToUpdate = viewModel.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(UpdateViewModelAttribute), true).Count() > 0);
            foreach(PropertyInfo pInfo in propertiesToUpdate)
            {
                UpdateViewModelAttribute attribute = (UpdateViewModelAttribute)pInfo.GetCustomAttributes(typeof(UpdateViewModelAttribute), true).First();
                PropertyInfo lookupPropertyInfo = viewModel.GetType().GetProperty(attribute.ViewModelPropertyName);
                object lookupKey = lookupPropertyInfo.GetValue(viewModel, null);
                object lookupObject = uow.GetObjectByKey(attribute.ModelType, lookupKey);
                if(lookupObject !=null && pInfo.PropertyType.IsAssignableFrom(attribute.ModelProperty.PropertyType))
                {
                    object valueToAssign = attribute.ModelProperty.GetValue(lookupObject, null);
                    pInfo.SetValue(viewModel, valueToAssign, null);
                }
            }
        }
    }
}


