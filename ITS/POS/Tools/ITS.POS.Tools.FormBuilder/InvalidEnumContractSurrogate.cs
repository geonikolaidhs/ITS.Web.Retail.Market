using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.POS.Tools.FormBuilder
{
    public class InvalidEnumContractSurrogate : IDataContractSurrogate
    {
        private HashSet<Type> typelist;

        /// <summary>
        /// Create new Data Contract Surrogate to handle the specified Enum type
        /// </summary>
        /// <param name="type">Enum Type</param>
        public InvalidEnumContractSurrogate(Type type)
        {
            typelist = new HashSet<Type>();
            if (!type.IsEnum)
            {
                throw new ArgumentException(type.Name + " is not an enum", "type");
            }
            typelist.Add(type);
        }

        /// <summary>
        /// Create new Data Contract Surrogate to handle the specified Enum types
        /// </summary>
        /// <param name="types">IEnumerable of Enum Types</param>
        public InvalidEnumContractSurrogate(IEnumerable<Type> types)
        {
            typelist = new HashSet<Type>();
            foreach (var type in types)
            {
                if (!type.IsEnum)
                {
                    throw new ArgumentException(type.Name + " is not an enum", "type");
                }
                typelist.Add(type);
            }
        }



        public Type GetDataContractType(Type type)
        {
            if (typelist.Contains(type))
            {
                return typeof(int);
            }
            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (typelist.Contains(obj.GetType()))
            {
                return (int)obj;
            }
            return obj;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (typelist.Contains(targetType))
            {
                return Enum.ToObject(targetType, obj);
            }
            return obj;
        }

        public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
        {
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }
    }
}
