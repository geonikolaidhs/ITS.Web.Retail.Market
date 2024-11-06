using DevExpress.Xpo;
using ITS.Retail.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ItemCategoryHelper
    {
        private static string serverUrl;
        public static string GenerateTreeHtml(XPCollection<ItemCategory> categories,string serverMapPathUrl)
        {
            string htmlTree = "";

            serverUrl = serverMapPathUrl;

            if(categories.Count > 0)
            {
                foreach(ItemCategory category in categories)
                {
                    htmlTree += string.Format("<ul>");
                    htmlTree += GetOneNodeHtml(category);
                    htmlTree += "</ul>";
                }
            }
            return htmlTree;
        }   

        private static string GetOneNodeHtml(ItemCategory category)
        {
            string html = "";
            string url = serverUrl + category.Oid;

            html += string.Format("<li><a href='{0}' target='_blank'>{1}</a>", url /*+ category.Oid.ToString()*/, category.Description);

            if (category.HasChild)
            {
                html += string.Format("<h2><a href="+ url +">{0}</a></h2><ul>", category.Description);

                foreach (ItemCategory cat in category.ChildCategories)
                {
                    html += GetOneNodeHtml(cat);
                }
                html += "</ul>";
            }

            html += "</li>";


            return html;
        }


        public static JObject GenerateTreeJson(XPCollection<ItemCategory> categories)
        {
            JObject jTree = new JObject();
            JArray jArray = new JArray();

            //jTree.Add("title", ResourcesLib.Resources.ItemCategories);
            //jTree.Add("icon", "fa fa-tags fa-2x");

            if (categories.Count > 0)
            {
                foreach (ItemCategory category in categories)
                {
                    jArray.Add(GetOneNodeJson(category));
                }
            }


            jTree.Add("items",jArray);
            return jTree;
        }

        private static JObject GetOneNodeJson(ItemCategory category)
        {
            JObject jNode = new JObject();
                        
            //jNode.Add("id",category.Oid.ToString());


            jNode.Add("link", "/Products?CategoryID=" + category.Oid);
            jNode.Add("icon", "");

            //string property = level % 2 != 0 ? "name" : "title";
            //jNode.Add(property, category.Description);
            

            //if(title && category.HasChild)
            //{
            //    jNode.Add("title",category.Parent.Description);
            //}
            //else if(!category.HasChild)
            //{
            //    jNode.Add("name", category.Description);
            //}
            
            //if (category.HasChild)
            //{
            //    //if (!title)
            //    //{
            //    //    jNode.Add("name", category.Description);
            //    //}
                
            //    JArray items = new JArray();
            //    foreach (CategoryNode childCategory in category.ChildCategories)
            //    {
            //        items.Add( GetOneNodeJson((ItemCategory)childCategory,level+1) );
            //    }
            //    jNode.Add("items", items);
            //}           

            return jNode;
        }
    }
}