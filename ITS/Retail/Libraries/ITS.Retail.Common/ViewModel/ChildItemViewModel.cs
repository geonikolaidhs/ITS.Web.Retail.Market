using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class ChildItemViewModel: BasePersistableViewModel
    {
        public Guid Item
        {
            get
            {
                return this._Item;
            }
            set
            {
                SetPropertyValue("Item", ref this._Item, value);
            }
        }

        public override Type PersistedType
        {
            get
            {
                return null;
            }
        }

        private Guid _Item;
    }
}
