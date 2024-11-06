using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class MenuInfo
    {
        public MenuNode Menu { get; set;}
        public DateTime CreatedAt { get; set; }

        public MenuInfo()
        {
            CreatedAt = DateTime.Now;
        }
    }
}