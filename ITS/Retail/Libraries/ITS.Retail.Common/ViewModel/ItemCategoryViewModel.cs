using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class ItemCategoryViewModel : BasePersistableViewModel
    {
        public override Type PersistedType { get { return typeof(ItemCategory); } }


    }
}
