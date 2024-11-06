using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Platform.Enumerations.Attributes
{
    public class WrmDisplayAttribute : System.Attribute
    {
        public string Name { get; set; }
        public Type ResourceType { get; set; }



        public string GetName()
        {
            string toReturn = null;
            if (ResourceType == typeof(ITS.POS.Resources.POSClientResources))
            {
                toReturn = POS.Resources.POSClientResources.ResourceManager.GetString(Name);
            }
            else if (ResourceType == typeof(ITS.Retail.ResourcesLib.Resources))
            {
                toReturn = ResourcesLib.Resources.ResourceManager.GetString(Name);
            }
            else
            {
                try
                {
                    PropertyInfo pInfo = ResourceType.GetProperty("ResourceManager");
                    if (pInfo != null)
                    {
                        System.Resources.ResourceManager manager = pInfo.GetValue(null, null) as System.Resources.ResourceManager;
                        if (manager != null)
                        {
                            toReturn = manager.GetString(Name);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            if (string.IsNullOrWhiteSpace(toReturn))
            {
                return Name;
            }
            return toReturn;
        }


    }
}
