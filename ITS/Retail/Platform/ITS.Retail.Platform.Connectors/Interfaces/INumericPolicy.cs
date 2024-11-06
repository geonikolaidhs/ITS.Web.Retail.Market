using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.Interfaces
{
    interface INumericPolicy<T>
    {
        T Zero();
        T Add(T a, T b);
        // add more functions here, such as multiplication etc.
    }
}
