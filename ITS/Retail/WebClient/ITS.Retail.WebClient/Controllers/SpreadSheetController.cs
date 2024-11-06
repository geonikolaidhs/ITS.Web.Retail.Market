using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Web.Mvc;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Spreadsheet;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class SpreadSheetController : BaseObjController<SpreadSheet>
    {
        /// <summary>
        /// The uow
        /// </summary>
        UnitOfWork uow;

        /// <summary>
        /// Generates the unit of work.
        /// </summary>
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

        // GET: /SpreadSheet/        
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [Security(ReturnsPartial = false)]
        public ActionResult Index() 
        {
            this.CustomJSProperties.AddJSProperty("gridName", "grdSpreadSheet");
            this.ToolbarOptions.ForceVisible = false;
            object getListAsEnumerable = GetList<SpreadSheet>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<SpreadSheet>();
            if (this.CurrentUser.Role.Type != Platform.Enumerations.eRoleType.CompanyAdministrator && this.CurrentUser.Role.Type != Platform.Enumerations.eRoleType.SystemAdministrator)
            {
     
                getListAsEnumerable = GetList<SpreadSheet>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("User", this.CurrentUser.Oid)).AsEnumerable<SpreadSheet>();
            }

            return View("Index", getListAsEnumerable);
        }


        /// <summary>
        /// SpreadSheet partial.
        /// </summary>
        /// <returns></returns>
        public ActionResult SpreadSheetPartial(){

            DevExpressHelper.Theme = "MetropolisBlue";
            return PartialView();
        }

        /// <summary>
        /// Creates this instance.
        /// GET: /SpreadSheet/Create
        /// </summary>
        /// <returns></returns>
        [Security(ReturnsPartial = false)]
        public ActionResult Create()
        {            
            DevExpressHelper.Theme = "MetropolisBlue";
            return View();
        }


        /// <summary>
        /// Creates the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {   
            try
            { 
                // TODO: Add insert logic here
                GenerateUnitOfWork();   
                
                IWorkbook book = SpreadsheetExtension.GetCurrentDocument("Spreadsheet");
                byte[] docBytes = book.SaveDocument(DocumentFormat.Xlsx);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork()) 
                {
                    
                    SpreadSheet spreadsheet = new SpreadSheet(uow) 
                    { 
                        BinaryFile = docBytes,
                        User = uow.GetObjectByKey<User>(this.CurrentUser.Oid),
                        Code = Request["Code"],
                        Title = Request["Title"],
                        FileName = Request["FileName"]
                    };

                    spreadsheet.Save();

                    uow.CommitTransaction();
                }

                Session["Notice"] = Resources.FileSuccessfullyCreated;
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                Session["Error"] = Resources.Error;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Edits the specified oid.
        /// </summary>
        /// <param name="Oid">Oid.</param>
        /// <returns></returns>
        [Security(ReturnsPartial = false)]
        public ActionResult Edit(Guid Oid)
        {
            GenerateUnitOfWork();   
            Guid spreadSheetGuid = Oid;
            SpreadSheet spreadSheet;
            UnitOfWork uow = (UnitOfWork)Session["uow"];

            spreadSheet = uow.GetObjectByKey<SpreadSheet>(Oid);


            DevExpressHelper.Theme = "MetropolisBlue";
            return View("Create",spreadSheet);
        }

        /// <summary>
        /// Edits the specified oid.
        /// </summary>
        /// <param name="Oid">The oid.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Guid Oid, FormCollection collection)
        {
            try
            {                
                // TODO: Add update logic here                

                GenerateUnitOfWork();
                IWorkbook book = SpreadsheetExtension.GetCurrentDocument("Spreadsheet");
                byte[] docBytes = book.SaveDocument(DocumentFormat.Xlsx);

                SpreadSheet spreadSheet = uow.GetObjectByKey<SpreadSheet>(Oid);

                spreadSheet.BinaryFile = docBytes;
                spreadSheet.User = uow.GetObjectByKey<User>(this.CurrentUser.Oid);
                spreadSheet.Code = Request["Code"];
                spreadSheet.Title = Request["Title"];
                spreadSheet.FileName = Request["FileName"];

                uow.CommitTransaction();

                Session["Notice"] = Resources.FileSuccessfullyUpdated;

                return RedirectToAction("Index");
            }
            catch
            {
                Session["Error"] = Resources.Error;
                return View();
            }
        }


        /// <summary>
        /// Deletes the specified ct.
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] SpreadSheet ct)
        {
            if (!TableCanDelete) return null;
            try
            {
                // TODO: Add delete logic here
                DeleteT(ct);
                Session["Notice"] = Resources.SuccesfullyDeleted;
                //return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                string errorMessage = e.GetFullMessage();
                Session["Error"] = Resources.Error;
            }
        
            GenerateUnitOfWork();

            return PartialView("Grid", GetList<SpreadSheet>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<SpreadSheet>());
        }
        
    }
}
