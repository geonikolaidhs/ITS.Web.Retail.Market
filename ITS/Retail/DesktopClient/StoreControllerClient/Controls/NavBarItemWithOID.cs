using DevExpress.XtraNavBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class NavBarItemWithOID : NavBarItem
    {
        public NavBarItemWithOID() : base()
        {

        }
        public NavBarItemWithOID(Guid OID, string Caption) : base(Caption)
        {
            this.OID = OID;
        }
        public Guid? OID { get; set; }
    }
}