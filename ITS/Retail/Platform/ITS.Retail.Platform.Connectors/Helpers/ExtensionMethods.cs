using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace System
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Limits a string to a maximum size, truncating anything that exceeds the limit
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string Limit(this string input, int maxSize)
        {
            int length = input.Length;
            int difference = length - maxSize;
            if (difference > 0)
            {
                return input.Substring(0, length - difference);
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Replaces a char at the given index
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public static string ReplaceAt(this string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        /// <summary>
        /// Reports all the zero-based indexes of the specified Unicode character in this string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int[] IndexesOf(this string input,char character)
        {
            var foundIndexes = new List<int>();
            for (int i = input.IndexOf(character); i > -1; i = input.IndexOf(character, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                foundIndexes.Add(i);
            }
            return foundIndexes.ToArray();
        }

        /// <summary>
        /// Gets the full message of the inner exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetFullMessage(this Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }
            else
            {
                return ex.Message + Environment.NewLine + GetFullMessage(ex.InnerException);
            }
        }

        /// <summary>
        /// Gets the full stack trace of the inner exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetFullStackTrace(this Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.StackTrace;
            }
            else
            {
                return ex.StackTrace + Environment.NewLine + GetFullStackTrace(ex.InnerException);
            }
        }

        public static string GetDisplayName(this MemberInfo type)
        {
            DisplayAttribute disp = type.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().FirstOrDefault();
            string value = type.Name;
            if (disp != null)
            {
                var Rm = disp.ResourceType.GetProperty(disp.Name);
                if (Rm != null)
                {
                    value = Rm.GetValue(null, null).ToString();
                }
            }
            return value;           
        }

        /// <summary>
        /// Checks if an IEnumerable contains an other IEnumerable sequence, respecting order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool ContainsSequence<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            int count = other.Count();

            while (source.Any())
            {
                if (source.Take(count).SequenceEqual(other))
                    return true;
                source = source.Skip(1);
            }
            return false;
        }


        public static bool IsReallyAssignableFrom(this Type type, Type otherType)
        {
            if (type.IsAssignableFrom(otherType))
                return true;

            try
            {
                var v = Expression.Variable(otherType);
                var expr = Expression.Convert(v, type);
                return expr.Method == null || expr.Method.Name == "op_Implicit";
            }
            catch (InvalidOperationException ex)
            {
                string errorMessage = ex.GetFullMessage();
                return false;
            }
        }
    }
}
