using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class ReportBarButton : BarButtonItem
    {
        public ReportBarButton() : base()
        {

        }
        public CustomReport CustomReport { get; set; }
    }
}