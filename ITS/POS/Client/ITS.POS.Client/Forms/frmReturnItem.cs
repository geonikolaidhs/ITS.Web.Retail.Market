using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Common.Helpers;
using DevExpress.Xpo;
using ITS.Retail.Platform.Common.AuxilliaryClasses;

namespace ITS.POS.Client.Forms
{
    public partial class frmReturnItem : frmInputFormBase, IObserverContainer
    {
        public frmReturnItem(IPosKernel kernel)
            : base(kernel)
        {
            InitializeComponent();
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            lblTitle.Text = POSClientResources.CODE_OR_BARCODE_OF_ITEM;
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            ucLineQuantity.SetQuantity(AppContext.MainForm.SelectedQty);
            ValidActions.Add(eActions.MULTIPLY_QTY);
        }

        private void frmReturnItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //edtItemCode.HideTouchPad();
        }

        public override void Update(ObserverPattern.ObserverParameters.KeyListenerParams parameters)
        {
            if (parameters.NotificationType == eNotificationsTypes.ACTION && ValidActions.Contains(parameters.ActionCode))
            {
                IActionExecutor actionExecutor = Kernel.GetModule<IActionExecutor>();
                
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    actionExecutor.ExecuteAction(parameters.ActionCode, parameters.ActionParameters);
                }));

                return;
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY && parameters.RedirectTo != Keys.None) //Redirect key detected, Call update with the new key
            {
                this.Update(new KeyListenerParams(eActions.NONE, eNotificationsTypes.KEY, Keys.None, parameters.RedirectTo, parameters.ActionParameters));
            }
            else if (parameters.NotificationType == eNotificationsTypes.KEY)// && keyMapping.ActionCode == eActions.KEYBOARD)
            {
                MethodInfo callMethod = this.GetType().GetMethod(parameters.KeyData.ToString() + "Key");
                if (callMethod != null)
                {
                    try
                    {
                        string textAfter = callMethod.Invoke(this, new object[] { edtItemCode.Text.Trim() }) as string;
                        Application.DoEvents();
                        edtItemCode.Text = textAfter;
                    }
                    catch
                    {
                        callMethod.Invoke(this, null);
                    }
                }
                return;
            }
        }


        public string MultiplyKey(string txt) // Multiply = '*'
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT || appContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    string qtyStr = txt;
                    decimal qty;
                    if (!decimal.TryParse(qtyStr, out qty) || qty <= 0)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_QUANTITY));
                    }
                    else
                    {
                        //actionManager.GetAction(eActions.PUBLISH_LINE_QUANTITY_INFO).Execute(new ActionPublishLineQuantityInfoParams(qty, ""));
                        appContext.MainForm.SelectedQty = qty;
                    }
                }
            }
            catch (Exception e)
            {
                Kernel.LogFile.Info(e, "frmReturnItem:MultiplyKey,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(e.GetFullMessage()));
            }
            return "";
        }

        public string BackKey(string txt)
        {
            return txt.Length != 0 ? txt.Remove(txt.Length - 1, 1) : "";
        }

        public string ReturnKey(string txt)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IItemService itemService = Kernel.GetModule<IItemService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();

            OwnerApplicationSettings appSettings = config.GetAppSettings();

            try
            {
                string code = txt;
                List<BarcodeType> barcodeTypes = CustomBarcodeHelper.GetMatchingMasks(code, typeof(POS.Model.Master.Item).FullName, sessionManager.GetSession<BarcodeType>());
                BarcodeParseResult result = CustomBarcodeHelper.ParseCustomBarcode<BarcodeType>(barcodeTypes,
                    code, appSettings.PadBarcodes, appSettings.BarcodeLength, (String.IsNullOrWhiteSpace(appSettings.BarcodePaddingCharacter) ? '0' : appSettings.BarcodePaddingCharacter[0]));


                bool foundButInactive;
                KeyValuePair<Item, Barcode> item_bc = itemService.GetItemAndBarcodeByCode(code, config.POSSellsInactiveItems, out foundButInactive, result.PLU);
                Item item = item_bc.Key;

                if (item == null)
                {
                    UtilsHelper.HardwareBeep();
                    if (foundButInactive)
                    {
                        formManager.ShowCancelOnlyMessageBox(code + " - " + POSClientResources.ITEM_IS_INACTIVE);
                    }
                    else
                    {
                        formManager.ShowCancelOnlyMessageBox(code + " - " + POSClientResources.ITEM_NOT_FOUND);
                    }
                }
                else
                {
                    actionManager.GetAction(eActions.RETURN_ITEM).Execute(new ActionReturnItemParams(code, appContext.MainForm.SelectedQty, false), true);
                    appContext.MainForm.GetMainInput().FromScanner = false;
                    //actionManager.GetAction(eActions.PUBLISH_LINE_QUANTITY_INFO).Execute(new ActionPublishLineQuantityInfoParams(1, ""));
                    appContext.MainForm.SelectedQty = 1;
                    appContext.MainForm.ResetInputText();
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "frmCheckPrice:edtItemCode_KeyDown,Exception catched");
                formManager.ShowMessageBox(ex.GetFullMessage());
            }
            finally
            {
                edtItemCode.Text = "";
                edtItemCode.Focus();
            }
            return "";
        }

        private bool _subscriptionsAreInitialized;
        public bool SubscriptionsAreInitialized
        {
            get { return _subscriptionsAreInitialized; }
        }

        public void IntitializeSubscriptions()
        {

            if (!SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (typeof(IObserver).IsAssignableFrom(control.GetType()))
                    {
                        ((IObserver)control).InitializeActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = true;
            }
        }

        public void DropSubscriptions()
        {
            if (SubscriptionsAreInitialized)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formManager.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (control is ucObserver)
                    {
                        (control as ucObserver).DropActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReturnKey(this.edtItemCode.Text.ToString());
        }
    }
}
