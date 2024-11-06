using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public interface IPersistableViewModel
    {
        Guid Oid { get; }
        Type PersistedType { get; }
        bool IsDeleted { get; }


        void UpdateModel(Session uow);

        bool Validate(out string message);
    }
}
