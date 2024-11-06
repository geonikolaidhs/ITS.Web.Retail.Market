using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenNETCF;
using OpenNETCF.Windows.Forms;
using ITS.Common.Keyboards.Compact;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Utilities.Compact;
using ITS.MobileAtStore.Properties;
using ITS.Common.Utilities.EAN128BarcodeNS;
using ITS.MobileAtStore.Helpers;
using ITS.MobileAtStore.AuxilliaryClasses;
using System.Net;
using DevExpress.Xpo;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// This is the common document form that is used for all documents and modifies its behavior according to doc type
    /// </summary>
    public class DocDataShortForm : Form
    {
        /// <summary>
        /// The greek Culture Info
        /// </summary>
        private System.Globalization.CultureInfo cultureInfoEl;

        /// <summary>
        /// The document type of the currently editing document
        /// </summary>
        private DOC_TYPES docType;

        /// <summary>
        /// The document code
        /// </summary>
        private string docCode;

        /// <summary>
        /// The currently editing document
        /// </summary>
        private Document document;

        /// <summary>
        /// Gets or sets the mode of the document
        /// </summary>
        public bool ForcedOfflineMode { get; set; }

        #region Windows Form Designer generated code
        private Panel panForm;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private System.Windows.Forms.Label lblProduct;
        private TextBox txtProduct;
        private TextBox txtQty;
        private CheckBox chkDefaultQty;
        private Label lblRecords;
        private TextBox txtRecords;
        private Button2 btnExport;
        private TextBox2 txtDescr;
        private Label lblDescr;
        private Label lblStockValue;
        private Label lblStockName;
        private Button2 btnFlyer;
        private ImageList ilstFilladio;
        private Label lblSupplier;
        private Label lblExtraInfo;
        private Label label2;
        private Label label1;
        private Label lblQty;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DocDataShortForm" /> class.
        /// By default, an online Inventory document type with empty code
        /// </summary>
        public DocDataShortForm()
            : this(DOC_TYPES.INVENTORY, string.Empty, false)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocDataShortForm" /> class.
        /// Using an online document with type <paramref name="docType"/> and code <paramref name="docCode"/>
        /// </summary>
        /// <param name="docType">The document type</param>
        /// <param name="docCode">The document code</param>
        public DocDataShortForm(DOC_TYPES docType, string docCode)
            : this(docType, docCode, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocDataShortForm" /> class.
        /// Using a document with type <paramref name="docType"/> and code <paramref name="docCode"/>. 
        /// The mode is defined by <paramref name="forcedOfflineMode"/> parameter.
        /// </summary>
        /// <param name="docType">The document type</param>
        /// <param name="docCode">The document code</param>
        /// <param name="forcedOfflineMode">The mode of the document (online or offline)</param>
        public DocDataShortForm(DOC_TYPES docType, string docCode, bool forcedOfflineMode)
        {            
            //Initializing the form
            this.InitializeComponent();

            //Some initializations for WinCE and Pocket PC
            if ((Environment.OSVersion.Platform.ToString() == "WinCE") && (Environment2.OSVersion.Platform.ToString() != "PocketPC"))
            {
                this.Menu = null;
            }

            //Setting the variables
            this.cultureInfoEl = ObjectModel.Common.CultureInfo;
            this.docCode = (docCode == null) ? string.Empty : docCode;
            this.docType = docType == DOC_TYPES.ALL_TYPES ? DOC_TYPES.INVENTORY : docType;
            this.btnFlyer.ImageIndex = 0;
            this.btnFlyer.Visible = false;
            this.ForcedOfflineMode = forcedOfflineMode;

            //Changing titles according to document type
            switch (this.docType)
            {
                case DOC_TYPES.ALL_TYPES:
                    this.Text = string.Empty;
                    break;

                case DOC_TYPES.ORDER:
                    this.Text = "ΠΑΡΑΓΓΕΛΙΑ" + (forcedOfflineMode ? " OFFLINE" : string.Empty);
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.INVOICE:
                    this.Text = "ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ" + (forcedOfflineMode ? " OFFLINE" : string.Empty);
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.INVOICE_SALES:
                    this.Text = "ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ ΠΩΛΗΣΕΩΝ" + (forcedOfflineMode ? " OFFLINE" : string.Empty);
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.RECEPTION:
                    this.Text = "ΑΠΟΓΡΑΦΗ";
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;
                case DOC_TYPES.MATCHING:
                    this.Text = "ΑΝΤΙΣΤΟΙΧΙΑ";
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.PICKING:
                    this.Text = string.Empty;
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.TRANSFER:
                    //this.Text = string.Empty;
                    this.Text = "Ενδοδιακίνηση" + (forcedOfflineMode ? " OFFLINE" : string.Empty);
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.TAG:
                    this.Text = "ΕΤΙΚΕΤΑ";
                    this.chkDefaultQty.Enabled = false;
                    this.txtQty.Enabled = false;
                    this.lblQty.Visible = false;
                    this.txtQty.Visible = false;
                    this.chkDefaultQty.Visible = false;
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.PRICE_CHECK:
                    this.Text = string.Empty;
                    NumKeypad.LastValueText = Resources.LastQtyText;
                    break;

                case DOC_TYPES.COMPETITION:
                    this.Text = "ΑΝΤΑΓΩΝΙΣΜΟΣ";
                    this.lblQty.Text = "Τιμή";
                    this.lblQty.Visible = false;
                    this.txtQty.Visible = true;
                    this.chkDefaultQty.Visible = false;
                    this.btnFlyer.Visible = true;
                    NumKeypad.LastValueText = Resources.LastPriceText;
                    break;
                case DOC_TYPES.QUEUE_QR:
                    this.txtQty.Enabled = true;
                    this.lblQty.Visible = true;
                    this.txtQty.Visible = true;
                    this.txtQty.Text = "1";
                    this.chkDefaultQty.Visible = true;
                    this.chkDefaultQty.Checked = true;
                    this.Text = "ΠΡΟΩΘΗΣΗ ΟΥΡΑΣ";
                    break;
                default:
                    this.Text = string.Empty;
                    break;
            }

            //Creating or fetching the document from the database
            this.document = this.CreateDocument();            
            this.Paint += new PaintEventHandler(Main.Form_Paint);
            this.txtProduct.Focus();
        }        

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates or fetches a document based on this DocDataShortForm docType and initializes the forms data
        /// </summary>
        /// <returns>The document</returns>
        private Document CreateDocument()
        {
            Document doc = new Document(MobileAtStore.TransactionsDL).Load(this.docType, this.ForcedOfflineMode);
            doc.Header.TerminalID = AppSettings.Terminal.ID;
            doc.Header.Code = this.docCode;
            doc.Header.DocType = this.docType;
            if (string.IsNullOrEmpty(doc.Header.CustomerCode))
            {
                doc.Header.CustomerCode = "00000";
            }
            this.txtRecords.Text = doc.Header.CountLines().ToString();
            return doc;
        }

        /// <summary>
        /// Adds a line with a given code and quantity
        /// </summary>
        /// <param name="code">the code of the product</param>
        /// <param name="qty">the quantity of the product</param>
        /// <param name="flyer">an integer that represents if the product is on flyer or not</param>
        private void AddLine(string code, decimal qty, int flyer)
        {
            this.document.Add(code, qty, flyer);
        }   

 

        /// <summary>
        /// Adds an order line for the provided code
        /// </summary>
        /// <param name="code">the code of the product</param>
        /// <param name="barcode">the barcode of the product</param>
        /// <param name="forcedOffline">if the document is on offline mode</param>
        private void AddOrderLine(string code, EAN128Barcode barcode, bool forcedOffline)
        {
            string ktitle;
            decimal qty;
            decimal lastQty;
            
            this.panForm.Enabled = false;
            string prodid = string.Empty;
            Line line = null;

            //These variables already exist in product
            string productDescription = string.Empty;
            decimal avgqty = 0;
            string prodcode = string.Empty;
            string prodbarcode = string.Empty;
            string stock = string.Empty;
            decimal orderMeasureMent = 0;
            string extrainfo = string.Empty;
            string supplier = string.Empty, basicsupplier = string.Empty;
            int bscomment = 0;
            bool shouldSearchOnline = true;
            Product product = null;
            bool searchedOnline = false;

            try
            {
                this.lblStockValue.Text = "-";
                this.txtDescr.Text = string.Empty;
                this.lblExtraInfo.Text = string.Empty;
                this.lblSupplier.Text = string.Empty;
                if (forcedOffline ||
                    (AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE &&
                    ((this.docType == DOC_TYPES.ORDER 
                    || this.docType == DOC_TYPES.INVOICE
                    || this.docType == DOC_TYPES.INVOICE_SALES 
                    || this.docType == DOC_TYPES.TRANSFER
                    || this.docType == DOC_TYPES.QUEUE_QR
                    || this.docType == DOC_TYPES.COMPETITION) && AppSettings.ConnectedToWebService)))
                {
                    try
                    {
                        if (forcedOffline)
                        {
                            product = Product.Search(code, MobileAtStore.ItemsDL);
                        }
                        else
                        {
                            product = CommonUtilities.GetProductFromWebService(code, null, EnumerationMapping.GetWebeDocumentType(this.docType));
                        }
                        if (product == null)
                        {
                            MessageForm.Execute("Ενημέρωση", "Το προϊόν δεν βρέθηκε", MessageForm.DialogTypes.MESSAGE);
                            return;
                        }                  
                        else if (AppSettings.ReplaceInActiveItemWithActiveMaternal[this.docType] == true && product.IsActive == false && product.ExtraCodeIsActive == true)
                        {
                            MessageForm.Execute("Ενημέρωση", "Μη ενεργό είδος –  Θα αντικατασταθεί  με Μητρικό", MessageForm.DialogTypes.MESSAGE);
                        }
                        else if (AppSettings.PerformActiveItemCheck[this.docType] == true && product.IsActive == false && product.ExtraCodeIsActive == false )
                        {
                            MessageForm.Execute("Ενημέρωση", "Μη ενεργό είδος", MessageForm.DialogTypes.MESSAGE);
                            return;
                        }
                        else if (AppSettings.PerformActiveItemCheck[this.docType] == true && product.IsActiveOnSupplier == false)
                        {
                            MessageForm.Execute("Ενημέρωση", "Μη ενεργό είδος στον προμηθευτή", MessageForm.DialogTypes.MESSAGE);
                            return;
                        }

                        if (document.Header.DocType == DOC_TYPES.QUEUE_QR)
                            prodcode = prodbarcode = code;
                        else
                            prodcode = product.Code;
                        avgqty = product.AverageMonthSales;
                        prodbarcode = product.Barcode;
                        productDescription = product.Description;
                        stock = product.Stock.ToString();
                        orderMeasureMent = product.OrderMM;
                        extrainfo = product.extrainfo;
                        supplier = product.Supplier;
                        basicsupplier = product.BasicSupplier;
                        bscomment = product.BasicSupplierColor;
                        this.txtDescr.Text = product.ToString();
                        this.lblExtraInfo.Text = product.Supplier;
                        this.lblSupplier.Text = product.BasicSupplier;
                        prodid = product.Code;
                        this.lblStockValue.Text = product.Stock.ToString();
                        this.lblSupplier.ForeColor = (product.BasicSupplierColor == 1) ? Color.Red : Color.DarkBlue;

                

                        switch (product.BarcodeParsingResult)
                        {
                            case ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.ITEM_CODE_QUANTITY:
                                this.document.Add(product, this.btnFlyer.ImageIndex);
                                return;
                            case ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.ITEM_CODE_VALUE:
                                this.document.Add(product, this.btnFlyer.ImageIndex);
                                //TODO what about the enclosed value got from the webservice???
                                return;
                            case ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.CUSTOMER:
                                document.SetHeaderCustomer(product.Code);
                                return;
                            default:
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        //do nothing if it failed during online..
                        MessageBox.Show("Πρόβλημα στην επικοινωνία:\n" +  ex.Message + "Παρακαλώ ξαναδοκιμάστε.", "Πρόβλημα στην επικοινωνία",MessageBoxButtons.OK,MessageBoxIcon.Asterisk,MessageBoxDefaultButton.Button1);
                        return;
                    }
                }

                line = this.document.Exists(code);

                //kane deytero search me to prodid
                if (line == null && String.IsNullOrEmpty(prodid)==false)
                {
                    line = this.document.Exists(prodid);
                }     


                if (line != null)
                {
                    ktitle = "* " + code;
                    lastQty = qty = (decimal)line.Qty1;
                }
                else
                {
                    ktitle = code;
                    lastQty = qty = 0;
                }

                //TODO: There is a logical problem here. To discuss with support
                if (!this.chkDefaultQty.Checked /*|| (barcode.IsEAN128Barcode && barcode.QuantityUnits > 0)*/)
                {
                    if (product==null || ( string.IsNullOrEmpty(product.Description) && AppSettings.DisableOffine[this.docType]))
                    {
                        return;
                    }               

                    if (AppSettings.ShowAvgForm[this.docType] && AppSettings.ConnectedToWebService)
                    {
                        using (AvgForm frm = new AvgForm(product, lastQty, this.docType))
                        {
                            frm.ShowDialog();
                            if (frm._qty >= AppSettings.MaximumAllowedQuantity)
                            {
                                MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                return;
                            }
                            qty = frm._qty;
                        }
                    }
                    else
                    {
                        int finalNumberOfDecimal = product!= null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                        bool result = KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalNumberOfDecimal, NumKeypad.OPERATOR.FORBID_OPERATORS);
                        if (qty >= AppSettings.MaximumAllowedQuantity)
                        {
                            MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                            return;
                        }
                    }

                    if (line != null)
                    {
                        bool checkAvgQty = AppSettings.PerformAverageQuantityCheck[this.document.Header.DocType] && !forcedOffline;
                        this.CheckQty(product, lastQty, !checkAvgQty, ref qty);
                        if (qty == 0)
                        {
                            DeleteDocumentLine(line);
                        }
                    }
                    this.txtQty.Text = qty.ToString();
                }
                else
                {
                    qty = qty + decimal.Parse(this.txtQty.Text);
                }
                if (qty == 0)
                {
                    DeleteDocumentLine(line);
                    return;
                }

              
                if (line != null)
                {
                    line.Flyer = this.btnFlyer.ImageIndex;
                    line.Qty1 = qty;             
                    line.Save();
                }
                else
                {
                    if (
                        (this.docType == DOC_TYPES.COMPETITION || this.docType == DOC_TYPES.ORDER)
                        && AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE && AppSettings.ConnectedToWebService)
                    {
                        if (prodid == code)
                        {
                            this.document.Add(prodid, qty, this.btnFlyer.ImageIndex);
                        }
                        else
                        {
                            this.document.Add(prodid, product.Barcode, qty, this.btnFlyer.ImageIndex);
                        }
                    }
                    else if (this.docType == DOC_TYPES.INVOICE)
                    {
                        this.document.Add(prodid, product.Barcode, qty, this.btnFlyer.ImageIndex);
                    }
                    else
                    {
                      Line l=  this.document.Add(code, product.Barcode, qty, this.btnFlyer.ImageIndex);              
                    }
                }
            }
            finally
            {
                this.panForm.Enabled = true;
            }
        }

        /// <summary>
        /// Recursive function checking the quantity
        /// </summary>        
        /// <param name="lastQty">Previous Quantity</param>
        /// <param name="qty">Currently adding quantity</param>
        /// <param name="avgqty">Average Quantity</param>
        /// <param name="productDescription">Product Description</param>
        /// <param name="prodcode">Product Code</param>
        /// <param name="prodbarcode">Product Barcode</param>
        /// <param name="stock">Product Stock</param>
        /// <param name="ordermm">Product Order Measurement Unit</param>
        /// <param name="extrainfo">Product Extra Information</param>
        /// <param name="skipAvgCheck">If true, average quantity check is performed</param>
        private void CheckQty(Product product, decimal lastQty, bool skipAverageCheck, ref decimal qty)           
        {
            AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();
            this.Show(); //force the re-show of the original form..
            if (addOrReplaceResult == AddOrReplaceResult.ADD)
            {
                if (this.docType == DOC_TYPES.ORDER)
                {
                    if ((lastQty + qty > product.AverageMonthSales * 6) && !skipAverageCheck)
                    {
                        DialogResult r = MessageBox.Show("Η ποσότητα παραγγελίας υπερβαίνει τη μέγιστη επιτρεπτή. Να καταχωρηθεί;", "ΠΡΟΣΟΧΗ", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (r == DialogResult.Yes)
                        {
                            qty = lastQty + qty;
                        }
                        else if (r == DialogResult.No)
                        {
                            if (AppSettings.ShowAvgForm[this.docType])
                            {
                                using (AvgForm frm = new AvgForm(product, lastQty, this.docType))
                                    //(productDescription, prodcode, prodbarcode, avgqty, stock, ordermm, this.docType, extrainfo, lastQty.ToString(), string.Empty, string.Empty, 0))
                                {
                                    frm.ShowDialog();
                                    qty = frm._qty;
                                }
                            }
                            else
                            {
                                int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                                bool result = KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS);
                                if (qty >= AppSettings.MaximumAllowedQuantity)
                                {
                                    MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                    return;
                                }
                            }
                            this.CheckQty(product, lastQty, skipAverageCheck, ref qty);
                        }
                    }
                    else
                    {
                        qty = lastQty + qty;
                    }
                }
                else
                {
                    qty = lastQty + qty;
                }
            }
        }

        /// <summary>
        /// Validates and saves the document
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.document.Header.Save();
            this.Close();
        }

        /// <summary>
        /// Processing of key press inside the product field
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, this.txtProduct))
            {
                return;
            }
            if ((e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab) && String.IsNullOrEmpty(this.txtProduct.Text) == false)
            {
                EAN128Barcode b = new EAN128Barcode(this.txtProduct.Text.Trim(), AppSettings.B128Settings);
                string search = this.txtProduct.Text.Trim();
                search = CommonBusinessLogic.GetEAN128ProductCode(this.document.Header.DocType, b, search, this.txtProduct);
                decimal qty = 0m;
                decimal startupQty = 0m;
                if (this.docType == DOC_TYPES.TAG)
                {
                    qty = 1m;
                    this.txtQty.Text = qty.ToString();
                }
                else
                {
                    if ( (this.docType == DOC_TYPES.ORDER)
                        || (this.docType == DOC_TYPES.INVOICE) 
                        || (this.docType == DOC_TYPES.INVENTORY)
                        || this.docType == DOC_TYPES.INVOICE_SALES 
                        || this.docType == DOC_TYPES.TRANSFER
                        || this.docType == DOC_TYPES.QUEUE_QR                        
                        )
                    {
                        this.AddOrderLine(search, b, this.ForcedOfflineMode);
                    }
                    else if (this.docType == DOC_TYPES.DECOMPOSITION || this.docType == DOC_TYPES.COMPOSITION)
                    {
                        this.AddDecompositionLine(search, b);
                    }
                    else
                    {
                        this.panForm.Enabled = false;
                        if (!this.chkDefaultQty.Checked /* || (b.IsEAN128Barcode && b.QuantityUnits > 0)*/)
                        {
                            qty = decimal.Parse(this.txtQty.Text);
                            KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, AppSettings.QuantityNumberOfDecimalDigits, NumKeypad.OPERATOR.NO_OPERATOR, startupQty);
                            if (qty >= AppSettings.MaximumAllowedQuantity)
                            {
                                MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                return;
                            }
                            this.txtQty.Text = qty.ToString();
                        }
                        else
                        {
                            qty = decimal.Parse(this.txtQty.Text);
                        }
                        this.panForm.Enabled = true;
                    }
                }
                if (this.docType == DOC_TYPES.TAG || this.docType == DOC_TYPES.COMPETITION)
                {
                    this.AddLine(search, qty, this.btnFlyer.ImageIndex);
                }
                this.txtProduct.Text = string.Empty;
                this.txtQty.Text = this.chkDefaultQty.Checked ? this.txtQty.Text : "0";
                this.txtRecords.Text = this.document.Header.CountLines().ToString();
                this.txtProduct.Focus();
                e.Handled = true;
            }
        }

        private void AddDecompositionLine(string code, EAN128Barcode b)
        {
            try
            {
                try
                {
                    Product product = CommonUtilities.GetProductFromWebService(code, null, EnumerationMapping.GetWebeDocumentType(this.docType));
                    if (product == null)
                    {
                        MessageForm.Execute("Σφάλμα", "Το είδος δεν βρέθηκε.");
                        return;
                    }
                    //Always Insert new Line
                    decimal qty=0;
                    if (KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true,
                        AppSettings.QuantityNumberOfTotalDigits, AppSettings.QuantityNumberOfDecimalDigits,
                        NumKeypad.OPERATOR.NO_OPERATOR, 0m, product.Description) && qty > 0)
                    {
                        Line mainLine = this.document.Add(product.Code, product.Barcode, qty, 0);
                        mainLine.Description = product.Description;
                        mainLine.Save();
                        using (DecompositionDetailForm decompForm = new DecompositionDetailForm(this.document, mainLine))
                        {
                            decompForm.ShowDialog();
                        }
                    }
                }
                catch (WebException webException)
                {
                    MessageForm.Execute("Σφάλμα", "Πρέπει να είστε online κατά την δημιουργία ανάλωσης.");
                }
                catch (Exception innerException)
                {
                    MessageForm.Execute("Σφάλμα", "Μη αναμενόμενο Σφάλμα κατά την δημιουργία ανάλωσης." + innerException.Message);
                }

            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Ένα σφαλμα προέκυψε κατά την δημιουργία ανάλωσης." + ex.Message);
            }
        }

        /// <summary>
        /// Making sure that nothing happens when someone presses a key (such as .. enter) in the <see cref="txtQty"/> field,
        /// normally we do this if we want to prevent new lines into making it to the field
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;            
        }

        /// <summary>
        /// If someone checks or unchecks the default quantity checkbox we take the appropriate actions
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void chkDefaultQty_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.chkDefaultQty.Checked)
            {
                this.AppearDefaultQty(true);
                decimal defQuant = decimal.Parse(this.txtQty.Text);
                KeyboardGateway.OpenNumeric(ref defQuant, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, AppSettings.QuantityNumberOfDecimalDigits, NumKeypad.OPERATOR.NO_OPERATOR);
                if (defQuant >= AppSettings.MaximumAllowedQuantity)
                {
                    MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                    return;
                }
                this.txtQty.Text = defQuant.ToString();
            }
            else
            {
                this.AppearDefaultQty(false);
            }
            this.txtProduct.Focus();
        }

        /// <summary>
        /// Making sure that nothing happens when someone presses a key(such as .. enter) in the <see cref="txtQty"/> field,
        /// normally we do this if we want to prevent new lines into making it to the field
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void txtRecords_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// If the user presses escape then we hit the return button
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void DocDataShortForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                this.btnReturn_Click(this, new EventArgs());
            }
        }

        /// <summary>
        /// Setting the Document Status Button to open or closed depending on <paramref name="closed"/>
        /// </summary>
        /// <param name="closed">a boolean that represents if the status will be set to closed or not</param>
        private void SetDocStatusButton(bool closed)
        {
            if (closed)
            {
                this.btnExport.Text = "Κλειστό";
                this.btnExport.ForeColor = Color.Red;
            }
            else
            {
                this.btnExport.Text = "Ανοιχτό";
                this.btnExport.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// On form closing we nullify the form document.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void DocDataShortForm_Closing(object sender, CancelEventArgs e)
        {
            this.document = null;
        }

        /// <summary>
        /// When someone presses enter in the doc number field we give focus to the product code(forces validation of doc number)
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void txtDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.txtProduct.Focus();
                e.Handled = true;
            }
        }       

        /// <summary>
        /// Performing Document Export
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event</param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                MessageForm.Execute("Πληροφορία", "Πρέπει να είστε Online για να πραγματοποιηθεί η εξαγωγή.");
                return;
            }

            if (this.document.Header.CountLines() <= 0)
            {
                MessageForm.Execute("Πληροφορία", "Έισάγετε τουλάχιστον μία εγγραφή και ξαναπροσπαθήστε");
                return;
            }

            try
            {
                this.btnExport.Enabled = false;
                this.document.Header.Save();
                if (ExportHelper.ExportDocuments(this.document.Header.DocType, this.ForcedOfflineMode))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα κατά την Online εξαγωγή\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
            finally
            {
                this.btnExport.Enabled = true;
            }
        }

        /// <summary>
        /// Switching the default quantity to active or inactive according to <paramref name="active"/> parameter
        /// </summary>
        /// <param name="active">If is active or not</param>
        private void AppearDefaultQty(bool active)
        {
            this.txtQty.BackColor = active ? Color.LightBlue : Color.Gainsboro;
            this.txtQty.ForeColor = active ? Color.DarkBlue : Color.Gray;
        }

        /// <summary>
        /// The handler of the <see cref="txtRecords" /> got focus event.
        /// This object normally can not get focus. With this function the case of user click on the 
        /// object is prevented.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void txtRecords_GotFocus(object sender, EventArgs e)
        {
            this.txtProduct.Focus();
        }

        /// <summary>
        /// The handler of the <see cref="txtDescr" /> got focus event.
        /// This object normally can not get focus. With this function the case of user click on the 
        /// object is prevented.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            this.txtProduct.Focus();
        }

        /// <summary>
        /// The handler of the <see cref="DocDataShortForm" /> paint event.
        /// It correctly positions the form in user screen.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void DocDataShortForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }

        /// <summary>
        /// The handler of the <see cref="btnFlyer" /> click event.
        /// The current document line flyer boolean value is switched. 
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void btnFlyer_Click(object sender, EventArgs e)
        {
            if (sender is Button2 && ((Button2)sender).Name == "btnFlyer")
            {
                this.btnFlyer.ImageIndex = this.btnFlyer.ImageIndex ^ 0x1;
                this.btnFlyer.Text = this.btnFlyer.ImageIndex == 0 ? "ΔΕΝ είναι προσφορά" : "ΕΙΝΑΙ προσφορά";
                this.btnFlyer.BackColor = this.btnFlyer.ImageIndex == 0 ? Color.LightGray : Color.LightGreen;
                this.txtProduct.Focus();
            }
        }

        /// <summary>
        /// The On load function of the form. If required the Master form is initiated
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void DocDataShortForm_Load(object sender, EventArgs e)
        {
            if (   this.document.Header.DocType == DOC_TYPES.INVOICE 
                || this.document.Header.DocType == DOC_TYPES.INVOICE_SALES
                || this.document.Header.DocType == DOC_TYPES.TRANSFER
                )
            {
                using (MasterForm frm = new MasterForm(this.document))
                {
                    var result = frm.ShowDialog();
                    if (result != DialogResult.OK)
                    {                        
                        this.DialogResult = result;
                        return;
                    }
                }
            }
        }

        private void DeleteDocumentLine (Line line)
        {
            if (line != null && line.Oid != null && line.Oid != Guid.Empty)
            {
                using (UnitOfWork uow = new UnitOfWork(MobileAtStore.TransactionsDL))
                {                    
                    try
                    {
                        Line docLine = uow.GetObjectByKey<Line>(line.Oid);
                        if (docLine != null)
                        {
                            uow.Delete(docLine);
                            uow.CommitChanges();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageForm.Execute("Σφάλμα", "το παραστατικό παρέμεινε στο φορητό τερματικό\r\n" + exception.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);                        
                    }
                }

            }
        
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocDataShortForm));
            this.panForm = new System.Windows.Forms.Panel();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.lblExtraInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFlyer = new OpenNETCF.Windows.Forms.Button2();
            this.ilstFilladio = new System.Windows.Forms.ImageList();
            this.lblStockValue = new System.Windows.Forms.Label();
            this.lblStockName = new System.Windows.Forms.Label();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.lblDescr = new System.Windows.Forms.Label();
            this.btnExport = new OpenNETCF.Windows.Forms.Button2();
            this.txtRecords = new System.Windows.Forms.TextBox();
            this.lblRecords = new System.Windows.Forms.Label();
            this.chkDefaultQty = new System.Windows.Forms.CheckBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.lblQty = new System.Windows.Forms.Label();
            this.panForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // panForm
            // 
            this.panForm.Controls.Add(this.lblSupplier);
            this.panForm.Controls.Add(this.lblExtraInfo);
            this.panForm.Controls.Add(this.label2);
            this.panForm.Controls.Add(this.label1);
            this.panForm.Controls.Add(this.btnFlyer);
            this.panForm.Controls.Add(this.lblStockValue);
            this.panForm.Controls.Add(this.lblStockName);
            this.panForm.Controls.Add(this.txtDescr);
            this.panForm.Controls.Add(this.lblDescr);
            this.panForm.Controls.Add(this.btnExport);
            this.panForm.Controls.Add(this.txtRecords);
            this.panForm.Controls.Add(this.lblRecords);
            this.panForm.Controls.Add(this.chkDefaultQty);
            this.panForm.Controls.Add(this.lblProduct);
            this.panForm.Controls.Add(this.txtProduct);
            this.panForm.Controls.Add(this.txtQty);
            this.panForm.Controls.Add(this.btnReturn);
            this.panForm.Controls.Add(this.lblQty);
            this.panForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panForm.Location = new System.Drawing.Point(0, 0);
            this.panForm.Name = "panForm";
            this.panForm.Size = new System.Drawing.Size(238, 268);
            // 
            // lblSupplier
            // 
            this.lblSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSupplier.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblSupplier.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblSupplier.Location = new System.Drawing.Point(37, 212);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(197, 20);
            // 
            // lblExtraInfo
            // 
            this.lblExtraInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExtraInfo.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblExtraInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblExtraInfo.Location = new System.Drawing.Point(37, 194);
            this.lblExtraInfo.Name = "lblExtraInfo";
            this.lblExtraInfo.Size = new System.Drawing.Size(197, 20);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label2.Location = new System.Drawing.Point(6, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 20);
            this.label2.Text = "ΒΠ:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(6, 194);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 20);
            this.label1.Text = "ΑΧ:";
            // 
            // btnFlyer
            // 
            this.btnFlyer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFlyer.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.TopCenter;
            this.btnFlyer.ImageIndex = 1;
            this.btnFlyer.ImageList = this.ilstFilladio;
            this.btnFlyer.Location = new System.Drawing.Point(154, 95);
            this.btnFlyer.Name = "btnFlyer";
            this.btnFlyer.Size = new System.Drawing.Size(80, 52);
            this.btnFlyer.TabIndex = 69;
            this.btnFlyer.Text = "ΔΕΝ είναι προσφορά";
            this.btnFlyer.Visible = false;
            this.btnFlyer.Click += new System.EventHandler(this.btnFlyer_Click);
            this.ilstFilladio.Images.Clear();
            this.ilstFilladio.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.ilstFilladio.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            // 
            // lblStockValue
            // 
            this.lblStockValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStockValue.BackColor = System.Drawing.Color.LightBlue;
            this.lblStockValue.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblStockValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblStockValue.Location = new System.Drawing.Point(154, 167);
            this.lblStockValue.Name = "lblStockValue";
            this.lblStockValue.Size = new System.Drawing.Size(80, 24);
            this.lblStockValue.Text = "-";
            this.lblStockValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblStockName
            // 
            this.lblStockName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStockName.BackColor = System.Drawing.Color.Gainsboro;
            this.lblStockName.ForeColor = System.Drawing.Color.Black;
            this.lblStockName.Location = new System.Drawing.Point(154, 147);
            this.lblStockName.Name = "lblStockName";
            this.lblStockName.Size = new System.Drawing.Size(80, 20);
            this.lblStockName.Text = "Απόθεμα";
            this.lblStockName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(4, 95);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(144, 96);
            this.txtDescr.TabIndex = 64;
            this.txtDescr.GotFocus += new System.EventHandler(this.txtDescr_GotFocus);
            // 
            // lblDescr
            // 
            this.lblDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescr.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDescr.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblDescr.ForeColor = System.Drawing.Color.Black;
            this.lblDescr.Location = new System.Drawing.Point(4, 73);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(230, 24);
            this.lblDescr.Text = "Στοιχεία είδους";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnExport.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnExport.ImageIndex = -1;
            this.btnExport.ImageList = null;
            this.btnExport.Location = new System.Drawing.Point(4, 233);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(120, 32);
            this.btnExport.TabIndex = 19;
            this.btnExport.Text = "Εξαγωγή";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // txtRecords
            // 
            this.txtRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecords.BackColor = System.Drawing.Color.LightBlue;
            this.txtRecords.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtRecords.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtRecords.Location = new System.Drawing.Point(107, 1);
            this.txtRecords.Multiline = true;
            this.txtRecords.Name = "txtRecords";
            this.txtRecords.Size = new System.Drawing.Size(128, 24);
            this.txtRecords.TabIndex = 3;
            this.txtRecords.TabStop = false;
            this.txtRecords.Text = "0";
            this.txtRecords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecords.GotFocus += new System.EventHandler(this.txtRecords_GotFocus);
            this.txtRecords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRecords_KeyPress);
            // 
            // lblRecords
            // 
            this.lblRecords.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblRecords.BackColor = System.Drawing.Color.Gainsboro;
            this.lblRecords.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblRecords.ForeColor = System.Drawing.Color.Black;
            this.lblRecords.Location = new System.Drawing.Point(4, 1);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(97, 24);
            this.lblRecords.Text = "Εγγραφές";
            // 
            // chkDefaultQty
            // 
            this.chkDefaultQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDefaultQty.BackColor = System.Drawing.Color.Gainsboro;
            this.chkDefaultQty.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.chkDefaultQty.ForeColor = System.Drawing.Color.Black;
            this.chkDefaultQty.Location = new System.Drawing.Point(107, 49);
            this.chkDefaultQty.Name = "chkDefaultQty";
            this.chkDefaultQty.Size = new System.Drawing.Size(56, 24);
            this.chkDefaultQty.TabIndex = 2;
            this.chkDefaultQty.CheckStateChanged += new System.EventHandler(this.chkDefaultQty_CheckStateChanged);
            // 
            // lblProduct
            // 
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(4, 25);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(97, 24);
            this.lblProduct.Text = "Κωδ. Είδους";
            // 
            // txtProduct
            // 
            this.txtProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProduct.Location = new System.Drawing.Point(107, 25);
            this.txtProduct.MaxLength = 18;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(128, 24);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
            // 
            // txtQty
            // 
            this.txtQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQty.BackColor = System.Drawing.Color.Gainsboro;
            this.txtQty.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtQty.ForeColor = System.Drawing.Color.Gray;
            this.txtQty.Location = new System.Drawing.Point(169, 49);
            this.txtQty.Multiline = true;
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(66, 24);
            this.txtQty.TabIndex = 3;
            this.txtQty.Text = "0";
            this.txtQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQty_KeyPress);
            // 
            // btnReturn
            // 
            this.btnReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReturn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnReturn.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnReturn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReturn.Image = ((System.Drawing.Image)(resources.GetObject("btnReturn.Image")));
            this.btnReturn.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnReturn.ImageIndex = -1;
            this.btnReturn.ImageList = null;
            this.btnReturn.Location = new System.Drawing.Point(127, 233);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(108, 32);
            this.btnReturn.TabIndex = 1;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // lblQty
            // 
            this.lblQty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQty.BackColor = System.Drawing.Color.Gainsboro;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblQty.ForeColor = System.Drawing.Color.Black;
            this.lblQty.Location = new System.Drawing.Point(4, 49);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(97, 24);
            this.lblQty.Text = "Προεπ. Ποσ.";
            // 
            // DocDataShortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 268);
            this.ControlBox = false;
            this.Controls.Add(this.panForm);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocDataShortForm";
            this.Load += new System.EventHandler(this.DocDataShortForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DocDataShortForm_Paint);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DocDataShortForm_Closing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DocDataShortForm_KeyPress);
            this.panForm.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
