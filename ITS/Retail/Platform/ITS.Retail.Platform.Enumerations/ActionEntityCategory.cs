using ITS.Retail.Platform.Enumerations.Attributes;
using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ActionEntityCategory
    {
        [Display(Name = "Document", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader")]
        Document,
        [Display(Name = "Item", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader","DocumentDetails.Item")]
        DocumentItem,
        [Display(Name = "ItemStock", ResourceType = typeof(Resources)), ActionTypeInfo("DocumentHeader", "DocumentDetails.Item.ItemStocks", "", "Store")]
        DocumentItemStock,
        [Display(Name = "Customer", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader","Customer")]
        DocumentCustomer,
        [Display(Name = "Supplier", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader", "Supplier")]
        DocumentSupplier,
        [Display(Name = "DocumentDetails", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader", "DocumentDetails", "IsCanceled = false")]
        DocumentDetails,
        [Display(Name = "PaymentMethod", ResourceType = typeof(Resources)),ActionTypeInfo("DocumentHeader", "DocumentPayments")]
        DocumentPaymentMethod
    }
}
