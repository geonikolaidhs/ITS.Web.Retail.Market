using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ITS.Common.Communication
{
    internal static class Constants
    {
        public const char EndOfMessageChar = '\x1B';
        public const string MessageFormat = "TYPE:{0}:DATA:{1}\x1B";
        public const string MessagePattern = "TYPE:([^:]*):DATA:([^\x1B]*)\x1B";
    }

    internal static class MessageDecorator
    {
        private static Regex messageRegex = new Regex(Constants.MessagePattern, RegexOptions.Singleline);

        //public static string DecorateMessage(IMessage message)
        //{
        //    string typeName = message.GetType().Name;
        //    return String.Format(Constants.MessageFormat, typeName, message.Serialize());
        //}

        public static string DecorateMessage(IMessage message)
        {
            string typeName = message.GetType().Name;
            return String.Format(Constants.MessageFormat,typeName, message.Serialize());
        }

        public static string GetMessageTypeName(string decoratedMessage)
        {
            return messageRegex.Match(decoratedMessage).Groups[1].Value;
        }

        public static string UndecorateMessage(string decoratedMessage)
        {
            return messageRegex.Match(decoratedMessage).Groups[2].Value;
        }
    }
}
