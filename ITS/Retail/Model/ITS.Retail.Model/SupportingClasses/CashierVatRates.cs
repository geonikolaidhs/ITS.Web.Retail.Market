using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    [DataContract]
    public class CashierVatRates
    {
        [DataMember]
        public Decimal VatRateA { get; set; }
        [DataMember]
        public Decimal VatRateB { get; set; }
        [DataMember]
        public Decimal VatRateC { get; set; }
        [DataMember]
        public Decimal VatRateD { get; set; }
        [DataMember]
        public Decimal VatRateE { get; set; }
    }
}
