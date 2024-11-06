using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class ApplicationSettingsController : BaseObjController<ApplicationSettings>
    {
        UnitOfWork uow;

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
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {

                GenerateUnitOfWork();

                ToolbarOptions.ForceVisible = false;

                XPCollection<ApplicationSettings> appSettingsCollection = GetList<ApplicationSettings>(uow);
                if (appSettingsCollection.Count == 0)
                {
                    ApplicationSettings appSettings = new ApplicationSettings(uow);
                    appSettings.Save();
                    XpoHelper.CommitTransaction(uow);
                    return View("Index", appSettings);
                }

                return View("Index", appSettingsCollection[0]);
            
        }

        public ActionResult TermsEditor()
        {

            return PartialView();
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Save()
        {
            GenerateUnitOfWork();
            XPCollection<ApplicationSettings> appSettingsCollection = GetList<ApplicationSettings>(uow);
            ApplicationSettings appSettings = appSettingsCollection[0];

            ToolbarOptions.ForceVisible = true;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CustomButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = true;
            ToolbarOptions.ShowHideMenu.Visible = true;
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;

            if (Request.HttpMethod == "POST" && MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                //bool doPadding = Request["DoPaddingEdit"] == "C" ? true : false;
                //int barcodeLength = int.Parse(Request["BarcodeLengthEdit"]);
                //int itemCodeLength = int.Parse(Request["ItemCodeLengthEdit"]);
                //string barcodePaddingCharacter = Request["BarcodePaddingCharacterEdit"];
                //string itemCodePaddingCharacter = Request["ItemCodePaddingCharacterEdit"];
                //double computeDigits = Double.Parse(Request["ComputeDigitsEdit"]);
                //double displayDigits = Double.Parse(Request["DisplayDigitsEdit"]);
                //double displayValueDigits = Double.Parse(Request["DisplayValueDigitsEdit"]);
                //double maxItemOrderQty = Double.Parse(Request["MaxItemOrderQtyEdit"]);
                //bool mergedSameDocumentLines = Request["MergedSameDocumentLinesEdit"] == "C" ? true : false;
                //bool useBarcodeRelationFactor = Request["UseBarcodeRelationFactorEdit"] == "C" ? true : false;
                //bool DiscountPermited = Request["DiscountPermitedEdit"] == "C" ? true : false;
                //bool RecomputePrices = Request["RecomputePricesEdit"] == "C" ? true : false;
                //string terms = WebUtility.HtmlDecode(Request["Terms_Html"] == null ? "" : Request["Terms_Html"]);
                string loglevelstr = Request["loglevel"];
                
                
                //appSettings.TransactionFilesFolder = Request["TransactionFilesFolder"];

                foreach (LogLevel loglevel in Enum.GetValues(typeof(LogLevel)))
                {
                    if(loglevel.ToString().Equals(loglevelstr))
                    {
                        appSettings.LogingLevel = loglevel;
                        break;
                    }
                }

                //appSettings.DoPadding = doPadding;
                //appSettings.BarcodeLength = barcodeLength;
                //appSettings.ItemCodeLength = itemCodeLength;
                //appSettings.BarcodePaddingCharacter = barcodePaddingCharacter;
                //appSettings.ItemCodePaddingCharacter = itemCodePaddingCharacter;
                //appSettings.ComputeDigits = computeDigits;
                //appSettings.DisplayDigits = displayDigits;
                //appSettings.DisplayValueDigits = displayValueDigits;
                //appSettings.MaxItemOrderQty = maxItemOrderQty;
                //appSettings.MergedSameDocumentLines = mergedSameDocumentLines;
                //appSettings.UseBarcodeRelationFactor = useBarcodeRelationFactor;
                //appSettings.DiscountPermited = DiscountPermited;
                //appSettings.RecomputePrices = RecomputePrices;
                //appSettings.ApplicationTerms = terms;
                appSettings.Save();
                //uow.CommitTransaction();
                XpoHelper.CommitTransaction(uow);
                AppiSettings.ReadSettings(appSettings);

                Session["Notice"] = Resources.SavedSuccesfully;
            }

            return View("Index", appSettings);
        }

    }
}
