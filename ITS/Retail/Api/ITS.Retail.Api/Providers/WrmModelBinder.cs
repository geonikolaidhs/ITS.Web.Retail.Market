
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using ITS.Retail.Platform.Kernel.Model;
using Newtonsoft.Json.Linq;
using ITS.Retail.Model;

namespace ITS.Retail.Api.Providers
{
    /// <summary>
    /// The default binder for WRM Model objects
    /// </summary>
    public class WrmModelBinder : IModelBinder
    {

        /// <summary>
        /// Creates a new WRM Model Binder, the default binder for WRM Model objects
        /// </summary>
        public WrmModelBinder()
        {

        }

        /// <summary>
        /// The default binding procedure, as required by IModelBinder interfache
        /// </summary>
        /// <param name="actionContext">The action context</param>
        /// <param name="bindingContext">The binding context</param>
        /// <returns></returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (typeof(IPersistentObject).IsAssignableFrom(bindingContext.ModelType) == false)
            {
                return false;
            }

            try
            {
                //Creating a new IWRMDbModule 
                IWRMDbModule wrmDbModule = actionContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(IWRMDbModule)) as IWRMDbModule;
                if (wrmDbModule == null)
                {
                    return false;
                }

                BasicObj bindingModel = null;
                Guid? providedKey = null, jsonObjectKey = null;
                Guid actualKey = Guid.Empty;
                if (actionContext.ActionArguments.ContainsKey("key") && actionContext.ActionArguments["key"] is Guid)
                {
                    providedKey = (Guid)actionContext.ActionArguments["key"];
                }

                string json = ExtractRequestJson(actionContext);
                JObject jsonObject = JObject.Parse(json);
                JProperty jsonKeyProperty = jsonObject.Properties().FirstOrDefault(prop => prop.Name == "Oid");
                if (jsonKeyProperty != null)
                {
                    if (jsonKeyProperty.Value.Type == JTokenType.Guid)
                    {
                        jsonObjectKey = jsonKeyProperty.Value.Value<Guid?>();
                    }
                    else if (jsonKeyProperty.Value.Type == JTokenType.String)
                    {
                        Guid tmp;
                        if(Guid.TryParse(jsonKeyProperty.Value.Value<string>(),out tmp))
                        {
                            jsonObjectKey = tmp;
                        }
                    }
                }
                if (jsonObjectKey.HasValue && providedKey.HasValue)
                {
                    if (jsonObjectKey.Value == providedKey.Value)
                    {
                        actualKey = providedKey.Value;
                    }
                    else
                    {
                        throw new Exception("Key and Oid are different");
                    }
                }
                else if (jsonObjectKey.HasValue)
                {
                    actualKey = jsonObjectKey.Value;
                }
                else if (providedKey.HasValue)
                {
                    actualKey = providedKey.Value;
                }


                bindingModel = wrmDbModule.GetObjectByKey(actualKey, bindingContext.ModelType) as BasicObj;
                if (actualKey == Guid.Empty || bindingModel == null)
                {
                    bindingModel = wrmDbModule.CreateObject(bindingContext.ModelType) as BasicObj;
                    if (actualKey != Guid.Empty)
                    {
                        bindingModel.Oid = actualKey;
                    }
                }
                string error;
                if (bindingModel.FromJson(jsonObject, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error) == false)
                {
                    bindingContext.ModelState.AddModelError("binding", error);
                }
                bindingContext.Model = bindingModel;

                //bindingContext.OperationBindingContext.HttpContext.RequestServices
                return true;
            }
            catch (Exception exception)
            {
                bindingContext.ModelState.AddModelError("JsonDeserializationException", exception);
                return false;
            }
        }
        private string ExtractRequestJson(HttpActionContext actionContext)
        {
            var content = actionContext.Request.Content;
            string json = content.ReadAsStringAsync().Result;
            return json;
        }

        /*public WrmModelBinder(IWRMDbModule wrmDbModule)
{
   this.wrmDbModule = wrmDbModule;
}*/

        /* protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
         {
             if (typeof(IPersistentObject).IsAssignableFrom(modelType))
             {
                 ValueProviderResult result = bindingContext.ValueProvider.GetValue("Oid");
                 if(result !=null)
                 {
                     object objToRetun = wrmDbModule.GetObjectByKey((Guid)result.ConvertTo(typeof(Guid)), modelType);
                     if(objToRetun !=null)
                     {
                         return objToRetun;
                     }
                 }
                 return wrmDbModule.CreateObject(modelType);
             }
             else
             {
                 return base.CreateModel(controllerContext, bindingContext, modelType);
             }
         }*/

    }
}