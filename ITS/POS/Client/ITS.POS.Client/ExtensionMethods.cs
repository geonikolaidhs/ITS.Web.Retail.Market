using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension Method to get the real name
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string GetName(this Control control)
        {
            try
            {
                string name = control.ToString();
                return name.Split(' ')[0];
            }
            catch
            {
                return control.Name;
            }
        }
    }

    public static class DataGridViewExtensions
    {


        public static int LocateByValue(this DataGridView grd, String field, object value)
        {
            if (grd.Rows.Count <= 0)
                return -1;
            Type tp = grd.Rows[0].DataBoundItem.GetType();
            PropertyInfo prop = tp.GetProperty(field);
            if (prop == null)
                return -1;

            var row = grd.Rows.Cast<DataGridViewRow>().FirstOrDefault(g => prop.GetValue(g.DataBoundItem, null).Equals(value));
            return row.Index;
        }
    }

}
