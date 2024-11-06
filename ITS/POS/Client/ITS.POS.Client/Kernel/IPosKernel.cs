using ITS.Retail.Platform.Kernel;
using NLog;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Pos specific kernel.
    /// </summary>
    public interface IPosKernel : IKernel
    {
        Logger LogFile { get; set; }
    }
}
