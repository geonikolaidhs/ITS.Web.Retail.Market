using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WRM.Kernel
{
    public class WRMDatabaseConfiguration
    {
        public DBType DataBaseType { get; set; }
        public string Server { get; set; }
        public string DataBase { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
