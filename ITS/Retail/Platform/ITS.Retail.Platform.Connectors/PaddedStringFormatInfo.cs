using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform
{
    public sealed class PaddedStringFormatInfo : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (typeof(ICustomFormatter).Equals(formatType))
            {
                return this;
            }
            return null;
        }

        

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("Argument cannot be null");
            }

            string[] args;
            if (format != null)
            {
                args = format.Split(':');
            }
            else
            {
                return arg.ToString();
            }

            if (args.Length == 1)
            {
                String.Format("{0, " + format + "}", arg);
            }


            switch (args.Length)
            {
                case 2://Padded format
                    if (args[1].Length > 1 && args[1][0] == 'x')
                    {
                        try
                        {
                            string multiplierStr = args[1].Substring(1);
                            int multiplier = int.Parse(multiplierStr);
                            return String.Format("{0:" + args[0] + "}", decimal.Parse(arg.ToString()) * multiplier);
                        }
                        catch (Exception exception)
                        {
                            throw new ArgumentException("Multiplier should be an integer", exception);
                        }
                    }
                    else
                    {
                        int padLength = 0;
                        try
                        {
                            padLength = int.Parse(args[0]);
                        }
                        catch (Exception exception)
                        {
                            throw new ArgumentException("Padding length should be an integer", exception);
                        }
                        if (padLength > 0)
                        {
                            return (arg as string).PadLeft(padLength, args[1][0]);
                        }
                        return (arg as string).PadRight(padLength * -1, args[1][0]);
                    }
                default://Use default string.format
                    if(arg is string && string.IsNullOrWhiteSpace(format)==false)
                    {
                        int len;
                        if(int.TryParse(format, out len))
                        {
                            len = Math.Abs(len);
                            if (len < ((string)arg).Length)
                            {
                                return string.Format("{0," + format + "}", ((string)arg).Substring(0, len));
                            }
                        }
                        //else
                        //{
                        //    int r = 0;
                        //}
                    }
                    return string.Format("{0," + format + "}", arg);
            }


        }
    }
}
