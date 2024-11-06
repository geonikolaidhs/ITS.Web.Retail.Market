using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Kernel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Custom IoC container for the POS application. Uses singleton scope.
    /// </summary>
    public class PosKernel : IPosKernel
    {
        public PosKernel()
        {
            KernelComponents = new Dictionary<Type, IKernelModule>();
        }

        private Dictionary<Type, IKernelModule> KernelComponents { get; set; }

        public void RegisterModule<TBaseType, VConcreteType>(VConcreteType kernelComponent)
            where TBaseType : IKernelModule
            where VConcreteType : TBaseType
        {
            Type managerInterfaceType = typeof(TBaseType);
            if (KernelComponents.ContainsKey(managerInterfaceType))
            {
                throw new POSException("KernelComponent of type '" + typeof(TBaseType).Name + "' is already registered");
            }

            KernelComponents[managerInterfaceType] = kernelComponent;
        }

        public T GetModule<T>() where T : IKernelModule
        {
            Type requestedManagerInterfaceType = typeof(T);
            if (KernelComponents.ContainsKey(requestedManagerInterfaceType))
            {
                return (T)KernelComponents[requestedManagerInterfaceType];
            }

            throw new POSException("KernelComponent of type '" + typeof(T).Name + "' was not registered");
        }


        public Logger LogFile { get; set; }
    }
}
