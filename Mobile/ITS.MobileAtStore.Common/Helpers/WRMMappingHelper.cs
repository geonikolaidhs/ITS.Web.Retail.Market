
using System;
using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore.Common.Helpers
{
    public static class WRMMappingHelper
    {
        public static eDivision GetDivisionForDocumentType(DOC_TYPES documentType)
        {
            switch(documentType)
            {
                //case DOC_TYPES.ALL_TYPES: break; //INVALID
                case DOC_TYPES.ORDER:
                //case DOC_TYPES.NULL2: break; //INVALID
                case DOC_TYPES.INVOICE:
                    return eDivision.Purchase;
                case DOC_TYPES.INVOICE_SALES:
                    return eDivision.Sales;
                //case DOC_TYPES.NULL4: break; //INVALID
                case DOC_TYPES.RECEPTION:
                    return eDivision.Sales;
                case DOC_TYPES.INVENTORY:                    
                //case DOC_TYPES.MATCHING : break; //UNKNOWN
                case DOC_TYPES.PICKING:
                    return eDivision.Store;
                case DOC_TYPES.TRANSFER:
                    return eDivision.Sales;
                case DOC_TYPES.TAG: //UNKOWN
                    return eDivision.Sales;
                //case DOC_TYPES.PRICE_CHECK : break; //UNKOWN
                //case DOC_TYPES.COMPETITION: break;  //UNKOWN
                default:
                    throw new Exception("Do now know how to map Type: "+documentType.ToString());
            }
        }
    }
}
