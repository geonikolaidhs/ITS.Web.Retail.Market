using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ITS.Retail.WebClient.Extensions
{
    public static class HtmlExtensions
    {

        public static MvcHtmlString EditorForProperty<TModel>(this HtmlHelper<TModel> html, PropertyInfo property, TModel model)
        {

            ParameterExpression pe = System.Linq.Expressions.Expression.Parameter(typeof(TModel), "model");
            MemberExpression propertyCall = System.Linq.Expressions.Expression.Property(pe, property);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TModel),property.PropertyType);
            var exprType = typeof(Expression<>).MakeGenericType(funcType);
            var labdaCreationMethod = typeof(Expression).GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(x => x.Name == "Lambda" && x.IsGenericMethod == true && x.GetParameters().Count() == 2 &&
                                                            x.GetParameters()[0].ParameterType == typeof(Expression) &&
                                                            x.GetParameters()[1].ParameterType == typeof(ParameterExpression[])).MakeGenericMethod(funcType);
            object expresion = labdaCreationMethod.Invoke(null, new object[] { propertyCall, new ParameterExpression[] { pe } });

            //var expresion2 = Expression.Lambda<Func<TModel, TValue>>(propertyCall, pe);
            MethodInfo editorForMethod = typeof(EditorExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(x => x.GetParameters().Count() == 2 && x.Name == "EditorFor").MakeGenericMethod(typeof(TModel),property.PropertyType);
            return editorForMethod.Invoke(null, new object[] { html, expresion}) as MvcHtmlString;//html.EditorFor<TModel, TValue>(expresion);
        }

        // Extension method
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, string controller, object routeValues, string imagePath, string alt, string title = null)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            imgBuilder.MergeAttribute("title", title);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, controller, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText = null, string title = null, string height = null)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", url.Content(src));
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("title", title);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}