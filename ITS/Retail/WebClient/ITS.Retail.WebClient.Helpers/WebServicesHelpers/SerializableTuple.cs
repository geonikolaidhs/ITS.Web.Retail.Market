using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace POSCommandsLibrary
{
    [DataContract(Name = "SerializableTupleOf{0}And{1}")]
    public struct SerializableTuple<T, V>
    {
        [DataMember]
        public T Item1 { get; set; }
        [DataMember]
        public V Item2 { get; set; }
    }
}
