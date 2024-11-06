using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class PaymentMethodField : CustomField
    {
        public PaymentMethodField()
        {

        }

        public PaymentMethodField(Session session)
            : base(session)
        {

        }

        // Fields...
        private Guid _PaymentMethod;
        public Guid PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }

    }
}
