using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore.AuxilliaryClasses
{
    public static class EnumerationMapping
    {
        public static ITS.MobileAtStore.WRMMobileAtStore.eDocumentType GetWebeDocumentType(DOC_TYPES docType)
        {
            switch (docType)
            {
                case DOC_TYPES.ALL_TYPES:
                    throw new NotImplementedException("GetWebeDocumentType " + docType.ToString());
                case DOC_TYPES.COMPETITION:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.COMPETITION;
                case DOC_TYPES.ESL_INV:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.ESL_INV;
                case DOC_TYPES.INVENTORY:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.INVENTORY;
                case DOC_TYPES.INVOICE:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.INVOICE;
                case DOC_TYPES.INVOICE_SALES:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.INVOICE_SALES;
                case DOC_TYPES.MATCHING:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.MATCHING;
                case DOC_TYPES.ORDER:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.ORDER;
                case DOC_TYPES.PICKING:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.PICKING;
                case DOC_TYPES.PRICE_CHECK:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.PRICE_CHECK;
                case DOC_TYPES.RECEPTION:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.RECEPTION;
                case DOC_TYPES.TAG:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.TAG;
                case DOC_TYPES.TRANSFER:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.TRANSFER;
                case DOC_TYPES.DECOMPOSITION:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.DECOMPOSITION;
                case DOC_TYPES.COMPOSITION:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.COMPOSITION;
                case DOC_TYPES.QUEUE_QR:
                    return ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.QUEUE_QR;
                default:
                    throw new NotImplementedException("GetWebeDocumentType " + docType.ToString());
            }
        }
    }
}
