using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common;
using ITS.Retail.Model.SupportingClasses;
using System.Diagnostics;
using System.IO;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using DevExpress.XtraBars.Ribbon;
using ITS.Retail.DesktopClient.StoreControllerClient.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.Data;
using DevExpress.Utils;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms.Items;
using POSCommandsLibrary;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.PrintServer.Common;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

using ITS.POS.Hardware;
using System.IO.Ports;
using ITS.Retail.WebClient.Helpers.Factories;
using System.Threading;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters;
using System.Globalization;
using ITS.Hardware.RBSPOSEliot.CashRegister;
using DevExpress.XtraReports.UI;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class MainForm : XtraLocalizedRibbonForm
    {
        private ITS.Retail.Model.POS _CashRegisterDevice;
        protected bool ReturnToLoginForm { get; set; }

        protected UnitOfWork GridUnitOfWork { get; set; }

        protected bool IsClosing { get; set; }

        private UnitOfWork _Uow = XpoHelper.GetNewUnitOfWork();

        private List<Guid> SelectedDocumentOids = null;

        protected int DocumentLabelsGridRowCount { get; set; }
        protected bool LastReportSelectionIsHQ { get; set; }

        Dictionary<int, GridView> detailCache = null;
        public MainForm()
        {
            SelectedDocumentOids = new List<Guid>();
            InitializeComponent();
            LocaliseApplication();
            SetBindings();
            LoadPOS();
            FillCboCashierDevices();
            InitializeSearchFilterPanels();
            this.ReturnToLoginForm = false;
            gridControlMain.DefaultViewChanged += DefaultViewChanges;
            grdViewDocuments.EditFormShowing += EditFormShowing;
            grdViewDocuments.DataSourceChanged += DataSourceChanged;
            IsClosing = false;
            navBarItemCashierController.Visible = false;
            if (Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE)
                navBarGroupHQReports.Visible = false;


        }

        private void DefaultViewChanges(object sender, EventArgs e)
        {
            SelectedDocumentOids = new List<Guid>();
        }


        private void EditFormShowing(object sender, EventArgs e)
        {
            SelectedDocumentOids = new List<Guid>();
        }

        private void DataSourceChanged(object sender, EventArgs e)
        {
            SelectedDocumentOids = new List<Guid>();
        }



        private void CheckUserPermissions()
        {
            IEnumerable<RoleEntityAccessPermision> userPermissions = Program.Settings.CurrentUser.Role.RoleEntityAccessPermisions;
            if (userPermissions.Count() > 0)
            {
                EntityAccessPermision userForCustomerPermission = userPermissions.Select(perm => perm.EnityAccessPermision).FirstOrDefault(ent => ent.EntityType == "Customer");
                if (userForCustomerPermission != null)
                {
                    this.barButtonItemEditCustomer.Enabled = userForCustomerPermission.CanUpdate;
                    this.barButtonItemEditCustomer.Visibility = userForCustomerPermission.CanUpdate ? BarItemVisibility.Always : BarItemVisibility.Never;
                    this.barButtonItemAddCustomer.Enabled = userForCustomerPermission.CanInsert;
                    this.barButtonItemAddCustomer.Visibility = userForCustomerPermission.CanInsert ? BarItemVisibility.Always : BarItemVisibility.Never;
                }
                EntityAccessPermision userForSupplierPermission = userPermissions.Select(perm => perm.EnityAccessPermision).FirstOrDefault(ent => ent.EntityType == "Supplier");
                if (userForSupplierPermission != null)
                {
                    this.barButtonItemEditSupplier.Enabled = userForSupplierPermission.CanUpdate;
                    this.barButtonItemEditSupplier.Visibility = userForSupplierPermission.CanUpdate ? BarItemVisibility.Always : BarItemVisibility.Never;
                    this.barButtonItemAddSupplier.Enabled = userForSupplierPermission.CanInsert;
                    this.barButtonItemAddSupplier.Visibility = userForSupplierPermission.CanInsert ? BarItemVisibility.Always : BarItemVisibility.Never;
                }
                EntityAccessPermision userForItemPermission = userPermissions.Select(perm => perm.EnityAccessPermision).FirstOrDefault(ent => ent.EntityType == "Item");
                if (userForItemPermission != null)
                {
                    bool userCanInsertItem = userForItemPermission.CanInsert && Program.Settings.MasterAppInstance != eApplicationInstance.STORE_CONTROLER;
                    this.barButtonItemAddItem.Enabled = userCanInsertItem;
                    this.barButtonItemAddItem.Visibility = userCanInsertItem ? BarItemVisibility.Always : BarItemVisibility.Never;
                    this.barButtonItemEditItem.Enabled = userForItemPermission.CanUpdate;
                    this.barButtonItemEditItem.Visibility = userForItemPermission.CanUpdate ? BarItemVisibility.Always : BarItemVisibility.Never;
                    this.barButtonItemDeleteItem.Enabled = userForItemPermission.CanDelete;
                    this.barButtonItemDeleteItem.Visibility = userForItemPermission.CanDelete ? BarItemVisibility.Always : BarItemVisibility.Never;
                }
            }
        }

        private void LoadPOS()
        {
            Dictionary<Guid, string> terminals = InitializePOS();

            repositoryItemLookUpEditPOSSendChanges.DataSource = terminals;
            repositoryItemLookUpEditPOSSendChanges.ValueMember = "Key";
            repositoryItemLookUpEditPOSSendChanges.DisplayMember = "Value";

            repositoryItemCheckedComboBoxEditPOSSendCommands.DataSource = terminals;
            repositoryItemCheckedComboBoxEditPOSSendCommands.ValueMember = "Key";
            repositoryItemCheckedComboBoxEditPOSSendCommands.DisplayMember = "Value";

            IDictionary<ePosCommand, string> commands = Enum<ePosCommand>.GetLocalizedDictionary();
            commands.Remove(ePosCommand.SEND_CHANGES);
            commands.Remove(ePosCommand.RETRY_IMMEDIATE);
            commands.Remove(ePosCommand.NONE);
            repositoryItemLookUpEditPOSSendAutoCommands.DataSource = commands;
            repositoryItemLookUpEditPOSSendAutoCommands.ValueMember = "Key";
            repositoryItemLookUpEditPOSSendAutoCommands.DisplayMember = "Value";
            repositoryItemLookUpEditPOSSendAutoCommands.Columns.Clear();
            repositoryItemLookUpEditPOSSendAutoCommands.Columns.Add(new LookUpColumnInfo("Value", Resources.Command));
        }
        private void FillCboCashierDevices()
        {
            XPCollection<ITS.Retail.Model.POS> poss = new XPCollection<ITS.Retail.Model.POS>(Program.Settings.ReadOnlyUnitOfWork, new BinaryOperator("Store", Program.Settings.StoreControllerSettings.Store.Oid));

            Dictionary<Guid, string> CashierDevices = new Dictionary<Guid, string>();
            foreach (var currentDevice in poss.Where(x => x.IsCashierRegister).ToList())
            {
                CashierDevices.Add(currentDevice.Oid, currentDevice.Name);
            }
            repositoryItemLookUpEditCashier.DataSource = CashierDevices.ToList();
            repositoryItemLookUpEditCashier.ValueMember = "Key";
            repositoryItemLookUpEditCashier.DisplayMember = "Value";
            repositoryItemLookUpEditCashier.Columns.Clear();
            repositoryItemLookUpEditCashier.Columns.Add(new LookUpColumnInfo("Value", Resources.CashierRegister));
        }

        private static Dictionary<Guid, string> InitializePOS()
        {
            XPCollection<ITS.Retail.Model.POS> poss = new XPCollection<ITS.Retail.Model.POS>(Program.Settings.ReadOnlyUnitOfWork, new BinaryOperator("Store", Program.Settings.StoreControllerSettings.Store.Oid));

            int scalesCount = (new XPCollection<ITS.Retail.Model.Scale>(Program.Settings.ReadOnlyUnitOfWork)).Count;

            Dictionary<Guid, string> terminals = new Dictionary<Guid, string>();

            foreach (ITS.Retail.Model.POS pos in poss)
            {
                terminals.Add(pos.Oid, pos.Name);
            }
            if (scalesCount > 0)
            {
                terminals.Add(Guid.Empty, Resources.Scales);
            }
            return terminals;
        }

        private void SetBindings()
        {
            txtPrinter.DataBindings.Add("EditValue", Program.Settings, "DefaultLabelPrinter", true, DataSourceUpdateMode.OnPropertyChanged);

            XPCollection<PrintLabelSettings> pls = new XPCollection<PrintLabelSettings>(Program.Settings.ReadOnlyUnitOfWork);

            repositoryItemComboBox1.ValueMember = "This";
            repositoryItemComboBox1.DisplayMember = "Description";
            repositoryItemComboBox1.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            repositoryItemComboBox1.DataSource = pls;


            PrintLabelSettings selectedValue = pls.FirstOrDefault(x => x.IsDefault);
            if (selectedValue == null && pls.Count == 1)
            {
                selectedValue = pls[0];
            }
            cmbLabel.EditValue = selectedValue;
            Copies.EditValue = selectedValue?.Copies ?? 0;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateStatusBar();
            CheckUserPermissions();
            if (Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE)
            {
                navBarGroupHQReports.Visible = false;
            }
            DeactivateNonAvailableButtons(Program.Settings.MasterAppInstance);
            RestoreLayout(grdViewDocuments, Program.Settings.DocumentProformaGridViewSettings);
            RestoreLayout(grdViewPosStatus, Program.Settings.PosGridViewSettings);
            RestoreLayout(grdViewLabelsDocumentGrid, Program.Settings.LabelDocumentGridViewSettings);
            RestoreLayout(grdViewLabelsPriceCatalogDetails, Program.Settings.LabelPCDGridViewSettings);
            RestoreLayout(grdViewCustomers, Program.Settings.CustomerGridViewSettings);
            RestoreLayout(grdViewItems, Program.Settings.ItemGridViewSettings);
            RestoreLayout(grdViewAddItemsToCash, Program.Settings.CashierGridViewSettings);
            RestoreLayout(grdViewCustomers, Program.Settings.CustomerGridViewSettings);
            RestoreLayout(grdViewAddItemsToCash, Program.Settings.CashierGridViewAddItemSettings);
            RestoreLayout(grdViewSuppliers, Program.Settings.SupplierGridViewSettings);
            RestoreLayout(grdViewDocumentLabelDetails, Program.Settings.DocumentLabelGridViewSettings);
            RestoreLayout(this.Ribbon.Toolbar, Program.Settings.QuickAccessToolbarLayout);
            ribbonControlMain.Toolbar.ItemLinks.CollectionChanged += ItemLinks_CollectionChanged;

            try
            {
                //navBarGroupReports
                Dictionary<Guid, String> reportCategories = ReportsHelper.GetVisibleReportCategoriesDict(Program.Settings.CurrentUser);
                int ctr = 0;
                foreach (KeyValuePair<Guid, string> entry in reportCategories)
                {
                    NavBarItemWithOID x = new NavBarItemWithOID();
                    x.Caption = entry.Value;
                    x.Name = "Report_" + ctr.ToString();
                    x.Tag = "Report_" + ctr.ToString();
                    x.OID = entry.Key;
                    ctr += 1;
                    navBarGroupReports.ItemLinks.Add(x);
                    NavBarItemWithOID x2 = new NavBarItemWithOID();
                    x2.Caption = entry.Value;
                    x2.Name = "HQReport_" + ctr.ToString();
                    x2.Tag = "HQReport_" + ctr.ToString();
                    x2.OID = entry.Key;
                    ctr += 1;
                    navBarGroupHQReports.ItemLinks.Add(x2);
                }




            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                {
                    eApplicationInstance appInstance = eApplicationInstance.RETAIL;
                    string url = null;
                    itsService.SendMasterInfo(out appInstance, out url);
                    Program.Settings.MasterAppInstance = appInstance;
                    Program.Settings.MasterURLService = url;
                    Program.Settings.UnderlyingeDatabaseType = itsService.GetDbType();
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }

            navBarItemSpecialProforma.Visible = StoreHelper.StoreHasSpecialProformaTypeAndSeries(Program.Settings.StoreControllerSettings.Store);
            ShowRibbon("Proforma");
            navBarControlNavigation.ActiveGroup = navBarGroupDocuments;
            navBarItemCashierController.Visible = false;
            DeactivateSortingOfNonPersistentFields();
            this.MinimumSize = new Size(1024, 768);
            //#if DEBUG
            this.cmdText.Visible = false;
            //#endif
        }

        private void DeactivateNonAvailableButtons(eApplicationInstance masterAppInstance)
        {
            switch (masterAppInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    this.barButtonItemAddSupplier.Enabled = false;
                    this.barButtonItemAddSupplier.Visibility = BarItemVisibility.Never;
                    this.barButtonItemEditSupplier.Enabled = false;
                    this.barButtonItemEditSupplier.Visibility = BarItemVisibility.Never;
                    this.barButtonItemAddItem.Enabled = false;
                    this.barButtonItemAddItem.Visibility = BarItemVisibility.Never;
                    this.barButtonItemDeleteItem.Enabled = false;
                    this.barButtonItemDeleteItem.Visibility = BarItemVisibility.Never;
                    break;
                default:
                    break;
            }
        }

        private void DeactivateSortingOfNonPersistentFields()
        {
            colStore.OptionsColumn.AllowSort = DefaultBoolean.False;
            colCancelsDocument.OptionsColumn.AllowSort = DefaultBoolean.False;
            gridColTaxOffice.OptionsColumn.AllowSort = DefaultBoolean.False;
        }

        private void LocaliseApplication()
        {
            this.flyoutPanelFilter.OptionsButtonPanel.Buttons.ForEach(x =>
            {
                if ((x is DevExpress.XtraEditors.ButtonPanel.BaseButton))
                {
                    (x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption = LocalizeString((x as DevExpress.XtraEditors.ButtonPanel.BaseButton).Caption);
                }
            });
        }

        private void RefreshGridUow()
        {
            if (gridControlMain.DataSource != null)
            {
                if (gridControlMain.DataSource is IDisposable)
                {
                    ((IDisposable)gridControlMain.DataSource).Dispose();
                }
                gridControlMain.DataSource = null;
            }
            if (this.GridUnitOfWork != null)
            {
                this.GridUnitOfWork.Dispose();
            }
            this.GridUnitOfWork = XpoHelper.GetNewUnitOfWork();
        }

        private void barButtonItemPrintLabels_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (cmbLabel.EditValue == null)
            {
                XtraMessageBox.Show(Resources.LabelIsEmpty, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (String.IsNullOrWhiteSpace(txtPrinter.EditValue as string))
            {
                XtraMessageBox.Show(Resources.NoPrinterSelected, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrintLabelSettings lblSettings = cmbLabel.EditValue as PrintLabelSettings;
            lblSettings.Copies = Convert.ToInt16(Copies.EditValue);

            string ejf = Encoding.UTF8.GetString(lblSettings.Label.LabelFile);
            int requestEncoding = lblSettings.PrinterEncoding.HasValue && lblSettings.PrinterEncoding.Value != 0 ? lblSettings.PrinterEncoding.Value : lblSettings.Label.PrinterEncoding;
            string[] dets = new string[] { };
            string output = "", message;
            byte[] bdata;
            if (gridControlMain.MainView == grdViewLabelsPriceCatalogDetails)
            {
                List<PriceCatalogDetail> pcds = grdViewLabelsPriceCatalogDetails.GetSelectedRows().Select(x => grdViewLabelsPriceCatalogDetails.GetRow(x) as PriceCatalogDetail).ToList();
                if (pcds.Count <= 0)
                {
                    XtraMessageBox.Show(Resources.PleaseSelectAnItem, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                output = ToolsHelper.GetLabelsStringToPrint(Program.Settings.StoreControllerSettings.Store, "pcd", pcds, dets, lblSettings.Copies, output, ejf, lblSettings.Label,
                    Program.Settings.ReadOnlyUnitOfWork, requestEncoding, new List<LeafletDetail>(), out message, out bdata, Program.Settings.UnderlyingeDatabaseType);

                bool printLocaly = lblSettings.Label.PrintServiceSettings == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService.IsActive == false;
                if (printLocaly)
                {
                    PrinterServiceHelper.SendByteArrayToPrinter(txtPrinter.EditValue as string, bdata);
                }
                else
                {
                    RemotePrintLabels(lblSettings, requestEncoding, output);
                }
            }
            else if (gridControlMain.MainView == grdViewItems)
            {
                List<Item> items = grdViewItems.GetSelectedRows().Select(x => ((Item)(((dynamic)grdViewItems.GetRow(x)).Item))).ToList();
                if (items.Count <= 0)
                {
                    XtraMessageBox.Show(Resources.PleaseSelectAtLeastOneRecordToPrint, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (items.Count > 1)
                {
                    XtraMessageBox.Show(Resources.PleaseSelectAnItem, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //CALL NEW FUNCTION
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(Program.Settings.StoreControllerSettings.Store);

                    IEnumerable<PriceCatalogDetail> priceCatalogDetails = items.Select(item => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, item))
                                                                                .Where(priceCatalogPolicyResult => priceCatalogPolicyResult != null
                                                                                                                && priceCatalogPolicyResult.PriceCatalogDetail != null
                                                                                      )
                                                                                .Select(priceCatalogPolicyResult => priceCatalogPolicyResult.PriceCatalogDetail);

                    //priceCatalogDetails = priceCatalogDetails.Where(priceDetail => priceDetail != null);

                    output = ToolsHelper.GetLabelsStringToPrint(Program.Settings.StoreControllerSettings.Store, "item", priceCatalogDetails, dets, lblSettings.Copies, output, ejf, lblSettings.Label,
                        Program.Settings.ReadOnlyUnitOfWork, requestEncoding, new List<LeafletDetail>(), out message, out bdata, Program.Settings.UnderlyingeDatabaseType);

                    bool printLocaly = lblSettings.Label.PrintServiceSettings == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService.IsActive == false;
                    if (printLocaly)
                    {
                        PrinterServiceHelper.SendByteArrayToPrinter(txtPrinter.EditValue as string, bdata);
                    }
                    else
                    {
                        RemotePrintLabels(lblSettings, requestEncoding, output);
                    }

                }
            }
            else if (gridControlMain.MainView == grdViewLabelsDocumentGrid)
            {
                if (detailCache == null)
                {
                    XtraMessageBox.Show(Resources.PleaseSelectAnItem, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                IEnumerable<GridView> lblGridViews = detailCache.Select(x => x.Value);
                List<Guid> docDetOids = new List<Guid>();
                List<PriceCatalogDetail> pcds = new List<PriceCatalogDetail>();
                List<LabelDocument> labelDocuments = new List<LabelDocument>();

                foreach (GridView lblgrd in lblGridViews)
                {
                    lblgrd.GetSelectedRows().ToList().ForEach(gridRow =>
                    {

                        LabelDocument label = lblgrd.GetRow(gridRow) as LabelDocument;
                        if (label != null)
                        {
                            labelDocuments.Add(label);
                        }
                    });
                }

                labelDocuments.OrderBy(labelDocument => labelDocument.DocumentDetailLineNumber).Select(label => label).ToList().ForEach(label =>
                {
                    docDetOids.Add(label.DocumentDetail.Oid);
                    pcds.Add(label.PriceCatalogDetail);
                });

                output = ToolsHelper.GetLabelsStringToPrint(Program.Settings.StoreControllerSettings.Store, "pcd", pcds, docDetOids.Select(x => x.ToString()).ToArray(),
                    lblSettings.Copies, output, ejf, lblSettings.Label,
                    Program.Settings.ReadOnlyUnitOfWork,
                    /*Program.Settings.StoreControllerSettings.Store.DefaultPriceCatalog,*/ requestEncoding, new List<LeafletDetail>(), out message, out bdata, Program.Settings.UnderlyingeDatabaseType);
                bool printLocaly = lblSettings.Label.PrintServiceSettings == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService.IsActive == false;
                if (printLocaly)
                {
                    PrinterServiceHelper.SendByteArrayToPrinter(txtPrinter.EditValue as string, bdata);
                }
                else
                {
                    RemotePrintLabels(lblSettings, requestEncoding, output);
                }
            }
            else if (gridControlMain.MainView == gridViewLeaflets)
            {
                List<Leaflet> leaflets = gridViewLeaflets.GetSelectedRows().Select(x => gridViewLeaflets.GetRow(x) as Leaflet).ToList();
                if (leaflets.Count <= 0)
                {
                    XtraMessageBox.Show(Resources.PleaseSelectAtLeastOneRecordToPrint, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

                List<PriceCatalogDetail> priceCatalogDetails = new List<PriceCatalogDetail>();
                List<LeafletDetail> leafletDetails = new List<LeafletDetail>();
                foreach (Leaflet leaflet in leaflets)
                {
                    if (leaflet.LeafletDetails.Count > 0)
                    {
                        foreach (LeafletDetail detail in leaflet.LeafletDetails)
                        {
                            leafletDetails.Add(detail);
                        }
                    }
                }

                lblSettings.Label.UseDirectSQL = false;
                output = ToolsHelper.GetLabelsStringToPrint(Program.Settings.StoreControllerSettings.Store, "leaflet", priceCatalogDetails, dets, lblSettings.Copies, output, ejf, lblSettings.Label,
                    Program.Settings.ReadOnlyUnitOfWork, requestEncoding, leafletDetails, out message, out bdata, Program.Settings.UnderlyingeDatabaseType);

                bool printLocaly = lblSettings.Label.PrintServiceSettings == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService == null || lblSettings.Label.PrintServiceSettings.RemotePrinterService.IsActive == false;
                if (printLocaly)
                {
                    PrinterServiceHelper.SendByteArrayToPrinter(txtPrinter.EditValue as string, bdata);
                }
                else
                {
                    RemotePrintLabels(lblSettings, requestEncoding, output);
                }

            }
        }

        private void RemotePrintLabels(PrintLabelSettings lblSettings, int requestEncoding, string output)
        {
            PrintServerPrintLabelResponse printServerPrintLabelResponse = PrinterServiceHelper.PrintLabel(lblSettings.Label.PrintServiceSettings.RemotePrinterService, output, requestEncoding, lblSettings.Label.PrintServiceSettings.PrinterNickName);
            if (printServerPrintLabelResponse == null)
            {
                XtraMessageBox.Show(ResourcesLib.Resources.Error, ResourcesLib.Resources.CouldNotEstablishConnection + " Remote Print Service :" + lblSettings.Label.PrintServiceSettings.RemotePrinterService.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            switch (printServerPrintLabelResponse.Result)
            {
                case ePrintServerResponseType.FAILURE:
                    XtraMessageBox.Show(ResourcesLib.Resources.Error, printServerPrintLabelResponse.Explanation, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                case ePrintServerResponseType.SUCCESS:
                    XtraMessageBox.Show(ResourcesLib.Resources.SuccefullyCompleted, ResourcesLib.Resources.SuccefullyCompleted, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        private byte[] ConvertToBytes(string sourceString, int to_encoding)
        {
            Encoding newencoding = Encoding.GetEncoding(to_encoding);
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(sourceString);

            // Perform the conversion from one encoding to the other.
            byte[] newBytes = Encoding.Convert(unicode, newencoding, unicodeBytes);
            return newBytes;
        }

        private void txtPrinter_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectTagPrinter();
        }

        private void SelectTagPrinter()
        {
            using (PrintDialog pd = new PrintDialog())
            {
                pd.PrinterSettings = new PrinterSettings();
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    txtPrinter.EditValue = pd.PrinterSettings.PrinterName;
                }
            }
        }

        private void grdLabelsDocumentGrid_MasterRowExpanding(object sender, MasterRowCanExpandEventArgs e)
        {
            e.Allow = true;
        }

        private void grdLabelsDocumentGrid_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            DocumentHeader documentHeader = (sender as GridView).GetRow(e.RowHandle) as DocumentHeader;
            EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(Program.Settings.StoreControllerSettings.Store);
            if (documentHeader != null)
            {
                e.ChildList = documentHeader.DocumentDetails.Select(documentDetail =>
                    new LabelDocument()
                    {
                        DocumentDetail = documentDetail,
                        PriceCatalogDetail = GetPriceCatalogDetail(documentDetail, documentHeader, effectivePriceCatalogPolicy)
                    }).Where(labelDocument => labelDocument.PriceCatalogDetail != null).ToList();
            }
            else
            {
                e.ChildList = new List<LabelDocument>();
            }
        }

        private void grdLabelsDocumentGrid_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 2;
        }

        private void grdLabelsDocumentGrid_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView detailView = (sender as GridView).GetDetailView(e.RowHandle, e.RelationIndex) as GridView;
            if (detailCache == null)
            {
                detailCache = new Dictionary<int, GridView>();
            }
            if (detailCache.ContainsKey(e.RowHandle) == false)
            {
                detailCache.Add(e.RowHandle, detailView);
            }
            else
            {
                detailCache[e.RowHandle] = detailView;
            }
        }

        private void grdLabelsDocumentGrid_MasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e)
        {
            e.DefaultView = grdViewDocumentLabelDetails;
        }

        private void barButtonEditDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null) return;

            if (DocumentHelper.CanEdit(documentHeader, Program.Settings.CurrentUser) == false)
            {
                XtraMessageBox.Show(Resources.YouCannotEditThisDocument, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ShowDocumentForm(uow.GetObjectByKey<DocumentHeader>(documentHeader.Oid));
                documentHeader.Session.Reload(documentHeader, true);
            }
            (panelFilter.Controls[0] as BaseFilterControl).Search();
        }

        private void barButtonAddDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateDocument(eDivision.Sales);
        }

        private void CreateDocument(eDivision division)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader documentHeader = new DocumentHeader(uow);
                using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                {
                    documentHeader.CreatedOnTicks = itsService.GetNowTicks();
                }
                documentHeader.CreatedBy = documentHeader.Session.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid);
                documentHeader.Store = uow.GetObjectByKey<Store>(Program.Settings.StoreControllerSettings.Store.Oid);
                documentHeader.Division = division;
                ShowDocumentForm(documentHeader, false);
            }
            (panelFilter.Controls[0] as BaseFilterControl).Search();
        }

        private void barButtonItemAddCustomer_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Customer customer = new Customer(uow);
                customer.Trader = new Trader(customer.Session);
                customer.Owner = customer.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                ShowCustomerForm(customer, false);
                gridControlMain.RefreshDataSource();
            }
        }

        private void barButtonItemEditCustomer_ItemClick(object sender, ItemClickEventArgs e)
        {
            Customer customer = GetSelectedCustomer();
            if (customer == null)
            {
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ShowCustomerForm(uow.GetObjectByKey<Customer>(customer.Oid), false);
                customer.Session.Reload(customer, true);
                this.grdViewCustomers.RefreshData();
            }
        }

        private void ShowDocumentForm(DocumentHeader header, bool previewDocument = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (DocumentEditForm frm = DocumentEditForm.CreateForm(header, previewDocument))
            {
                frm.ShowDialog();
            }

        }

        public void ShowCustomerForm(Customer customer, bool previewDocument = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (CustomerEditForm frm = new CustomerEditForm(customer, previewDocument))
            {
                frm.DuplicateTraderFound += new CustomerEditForm.MyEvent(frm_DuplicateCustomerFound);
                frm.ShowDialog();
            }
        }

        private void frm_DuplicateCustomerFound(Customer customer)
        {
            ShowCustomerForm(customer, false);
        }

        private void ShowItemForm(Item item, bool previewDocument = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (ItemEditForm frm = new ItemEditForm(item, previewDocument))
            {
                DialogResult result = frm.ShowDialog();
                if (result == DialogResult.Retry)
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        this.ShowItemForm(uow.GetObjectByKey<Item>(item.Oid), previewDocument);
                    }
                }
            }
        }

        public void ShowSupplierForm(SupplierNew supplier, bool preview = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (SupplierEditForm frm = new SupplierEditForm(supplier, preview))
            {
                frm.DuplicateTraderFound += new SupplierEditForm.MyEvent(frm_DuplicateSupplierFound);
                frm.ShowDialog();
            }
        }

        private void frm_DuplicateSupplierFound(SupplierNew supplier)
        {
            ShowSupplierForm(supplier, false);
        }

        private void ribbonControlMain_SelectedPageChanged(object sender, EventArgs e)
        {
            posStatusTimer.Enabled = false;
            if (ribbonControlMain.SelectedPage == ribbonPageDocuments || ribbonControlMain.SelectedPage == ribbonPageProforma)
            {
                gridControlMain.MainView = this.grdViewDocuments;
            }
            else if (ribbonControlMain.SelectedPage == ribbonPageTags)
            {
                gridControlMain.MainView = this.grdViewLabelsPriceCatalogDetails;
            }
            else if (ribbonControlMain.SelectedPage == ribbonPageCustomers)
            {
                gridControlMain.MainView = this.grdViewCustomers;
            }
            else if (ribbonControlMain.SelectedPage == ribbonPageItems)
            {
                gridControlMain.MainView = this.grdViewItems;
            }
            else if (ribbonControlMain.SelectedPage == ribbonPagePOS)
            {
                gridControlMain.MainView = this.grdViewPosStatus;
                UpdatePosStatus();
            }
            else if (ribbonControlMain.SelectedPage == ribbonPagePriceCatalogs)
            {
                if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    barButtonItemAddPriceCatalog.Visibility = BarItemVisibility.Never;
                }
            }
            else if (ribbonControlMain.SelectedPage == ribbonPageCashierController)
            {
                gridControlMain.MainView = this.grdViewAddItemsToCash;
            }
            else if (ribbonControlMain.SelectedPage == ribbonPageLeaflets)
            {
                gridControlMain.MainView = this.gridViewLeaflets;
            }
            gridControlMain.DataSource = null;

            SetLabelSelectionCount();
        }



        private void barButtonPrintDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null)
            {
                return;
            }
            bool autoprint = e.Item == this.barButtonPrintDocument;
            if (documentHeader.DocumentType.TakesDigitalSignature
                && documentHeader.DocumentNumber > 0
                && String.IsNullOrWhiteSpace(documentHeader.Signature)
               )
            {
                try
                {
                    StoreControllerSettings settings = documentHeader.Session.GetObjectByKey<StoreControllerSettings>(StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
                    List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                        Where(x =>
                                x.DocumentSeries.Any(y => y.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid)
                             && x.TerminalDevice is POSDevice
                             && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                        ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                    string signature = DocumentHelper.SignDocument(documentHeader, Program.Settings.CurrentUser, documentHeader.Owner, String.Empty/*MvcApplication.OLAPConnectionString*/, posDevices);
                    if (string.IsNullOrWhiteSpace(signature))
                    {
                        Program.Logger.Error(Resources.CannotRetreiveSignature);
                        XtraMessageBox.Show(Resources.CannotRetreiveSignature, Resources.CannotRetreiveSignature, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    documentHeader.Signature = signature;
                    documentHeader.Save();
                    XpoHelper.CommitTransaction(documentHeader.Session);
                }
                catch (Exception exception)
                {
                    string message = String.Format("{0} {1}", Resources.CannotRetreiveSignature, exception.GetFullMessage());
                    Program.Logger.Error(exception, message);
                    XtraMessageBox.Show(exception.GetFullMessage(), Resources.CannotRetreiveSignature, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            PrintDocumentHelper.PrintDocument(documentHeader, autoprint);
        }

        private void barButtonDeleteDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null) return;

            string failureReason;
            if (DocumentHelper.DocumentCanBeDeleted(documentHeader.Oid, Program.Settings.CurrentUser, out failureReason) == false
                || DialogResult.Yes != XtraMessageBox.Show(Resources.ConfirmDelete, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                XtraMessageBox.Show(failureReason, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                uow.GetObjectByKey<DocumentHeader>(documentHeader.Oid).Delete();
                uow.CommitTransaction();
                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
                XtraMessageBox.Show(Resources.SuccesfullyDeleted, Resources.SuccesfullyDeleted, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();

                (panelFilter.Controls[0] as BaseFilterControl).Search();
            }
        }

        private void barButtonCopyDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null) return;
            if (DocumentHelper.CanCopy(documentHeader, Program.Settings.CurrentUser) == false)
            {
                XtraMessageBox.Show(Resources.YouCannotCopyThisDocument, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader document = (DocumentHeader)DocumentHelper.CopyDocument(documentHeader.Oid, uow);
                if (document == null)
                {
                    XtraMessageBox.Show(Resources.YouCannotCopyThisDocument, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                ShowDocumentForm(document);

                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
            }
            (panelFilter.Controls[0] as BaseFilterControl).Search();
        }

        private void barButtonCancelDocument_ItemClick(object sender, ItemClickEventArgs e)
        {

            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null)
            {
                return;
            }
            string failureReason;
            if (DocumentHelper.DocumentCanBeCanceled(documentHeader.Oid, Program.Settings.CurrentUser, out failureReason) == false)
            {
                XtraMessageBox.Show(failureReason, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (XtraMessageBox.Show(Resources.CancelDocument, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                long newUpdatedOnTicks;
                try
                {
                    using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                    {
                        newUpdatedOnTicks = itsService.GetNowTicks();
                    }
                }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(Resources.ConnectionTimeOut + Environment.NewLine + exception.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                Guid CancelingDocumentGuid = DocumentHelper.CancelDocument(documentHeader.Oid, Program.Settings.CurrentUser.Oid, newUpdatedOnTicks);
                DocumentHelper.SignDocumentIfNecessary(CancelingDocumentGuid, Program.Settings.CurrentUser, Program.Settings.StoreControllerSettings.Oid);

                XtraMessageBox.Show(Resources.DocumentSuccessfullyCanceled, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
                barButtonSearchDocuments.PerformClick();

                DocumentHeader CanceledByDocument = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentHeader>(CancelingDocumentGuid);

                try
                {
                    PrintDocumentHelper.PrintDocument(CanceledByDocument, false);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                    XtraMessageBox.Show(string.Format(Resources.DocumentHaveBeenSavedButCouldNotBePrinted, documentHeader.DocumentSeries.Description), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                (panelFilter.Controls[0] as BaseFilterControl).Search();
            }
        }




        private DocumentHeader GetSelectedDocument()
        {

            var rows = (gridControlMain.MainView as GridView).GetSelectedRows();
            DocumentHeader documentHeader = null;
            if (rows.Count() > 1)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
            else if (rows.Count() == 1)
            {
                documentHeader = (gridControlMain.MainView as GridView).GetFocusedRow() as DocumentHeader;
            }

            if (documentHeader == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return documentHeader;
        }

        private Customer GetSelectedCustomer()
        {
            Customer customer = (gridControlMain.MainView as GridView).GetFocusedRow() as Customer;
            if (customer == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return customer;
        }

        private Item GetSelectedItem()
        {
            Item item = null;
            try
            {
                dynamic x = (gridControlMain.MainView as GridView).GetFocusedRow();
                item = x.Item as Item;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            if (item == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return item;
        }

        private SupplierNew GetSelectedSupplier()
        {
            SupplierNew supplier = (gridControlMain.MainView as GridView).GetFocusedRow() as SupplierNew;
            if (supplier == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return supplier;
        }

        private void barButtonTransformDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null)
            {
                return;
            }

            string result = DocumentHelper.DocumentCanBeTransformed(documentHeader);
            if (string.IsNullOrEmpty(result) == false)
            {
                XtraMessageBox.Show(result, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string errorMessage;
            eModule module = DocumentHelper.GetDocSeriesModule(Program.Settings.MasterAppInstance);
            //Program.Settings.MasterAppInstance == eApplicationInstance.DUAL_MODE ? eModule.DUAL : eModule.STORECONTROLLER;

            List<TransformationRule> transformationRules = DocumentHelper.AllowedTransformationRules(new List<DocumentType>() { documentHeader.DocumentType },
                documentHeader.Store, module, documentHeader.Division, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage) == false)
            {
                XtraMessageBox.Show(errorMessage, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DocumentType transformationDocumentType = null;
            DocumentSeries transformationDocumentSeries = null;
            if (transformationRules.Count == 1)
            {
                transformationDocumentType = transformationRules[0].DerrivedType;
                IEnumerable<DocumentSeries> series = StoreHelper.StoreSeriesForDocumentType(documentHeader.Store, transformationDocumentType, module);
                if (series.Count() == 1)
                {
                    transformationDocumentSeries = series.First();
                }
            }
            if (transformationDocumentType == null || transformationDocumentSeries == null)
            {
                using (TransformDocumentsForm transformDocumentForm = new TransformDocumentsForm(documentHeader, transformationRules))
                {
                    if (transformDocumentForm.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    transformationDocumentSeries = transformDocumentForm.TransformationDocumentSeries;
                    transformationDocumentType = transformDocumentForm.TransformationDocumentType;
                }
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                List<DocumentDetail> linkedlines = DocumentHelper.GetLinkedLines(new XPCollection<DocumentHeader>(uow, new BinaryOperator("Oid", documentHeader.Oid)));
                List<DocumentDetailAssociation> documentDetailAssociations = DocumentHelper.CreateDocumentDetailAssociations(new XPCollection<DocumentHeader>(uow, new BinaryOperator("Oid", documentHeader.Oid)));
                DocumentHeader derrivedDocument = DocumentHelper.CreateDerivativeDocument(documentDetailAssociations, Program.Settings.CurrentUser, transformationDocumentType, transformationDocumentSeries, linkedlines);
                ShowDocumentForm(derrivedDocument);
                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
            }
            (panelFilter.Controls[0] as BaseFilterControl).Search();
        }

        private void barButtonViewDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            DocumentHeader documentHeader = GetSelectedDocument();
            if (documentHeader == null)
            {
                return;
            }
            ShowDocumentForm(documentHeader, true);
        }


        private void repositoryItemTextEditPrinter_Click(object sender, EventArgs e)
        {
            SelectTagPrinter();
        }

        private void barButtonPosStatus_ItemClick(object sender, ItemClickEventArgs e)
        {
            UpdatePosStatus();
        }

        private void posStatusTimer_Tick(object sender, EventArgs e)
        {
            posStatusTimer.Enabled = false;
            if (gridControlMain.MainView == this.grdViewPosStatus)
            {
                UpdatePosStatus();
                new Thread(() => UpdateCashierStatus()) { IsBackground = true }.Start();
            }
        }

        private void UpdatePosStatus()
        {
            RefreshGridUow();
            gridControlMain.MainView = this.grdViewPosStatus;

            using (XPCollection<ITS.Retail.Model.POS> poses = new XPCollection<ITS.Retail.Model.POS>(this.GridUnitOfWork))
            {
                gridControlMain.DataSource = poses.Select(posViewModel => new PosStatusViewModel()
                {
                    ID = posViewModel.ID,
                    Name = posViewModel.Name,
                    IPAddress = posViewModel.IPAddress,
                    IsCashierRegister = posViewModel.IsCashierRegister,
                    IsAlive = posViewModel.IsAlive,
                    MachineStatus = posViewModel.Status.MachineStatus,
                    MachineStatusDate = posViewModel.Status.MachineStatusDate,
                    XCount = POSHelper.GetCurrentXCount(posViewModel),
                    XValue = POSHelper.GetCurrentXAmount(posViewModel),
                    ZCount = POSHelper.GetCurrentZCount(posViewModel),
                    ZValue = POSHelper.GetCurrentZAmount(posViewModel)
                }).ToList();
            }
            posStatusTimer.Enabled = true;
            grdViewPosStatus.OptionsView.ShowIndicator = false;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }
        private void UpdateCashierStatus()
        {
            //try
            //{
            //    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Store", Program.Settings.StoreControllerSettings.Store.Oid),
            //                                                      new BinaryOperator("IsCashierRegister", true)
            //                                                    );
            //    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            //    {
            //        XPCollection<ITS.Retail.Model.POS> cashierDevice = new XPCollection<ITS.Retail.Model.POS>(uow, criteria);

            //        foreach (var currentDevice in cashierDevice)
            //        {
            //            if (currentDevice.TerminalDeviceAssociations.FirstOrDefault() != null)
            //            {
            //                if ((currentDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice) != null)
            //                {
            //                    DeviceSettings settings = (currentDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice).DeviceSettings;
            //                    CashRegisterFactory cashRegisterFactory = new CashRegisterFactory();
            //                    ConnectionType connectionType = currentDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice.ConnectionType;
            //                    string message;
            //                    CashRegisterHardware cashRegisterHardware = cashRegisterFactory.GetCashRegisterHardware(settings.DeviceType, settings, currentDevice.Name, currentDevice.ID, connectionType, settings.LineChars, settings.CommandChars);
            //                    eMachineStatus Statusresult = cashRegisterHardware.ReadStatusOfDevice(out message);

            //                    string messageDailyTotals = "";
            //                    cashRegisterHardware.GetTotalSalesOfDay(out messageDailyTotals);
            //                    decimal Totals = 0;
            //                    if (messageDailyTotals != "")
            //                    {
            //                        Totals = (cashRegisterHardware as RBSElioCashRegister).CalculateDaylyTotals(messageDailyTotals).DailyTotals;
            //                    }

            //                    string ip = (settings as EthernetDeviceSettings).IPAddress;
            //                    currentDevice.IPAddress = ip;
            //                    if (String.IsNullOrWhiteSpace(message))
            //                    {
            //                        currentDevice.IsActive = true;
            //                        currentDevice.Status.MachineStatusTicks = DateTime.Now.Ticks;
            //                        currentDevice.Status.MachineStatus = Statusresult;

            //                    }
            //                    else
            //                    {
            //                        currentDevice.IsActive = false;
            //                        currentDevice.Status.MachineStatusTicks = DateTime.Now.Ticks;
            //                        currentDevice.Status.MachineStatus = eMachineStatus.UNKNOWN;
            //                        currentDevice.Save();
            //                        currentDevice.Status.Save();
            //                    }
            //                    currentDevice.Save();
            //                    currentDevice.Status.Save();
            //                }
            //            }
            //            else
            //            {
            //                throw new Exception("No cashier Regiser found");
            //            }
            //        }
            //        XpoHelper.CommitTransaction(uow);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    //XtraMessageBox.Show(exception.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void UpdateStatusBar()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
            barStaticItemApplication.Caption = GetType().Assembly.GetName().Name;

            string patchVersion = String.Empty;
            string patchFile = "./patch.txt";
            if (File.Exists(patchFile))
            {
                patchVersion = " " + File.ReadAllText(patchFile);
            }

            barStaticItemApplicationVersion.Caption = GetType().Assembly.GetName().Version.ToString(4) + patchVersion;
            barStaticItemCopyright.Caption = versionInfo.LegalCopyright;

            barStaticItemStore.Caption = Program.Settings.StoreControllerSettings.Store.Name;
            if (Program.Settings.CurrentUser != null)
            {
                barStaticItemUser.Caption = Program.Settings.CurrentUser.UserName;
            }
            barStaticItemDateTime.Caption = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            barStaticItemDateTime.Refresh();
            ribStatus.Invalidate();
        }

        private void gridViewDocuments_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colTransformationStatus && e.Value != null)
            {
                string value = Resources.ResourceManager.GetString(e.Value.ToString());
                if (string.IsNullOrEmpty(value) == false)
                {
                    e.DisplayText = value;
                }
            }
        }

        //Restore GridView Layouts
        private void gridViewDocuments_Layout(object sender, EventArgs e)
        {
            if (panelFilter.Controls.Count > 0)
            {
                if (panelFilter.Controls[0] is ProformaDocumentFilter && panelFilter.Controls[0].Name == "Proforma")
                {
                    Program.Settings.DocumentProformaGridViewSettings = GetLayout(grdViewDocuments);
                }
                else if (panelFilter.Controls[0] is ProformaDocumentFilter)
                {
                    Program.Settings.DocumentSpecialProformaGridViewSettings = GetLayout(grdViewDocuments);
                }
                else if (panelFilter.Controls[0] is SalesDocumentFilter)
                {
                    Program.Settings.DocumentSalesGridViewSettings = GetLayout(grdViewDocuments);
                }
                else if (panelFilter.Controls[0] is PurchaseDocumentFilter)
                {
                    Program.Settings.DocumentPurchaseGridViewSettings = GetLayout(grdViewDocuments);
                }
                else if (panelFilter.Controls[0] is StoreDocumentFilter)
                {
                    Program.Settings.DocumentStoreGridViewSettings = GetLayout(grdViewDocuments);
                }
                else if (panelFilter.Controls[0] is FinancialDocumentFilter)
                {
                    Program.Settings.DocumentStoreGridViewSettings = GetLayout(grdViewDocuments);
                }
            }
        }

        private void grdPosStatus_Layout(object sender, EventArgs e)
        {
            Program.Settings.PosGridViewSettings = GetLayout(grdViewPosStatus);
        }

        private void grdLabelsPriceCatalogDetails_Layout(object sender, EventArgs e)
        {
            Program.Settings.LabelPCDGridViewSettings = GetLayout(grdViewLabelsPriceCatalogDetails);
        }

        private void grdLabelsDocumentGrid_Layout(object sender, EventArgs e)
        {
            Program.Settings.LabelDocumentGridViewSettings = GetLayout(grdViewLabelsDocumentGrid);
        }

        private void grdViewCustomers_Layout(object sender, EventArgs e)
        {
            Program.Settings.CustomerGridViewSettings = GetLayout(grdViewCustomers);
        }

        private void grdViewItems_Layout(object sender, EventArgs e)
        {
            Program.Settings.ItemGridViewSettings = GetLayout(grdViewItems);
        }
        private void grdCashierRegister_Layout(object sender, EventArgs e)
        {
            Program.Settings.CashierGridViewSettings = GetLayout(grdCashierRegister);
        }
        private void grdViewSuppliers_Layout(object sender, EventArgs e)
        {
            Program.Settings.SupplierGridViewSettings = GetLayout(grdViewSuppliers);
        }

        private void grdViewDocumentLabelDetails_Layout(object sender, EventArgs e)
        {
            Program.Settings.DocumentLabelGridViewSettings = GetLayout(grdViewDocumentLabelDetails);
        }

        private void gridViewDocuments_RowStyle(object sender, RowStyleEventArgs e)
        {
            DocumentHeader header = grdViewDocuments.GetRow(e.RowHandle) as DocumentHeader;

            if (header != null && header.IsCanceled)
            {
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Strikeout);
                e.Appearance.ForeColor = Color.Red;
            }
            else if (header != null && header.IsCancelingAnotherDocument)
            {
                e.Appearance.ForeColor = Color.MediumOrchid;
            }
        }

        private void beiPurchaseAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateDocument(eDivision.Purchase);
        }

        private void barButtonExportDocumentToCSV_ItemClick(object sender, ItemClickEventArgs e)
        {

        }


        private void barButtonItemLogOut_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult dialogResult = XtraMessageBox.Show(Resources.AreYouSureYouWantToSignOut, Resources.AreYouSureYouWantToSignOut, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Program.Settings.CurrentUser = null;
                LoginForm loginForm = new LoginForm(true);
                loginForm.Show();
                this.ReturnToLoginForm = true;
                this.Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsClosing = true;
            if (e.CloseReason != CloseReason.ApplicationExitCall && this.ReturnToLoginForm == false)
            {
                Application.Exit();
            }
        }

        private void barButtonItemExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult dialogResult = XtraMessageBox.Show(Resources.Exit, Resources.Exit, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void barButtonItemGreek_ItemClick(object sender, ItemClickEventArgs e)
        {
            AskUserPermissionToChangeLanguageAndClose(eCultureInfo.Ελληνικά);
        }

        private void barButtonItemEnglish_ItemClick(object sender, ItemClickEventArgs e)
        {
            AskUserPermissionToChangeLanguageAndClose(eCultureInfo.English);
        }

        private void barButtonItemDeutch_ItemClick(object sender, ItemClickEventArgs e)
        {
            AskUserPermissionToChangeLanguageAndClose(eCultureInfo.Deutch);
        }

        private void barButtonItemNorsk_ItemClick(object sender, ItemClickEventArgs e)
        {
            AskUserPermissionToChangeLanguageAndClose(eCultureInfo.Norsk);
        }

        private void AskUserPermissionToChangeLanguageAndClose(eCultureInfo eCulture)
        {
            DialogResult dialogResult = XtraMessageBox.Show(Resources.ApplicationMustCloseAreYouSure, Resources.ApplicationMustCloseAreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Program.Settings.Culture = Enum<eCultureInfo>.ToLocalizedString(eCulture);
                Application.Restart();
            }
        }

        private void barButtonSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            Program.ShowSettings();
        }

        private void barButtonItemSendChanges_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<object> selectedObjects = ((List<object>)repositoryItemLookUpEditPOSSendChanges.GetCheckedItems());
            if (selectedObjects.Count() <= 0)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.PleaseSelectARecord, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            List<Guid> selectedPOSs = new List<Guid>();
            foreach (object posGuid in selectedObjects)
            {
                selectedPOSs.Add((Guid)posGuid);
            }

            ePosCommand eCommand = ePosCommand.SEND_CHANGES;
            using (SelectEntitiesToUpdateOnPOSForm selectEntitiesToUpdateOnPOSForm = new SelectEntitiesToUpdateOnPOSForm(selectedPOSs, eCommand, true))
            {
                selectEntitiesToUpdateOnPOSForm.ShowDialog();
            }


        }

        private void barButtonItemSendCommand_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<object> selectedObjects = ((List<object>)repositoryItemCheckedComboBoxEditPOSSendCommands.GetCheckedItems());
            if (selectedObjects.Count() <= 0 || barEditItemPOSSendAutoCommands.EditValue == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.PleaseSelectARecord, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            List<Guid> selectedPOSs = new List<Guid>();
            foreach (object posGuid in selectedObjects)
            {
                selectedPOSs.Add((Guid)posGuid);
            }

            ePosCommand eCommand = (ePosCommand)barEditItemPOSSendAutoCommands.EditValue;
            if (eCommand == ePosCommand.RELOAD_ENTITIES)
            {
                using (SelectEntitiesToUpdateOnPOSForm selectEntitiesToUpdateOnPOSForm = new SelectEntitiesToUpdateOnPOSForm(selectedPOSs, eCommand, false))
                {
                    selectEntitiesToUpdateOnPOSForm.ShowDialog();
                }
            }
            else
            {
                try
                {
                    using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                    {

                        List<POSCommandDescription> commands = new List<POSCommandDescription>();

                        foreach (Guid posGuid in selectedPOSs)
                        {
                            POSCommandDescription posCommandDescription = new POSCommandDescription();
                            posCommandDescription.POSOid = posGuid;

                            SerializableTuple<ePosCommand, string> tuple = new SerializableTuple<ePosCommand, string>();
                            tuple.Item1 = eCommand;
                            tuple.Item2 = "";
                            POSCommandSet posCommandSet = new POSCommandSet();
                            posCommandSet.Commands = new List<SerializableTuple<ePosCommand, string>>() { tuple };
                            posCommandSet.Expire = DateTime.Now.AddSeconds(60).Ticks;
                            posCommandDescription.POSCommandSet = posCommandSet;
                            commands.Add(posCommandDescription);
                        }

                        itsService.SendPOSCommands(commands.ToArray());
                    }
                }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(exception.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void repositoryItemLookUpEditPOSSendAutoCommands_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                barEditItemPOSSendAutoCommands.EditValue = null;
            }
        }

        private void barButtonItemBrowser_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start(Program.Settings.StoreControllerURL);
        }

        private void ribbonControlMain_Layout(object sender, LayoutEventArgs e)
        {

        }

        void ItemLinks_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (!IsClosing)
            {
                Program.Settings.QuickAccessToolbarLayout = GetLayout(this.Ribbon.Toolbar);
            }
        }

        private void RestoreLayout(ISupportXtraSerializer view, string setting)
        {
            if (string.IsNullOrWhiteSpace(setting))
            {
                if (view == grdViewDocuments)
                {
                    SetDefaultDocumentGridViewLayout();
                }
                return;
            }
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(setting);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    view.RestoreLayoutFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Info(ex, "Error Restoring Layout");
            }
        }

        private void SetDefaultDocumentGridViewLayout()
        {
            if (panelFilter.Controls.Count > 0)
            {
                if (panelFilter.Controls[0] is ProformaDocumentFilter)
                {
                    grcCustomer.Visible = true;
                    grcSupplier.Visible = false;
                    grcSecondaryStore.Visible = false;
                }
                else if (panelFilter.Controls[0] is SalesDocumentFilter)
                {
                    grcCustomer.Visible = true;
                    grcSupplier.Visible = false;
                    grcSecondaryStore.Visible = false;
                }
                else if (panelFilter.Controls[0] is PurchaseDocumentFilter)
                {
                    grcCustomer.Visible = false;
                    grcSupplier.Visible = true;
                    grcSecondaryStore.Visible = false;
                }
                else if (panelFilter.Controls[0] is StoreDocumentFilter)
                {
                    grcCustomer.Visible = false;
                    grcSupplier.Visible = false;
                    grcSecondaryStore.Visible = true;
                }
            }
        }

        internal static string GetLayout(ISupportXtraSerializer view)
        {
            using (Stream str = new MemoryStream())
            {
                view.SaveLayoutToStream(str);
                str.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(str))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        private void repositoryItemLookUpEditPOSSendChanges_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

            POSListDisplayText(sender, e);
        }

        private static void POSListDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            RepositoryItemCheckedComboBoxEdit cmb = sender as RepositoryItemCheckedComboBoxEdit;
            if (String.IsNullOrWhiteSpace(e.DisplayText) == false && cmb != null)
            {
                e.DisplayText = "";
                Dictionary<Guid, string> datasource = cmb.DataSource as Dictionary<Guid, string>;
                IEnumerable<object> selectedValues = e.Value as IEnumerable<object>;
                if (datasource != null && selectedValues != null)
                {
                    IEnumerable<string> datasourceWhereSelect = datasource.Where(x => selectedValues.Contains(x.Key)).Select(x => x.Value);
                    if (datasourceWhereSelect.Count() > 0)
                    {
                        e.DisplayText = datasourceWhereSelect.Aggregate((f, s) => f + "," + s);
                    }
                }
            }
        }

        private void repositoryItemCheckedComboBoxEditPOSSendCommands_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            POSListDisplayText(sender, e);
        }


        private void btnNewStoreDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateDocument(eDivision.Store);
        }

        private void barButtonItemImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (ImportForm frm = new ImportForm())
            {
                frm.ShowDialog();
            }
        }


        private void barButtonItemViewCustomer_ItemClick(object sender, ItemClickEventArgs e)
        {
            Customer customer = GetSelectedCustomer();
            if (customer == null) return;
            ShowCustomerForm(customer, true);
        }

        private void barButtonItemAddSupplier_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                SupplierNew supplier = new SupplierNew(uow);
                supplier.Trader = new Trader(supplier.Session);
                supplier.Owner = supplier.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                ShowSupplierForm(supplier, false);
                this.grdViewSuppliers.RefreshData();
            }
        }

        private void barButtonItemEditSupplier_ItemClick(object sender, ItemClickEventArgs e)
        {
            SupplierNew supplier = GetSelectedSupplier();
            if (supplier == null)
            {
                return;
            }
            ShowSupplierForm(supplier, false);
            supplier.Session.Reload(supplier, true);
            //this.grdViewSuppliers.RefreshData();
        }

        private void barButtonItemViewSupplier_ItemClick(object sender, ItemClickEventArgs e)
        {
            SupplierNew supplier = GetSelectedSupplier();
            if (supplier == null) return;
            ShowSupplierForm(supplier, true);
        }

        private void barButtonItemAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Item item = new Item(uow);
                item.Owner = item.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                ShowItemForm(item, false);
                gridControlMain.RefreshDataSource();
            }
        }

        private void barButtonItemEditItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            Item item = GetSelectedItem();
            if (item == null) return;

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ShowItemForm(uow.GetObjectByKey<Item>(item.Oid), false);
                item.Session.Reload(item, true);
                filterControls["Item"].Search();
            }
        }

        private void barButtonItemDeleteItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            Item item = GetSelectedItem();
            if (item == null || DialogResult.Yes != XtraMessageBox.Show(Resources.ConfirmDelete, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                uow.GetObjectByKey<Item>(item.Oid).Delete();
                uow.CommitTransaction();
                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
                XtraMessageBox.Show(Resources.SuccesfullyDeleted, Resources.SuccesfullyDeleted, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Program.Settings.ReadOnlyUnitOfWork.ReloadChangedObjects();
                (panelFilter.Controls[0] as BaseFilterControl).Search();
            }
        }

        private void barButtonItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Item item = GetSelectedItem();
            if (item == null) return;
            ShowItemForm(item, true);
        }

        private void navBarControlNavigation_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            //NavBarItem item = sender as NavBarItem;
            try
            {
                if (e.Link.Item.Name.StartsWith("Report_"))
                {
                    ShowReports((Guid)((NavBarItemWithOID)e.Link.Item).OID, false);
                    uscSecondaryFilterPosition.Controls.Clear();
                }
                else if (e.Link.Item.Name.StartsWith("HQReport_"))
                {
                    ShowReports((Guid)((NavBarItemWithOID)e.Link.Item).OID, true);
                    uscSecondaryFilterPosition.Controls.Clear();
                }
                else if (e.Link.Item.Name == "navBarCashierRegisterDevice")
                {
                    CashRegisters.AddNewItemsInCashRegister x = new CashRegisters.AddNewItemsInCashRegister();
                    x.Show();
                }
                else
                {
                    if (e.Link.Item.Tag != null && String.IsNullOrWhiteSpace(e.Link.Item.Tag.ToString()) == false)
                    {
                        ShowRibbon(e.Link.Item.Tag.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowRibbon(string tag)
        {

            List<RibbonPage> list = this.ribbonControlMain.Pages.Cast<RibbonPage>().ToList();
            list.ForEach(x => x.Visible = false);
            list.Where(x => x.Tag as string == tag).ToList().ForEach(x => x.Visible = true);
            uscSecondaryFilterPosition.Controls.Clear();
            if (tag == "POS" || tag == "UserAction")
            {
                simpleButtonFilters.Visible = false;
                layoutControlItemFilters.Visibility = LayoutVisibility.Never;
                emptySpaceItemFilter.Visibility = LayoutVisibility.Never;
                this.flyoutPanelFilter.HidePopup();
                flyoutPanelFilter.Visible = false;
            }
            panelFilter.Controls.Clear();
            if (filterControls.ContainsKey(tag))
            {
                simpleButtonFilters.Visible = true;
                layoutControlItemFilters.Visibility = LayoutVisibility.Always;
                emptySpaceItemFilter.Visibility = LayoutVisibility.Always;



                BaseFilterControl control = filterControls[tag];
                panelFilter.Controls.Add(control);
                if (control.SecondaryFilterControl != null)
                {
                    control.SecondaryFilterControl.Dock = DockStyle.Fill;
                    uscSecondaryFilterPosition.Controls.Add(control.SecondaryFilterControl);
                    flyoutPanelFilter.Visible = true;
                    this.flyoutPanelFilter.HidePopup();
                    //flyoutPanelFilter.Visible = false;
                    this.simpleButtonFilters.Text = Resources.MoreFilters;
                }
                else
                {
                    flyoutPanelFilter.Visible = true;
                    this.flyoutPanelFilter.ShowPopup();
                    this.flyoutPanelFilter.Focus();
                    this.simpleButtonFilters.Text = Resources.SearchFilters;
                }
                if (control is ProformaDocumentFilter && panelFilter.Controls[0].Name == "Proforma")
                {
                    RestoreLayout(grdViewDocuments, Program.Settings.DocumentProformaGridViewSettings);
                }
                else if (control is ProformaDocumentFilter)
                {
                    RestoreLayout(grdViewDocuments, Program.Settings.DocumentSpecialProformaGridViewSettings);
                }
                else if (control is SalesDocumentFilter)
                {
                    grdViewDocuments.OptionsSelection.MultiSelect = true;
                    grdViewDocuments.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                    RestoreLayout(grdViewDocuments, Program.Settings.DocumentSalesGridViewSettings);
                }
                else if (control is PurchaseDocumentFilter)
                {
                    RestoreLayout(grdViewDocuments, Program.Settings.DocumentPurchaseGridViewSettings);
                }
                else if (control is StoreDocumentFilter)
                {
                    grdViewDocuments.OptionsSelection.MultiSelect = true;
                    grdViewDocuments.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                    RestoreLayout(grdViewDocuments, Program.Settings.DocumentStoreGridViewSettings);
                }
                control.Dock = DockStyle.Fill;
                control.Visible = false;
                control.Visible = true;
                //this.flyoutPanelFilter.ShowPopup();
                //this.flyoutPanelFilter.Focus();
                if (control.Expandable == false)
                {
                    simpleButtonFilters.Visible = false;
                    layoutControlItemFilters.Visibility = LayoutVisibility.Never;
                    emptySpaceItemFilter.Visibility = LayoutVisibility.Never;
                    this.flyoutPanelFilter.HidePopup();
                    flyoutPanelFilter.Visible = false;
                }
                else
                {
                    simpleButtonFilters.Visible = true;
                    layoutControlItemFilters.Visibility = LayoutVisibility.Always;
                    emptySpaceItemFilter.Visibility = LayoutVisibility.Always;
                }
            }
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                switch (tag)
                {
                    case "Supplier":
                        if (grdViewSuppliers.Columns.Contains(gridColSupplierPhone))
                            grdViewSuppliers.Columns.Remove(gridColSupplierPhone);
                        break;
                    case "Customer":
                        if (grdViewCustomers.Columns.Contains(gridColDefaultPhone))
                            grdViewCustomers.Columns.Remove(gridColDefaultPhone);
                        break;
                    default:

                        break;
                }


            }
        }
        private void ShowReports(Guid report, bool HQ)
        {
            try
            {
                LastReportSelectionIsHQ = HQ;
                List<RibbonPage> list = this.ribbonControlMain.Pages.Cast<RibbonPage>().ToList();
                list.ForEach(x => x.Visible = false);
                ribbonPageCustomReports.Visible = true;
                ReportCategory GetVisibleReportCategory = ReportsHelper.GetVisibleReportCategory(Program.Settings.CurrentUser, report);
                simpleButtonFilters.Visible = false;
                layoutControlItemFilters.Visibility = LayoutVisibility.Never;
                emptySpaceItemFilter.Visibility = LayoutVisibility.Never;
                this.flyoutPanelFilter.HidePopup();
                flyoutPanelFilter.Visible = false;
                panelFilter.Controls.Clear();
                gridControlMain.DataSource = null;
                gridControlMain.MainView = this.gridViewReports;
                gridControlMain.DataSource = GetVisibleReportCategory.CustomReports;
                grcReportsTitle.FieldName = "Description";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        Dictionary<string, BaseFilterControl> filterControls = new Dictionary<string, BaseFilterControl>();

        private void InitializeSearchFilterPanels()
        {
            filterControls.Add("Proforma", new ProformaDocumentFilter(DocumentHelper.GetProformaTypes(Program.Settings.ReadOnlyUnitOfWork).Select(type => type.Oid).ToList()) { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader), Name = "Proforma" });
            filterControls.Add("SpecialProforma", new ProformaDocumentFilter(DocumentHelper.GetSpecialProformaTypes(Program.Settings.ReadOnlyUnitOfWork).Select(type => type.Oid).ToList()) { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader), Name = "SpecialProforma" });
            filterControls.Add("Sales", new SalesDocumentFilter() { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader) });
            filterControls.Add("Store", new StoreDocumentFilter() { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader) });
            filterControls.Add("Purchase", new PurchaseDocumentFilter() { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader) });
            filterControls.Add("Financial", new FinancialDocumentFilter() { GridView = this.grdViewDocuments, PersistentType = typeof(DocumentHeader) });

            filterControls.Add("Customer", new CustomerFilter() { GridView = this.grdViewCustomers, PersistentType = typeof(Customer) });
            filterControls.Add("Supplier", new SupplierFilter() { GridView = this.grdViewSuppliers, PersistentType = typeof(SupplierNew) });
            filterControls.Add("Tag", new LabelItemChangeFilter() { GridView = this.grdViewLabelsPriceCatalogDetails, PersistentType = null });
            filterControls.Add("TagTerminal", new LabelFilter() { GridView = this.grdViewLabelsDocumentGrid, PersistentType = null, DetailGridView = grdViewDocumentLabelDetails });
            filterControls.Add("Item", new ItemFilter() { GridView = this.grdViewItems, PersistentType = typeof(Item) });
            filterControls.Add("PriceCatalogs", new PriceCatalogFilter() { GridView = this.gridViewPriceCatalogs, PersistentType = typeof(PriceCatalog) });
            filterControls.Add("Leaflets", new LeafletFilter() { GridView = this.gridViewLeaflets, PersistentType = typeof(Leaflet) });
            filterControls.Add("PriceCheck", new PriceCheckFilter() { GridView = this.cardViewPriceCheck });
            //filterControls.Add("CashierController", new CashierItemFilter(null) { GridView = this.grdCashierRegister, PersistentType = typeof(PriceCatalogDetail) });

            filterControls.Select(filterControl => filterControl.Value).ToList().ForEach(baseFilterControl =>
            {
                baseFilterControl.GridControl = this.gridControlMain;
                baseFilterControl.CreateUnitOfWork += CreateUnitOfWork;
                baseFilterControl.SearchComplete += SearchComplete;
            });

            MasterDetailHelper helper = new MasterDetailHelper(cardViewPriceCheck, ViewType.Grid);
            helper.CreateDetail();
        }

        private void SearchComplete(object sender, EventArgs e)
        {
            this.flyoutPanelFilter.HidePopup();
        }

        private void CreateUnitOfWork(BaseFilterControl sender, SearchEventArgs e)
        {
            RefreshGridUow();
            e.UnitOfWork = this.GridUnitOfWork;
        }



        private void simpleButtonFilter_Click(object sender, EventArgs e)
        {
            if (flyoutPanelFilter.IsPopupOpen)
            {
                this.flyoutPanelFilter.HidePopup();
            }
            else if (panelFilter.Controls.Cast<Control>().FirstOrDefault() is BaseFilterControl)
            {
                this.flyoutPanelFilter.ShowPopup();
                this.flyoutPanelFilter.Focus();
            }
        }

        private void flyoutPanelFilter_ButtonClick(object sender, FlyoutPanelButtonClickEventArgs e)
        {
            Control control = panelFilter.Controls.Cast<Control>().FirstOrDefault();
            if (control is BaseFilterControl)
            {
                BaseFilterControl filterControl = (BaseFilterControl)control;
                switch (e.Button.Tag.ToString())
                {
                    case "Search":
                        filterControl.Search();
                        break;
                    case "Clear":
                        filterControl.SearchFilter.Reset();
                        break;
                }
            }
        }

        private void barButtonItemClearPersonalSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            Program.Settings.CustomerGridViewSettings = "";
            Program.Settings.DocumentProformaGridViewSettings = "";
            Program.Settings.DocumentSalesGridViewSettings = "";
            Program.Settings.DocumentPurchaseGridViewSettings = "";
            Program.Settings.DocumentStoreGridViewSettings = "";
            Program.Settings.DocumentLabelGridViewSettings = "";
            Program.Settings.ItemGridViewSettings = "";
            Program.Settings.LabelDocumentGridViewSettings = "";
            Program.Settings.LabelPCDGridViewSettings = "";
            Program.Settings.LayoutPurchaseTotal = "";
            Program.Settings.LayoutSalesTotal = "";
            Program.Settings.PosGridViewSettings = "";
            Program.Settings.PurchaseDocumentDetail = "";
            Program.Settings.PurchaseDocumentHeader = "";
            Program.Settings.QuickAccessToolbarLayout = "";
            Program.Settings.SalesDocumentDetail = "";
            Program.Settings.SalesDocumentHeader = "";
            Program.Settings.SupplierGridViewSettings = "";

            if (DialogResult.Yes == XtraMessageBox.Show(Resources.ApplicationMustCloseAreYouSure, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Application.Restart();
            }
        }

        private void barEditItemPOSSendAutoCommands_ItemPress(object sender, ItemClickEventArgs e)
        {
            this.barEditItemPOSSendCommands.EditValue = null;
        }

        private void grdLabelsPriceCatalogDetails_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == grcPcdLabelPrintedOnDate && e.Value is DateTime && (DateTime)e.Value == DateTime.MinValue)
            {
                e.DisplayText = String.Empty;
            }
        }

        private void grdViewLabelsPriceCatalogDetails_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            SetLabelSelectionCount();
        }

        private void SetLabelSelectionCount()
        {
            if (gridControlMain.MainView == grdViewLabelsPriceCatalogDetails)
            {
                emptySpaceItemFilter.Text = string.Format(Resources.SelectedXLabels, grdViewLabelsPriceCatalogDetails.GetSelectedRows().Length);
            }
            else if (gridControlMain.MainView == grdViewLabelsDocumentGrid)
            {
                int total = 0;
                if (detailCache != null)
                {
                    total = detailCache.Select(x => x.Value.GetSelectedRows().Length).Sum();
                }
                emptySpaceItemFilter.Text = string.Format(Resources.SelectedXLabels, total);
            }
            else
            {
                emptySpaceItemFilter.Text = " ";
            }
        }

        private void gridControlMain_DataSourceChanged(object sender, EventArgs e)
        {
            SetLabelSelectionCount();
        }

        private void grdViewDocumentLabelDetails_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            SetLabelSelectionCount();
        }

        private void grdViewLabelsDocumentGrid_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            SetLabelSelectionCount();
        }

        private void grdViewLabelsDocumentGrid_MasterRowCollapsing(object sender, MasterRowCanExpandEventArgs e)
        {
            e.Allow = true;
        }


        private void barButtonItemAddPriceCatalog_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
            {
                return;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                PriceCatalog priceCatalog = new PriceCatalog(uow);
                priceCatalog.Owner = uow.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                ShowPriceCatalogForm(priceCatalog, false);
                this.gridViewPriceCatalogs.RefreshData();
            }
        }

        public void ShowPriceCatalogForm(PriceCatalog priceCatalog, bool preview = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (PriceCatalogEditForm form = new PriceCatalogEditForm(priceCatalog, preview))
            {
                form.ShowDialog();
            }
        }

        private void barButtonItemViewPriceCatalog_ItemClick(object sender, ItemClickEventArgs e)
        {
            PriceCatalog priceCatalog = GetSelectedPriceCatalog();
            if (priceCatalog == null)
            {
                return;
            }
            ShowPriceCatalogForm(priceCatalog, true);
        }

        private PriceCatalog GetSelectedPriceCatalog()
        {
            PriceCatalog priceCatalog = (gridControlMain.MainView as GridView).GetFocusedRow() as PriceCatalog;
            if (priceCatalog == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return priceCatalog;
        }

        private void barButtonItemPriceCatalogEdit_ItemClick(object sender, ItemClickEventArgs e)
        {

            PriceCatalog priceCatalog = GetSelectedPriceCatalog();
            if (priceCatalog == null)
            {
                return;
            }

            if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER
              && (priceCatalog.IsEditableAtStore == null || priceCatalog.IsEditableAtStore.Oid != Program.Settings.StoreControllerSettings.Store.Oid)
              )
            {
                XtraMessageBox.Show(Resources.YouCannotEditThisElement, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ShowPriceCatalogForm(uow.GetObjectByKey<PriceCatalog>(priceCatalog.Oid), false);
                //priceCatalog.Session.Reload(priceCatalog, true);//UI thread crashes.Why???
                this.gridViewPriceCatalogs.RefreshData();
            }
        }

        private void barButtonItemNewFinancialDocument_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateDocument(eDivision.Financial);
        }

        private static PriceCatalogDetail GetPriceCatalogDetail(DocumentDetail documentDetail, DocumentHeader documentHeader, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy)
        {
            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(documentHeader.Session as UnitOfWork,
                                                                effectivePriceCatalogPolicy,
                                                                documentDetail.Item);
            return priceCatalogPolicyPriceResult == null || priceCatalogPolicyPriceResult.PriceCatalogDetail == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
        }

        private void btbViewDevice_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var selectedDeviceOid = cboCashierRegister.EditValue;
                if (selectedDeviceOid != null)
                {

                    //using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    //{
                    _CashRegisterDevice = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<ITS.Retail.Model.POS>(selectedDeviceOid);
                    DeviceSettings settings = (_CashRegisterDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice).DeviceSettings;
                    CashRegisterFactory cashRegisterFactory = new CashRegisterFactory();
                    ConnectionType connectionType = _CashRegisterDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice.ConnectionType;


                    POSDevice deviceSettings = (_CashRegisterDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice);

                    if (filterControls.Where(x => x.Key.Contains("CashierController")).Count() > 0)
                    {
                        filterControls.Remove("CashierController");
                    }

                    filterControls.Add("CashierController", new CashierItemFilter(deviceSettings) { GridView = this.grdViewAddItemsToCash, PersistentType = typeof(ItemCashRegister) });


                    KeyValuePair<string, BaseFilterControl> CashierController = filterControls.FirstOrDefault(filterControl => filterControl.Value is CashierItemFilter);
                    CashierController.Value.GridControl = this.gridControlMain;
                    CashierController.Value.CreateUnitOfWork += CreateUnitOfWork;
                    CashierController.Value.SearchComplete += SearchComplete;
                    ShowRibbon("CashierController");
                }
                else
                {
                    XtraMessageBox.Show(Environment.NewLine + ResourcesLib.Resources.NoSelectedAnyDevice, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(Environment.NewLine + exception.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private CashRegisterHardware GetCashRegister(ITS.Retail.Model.POS cashierDevice)
        {
            try
            {
                DeviceSettings settings = (cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice).DeviceSettings;
                CashRegisterFactory cashRegisterFactory = new CashRegisterFactory();
                ConnectionType connectionType = cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice.ConnectionType;
                CashRegisterHardware cashRegisterHardware = cashRegisterFactory.GetCashRegisterHardware(settings.DeviceType, settings, cashierDevice.Name, cashierDevice.ID, connectionType, settings.LineChars, settings.CommandChars);
                return cashRegisterHardware;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void barButtonXreport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);
                string message = string.Empty;
                cashRegisterHardware.IssueXReport(out message);
                if (message != "")
                {
                    SplashScreenManager.CloseForm();
                    MessageBox.Show(message);
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void barButtonCashRegisterDailyItemSales_ItemClick(object sender, ItemClickEventArgs e)
        {
            //try
            //{
            //    using (CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice))
            //    {
            //        try
            //        {
            //            string message = string.Empty;
            //            //cashRegisterHardware.GetDailySalesOfItem(out message);
            //            List<string> mashineItemSalesResults = new List<string>();
            //            //mashineItemSalesResults.Add(message);
            //            bool ExceptionHappent = false;
            //            do
            //            {
            //                try
            //                {
            //                    cashRegisterHardware.GetDailySalesOfItem(out message);
            //                    if (message.Contains("The requested fiscal record number is wrong"))
            //                    {
            //                        break;
            //                    }
            //                    mashineItemSalesResults.Add(message);
            //                }
            //                catch (Exception ex)
            //                {
            //                    ExceptionHappent = true;
            //                    break;
            //                    //continue;
            //                }
            //            }
            //            while (!ExceptionHappent);
            //            DailyItemSales DailyTotalItemSales = new DailyItemSales(mashineItemSalesResults);
            //            DailyTotalItemSales.ShowDialog();
            //        }
            //        catch (Exception ex)
            //        {
            //            XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }

        private void barButtonCahsRegiserDailySales_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string message = string.Empty;
                using (CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice))
                {
                    try
                    {
                        cashRegisterHardware.GetTotalSalesOfDay(out message);
                        TotalDailySales totalDailySales = new TotalDailySales(cashRegisterHardware, message);
                        totalDailySales.ShowDialog();
                    }
                    catch (Exception ex)
                    {

                        XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void barButtonDeviceInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string message = string.Empty;
                CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);

                cashRegisterHardware.GetDeviceInfo(out message);
                string[] result = message.Split('/');

                cashRegisterHardware.GetDeviceParameters(out message);
                string[] DeviceParameters = message.Split('/');

                string GeneralInformationDevice = string.Format("" + Resources.RegistrationNumber + "\n" + Resources.RegistrationDescr + "\n" + Resources.Model + "\n" + Resources.Firmware + "\n" + Resources.CompanyDeviceName + "\n", result[0], result[1], result[2], result[3], result[4], DeviceParameters[16]);
                MessageBox.Show(GeneralInformationDevice, "" + Resources.DeviceInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void PrepareItemsToSend()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                List<ItemCashRegister> dataToSend = new List<ItemCashRegister>();
                Dictionary<int, ItemCashRegister> dictionaryItems = new Dictionary<int, ItemCashRegister>();
                int deviceIndex = 0;
                int lastIndex = 0;
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<Item> itemsToDevice = new XPCollection<Item>(_Uow, new NotOperator(new NullOperator("CashierDeviceIndex")));
                    if (itemsToDevice.ToList().Count > 0)
                    {
                        var strLastIndex = itemsToDevice.Max(x => Convert.ToInt32(x.CashierDeviceIndex)).ToString();
                        Int32.TryParse(strLastIndex, out lastIndex);
                        if (lastIndex == 0 && itemsToDevice.ToList().Count > 0)
                        {

                            throw new Exception("Parse Error. Value is null or empty");
                        }
                    }
                }
                List<ItemCashRegister> sourceList = gridControlMain.DataSource as List<ItemCashRegister>;

                for (int i = 0; i < (gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).SelectedRowsCount; i++)
                {
                    int row = ((gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).GetSelectedRows()[i]);
                    ItemCashRegister currentSelectedRow = (gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).GetRow(row) as ItemCashRegister;
                    if (currentSelectedRow == null)
                    {
                        return;
                    }
                    dataToSend.Add(currentSelectedRow);
                    dictionaryItems.Add(row, currentSelectedRow);
                }

                string message = string.Empty;
                CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);

                dataToSend = (cashRegisterHardware as RBSElioCashRegister).PreparationItemsToAdd(dictionaryItems, dataToSend, lastIndex, deviceIndex, _Uow);
                gridControlMain.Refresh();
                foreach (var currentItem in dataToSend)
                {
                    var itemID = sourceList.FirstOrDefault(x => x.Item.Oid == currentItem.Item.Oid);//.Select(x => x.Item.CashierDeviceIndex = deviceIndex.ToString());
                    if (itemID != null && currentItem.Item.CashierDeviceIndex != null)
                    {
                        itemID.Item.CashierDeviceIndex = currentItem.Item.CashierDeviceIndex.ToString();
                    }
                }
                gridControlMain.DataSource = null;
                gridControlMain.DataSource = sourceList;
                gridControlMain.Refresh();
                SplashScreenManager.CloseForm();
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void barButtonSendItems_ItemClick(object sender, ItemClickEventArgs e)
        {

            PrepareItemsToSend();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string cmd = cmdText.Text;
                string errorMessage = String.Empty;
                CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);
                cashRegisterHardware.SendComand(cmd, out errorMessage);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void grdViewAddItemsToCash_RowStyle(object sender, RowStyleEventArgs e)
        {
            ItemCashRegister itemCahsRegister = grdViewAddItemsToCash.GetRow(e.RowHandle) as ItemCashRegister;
            if (itemCahsRegister != null && itemCahsRegister.Item.CashierDeviceIndex != null)
            {
                //e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Strikeout);
                e.Appearance.ForeColor = Color.Green;
            }
            if (itemCahsRegister != null)
            {
                switch (itemCahsRegister.eSItemtatus)
                {
                    case eCashRegisterItemStatus.WAITING:
                        e.Appearance.BackColor = Color.White;
                        e.Appearance.ForeColor = Color.Black;
                        break;
                    case eCashRegisterItemStatus.SENDED:
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.ForeColor = Color.White;
                        break;
                    case eCashRegisterItemStatus.FAILURE:
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.White;
                        break;
                    default:
                        e.Appearance.BackColor = Color.White;
                        e.Appearance.ForeColor = Color.Black;
                        break;
                }
            }
        }

        private void barButtonInsertAllItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DialogResult result = XtraMessageBox.Show(Environment.NewLine + ResourcesLib.Resources.DoYouWantClearAllData, Resources.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                    gridControlMain.MainView = this.grdViewAddItemsToCash;
                    POSDevice cashRegisterDevice = (_CashRegisterDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice);
                    CriteriaOperator criteria = CriteriaOperator.And(new NotOperator(new NullOperator("CashierDeviceIndex")),
                                                    new ContainsOperator("ItemAnalyticTrees", new BinaryOperator("Node.Oid", cashRegisterDevice.ItemCategory.Oid)));

                    Customer DefaultCustomer = Program.Settings.StoreControllerSettings.DefaultCustomer;
                    XPCollection<Item> items = new XPCollection<Item>(_Uow, criteria);

                    string message = string.Empty;
                    CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);
                    Dictionary<Guid, ItemCashRegister> dictItemsToRemove = new Dictionary<Guid, ItemCashRegister>();



                    List<ItemCashRegister> itemCashRegister = new List<ItemCashRegister>();
                    foreach (Item currentItem in items)
                    {
                        itemCashRegister.Add(new ItemCashRegister() { Item = currentItem });
                    }

                    (cashRegisterHardware as RBSElioCashRegister).PreparationDeleteAllItems(itemCashRegister, ResourcesLib.Resources.Item, items, _Uow);
                    gridControlMain.DataSource = null;
                    gridControlMain.Refresh();
                    SplashScreenManager.CloseForm();
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void repositoryItemLookUpEditCashier_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Glyph)
            {
                cboCashierRegister.EditValue = null;
            }
        }

        private void barButtonInserAllPaymentTypes_ItemClick(object sender, ItemClickEventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            string failurePaymentTypes = "";
            string HexMessage = String.Empty;
            CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);
            XPCollection<PaymentMethod> paymentMethods = new XPCollection<PaymentMethod>(_Uow);
            foreach (PaymentMethod currentPaymentMethod in paymentMethods)
            {
                try
                {
                    int paymentCode = 0;
                    Int32.TryParse(currentPaymentMethod.Code, out paymentCode);
                    cashRegisterHardware.ProgramPaymentType(paymentCode, currentPaymentMethod.Description, true, out HexMessage);
                }
                catch (Exception ex)
                {
                    failurePaymentTypes += string.Format(ResourcesLib.Resources.FaildToAddPaymentMethod, currentPaymentMethod.Code, currentPaymentMethod.Description);
                    continue;
                }
            }

            if (failurePaymentTypes.Length > 0)
            {
                XtraMessageBox.Show(Environment.NewLine + failurePaymentTypes, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            SplashScreenManager.CloseForm();
        }

        private DocumentHeader CreateDocumentForZIssue(List<ItemSales> itemSales, List<CashRegisterPaymentMethods> paymentMethods, POSDevice posDevice)
        {
            try
            {
                if (itemSales.Count < 0)
                {
                    return null;
                }

                DocumentType documentType = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentType>(posDevice.DocumentType.Oid);
                DocumentSeries documentSeries = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentSeries>(posDevice.DocumentSeries.Oid);
                DocumentStatus documentStatus = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentStatus>(posDevice.DocumentStatus.Oid);
                DocumentHeader documentHeader = DocumentHelper.CreateDocumentHeaderForCashier(_Uow, itemSales, documentType, documentSeries, documentStatus, Program.Settings.StoreControllerSettings.DefaultCustomer, Program.Settings.StoreControllerSettings.Store, Program.Settings.CurrentUser, paymentMethods, posDevice);
                return documentHeader;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }
        private void barButtonZreport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);

                SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                List<ItemSales> itemSales = (cashRegisterHardware as RBSElioCashRegister).GetItemDailySales();
                List<CashRegisterPaymentMethods> paymentDevice = (cashRegisterHardware as RBSElioCashRegister).LoadDailyPaymentMethods(_Uow);

                POSDevice posDevice = _CashRegisterDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice;
                DocumentHeader documentHeader = CreateDocumentForZIssue(itemSales, paymentDevice, posDevice);

                string abcPath = _CashRegisterDevice.ABCDirectory;
                int zReportNumber = 0;
                string pathToEJFiles;



                //Get daily totals 
                string messageDailyTotals = "";
                cashRegisterHardware.GetTotalSalesOfDay(out messageDailyTotals);
                decimal Totals = 0;
                DailyTotal dailyTotal = new DailyTotal();

                string messageDevice = "";
                cashRegisterHardware.GetDeviceInfo(out messageDevice);

                //Issue Z rerport
                string message = string.Empty;
                string status = string.Empty;
                eDeviceCheckResult result = cashRegisterHardware.IssueZReportCashierRegister(abcPath, out zReportNumber, out pathToEJFiles, out message);


                //check succcess get information totals and calculate daily totals
                if (result == eDeviceCheckResult.SUCCESS && documentHeader != null)
                {
                    documentHeader.Save();
                    XpoHelper.CommitTransaction(_Uow);
                    String[] splittedString = message.Split('/');

                    //int.TryParse(splittedString[2], out zReportNumber);

                    string dt = splittedString[0];

                    string dd = dt.Substring(0, dt.Length - 4);
                    string mm = dt.Substring(2, dt.Length - 4);
                    string yy = dt.Substring(4, dt.Length - 4);

                    string tm = splittedString[1];

                    string HH = tm.Substring(0, tm.Length - 4);
                    string min = tm.Substring(2, tm.Length - 4);
                    string ss = tm.Substring(4, tm.Length - 4);

                    string NewZDate = string.Format("{0}-{1}-20{2} {3}:{4}:{5}", dd, mm, yy, HH, min, ss);
                    DateTime zDate = DateTime.Parse(String.Format("{0:dd-MM-yyyy HH:mm:ss}", NewZDate));

                    if (messageDailyTotals != "" && messageDevice != "")
                    {
                        string[] devicResult = messageDevice.Split('/');
                        dailyTotal = (cashRegisterHardware as RBSElioCashRegister).CalculateDaylyTotals(messageDailyTotals);
                        Totals = dailyTotal.DailyTotals;


                        (cashRegisterHardware as RBSElioCashRegister).CalculateDailyTotalsExplantionDetails(zDate, zReportNumber, dailyTotal, devicResult[0], _CashRegisterDevice, Program.Settings.CurrentUser, _Uow);
                    }
                }
                else
                {
                    XtraMessageBox.Show(Environment.NewLine + ResourcesLib.Resources.ZIssueFaildPleaseTryAgain, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                SplashScreenManager.CloseForm();
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void DeleteSelectedItemsFromDevice()
        {
            DialogResult result = XtraMessageBox.Show(Environment.NewLine + ResourcesLib.Resources.DoYouWantToDeleteCheckedItems, Resources.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                try
                {
                    SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                    List<ItemCashRegister> itemsToRemove = new List<ItemCashRegister>();
                    List<Item> faildToRemoveItemList = new List<Item>();

                    for (int i = 0; i < (gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).SelectedRowsCount; i++)
                    {
                        int row = ((gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).GetSelectedRows()[i]);
                        ItemCashRegister currentSelectedRow = (gridControlMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).GetRow(row) as ItemCashRegister;
                        if (currentSelectedRow == null)
                        {
                            return;
                        }
                        itemsToRemove.Add(currentSelectedRow);
                    }

                    string message = string.Empty;
                    CashRegisterHardware cashRegisterHardware = GetCashRegister(_CashRegisterDevice);
                    List<ItemCashRegister> sourceList = gridControlMain.DataSource as List<ItemCashRegister>;

                    List<ItemCashRegister> resultSource = (cashRegisterHardware as RBSElioCashRegister).PreperationDeleteSpecificItems(itemsToRemove, ResourcesLib.Resources.Item, sourceList, _Uow);
                    gridControlMain.DataSource = null;
                    gridControlMain.DataSource = resultSource;
                    gridControlMain.Refresh();
                    SplashScreenManager.CloseForm();
                }
                catch (Exception ex)
                {
                    SplashScreenManager.CloseForm();
                    XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void gridControlMain_Click(object sender, EventArgs e)
        {

        }

        private void barButtonDeleteSelectedItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            DeleteSelectedItemsFromDevice();
        }

        private void gridViewReports_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                object Selection = gridViewReports.GetRow(gridViewReports.GetSelectedRows()[0]) as CustomReport;
                if (Selection.GetType().ToString() == "ITS.Retail.Model.CustomReport")
                {
                    if (LastReportSelectionIsHQ == false)
                    {
                        string title = "";
                        string description = "";
                        XtraReportBaseExtension xtraReport = ReportsHelper.GetXtraReport(((ITS.Retail.Model.CustomReport)Selection).Oid, Program.Settings.StoreControllerSettings.Owner, Program.Settings.CurrentUser, null, out title, out description);
                        ReportPrintTool printTool = new ReportPrintTool(xtraReport);
                        printTool.ShowRibbonPreview();
                    }
                    else
                    {
                        HQReportBrowser x = new HQReportBrowser();
                        x.ShowReport(((ITS.Retail.Model.CustomReport)Selection).Oid);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btbSendItemsToCashier_ItemClick(object sender, ItemClickEventArgs e)
        {
            CashRegisters.AddNewItemsInCashRegister x = new CashRegisters.AddNewItemsInCashRegister();
            x.Show();
        }
        private void barButtonItemViewLeaflet_ItemClick(object sender, ItemClickEventArgs e)
        {
            Leaflet Leaflet = GetSelectedLeaflet();
            if (Leaflet == null)
            {
                return;
            }
            ShowLeafletForm(Leaflet, true);
        }

        private Leaflet GetSelectedLeaflet()
        {
            Leaflet Leaflet = (gridControlMain.MainView as GridView).GetFocusedRow() as Leaflet;
            if (Leaflet == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return Leaflet;
        }

        public void ShowLeafletForm(Leaflet Leaflet, bool preview = false)
        {
            SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
            using (LeafletForm form = new LeafletForm(Leaflet, preview))
            {
                form.ShowDialog();
            }
        }

        private void Copies_ItemClick(object sender, EventArgs e)
        {

            //Copies.EditValue = edit.Text;
        }



        private void grdViewDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                GridView gridView = gridControlMain.FocusedView as GridView;
                int[] rows = grdViewDocuments.GetSelectedRows();
                if (rows == null || rows.Count() < 1)
                {
                    SelectedDocumentOids = new List<Guid>();
                    return;
                }
                else if (e.Action == CollectionChangeAction.Refresh)
                {
                    foreach (var rowIndex in rows)
                    {
                        try
                        {
                            var handle = grdViewDocuments.GetRowHandle(rowIndex);
                            var row = grdViewDocuments.GetRow(handle);
                            if (row != null)
                            {
                                DocumentHeader doc = row as DocumentHeader;
                                if (doc != null && !SelectedDocumentOids.Contains(doc.Oid))
                                {
                                    SelectedDocumentOids.Add(doc.Oid);
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                else
                {
                    DocumentHeader document = gridView.GetRow((sender as GridView).FocusedRowHandle) as DocumentHeader;
                    if (document != null && !SelectedDocumentOids.Contains(document.Oid) && e.Action == CollectionChangeAction.Add)
                    {
                        SelectedDocumentOids.Add(document.Oid);
                    }
                    else if (document != null && SelectedDocumentOids.Contains(document.Oid) && e.Action == CollectionChangeAction.Remove)
                    {
                        SelectedDocumentOids.Remove(document.Oid);
                    }
                }
            }
            catch (Exception ex)
            {
                grdViewDocuments.DeleteSelectedRows();
                SelectedDocumentOids = new List<Guid>();
            }
        }

        private void ShowMergeDocumentDetailGrid()
        {
            if (SelectedDocumentOids != null && SelectedDocumentOids.Count > 0)
            {
                List<MergedDocumentDetail> details = DocumentHelper.MergeDetails(SelectedDocumentOids, null);
                if (details != null && details.Count > 0)
                {
                    using (MergedDocumentDetailsForm form = new MergedDocumentDetailsForm(details))
                    {
                        form.ShowDialog(this);
                    }
                }
            }
            else
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.IsCanceled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void barButtonMergeSalesDocumentDetails_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowMergeDocumentDetailGrid();
        }
        private void barButtonMergePurchaseDocumentDetails_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowMergeDocumentDetailGrid();
        }
        private void barButtonMergeStoreDocumentDetails_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowMergeDocumentDetailGrid();
        }
        private void barButtonMergeFinancialDocumentDetails_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowMergeDocumentDetailGrid();
        }
    }

}
