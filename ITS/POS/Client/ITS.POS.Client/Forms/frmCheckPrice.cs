using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.POS.Client.Forms;
using ITS.Retail.Platform.Common.Helpers;
using DevExpress.Xpo;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Common.AuxilliaryClasses;

namespace ITS.POS.Client
{
    public partial class frmCheckPrice : frmInputFormBase
    {
        public frmCheckPrice(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            labelControl1.Text = POSClientResources.ItemCodeOrBarcode;
            lblDescription.Text = POSClientResources.DESCRIPTION;
            lblPrice.Text = POSClientResources.PRICE;
            btnClose.Text = POSClientResources.CLOSE;
            lblCode.Text = POSClientResources.CODE;
            lblVatCategory.Text = POSClientResources.VAT_CATEGORY;
            cbIsActive.Text = POSClientResources.ACTIVE;
            edtItemCodeSearch.Focus();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void edtItemCode_Leave(object sender, EventArgs e)
        {
            //edtItemCode.Focus();
        }

        private void edtItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            OwnerApplicationSettings appSettings = config.GetAppSettings();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IItemService itemService = Kernel.GetModule<IItemService>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (UtilsHelper.hKeyMap.ContainsKey(e.KeyCode.ToString()) && (UtilsHelper.hKeyMap[e.KeyCode.ToString()].ActionCode == eActions.CANCEL || UtilsHelper.hKeyMap[e.KeyCode.ToString()].ActionCode == eActions.BACKSPACE))
            {
                actionManager.GetAction(UtilsHelper.hKeyMap[e.KeyCode.ToString()].ActionCode).Execute();
                e.SuppressKeyPress = true;
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    try
                    {
                        string code = edtItemCodeSearch.Text.Trim();
                        bool foundButInactive;
                        KeyValuePair<Item, Barcode> item_bc = itemService.GetItemAndBarcodeByCode(code, true, out foundButInactive, plu: code);

                        if (item_bc.Value == null)
                        {
                            List<POS.Model.Settings.BarcodeType> barcodeTypes = CustomBarcodeHelper.GetMatchingMasks(code,typeof(POS.Model.Master.Item).FullName,sessionManager.GetSession<BarcodeType>());
                            BarcodeParseResult result = CustomBarcodeHelper.ParseCustomBarcode<BarcodeType>(barcodeTypes,
                                code, appSettings.PadBarcodes, appSettings.BarcodeLength, (String.IsNullOrWhiteSpace(appSettings.BarcodePaddingCharacter) ? '0' : appSettings.BarcodePaddingCharacter[0]));

                            item_bc = itemService.GetItemAndBarcodeByCode(result.DecodedCode, config.POSSellsInactiveItems, out foundButInactive, result.PLU);
                        }

                        Barcode bc = item_bc.Value;
                        Item item = item_bc.Key;

                        if (item == null)
                        {
                            medtDescription.Text = code + " - " + POSClientResources.ITEM_NOT_FOUND;
                            edtPrice.Text = "";
                            edtCode.Text = "";
                            cbIsActive.Checked = false;
                            edtVatCategory.Text = "";
                        }
                        else
                        {
                            PriceCatalogPolicy customerPriceCatalogPolicy = null, storePriceCatalogPolicy = null;
                            Customer cust = null;
                            if (appContext.CurrentCustomer != null)
                            {
                                cust = appContext.CurrentCustomer;
                            }
                            else
                            {
                                cust = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
                            }                         
                            customerPriceCatalogPolicy = sessionManager.GetObjectByKey<PriceCatalogPolicy>(cust.PriceCatalogPolicy);
                            Store currentStore = sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid);
                            storePriceCatalogPolicy = sessionManager.GetObjectByKey<PriceCatalogPolicy>(currentStore.DefaultPriceCatalogPolicy);

                            medtDescription.Text = item.Name;
                            edtCode.Text = item.Code;
                            VatCategory vatCat = sessionManager.GetObjectByKey<VatCategory>(item.VatCategory);
                            if (vatCat != null)
                            {
                                edtVatCategory.Text = vatCat.Description;
                            }
                            cbIsActive.Checked = item.IsActive;
                            cbIsActive.ForeColor = item.IsActive ? Color.Green : Color.Red;

                            PriceCatalogDetail pcd;
                            decimal itemHelperGetFinalUnitPrice = itemService.GetUnitPriceFromPolicies(customerPriceCatalogPolicy, storePriceCatalogPolicy,item, out pcd, bc);                            
                            if (itemHelperGetFinalUnitPrice >= 0)
                            {
                                string itemHelperGetFinalUnitPriceToString = itemHelperGetFinalUnitPrice.ToString("C2");
                                edtPrice.Text = itemHelperGetFinalUnitPriceToString;
                            }
                            else
                            {
                                edtPrice.Text = POSClientResources.NOT_FOUND;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Kernel.LogFile.Info(ex, "frmCheckPrice:edtItemCode_KeyDown,Exception catched");
                        IFormManager formManager = Kernel.GetModule<IFormManager>();
                        formManager.ShowMessageBox(ex.GetFullMessage());
                    }
                    finally
                    {
                        edtItemCodeSearch.Text = "";
                        edtItemCodeSearch.Focus();
                    }

                }
            }
        }

        private void edtPrice_Properties_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
        }

        private void frmCheckPrice_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void edtItemCode_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void edtItemCode_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void frmCheckPrice_FormClosed(object sender, FormClosedEventArgs e)
        {
            edtItemCodeSearch.HideTouchPad();
        }

    }
}