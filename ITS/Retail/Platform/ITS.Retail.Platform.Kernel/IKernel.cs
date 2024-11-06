namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Basic interface for a custom IoC container.
    /// </summary>
    public interface IKernel
    {
        T GetModule<T>() where T : IKernelModule;

        void RegisterModule<TBaseType, VConcreteType>(VConcreteType kernelModule)
            where TBaseType : IKernelModule
            where VConcreteType : TBaseType;
    }
}
