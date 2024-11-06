using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{

    public interface IPersistentObject
    {
        [System.ComponentModel.DataAnnotations.Key]
        Guid Oid { get; }
        [IgnoreDataMember]
        Session Session { get; }
        void Save();
    }
}
