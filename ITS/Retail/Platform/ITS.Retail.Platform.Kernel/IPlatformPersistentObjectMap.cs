using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public interface IPlatformPersistentObjectMap : IKernelModule
    {
        void RegisterType<TBaseType, VConcreteType>()
            where TBaseType : IPersistentObject
            where VConcreteType : class,TBaseType;
        Type ResolveType<TBaseType>() where TBaseType : IPersistentObject;
    }
}
