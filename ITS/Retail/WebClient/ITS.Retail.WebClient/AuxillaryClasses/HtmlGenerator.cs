using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using System.Web.Mvc.Html;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public static class HtmlGenerator
    {
        public static void CreateViewHeader(HtmlHelper htmlHelper,ViewDataDictionary ViewData,HeaderOptions headerOptions,bool renderPartial)
        {
            string viewPath = "../Shared/ViewHeader";
            ViewData["HeaderOptions"] = headerOptions;
            if (renderPartial)
            {
                htmlHelper.RenderPartial(viewPath,viewData: ViewData);
            }
            else
            {
                htmlHelper.Partial(viewPath, viewData: ViewData);
            }
        }
    }
}