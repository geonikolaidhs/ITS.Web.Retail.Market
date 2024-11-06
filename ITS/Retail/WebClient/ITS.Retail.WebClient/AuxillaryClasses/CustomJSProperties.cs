using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class CustomJSProperties : Dictionary<string,object>
    {
        //Dictionary<string, object> innerDict;

        public CustomJSProperties()
        {
            //innerDict = new Dictionary<string, object>();
        }

        public void AddJSProperty(string name, object value)
        {
            //innerDict[name] = value;
            if (this.ContainsKey(name))
            {
                this[name] = value;
            }
            else
            {
                this.Add(name, value);
            }
        }

        public void RemoveJSProperty(string name)
        {
            this.Remove(name);
        }

        public string ToJavascript()
        {
            string retval = "";
            foreach (KeyValuePair<string, object> pair in this)
            {
                if (pair.Value != null)
                {
                    retval += "var " + pair.Key + " = '" + pair.Value.ToString() + "' ; ";
                }
            }

            return retval;
        }
    }
}