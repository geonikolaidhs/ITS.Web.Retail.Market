using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ITS.Common.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform;


namespace ITS.POS.Fiscal.Common
{
    public enum eFiscalRequestType
    {
        SIGN_DOCUMENT,
        ISSUE_Z,
        RESEND_SIGN_FILES,
        CHECK_SYSTEM,
        GET_ID_SN,
        GET_VERSION_INFO,
        CHECK_ONLINE_STATUS,
        SET_FISCAL_ON_ERROR

    }

    //public enum eFiscalResponseType
    //{
    //    SUCCESS,
    //    FAILURE,
    //    FISCAL_IS_ON_ERROR
    //}

    public abstract class FiscalMessage : IMessage
    {
        public String Serialize()
        {

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("Type", this.GetType().Name);
            foreach (PropertyInfo prop in this.GetType().GetProperties().Where(p=>p.CanWrite))
            {
                string propName = prop.Name;
                object propValue = prop.GetValue(this, null);
                if (propValue != null && propValue.GetType().IsSubclassOf(typeof(Enum)))
                {
                    dict.Add(propName, ((int)propValue).ToString());
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
