using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POSCommandsLibrary
{

    public struct POSCommandSet
    {
        public List<SerializableTuple<ePosCommand, string>> Commands { get; set; }
        public long Expire { get; set; }
    }
}
