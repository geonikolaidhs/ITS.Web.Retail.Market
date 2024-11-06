using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient.Helpers
{
    public static class SliderHelper
    {
        public static List<string> GetSliderImagesNames(string path)//, CompanyNew owner)
        {
            String filters="*.png|*.jpg";
            CompanyNew owner = System.Web.HttpContext.Current.Session["currentOwner"] as CompanyNew;
            string mainFolder = path;

            if (owner == null)
            {
                try
                {
                    owner = StoreControllerAppiSettings.Owner;
                }
                catch
                {
                }
            }

            if (owner != null)
            {
                String ownerFolder = owner.Oid.ToString();
                path += "\\" + ownerFolder;
            }
            try
            {
                return filters.Split('|').SelectMany(filter => Directory.GetFiles(path, filter)).Select(g => g.Replace(mainFolder, "").Replace("\\","/")).ToList();
            }
            catch(Exception exc)
            {
                string errorMessage = exc.GetFullMessage();
                return new List<String>();
            }
        }
    }
}
