using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.RetailAtStoreModel
{
    public class ESLInvLine:InvLine
    {
             
        private string _InventoryNumber;
        public string InventoryNumber
        {
            get
            {
                return _InventoryNumber;
            }
            set
            {
                SetPropertyValue("InventoryNumber", ref _InventoryNumber, value);
            }
        }


        

    }
}