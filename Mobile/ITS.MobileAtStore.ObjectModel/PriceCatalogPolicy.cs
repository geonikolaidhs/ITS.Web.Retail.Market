using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.ObjectModel
{
    public class PriceCatalogPolicy
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string ErrorMessage;
        public override string ToString()
        {
            return Description;
        }
    }
}
