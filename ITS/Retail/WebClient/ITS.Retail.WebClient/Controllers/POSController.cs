//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System.Web.Script.Serialization;
using System.Reflection;
using ITS.Retail.Platform.Enumerations;
using System.Threading;
using ITS.Retail.WebClient.Providers;
using System.Text.RegularExpressions;
using System.Text;
using DevExpress.Xpo.DB;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSController : BaseObjController<ITS.Retail.Model.POS>
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

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.POSInfo;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.CustomButton.Visible = true;
            this.ToolbarOptions.CustomButton.CCSClass = "pos";
            this.ToolbarOptions.CustomButton.Title = Resources.MassivelyCreatePOS;
            this.ToolbarOptions.CustomButton.OnClick = "MassivelyCreatePOS";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "POSGuid");
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSs");


            GenerateUnitOfWork();
            FillLookupComboBoxes();
            CriteriaOperator filter = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
            return View("Index", GetList<ITS.Retail.Model.POS>(uow, filter).AsEnumerable<ITS.Retail.Model.POS>());
        }

        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            CriteriaOperator filter = null;

            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {

                if (Request.HttpMethod == "POST")
                {
                    string fid = Request["fid"] == null || Request["fid"] == "null" ? "" : Request["fid"];
                    string fname = Request["fname"] == null || Request["fname"] == "null" ? "" : Request["fname"];
                    string fstore = Request["fstore"] == null || Request["fstore"] == "null" ? "" : Request["fstore"];
                    string fcustomer = Request["fcustomer"] == null || Request["fcustomer"] == "null" ? "" : Request["fcustomer"];
                    string fIPAddress = Request["fIPAddress"] == null || Request["fIPAddress"] == "null" ? "" : Request["fIPAddress"];
                    string fFiscalDevice = Request["fFiscalDevice"] == null || Request["fFiscalDevice"] == "null" ? "" : Request["fFiscalDevice"];

                    CriteriaOperator idFilter = null;
                    if (fid != null && fid.Trim() != "")
                    {
                        if (fid.Replace('%', '*').Contains("*"))
                            idFilter = new BinaryOperator("ID", fid.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            idFilter = new BinaryOperator("ID", fid);
                    }

                    CriteriaOperator nameFilter = null;
                    if (fname != null && fname.Trim() != "")
                    {
                        if (fname.Replace('%', '*').Contains("*"))
                            nameFilter = new BinaryOperator("ID", fname.Replace('*', '%').Replace('=', '%'), BinaryOperatorType.Like);
                        else
                            nameFilter = new BinaryOperator("ID", fname);
                    }
                    eFiscalDevice fiscalDevice;
                    CriteriaOperator fiscalDeviceFilter = null;
                    if (Enum.TryParse<eFiscalDevice>(fFiscalDevice, true, out fiscalDevice))
                    {
                        fiscalDeviceFilter = new BinaryOperator("FiscalDevice", fiscalDevice);
                    }

                    CriteriaOperator storeFilter = null;
                    if (fstore != "")
                    {
                        storeFilter = new BinaryOperator("Store.Oid", fstore);
                    }

                    CriteriaOperator customerFilter = null;
                    if (fcustomer != "")
                    {
                        customerFilter = new BinaryOperator("DefaultCustomer.Oid", fcustomer);
                    }

                    CriteriaOperator visiblePOSs = null;

                    filter = CriteriaOperator.And(idFilter, nameFilter, fiscalDeviceFilter, storeFilter, customerFilter, visiblePOSs);
                    Session["POSFilter"] = filter;
                }
                else
                {
                    filter = new BinaryOperator("Oid", Guid.Empty);
                    Session["POSFilter"] = filter;
                }
            }
            GridFilter = (CriteriaOperator)Session["POSFilter"];
            return base.Grid();

        }

        public ActionResult Edit(string Oid)
        {
            GenerateUnitOfWork();
            Guid posGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);

            ViewData["EditMode"] = true;

            if (posGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (posGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            Model.POS pos;
            if (Session["UnsavedPOS"] == null)
            {
                if (posGuid != Guid.Empty)
                {
                    ViewBag.Mode = Resources.EditPOS;
                    pos = uow.FindObject<Model.POS>(new BinaryOperator("Oid", posGuid));
                    Session["IsNewPOS"] = false;
                }
                else
                {
                    ViewBag.Mode = Resources.NewPOS;
                    pos = new Model.POS(uow);
                    Session["IsNewPOS"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (posGuid != Guid.Empty && (Session["UnsavedPOS"] as Model.POS).Oid == posGuid)
                {
                    Session["IsRefreshed"] = true;
                    pos = (Model.POS)Session["UnsavedPOS"];
                }
                else if (posGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    pos = (Model.POS)Session["UnsavedPOS"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    pos = uow.FindObject<Model.POS>(new BinaryOperator("Oid", posGuid));
                }
            }
            FillLookupComboBoxes();
            ViewData["POSGuid"] = pos.Oid.ToString();
            Session["UnsavedPOS"] = pos;

            ViewBag.DocumentSeries = GetList<StoreDocumentSeriesType>(uow, CriteriaOperator.And(
                    new BinaryOperator("DocumentSeries.eModule", eModule.POS, BinaryOperatorType.NotEqual)

                ));

            return PartialView("Edit", pos);
        }

        public JsonResult Save()
        {
            GenerateUnitOfWork();
            Guid posGuid = Guid.Empty;

            bool correctPOSGuid = Request["POSGuid"] != null && Guid.TryParse(Request["POSGuid"].ToString(), out posGuid);
            if (correctPOSGuid)
            {
                ITS.Retail.Model.POS pos = (Session["UnsavedPOS"] as ITS.Retail.Model.POS);
                if (pos != null)
                {
                    int i = 0;
                    Int32.TryParse(Request["ID"], out i);
                    pos.ID = i;
                    pos.Name = Request["Name"];
                    pos.IPAddress = Request["IPAddress"];
                    pos.Remarks = Request["Remarks"];
                    pos.Store = uow.FindObject<Store>(new BinaryOperator("Oid", StoreControllerAppiSettings.CurrentStoreOid));
                    pos.ABCDirectory = Request["ABCDirectory"];
                    pos.CultureInfo = (eCultureInfo)Enum.Parse(typeof(eCultureInfo), Request["CultureInfo_VI"]);
                    pos.DefaultCustomer = uow.FindObject<Customer>(new BinaryOperator("Oid", (Request["Customer_VI"] == null || Request["Customer_VI"] == "") ? Guid.Empty : Guid.Parse(Request["Customer_VI"])));
                    pos.DefaultDocumentSeries = uow.FindObject<DocumentSeries>(new BinaryOperator("Oid", (Request["DefaultDocumentSeries_VI"] == null || Request["DefaultDocumentSeries_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DefaultDocumentSeries_VI"])));
                    pos.DefaultDocumentStatus = uow.FindObject<DocumentStatus>(new BinaryOperator("Oid", (Request["DefaultDocumentStatus_VI"] == null || Request["DefaultDocumentStatus_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DefaultDocumentStatus_VI"])));
                    pos.DefaultDocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Oid", (Request["DefaultDocumentType_VI"] == null || Request["DefaultDocumentType_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DefaultDocumentType_VI"])));
                    pos.ProFormaInvoiceDocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Oid", (Request["ProFormaInvoiceDocumentType_VI"] == null || Request["ProFormaInvoiceDocumentType_VI"] == "") ? Guid.Empty : Guid.Parse(Request["ProFormaInvoiceDocumentType_VI"])));
                    pos.ProFormaInvoiceDocumentSeries = uow.FindObject<DocumentSeries>(new BinaryOperator("Oid", (Request["ProFormaInvoiceDocumentSeries_VI"] == null || Request["ProFormaInvoiceDocumentSeries_VI"] == "") ? Guid.Empty : Guid.Parse(Request["ProFormaInvoiceDocumentSeries_VI"])));

                    Guid specialProformaDocumentTypeGuid = Guid.Empty;
                    if (Request["SpecialProformaDocumentType_VI"] != null && Guid.TryParse(Request["SpecialProformaDocumentType_VI"].ToString(), out specialProformaDocumentTypeGuid))
                    {
                        pos.SpecialProformaDocumentType = uow.GetObjectByKey<DocumentType>(specialProformaDocumentTypeGuid);
                    }
                    else
                    {
                        pos.SpecialProformaDocumentType = null;
                    }
                    Guid specialProformaDocumentSeriesGuid = Guid.Empty;
                    if (Request["SpecialProformaDocumentSeries_VI"] != null && Guid.TryParse(Request["SpecialProformaDocumentSeries_VI"].ToString(), out specialProformaDocumentSeriesGuid))
                    {
                        pos.SpecialProformaDocumentSeries = uow.GetObjectByKey<DocumentSeries>(specialProformaDocumentSeriesGuid);
                    }
                    else
                    {
                        pos.SpecialProformaDocumentSeries = null;
                    }



                    pos.WithdrawalDocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Oid", (Request["WithdrawalDocumentType_VI"] == null || Request["WithdrawalDocumentType_VI"] == "") ? Guid.Empty : Guid.Parse(Request["WithdrawalDocumentType_VI"])));
                    pos.WithdrawalDocumentSeries = uow.FindObject<DocumentSeries>(new BinaryOperator("Oid", (Request["WithdrawalDocumentSeries_VI"] == null || Request["WithdrawalDocumentSeries_VI"] == "") ? Guid.Empty : Guid.Parse(Request["WithdrawalDocumentSeries_VI"])));
                    pos.WithdrawalItem = uow.FindObject<SpecialItem>(new BinaryOperator("Oid", (Request["WithdrawalItemComboBox_VI"] == null || Request["WithdrawalItemComboBox_VI"] == "") ? Guid.Empty : Guid.Parse(Request["WithdrawalItemComboBox_VI"])));
                    pos.DepositDocumentType = uow.FindObject<DocumentType>(new BinaryOperator("Oid", (Request["DepositDocumentType_VI"] == null || Request["DepositDocumentType_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DepositDocumentType_VI"])));
                    pos.DepositDocumentSeries = uow.FindObject<DocumentSeries>(new BinaryOperator("Oid", (Request["DepositDocumentSeries_VI"] == null || Request["DepositDocumentSeries_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DepositDocumentSeries_VI"])));
                    pos.DepositItem = uow.FindObject<SpecialItem>(new BinaryOperator("Oid", (Request["DepositItemComboBox_VI"] == null || Request["DepositItemComboBox_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DepositItemComboBox_VI"])));
                    pos.DefaultPaymentMethod = uow.FindObject<PaymentMethod>(new BinaryOperator("Oid", (Request["DefaultPaymentMethod_VI"] == null || Request["DefaultPaymentMethod_VI"] == "") ? Guid.Empty : Guid.Parse(Request["DefaultPaymentMethod_VI"])));
                    pos.ReceiptFormat = uow.FindObject<POSPrintFormat>(new BinaryOperator("Oid", (Request["ReceiptFormat_VI"] == null || Request["ReceiptFormat_VI"] == "") ? Guid.Empty : Guid.Parse(Request["ReceiptFormat_VI"])));
                    pos.XFormat = uow.FindObject<POSPrintFormat>(new BinaryOperator("Oid", (Request["XFormat_VI"] == null || Request["XFormat_VI"] == "") ? Guid.Empty : Guid.Parse(Request["XFormat_VI"])));
                    pos.ZFormat = uow.FindObject<POSPrintFormat>(new BinaryOperator("Oid", (Request["ZFormat_VI"] == null || Request["ZFormat_VI"] == "") ? Guid.Empty : Guid.Parse(Request["ZFormat_VI"])));
                    pos.POSKeysLayout = uow.FindObject<POSKeysLayout>(new BinaryOperator("Oid", (Request["POSKeysLayout_VI"] == null || Request["POSKeysLayout_VI"] == "") ? Guid.Empty : Guid.Parse(Request["POSKeysLayout_VI"])));
                    pos.POSLayout = uow.FindObject<POSLayout>(new BinaryOperator("Oid", (Request["POSLayout_VI"] == null || Request["POSLayout_VI"] == "") ? Guid.Empty : Guid.Parse(Request["POSLayout_VI"])));
                    pos.POSActionLevelsSet = uow.FindObject<POSActionLevelsSet>(new BinaryOperator("Oid", (Request["POSActionLevelsSet_VI"] == null || Request["POSActionLevelsSet_VI"] == "") ? Guid.Empty : Guid.Parse(Request["POSActionLevelsSet_VI"])));

                    pos.FiscalDevice = (eFiscalDevice)Enum.Parse(typeof(eFiscalDevice), Request["FiscalDevice_VI"]);
                    pos.CurrencyPattern = (eCurrencyPattern)Enum.Parse(typeof(eCurrencyPattern), Request["CurrencyPattern_VI"]);
                    pos.ReceiptVariableIdentifier = Request["ReceiptVariableIdentifier"].FirstOrDefault();
                    pos.CurrencySymbol = Request["CurrencySymbol"].FirstOrDefault();
                    pos.UsesTouchScreen = Request["UsesTouchScreen"] == "C";
                    pos.UsesKeyLock = Request["UsesKeyLock"] == "C";
                    pos.AutoFocus = Request["AutoFocus"] == "C";
                    pos.AsksForStartingAmount = Request["AsksForStartingAmount"] == "C";
                    pos.PrintDiscountAnalysis = Request["PrintDiscountAnalysis"] == "C";
                    pos.AutoIssueZEAFDSS = Request["AutoIssueZEAFDSS"] == "C";
                    pos.EnableLowEndMode = Request["EnableLowEndMode"] == "C";
                    pos.DemoMode = Request["DemoMode"] == "C";
                    pos.AsksForFinalAmount = Request["AsksForFinalAmount"] == "C";
                    pos.IsCashierRegister = Request["IsCashierRegister"] == "C";
                    pos.UseSliderPauseForm = Request["UseSliderPauseForm"] == "C";
                    pos.UseCashCounter = Request["UseCashCounter"] == "C";
                    bool posIsActiveChanged = false;
                    if (pos.IsActive != (Request["IsActive"] == "C"))
                    {
                        posIsActiveChanged = true;
                    }

                    pos.IsActive = Request["IsActive"] == "C";

                    pos.ForcedWithdrawMode = (eForcedWithdrawMode)Enum.Parse(typeof(eForcedWithdrawMode), Request["ForcedWithdrawMode_VI"]);

                    decimal forcedWithdrawCashAmountLimit = 0;
                    decimal.TryParse(Request["ForcedWithdrawCashAmountLimit"], out forcedWithdrawCashAmountLimit);
                    pos.ForcedWithdrawCashAmountLimit = forcedWithdrawCashAmountLimit;

                    pos.StandaloneFiscalOnErrorMessage = Request["StandaloneFiscalOnErrorMessage"];

                    IEnumerable<Guid> selectedStoreDocumentSeriesTypes, storeDocumentSeriesTypesToAdd, storeDocumentSeriesTypesToRemove, existingStoreDocumentSeriesTypes;
                    existingStoreDocumentSeriesTypes = pos.POSDocumentSeries.Select(x => x.StoreDocumentSeriesType.Oid);
                    selectedStoreDocumentSeriesTypes = String.IsNullOrEmpty(Request["documentseries_selected"]) ? new List<Guid>() : Request["documentseries_selected"].Split(',').Select(x => Guid.Parse(x));
                    storeDocumentSeriesTypesToAdd = selectedStoreDocumentSeriesTypes.Except(existingStoreDocumentSeriesTypes);
                    storeDocumentSeriesTypesToRemove = existingStoreDocumentSeriesTypes.Except(selectedStoreDocumentSeriesTypes);
                    foreach (Guid series in storeDocumentSeriesTypesToAdd)
                    {
                        StoreDocumentSeriesType seriesObject = uow.GetObjectByKey<StoreDocumentSeriesType>(series);

                        if (seriesObject != null)
                        {
                            POSDocumentSeries newPosDocSeries = new POSDocumentSeries(uow) { StoreDocumentSeriesType = seriesObject };
                            pos.POSDocumentSeries.Add(newPosDocSeries);
                        }
                    }
                    pos.POSDocumentSeries.Where(x => storeDocumentSeriesTypesToRemove.Contains(x.StoreDocumentSeriesType.Oid)).ToList().ForEach(x => x.Delete());

                    try
                    {
                        //Check if by saving this pos, license limit is reached
                        if (pos.IsActive && MvcApplication.USES_LICENSE)
                        {
                            Guid serverID = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL ? Guid.Empty : StoreControllerAppiSettings.StoreControllerOid;
                            int licenseManagerMaxUsers = MvcApplication.ReadStoredLicenseInfo().MaxUsers;
                            int maximumAllowedUsers = MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE
                                                  ? licenseManagerMaxUsers
                                                  : LicenseUserDistributionHelper.GetMaximumAllowedUsers(MvcApplication.LicenseServerInstanceType, serverID, MvcApplication.ApplicationInstance, licenseManagerMaxUsers);

                            int newNonWebUsersCount;
                            if (pos.Session.IsNewObject(pos) || posIsActiveChanged)
                            {
                                newNonWebUsersCount = MvcApplication.ActiveUsersValidator.CountActiveNonWebUsers() + 1;
                            }
                            else
                            {
                                newNonWebUsersCount = MvcApplication.ActiveUsersValidator.CountActiveNonWebUsers();
                            }

                            if (newNonWebUsersCount > maximumAllowedUsers)
                            {
                                throw new Exception(Resources.CannotSaveBecauseLimitWouldBeExceeded);
                            }
                        }

                        pos.Save();
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {
                        uow.RollbackTransaction();
                        Session["Error"] = Resources.AnErrorOccurred + ":" + (e.InnerException == null ? e.Message : e.InnerException.Message);
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["IsNewPOS"] = null;
                        Session["uow"] = null;
                        Session["UnsavedPOS"] = null;
                        Session["IsRefreshed"] = null;
                    }

                }
            }
            return Json(new { });
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (Session["IsRefreshed"] != null && !Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewPOS"] = null;
                Session["UnsavedPOS"] = null;

            }
            return null;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = false)]
        public ActionResult POSStatus()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                List<PosCommandResult> commandResults = new List<PosCommandResult>();
                //DateTime cmdDate = new DateTime(DateTime.Now.Ticks);
                //cmdDate.Subtract(TimeSpan.FromDays(1));
                long ticksfilter = (DateTime.Now.Ticks - 84000000000);
                CriteriaOperator commandCriteria = CriteriaOperator.And(new BinaryOperator("CreatedOnTicks", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual));
                commandResults = new XPCollection<PosCommandResult>(uow, commandCriteria, new SortProperty("CreatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending)).ToList();

                ViewData.Add("commandResults", commandResults);
                ViewBag.commandResults = commandResults;
            }

            ToolbarOptions.ForceVisible = false;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CustomButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSStatuses");
            return View(GetList<ITS.Retail.Model.POS>(uow, null, "ID"));
        }

        public ActionResult POSStatusGrid()
        {
            GenerateUnitOfWork();
            if (Request["DXCallbackArgument"] != null)
            {
                try
                {
                    List<PosCommandResult> commandResults = new List<PosCommandResult>();
                    long ticksfilter = (DateTime.Now.Ticks - 84000000000);
                    CriteriaOperator commandCriteria = CriteriaOperator.And(new BinaryOperator("CreatedOnTicks", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual));
                    commandResults = new XPCollection<PosCommandResult>(uow, commandCriteria, new SortProperty("CreatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending)).ToList();

                    ViewData.Add("commandResults", commandResults);
                    ViewBag.commandResults = commandResults;


                    string lastValueOfDXCallaback = Request["DXCallbackArgument"].ToString().Split('|').Last();
                    string posGuidString = lastValueOfDXCallaback.Substring(0, lastValueOfDXCallaback.Length - 1);

                    Guid posGuid;
                    if (Guid.TryParse(posGuidString, out posGuid))
                    {
                        if (Request["DXCallbackArgument"].ToUpper().Contains("RESTART"))
                        {
                            POSHelper.AddPosCommand(posGuid, ePosCommand.RESTART_POS, "");
                        }
                        else if (Request["DXCallbackArgument"].ToUpper().Contains("ISSUE_X"))
                        {
                            POSHelper.AddPosCommand(posGuid, ePosCommand.ISSUE_X, "");
                        }
                        else if (Request["DXCallbackArgument"].ToUpper().Contains("ISSUE_Z"))
                        {
                            POSHelper.AddPosCommand(posGuid, ePosCommand.ISSUE_Z, "");
                        }
                    }
                }
                catch { }
            }

            return PartialView(GetList<ITS.Retail.Model.POS>(uow, null, "ID"));
        }



        public ActionResult POSComandsResultsGrid()
        {
            List<PosCommandResult> commandResults = new List<PosCommandResult>();
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    long ticksfilter = (DateTime.Now.Ticks - 84000000000);
                    CriteriaOperator commandCriteria = CriteriaOperator.And(new BinaryOperator("CreatedOnTicks", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual));
                    commandResults = new XPCollection<PosCommandResult>(uow, commandCriteria, new SortProperty("CreatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending)).ToList();

                    ViewData.Add("commandResults", commandResults);
                    ViewBag.commandResults = commandResults;
                }
            }
            catch (Exception ex)
            { }

            return PartialView(commandResults);
        }


        public string RemoveChars(string s, List<char> removeChars)
        {
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (!removeChars.Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }



        public JsonResult PosSqlCommands(String poses, String command, String commandType, int timeout)
        {
            String message = "ok";
            List<char> charList = new List<char>();
            charList.Add('\n');
            charList.Add('\r');
            command = RemoveChars(command, charList);
            command.TrimStart();
            if (timeout == 0)
                timeout = 6000;
            else
                timeout = timeout * 1000;

            if (!String.IsNullOrEmpty(poses) || !String.IsNullOrEmpty(commandType))
            {
                try
                {
                    ePosCommand cmdType;
                    Enum.TryParse(commandType, out cmdType);

                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        String[] guidStrings = poses.Split('|');
                        for (int i = 0; i < guidStrings.Length; i++)
                        {
                            Guid posGuid;
                            Guid.TryParse(guidStrings[i], out posGuid);
                            Model.POS pos = uow.GetObjectByKey<Model.POS>(posGuid);
                            POSCommand poscommand = new POSCommand(uow);
                            poscommand.POS = pos;
                            poscommand.CommandParameters = command;
                            poscommand.Command = cmdType;
                            poscommand.Timeout = timeout;
                            poscommand.Save();
                        }
                        XpoHelper.CommitChanges(uow);
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            else
            {
                return Json(new { result = "Invalid Command" });
            }

            return Json(new { result = message });
        }

        public static object ItemRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            if (e.Value != null)
            {
                SpecialItem obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<SpecialItem>(e.Value);
                return obj;
            }
            return null;
        }

        public static object GetItemByValue(object value)
        {
            return GetObjectByValue<SpecialItem>(value);

        }

        public static object ItemsRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string barcodeFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            UnitOfWork uowLocal = XpoHelper.GetNewUnitOfWork();
            XPCollection<SpecialItem> collection = GetList<SpecialItem>(uowLocal,
                                                                CriteriaOperator.Or(new BinaryOperator("Description", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Code", String.Format("{0}", codefilter), BinaryOperatorType.Like)),
                                                                "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult WithdrawalItemComboBox()
        {
            return PartialView();
        }

        public ActionResult DepositItemComboBox()
        {
            return PartialView();
        }

        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
            Store store = uow.GetObjectByKey<Store>(CurrentStore.Oid);
            XPCollection<POSDevice> devices = GetList<POSDevice>(uow);
            ViewBag.Devices = devices == null ? null : devices.OrderBy(dev => dev.Name);
            ViewBag.DocumentTypes = StoreHelper.StoreDocumentTypes(store, null, eModule.POS);
            ViewBag.DocumentStatus = GetList<DocumentStatus>(uow);
            ViewBag.PaymentMethods = GetList<PaymentMethod>(uow);
            ViewBag.ReceiptFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.Receipt));
            ViewBag.XFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.X));
            ViewBag.ZFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.Z));
            ViewBag.POSKeysLayouts = GetList<POSKeysLayout>(uow);
            ViewBag.POSLayouts = GetList<POSLayout>(uow);
            ViewBag.PosReports = GetList<PosReport>(uow);
            ViewBag.SpecialItems = GetList<SpecialItem>(uow);
            ViewBag.POSActionLevelsSets = GetList<POSActionLevelsSet>(uow);
            ViewBag.POSList = (GetList<Model.POS>(uow)).Select(p => new KeyValuePair<Guid, string>(p.Oid, p.Name)).OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
            ViewBag.ScalesCount = GetList<Scale>(uow).Count;

            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;
            if (pos != null)
            {
                ViewBag.Reports = GetList<POSPrintFormat>(pos.Session, ApplyOwnerCriteria(null, typeof(POSPrintFormat), EffectiveOwner));
                ViewBag.CustomReports = GetList<CustomReport>(pos.Session, ApplyOwnerCriteria(null, typeof(CustomReport), EffectiveOwner)).Where(x => x.ReportType == "Single Object Report");
            }
        }

        public static object StoresRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            CriteriaOperator visibleStores = null;
            if (!Boolean.Parse(System.Web.HttpContext.Current.Session["IsAdministrator"].ToString()))
            {
                User currentUser = CurrentUserStatic;

                List<CriteriaOperator> visibleStoresFilterList = UserHelper.GetUserSupplierStoresFilter(currentUser);
                if (visibleStoresFilterList.Count != 0)
                {
                    visibleStores = CriteriaOperator.Or(visibleStoresFilterList);
                }
                else
                {
                    visibleStores = new BinaryOperator("Oid", Guid.Empty, BinaryOperatorType.Equal);
                }
            }

            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork());
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            collection.Criteria = CriteriaOperator.And(visibleStores, CriteriaOperator.Or(new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                          new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                          new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)));
            return collection;
        }

        public static object CustomersRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            string nameFilter = e.Filter.Replace('*', '%').Replace('=', '%');
            string codefilter = e.Filter.Replace('*', '%').Replace('=', '%');
            XPCollection<Customer> collection = GetList<Customer>(XpoHelper.GetNewUnitOfWork(),
                                                                CriteriaOperator.Or(new BinaryOperator("CompanyName", String.Format("%{0}%", nameFilter), BinaryOperatorType.Like),
                                                                                    new BinaryOperator("Code", String.Format("%{0}%", codefilter), BinaryOperatorType.Like)),
                                                                "Code");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        public ActionResult CustomersComboBox(bool? IsFilter)
        {
            ViewData["IsFilter"] = IsFilter;
            return PartialView();
        }

        public ActionResult DeviceGrid(string POSGuid, bool editMode)
        {
            ViewData["EditMode"] = editMode;
            if (/*editMode == null ||*/ editMode == true)  //edit mode
            {
                GenerateUnitOfWork();
                FillLookupComboBoxes();
                if (Request["DXCallbackArgument"].Contains("ADDNEW"))
                {
                    TerminalDeviceAssociation tda = new TerminalDeviceAssociation(uow);
                    Session["UnsavedTerminalDeviceAssociation"] = tda;
                }
                else if (Request["DXCallbackArgument"].Contains("CANCELEDIT"))
                {
                    Session["UnsavedTerminalDeviceAssociation"] = null;
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];
                    foreach (TerminalDeviceAssociation tda in pos.TerminalDeviceAssociations)
                    {
                        Guid TerminalDeviceAssociationID = RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]);
                        if (tda.Oid == TerminalDeviceAssociationID)
                        {
                            Session["UnsavedTerminalDeviceAssociation"] = tda;
                            break;
                        }
                    }
                }

                return PartialView("DevicesGrid", ((ITS.Retail.Model.POS)Session["UnsavedPOS"]).TerminalDeviceAssociations);
            }
            else  //view mode
            {
                Guid POSGuidParsed = (POSGuid == null || POSGuid == "null" || POSGuid == "-1") ? Guid.Empty : Guid.Parse(POSGuid);
                ITS.Retail.Model.POS pos = XpoHelper.GetNewUnitOfWork().FindObject<ITS.Retail.Model.POS>(new BinaryOperator("Oid", POSGuidParsed, BinaryOperatorType.Equal));
                ViewData["POSGuid"] = POSGuid;
                return PartialView("DevicesGrid", pos.TerminalDeviceAssociations);
            }

        }

        [HttpPost]
        public ActionResult InsertDevice([ModelBinder(typeof(RetailModelBinder))] TerminalDeviceAssociation ct)
        {
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];
                    TerminalDeviceAssociation terminalDeviceAssociation = new TerminalDeviceAssociation(pos.Session);
                    terminalDeviceAssociation.GetData(ct, new List<string>() { "Session" });
                    Guid deviceID = Guid.Empty;
                    Guid.TryParse(Request["Device_VI"], out deviceID);
                    POSDevice device = pos.Session.GetObjectByKey<POSDevice>(deviceID);
                    terminalDeviceAssociation.TerminalDevice = device;
                    pos.TerminalDeviceAssociations.Add(terminalDeviceAssociation);
                    terminalDeviceAssociation.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            FillLookupComboBoxes();
            return PartialView("DevicesGrid", ((ITS.Retail.Model.POS)Session["UnsavedPOS"]).TerminalDeviceAssociations);
        }

        [HttpPost]
        public ActionResult UpdateDevice([ModelBinder(typeof(RetailModelBinder))] TerminalDeviceAssociation ct)
        {
            ViewData["EditMode"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];
                    TerminalDeviceAssociation terminalDeviceAssociation = pos.TerminalDeviceAssociations.Where(terminalDeviceAssoc => terminalDeviceAssoc.Oid == ct.Oid).FirstOrDefault();
                    terminalDeviceAssociation.GetData(ct, new List<string>() { "Session" });
                    Guid deviceID = Guid.Empty;
                    Guid.TryParse(Request["Device_VI"], out deviceID);
                    POSDevice device = pos.Session.GetObjectByKey<POSDevice>(deviceID);
                    terminalDeviceAssociation.TerminalDevice = device;
                    terminalDeviceAssociation.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            FillLookupComboBoxes();
            return PartialView("DevicesGrid", ((ITS.Retail.Model.POS)Session["UnsavedPOS"]).TerminalDeviceAssociations);
        }

        [HttpPost]
        public ActionResult DeleteDevice([ModelBinder(typeof(RetailModelBinder))] TerminalDeviceAssociation ct)
        {
            ViewData["EditMode"] = true;
            GenerateUnitOfWork();
            try
            {
                ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];
                pos.TerminalDeviceAssociations.First(terminalDeviceAssoc => terminalDeviceAssoc.Oid == ct.Oid).Delete();
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }

            FillLookupComboBoxes();
            return PartialView("DevicesGrid", ((ITS.Retail.Model.POS)Session["UnsavedPOS"]).TerminalDeviceAssociations);
        }

        private string GetDocumentTypeOid(ITS.Retail.Model.POS pos, ePOSDocumentTypes ePosDocumentType)
        {
            PropertyInfo property = pos.GetType().GetProperty(ePosDocumentType + "DocumentType");
            DocumentType value = property.GetValue(pos, null) as DocumentType;
            if (value != null)
            {
                return value.Oid.ToString();
            }
            else
            {
                return null;
            }
        }

        private void ClearDocumentSeries(ITS.Retail.Model.POS pos, ePOSDocumentTypes ePosDocumentType)
        {
            PropertyInfo property = pos.GetType().GetProperty(ePosDocumentType + "DocumentSeries");
            property.SetValue(pos, null, null);
        }

        public ActionResult DocumentSeriesCallbackPanel(ePOSDocumentTypes ePOSDocumentType)
        {
            GenerateUnitOfWork();
            ViewData["ePOSDocumentType"] = ePOSDocumentType;
            try
            {
                string oidStr = null;

                if (Request["DXCallbackArgument"] != null &&
                    Request["DXCallbackArgument"].Contains(":") &&
                    String.IsNullOrWhiteSpace(Request["DXCallbackArgument"].Split(':')[1]) == false)
                {
                    oidStr = Request["DXCallbackArgument"].Split(':')[1];
                    ClearDocumentSeries(Session["UnsavedPOS"] as ITS.Retail.Model.POS, ePOSDocumentType);
                }
                else
                {
                    if ((bool)Session["IsNewPOS"] == false)
                    {
                        oidStr = GetDocumentTypeOid(Session["UnsavedPOS"] as ITS.Retail.Model.POS, ePOSDocumentType);
                    }
                }


                if (!String.IsNullOrWhiteSpace(oidStr))
                {
                    Store store = uow.GetObjectByKey<Store>(CurrentStore.Oid);
                    Guid documentTypeGuid = Guid.Empty;
                    Guid.TryParse(oidStr, out documentTypeGuid);
                    DocumentType docType = uow.GetObjectByKey<DocumentType>(documentTypeGuid);
                    ViewBag.DocumentSeries = StoreHelper.StoreSeriesForDocumentType(store, docType, eModule.POS);
                }
                else
                {
                    ViewBag.DocumentSeries = GetList<DocumentSeries>(uow, new BinaryOperator("Oid", Guid.Empty));
                }
            }
            catch
            {
                ViewBag.DocumentSeries = GetList<DocumentSeries>(uow, new BinaryOperator("Oid", Guid.Empty));
            }

            return PartialView("DocumentSeriesCallbackPanel", Session["UnsavedPOS"]);
        }

        public ActionResult PosListCheck()
        {
            try
            {
                String[] selectedMachines = new JavaScriptSerializer().Deserialize<String[]>(Request["pos"].ToString());
                ePosCommand posCommand = (ePosCommand)Enum.Parse(typeof(ePosCommand), Request["command"].ToString());
                string parameters = Request["parameters"];
                foreach (String smachine in selectedMachines)
                {
                    Guid machine = Guid.Empty;
                    if (Guid.TryParse(smachine, out machine) == true && machine != Guid.Empty)
                    {
                        POSHelper.AddPosCommand(machine, posCommand, parameters);
                    }
                    else if (posCommand == ePosCommand.SEND_CHANGES && smachine.ToLower() == "scales")
                    {
                        ReloadScales();
                    }
                    else if (posCommand == ePosCommand.RELOAD_ENTITIES && smachine.ToLower() == "scales")
                    {
                        ReloadScales();
                    }
                }
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
            }

            if (Request["command"].ToString() == "SEND_CHANGES")
            {
                FillLookupComboBoxes();

                return PartialView("PosListCheckBasic");
            }
            else
            {
                FillLookupComboBoxes();
                return PartialView("PosListCheckAdvanced");
            }
        }

        public override ActionResult Dialog(List<string> arguments)
        {
            GenerateUnitOfWork();
            if (arguments.Contains("DATABASE_PROCESS_DIALOG"))
            {
                this.DialogOptions.OKButton.Visible = false;
                this.DialogOptions.OKButton.OnClick = "function (s,e) { Dialog.Hide(); }";
                this.DialogOptions.BodyPartialView = "POSDatabaseCreationDialog";
                this.DialogOptions.HeaderText = Resources.CreatePOSDatabase;
                this.DialogOptions.AdjustSizeOnInit = true;
                this.DialogOptions.CancelButton.Visible = false;
                this.DialogOptions.OnShownEvent = "Dialog_OnShown";
            }
            else  //POS Status command Dialogs
            {
                this.DialogOptions.OnShownEvent = "EntitiesDialogShown";
                Dictionary<string, string> localizedEntityNames = new Dictionary<string, string>();
                if (arguments.Contains("SEND_CHANGES"))
                {
                    this.DialogOptions.OKButton.OnClick = "DialogSendChangesOkButton_OnClick";
                    this.DialogOptions.BodyPartialView = "EntitiesSelectionDialogBasic";
                    XPCollection<UpdaterMode> updaterModes = GetList<UpdaterMode>(uow);
                    IEnumerable<string> manualEntityNames = updaterModes.Where(x => x.Mode == eUpdaterMode.MANUAL).Select(x => x.EntityName);

                    foreach (string entityname in manualEntityNames)
                    {
                        if (String.IsNullOrEmpty(entityname) == false)
                        {
                            Type entityType = typeof(Item).Assembly.GetType(typeof(Item).FullName.Replace(typeof(Item).Name, entityname));
                            if (entityType != null)
                            {
                                localizedEntityNames.Add(entityname, entityType.ToLocalizedString());
                            }
                        }
                    }
                    ViewData["EntityNames"] = localizedEntityNames;

                }
                else if (arguments.Contains("RELOAD_ENTITIES"))
                {
                    this.DialogOptions.BodyPartialView = "EntitiesSelectionDialogAdvanced";
                    this.DialogOptions.OKButton.OnClick = "DialogOkButton_OnClick";
                    var list = typeof(Item).Assembly.GetTypes().Where(x => (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() != null)
                                    && ((x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS)));
                    foreach (Type type in list)
                    {
                        if (type != null)
                        {
                            localizedEntityNames.Add(type.Name, type.ToLocalizedString());
                        }
                    }
                    ViewData["EntityNames"] = localizedEntityNames;
                }

                this.DialogOptions.HeaderText = Resources.Select;
                this.DialogOptions.AdjustSizeOnInit = true;
                this.DialogOptions.CancelButton.OnClick = "DialogCancelButton_OnClick";

            }
            return PartialView();
        }

        public ActionResult MassivelyCreatePOS()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            ViewBag.PaymentMethods = GetList<PaymentMethod>(uow);
            ViewBag.ReceiptFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.Receipt));
            ViewBag.XFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.X));
            ViewBag.ZFormats = GetList<POSPrintFormat>(uow, new BinaryOperator("FormatType", eFormatType.Z));
            ViewBag.POSKeysLayouts = GetList<POSKeysLayout>(uow);
            ViewBag.POSLayouts = GetList<POSLayout>(uow);
            ViewBag.POSActionLevelsSets = GetList<POSActionLevelsSet>(uow);
            return PartialView();
        }

        public JsonResult CreateMassivelyPOS()
        {
            bool success = false;
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<DocumentSeries> allDocumentSeries = GetList<DocumentSeries>(uow);
                    int minPOS, maxPOS;
                    if (Request["IDs"].ToString().Split('-').Count() == 2)
                    {
                        minPOS = int.Parse(Request["IDs"].ToString().Split('-').First());
                        maxPOS = int.Parse(Request["IDs"].ToString().Split('-').Last());
                    }
                    else
                    {
                        minPOS = 1;
                        maxPOS = int.Parse(Request["IDs"].ToString().Split('-').First());
                    }
                    bool validInput = true;
                    StoreControllerSettings storeControllerSettings = GetList<StoreControllerSettings>(uow, new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStoreOid)).FirstOrDefault();

                    string receiptSufix = "_Receipt";
                    string proformaSufix = "_Proforma";
                    string depositSufix = "_Deposit";
                    string withdrawSufix = "_Withdraw";
                    string specialProformaSufix = "_SpecialProforma";
                    string prefix = storeControllerSettings.Store.Code + "_";
                    //if (maxPOS > storeControllerSettings.MaximumNumberOfPOS)
                    //{
                    //    maxPOS = storeControllerSettings.MaximumNumberOfPOS;
                    //}

                    if (maxPOS > storeControllerSettings.MaximumNumberOfPOS || minPOS > maxPOS)
                    {
                        validInput = false;
                    }

                    eFiscalDevice FiscalDevice = (eFiscalDevice)Enum.Parse(typeof(eFiscalDevice), Request["FiscalDevice"]);
                    eForcedWithdrawMode ForcedWithdrawMode = (eForcedWithdrawMode)Enum.Parse(typeof(eForcedWithdrawMode), Request["ForcedWithdrawMode"]);
                    decimal forcedWithdrawCashAmountLimit = 0;
                    decimal.TryParse(Request["ForcedWithdrawCashAmountLimit"], out forcedWithdrawCashAmountLimit);

                    Guid ReceiptFormat = Guid.Parse(Request["ReceiptFormat"]);
                    Guid XFormat = Guid.Parse(Request["XFormat"]);
                    Guid ZFormat = Guid.Parse(Request["ZFormat"]);
                    Guid POSKeysLayout = String.IsNullOrEmpty(Request["POSKeysLayout"]) ? Guid.Empty : Guid.Parse(Request["POSKeysLayout"]);
                    Guid POSLayout = String.IsNullOrEmpty(Request["POSLayout"]) ? Guid.Empty : Guid.Parse(Request["POSLayout"]);
                    Guid DefaultPaymentMethod = Guid.Parse(Request["DefaultPaymentMethod"]);
                    Guid POSActionLevelsSet;
                    Guid.TryParse(Request["POSActionLevelsSet"], out POSActionLevelsSet);

                    if (validInput)
                    {
                        XPCollection<Model.POS> posses = GetList<Model.POS>(uow);


                        XPCollection<POSDevice> posDevices = null;
                        if (!String.IsNullOrEmpty(Request["POSDevices"]))
                        {
                            List<string> posDevicesOids = Request["POSDevices"].Split(',').ToList();
                            List<Guid> posDeviceGuids = new List<Guid>();
                            foreach (string posOid in posDevicesOids)
                            {
                                Guid tempGuid;
                                if (!Guid.TryParse(posOid, out tempGuid))
                                {
                                    throw new Exception(Resources.AnErrorOccurred);
                                }
                                else
                                {
                                    posDeviceGuids.Add(tempGuid);
                                }
                            }
                            posDevices = GetList<POSDevice>(uow, new InOperator("Oid", posDeviceGuids));
                        }

                        for (int i = minPOS; i <= maxPOS; i++)
                        {
                            string posName = "POS" + i;
                            string posNameWithPrefix = prefix + posName;
                            Model.POS pos = uow.FindObject<Model.POS>(new BinaryOperator("ID", i));
                            if (pos == null)
                            {
                                pos = new Model.POS(uow);
                                pos.Name = posName;
                                pos.ID = i;

                                pos.ABCDirectory = Request["ABCDirectory"].ToString();
                                bool UsesTouchScreen = false;
                                if (Boolean.TryParse(Request["UsesTouchScreen"], out UsesTouchScreen) == false)
                                {
                                    UsesTouchScreen = false;
                                }
                                bool UsesKeyLock = false;
                                if (Boolean.TryParse(Request["UsesKeyLock"], out UsesKeyLock) == false)
                                {
                                    UsesKeyLock = false;
                                }

                                bool AutoFocus = false;
                                if (Boolean.TryParse(Request["AutoFocus"], out AutoFocus) == false)
                                {
                                    AutoFocus = false;
                                }

                                bool EnableLowEndMode = false;
                                if (Boolean.TryParse(Request["EnableLowEndMode"], out EnableLowEndMode) == true)
                                {
                                    EnableLowEndMode = true;
                                }

                                bool DemoMode = false;
                                if (Boolean.TryParse(Request["DemoMode"], out DemoMode) == true)
                                {
                                    DemoMode = true;
                                }

                                bool AsksForStartingAmount = false;
                                if (Boolean.TryParse(Request["AsksForStartingAmount"], out AsksForStartingAmount) == false)
                                {
                                    AsksForStartingAmount = false;
                                }

                                bool OnIssueXClosePacketOnCreditDevice = false;
                                if (Boolean.TryParse(Request["OnIssueXClosePacketOnCreditDevice"], out OnIssueXClosePacketOnCreditDevice) == false)
                                {
                                    OnIssueXClosePacketOnCreditDevice = false;
                                }

                                bool AsksForFinalAmount = false;
                                if (Boolean.TryParse(Request["AsksForFinalAmount"], out AsksForFinalAmount) == false)
                                {
                                    AsksForFinalAmount = false;
                                }

                                bool PrintDiscountAnalysis = false;
                                if (Boolean.TryParse(Request["PrintDiscountAnalysis"], out PrintDiscountAnalysis) == false)
                                {
                                    PrintDiscountAnalysis = false;
                                }
                                pos.FiscalDevice = FiscalDevice;

                                pos.UsesTouchScreen = UsesTouchScreen;
                                pos.UsesKeyLock = UsesKeyLock;
                                pos.AutoFocus = AutoFocus;
                                pos.EnableLowEndMode = EnableLowEndMode;
                                pos.DemoMode = DemoMode;
                                pos.AsksForStartingAmount = AsksForStartingAmount;
                                pos.AsksForFinalAmount = AsksForFinalAmount;
                                pos.PrintDiscountAnalysis = PrintDiscountAnalysis;
                                pos.ReceiptFormat = uow.GetObjectByKey<POSPrintFormat>(ReceiptFormat);
                                pos.XFormat = uow.GetObjectByKey<POSPrintFormat>(XFormat);
                                pos.ZFormat = uow.GetObjectByKey<POSPrintFormat>(ZFormat);
                                pos.ForcedWithdrawMode = ForcedWithdrawMode;
                                pos.ForcedWithdrawCashAmountLimit = forcedWithdrawCashAmountLimit;

                                if (POSKeysLayout != Guid.Empty)
                                {
                                    pos.POSKeysLayout = uow.GetObjectByKey<POSKeysLayout>(POSKeysLayout);
                                }
                                if (POSLayout != Guid.Empty)
                                {
                                    pos.POSLayout = uow.GetObjectByKey<POSLayout>(POSLayout);
                                }
                                if (POSActionLevelsSet != Guid.Empty)
                                {
                                    pos.POSActionLevelsSet = uow.GetObjectByKey<POSActionLevelsSet>(POSActionLevelsSet);
                                }

                                pos.DefaultPaymentMethod = uow.GetObjectByKey<PaymentMethod>(DefaultPaymentMethod);
                                pos.DefaultCustomer = storeControllerSettings.DefaultCustomer;

                                DocumentSeries receiptDocumentSeries = null;
                                IEnumerable<DocumentSeries> documentSeries = allDocumentSeries.Where(g => g.Description == posNameWithPrefix + receiptSufix);
                                if (documentSeries.Count() > 0)
                                {
                                    receiptDocumentSeries = documentSeries.First();
                                }

                                DocumentSeries depositDocumentSeries = null;
                                documentSeries = allDocumentSeries.Where(g => g.Description == posNameWithPrefix + depositSufix);
                                if (documentSeries.Count() > 0)
                                {
                                    depositDocumentSeries = documentSeries.First();
                                }

                                DocumentSeries proformaDocumentSeries = null;
                                documentSeries = allDocumentSeries.Where(g => g.Description == posNameWithPrefix + proformaSufix);
                                if (documentSeries.Count() > 0)
                                {
                                    proformaDocumentSeries = documentSeries.First();
                                }

                                DocumentSeries withdrawalDocumentSeries = null;
                                documentSeries = allDocumentSeries.Where(g => g.Description == posNameWithPrefix + withdrawSufix);
                                if (documentSeries.Count() > 0)
                                {
                                    withdrawalDocumentSeries = documentSeries.First();
                                }

                                DocumentSeries specialProformaDocumentSeries = null;
                                documentSeries = allDocumentSeries.Where(g => g.Description == posNameWithPrefix + specialProformaSufix);
                                if (documentSeries.Count() > 0)
                                {
                                    specialProformaDocumentSeries = documentSeries.First();
                                }

                                if (pos.DefaultDocumentSeries == null || pos.DefaultDocumentSeries.Oid == Guid.Empty)
                                {
                                    receiptDocumentSeries.eModule = eModule.POS;
                                    receiptDocumentSeries.POS = pos;
                                    receiptDocumentSeries.Save();
                                    pos.DefaultDocumentSeries = receiptDocumentSeries;
                                }
                                if (pos.DepositDocumentSeries == null || pos.DepositDocumentSeries.Oid == Guid.Empty)
                                {
                                    depositDocumentSeries.eModule = eModule.POS;
                                    depositDocumentSeries.POS = pos;
                                    depositDocumentSeries.Save();
                                    pos.DepositDocumentSeries = depositDocumentSeries;
                                }
                                if (pos.ProFormaInvoiceDocumentSeries == null || pos.ProFormaInvoiceDocumentSeries.Oid == Guid.Empty)
                                {
                                    proformaDocumentSeries.eModule = eModule.POS;
                                    proformaDocumentSeries.POS = pos;
                                    proformaDocumentSeries.Save();
                                    pos.ProFormaInvoiceDocumentSeries = proformaDocumentSeries;
                                }

                                if (pos.WithdrawalDocumentSeries == null || pos.WithdrawalDocumentSeries.Oid == Guid.Empty)
                                {
                                    withdrawalDocumentSeries.eModule = eModule.POS;
                                    withdrawalDocumentSeries.POS = pos;
                                    withdrawalDocumentSeries.Save();
                                    pos.WithdrawalDocumentSeries = withdrawalDocumentSeries;
                                }

                                if (pos.SpecialProformaDocumentSeries == null || pos.SpecialProformaDocumentSeries.Oid == Guid.Empty)
                                {
                                    specialProformaDocumentSeries.eModule = eModule.POS;
                                    specialProformaDocumentSeries.POS = pos;
                                    specialProformaDocumentSeries.Save();
                                    pos.SpecialProformaDocumentSeries = specialProformaDocumentSeries;
                                }

                                pos.Store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                                pos.DefaultDocumentStatus = storeControllerSettings.DefaultDocumentStatus;

                                pos.DefaultDocumentType = storeControllerSettings.ReceiptDocumentType;
                                pos.ProFormaInvoiceDocumentType = storeControllerSettings.ProformaDocumentType;
                                pos.WithdrawalItem = storeControllerSettings.WithdrawalItem;
                                pos.WithdrawalDocumentType = storeControllerSettings.WithdrawalDocumentType;
                                pos.DepositItem = storeControllerSettings.DepositItem;
                                pos.DepositDocumentType = storeControllerSettings.DepositDocumentType;
                                pos.SpecialProformaDocumentType = storeControllerSettings.SpecialProformaDocumentType;

                                if (posDevices != null && posDevices.Count > 0)
                                {
                                    List<Guid> existingTerminalDevicesGuids = pos.TerminalDeviceAssociations.Select(tda => tda.TerminalDevice.Oid).ToList();
                                    foreach (POSDevice posDevice in posDevices)
                                    {
                                        if (existingTerminalDevicesGuids.Contains(posDevice.Oid) == false)
                                        {
                                            TerminalDeviceAssociation terminalDeviceAssociation = new TerminalDeviceAssociation(uow);
                                            terminalDeviceAssociation.Terminal = pos;
                                            terminalDeviceAssociation.TerminalDevice = posDevice;
                                            pos.TerminalDeviceAssociations.Add(terminalDeviceAssociation);
                                        }
                                    }
                                }
                                pos.Save();
                            }
                        }

                        XpoHelper.CommitChanges(uow);
                        success = true;
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else if (maxPOS > storeControllerSettings.MaximumNumberOfPOS)
                    {
                        Session["Error"] = String.Format("{0} {1} {2}", Resources.InvalidPOSIDS, "Max = ", storeControllerSettings.MaximumNumberOfPOS);
                    }
                    else
                    {
                        Session["Error"] = Resources.InvalidPOSIDS;
                    }
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
                success = false;
            }
            return Json(new { success = success });
        }

        public JsonResult jsonCheckPOSDatabaseRunning()
        {
            Session["KeepAlive"] = DateTime.Now;
            if (POSMasterDBPreparationHelper.IsProcessing)
            {
                return Json(new { done = false });
            }
            else
            {
                return Json(new { done = true });
            }

        }

        public void CreatePOSDatabaseThread(object param)
        {
            if (!POSMasterDBPreparationHelper.IsProcessing)
            {
                POSMasterDBPreparationHelper.PreparePOSMaster(StoreControllerAppiSettings.CurrentStoreOid, Server.MapPath("~/POS"));
            }
        }

        public JsonResult CreatePOSDatabase()
        {
            bool success = false;
            try
            {
                Thread thread = new Thread(CreatePOSDatabaseThread);
                thread.Start();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new { success = success });
        }

        public ActionResult POSDeviceGrid()
        {
            return PartialView(GetList<POSDevice>(XpoHelper.GetNewUnitOfWork()));
        }

        private void ReloadScales()
        {
            string message;
            POSHelper.ReloadScales(XpoSession, out message, StoreControllerAppiSettings.CurrentStore);
            if (string.IsNullOrEmpty(message) == false)
            {
                MvcApplication.WRMLogModule.Log(message);
                Session["Error"] = message;
            }
        }

        public ActionResult POSReportSettingsGrid()
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;

            FillLookupComboBoxes();
            return PartialView(pos.POSReportSettings);
        }

        [HttpPost]
        public ActionResult AddPOSReportSettings([ModelBinder(typeof(RetailModelBinder))] POSReportSetting posReportSetting)
        {
            ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];
            if (ModelState.IsValid)
            {
                try
                {
                    POSReportSetting reportSettings = new POSReportSetting(pos.Session);
                    reportSettings.GetData(posReportSetting, new List<string>() { "Session" });
                    reportSettings.POS = pos;

                    Guid documentTypeGuid;
                    Guid.TryParse(Request["ReportDocumentType_VI"], out documentTypeGuid);
                    reportSettings.DocumentType = reportSettings.Session.GetObjectByKey<DocumentType>(documentTypeGuid);

                    Guid reportGuid;
                    Guid.TryParse(Request["ThermalReport_VI"], out reportGuid);
                    reportSettings.PrintFormat = reportSettings.Session.GetObjectByKey<POSPrintFormat>(reportGuid);

                    Guid customReportGuid;
                    Guid.TryParse(Request["WindowsReport_VI"], out customReportGuid);
                    reportSettings.Report = reportSettings.Session.GetObjectByKey<CustomReport>(customReportGuid);

                    Guid printerGuid;
                    Guid.TryParse(Request["SelectedPrinter_VI"], out printerGuid);
                    reportSettings.Printer = reportSettings.Session.GetObjectByKey<POSDevice>(printerGuid);

                    reportSettings.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                string errorMessage = String.Empty;
                foreach (ModelState state in ModelState.Values)
                {
                    string message = String.Empty;
                    foreach (ModelError modelError in state.Errors)
                    {
                        if (!String.IsNullOrEmpty(modelError.ErrorMessage))
                        {
                            message += modelError.ErrorMessage;
                        }
                        if (modelError.Exception != null)
                        {
                            if (!String.IsNullOrEmpty(modelError.Exception.Message))
                            {
                                message += " " + modelError.Exception.Message;
                            }
                            if (modelError.Exception.InnerException != null && !String.IsNullOrEmpty(modelError.Exception.InnerException.Message))
                            {
                                message += " " + modelError.Exception.InnerException.Message;
                            }
                        }
                        errorMessage += message + Environment.NewLine;
                    }
                }
                Session["Error"] = errorMessage;
            }

            FillLookupComboBoxes();
            return PartialView("POSReportSettingsGrid", pos.POSReportSettings);
        }

        public ActionResult DeletePOSReportSettings([ModelBinder(typeof(RetailModelBinder))] POSReportSetting posReportSetting)
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;

            POSReportSetting deletePOSReportSetting = pos.POSReportSettings.Where(reportSettings => reportSettings.Oid == posReportSetting.Oid).FirstOrDefault();

            if (deletePOSReportSetting != null)
            {
                deletePOSReportSetting.Delete();
            }

            FillLookupComboBoxes();
            return PartialView("POSReportSettingsGrid", pos.POSReportSettings);
        }

        public ActionResult UpdatePOSReportSettings([ModelBinder(typeof(RetailModelBinder))] POSReportSetting posReportSetting)
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;

            POSReportSetting currentPOSReportSetting = pos.POSReportSettings.Where(reportSettings => reportSettings.Oid == posReportSetting.Oid).FirstOrDefault();

            if (currentPOSReportSetting != null)
            {
                try
                {
                    currentPOSReportSetting.GetData(posReportSetting, new List<string>() { "Session" });
                    currentPOSReportSetting.POS = pos;

                    Guid documentTypeGuid;
                    Guid.TryParse(Request["ReportDocumentType_VI"], out documentTypeGuid);
                    currentPOSReportSetting.DocumentType = currentPOSReportSetting.Session.GetObjectByKey<DocumentType>(documentTypeGuid);

                    Guid reportGuid;
                    Guid.TryParse(Request["ThermalReport_VI"], out reportGuid);
                    currentPOSReportSetting.PrintFormat = currentPOSReportSetting.Session.GetObjectByKey<POSPrintFormat>(reportGuid);

                    Guid customReportGuid;
                    Guid.TryParse(Request["WindowsReport_VI"], out customReportGuid);
                    currentPOSReportSetting.Report = currentPOSReportSetting.Session.GetObjectByKey<CustomReport>(customReportGuid);

                    Guid printerGuid;
                    Guid.TryParse(Request["SelectedPrinter_VI"], out printerGuid);
                    currentPOSReportSetting.Printer = currentPOSReportSetting.Session.GetObjectByKey<POSDevice>(printerGuid);

                    currentPOSReportSetting.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }

            FillLookupComboBoxes();
            return PartialView("POSReportSettingsGrid", pos.POSReportSettings);
        }

        public ActionResult POSOposReportSettingsGrid()
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;
            PersistentCriteriaEvaluationBehavior behavior = PersistentCriteriaEvaluationBehavior.BeforeTransaction;
            List<PosReport> posreports = new XPCollection<PosReport>(pos.Session, CriteriaOperator.And(new BinaryOperator("IsActive", true))).ToList();
            ViewBag.PosReports = posreports;
            FillLookupComboBoxes();
            return PartialView(pos.PosOposReportSettings);
        }

        [HttpPost]
        public ActionResult AddPOSOposReportSettings([ModelBinder(typeof(RetailModelBinder))] PosOposReportSettings posReportSetting)
        {
            ITS.Retail.Model.POS pos = (ITS.Retail.Model.POS)Session["UnsavedPOS"];

            if (ModelState.IsValid)
            {
                try
                {
                    PosOposReportSettings reportSettings = new PosOposReportSettings(pos.Session);
                    reportSettings.GetData(posReportSetting, new List<string>() { "Session" });
                    reportSettings.POS = pos;

                    Guid reportGuid;
                    Guid.TryParse(Request["SelectedPosReport_VI"], out reportGuid);
                    reportSettings.PrintFormat = reportSettings.Session.GetObjectByKey<PosReport>(reportGuid);

                    Guid printerGuid;
                    Guid.TryParse(Request["SelectedPrinter_VI"], out printerGuid);
                    reportSettings.Printer = reportSettings.Session.GetObjectByKey<POSDevice>(printerGuid);
                    reportSettings.PrinterName = reportSettings.Printer.Name;

                    reportSettings.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                string errorMessage = String.Empty;
                foreach (ModelState state in ModelState.Values)
                {
                    string message = String.Empty;
                    foreach (ModelError modelError in state.Errors)
                    {
                        if (!String.IsNullOrEmpty(modelError.ErrorMessage))
                        {
                            message += modelError.ErrorMessage;
                        }
                        if (modelError.Exception != null)
                        {
                            if (!String.IsNullOrEmpty(modelError.Exception.Message))
                            {
                                message += " " + modelError.Exception.Message;
                            }
                            if (modelError.Exception.InnerException != null && !String.IsNullOrEmpty(modelError.Exception.InnerException.Message))
                            {
                                message += " " + modelError.Exception.InnerException.Message;
                            }
                        }
                        errorMessage += message + Environment.NewLine;
                    }
                }
                Session["Error"] = errorMessage;
            }

            FillLookupComboBoxes();
            return PartialView("POSOposReportSettingsGrid", pos.PosOposReportSettings);
        }

        public ActionResult DeletePOSOposReportSettings([ModelBinder(typeof(RetailModelBinder))] PosOposReportSettings posReportSetting)
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;

            PosOposReportSettings deletePOSReportSetting = pos.PosOposReportSettings.Where(x => x.Oid == posReportSetting.Oid).FirstOrDefault();

            if (deletePOSReportSetting != null)
            {
                deletePOSReportSetting.Delete();
            }

            FillLookupComboBoxes();
            return PartialView("POSOposReportSettingsGrid", pos.PosOposReportSettings);
        }

        public ActionResult UpdatePOSOposReportSettings([ModelBinder(typeof(RetailModelBinder))] PosOposReportSettings posReportSetting)
        {
            ITS.Retail.Model.POS pos = Session["UnsavedPOS"] as ITS.Retail.Model.POS;


            PosOposReportSettings currentPOSReportSetting = pos.PosOposReportSettings.Where(x => x.Oid == posReportSetting.Oid).FirstOrDefault();

            if (currentPOSReportSetting != null)
            {
                try
                {
                    currentPOSReportSetting.GetData(posReportSetting, new List<string>() { "Session" });
                    currentPOSReportSetting.POS = pos;

                    Guid reportGuid;
                    Guid.TryParse(Request["SelectedPosReport_VI"], out reportGuid);
                    currentPOSReportSetting.PrintFormat = currentPOSReportSetting.Session.GetObjectByKey<PosReport>(reportGuid);


                    Guid printerGuid;
                    Guid.TryParse(Request["SelectedPrinter_VI"], out printerGuid);
                    currentPOSReportSetting.Printer = currentPOSReportSetting.Session.GetObjectByKey<POSDevice>(printerGuid);
                    currentPOSReportSetting.PrinterName = currentPOSReportSetting.Printer.Name;

                    currentPOSReportSetting.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }

            FillLookupComboBoxes();
            return PartialView("POSOposReportSettingsGrid", pos.PosOposReportSettings);
        }

    }
}
//#endif