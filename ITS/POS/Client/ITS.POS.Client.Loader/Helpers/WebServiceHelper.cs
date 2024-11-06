using POSLoader.WebServiceAutoGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace POSLoader.Helpers
{
    public static class WebServiceHelper
    {
        private static Dictionary<string, WebServiceList> webServiceLists = new Dictionary<string, WebServiceList>();


        public static object ExecuteCallWebServiceCommand(string web_service_url,string web_service_method, Dictionary<string,object> args )
		{
            object ret = null;
			try
			{
				WebServiceList wlist;
                if(!webServiceLists.ContainsKey(web_service_url) ){
                    webServiceLists[web_service_url] = WebServiceList.LoadFromUrl(web_service_url);
                }               
                wlist = webServiceLists[web_service_url];

				if (wlist.Services.Count() == 0)
					throw new Exception("No Webservice found at " + web_service_url);
				WebMethod wm = wlist.FindMethod(web_service_method);
				if (wm == null)
				{
					throw new Exception("The method " + web_service_method + " cannot be found");
				}

				object arg = wm.Arg;
				if (args != null)
				{
					WebMethodHelper.AssignWebMethodParameters(ref arg,args);
				}

                object[] outputParameters = null;

				
				ret = wm.Invoke(out outputParameters);

			}
			catch (Exception )
			{
                throw;
			}
			return ret;
		}
    }
}
