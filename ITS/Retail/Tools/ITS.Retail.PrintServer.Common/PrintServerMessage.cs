using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITS.Common.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform;

namespace ITS.Retail.PrintServer.Common
{
    public enum ePrintServerRequestType
    {
        PRINT_DOCUMENT,
        GET_PRINTERS,
        PRINT_LABEL
    }

    public enum ePrintServerResponseType
    {
        SUCCESS,
        FAILURE
    }

    public abstract class PrintServerMessage : IMessage
    {
        public String Serialize()
        {

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("Type", this.GetType().Name);
            foreach (PropertyInfo prop in this.GetType().GetProperties().Where(p => p.CanWrite))
            {
                string propName = prop.Name;
                object propValue = prop.GetValue(this, null);
                if (propValue != null && propValue.GetType().IsSubclassOf(typeof(Enum)))
                {
                    dict.Add(propName, ((int)propValue).ToString());
                }
                else if ( prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) )
                {
                    dict.Add(propName, propValue);
                }
                else
                {
                    dict.Add(propName, propValue != null ? propValue.ToString() : "");
                }
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, PlatformConstants.JSON_SERIALIZER_SETTINGS);
        }


        public void Deserialize(String input)
        {
            JObject jsonItem = JObject.Parse(input);
            IEnumerable<JProperty> itemValues = jsonItem.Properties();
            JProperty objectType = itemValues.FirstOrDefault(g => g.Name == "Type");
            Type type = this.GetType();
            foreach (JProperty itmValue in itemValues)
            {
                try
                {
                    PropertyInfo prop = type.GetProperty(itmValue.Name);
                    if (prop != null)
                    {
                        prop.SetValue(this, itmValue.Value.ToObject(prop.PropertyType), null);
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        public string ErrorMessage { get; set; }
    }

}
