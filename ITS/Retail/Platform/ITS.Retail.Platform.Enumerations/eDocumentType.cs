using System.ComponentModel.DataAnnotations;
using ITS.Retail.ResourcesLib;
using System.Runtime.Serialization;

namespace ITS.Retail.Mobile.AuxilliaryClasses
{
    [DataContract]
    public enum eDocumentType
    {

        //value from warehouse
        [Display(ResourceType = typeof(Resources), Name = "MobNone")]
        [EnumMember]
        NONE = 0,//ALL_TYPES = 0,
        [Display(ResourceType = typeof(Resources), Name = "MobOrder")]
        [EnumMember]
        ORDER = 1,
        //Do not remove: Required for wcf service (otherwise
        [EnumMember]
        EMPTY_2 = 2,
        [Display(ResourceType = typeof(Resources), Name = "MobInvoice")]
        [EnumMember]
        INVOICE = 3,//3,
        //Do not remove
        [EnumMember]
        EMPTY_4 = 4,
        [Display(ResourceType = typeof(Resources), Name = "MobReception")]
        [EnumMember]
        RECEPTION = 5,//3,//5,
        [Display(ResourceType = typeof(Resources), Name = "MobInventory")]
        [EnumMember]
        INVENTORY = 6,//4,//6,
        [Display(ResourceType = typeof(Resources), Name = "MobMatching")]
        [EnumMember]
        MATCHING = 7,//5,//7,
        [Display(ResourceType = typeof(Resources), Name = "MobPicking")]
        [EnumMember]
        PICKING = 8,//6,//8,
        [Display(ResourceType = typeof(Resources), Name = "MobTransfer")]
        [EnumMember]
        TRANSFER = 9,//7,//9,
        [Display(ResourceType = typeof(Resources), Name = "MobTag")]
        [EnumMember]
        TAG = 10,//8,//10,
        [Display(ResourceType = typeof(Resources), Name = "MobPriceCheck")]
        [EnumMember]
        PRICE_CHECK = 11,//9,//11,
        [Display(ResourceType = typeof(Resources), Name = "MobCompetition")]
        [EnumMember]
        COMPETITION = 12,//10,//12,
        [Display(ResourceType = typeof(Resources), Name = "MobPacking")]
        [EnumMember]
        PACKING = 13,//11,//13,
        [Display(ResourceType = typeof(Resources), Name = "MobPackingOrPicking")]
        [EnumMember]
        PACKINGORPICKING = 14,//12,//14
        [Display(ResourceType = typeof(Resources), Name = "MobEslInventory")]
        [EnumMember]
        ESL_INV = 15,//13//ESL_INV = 13
        [Display(ResourceType = typeof(Resources), Name = "MobInvoiceSales")]
        [EnumMember]
        INVOICE_SALES = 16, //Added at 07 01 2014 after a request

        [Display(ResourceType = typeof(Resources), Name = "MobDecomposition")]
        [EnumMember]
        DECOMPOSITION = 17, //Added at 10 01 2017 after a request

        [Display(ResourceType = typeof(Resources), Name = "MobComposition")]
        [EnumMember]
        COMPOSITION = 18, //Added at 10 02 2017 after a request

        [EnumMember]
        QUEUE_QR = 19 //Added at 10 02 2017 after a request
        //Copy from Datalogger
        // ALL_TYPES = 0,
        //ORDER = 1,
        //NULL2 = 2,
        //INVOICE = 3,
        //NULL4 = 4,
        //RECEPTION = 5,
        //INVENTORY = 6,
        //MATCHING = 7,
        //PICKING = 8,
        //TRANSFER = 9,
        //TAG = 10,
        //PRICE_CHECK = 11,
        //COMPETITION = 12,
        ///// <summary>
        ///// Not used in documents
        ///// </summary>
        //ESL_INV = 13

        //Copy from Warehouse
        //ALL_TYPES = 0,
        //ORDER = 1,
        //INVOICE = 3,
        //RECEPTION = 5,
        //INVENTORY = 6,
        //MATCHING = 7,
        //PICKING = 8,
        //TRANSFER = 9,
        //TAG = 10,
        //PRICE_CHECK = 11,
        //COMPETITION = 12,
        //PACKING = 13,
        //PACKINGORPICKING = 14
    }
}
