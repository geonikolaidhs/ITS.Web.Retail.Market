using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.AuxilliaryClasses
{
    public class ObjectSignature
    {
        public Guid Oid { get; set; }
        public string Hash { get; set; }
        public long UpdatedOnTicks { get; set; }

        public ObjectSignature(Guid oid, string hash, long updatedOnTicks)
        {
            Oid = oid;
            Hash = hash;
            UpdatedOnTicks = updatedOnTicks;
        }
    }
}
