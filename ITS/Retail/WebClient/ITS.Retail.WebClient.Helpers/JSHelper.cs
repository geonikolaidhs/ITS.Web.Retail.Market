using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ITS.Retail.WebClient.Helpers
{
    public static class JSHelper
    {
        /// <summary>
        /// Returns the string value of the field Field
        /// from the Web Form.        /// 
        /// </summary>
        /// <param name="field">The field as it appears on the form whithout _I,_S on the end etc...</param>
        /// <param name="ajaxInput">The $.ajax input posted back to the Server</param>
        /// <returns></returns>
        public static string GetInputValue(string field,string ajaxInput ="form_data"){
            return HttpContext.Current.Request[ajaxInput+'['+field+']'];
        }
    }
}
