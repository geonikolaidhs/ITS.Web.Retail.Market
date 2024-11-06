using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace ITS.Licensing.Service
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String conn = DevExpress.Xpo.DB.MSSqlConnectionProvider.GetConnectionString("localhost", "sa", "123456", "Licenses");
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(conn, AutoCreateOption.DatabaseAndSchema);
            XpoDefault.Session = new Session(XpoDefault.DataLayer);

            XpoDataSource1.Session = XpoDefault.Session;
        }
    }
}
