using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDocumentPayment : IPersistentObject
    {
        decimal Amount { get; set; }
        string PaymentMethodCode { get; set; }
        string PaymentMethodDescription { get; set; }
        IDocumentHeader DocumentHeader { get; set; }
        IPaymentMethod PaymentMethod { get; set; }
        bool IncreasesDrawerAmount { get; set; }
    }
}
