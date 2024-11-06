
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using ITS.Retail.WebClient;

namespace System.Web.Mvc.Html
{
    public static class LinkExtensions
    {
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, routeValues, new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, null, routeValues, htmlAttributes, requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool requireAbsoluteUrl)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool requireAbsoluteUrl)
        {
            if (requireAbsoluteUrl)
            {
                HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
                RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);

                routeData.Values["controller"] = controllerName;
                routeData.Values["action"] = actionName;

                DomainRoute domainRoute = routeData.Route as DomainRoute;
                if (domainRoute != null)
                {
                    DomainData domainData = domainRoute.GetDomainData(new RequestContext(currentContext, routeData), routeData.Values);
                    return htmlHelper.ActionLink(linkText, actionName, controllerName, domainData.Protocol, domainData.HostName, domainData.Fragment, routeData.Values, null);
                }
            }
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }
        /*
        public static IHtmlString AdvRouteLink(this HtmlHelper htmlHelper, string linkText, string routeName)
        {
            return htmlHelper.AdvRouteLink(linkText, routeName, new { }, new { });
        }

        public static IHtmlString AdvRouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues)
        {
            return htmlHelper.AdvRouteLink(linkText, routeName, routeValues, new { });
        }

        public static IHtmlString AdvRouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, object routeValues, object htmlAttributes)
        {
            RouteValueDictionary routeValueDict = new RouteValueDictionary(routeValues);
            var request = htmlHelper.ViewContext.RequestContext.HttpContext.Request;
            string host = request.IsLocal ? request.Headers["Host"] : request.Url.Host;
            if (host.IndexOf(":") >= 0)
                host = host.Substring(0, host.IndexOf(":"));

            string url = UrlHelper.GenerateUrl(routeName, null, null, routeValueDict, RouteTable.Routes, htmlHelper.ViewContext.RequestContext, false);
            var virtualPathData = RouteTable.Routes.GetVirtualPathForArea(htmlHelper.ViewContext.RequestContext, routeName, routeValueDict);

            var route = virtualPathData.Route as DomainRoute;

            string actualSubdomain = SubdomainRoute.GetSubdomain(host);
            if (!string.IsNullOrEmpty(actualSubdomain))
                host = host.Substring(host.IndexOf(".") + 1);

            if (!string.IsNullOrEmpty(route.Subdomain))
                host = string.Concat(route.Subdomain, ".", host);
            else
                host = host.Substring(host.IndexOf(".") + 1);

            UriBuilder builder = new UriBuilder(request.Url.Scheme, host, 80, url);

            if (request.IsLocal)
                builder.Port = request.Url.Port;

            url = builder.Uri.ToString();

            return htmlHelper.Link(linkText, url, htmlAttributes);
        }*/
    }
}