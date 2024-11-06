using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class ReflectionModelDescription
    {
        private Dictionary<eUpdateDirection, List<PropertyInfo>> PropertiesPerDirection = new Dictionary<eUpdateDirection, List<PropertyInfo>>();
        private Dictionary<eUpdateDirection, List<PropertyInfo>> RemovedProperitesPerDirection = new Dictionary<eUpdateDirection, List<PropertyInfo>>();

        public ReflectionModelDescription(Type type)
        {
            this.Type = type;

        }

        public Type Type { get; set; }

        public List<PropertyInfo> GetProperties(eUpdateDirection direction)
        {
            if (PropertiesPerDirection.ContainsKey(direction) == false)
            {
                List<PropertyInfo> properties = this.Type.GetProperties().ToList();
                List<PropertyInfo> removedProperties = new List<PropertyInfo>();

                Dictionary<PropertyInfo, NonPersistentAttribute> propertiesNonPersistent = new Dictionary<PropertyInfo, NonPersistentAttribute>();
                Dictionary<PropertyInfo, UpdaterIgnoreFieldAttribute> propertiesUpdaterIgnoreFieldAttribute = new Dictionary<PropertyInfo, UpdaterIgnoreFieldAttribute>();
                Dictionary<PropertyInfo, MemberDesignTimeVisibilityAttribute> propertiesMemberDesignTimeVisibilityAttribute = new Dictionary<PropertyInfo, MemberDesignTimeVisibilityAttribute>();
                List<PropertyInfo> extraPropertiesToRemove = new List<PropertyInfo>();

                foreach (PropertyInfo prop in properties)
                {
                    IEnumerable<UpdaterIgnoreFieldAttribute> ignoreFields = prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).Cast<UpdaterIgnoreFieldAttribute>();
                    if (ignoreFields.Count() > 0)
                    {
                        propertiesUpdaterIgnoreFieldAttribute.Add(prop, ignoreFields.First());
                    }

                    IEnumerable<MemberDesignTimeVisibilityAttribute> memberFields = prop.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Cast<MemberDesignTimeVisibilityAttribute>();
                    if (memberFields.Count() > 0)
                    {
                        propertiesMemberDesignTimeVisibilityAttribute.Add(prop, memberFields.First());
                    }
                    string propName = prop.Name;
                    if (propName == "Session" || propName == "ClassInfo" || propName == "IsLoading" || !prop.CanWrite || typeof(XPBaseCollection).IsAssignableFrom(prop.PropertyType))
                    {
                        extraPropertiesToRemove.Add(prop);
                    }
                }
                IEnumerable<PropertyInfo> propsToRemove = extraPropertiesToRemove
                                                                    .Union(propertiesNonPersistent.Select(x => x.Key))
                                                                    .Union(propertiesUpdaterIgnoreFieldAttribute.Where(x => x.Value.IgnoreWhenDirection.HasFlag(direction)).Select(x => x.Key))
                                                                    .Union(propertiesMemberDesignTimeVisibilityAttribute.Select(x => x.Key))
                                                                    .Distinct();
                removedProperties = propsToRemove.ToList();
                properties.RemoveAll(x => removedProperties.Contains(x));
                PropertiesPerDirection[direction] = properties;
                return properties;
            }
            else
            {
                return PropertiesPerDirection[direction];
            }
        }

        //public List<PropertyInfo> GetRemovedProperites(eUpdateDirection direction)
        //{

        //}
    }
}
