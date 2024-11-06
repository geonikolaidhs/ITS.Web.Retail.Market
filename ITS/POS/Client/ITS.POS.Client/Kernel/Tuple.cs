using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public class Tuple<T,U>
    {
        public T Key1 { get; set; }
        public U Key2 { get; set; }
    }
}
