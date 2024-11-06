using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Controllers;
using ITS.Retail.Common.ViewModel;
using System.Collections;
using ITS.Retail.Common.Attributes;

namespace ITS.Retail.WebClient.Providers
{
    
    public class RetailModelBinder : DevExpressEditorsBinder
    {


        protected object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            return null;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (typeof(XPBaseObject).IsAssignableFrom(modelType))
            {


                IBaseController xpoController = controllerContext.Controller as IBaseController;
                if (xpoController == null)
                {
                    throw new InvalidOperationException("The controller does not support BaseController interface");
                }
                XPClassInfo classInfo = xpoController.XpoSession.GetClassInfo(modelType);
                if (classInfo.IsPersistent)
                {
                    ValueProviderResult result = bindingContext.ValueProvider.GetValue(classInfo.KeyProperty.Name);
                    if (result != null)
                    {
                        object ob = xpoController.XpoSession.GetObjectByKey(classInfo, result.ConvertTo(classInfo.KeyProperty.MemberType));
                        if (ob != null)
                        {
                            return ob;
                        }
                    }
                }
                return classInfo.CreateNewObject(xpoController.XpoSession);
            }
            else
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
        }

        protected virtual void BindCustomProperty(HttpRequestBase Request, PropertyInfo propertyInfo, object Model, BindingAttribute attribute, string keyprefix = "")
        {
            if (attribute == null || attribute.ComplexObject == false)
            {
                string searchKey = keyprefix + (attribute == null ? propertyInfo.Name : attribute.BindingRequestKey);
                if(Request.Params.AllKeys.Contains(searchKey) == false )
                {
                    return;
                }
                string stringValue = Request[searchKey];
                object value;

                value = (attribute == null || attribute.DefaultValue == null) ? GetDefaultValue(propertyInfo.PropertyType) : attribute.DefaultValue;

                if (attribute != null && attribute.IsCheckBox)
                {
                    value = stringValue == "C";
                }
                else if (typeof(Guid).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    Guid gValue = Guid.Empty;
                    Guid.TryParse(stringValue, out gValue);
                    value = gValue;
                }
                else if (typeof(Guid?).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    Guid? gValue = null;
                    Guid gv;
                    if (Guid.TryParse(stringValue, out gv))
                    {
                        gValue = gv;
                    }
                    value = gValue;
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    if (!String.IsNullOrWhiteSpace(stringValue))
                    {
                        value = Enum.Parse(propertyInfo.PropertyType, stringValue);
                    }
                    else
                    {
                        value = GetDefaultValue(propertyInfo.PropertyType);
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && attribute !=null && attribute.EnumerableType !=null)
                {
                    MethodInfo method = typeof(CheckBoxListExtension).GetMethod("GetSelectedValues");
                    if(method !=null)
                    {
                        method = method.MakeGenericMethod(attribute.EnumerableType);
                        value = method.Invoke(null, new[] { searchKey });
                    }
                }
                else
                {
                    Type undelyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    if (!String.IsNullOrWhiteSpace(stringValue))
                    {
                        if (undelyingType == typeof(DateTime))
                        {
                            DateTime dtime;
                            if (DateTime.TryParse(stringValue, out dtime) == true)
                            {
                                value = dtime;
                            }
                        }
                        else
                        {
                            value = Convert.ChangeType(stringValue, undelyingType);
                        }
                    }
                    else if(undelyingType == typeof(String))
                    {
                        value = stringValue;
                    }
                }
                propertyInfo.SetValue(Model, value, null);
            }
            else
            {
                object ComplexObject = propertyInfo.GetValue(Model, null);
                if (ComplexObject == null)
                {
                    ComplexObject = Activator.CreateInstance(propertyInfo.GetType());
                }
                foreach (PropertyInfo propInfo in ComplexObject.GetType().GetProperties().Where(p => p.CanWrite == true))
                {
                    BindingAttribute subAttribute = propInfo.GetCustomAttributes(typeof(BindingAttribute), true).Cast<BindingAttribute>().FirstOrDefault();
                    BindCustomProperty(Request, propInfo, ComplexObject, subAttribute, propertyInfo.Name + attribute.Separator);
                }
                propertyInfo.SetValue(Model, ComplexObject, null);
            }
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            BindingAttribute attribute = propertyDescriptor.Attributes.Cast<Attribute>().Where(g => typeof(BindingAttribute).IsAssignableFrom(g.GetType())).Cast<BindingAttribute>().FirstOrDefault();

            BindingListAttribute bindingListAttribute = propertyDescriptor.Attributes.Cast<Attribute>().Where(g => typeof(BindingListAttribute).IsAssignableFrom(g.GetType())).Cast<BindingListAttribute>().FirstOrDefault();//There is only one attribute defined per property or no attribute at all

            if (attribute == null && bindingListAttribute == null)
            {
                try
                {
                    base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
                }
                catch
                {                   
                } 
                return;
            }

            if ( attribute !=null  )
            {
                try
                {
                    BindCustomProperty(controllerContext.RequestContext.HttpContext.Request, propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name), bindingContext.Model, attribute);
            }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(propertyDescriptor.Name, ex);
                }
            }


