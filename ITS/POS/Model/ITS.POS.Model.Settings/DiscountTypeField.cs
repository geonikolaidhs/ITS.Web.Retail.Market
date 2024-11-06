using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Settings
{
    public class DiscountTypeField : CustomField
    {
        public DiscountTypeField()
        {
            
        }
        public DiscountTypeField(Session session)
            : base(session)
        {
            
        }

        // Fields...
        private Guid _DiscountType;
        public Guid DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }

    }
}
