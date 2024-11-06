using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class CashRegister : Terminal
    {
        public CashRegisterDevice CashRegisterDevice { get; set; }
        public ConnectionType ConnectionType { get; set; }
       
    }
}
