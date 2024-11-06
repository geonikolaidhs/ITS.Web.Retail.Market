using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 530, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PaymentMethodField : CustomField, IPaymentMethodField
    {
        public PaymentMethodField()
        {

        }

        public PaymentMethodField(Session session)
            : base(session)
        {

        }

        // Fields...
        [NonPersistent]
        private PaymentMethod _PaymentMethod;
        [Association("PaymentMethod-PaymentMethodFields"), Indexed(Unique = false)]
        public Guid PaymentMethod { get; set; }
        
        

        IPaymentMethod IPaymentMethodField.PaymentMethod
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}