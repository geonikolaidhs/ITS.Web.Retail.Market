using ITS.Retail.Platform.Enumerations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.Platform.Enumerations
{

    public class EntityDisplayNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type ResourceType { get; private set; }
        private DisplayAttribute display;

        public EntityDisplayNameAttribute(string name, Type resourceType)
        {
            this.Name = name;
            this.ResourceType = resourceType;
            display = new DisplayAttribute() { Name = name, ResourceType = resourceType };

        }

        public string GetName()
        {
            return display.GetName();
        }

        public string GetDescription()
        {
            return display.GetDescription();
        }
    }

    public static class ExtensionMethods
    {
        public static Dictionary<T, string> ToDictionary<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            T obj = Activator.CreateInstance<T>();

            //IEnumerable<String> names = Enum.GetNames(typeof(T));
            IEnumerable<T> values = Enum.GetValues(typeof(T)).Cast<T>();
            return values.ToDictionary(x => x, x => Enum<T>.ToLocalizedString(x));
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }

        public static string ToLocalizedString(this Type type)
        {
            EntityDisplayNameAttribute attr = type.GetCustomAttributes(typeof(EntityDisplayNameAttribute), false).FirstOrDefault() as EntityDisplayNameAttribute;
            if (attr != null && !String.IsNullOrWhiteSpace(attr.Name))
            {
                Type resourceType = attr.ResourceType;
                PropertyInfo resource = resourceType.GetProperty(attr.Name,
                                   BindingFlags.Static | BindingFlags.Public);
                if (resource != null)
                {
                    return resource.GetValue(null, null) as String;
                }
                else
                {
                    return attr.Name;
                }
            }

            return type.Name;
        }

        public static string ToLocalizedString(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (string.IsNullOrEmpty(name) == false)//if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DisplayAttribute attr = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
                    if (attr != null && !String.IsNullOrWhiteSpace(attr.Name))
                    {
                        Type resourceType = attr.ResourceType;
                        PropertyInfo resource = resourceType.GetProperty(attr.Name, BindingFlags.Static | BindingFlags.Public);
                        if (resource != null)
                        {
                            return resource.GetValue(null, null) as String;
                        }
                        else
                        {
                            return attr.Name;
                        }
                    }
                }
            }
            return value.ToString();
        }

        public static Dictionary<string, Type> GetActionKeybindParameters(this eActions value)
        {
            FieldInfo field = typeof(eActions).GetField(value.ToString());
            if (field != null)
            {
                ActionKeybindParameterAttribute[] attributes =
                       Attribute.GetCustomAttributes(field,
                         typeof(ActionKeybindParameterAttribute)) as ActionKeybindParameterAttribute[];
                if (attributes != null)
                {
                    Dictionary<string, Type> parameterTypeDictionary = new Dictionary<string, Type>();

                    foreach (ActionKeybindParameterAttribute attribute in attributes)
                    {
                        parameterTypeDictionary.Add(attribute.Parameter, attribute.ParameterType);
                    }
                    return parameterTypeDictionary;
                }
            }
            return new Dictionary<string, Type>();
        }

        public static NLog.LogLevel GetNLogLogLevel(this KernelLogLevel logLevel)
        {
            switch (logLevel)
            {
                case KernelLogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case KernelLogLevel.Error:
                    return NLog.LogLevel.Error;
                case KernelLogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case KernelLogLevel.Info:
                    return NLog.LogLevel.Info;
                case KernelLogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case KernelLogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case KernelLogLevel.Off:
                    return NLog.LogLevel.Off;
                default:
                    return NLog.LogLevel.Debug;
            }
        }

        public static eDocumentTraderType GetAvailableTraderTypes(this eDivision division)
        {
            FieldInfo field = typeof(eDivision).GetField(division.ToString());
            if (field != null)
            {
                AvailabeTraderTypeAttribute[] attributes =
                       Attribute.GetCustomAttributes(field,
                         typeof(AvailabeTraderTypeAttribute)) as AvailabeTraderTypeAttribute[];
                if (attributes != null && attributes.Length == 1)
                {
                    return attributes[0].AvailableTraderTypes;
                }                            
            }
            return eDocumentTraderType.NONE;
        }

        
        public static IEnumerable<T> ToEnumValuesEnumerable<T>(this T mask) where T: struct, IComparable, IFormattable, IConvertible
        {
            if (mask is Enum == false)
            {
                throw new ArgumentException();
            }
            Enum maskEnum = mask as Enum;
            return Enum.GetValues(typeof(T))
                                 .Cast<Enum>()
                                 .Where(val => maskEnum.HasFlag(val) && Convert.ToInt32(val) != 0)
                                 .Cast<T>();
        }

        
    }
}
