using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Handles  an action request by the user.
    /// </summary>
    public class ActionExecutor : IActionExecutor
    {
        IAppContext AppContext { get; set; }
        ISessionManager SessionManager { get; set; }
        IConfigurationManager Configuration { get; set; }
        IDeviceManager DeviceManager { get; set; }
        IActionManager ActionManager { get; set; }

        public ActionExecutor(IAppContext appContext, ISessionManager sessionManager, IConfigurationManager config, IDeviceManager deviceManager, IActionManager actionManager)
        {
            this.AppContext = appContext;
            this.SessionManager = sessionManager;
            this.Configuration = config;
            this.DeviceManager = deviceManager;
            this.ActionManager = actionManager;
        }

        public void ExecuteAction(eActions actionCode, string actionParameters, bool dontCheckPermissions = false, bool validateMachineStatus = true)
        {
            Dictionary<string, string> parsedParameters = new Dictionary<string, string>();

            if (!String.IsNullOrWhiteSpace(actionParameters))
            {
                parsedParameters = JsonConvert.DeserializeObject(actionParameters, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            }

            switch (actionCode)
            {
                case eActions.DELETE_ITEM:
                    if (AppContext.CurrentDocumentLine != null)
                    {
                        ActionManager.GetAction(eActions.DELETE_ITEM).Execute(new ActionDeleteItemParams(AppContext.CurrentDocumentLine, true), dontCheckPermissions, validateMachineStatus);
                    }
                    break;
                case eActions.ADD_LINE_DISCOUNT:
                    if (AppContext.CurrentDocumentLine != null && AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
                    {
                        if (parsedParameters.ContainsKey("DiscountTypeCode") == false || String.IsNullOrWhiteSpace(parsedParameters["DiscountTypeCode"]))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT_TYPE_CODE), dontCheckPermissions, validateMachineStatus);
                        }
                        else
                        {
                            decimal discount = 0;
                            string input = AppContext.MainForm.InputText.Trim();
                            AppContext.MainForm.ResetInputText();
                            if (decimal.TryParse(input, out discount) == false)
                            {
                                ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT), dontCheckPermissions, validateMachineStatus);
                            }
                            else
                            {
                                DiscountType discountType = SessionManager.FindObject<DiscountType>(CriteriaOperator.And(new BinaryOperator("Code", parsedParameters["DiscountTypeCode"]),
                                                                                            new BinaryOperator("IsHeaderDiscount", false)));
                                if (discountType != null)
                                {
                                    discount = discountType.eDiscountType == eDiscountType.PERCENTAGE ? (discount / 100) : discount;
                                    ActionManager.GetAction(eActions.ADD_LINE_DISCOUNT).Execute(new ActionAddLineDiscountParams(discount, discountType, true), dontCheckPermissions, validateMachineStatus);
                                }
                                else
                                {
                                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.DISCOUNT_TYPE_NOT_FOUND + " ('" + parsedParameters["DiscountTypeCode"] + "')"), dontCheckPermissions, validateMachineStatus);
                                }
                            }
                        }

                    }
                    break;
                case eActions.ADD_DOCUMENT_DISCOUNT:
                    if (AppContext.CurrentDocument != null && AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        if (parsedParameters.ContainsKey("DiscountTypeCode") == false || String.IsNullOrWhiteSpace(parsedParameters["DiscountTypeCode"]))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT_TYPE_CODE), dontCheckPermissions, validateMachineStatus);
                        }
                        else
                        {
                            decimal discount = 0;
                            string input = AppContext.MainForm.InputText.Trim();
                            AppContext.MainForm.ResetInputText();
                            if (decimal.TryParse(input, out discount) == false)
                            {
                                ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT), dontCheckPermissions, validateMachineStatus);
                            }
                            else
                            {
                                DiscountType discountType = SessionManager.FindObject<DiscountType>(CriteriaOperator.And(new BinaryOperator("Code", parsedParameters["DiscountTypeCode"]),
                                                                                                                        new BinaryOperator("IsHeaderDiscount", true)));
                                if (discountType != null)
                                {
                                    discount = discountType.eDiscountType == eDiscountType.PERCENTAGE ? (discount / 100) : discount;
                                    ActionManager.GetAction(eActions.ADD_DOCUMENT_DISCOUNT).Execute(new ActionAddDocumentDiscountParams(discount, discountType), dontCheckPermissions, validateMachineStatus);
                                }
                                else
                                {
                                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.DISCOUNT_TYPE_NOT_FOUND + " ('" + parsedParameters["DiscountTypeCode"] + "')"), dontCheckPermissions, validateMachineStatus);
                                }
                            }
                        }

                    }
                    break;
                case eActions.ADD_LINE_DISCOUNT_FROM_FORM:
                    {
                        decimal discount = 0;
                        string input = AppContext.MainForm.InputText.Trim();
                        AppContext.MainForm.ResetInputText();
                        if (decimal.TryParse(input, out discount) == false)
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT), true, validateMachineStatus);
                        }
                        else
                        {
                            ActionManager.GetAction(eActions.ADD_LINE_DISCOUNT_FROM_FORM).Execute(new ActionAddLineDiscountFromFormParams(discount), dontCheckPermissions, validateMachineStatus);
                        }
                    }
                    break;
                case eActions.ADD_DOCUMENT_DISCOUNT_FROM_FORM:
                    {
                        decimal discount = 0;
                        string input = AppContext.MainForm.InputText.Trim();
                        AppContext.MainForm.ResetInputText();
                        if (decimal.TryParse(input, out discount) == false)
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_DISCOUNT), true, validateMachineStatus);
                        }
                        else
                        {
                            ActionManager.GetAction(eActions.ADD_DOCUMENT_DISCOUNT_FROM_FORM).Execute(new ActionAddDocumentDiscountFromFormParams(discount), dontCheckPermissions, validateMachineStatus);
                        }
                    }
                    break;
                case eActions.DELETE_PAYMENT:
                    if (AppContext.CurrentDocumentPayment != null && AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        ActionManager.GetAction(eActions.DELETE_PAYMENT).Execute(new ActionDeletePaymentParams(AppContext.CurrentDocumentPayment), dontCheckPermissions, validateMachineStatus);
                    }
                    break;
                case eActions.FORCE_DELETE_PAYMENT:
                    if (AppContext.CurrentDocumentPayment != null && AppContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        ActionManager.GetAction(eActions.FORCE_DELETE_PAYMENT).Execute(new ActionForceDeletePaymentParams(AppContext.CurrentDocumentPayment), dontCheckPermissions, validateMachineStatus);
                    }
                    break;
                case eActions.CALL_OTHER_ACTION:
                    if (AppContext.GetMachineStatus() == eMachineStatus.DAYSTARTED || AppContext.GetMachineStatus() == eMachineStatus.CLOSED)
                    {
                        AppContext.SplashForm.cmdTextBox.BackColor = (System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(155)))), ((int)(((byte)(92))))));
                        AppContext.SplashForm.ShowSpecialCommandBar = true;
                    }
                    else
                    {
                        AppContext.MainForm.SetInputBackground(System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(155)))), ((int)(((byte)(92))))));
                        AppContext.MainForm.Update(new MessengerParams(ucPOSInput.commandText));
                        AppContext.MainForm.waitSpecialCommand = true;
                    }
                    break;
                case eActions.ADD_ITEM:
                    if (parsedParameters.ContainsKey("ItemCode"))
                    {
                        ActionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(parsedParameters["ItemCode"], AppContext.MainForm.SelectedQty), dontCheckPermissions, validateMachineStatus);
                    }
                    else
                    {
                        ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.NO_ITEM_CODE_DEFINED), dontCheckPermissions, validateMachineStatus);
                    }
                    AppContext.MainForm.SelectedQty = 1;
                    AppContext.MainForm.ResetInputText();
                    break;
                case eActions.ADD_ITEM_WEIGHTED:
                    ////If the item code is assigned on the keybind then use that code, else get the code from the InputText
                    string code = "";
                    if (parsedParameters.ContainsKey("ItemCode") && String.IsNullOrWhiteSpace(parsedParameters["ItemCode"]) == false)
                    {
                        code = parsedParameters["ItemCode"];
                    }
                    else
                    {
                        code = AppContext.MainForm.InputText;
                    }
                    ActionManager.GetAction(eActions.ADD_ITEM_WEIGHTED).Execute(new ActionAddWeightedItemParams(code), dontCheckPermissions, validateMachineStatus);
                    AppContext.MainForm.SelectedQty = 1;
                    AppContext.MainForm.ResetInputText();
                    break;
                case eActions.ADD_PAYMENT_FROM_FORM:
                    //if (GlobalContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        decimal amount;
                        if (!decimal.TryParse(AppContext.MainForm.InputText, out amount))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT), dontCheckPermissions, validateMachineStatus);
                        }
                        else
                        {
                            ActionManager.GetAction(eActions.ADD_PAYMENT_FROM_FORM).Execute(new ActionAddPaymentFromFormParams(amount), dontCheckPermissions, validateMachineStatus);
                        }
                        AppContext.MainForm.ResetInputText();
                    }
                    break;
                case eActions.ADD_TOTAL_PAYMENT_FROM_FORM:
                    {
                        decimal amount;
                        decimal? finalAmount = null;
                        if (decimal.TryParse(AppContext.MainForm.InputText, out amount))
                        {
                            finalAmount = amount;
                        }
                        else if (!String.IsNullOrWhiteSpace(AppContext.MainForm.InputText))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT), dontCheckPermissions, validateMachineStatus);
                            return;
                        }

                        ActionManager.GetAction(eActions.ADD_TOTAL_PAYMENT_FROM_FORM).Execute(new ActionAddTotalPaymentFromFormParams(finalAmount), dontCheckPermissions, validateMachineStatus);
                        AppContext.MainForm.ResetInputText();
                    }
                    break;
                case eActions.ADD_TOTAL_PAYMENT:
                    //if (GlobalContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        decimal amount;
                        decimal? finalAmount = null;
                        if (decimal.TryParse(AppContext.MainForm.InputText, out amount))
                        {
                            finalAmount = amount;
                        }
                        else if (!String.IsNullOrWhiteSpace(AppContext.MainForm.InputText))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT), dontCheckPermissions, validateMachineStatus);
                            return;
                        }

                        if (parsedParameters.ContainsKey("PaymentMethodCode"))
                        {
                            PaymentMethod paymentMethod = SessionManager.FindObject<PaymentMethod>(new BinaryOperator("Code", parsedParameters["PaymentMethodCode"]));
                            if (paymentMethod != null)
                            {
                                ActionManager.GetAction(eActions.ADD_TOTAL_PAYMENT).Execute(new ActionAddTotalPaymentParams(paymentMethod, finalAmount), dontCheckPermissions, validateMachineStatus);
                            }
                            else
                            {
                                ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAYMENT_METHOD_NOT_FOUND + " ('" + parsedParameters["PaymentMethodCode"] + "')"), dontCheckPermissions, validateMachineStatus);
                            }
                        }
                        else
                        {
                            ActionManager.GetAction(eActions.ADD_TOTAL_PAYMENT).Execute(new ActionAddTotalPaymentParams(SessionManager.GetObjectByKey<PaymentMethod>(Configuration.DefaultPaymentMethodOid), finalAmount), dontCheckPermissions, validateMachineStatus);
                        }

                        AppContext.MainForm.ResetInputText();
                    }
                    break;
                case eActions.ADD_PAYMENT:
                    //if (GlobalContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        decimal amount;
                        if (!decimal.TryParse(AppContext.MainForm.InputText, out amount))
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT), dontCheckPermissions, validateMachineStatus);
                        }
                        else
                        {
                            if (parsedParameters.ContainsKey("PaymentMethodCode"))
                            {
                                PaymentMethod paymentMethod = SessionManager.FindObject<PaymentMethod>(new BinaryOperator("Code", parsedParameters["PaymentMethodCode"]));
                                if (paymentMethod != null)
                                {
                                    ActionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(paymentMethod, amount), dontCheckPermissions, validateMachineStatus);
                                }
                                else
                                {
                                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAYMENT_METHOD_NOT_FOUND + " ('" + parsedParameters["PaymentMethodCode"] + "')"), dontCheckPermissions, validateMachineStatus);
                                }
                            }
                            else
                            {
                                ActionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(SessionManager.GetObjectByKey<PaymentMethod>(Configuration.DefaultPaymentMethodOid), amount), dontCheckPermissions, validateMachineStatus);
                            }
                        }
                        AppContext.MainForm.ResetInputText();
                    }
                    break;
                case eActions.OPEN_DRAWER:
                    ActionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(DeviceManager.GetPrimaryDevice<Drawer>(), true), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.RETURN_ITEM:
                    if (AppContext.GetMachineStatus() == eMachineStatus.SALE)
                    {
                        ActionManager.GetAction(eActions.START_NEW_DOCUMENT).Execute(new ActionStartNewDocumentParams(true), dontCheckPermissions, validateMachineStatus);
                    }
                    ActionManager.GetAction(eActions.RETURN_ITEM).Execute(new ActionReturnItemParams(AppContext.MainForm.InputText, AppContext.MainForm.SelectedQty, AppContext.MainForm.GetMainInput().FromScanner), dontCheckPermissions, validateMachineStatus);
                    AppContext.MainForm.GetMainInput().FromScanner = false;
                    ActionManager.GetAction(eActions.PUBLISH_LINE_QUANTITY_INFO).Execute(new ActionPublishLineQuantityInfoParams(1, ""), dontCheckPermissions, validateMachineStatus);
                    AppContext.MainForm.SelectedQty = 1;
                    AppContext.MainForm.ResetInputText();
                    break;
                case eActions.CHANGE_ITEM_PRICE:
                    if (AppContext.CurrentDocumentLine != null)
                    {
                        ActionManager.GetAction(eActions.CHANGE_ITEM_PRICE).Execute(new ActionChangeItemPriceParams(AppContext.CurrentDocumentLine), dontCheckPermissions, validateMachineStatus);
                    }
                    break;
                case eActions.STRESS_TEST:
                    int numberOfReceipts = 100;
                    int itemsPerReceipt = 1;
                    bool randomCustomer = false;
                    bool randomPayment = false;
                    bool randomCancelLines = false;
                    bool randomCancelDocument = false;
                    bool randomProforma = false;
                    if (parsedParameters.ContainsKey("NumberOfReceipts"))
                    {
                        int.TryParse(parsedParameters["NumberOfReceipts"], out numberOfReceipts);
                    }
                    if (parsedParameters.ContainsKey("ItemsPerReceipt"))
                    {
                        int.TryParse(parsedParameters["ItemsPerReceipt"], out itemsPerReceipt);
                    }
                    if (parsedParameters.ContainsKey("RandomCustomer"))
                    {
                        bool.TryParse(parsedParameters["RandomCustomer"], out randomCustomer);
                    }
                    if (parsedParameters.ContainsKey("RandomPayment"))
                    {
                        bool.TryParse(parsedParameters["RandomPayment"], out randomPayment);
                    }
                    if (parsedParameters.ContainsKey("RandomCancelLines"))
                    {
                        bool.TryParse(parsedParameters["RandomCancelLines"], out randomCancelLines);
                    }
                    if (parsedParameters.ContainsKey("RandomCancelDocument"))
                    {
                        bool.TryParse(parsedParameters["RandomCancelDocument"], out randomCancelDocument);
                    }
                    if (parsedParameters.ContainsKey("RandomProforma"))
                    {
                        bool.TryParse(parsedParameters["RandomProforma"], out randomProforma);
                    }

                    ActionManager.GetAction(eActions.STRESS_TEST).Execute(new ActionStressTestParams(numberOfReceipts,
                                                                        itemsPerReceipt, randomCustomer, randomPayment
                                                                        , randomCancelLines, randomCancelDocument, randomProforma),
                                                                        dontCheckPermissions, validateMachineStatus);

                    break;
                case eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS:
                    bool useDateTimeFilter = false;
                    eReprintZReportsMode mode = eReprintZReportsMode.ANALYTIC;

                    if (parsedParameters.ContainsKey("UseDateFilter"))
                    {
                        bool.TryParse(parsedParameters["UseDateFilter"], out useDateTimeFilter);
                    }

                    if (parsedParameters.ContainsKey("Mode"))
                    {
                        Enum.TryParse(parsedParameters["Mode"], true, out mode);
                    }


                    ActionManager.GetAction(eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS).Execute(new ActionFiscalPrinterReprintZReportsParams(useDateTimeFilter, mode), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.SET_FISCAL_ON_ERROR:
                    bool setOnError = true;
                    if (parsedParameters.ContainsKey("SetFiscalOnError"))
                    {
                        bool.TryParse(parsedParameters["SetFiscalOnError"], out setOnError);
                    }
                    ActionManager.GetAction(eActions.SET_FISCAL_ON_ERROR).Execute(new ActionSetFiscalOnErrorParams(setOnError), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.SET_STANDALONE_FISCAL_ON_ERROR:
                    bool setStandaloneFiscalOnError = true;
                    if (parsedParameters.ContainsKey("SetStandaloneFiscalOnError"))
                    {
                        bool.TryParse(parsedParameters["SetStandaloneFiscalOnError"], out setStandaloneFiscalOnError);
                    }
                    ActionManager.GetAction(eActions.SET_STANDALONE_FISCAL_ON_ERROR).Execute(new ActionSetStandaloneFiscalOnErrorParams(setStandaloneFiscalOnError), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.GENERIC_CANCEL:
                    int modeCode;
                    if (int.TryParse(AppContext.MainForm.InputText, out modeCode))
                    {
                        eActionGenericCancelMode cancelMode;
                        try
                        {
                            cancelMode = (eActionGenericCancelMode)modeCode;
                        }
                        catch
                        {
                            throw new POSException(POSClientResources.INVALID_ACTION);
                        }
                        ActionManager.GetAction(eActions.GENERIC_CANCEL).Execute(new ActionGenericCancelParams(cancelMode), dontCheckPermissions, validateMachineStatus);
                    }
                    AppContext.MainForm.ResetInputText();
                    break;
                case eActions.ISSUE_X:
                    ActionManager.GetAction(eActions.ISSUE_X).Execute(new ActionIssueXReportParams(false), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.ISSUE_Z:
                    ActionManager.GetAction(eActions.ISSUE_Z).Execute(new ActionIssueZReportParams(false, false), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.SERVICE_FORCED_CANCEL_DOCUMENT:
                    ActionManager.GetAction(eActions.SERVICE_FORCED_CANCEL_DOCUMENT).Execute(new ActionServiceForcedCancelDocumentParams(true), dontCheckPermissions, validateMachineStatus);
                    break;
                case eActions.ADD_COUPON:
                    ActionManager.GetAction(eActions.ADD_COUPON).Execute(new ActionAddCouponParams(AppContext.MainForm.InputText));
                    break;
                case eActions.USE_DOCUMENT_TYPE:
                    string documentTypeCode = "";
                    if (parsedParameters.ContainsKey("DocumentTypeCode"))
                    {
                        documentTypeCode = parsedParameters["DocumentTypeCode"];
                    }
                    ActionManager.GetAction(eActions.USE_DOCUMENT_TYPE).Execute(new ActionUserDocumentTypeParams(documentTypeCode));
                    break;
                case eActions.USE_OPOS_REPORT:
                    string posReportCode = "";
                    if (parsedParameters.ContainsKey("PosReportCode"))
                    {
                        posReportCode = parsedParameters["PosReportCode"];
                    }
                    ActionManager.GetAction(eActions.USE_OPOS_REPORT).Execute(new ActionUseOposReportParams(posReportCode));
                    break;
                default:
                    ActionManager.GetAction(actionCode).Execute(dontCheckPermissions: dontCheckPermissions, validateMachineStatus: validateMachineStatus);
                    break;
            }
        }
    }
}
