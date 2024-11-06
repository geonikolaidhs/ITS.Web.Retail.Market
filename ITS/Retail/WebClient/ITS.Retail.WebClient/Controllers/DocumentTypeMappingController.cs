using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class DocumentTypeMappingController : BaseObjController<DocumentTypeMapping>
    {

        private static readonly Dictionary<PropertyInfo, String> propMapping = new Dictionary<PropertyInfo, string>() 
        {
            { typeof(DocumentTypeMapping).GetProperty("DocumentType"), "DocType_VI" }
        };

        protected override Dictionary<PropertyInfo, String> PropertyMapping
        {
            get
            {
                return propMapping;
            }
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {


                this.ToolbarOptions.ExportToButton.Visible = false;
                this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
                this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
                this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";

                this.CustomJSProperties.AddJSProperty("gridName", "grdDocumentTypeMapping");

                return View(GetList<DocumentTypeMapping>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<DocumentTypeMapping>());
            
        }

        public override ActionResult Grid()
        {
            ViewBag.DocumentTypes = GetList<DocumentType>(XpoHelper.GetNewUnitOfWork());
            return base.Grid();
        }

    }
}
