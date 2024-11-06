using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using ITS.MobileAtStore.Helpers;
using OpenNETCF;
using OpenNETCF.Drawing;
using OpenNETCF.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Common.Utilities.Compact;
//using ITS.MobileAtStore.AuxilliaryClasses;


namespace ITS.MobileAtStore
{
    /// <summary>
    /// Summary description for Main.
    /// </summary>
    public class Main : System.Windows.Forms.Form
    {
        #region Data Members
        private OpenNETCF.Windows.Forms.Button2 btnExit;
        private OpenNETCF.Windows.Forms.Button2 exportOrdersButton;
        private OpenNETCF.Windows.Forms.Button2 exportCompetitionButton;
        private OpenNETCF.Windows.Forms.Button2 exportTagsButton;
        private OpenNETCF.Windows.Forms.Button2 exportInvoiceButton;
        private OpenNETCF.Windows.Forms.Button2 exportInventoryButton;
        public SideBar80x320 mySideBar;
        private Button2 btnLabel;
        private Button2 btnInventory;
        private Button2 btnOrder;
        private Button2 btnCompetition;
        private Button2 btnInvoice;
        private Button2 btnPriceCheck;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button2 backToMainButton;
        private Button2 goToExportButton;
        private TabPage tabPage3;
        private Button2 btnSettings;
        private Button2 btnHelp;
        private Button2 btnFromExportToSettings;
        private Button2 btnFromSettingsToMain;
        private Button2 btnFromSettingsToExport;
        private Button2 btnReceipt;
        private Button2 btnExportReceipts;
        private System.Windows.Forms.Timer timerRefresh;
        private Button2 btnESLInv;
        private Panel panelExportButtons;
        private Button2 btnPriceAtStore;
        private Button2 btnOfflineOrder;
        private Button2 btnOfflineInvoice;
        private TabPage tabPage4;
        private Button2 buttonExportDocuments;
        private Button2 buttonPreviousDocuments;
        private Button2 buttonInvoiceSales;
        private Button2 buttonExportInvoiceSales;
        private Button2 buttonTransfer;
        private Button2 buttonExportTransfer;
        private Button2 btnDecomposition;
        private Button2 btnComposition;
        private Button2 btnQueueBustingQR;
        private System.Windows.Forms.SaveFileDialog dlgExportFile;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the main form and resets the counts of document lines for the side bars
        /// </summary>
        public Main(SideBar80x320 sideBar)
        {
            mySideBar = sideBar;
            InitializeComponent();
            if ((Environment.OSVersion.Platform.ToString() == "WinCE") && (Environment2.OSVersion.Platform.ToString() != "PocketPC"))
            {
                this.WindowState = FormWindowState.Normal;
            }
            if (AppSettings.UseSales == false)
            {
                tabControl1.TabPages.RemoveAt(1);
                goToExportButton.Text = "(9) Εξαγωγή";
                backToMainButton.Text = "(8) Παραστατικά";
            }
            RefreshAllCounts();
            timerRefresh.Enabled = true;
            tabControl1.SelectedIndex = 0;
            this.Paint += new PaintEventHandler(Main.Form_Paint);
        }
        #endregion

        #region Disposer
        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Refreshes all document line counts
        /// </summary>
        private void RefreshAllCounts()
        {
            try
            {
                MobileAtStore.ConnectDataLayers();
                RefreshCount(DOC_TYPES.ORDER);
                RefreshCount(DOC_TYPES.INVENTORY);
                RefreshCount(DOC_TYPES.INVOICE);
                RefreshCount(DOC_TYPES.INVOICE_SALES);
                RefreshCount(DOC_TYPES.TRANSFER);
                RefreshCount(DOC_TYPES.TAG);
                RefreshCount(DOC_TYPES.COMPETITION);
                RefreshCount(DOC_TYPES.RECEPTION);
            }
            finally
            {
                MobileAtStore.DisconnectDataLayers();
            }
        }

        /// <summary>
        /// Refresh a specific document type line count
        /// </summary>
        /// <param name="docType"></param>
        private void RefreshCount(DOC_TYPES docType)
        {
            string result = Document.GetDocItemCount(docType, MobileAtStore.TransactionsDL, false).ToString();
            switch (docType)
            {
                case (DOC_TYPES.ORDER):
                    mySideBar.OrderCount = result;
                    break;
                case (DOC_TYPES.INVENTORY):
                    //mySideBar.InventoryCount = result;
                    break;
                case (DOC_TYPES.INVOICE):
                    mySideBar.InvoiceCount = result;
                    break;
                case (DOC_TYPES.TRANSFER):
                    mySideBar.TransferCount = result;
                    break;
                case (DOC_TYPES.INVOICE_SALES):
                    mySideBar.InvoiceSales = result;
                    break;
                case (DOC_TYPES.TAG):
                    mySideBar.LabelCount = result;
                    break;
                case (DOC_TYPES.COMPETITION):
                    mySideBar.CompetitionCount = result;
                    break;
                case (DOC_TYPES.RECEPTION):
                    mySideBar.ReceiptCount = result;
                    break;
            }
        }

        /// <summary>
        /// Shows the waiting cursor
        /// </summary>
        public static void ShowWaitingCursor()
        {
            Thread.Sleep(200);
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
        }

        /// <summary>
        /// Shows the default cursor
        /// </summary>
        public static void ShowDefaultCursor()
        {
            Application.DoEvents();
            Cursor.Current = Cursors.Default;
            Application.DoEvents();
        }

        /// <summary>
        /// Highlights the export buttons whether they have lines to export or not
        /// </summary>
        private void HighlightExportButtons()
        {
            timerRefresh.Enabled = false;
            MobileAtStore.ConnectDataLayers();
            if (Document.GetDocItemCount(DOC_TYPES.ORDER, MobileAtStore.TransactionsDL, false) > 0)
            {
                exportOrdersButton.Enabled = true;
            }
            else
                exportOrdersButton.Enabled = false;

            using (WRMMobileAtStore.WRMMobileAtStore webService = MobileAtStore.GetWebService(AppSettings.Timeout))
            {
                int result;
                bool resultSetted;
                webService.CountInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out result, out resultSetted);
                exportInventoryButton.Enabled = resultSetted && result > 0;
            }

            if (Document.GetDocItemCount(DOC_TYPES.INVOICE, MobileAtStore.TransactionsDL, false) > 0)
            {
                exportInvoiceButton.Enabled = true;
            }
            else
            {
                exportInvoiceButton.Enabled = false;
            }

            if (Document.GetDocItemCount(DOC_TYPES.INVOICE_SALES, MobileAtStore.TransactionsDL, false) > 0)
            {
                buttonExportInvoiceSales.Enabled = true;
            }
            else
            {
                buttonExportInvoiceSales.Enabled = false;
            }

            if (Document.GetDocItemCount(DOC_TYPES.TRANSFER, MobileAtStore.TransactionsDL, false) > 0)
            {
                buttonExportTransfer.Enabled = true;
            }
            else
            {
                buttonExportTransfer.Enabled = false;
            }

            if (Document.GetDocItemCount(DOC_TYPES.TAG, MobileAtStore.TransactionsDL, false) > 0)
            {
                exportTagsButton.Enabled = true;
            }
            else
                exportTagsButton.Enabled = false;

