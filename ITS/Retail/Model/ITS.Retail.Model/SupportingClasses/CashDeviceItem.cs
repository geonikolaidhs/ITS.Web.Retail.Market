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
    public class CashDeviceItem
    {
        [DataMember]
        public int deviceIndex { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal price { get; set; }
        [DataMember]
        public int VatCode { get; set; }
        [DataMember]
        public decimal Qty { get; set; }
        [DataMember]
        public bool IsAvaledToSale { get; set; }
        [DataMember]
        public bool IsChecked { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public DateTime UpdatedOn { get; set; }
        [DataMember]
        public Decimal VatPercent { get; set; }
    }
}
