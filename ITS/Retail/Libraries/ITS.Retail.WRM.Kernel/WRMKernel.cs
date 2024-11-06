using System;
using System.Collections.Generic;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.WRM.Kernel.Interface;

namespace ITS.Retail.WRM.Kernel
{
    public class WRMKernel : IWRMKernel
    {
        public WRMKernel()
        {
            KernelComponents = new Dictionary<Type, IKernelModule>();
        }

        private Dictionary<Type, IKernelModule> KernelComponents { get; set; }
        public T GetModule<T>() where T : IKernelModule
        {
            Type requestedManagerInterfaceType = typeof(T);
            if (KernelComponents.ContainsKey(requestedManagerInterfaceType))
            {
                return (T)KernelComponents[requestedManagerInterfaceType];
            }

            throw new Exception("KernelComponent of type '" + typeof(T).Name + "' was not registered");
        }

        public void RegisterModule<TBaseType, VConcreteType>(VConcreteType kernelModule)
            where TBaseType : IKernelModule
            where VConcreteType : TBaseType
        {
            Type managerInterfaceType = typeof(TBaseType);
            if (KernelComponents.ContainsKey(managerInterfaceType))
            {
                throw new Exception("KernelComponent of type '" + typeof(TBaseType).Name + "' is already registered");
            }

            KernelComponents[managerInterfaceType] = kernelModule;
        }
    }
}