            if (Document.GetDocItemCount(DOC_TYPES.COMPETITION, MobileAtStore.TransactionsDL, false) > 0)
            {
                exportCompetitionButton.Enabled = true;
            }
            else
                exportCompetitionButton.Enabled = false;
            if (Document.GetDocItemCount(DOC_TYPES.RECEPTION, MobileAtStore.TransactionsDL, false) > 0)
            {
                btnExportReceipts.Enabled = true;
            }
            else
                btnExportReceipts.Enabled = false;
            MobileAtStore.DisconnectDataLayers();
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Paints the form in 0,0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Based on input we navigate the user to other tab pages or document forms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                switch (e.KeyChar)
                {
                    case (char)Keys.D1:
                        this.btnOrder_Click(this, null);
                        break;
                    case (char)Keys.D2:
                        this.btnInventory_Click(this, null);
                        break;
                    case (char)Keys.D3:
                        this.btnInvoice_Click(this, null);
                        break;
                    case (char)Keys.D4:
                        this.btnLabel_Click(this, null);
                        break;
                    case (char)Keys.D5:
                        this.btnCompetition_Click(this, null);
                        break;
                    case (char)Keys.D6:
                        this.btnReceipt_Click(this, null);
                        break;
                    case (char)Keys.D7:
                        this.btnPriceCheck_Click(this, null);
                        break;
                    case (char)Keys.D8:
                        this.btnESLInv_Click(this, null);
                        break;
                    case (char)Keys.D9:
                        this.goToExportButton_Click(this, null);
                        break;
                    case (char)Keys.D0:
                        this.btnExit_Click(this, null);
                        break;
                    case (char)Keys.F11:
                        App.LockDown.Execute(false);
                        break;
                }
            }
            else if (AppSettings.UseSales && this.tabControl1.SelectedIndex == 1)
            {
                switch (e.KeyChar)
                {
                    case (char)Keys.D1:
                        if (this.buttonInvoiceSales.Visible)
                        {
                            this.buttonInvoiceSales_Click(this, null);
                        }
                        break;
                    case (char)Keys.D2:
                        if (this.buttonTransfer.Visible)
                        {
                            this.buttonTransfer_Click(this, null);
                        }
                        break;
                    case (char)Keys.D3:
                        if (this.btnDecomposition.Visible)
                        {
                            this.btnDecomposition_Click(this, null);
                        }
                        break;
                    case (char)Keys.D4:
                        break;
                    case (char)Keys.D5:
                        break;
                    case (char)Keys.D6:
                        break;
                    case (char)Keys.D7:
                        this.buttonPreviousDocuments_Click(this, null);
                        break;
                    case (char)Keys.D9:
                        this.buttonExportDocuments_Click(this, null);
                        break;
                }
            }
            else
                if ((AppSettings.UseSales && this.tabControl1.SelectedIndex == 2) || (AppSettings.UseSales == false && this.tabControl1.SelectedIndex == 1))
                {
                    switch (e.KeyChar)
                    {
                        case (char)Keys.D1:
                            if (this.exportOrdersButton.Visible && this.exportOrdersButton.Enabled)
                            {
                                this.exportOrdersButton_Click(this, null);
                            }
                            break;
                        case (char)Keys.D2:
                            if (this.exportInventoryButton.Visible && this.exportInventoryButton.Enabled)
                            {
                                this.exportInventoryButton_Click(this, null);
                            }
                            break;
                        case (char)Keys.D3:
                            if (this.exportInvoiceButton.Visible && this.exportInvoiceButton.Enabled)
                            {
                                this.exportInvoiceButton_Click(this, null);
                            }
                            break;
                        case (char)Keys.D4:
                            if (this.exportTagsButton.Visible && this.exportTagsButton.Enabled)
                            {
                                this.exportTagsButton_Click(this, null);
                            }
                            break;
                        case (char)Keys.D5:
                            if (this.exportCompetitionButton.Visible && this.exportCompetitionButton.Enabled)
                            {
                                this.exportCompetitionButton_Click(this, null);
                            }
                            break;
                        case (char)Keys.D6:
                            if (this.btnExportReceipts.Visible && this.btnExportReceipts.Enabled)
                            {
                                this.btnExportReceipts_Click(this, null);
                            }
                            break;
                        case (char)Keys.D7:
                            this.buttonExportInvoiceSales_Click(this, null);
                            break;
                        case (char)Keys.D8:
                            this.backToMainButton_Click(this, null);
                            break;
                        case (char)Keys.D9:
                            this.btnFromExportToSettings_Click(this, null);
                            break;
                    }
                }
                else
                    if ((AppSettings.UseSales && this.tabControl1.SelectedIndex == 3) || (AppSettings.UseSales == false && this.tabControl1.SelectedIndex == 2))
                    {
                        switch (e.KeyChar)
                        {
                            case (char)Keys.D7:
                                this.btnFromSettingsToExport_Click(this, null);
                                break;
                            case (char)Keys.D9:
                                this.btnFromSettingsToDocuments_Click(this, null);
                                break;
                        }
                    }
        }

        /// <summary>
        /// Opens the document form as an order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrder_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.ORDER, "", false);
        }


        //private void btnMatching_Click(object sender, EventArgs e)
        //{
        //    timerRefresh.Enabled = false;
        //    MobileAtStore.ConnectDataLayers();
        //    using (MatchingForm form = new MatchingForm())
        //    {
        //        form.ShowDialog();
        //    }
        //    RefreshCount(DOC_TYPES.ORDER);
        //    MobileAtStore.DisconnectDataLayers();
        //    timerRefresh.Enabled = true;
        //}

        /// <summary>
        /// Opens the document form as an inventory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInventory_Click(object sender, EventArgs e)
        {
            timerRefresh.Enabled = false;
            MobileAtStore.ConnectDataLayers();
            using (InvMasterForm f = new InvMasterForm())
            {
                f.ShowDialog();
            }
            MobileAtStore.DisconnectDataLayers();
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Opens the document form as an invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvoice_Click(object sender, System.EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.INVOICE, "", false);
        }

        /// <summary>
        /// Opens the document form as a tag entry form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLabel_Click(object sender, System.EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.TAG, "", false);
        }

        /// <summary>
        /// Opens the price check form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPriceCheck_Click(object sender, EventArgs e)
        {
            timerRefresh.Enabled = false;
            MobileAtStore.ConnectDataLayers();
            using (PriceCheck priceCheck = new PriceCheck())
            {
                priceCheck.ShowDialog();
            }
            MobileAtStore.DisconnectDataLayers();
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Opens the document form as competition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompetition_Click(object sender, System.EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.COMPETITION, "", false);
        }

        private void btnReceipt_Click(object sender, EventArgs e)
        {
            timerRefresh.Enabled = false;
            MobileAtStore.ConnectDataLayers();
            using (MasterReceiptForm mrf = new MasterReceiptForm())
            {
                mrf.ShowDialog();
                RefreshCount(DOC_TYPES.RECEPTION);
            }
            MobileAtStore.DisconnectDataLayers();
            timerRefresh.Enabled = true;
        }

        /// <summary>
        /// Opens the Help form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            using (AboutInfo about = new AboutInfo())
            {
                about.ShowDialog();
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            if (MessageForm.Execute("Ερώτηση", "Είστε σίγουροι πως θέλετε να κλείσετε την εφαρμογή ?", MessageForm.DialogTypes.YESNO, MessageForm.MessageTypes.QUESTION) == DialogResult.Yes)
                this.Close();
        }

        /// <summary>
        /// Process the change of the selected index tab page in the main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.TabPages[tabControl1.SelectedIndex].Text == "Εξαγωγή")
            {
                HighlightExportButtons();
            }

            //if (tabControl1.SelectedIndex == 1)
            //{
            //HighlightExportButtons();
            //CommonUtilities.FillOutputPathsList(listboxOutputLocations, DOC_TYPES.ALL_TYPES);
            //}
            //if (tabControl1.SelectedIndex == 0)
            //{
            //    //Do something when we have switched to the main tab page
            //}
        }

        /// <summary>
        /// Process the export of Order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportOrdersButton_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.ORDER, false))
                this.exportOrdersButton.Enabled = false;

        }

        /// <summary>
        /// Process the export of Inventory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportInventoryButton_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.INVENTORY, false))
            {
                exportInventoryButton.Enabled = false;
            }
        }

        /// <summary>
        /// Process the export of Invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportInvoiceButton_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.INVOICE, false))
                exportInvoiceButton.Enabled = false;
        }

        /// <summary>
        /// Process the export of Tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportTagsButton_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.TAG, false))
                exportTagsButton.Enabled = false;
        }

        /// <summary>
        /// Process the export of Competition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportCompetitionButton_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.COMPETITION, false))
                exportCompetitionButton.Enabled = false;
        }

        private bool InitiateOutputTypeDocumentOnlineExport(DOC_TYPES doc_type, bool forcedOffline)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                MessageForm.Execute("Πληροφορία", "Πρέπει να είστε Online για να πραγματοποιηθεί η εξαγωγή.");
                return false;
            }

            try
            {
                panelExportButtons.Enabled = false;
                timerRefresh.Enabled = false;
                MobileAtStore.ConnectDataLayers();
                //For receipts don't check output locations. otherwise force him to select one
                //if (listboxOutputLocations.SelectedIndex == -1)
                //{
                //    MessageForm.Execute("Πληροφορία", "Παρακαλώ επιλέξτε μία τοποθεσία εξαγωγής απο την λίστα και ξαναπροσπαθήστε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                //    return false;
                //}
                //else
                {
                    //FileExport outputPath = (FileExport)listboxOutputLocations.SelectedItem;
                    //if (!CommonUtilities.OutputPathIncludesDocType(outputPath, doc_type)) //Check if the output path is available for this doc type
                    //{
                    //    MessageForm.Execute("Ειδοποίηση", "Η τοποθεσία εξαγωγής που έχετε επιλέξει δεν είναι διαθέσιμη για τον συγκεκριμένο τύπο παραστατικού");
                    //    return false;
                    //}
                    ShowWaitingCursor();
                    string errorMessage = "";
                    if (ExportDocument(doc_type, forcedOffline, out errorMessage))
                    {
                        RefreshCount(doc_type);
                        ShowDefaultCursor();
                        MessageForm.Execute("Ενημέρωση", "Η Εξαγωγή ήταν επιτυχής\r\n" + errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                ShowDefaultCursor();
                MobileAtStore.DisconnectDataLayers();
                timerRefresh.Enabled = true;
                panelExportButtons.Enabled = true;
            }
            //}
            //else MessageBox.Show("Παρακαλώ ελέγξτε την συνδεσιμότητα με τον Server \n και ξαναπροσπαθήστε", "Πληροφορία", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Exports the receipts to the online database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportReceipts_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.RECEPTION, false))
                btnExportReceipts.Enabled = false;
        }

        public static bool ExportInventory(out string errorMessage)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                errorMessage = "Πληροφορία : Πρέπει να είστε Online για να πραγματοποιηθεί η εξαγωγή.";
                return false;
            }

            using (var service = MobileAtStore.GetWebService(0))
            {
                int result;
                bool boolResult, resultSetted;
                service.CountInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out result, out resultSetted);
                if (result <= 0)
                {
                    errorMessage = "Δεν υπάρχουν είδη προς απογραφή";
                    return false;
                }

                service.UpdateInvLine(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out boolResult, out resultSetted);
                if (boolResult && resultSetted)
                {
                    service.PerformInventoryExport(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out boolResult, out resultSetted, out errorMessage);
                    if (boolResult && resultSetted)
                    {
                        errorMessage = "Επιτυχία : H εξαγωγή ήταν επιτυχής!";
                        return true;
                    }
                    else
                    {
                        //errorMessage = "Αποτυχία : Αποτυχία στην εξαγωγή";
                        return false;
                    }
                }
                else
                {
                    errorMessage = "Αποτυχία : Αποτυχία στην ενημέρωση της βάσης";
                    return false;
                }
            }
        }

        /// <summary>
        /// Tranfers the selected document type to the online database so it will get queued for export
        /// </summary>
        /// <param name="doc_type"></param>
        /// <returns></returns>
        public static bool ExportDocument(DOC_TYPES doc_type, bool forcedOffline, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (doc_type == DOC_TYPES.QUEUE_QR)
            {
                //Print on bluetooth printer
                try
                {
                    Header header;
                    string data = null;
                    using (UnitOfWork uow = new UnitOfWork(MobileAtStore.TransactionsDL))
                    {
                        header = uow.FindObject<Header>(new BinaryOperator("DocType", DOC_TYPES.QUEUE_QR));
                        if (header == null)
                        {
                            MessageForm.Execute("Αποτυχία", "Δεν βρέθηκε παραστατικό", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                            return false;
                        }
                        data = AppSettings.Format.GetPrintFormat(header);

                        if (BluetoothPrinterHelper.Print(data, out errorMessage))
                        {
                            if (DialogResult.No == MessageForm.Execute("Ερώτηση", "Το παραστατικό εκτυπώθηκε σωστά;", MessageForm.DialogTypes.YESNO, MessageForm.MessageTypes.QUESTION))
                            {
                                return false;
                            }
                            try
                            {
                                XPCollection<Line> col = new XPCollection<Line>(uow, new BinaryOperator("Header", header.Oid));
                                col.DeleteObjectOnRemove = true;
                                uow.Delete(col);
                                header.Delete();
                                uow.CommitChanges();
                                MessageForm.Execute("Επιτυχία", "Το παραστατικό εκτυπώθηκε επιτυχώς.", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                            }
                            catch (Exception exception)
                            {
                                errorMessage = exception.Message;
                                MessageForm.Execute("Μερική επιτυχία", "Το παραστατικό εκτυπώθηκε επιτυχώς αλλά δεν αφαιρέθηκε από το φορητό. \r\n" + errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                            }
                            return true;
                        }
                    }
                    MessageForm.Execute("Αποτυχία", errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                    return false;
                }
                catch (Exception exception)
                {
                    errorMessage += exception.Message + "\r\n" + exception.StackTrace;
                    MessageForm.Execute("Αποτυχία", errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                    return false;
                }
            }

            if (doc_type == DOC_TYPES.INVENTORY)
            {
                if (ExportInventory(out errorMessage))
                {
                    return true;
                }
                else
                {
                    MessageForm.Execute("Αποτυχία", errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                    return false;
                }
            }
            Dictionary<Header, string> failedHeaders = new Dictionary<Header, string>();
            try
            {
                bool result, resultSetted;
                using (ITS.MobileAtStore.WRMMobileAtStore.WRMMobileAtStore service = MobileAtStore.GetWebService(0))
                {
                    if (doc_type == DOC_TYPES.INVENTORY)
                    {
                        service.PerformInventoryExport(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, out result, out resultSetted, out errorMessage);
                    }
                    else
                    {
                        using (UnitOfWork uow = new UnitOfWork(MobileAtStore.TransactionsDL))
                        {
                            List<Guid> headersToBeExportedOids = new List<Guid>();
                            using (XPCollection<Header> headersToBeExported = new XPCollection<Header>(uow,
                                CriteriaOperator.And(
                                    new BinaryOperator("DocType", (int)doc_type),
                                    new BinaryOperator("ForcedOffline", forcedOffline)
                                    )
                                )
                                )
                            {
                                foreach (Header head in headersToBeExported)
                                {
                                    headersToBeExportedOids.Add(head.Oid);
                                }
                            }

                            if (headersToBeExportedOids.Count == 0)
                            {
                                MessageForm.Execute("Αποτυχία", "Δεν υπάρχουν παραστατικά.", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                                return false;
                            }

                            while (headersToBeExportedOids.Count > 0)
                            {
                                Header head = uow.GetObjectByKey<Header>(headersToBeExportedOids[0]);
                                Header header = head;
                                DateTime start = DateTime.Now;
                                ITS.MobileAtStore.WRMMobileAtStore.Header cHeader = ConvertHeaderToWebType(header);
                                DateTime stop = DateTime.Now;
                                service.ExportDocument(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, cHeader, out result, out resultSetted, out errorMessage);

                                bool exportSucceeded = result && resultSetted;
                                try
                                {
                                    if (!exportSucceeded)
                                    {
                                        throw new Exception("Η εξαγωγή απέτυχε." + Environment2.NewLine + errorMessage);
                                    }
                                    XPCollection<Line> col = new XPCollection<Line>(uow, new BinaryOperator("Header", header.Oid));
                                    col.DeleteObjectOnRemove = true;
                                    uow.Delete(col);
                                    header.Delete();
                                    uow.CommitChanges();
                                }
                                catch (Exception exception)
                                {
                                    if (!failedHeaders.ContainsKey(header))
                                    {
                                        failedHeaders.Add(header, exception.Message);
                                    }
                                }
                                finally
                                {
                                    headersToBeExportedOids.RemoveAt(0);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Αποτυχία", ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                errorMessage = ex.Message;
                return false;//success = false;
            }
            if (failedHeaders.Count > 0)
            {
                string headerCodes = Environment2.NewLine;
                foreach (var failedHeaderPair in failedHeaders)
                {
                    headerCodes += string.Format("{0} - {1}{2} ", failedHeaderPair.Key.Code, failedHeaderPair.Value, Environment2.NewLine);
                }
                headerCodes = headerCodes.Substring(0, headerCodes.Length - 1);
                MessageForm.Execute("Αποτυχία", "Τα παραστατικά με κωδικό " + headerCodes + " δεν αποθηκεύτηκαν.", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            }
            return failedHeaders.Count == 0;//return success;
        }

        /// <summary>
        /// Switches the data in the local Header object to a web service type Header object so it can be sent over the wire
        /// </summary>
        /// <param name="gHeader"></param>
        /// <returns></returns>
        private static WRMMobileAtStore.Header ConvertHeaderToWebType(Header gHeader)
        {
            WRMMobileAtStore.Header header = new WRMMobileAtStore.Header
            {
                Append = gHeader.Append,
                AppendSpecified = true,
                Code = gHeader.Code,
                CreatedOn = gHeader.CreatedOn,
                CreatedOnSpecified = true,
                CustomerCode = gHeader.CustomerCode,
                CustomerName = gHeader.CustomerName,
                CustomerAFM = gHeader.CustomerAFM,
                DocDate = gHeader.DocDate,
                DocDateSpecified = true,
                DocNumber = gHeader.DocNumber,
                DocNumberSpecified = true,
                OutputId = gHeader.OutputId,
                OutputIdSpecified = true,
                DocStatus = (WRMMobileAtStore.DOC_STATUS)gHeader.DocStatus,
                DocStatusSpecified = true,
                DocType = (WRMMobileAtStore.eDocumentType)gHeader.DocType,
                DocTypeSpecified = true,
                Oid = gHeader.Oid.ToString(),
                TerminalID = gHeader.TerminalID,
                TerminalIDSpecified = true,
                ForcedOffline = gHeader.ForcedOffline,
                ForcedOfflineSpecified = true,
                UpdatedOn = gHeader.UpdatedOn,
                UpdatedOnSpecified = true
            };

            List<WRMMobileAtStore.Line> tmpWebLinesList = new List<WRMMobileAtStore.Line>();
            foreach (Line mobileLine in gHeader.Lines)
            {
                WRMMobileAtStore.Line webLine = new WRMMobileAtStore.Line
                {
                    CreatedOn = mobileLine.CreatedOn,
                    CreatedOnSpecified = true,
                    Counter = mobileLine.Counter,
                    CounterSpecified = true,
                    Header = mobileLine.Header.ToString(),
                    Oid = mobileLine.Oid.ToString(),
                    Uniqueid = mobileLine.Uniqueid.ToString(),
                    ProdBarcode = mobileLine.ProdBarcode,
                    ProdCode = mobileLine.ProdCode,
                    Qty1Specified = true,
                    Qty1 = mobileLine.Qty1,
                    UpdatedOnSpecified = true,
                    UpdatedOn = mobileLine.UpdatedOn,
                    FlyerSpecified = true,
                    Flyer = mobileLine.Flyer,
                    BarcodeParsingResult = mobileLine.BarcodeParsingResult,
                    BarcodeParsingResultSpecified = true,
                    WeightedBarcodeValue = mobileLine.WeightedBarcodeValue,
                    WeightedBarcodeValueSpecified = true,
                    WeightedDecodedBarcode = mobileLine.WeightedDecodedBarcode,
                    LinkedLine = mobileLine.LinkedLine.ToString()

                };

                tmpWebLinesList.Add(webLine);
            }
            header.Details = tmpWebLinesList.ToArray();
            return header;
        }

        /// <summary>
        /// Goes to export tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goToExportButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        /// <summary>
        /// Goes to main form tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backToMainButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex -= 1;
        }

        /// <summary>
        /// Opens the settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        /// <summary>
        /// Goes from settings tab page to documents tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromSettingsToDocuments_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        /// <summary>
        /// Goes from settings tab page to export tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromSettingsToExport_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex -= 1;
        }

        /// <summary>
        /// Goes from export tab page to settings tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFromExportToSettings_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex += 1;
        }


        /// <summary>
        /// Checks if the desktop app sended the signal file that an export has happened, so we can refresh the quantities and other stuff.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (File.Exists(Application2.StartupPath + "\\ExportHappened"))
            {
                try
                {
                    ShowWaitingCursor();
                    RefreshAllCounts();
                    File.Delete(Application2.StartupPath + "\\ExportHappened");
                }
                finally
                {
                    ShowDefaultCursor();
                }
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnExit = new OpenNETCF.Windows.Forms.Button2();
            this.dlgExportFile = new System.Windows.Forms.SaveFileDialog();
            this.btnLabel = new OpenNETCF.Windows.Forms.Button2();
            this.btnInventory = new OpenNETCF.Windows.Forms.Button2();
            this.btnOrder = new OpenNETCF.Windows.Forms.Button2();
            this.btnCompetition = new OpenNETCF.Windows.Forms.Button2();
            this.btnInvoice = new OpenNETCF.Windows.Forms.Button2();
            this.btnPriceCheck = new OpenNETCF.Windows.Forms.Button2();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnPriceAtStore = new OpenNETCF.Windows.Forms.Button2();
            this.btnESLInv = new OpenNETCF.Windows.Forms.Button2();
            this.btnReceipt = new OpenNETCF.Windows.Forms.Button2();
            this.goToExportButton = new OpenNETCF.Windows.Forms.Button2();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnComposition = new OpenNETCF.Windows.Forms.Button2();
            this.btnDecomposition = new OpenNETCF.Windows.Forms.Button2();
            this.buttonTransfer = new OpenNETCF.Windows.Forms.Button2();
            this.buttonInvoiceSales = new OpenNETCF.Windows.Forms.Button2();
            this.buttonExportDocuments = new OpenNETCF.Windows.Forms.Button2();
            this.buttonPreviousDocuments = new OpenNETCF.Windows.Forms.Button2();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panelExportButtons = new System.Windows.Forms.Panel();
            this.buttonExportTransfer = new OpenNETCF.Windows.Forms.Button2();
            this.buttonExportInvoiceSales = new OpenNETCF.Windows.Forms.Button2();
            this.exportInvoiceButton = new OpenNETCF.Windows.Forms.Button2();
            this.btnExportReceipts = new OpenNETCF.Windows.Forms.Button2();
            this.exportInventoryButton = new OpenNETCF.Windows.Forms.Button2();
            this.exportTagsButton = new OpenNETCF.Windows.Forms.Button2();
            this.exportCompetitionButton = new OpenNETCF.Windows.Forms.Button2();
            this.exportOrdersButton = new OpenNETCF.Windows.Forms.Button2();
            this.btnFromExportToSettings = new OpenNETCF.Windows.Forms.Button2();
            this.backToMainButton = new OpenNETCF.Windows.Forms.Button2();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnQueueBustingQR = new OpenNETCF.Windows.Forms.Button2();
            this.btnOfflineInvoice = new OpenNETCF.Windows.Forms.Button2();
            this.btnOfflineOrder = new OpenNETCF.Windows.Forms.Button2();
            this.btnFromSettingsToMain = new OpenNETCF.Windows.Forms.Button2();
            this.btnFromSettingsToExport = new OpenNETCF.Windows.Forms.Button2();
            this.btnHelp = new OpenNETCF.Windows.Forms.Button2();
            this.btnSettings = new OpenNETCF.Windows.Forms.Button2();
            this.timerRefresh = new System.Windows.Forms.Timer();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelExportButtons.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnExit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnExit.ForeColor = System.Drawing.Color.Navy;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnExit.ImageIndex = -1;
            this.btnExit.ImageList = null;
            this.btnExit.Location = new System.Drawing.Point(0, 181);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(115, 63);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "(0) Έξοδος";
            this.btnExit.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dlgExportFile
            // 
            this.dlgExportFile.FileName = "export.csv";
            this.dlgExportFile.Filter = "Με διαχωριστικό tab (*.csv)|*.csv|Απλό κείμενο (*.txt)|*.txt|Όλα τα αρχεία (*.*)|" +
                "*.*";
            this.dlgExportFile.InitialDirectory = "\\Temp";
            // 
            // btnLabel
            // 
            this.btnLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnLabel.ForeColor = System.Drawing.Color.Navy;
            this.btnLabel.Image = ((System.Drawing.Image)(resources.GetObject("btnLabel.Image")));
            this.btnLabel.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnLabel.ImageIndex = -1;
            this.btnLabel.ImageList = null;
            this.btnLabel.Location = new System.Drawing.Point(0, 61);
            this.btnLabel.Name = "btnLabel";
            this.btnLabel.Size = new System.Drawing.Size(77, 60);
            this.btnLabel.TabIndex = 4;
            this.btnLabel.Text = "(4) Ετικέτα";
            this.btnLabel.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnLabel.Click += new System.EventHandler(this.btnLabel_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnInventory.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnInventory.ForeColor = System.Drawing.Color.Navy;
            this.btnInventory.Image = ((System.Drawing.Image)(resources.GetObject("btnInventory.Image")));
            this.btnInventory.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnInventory.ImageIndex = -1;
            this.btnInventory.ImageList = null;
            this.btnInventory.Location = new System.Drawing.Point(77, 0);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(77, 60);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "(2) Απογρ.";
            this.btnInventory.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnOrder
            // 
            this.btnOrder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnOrder.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnOrder.ForeColor = System.Drawing.Color.Navy;
            this.btnOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnOrder.Image")));
            this.btnOrder.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnOrder.ImageIndex = -1;
            this.btnOrder.ImageList = null;
            this.btnOrder.Location = new System.Drawing.Point(0, 0);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(77, 60);
            this.btnOrder.TabIndex = 0;
            this.btnOrder.Text = "(1) Παραγ.";
            this.btnOrder.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            // 
            // btnCompetition
            // 
            this.btnCompetition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnCompetition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnCompetition.ForeColor = System.Drawing.Color.Navy;
            this.btnCompetition.Image = ((System.Drawing.Image)(resources.GetObject("btnCompetition.Image")));
            this.btnCompetition.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnCompetition.ImageIndex = -1;
            this.btnCompetition.ImageList = null;
            this.btnCompetition.Location = new System.Drawing.Point(77, 61);
            this.btnCompetition.Name = "btnCompetition";
            this.btnCompetition.Size = new System.Drawing.Size(77, 60);
            this.btnCompetition.TabIndex = 5;
            this.btnCompetition.Text = "(5) Ανταγ.";
            this.btnCompetition.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnCompetition.Click += new System.EventHandler(this.btnCompetition_Click);
            // 
            // btnInvoice
            // 
            this.btnInvoice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnInvoice.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnInvoice.ForeColor = System.Drawing.Color.Navy;
            this.btnInvoice.Image = ((System.Drawing.Image)(resources.GetObject("btnInvoice.Image")));
            this.btnInvoice.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnInvoice.ImageIndex = -1;
            this.btnInvoice.ImageList = null;
            this.btnInvoice.Location = new System.Drawing.Point(154, 0);
            this.btnInvoice.Name = "btnInvoice";
            this.btnInvoice.Size = new System.Drawing.Size(76, 60);
            this.btnInvoice.TabIndex = 3;
            this.btnInvoice.Text = "(3) Δελ. Απ";
            this.btnInvoice.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnInvoice.Click += new System.EventHandler(this.btnInvoice_Click);
            // 
            // btnPriceCheck
            // 
            this.btnPriceCheck.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnPriceCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnPriceCheck.ForeColor = System.Drawing.Color.Navy;
            this.btnPriceCheck.Image = ((System.Drawing.Image)(resources.GetObject("btnPriceCheck.Image")));
            this.btnPriceCheck.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnPriceCheck.ImageIndex = -1;
            this.btnPriceCheck.ImageList = null;
            this.btnPriceCheck.Location = new System.Drawing.Point(0, 121);
            this.btnPriceCheck.Name = "btnPriceCheck";
            this.btnPriceCheck.Size = new System.Drawing.Size(77, 60);
            this.btnPriceCheck.TabIndex = 9;
            this.btnPriceCheck.Text = "(7) Έλ. Τιμ.";
            this.btnPriceCheck.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnPriceCheck.Click += new System.EventHandler(this.btnPriceCheck_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(238, 273);
            this.tabControl1.TabIndex = 10;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnPriceAtStore);
            this.tabPage1.Controls.Add(this.btnESLInv);
            this.tabPage1.Controls.Add(this.btnReceipt);
            this.tabPage1.Controls.Add(this.goToExportButton);
            this.tabPage1.Controls.Add(this.btnOrder);
            this.tabPage1.Controls.Add(this.btnExit);
            this.tabPage1.Controls.Add(this.btnCompetition);
            this.tabPage1.Controls.Add(this.btnLabel);
            this.tabPage1.Controls.Add(this.btnPriceCheck);
            this.tabPage1.Controls.Add(this.btnInventory);
            this.tabPage1.Controls.Add(this.btnInvoice);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(230, 244);
            this.tabPage1.Text = "Παραστατικά";
            // 
            // btnPriceAtStore
            // 
            this.btnPriceAtStore.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnPriceAtStore.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnPriceAtStore.ForeColor = System.Drawing.Color.Navy;
            this.btnPriceAtStore.Image = ((System.Drawing.Image)(resources.GetObject("btnPriceAtStore.Image")));
            this.btnPriceAtStore.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnPriceAtStore.ImageIndex = -1;
            this.btnPriceAtStore.ImageList = null;
            this.btnPriceAtStore.Location = new System.Drawing.Point(154, 121);
            this.btnPriceAtStore.Name = "btnPriceAtStore";
            this.btnPriceAtStore.Size = new System.Drawing.Size(77, 60);
            this.btnPriceAtStore.TabIndex = 25;
            this.btnPriceAtStore.Text = "Έλ. προσφ.";
            this.btnPriceAtStore.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnPriceAtStore.Click += new System.EventHandler(this.btnPriceAtStore_Click);
            // 
            // btnESLInv
            // 
            this.btnESLInv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnESLInv.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnESLInv.ForeColor = System.Drawing.Color.Navy;
            this.btnESLInv.Image = ((System.Drawing.Image)(resources.GetObject("btnESLInv.Image")));
            this.btnESLInv.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnESLInv.ImageIndex = -1;
            this.btnESLInv.ImageList = null;
            this.btnESLInv.Location = new System.Drawing.Point(77, 121);
            this.btnESLInv.Name = "btnESLInv";
            this.btnESLInv.Size = new System.Drawing.Size(76, 60);
            this.btnESLInv.TabIndex = 24;
            this.btnESLInv.Text = "(8) Απ.ESL";
            this.btnESLInv.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnESLInv.Click += new System.EventHandler(this.btnESLInv_Click);
            // 
            // btnReceipt
            // 
            this.btnReceipt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReceipt.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnReceipt.ForeColor = System.Drawing.Color.Navy;
            this.btnReceipt.Image = ((System.Drawing.Image)(resources.GetObject("btnReceipt.Image")));
            this.btnReceipt.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnReceipt.ImageIndex = -1;
            this.btnReceipt.ImageList = null;
            this.btnReceipt.Location = new System.Drawing.Point(154, 61);
            this.btnReceipt.Name = "btnReceipt";
            this.btnReceipt.Size = new System.Drawing.Size(76, 60);
            this.btnReceipt.TabIndex = 22;
            this.btnReceipt.Text = "(6) Παραλ.";
            this.btnReceipt.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnReceipt.Click += new System.EventHandler(this.btnReceipt_Click);
            // 
            // goToExportButton
            // 
            this.goToExportButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.goToExportButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.goToExportButton.ForeColor = System.Drawing.Color.Navy;
            this.goToExportButton.Image = ((System.Drawing.Image)(resources.GetObject("goToExportButton.Image")));
            this.goToExportButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.goToExportButton.ImageIndex = -1;
            this.goToExportButton.ImageList = null;
            this.goToExportButton.Location = new System.Drawing.Point(116, 181);
            this.goToExportButton.Name = "goToExportButton";
            this.goToExportButton.Size = new System.Drawing.Size(115, 63);
            this.goToExportButton.TabIndex = 10;
            this.goToExportButton.Text = "(9) Πωλήσεις";
            this.goToExportButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.goToExportButton.Click += new System.EventHandler(this.goToExportButton_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnComposition);
            this.tabPage4.Controls.Add(this.btnDecomposition);
            this.tabPage4.Controls.Add(this.buttonTransfer);
            this.tabPage4.Controls.Add(this.buttonInvoiceSales);
            this.tabPage4.Controls.Add(this.buttonExportDocuments);
            this.tabPage4.Controls.Add(this.buttonPreviousDocuments);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(230, 244);
            this.tabPage4.Text = "Πωλήσεις";
            // 
            // btnComposition
            // 
            this.btnComposition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnComposition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnComposition.ForeColor = System.Drawing.Color.Navy;
            this.btnComposition.Image = ((System.Drawing.Image)(resources.GetObject("btnComposition.Image")));
            this.btnComposition.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnComposition.ImageIndex = -1;
            this.btnComposition.ImageList = null;
            this.btnComposition.Location = new System.Drawing.Point(115, 46);
            this.btnComposition.Name = "btnComposition";
            this.btnComposition.Size = new System.Drawing.Size(115, 54);
            this.btnComposition.TabIndex = 22;
            this.btnComposition.Text = "(4) Σύνθεση";
            this.btnComposition.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnComposition.Click += new System.EventHandler(this.btnComposition_Click);
            // 
            // btnDecomposition
            // 
            this.btnDecomposition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnDecomposition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnDecomposition.ForeColor = System.Drawing.Color.Navy;
            this.btnDecomposition.Image = ((System.Drawing.Image)(resources.GetObject("btnDecomposition.Image")));
            this.btnDecomposition.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnDecomposition.ImageIndex = -1;
            this.btnDecomposition.ImageList = null;
            this.btnDecomposition.Location = new System.Drawing.Point(0, 46);
            this.btnDecomposition.Name = "btnDecomposition";
            this.btnDecomposition.Size = new System.Drawing.Size(115, 54);
            this.btnDecomposition.TabIndex = 22;
            this.btnDecomposition.Text = "(3) Ανάλωση";
            this.btnDecomposition.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnDecomposition.Click += new System.EventHandler(this.btnDecomposition_Click);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonTransfer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonTransfer.ForeColor = System.Drawing.Color.Navy;
            this.buttonTransfer.Image = ((System.Drawing.Image)(resources.GetObject("buttonTransfer.Image")));
            this.buttonTransfer.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.buttonTransfer.ImageIndex = -1;
            this.buttonTransfer.ImageList = null;
            this.buttonTransfer.Location = new System.Drawing.Point(116, 0);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(115, 46);
            this.buttonTransfer.TabIndex = 22;
            this.buttonTransfer.Text = "(2) Ενδοδιακίνηση";
            this.buttonTransfer.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // buttonInvoiceSales
            // 
            this.buttonInvoiceSales.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonInvoiceSales.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonInvoiceSales.ForeColor = System.Drawing.Color.Navy;
            this.buttonInvoiceSales.Image = ((System.Drawing.Image)(resources.GetObject("buttonInvoiceSales.Image")));
            this.buttonInvoiceSales.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.buttonInvoiceSales.ImageIndex = -1;
            this.buttonInvoiceSales.ImageList = null;
            this.buttonInvoiceSales.Location = new System.Drawing.Point(0, 0);
            this.buttonInvoiceSales.Name = "buttonInvoiceSales";
            this.buttonInvoiceSales.Size = new System.Drawing.Size(115, 46);
            this.buttonInvoiceSales.TabIndex = 21;
            this.buttonInvoiceSales.Text = "(1) Δελ. Απ. Πωλήσεων";
            this.buttonInvoiceSales.Click += new System.EventHandler(this.buttonInvoiceSales_Click);
            // 
            // buttonExportDocuments
            // 
            this.buttonExportDocuments.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonExportDocuments.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonExportDocuments.ForeColor = System.Drawing.Color.Navy;
            this.buttonExportDocuments.Image = ((System.Drawing.Image)(resources.GetObject("buttonExportDocuments.Image")));
            this.buttonExportDocuments.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.buttonExportDocuments.ImageIndex = -1;
            this.buttonExportDocuments.ImageList = null;
            this.buttonExportDocuments.Location = new System.Drawing.Point(115, 193);
            this.buttonExportDocuments.Name = "buttonExportDocuments";
            this.buttonExportDocuments.Size = new System.Drawing.Size(115, 52);
            this.buttonExportDocuments.TabIndex = 20;
            this.buttonExportDocuments.Text = "(9) Εξαγωγή";
            this.buttonExportDocuments.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.buttonExportDocuments.Click += new System.EventHandler(this.buttonExportDocuments_Click);
            // 
            // buttonPreviousDocuments
            // 
            this.buttonPreviousDocuments.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonPreviousDocuments.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonPreviousDocuments.ForeColor = System.Drawing.Color.Navy;
            this.buttonPreviousDocuments.Image = ((System.Drawing.Image)(resources.GetObject("buttonPreviousDocuments.Image")));
            this.buttonPreviousDocuments.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.buttonPreviousDocuments.ImageIndex = -1;
            this.buttonPreviousDocuments.ImageList = null;
            this.buttonPreviousDocuments.Location = new System.Drawing.Point(0, 193);
            this.buttonPreviousDocuments.Name = "buttonPreviousDocuments";
            this.buttonPreviousDocuments.Size = new System.Drawing.Size(115, 52);
            this.buttonPreviousDocuments.TabIndex = 19;
            this.buttonPreviousDocuments.Text = "(7) Παραστατικά";
            this.buttonPreviousDocuments.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.buttonPreviousDocuments.Click += new System.EventHandler(this.buttonPreviousDocuments_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelExportButtons);
            this.tabPage2.Controls.Add(this.btnFromExportToSettings);
            this.tabPage2.Controls.Add(this.backToMainButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(230, 244);
            this.tabPage2.Text = "Εξαγωγή";
            // 
            // panelExportButtons
            // 
            this.panelExportButtons.Controls.Add(this.buttonExportTransfer);
            this.panelExportButtons.Controls.Add(this.buttonExportInvoiceSales);
            this.panelExportButtons.Controls.Add(this.exportInvoiceButton);
            this.panelExportButtons.Controls.Add(this.btnExportReceipts);
            this.panelExportButtons.Controls.Add(this.exportInventoryButton);
            this.panelExportButtons.Controls.Add(this.exportTagsButton);
            this.panelExportButtons.Controls.Add(this.exportCompetitionButton);
            this.panelExportButtons.Controls.Add(this.exportOrdersButton);
            this.panelExportButtons.Location = new System.Drawing.Point(1, 15);
            this.panelExportButtons.Name = "panelExportButtons";
            this.panelExportButtons.Size = new System.Drawing.Size(230, 160);
            // 
            // buttonExportTransfer
            // 
            this.buttonExportTransfer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonExportTransfer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonExportTransfer.ForeColor = System.Drawing.Color.Navy;
            this.buttonExportTransfer.Image = ((System.Drawing.Image)(resources.GetObject("buttonExportTransfer.Image")));
            this.buttonExportTransfer.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.buttonExportTransfer.ImageIndex = -1;
            this.buttonExportTransfer.ImageList = null;
            this.buttonExportTransfer.Location = new System.Drawing.Point(114, 120);
            this.buttonExportTransfer.Name = "buttonExportTransfer";
            this.buttonExportTransfer.Size = new System.Drawing.Size(115, 40);
            this.buttonExportTransfer.TabIndex = 21;
            this.buttonExportTransfer.Text = "Ενδοδιακίνηση";
            this.buttonExportTransfer.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.buttonExportTransfer.Click += new System.EventHandler(this.buttonExportTransfer_Click);
            // 
            // buttonExportInvoiceSales
            // 
            this.buttonExportInvoiceSales.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonExportInvoiceSales.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.buttonExportInvoiceSales.ForeColor = System.Drawing.Color.Navy;
            this.buttonExportInvoiceSales.Image = ((System.Drawing.Image)(resources.GetObject("buttonExportInvoiceSales.Image")));
            this.buttonExportInvoiceSales.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.buttonExportInvoiceSales.ImageIndex = -1;
            this.buttonExportInvoiceSales.ImageList = null;
            this.buttonExportInvoiceSales.Location = new System.Drawing.Point(-1, 120);
            this.buttonExportInvoiceSales.Name = "buttonExportInvoiceSales";
            this.buttonExportInvoiceSales.Size = new System.Drawing.Size(115, 40);
            this.buttonExportInvoiceSales.TabIndex = 20;
            this.buttonExportInvoiceSales.Text = "(7) Δ.Απ.Πωλ.";
            this.buttonExportInvoiceSales.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.buttonExportInvoiceSales.Click += new System.EventHandler(this.buttonExportInvoiceSales_Click);
            // 
            // exportInvoiceButton
            // 
            this.exportInvoiceButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exportInvoiceButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.exportInvoiceButton.ForeColor = System.Drawing.Color.Navy;
            this.exportInvoiceButton.Image = ((System.Drawing.Image)(resources.GetObject("exportInvoiceButton.Image")));
            this.exportInvoiceButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.exportInvoiceButton.ImageIndex = -1;
            this.exportInvoiceButton.ImageList = null;
            this.exportInvoiceButton.Location = new System.Drawing.Point(0, 40);
            this.exportInvoiceButton.Name = "exportInvoiceButton";
            this.exportInvoiceButton.Size = new System.Drawing.Size(115, 40);
            this.exportInvoiceButton.TabIndex = 12;
            this.exportInvoiceButton.Text = "(3) Δελ. Αποσ.";
            this.exportInvoiceButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.exportInvoiceButton.Click += new System.EventHandler(this.exportInvoiceButton_Click);
            // 
            // btnExportReceipts
            // 
            this.btnExportReceipts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnExportReceipts.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnExportReceipts.ForeColor = System.Drawing.Color.Navy;
            this.btnExportReceipts.Image = ((System.Drawing.Image)(resources.GetObject("btnExportReceipts.Image")));
            this.btnExportReceipts.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnExportReceipts.ImageIndex = -1;
            this.btnExportReceipts.ImageList = null;
            this.btnExportReceipts.Location = new System.Drawing.Point(115, 80);
            this.btnExportReceipts.Name = "btnExportReceipts";
            this.btnExportReceipts.Size = new System.Drawing.Size(115, 40);
            this.btnExportReceipts.TabIndex = 19;
            this.btnExportReceipts.Text = "(6) Παραλαβ.";
            this.btnExportReceipts.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnExportReceipts.Click += new System.EventHandler(this.btnExportReceipts_Click);
            // 
            // exportInventoryButton
            // 
            this.exportInventoryButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exportInventoryButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.exportInventoryButton.ForeColor = System.Drawing.Color.Navy;
            this.exportInventoryButton.Image = ((System.Drawing.Image)(resources.GetObject("exportInventoryButton.Image")));
            this.exportInventoryButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.exportInventoryButton.ImageIndex = -1;
            this.exportInventoryButton.ImageList = null;
            this.exportInventoryButton.Location = new System.Drawing.Point(115, 0);
            this.exportInventoryButton.Name = "exportInventoryButton";
            this.exportInventoryButton.Size = new System.Drawing.Size(115, 40);
            this.exportInventoryButton.TabIndex = 11;
            this.exportInventoryButton.Text = "(2) Απογραφή";
            this.exportInventoryButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.exportInventoryButton.Click += new System.EventHandler(this.exportInventoryButton_Click);
            // 
            // exportTagsButton
            // 
            this.exportTagsButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exportTagsButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.exportTagsButton.ForeColor = System.Drawing.Color.Navy;
            this.exportTagsButton.Image = ((System.Drawing.Image)(resources.GetObject("exportTagsButton.Image")));
            this.exportTagsButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.exportTagsButton.ImageIndex = -1;
            this.exportTagsButton.ImageList = null;
            this.exportTagsButton.Location = new System.Drawing.Point(115, 40);
            this.exportTagsButton.Name = "exportTagsButton";
            this.exportTagsButton.Size = new System.Drawing.Size(115, 40);
            this.exportTagsButton.TabIndex = 13;
            this.exportTagsButton.Text = "(4) Ετικέτες";
            this.exportTagsButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.exportTagsButton.Click += new System.EventHandler(this.exportTagsButton_Click);
            // 
            // exportCompetitionButton
            // 
            this.exportCompetitionButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exportCompetitionButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.exportCompetitionButton.ForeColor = System.Drawing.Color.Navy;
            this.exportCompetitionButton.Image = ((System.Drawing.Image)(resources.GetObject("exportCompetitionButton.Image")));
            this.exportCompetitionButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.exportCompetitionButton.ImageIndex = -1;
            this.exportCompetitionButton.ImageList = null;
            this.exportCompetitionButton.Location = new System.Drawing.Point(0, 80);
            this.exportCompetitionButton.Name = "exportCompetitionButton";
            this.exportCompetitionButton.Size = new System.Drawing.Size(115, 40);
            this.exportCompetitionButton.TabIndex = 14;
            this.exportCompetitionButton.Text = "(5) Ανταγων.";
            this.exportCompetitionButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.exportCompetitionButton.Click += new System.EventHandler(this.exportCompetitionButton_Click);
            // 
            // exportOrdersButton
            // 
            this.exportOrdersButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exportOrdersButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.exportOrdersButton.ForeColor = System.Drawing.Color.Navy;
            this.exportOrdersButton.Image = ((System.Drawing.Image)(resources.GetObject("exportOrdersButton.Image")));
            this.exportOrdersButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.exportOrdersButton.ImageIndex = -1;
            this.exportOrdersButton.ImageList = null;
            this.exportOrdersButton.Location = new System.Drawing.Point(0, 0);
            this.exportOrdersButton.Name = "exportOrdersButton";
            this.exportOrdersButton.Size = new System.Drawing.Size(115, 40);
            this.exportOrdersButton.TabIndex = 10;
            this.exportOrdersButton.Text = "(1) Παραγγε.";
            this.exportOrdersButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.exportOrdersButton.Click += new System.EventHandler(this.exportOrdersButton_Click);
            // 
            // btnFromExportToSettings
            // 
            this.btnFromExportToSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnFromExportToSettings.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnFromExportToSettings.ForeColor = System.Drawing.Color.Navy;
            this.btnFromExportToSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnFromExportToSettings.Image")));
            this.btnFromExportToSettings.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnFromExportToSettings.ImageIndex = -1;
            this.btnFromExportToSettings.ImageList = null;
            this.btnFromExportToSettings.Location = new System.Drawing.Point(115, 205);
            this.btnFromExportToSettings.Name = "btnFromExportToSettings";
            this.btnFromExportToSettings.Size = new System.Drawing.Size(115, 40);
            this.btnFromExportToSettings.TabIndex = 18;
            this.btnFromExportToSettings.Text = "(9) Ρυθμίσεις";
            this.btnFromExportToSettings.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnFromExportToSettings.Click += new System.EventHandler(this.btnFromExportToSettings_Click);
            // 
            // backToMainButton
            // 
            this.backToMainButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.backToMainButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.backToMainButton.ForeColor = System.Drawing.Color.Navy;
            this.backToMainButton.Image = ((System.Drawing.Image)(resources.GetObject("backToMainButton.Image")));
            this.backToMainButton.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.backToMainButton.ImageIndex = -1;
            this.backToMainButton.ImageList = null;
            this.backToMainButton.Location = new System.Drawing.Point(0, 205);
            this.backToMainButton.Name = "backToMainButton";
            this.backToMainButton.Size = new System.Drawing.Size(115, 40);
            this.backToMainButton.TabIndex = 15;
            this.backToMainButton.Text = "(8) Πωλήσεις";
            this.backToMainButton.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.backToMainButton.Click += new System.EventHandler(this.backToMainButton_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnQueueBustingQR);
            this.tabPage3.Controls.Add(this.btnOfflineInvoice);
            this.tabPage3.Controls.Add(this.btnOfflineOrder);
            this.tabPage3.Controls.Add(this.btnFromSettingsToMain);
            this.tabPage3.Controls.Add(this.btnFromSettingsToExport);
            this.tabPage3.Controls.Add(this.btnHelp);
            this.tabPage3.Controls.Add(this.btnSettings);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(230, 244);
            this.tabPage3.Text = "Βοηθητικά";
            // 
            // btnQueueBustingQR
            // 
            this.btnQueueBustingQR.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnQueueBustingQR.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnQueueBustingQR.ForeColor = System.Drawing.Color.Navy;
            this.btnQueueBustingQR.Image = ((System.Drawing.Image)(resources.GetObject("btnQueueBustingQR.Image")));
            this.btnQueueBustingQR.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnQueueBustingQR.ImageIndex = -1;
            this.btnQueueBustingQR.ImageList = null;
            this.btnQueueBustingQR.Location = new System.Drawing.Point(116, 122);
            this.btnQueueBustingQR.Name = "btnQueueBustingQR";
            this.btnQueueBustingQR.Size = new System.Drawing.Size(114, 61);
            this.btnQueueBustingQR.TabIndex = 23;
            this.btnQueueBustingQR.Text = "Προώθηση Ουράς ";
            this.btnQueueBustingQR.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnQueueBustingQR.Click += new System.EventHandler(this.btnQueueBustingQR_Click_1);
            // 
            // btnOfflineInvoice
            // 
            this.btnOfflineInvoice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnOfflineInvoice.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnOfflineInvoice.ForeColor = System.Drawing.Color.Navy;
            this.btnOfflineInvoice.Image = ((System.Drawing.Image)(resources.GetObject("btnOfflineInvoice.Image")));
            this.btnOfflineInvoice.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnOfflineInvoice.ImageIndex = -1;
            this.btnOfflineInvoice.ImageList = null;
            this.btnOfflineInvoice.Location = new System.Drawing.Point(116, 61);
            this.btnOfflineInvoice.Name = "btnOfflineInvoice";
            this.btnOfflineInvoice.Size = new System.Drawing.Size(114, 61);
            this.btnOfflineInvoice.TabIndex = 22;
            this.btnOfflineInvoice.Text = "Δελ. Απ. Offline";
            this.btnOfflineInvoice.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnOfflineInvoice.Click += new System.EventHandler(this.btnOfflineInvoice_Click);
            // 
            // btnOfflineOrder
            // 
            this.btnOfflineOrder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnOfflineOrder.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnOfflineOrder.ForeColor = System.Drawing.Color.Navy;
            this.btnOfflineOrder.Image = ((System.Drawing.Image)(resources.GetObject("btnOfflineOrder.Image")));
            this.btnOfflineOrder.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnOfflineOrder.ImageIndex = -1;
            this.btnOfflineOrder.ImageList = null;
            this.btnOfflineOrder.Location = new System.Drawing.Point(0, 61);
            this.btnOfflineOrder.Name = "btnOfflineOrder";
            this.btnOfflineOrder.Size = new System.Drawing.Size(116, 61);
            this.btnOfflineOrder.TabIndex = 21;
            this.btnOfflineOrder.Text = "Παραγ. Offline";
            this.btnOfflineOrder.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnOfflineOrder.Click += new System.EventHandler(this.btnOfflineOrder_Click);
            // 
            // btnFromSettingsToMain
            // 
            this.btnFromSettingsToMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnFromSettingsToMain.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnFromSettingsToMain.ForeColor = System.Drawing.Color.Navy;
            this.btnFromSettingsToMain.Image = ((System.Drawing.Image)(resources.GetObject("btnFromSettingsToMain.Image")));
            this.btnFromSettingsToMain.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnFromSettingsToMain.ImageIndex = -1;
            this.btnFromSettingsToMain.ImageList = null;
            this.btnFromSettingsToMain.Location = new System.Drawing.Point(115, 205);
            this.btnFromSettingsToMain.Name = "btnFromSettingsToMain";
            this.btnFromSettingsToMain.Size = new System.Drawing.Size(115, 40);
            this.btnFromSettingsToMain.TabIndex = 20;
            this.btnFromSettingsToMain.Text = "(9) Παραστατικά";
            this.btnFromSettingsToMain.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnFromSettingsToMain.Click += new System.EventHandler(this.btnFromSettingsToDocuments_Click);
            // 
            // btnFromSettingsToExport
            // 
            this.btnFromSettingsToExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnFromSettingsToExport.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnFromSettingsToExport.ForeColor = System.Drawing.Color.Navy;
            this.btnFromSettingsToExport.Image = ((System.Drawing.Image)(resources.GetObject("btnFromSettingsToExport.Image")));
            this.btnFromSettingsToExport.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnFromSettingsToExport.ImageIndex = -1;
            this.btnFromSettingsToExport.ImageList = null;
            this.btnFromSettingsToExport.Location = new System.Drawing.Point(0, 205);
            this.btnFromSettingsToExport.Name = "btnFromSettingsToExport";
            this.btnFromSettingsToExport.Size = new System.Drawing.Size(115, 40);
            this.btnFromSettingsToExport.TabIndex = 19;
            this.btnFromSettingsToExport.Text = "(7) Εξαγωγή";
            this.btnFromSettingsToExport.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnFromSettingsToExport.Click += new System.EventHandler(this.btnFromSettingsToExport_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnHelp.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnHelp.ForeColor = System.Drawing.Color.Navy;
            this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
            this.btnHelp.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnHelp.ImageIndex = -1;
            this.btnHelp.ImageList = null;
            this.btnHelp.Location = new System.Drawing.Point(116, 0);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(114, 61);
            this.btnHelp.TabIndex = 17;
            this.btnHelp.Text = "Βοήθεια";
            this.btnHelp.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnSettings.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.btnSettings.ForeColor = System.Drawing.Color.Navy;
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnSettings.ImageIndex = -1;
            this.btnSettings.ImageList = null;
            this.btnSettings.Location = new System.Drawing.Point(0, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(116, 61);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Ρυθμίσεις";
            this.btnSettings.TextAlign = OpenNETCF.Drawing.ContentAlignment2.BottomCenter;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 5000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(238, 273);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "WRM Mobile@Store";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Main_Closing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Main_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panelExportButtons.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void btnESLInv_Click(object sender, EventArgs e)
        {
            using (ESLInvMasterForm form = new ESLInvMasterForm())
            {
                timerRefresh.Enabled = false;
                MobileAtStore.ConnectDataLayers();
                form.ShowDialog();
                MobileAtStore.DisconnectDataLayers();
                timerRefresh.Enabled = true;
            }
        }

        private void btnPriceAtStore_Click(object sender, EventArgs e)
        {
            using (PriceAtStoreForm form = new PriceAtStoreForm())
            {
                timerRefresh.Enabled = false;
                form.ShowDialog();
                timerRefresh.Enabled = true;
            }
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                ITS.MobileAtStore.WRMMobileAtStore.WRMMobileAtStore ws = null;
                ws = MobileAtStore.GetWebService(AppSettings.Timeout);
                ws.GetWebServiceVersion(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
            }
        }



        //akm insert: auto update svc


        /// <summary>
        /// Begin update process from other thread
        /// </summary>
        public void BeginUpdateFromOtherThread()
        {
            BeginUpdater update = new BeginUpdater(BeginUpdate);
            this.Invoke(update);
        }

        public delegate void BeginUpdater();
        /// <summary>
        /// Begin Update process
        /// </summary>
        public void BeginUpdate()
        {
            System.Windows.Forms.MessageBox.Show("Υπάρχει νεότερη έκδοση του λογισμικού.", "Mobile@Store");

            //Create the updater directory
            Directory.CreateDirectory("\\Application\\its\\Updater");
            DirectoryInfo di_to = new DirectoryInfo("\\Application\\its\\Updater");
            //Determine the required files to be copied 
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\updater";
            //Get Directory info
            DirectoryInfo di = new DirectoryInfo(path);
            // Create an array representing the files in the current directory.
            FileInfo[] fi = di.GetFiles();
            // Print out the names of the files in the current directory.
            foreach (FileInfo fiTemp in fi)
            {
                if (File.Exists("\\Application\\its\\Updater\\" + fiTemp.Name))
                    File.Delete("\\Application\\its\\Updater\\" + fiTemp.Name);
                File.Copy(fiTemp.FullName, "\\Application\\its\\Updater\\" + fiTemp.Name);
            }

            System.Diagnostics.Process.Start("\\Application\\its\\Updater\\updater.exe", "\"" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\"");

            this.Close();
            Application.Exit();
            //Application2.Exit();



        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnOfflineOrder_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.ORDER, "", true);
        }

        private void btnOfflineInvoice_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.INVOICE, "", true);
        }

        private void ShowDocDataShortForm(DOC_TYPES type, string doc_code, bool forcedOffline)
        {
            timerRefresh.Enabled = false;
            MobileAtStore.ConnectDataLayers();
            using (DocDataShortForm docShortForm = new DocDataShortForm(type, doc_code, forcedOffline))
            {
                docShortForm.ShowDialog();
            }
            RefreshCount(type);
            MobileAtStore.DisconnectDataLayers();
            timerRefresh.Enabled = true;
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F11:
                    App.LockDown.Execute(true);
                    break;
                case Keys.F12:
                    App.LockDown.Execute(false);
                    break;

            }
        }

        private void buttonPreviousDocuments_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex -= 1;
        }

        private void buttonExportDocuments_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex += 1;
        }

        private void buttonInvoiceSales_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.INVOICE_SALES, "", false);
        }

        private void buttonExportInvoiceSales_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.INVOICE_SALES, false))
            {
                buttonExportInvoiceSales.Enabled = false;
            }
        }

        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.TRANSFER, "", false);
        }

        private void buttonExportTransfer_Click(object sender, EventArgs e)
        {
            if (InitiateOutputTypeDocumentOnlineExport(DOC_TYPES.TRANSFER, false))
            {
                buttonTransfer.Enabled = false;
            }
        }

        private void btnDecomposition_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.DECOMPOSITION, "", false);
        }

        private void btnComposition_Click(object sender, EventArgs e)
        {
            ShowDocDataShortForm(DOC_TYPES.COMPOSITION, "", false);
        }

        private void btnQueueBustingQR_Click(object sender, EventArgs e)
        {

        }

        //private void btnQueueBustingQR_Click(object sender, EventArgs e)
        //{
        //    if (!AppSettings.ConnectedToWebService)
        //    {
        //        MessageForm.Execute("Προειδοποίηση", "Δεν είναι εφικτή αυτή η λειτουργία χωρίς σύνδεση.Παρακαλώ συνδεθείτε και ξαναδοκιμάστε", MessageForm.DialogTypes.MESSAGE);
        //        return;
        //    }
        //    ShowDocDataShortForm(DOC_TYPES.QUEUE_QR, string.Empty, false);
        //}

        private void btnQueueBustingQR_Click_1(object sender, EventArgs e)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                MessageForm.Execute("Προειδοποίηση", "Δεν είναι εφικτή αυτή η λειτουργία χωρίς σύνδεση.Παρακαλώ συνδεθείτε και ξαναδοκιμάστε", MessageForm.DialogTypes.MESSAGE);
                return;
            }
            ShowDocDataShortForm(DOC_TYPES.QUEUE_QR, string.Empty, false);
        }
    }
}
