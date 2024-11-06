using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IRequiredStore
    {
        IStore Store { get; }
    }

    public interface IHasStores
    {
        IEnumerable<IStore> Stores { get; }
    }
}
