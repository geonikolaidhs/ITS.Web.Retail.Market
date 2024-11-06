using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.POS.Model.Master;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace ITS.POS.Client.Forms
{
    public partial class frmFindItemByDesc : frmInputFormBase, IObserverItemGrid, IObserver
    {
        protected BindingList<Item> Items;
        private Customer cust;
        private PriceCatalogPolicy customerPriceCatalogPolicy = null;
        private PriceCatalogPolicy storePriceCatalogPolicy = null;
        public frmFindItemByDesc(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            btnSearch.Text = POSClientResources.SEARCH;
            lblTitle.Text = POSClientResources.ITEM_DESCRIPTION_SEARCH;
            gridColumn2.Caption = POSClientResources.DESCRIPTION;
            gridColumn3.Caption = POSClientResources.PRICE;
            InitBindings();
            edtItemDescription.Focus();
            this.ActionsToObserve = new List<eActions>();
            this.ActionsToObserve.Add(eActions.MOVE_DOWN);
            this.ActionsToObserve.Add(eActions.MOVE_UP);
            foreach (eActions act in ActionsToObserve)
            {
                actionManager.GetAction(act).Attach(this);
            }
        }

        private void InitBindings()
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            Items = new BindingList<Item>();
            blItems.DataSource = Items;
            grdFoundItems.DataSource = Items;

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                IItemService itemService = Kernel.GetModule<IItemService>();
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
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
                this.Items.Clear();
                List<Item> itemList = new List<Item>();
                string operand = null;
                if (string.IsNullOrWhiteSpace(edtItemDescription.Text) == false)
                {
                    itemList = (itemService.GetItemFromDescription(edtItemDescription.Text).ToList());
                    if (itemList.Count > 0)
                    {
                        itemList.ForEach(x => this.Items.Add(x));
                        grdFoundItems.DataSource = this.Items;

                        Update();
                    }
                    else
                    {
                        if (DialogResult.OK == formManager.ShowMessageBox(POSClientResources.ITEM_NOT_FOUND))
                        {
                            edtItemDescription.ResetText();
                        }
                    }
                }
                else
                {
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.INSERT_VALUES);
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IAppContext applicationContext = Kernel.GetModule<IAppContext>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                IItemService itemService = Kernel.GetModule<IItemService>();
                if (this.Items.Count == 0 || string.IsNullOrWhiteSpace(edtItemDescription.Text))
                {
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.INSERT_VALUES);
                }
                else
                {
                    Item item = Items.ElementAt(gridView1.GetSelectedRows()[0]);
                    actionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(item.Code, applicationContext.MainForm.SelectedQty));
                    applicationContext.MainForm.SelectedQty = 1;
                    applicationContext.MainForm.ResetInputText();
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            { }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
        }
        private void edtItemDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
            }
            else if (e.KeyCode == Keys.Left)
            {
                
            }
        }
        private void grdFoundItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && gridView1.SelectedRowsCount > 0)
            {
                btnOK.PerformClick();
            }
        }
        private void gridView1_RowClick(object sender, RowClickEventArgs e)
        {
            IItemService itemService = Kernel.GetModule<IItemService>();
            GridView gridView = sender as GridView;
            try
            {
                Item item = gridView.GetRow(gridView.GetSelectedRows()[0]) as Item;
                bool foundButInactive;
                KeyValuePair<Item, Barcode> itbc = itemService.GetItemAndBarcodeByCode(item.Code, false, out foundButInactive, plu: item.Code);
                PriceCatalogDetail pcd;
                decimal price = itemService.GetUnitPriceFromPolicies(customerPriceCatalogPolicy, storePriceCatalogPolicy, item, out pcd, itbc.Value);
                edtPrice.Text = price.ToString("C2");
                btnOK.Focus();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        public override void Update(ItemGridParams parameters)
        {
            if (parameters.Navigation != null)
            {
                switch (parameters.Navigation)
                {
                    case eNavigation.MOVEUP:
                        if (gridView1.FocusedRowHandle > 0)
                        {
                            gridView1.FocusedRowHandle--;

                        }
                        break;
                    case eNavigation.MOVEDOWN:
                        if (gridView1.FocusedRowHandle >= 0 && gridView1.FocusedRowHandle < (gridView1.RowCount - 1))
                        {
                            gridView1.FocusedRowHandle++;
                        }
                        break;
                }
            }
        }
        public override Type[] GetParamsTypes()
        {
            return new Type[] { typeof(ItemGridParams) };
        }
        [Browsable(false)]
        public List<eActions> ActionsToObserve
        {
            get;
            set;
        }
        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            IItemService itemService = Kernel.GetModule<IItemService>();
            try
            {
                GridView gridView = sender as GridView;
                Item item = gridView.GetRow(gridView.GetSelectedRows()[0]) as Item;
                bool foundButInactive;
                KeyValuePair<Item, Barcode> itbc = itemService.GetItemAndBarcodeByCode(item.Code, false, out foundButInactive, plu: item.Code);
                PriceCatalogDetail pcd;
                decimal price = itemService.GetUnitPriceFromPolicies(customerPriceCatalogPolicy, storePriceCatalogPolicy, item, out pcd, itbc.Value);
                edtPrice.Text = price.ToString("C2");
                btnOK.Focus();
            }
            catch (Exception ex)
            { }
        }

        private void edtItemDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right);
        }
    }
}