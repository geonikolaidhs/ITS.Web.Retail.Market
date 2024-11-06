using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    /// <summary>
    /// Το συγκεκριμένο Attribute αφορά τις διαδικασίες του update
    /// To Order μας δείχνει την σειρά με την οποία γίνεται το update
    /// Το Permissions μας δείχνει ποιος μπορεί να στείελι και πού.
    /// </summary>
    public class UpdaterAttribute : Attribute
    {
        public int Order;
        public eUpdateDirection Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS;
    }

    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class UpdaterIgnoreFieldAttribute : Attribute
    {
        public eUpdateDirection IgnoreWhenDirection { get; set; }

        public UpdaterIgnoreFieldAttribute()
        {
            IgnoreWhenDirection = eUpdateDirection.MASTER_TO_STORECONTROLLER | 
                                  eUpdateDirection.POS_TO_STORECONTROLLER | 
                                  eUpdateDirection.STORECONTROLLER_TO_MASTER | 
                                  eUpdateDirection.STORECONTROLLER_TO_POS;
        }

        public UpdaterIgnoreFieldAttribute(eUpdateDirection ignoreWhenDirection)
        {
            IgnoreWhenDirection = ignoreWhenDirection;
        }
    }
}
