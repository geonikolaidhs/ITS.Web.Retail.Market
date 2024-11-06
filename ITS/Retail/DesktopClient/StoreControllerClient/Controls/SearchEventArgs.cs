using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class SearchEventArgs: EventArgs
    {
        public UnitOfWork UnitOfWork { get; set; }
    }
}