            if (bindingListAttribute != null)
            {
            try
            {
                    BindCustomListProperty(controllerContext.RequestContext.HttpContext.Request, propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name), bindingContext.Model, bindingListAttribute);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(propertyDescriptor.Name, ex);
            }
            }

        }

        private void BindCustomListProperty(HttpRequestBase request, PropertyInfo propertyInfo, object model, BindingListAttribute bindingListAttribute)
        {
            IEnumerable<string> requestKeys = request.Params.AllKeys.Where(key => key.StartsWith(bindingListAttribute.Prefix));
            Type underlyingListType= propertyInfo.PropertyType.GetGenericArguments().ToList().FirstOrDefault();
            //Type runtimeListType = typeof(List<>).MakeGenericType(underlyingListType);
            IList list = Activator.CreateInstance(propertyInfo.PropertyType) as IList;


            List<PropertyInfo> reflectedTypeProperties = underlyingListType ==null ? new List<PropertyInfo>() : underlyingListType.GetProperties().Where(property => property.CanWrite).ToList();

            if (reflectedTypeProperties.Count > 0)
            {

                string seperator = GetBindListAttributeSeperator();

                int numberOfListElements = GetNumberOfListElements(request, bindingListAttribute,reflectedTypeProperties.First());

                for (int counter = 1; counter <= numberOfListElements; counter++)
                {
                    var listObject = Activator.CreateInstance(underlyingListType);
                    foreach (PropertyInfo reflectedPropertyInfo in reflectedTypeProperties)
                    {
                        BindingListAttributeSuffix bindingListAttributeSuffix = reflectedPropertyInfo.GetCustomAttributes(typeof(BindingListAttributeSuffix), true).Cast<BindingListAttributeSuffix>().FirstOrDefault();

                        
                        string requestKey = BuildRequestKey(bindingListAttribute, reflectedPropertyInfo, counter);
                        if (request.Params.AllKeys.Contains(requestKey))
                        {
                            object requestValue = request[requestKey];
                            var val = requestValue;

                            if (reflectedPropertyInfo.PropertyType.IsEnum)
                            {
                                val = Enum.Parse(reflectedPropertyInfo.PropertyType, requestValue.ToString());
                            }
                            reflectedPropertyInfo.SetValue(listObject, val, null);
                        }                        
                    }
                    list.Add(listObject);
                }
                propertyInfo.SetValue(model, list, null);
            }
        }

        private int GetNumberOfListElements(HttpRequestBase request, BindingListAttribute bindingListAttribute , PropertyInfo propertyInfo)
        {
            BindingListAttributeSuffix bindingListAttributeSuffix = propertyInfo.GetCustomAttributes(typeof(BindingListAttributeSuffix), true).Cast<BindingListAttributeSuffix>().FirstOrDefault();
            
            int counter = 1;
            string requestKey = BuildRequestKey(bindingListAttribute, propertyInfo, counter);
            while (request.Params.AllKeys.Contains(requestKey))
            {
                counter++;
                requestKey = BuildRequestKey(bindingListAttribute, propertyInfo, counter);
            }
            return --counter;
        }

        private string BuildRequestKey(BindingListAttribute bindingListAttribute, PropertyInfo reflectedPropertyInfo,int counter)
        {
            string seperator = GetBindListAttributeSeperator();
            string requestKey = bindingListAttribute.Prefix + seperator + counter + seperator + reflectedPropertyInfo.Name;
            BindingListAttributeSuffix bindingListAttributeSuffix = reflectedPropertyInfo.GetCustomAttributes(typeof(BindingListAttributeSuffix), true).Cast<BindingListAttributeSuffix>().FirstOrDefault();

            if (bindingListAttributeSuffix != null)
            {
                requestKey += seperator + bindingListAttributeSuffix.Suffix;
            }

            return requestKey;
        }

        private string GetBindListAttributeSeperator()
        {
            return "_";
        }
    }
}