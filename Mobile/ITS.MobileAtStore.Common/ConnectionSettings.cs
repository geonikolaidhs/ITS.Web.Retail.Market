using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{
    public class ConnectionSettings : IXmlSubitems
    {
        public ConnectionMode ConnectionMode { get; set; }
        public String Server { get; set; }
        public String DatabaseName { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
    }
}
