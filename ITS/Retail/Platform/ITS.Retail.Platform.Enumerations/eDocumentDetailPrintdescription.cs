using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDocumentDetailPrintDescription
    {
        //Prints ItemName + ItemExtraInfoName
        CustomDescription,
        //Prints ItemName ignores ItemExtraInfoName
        ItemName,
        //Prints ItemExtraInfoName ignores ItemName
        //If ItemExtraInfoName does not exists Prints ItemName
        ItemExtraInfoName
    }
}
