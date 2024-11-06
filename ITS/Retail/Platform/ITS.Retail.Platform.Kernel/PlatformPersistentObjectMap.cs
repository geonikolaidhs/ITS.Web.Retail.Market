using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Provides model implementation type mapping for the common platform entity interfaces.
    /// </summary>
    public class PlatformPersistentObjectMap : IPlatformPersistentObjectMap
    {
        private Dictionary<Type, Type> TypeMapping { get; set; }

        public PlatformPersistentObjectMap()
        {
            TypeMapping = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Returns the concrete type of the Platform entity.
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <returns></returns>
        public Type ResolveType<TBaseType>() where TBaseType : IPersistentObject
        {
            Type baseType = typeof(TBaseType);
            if (TypeMapping.ContainsKey(baseType))
            {
                return TypeMapping[baseType];
            }

            throw new Exception("Unable to resolve type " + baseType.Name);
        }

        /// <summary>
        /// Registers a mapping between a platform entity and a concrete implementation.
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <typeparam name="VConcreteType"></typeparam>
        public void RegisterType<TBaseType, VConcreteType>()
            where TBaseType : IPersistentObject
            where VConcreteType : class,TBaseType
        {
            if (TypeMapping.ContainsKey(typeof(TBaseType)))
            {
                throw new Exception("Type (" + typeof(TBaseType).Name + ") already registered");
            }

            TypeMapping.Add(typeof(TBaseType), typeof(VConcreteType));
        }
    }
}
