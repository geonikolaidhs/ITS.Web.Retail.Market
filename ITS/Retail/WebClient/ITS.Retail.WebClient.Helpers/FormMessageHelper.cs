using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class FormMessageHelper
    {
        public static bool FormMessageIsValid(FormMessage formMessage)
        {
            var groups = formMessage.FormMessageDetails.Where(formMessageDetail => formMessageDetail.IsDefault).GroupBy(formMessageDetail => formMessageDetail.Locale);
            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
