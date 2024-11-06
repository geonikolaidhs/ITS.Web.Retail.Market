using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ResourcesExtensionMethods
    {

        public static string ToUpperGR(this string str)
        {
            string result = str.ToUpper();

            result = result.Replace("Ά", "Α").Replace("Έ", "Ε").Replace("Ύ", "Υ").Replace("Ί", "Ι").Replace("Ό", "Ο").Replace("Ή", "Η").Replace("Ώ", "Ω")
                     .Replace("΅Ι", "Ϊ").Replace("΅Υ", "Ϋ");
            return result;
        }

    }
}
