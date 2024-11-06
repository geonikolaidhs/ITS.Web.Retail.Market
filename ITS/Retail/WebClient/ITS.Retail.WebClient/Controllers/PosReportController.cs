//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using System.Drawing.Imaging;
using ITS.Retail.ResourcesLib;
using Ionic.Zip;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using System.Web.Routing;
using System.Xml;
using System.Text;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class PosReportController : BaseObjController<PosReport>
    {

        UnitOfWork uow;
        string sql;

        protected void GenerateUnitOfWork()
        {

            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            //List<Item> items = new List<Item>();
            //using (UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
            //{
            //    items = GetList<Item>(uow2).ToList<Item>();
            //}
            //foreach (Item i in items)
            //{
            //    var a = i.DefaultBarcode.Oid;
            //}


            //XPCollection<Item> items = new XPCollection<Item>();
            //using (UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
            //{
            //    items = GetList<Item>(uow2);

            //    foreach (Item i in items)
            //    {
            //        var a = i.DefaultBarcode.Oid;
            //    }
            //}


            GenerateUnitOfWork();
            User currentUser = CurrentUser;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            return View("Index", GetList<PosReport>(uow).AsEnumerable());
        }


        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            return PartialView("Grid", GetList<PosReport>(uow).AsEnumerable());
        }

        [ValidateInput(false)]
        public ActionResult Save()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                Guid editOid = Guid.Empty;
                if (!String.IsNullOrEmpty(Request["Oid"]))
                {
                    if (Guid.TryParse(Request["Oid"], out editOid))
                    {
                        PosReport posReport = uow.GetObjectByKey<PosReport>(editOid);
                        if (posReport == null)
                        {
                            posReport = new PosReport(uow);
                        }
                        String startFormat = String.Empty;
                        //String replacedFormat1 = String.Empty;
                        //String replacedFormat2 = String.Empty;
                        startFormat = Request["Format"];
                        //replacedFormat1 = replacedFormat1.Replace("***", ">");
                        //replacedFormat2 = startFormat.Replace("**", "<");


                        posReport.Format = ReverseCleanJavaScriptString(startFormat);

                        posReport.Description = Request["Description"];
                        posReport.Code = Request["Code"];
                        posReport.Save();
                        uow.CommitTransaction();
                        uow.Dispose();
                        Session["uow"] = null;
                    }
                }
                return RedirectToAction("Index");
            }
        }


        public ActionResult Delete(String id)
        {
            GenerateUnitOfWork();
            Guid editOid = Guid.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                if (Guid.TryParse(id, out editOid))
                {
                    PosReport posReport = uow.FindObject<PosReport>(CriteriaOperator.And(new BinaryOperator("Oid", editOid)));
                    if (posReport != null)
                    {
                        posReport.Delete();
                        uow.CommitTransaction();
                        uow.Dispose();
                        Session["uow"] = null;
                    }
                }
            }
            return RedirectToAction("Index");
        }


        public ActionResult Edit(String id)
        {
            GenerateUnitOfWork();
            this.ToolbarOptions.CopyButton.Visible = false;
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.ToolbarOptions.ClearAllItems.Visible = false;
            this.ToolbarOptions.EditButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.NewButton.Visible = false;
            this.ToolbarOptions.ForceVisible = false;

            Guid editOid = Guid.Empty;
            Guid.TryParse(id, out editOid);
            ViewBag.HeaderTitle = Resources.Pos_Report_Edit;
            PosReport posReport = uow.FindObject<PosReport>(CriteriaOperator.And(new BinaryOperator("Oid", editOid)));
            if (posReport != null)
            {
                String posreReportFormat = posReport.Format;
                //int a = posReport.Format.LastIndexOf("XpoHelper.sqlserver");
                //string con = posReport.Format.Substring()
                posReport.Format = CleanJavaScriptString(posreReportFormat);
            }
            else
            {
                posReport = new PosReport(uow);
                ViewBag.HeaderTitle = Resources.Pos_Report_New;
                String sqlserver = String.Empty;
                String username = String.Empty;
                String pass = String.Empty;
                String database = String.Empty;
                String dbtype = String.Empty;
                PrepareXpoHelper(out sqlserver, out username, out pass, out database, out dbtype);
                posReport.Format = CleanJavaScriptString(CreateDefaultClass(sqlserver, username, pass, database, dbtype));

            }
            return PartialView("Edit", posReport);
        }


        private String CreateDefaultClass(String sqlserver, String username, String pass, String database, String dbtype)
        {
            ;
            StringBuilder sb = new StringBuilder();
            sb.Append("using System.IO;" + System.Environment.NewLine);
            sb.Append("using System.Collections;" + System.Environment.NewLine);
            sb.Append("using System.Linq;" + System.Environment.NewLine);
            sb.Append("using System.Collections.Generic;" + System.Environment.NewLine);
            sb.Append("using System.Data;" + System.Environment.NewLine);
            sb.Append("using System.Data.Common;" + System.Environment.NewLine);
            sb.Append("using System.Data.SqlClient;" + System.Environment.NewLine);
            sb.Append("using ITS.POS.Model.Transactions;" + System.Environment.NewLine);
            sb.Append("using ITS.POS.Model.Master;" + System.Environment.NewLine);
            sb.Append("using ITS.POS.Model.Versions;" + System.Environment.NewLine);
            sb.Append("using ITS.POS.Model.Settings;" + System.Environment.NewLine);
            sb.Append("using ITS.POS.Client.Kernel;" + System.Environment.NewLine);
            sb.Append("using ITS.Retail.Platform.Kernel;" + System.Environment.NewLine);
            sb.Append("using ITS.Retail.Platform.Enumerations.ViewModel;" + System.Environment.NewLine);
            sb.Append("using ITS.Retail.Platform.Enumerations;" + System.Environment.NewLine);
            sb.Append("using DevExpress.Xpo;" + System.Environment.NewLine);
            sb.Append("using DevExpress.Data.Filtering;" + System.Environment.NewLine);
            sb.Append("using DevExpress.Data;" + System.Environment.NewLine);
            sb.Append("using ITS.Retail.Common;" + System.Environment.NewLine);
            sb.Append("using NLog;" + System.Environment.NewLine);
            sb.Append(System.Environment.NewLine);
            sb.Append("namespace ITS.POS.Client" + System.Environment.NewLine);
            sb.Append("{" + System.Environment.NewLine);
            sb.Append("     public class " + "PosOposReport" + " " + System.Environment.NewLine);
            sb.Append("     {");
            sb.Append(System.Environment.NewLine);
            sb.Append("         private ISessionManager sessionManager { get; set; }" + Environment.NewLine);
            sb.Append("         private IConfigurationManager config { get; set; }" + Environment.NewLine);
            sb.Append("         private IPosKernel kernel { get; set; }" + Environment.NewLine);
            sb.Append(System.Environment.NewLine);
            sb.Append("         public PosOposReport(ISessionManager sessionManager, IConfigurationManager config, IPosKernel kernel)" + Environment.NewLine);
            sb.Append("         {" + Environment.NewLine);
            sb.Append("             this.sessionManager = sessionManager;");
            sb.Append(Environment.NewLine);
            sb.Append("             this.config = config;");
            sb.Append(Environment.NewLine);
            sb.Append("             this.kernel = kernel;" + Environment.NewLine);
            sb.Append("             XpoHelper.sqlserver = @\"" + sqlserver + "\"" + "; " + Environment.NewLine);
            sb.Append("             XpoHelper.username =  \"" + username + "\"" + "; " + Environment.NewLine);
            sb.Append("             XpoHelper.pass = \"" + pass + "\"" + "; " + Environment.NewLine);
            sb.Append("             XpoHelper.database  = \"" + database + "\"" + "; " + Environment.NewLine);
            sb.Append("             try " + Environment.NewLine);
            sb.Append("             { " + Environment.NewLine);
            sb.Append("              DBType dt = DBType.SQLServer; " + Environment.NewLine);
            sb.Append("              System.Enum.TryParse<DBType>(" + "\"" + dbtype + "\"" + ", out dt); " + Environment.NewLine);
            sb.Append("              XpoHelper.databasetype = dt;" + Environment.NewLine);
            sb.Append("             } " + Environment.NewLine);
            sb.Append("             catch " + Environment.NewLine);
            sb.Append("             { " + Environment.NewLine);
            sb.Append("                 XpoHelper.databasetype = DBType.SQLServer; " + Environment.NewLine);
            sb.Append("             } " + Environment.NewLine);
            sb.Append("         }" + Environment.NewLine);
            sb.Append("         private void AlwaysExecuted(){}" + Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("     }" + System.Environment.NewLine);
            sb.Append("}" + System.Environment.NewLine);
            return sb.ToString();
        }

        private void AlwaysExecuted() { }

        private void PrepareXpoHelper(out String sqlserver, out String username, out String pass, out String database, out String dbtype)
        {
            sqlserver = String.Empty;
            username = String.Empty;
            pass = String.Empty;
            database = String.Empty;
            dbtype = String.Empty;

            string configurationXMLFile = Server.MapPath("~/Configuration/config.xml");
            if (System.IO.File.Exists(configurationXMLFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configurationXMLFile);

                XmlNode settingsNode = doc.SelectSingleNode("settings");
                XmlNode xmlNode = settingsNode.SelectSingleNode("sqlserver");
                if (xmlNode != null)
                {
                    string server = @xmlNode.InnerText.Trim();
                    sqlserver = server.Replace("\\", "\\\\");
                    Session["sql"] = sqlserver;
                }

                xmlNode = settingsNode.SelectSingleNode("username");
                if (xmlNode != null)
                {
                    username = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("pass");
                if (xmlNode != null)
                {
                    pass = xmlNode.InnerText.Trim();
                }
                xmlNode = settingsNode.SelectSingleNode("database");
                if (xmlNode != null)
                {
                    database = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("dbtype");
                if (xmlNode != null)
                {
                    dbtype = xmlNode.InnerText.Trim();
                }
            }
        }


        private string CleanJavaScriptString(string stringToClean)
        {
            //string cleanString = stringToClean.Replace("'", "\'");
            string cleanString = stringToClean.Replace(Environment.NewLine, "\\n");
            return cleanString;
        }


        private string ReverseCleanJavaScriptString(string stringToClean)
        {
            //string cleanString = stringToClean.Replace("\'", "'");
            string cleanString = stringToClean.Replace("\\n", Environment.NewLine);
            return cleanString;
        }



    }
}
//#endif