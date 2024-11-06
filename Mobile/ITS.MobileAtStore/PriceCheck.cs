using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenNETCF;
using OpenNETCF.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using ITS.Common.Utilities.Compact;
using ITS.Common.Utilities.EAN128BarcodeNS;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public class PriceCheck : Form
    {
        #region Data Members
        private Document document;
        private Product product;
        private System.Globalization.CultureInfo ci_el;
        private Font normalDescrFont;
        private Font boldDescrFont;
        private Label lblInputCode;
        private Label lblDescr;
        private TextBox2 txtInputCode;
        private TextBox2 txtDescr;
        private TextBox2 txtPrice;
        private Button2 btnReturn;
        private Button2 btnAccept;
        private System.Windows.Forms.MainMenu mainMenu1;
        private StatusBar stbMain;
        private OpenNETCF.Media.SoundPlayer sndSpeaker;
        private Button2 downloadNewItemsButton;
        private Label lblExtraInfo;
        private Label lblStockValue;
        private Label lblStockName;
        private Button2 btnAdvancedPC;
        private Label lblSupplier;
        private Label label1;
        private Label label2;
        private System.ComponentModel.IContainer components = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes all the members required by the PriceCheck form
        /// </summary>
        public PriceCheck()
        {
            InitializeComponent();
            //if (!AppSettings.AdvancedPriceCheckingActive)
            btnAdvancedPC.Visible = AppSettings.AdvancedPriceCheckingActive;
            lblExtraInfo.Text = "";
            lblSupplier.Text = "";
            this.WindowState = FormWindowState.Maximized;
            if ((Environment.OSVersion.Platform.ToString() == "WinCE") && (Environment2.OSVersion.Platform.ToString() != "PocketPC"))
            {
                this.Menu = null;
            }
            this.ci_el = ObjectModel.Common.CultureInfo;
            this.document = new Document(MobileAtStore.TransactionsDL).Load(DOC_TYPES.TAG, false);
            this.document.Header.TerminalID = AppSettings.Terminal.ID;
            normalDescrFont = new Font("Tahoma", 13, FontStyle.Bold);
            boldDescrFont = new Font("Tahoma", 13, FontStyle.Bold);

            this.document.Header.DocType = DOC_TYPES.TAG;
            this.document.Header.TerminalID = 0;
            this.Paint += new PaintEventHandler(Main.Form_Paint);
            this.txtInputCode.Focus();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a tag line with the given quantity and code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="qty"></param>
        private void AddLine(string code, decimal qty)
        {
            Line line = this.document.Exists(code);
            if (line != null)
            {
                line.Qty1 += qty;
                line.Save();
            }
            else
            {
                this.document.Add(code, qty,0);
            }
        }

        /// <summary>
        /// Retrieves an online product from the web service
        /// </summary>
        /// <param name="givenCode"></param>
        /// <returns></returns>
        private Product GetOnlineProductInfo(string givenCode, string compCode, string priceCatalogPolicy)
        {
            Product transmutedProduct = null;
            ITS.MobileAtStore.WRMMobileAtStore.Product givenProduct = null;
            try
            {
                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    givenProduct = service.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, givenCode, null, compCode, priceCatalogPolicy,ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.PRICE_CHECK,true);
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την σύνδεση με την υπηρεσία ελέγχου τιμών/r/n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }


            if (givenProduct != null && givenProduct.Code != "")
            {
                transmutedProduct = CommonUtilities.ConvertProduct(givenProduct, MobileAtStore.ItemsDL);
                CommonUtilities.SetExtraInfoText(transmutedProduct, lblExtraInfo);
                lblSupplier.Text = transmutedProduct.BasicSupplier;
                if (transmutedProduct.BasicSupplierColor == 1)
                {
                    lblSupplier.ForeColor = Color.Red;
                }
                else
                {
                    lblSupplier.ForeColor = Color.DarkBlue;
                }
            }
            else
            {
                if (givenProduct != null && givenProduct.ErrorMessage != "")
                {
                    MessageForm.Execute("Σφάλμα", givenProduct.ErrorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }
            }

            return transmutedProduct;
        }

        private decimal GetProductFinalValue(Product product, string compCode)
        {
            try
            {
                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    WRMMobileAtStore.Offer[] offers = service.GetOffers(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, product.Code, compCode);
                    if(offers.Length==0)
                    {
                        return product.CalculatedTotalPrice;
                    }
                    else if(offers.Length==1)
                    {
                        return offers[0].FinalPrice;
                    }
                    return product.Price;//Ποια προσφορά; Οεο;
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την σύνδεση με την υπηρεσία ελέγχου προσφορών/r/n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                return product.Price;
            }
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Processes the keycode input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInputCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtInputCode))
            {
                return;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!string.IsNullOrEmpty(this.txtInputCode.Text))
                {
                    string code = this.txtInputCode.Text.Trim(new char[] { ' ', '\t', '\r', '\n' });
                    EAN128Barcode b128 = new EAN128Barcode(code, AppSettings.B128Settings);
                    code = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.PRICE_CHECK, b128, code, this.txtInputCode);
                    if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE)
                    {
                        this.product = GetOnlineProductInfo(code, AppSettings.CompCode, AppSettings.PriceList);
                    }
                    else
                    {
                        if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
                        {
                            this.product = Product.Search(code, MobileAtStore.ItemsDL);
                        }
                    }

                    if (this.product != null)
                    {


                        this.txtDescr.Font = normalDescrFont;
                        this.txtPrice.Text = this.product.Price.ToString("#0.00", ObjectModel.Common.CultureInfo) + "€";//GetProductFinalValue(this.product, AppSettings.CompCode).ToString("#0.00", ObjectModel.Common.CultureInfo) + "€";//this.product.Price.ToString("#0.00", ObjectModel.Common.CultureInfo) + "€";
                        this.txtDescr.Text = this.product.ToString();
                        lblStockValue.Text = product.Stock.ToString();
                        /*if (!string.IsNullOrEmpty(product.BasicSupplier))
                        {
                            lblSupplier.Text = product.BasicSupplier;
                        }*/
                        if ( product.IsActive && product.IsActiveOnSupplier && !string.IsNullOrEmpty(product.Supplier))
                        {
                            lblExtraInfo.ForeColor = System.Drawing.Color.DarkBlue;
                            lblExtraInfo.Text = product.Supplier;
                        }
                        this.stbMain.Text = "";
                    }
                    else
                    {
                        this.txtDescr.Font = boldDescrFont;
                        this.txtPrice.Text = "XXXXXXXXXXX";
                        this.lblExtraInfo.Text = "";
                        this.lblSupplier.Text = "";
                        lblStockValue.Text = "-";
                        this.txtDescr.Text = "Ανύπαρκτος κωδικός";
                        this.stbMain.Text = "Ανύπαρκτος κωδικός";
                        this.sndSpeaker.SoundLocation = @"\Windows\exclam.wav";
                        this.sndSpeaker.PlaySync();
                    }
                    this.txtInputCode.Text = "";
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        /// <summary>
        /// Prevents keypress in the description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDescr_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Prevents keypress in the description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Adds a line in the tag document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.product != null)
            {
                this.AddLine(this.product.Code, 1m);
                this.stbMain.Text = "Καταχωρήθηκε";
            }
            this.txtInputCode.Focus();
        }

        /// <summary>
        /// Prevents the description textbox from getting focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            this.txtInputCode.Focus();
        }

        /// <summary>
        /// Prevents the price textbox from getting focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPrice_GotFocus(object sender, EventArgs e)
        {
            this.txtInputCode.Focus();
        }

        /// <summary>
        /// Saves the tag document and closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.document.Header.Save();
            this.Close();
        }
        #endregion

        #region Properties
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PriceCheck));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.lblInputCode = new System.Windows.Forms.Label();
            this.lblDescr = new System.Windows.Forms.Label();
            this.txtInputCode = new OpenNETCF.Windows.Forms.TextBox2();
            this.txtDescr = new OpenNETCF.Windows.Forms.TextBox2();
            this.txtPrice = new OpenNETCF.Windows.Forms.TextBox2();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.btnAccept = new OpenNETCF.Windows.Forms.Button2();
            this.stbMain = new System.Windows.Forms.StatusBar();
            this.sndSpeaker = new OpenNETCF.Media.SoundPlayer();
            this.downloadNewItemsButton = new OpenNETCF.Windows.Forms.Button2();
            this.lblExtraInfo = new System.Windows.Forms.Label();
            this.lblStockValue = new System.Windows.Forms.Label();
            this.lblStockName = new System.Windows.Forms.Label();
            this.btnAdvancedPC = new OpenNETCF.Windows.Forms.Button2();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInputCode
            // 
            this.lblInputCode.BackColor = System.Drawing.Color.Gainsboro;
            this.lblInputCode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblInputCode.ForeColor = System.Drawing.Color.Black;
            this.lblInputCode.Location = new System.Drawing.Point(5, 4);
            this.lblInputCode.Name = "lblInputCode";
            this.lblInputCode.Size = new System.Drawing.Size(113, 20);
            this.lblInputCode.Text = "Κωδικός";
            // 
            // lblDescr
            // 
            this.lblDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescr.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDescr.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblDescr.ForeColor = System.Drawing.Color.Black;
            this.lblDescr.Location = new System.Drawing.Point(7, 54);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(226, 18);
            this.lblDescr.Text = "Στοιχεία είδους";
            // 
            // txtInputCode
            // 
            this.txtInputCode.BackColor = System.Drawing.Color.White;
            this.txtInputCode.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtInputCode.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtInputCode.ForeColor = System.Drawing.Color.Navy;
            this.txtInputCode.Location = new System.Drawing.Point(5, 27);
            this.txtInputCode.Name = "txtInputCode";
            this.txtInputCode.Size = new System.Drawing.Size(113, 24);
            this.txtInputCode.TabIndex = 3;
            this.txtInputCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInputCode_KeyPress);
            // 
            // txtDescr
            // 
            this.txtDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescr.BackColor = System.Drawing.Color.LightBlue;
            this.txtDescr.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtDescr.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular);
            this.txtDescr.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtDescr.Location = new System.Drawing.Point(5, 75);
            this.txtDescr.Multiline = true;
            this.txtDescr.Name = "txtDescr";
            this.txtDescr.Size = new System.Drawing.Size(140, 92);
            this.txtDescr.TabIndex = 4;
            this.txtDescr.GotFocus += new System.EventHandler(this.txtDescr_GotFocus);
            this.txtDescr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDescr_KeyPress);
            // 
            // txtPrice
            // 
            this.txtPrice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrice.BackColor = System.Drawing.Color.LightBlue;
            this.txtPrice.CharacterCasing = OpenNETCF.Windows.Forms.CharacterCasing.Normal;
            this.txtPrice.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Bold);
            this.txtPrice.ForeColor = System.Drawing.Color.DarkBlue;
            this.txtPrice.Location = new System.Drawing.Point(124, 4);
            this.txtPrice.Multiline = true;
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(107, 47);
            this.txtPrice.TabIndex = 5;
            this.txtPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPrice.GotFocus += new System.EventHandler(this.txtPrice_GotFocus);
            this.txtPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPrice_KeyPress);
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
            this.btnReturn.Location = new System.Drawing.Point(144, 211);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(89, 32);
            this.btnReturn.TabIndex = 8;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnAccept.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnAccept.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAccept.Image = ((System.Drawing.Image)(resources.GetObject("btnAccept.Image")));
            this.btnAccept.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnAccept.ImageIndex = -1;
            this.btnAccept.ImageList = null;
            this.btnAccept.Location = new System.Drawing.Point(5, 211);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(133, 32);
            this.btnAccept.TabIndex = 9;
            this.btnAccept.Text = "Καταχώρηση ετικ.";
            this.btnAccept.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // stbMain
            // 
            this.stbMain.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.stbMain.Location = new System.Drawing.Point(0, 243);
            this.stbMain.Name = "stbMain";
            this.stbMain.Size = new System.Drawing.Size(238, 26);
            // 
            // downloadNewItemsButton
            // 
            this.downloadNewItemsButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.downloadNewItemsButton.ImageIndex = -1;
            this.downloadNewItemsButton.ImageList = null;
            this.downloadNewItemsButton.Location = new System.Drawing.Point(0, 0);
            this.downloadNewItemsButton.Name = "downloadNewItemsButton";
            this.downloadNewItemsButton.Size = new System.Drawing.Size(72, 20);
            this.downloadNewItemsButton.TabIndex = 0;
            // 
            // lblExtraInfo
            // 
            this.lblExtraInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExtraInfo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblExtraInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblExtraInfo.Location = new System.Drawing.Point(36, 170);
            this.lblExtraInfo.Name = "lblExtraInfo";
            this.lblExtraInfo.Size = new System.Drawing.Size(197, 20);
            this.lblExtraInfo.Text = "ΤΕΣΤ ΚΕΙΜΕΝΟ";
            // 
            // lblStockValue
            // 
            this.lblStockValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStockValue.BackColor = System.Drawing.Color.LightBlue;
            this.lblStockValue.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblStockValue.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblStockValue.Location = new System.Drawing.Point(151, 95);
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
            this.lblStockName.Location = new System.Drawing.Point(151, 75);
            this.lblStockName.Name = "lblStockName";
            this.lblStockName.Size = new System.Drawing.Size(80, 20);
            this.lblStockName.Text = "Απόθεμα";
            this.lblStockName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnAdvancedPC
            // 
            this.btnAdvancedPC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvancedPC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.btnAdvancedPC.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnAdvancedPC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdvancedPC.Image = ((System.Drawing.Image)(resources.GetObject("btnAdvancedPC.Image")));
            this.btnAdvancedPC.ImageAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleLeft;
            this.btnAdvancedPC.ImageIndex = -1;
            this.btnAdvancedPC.ImageList = null;
            this.btnAdvancedPC.Location = new System.Drawing.Point(151, 135);
            this.btnAdvancedPC.Name = "btnAdvancedPC";
            this.btnAdvancedPC.Size = new System.Drawing.Size(80, 32);
            this.btnAdvancedPC.TabIndex = 12;
            this.btnAdvancedPC.Text = "Ρυθμ.";
            this.btnAdvancedPC.TextAlign = OpenNETCF.Drawing.ContentAlignment2.MiddleRight;
            this.btnAdvancedPC.Click += new System.EventHandler(this.button21_Click);
            // 
            // lblSupplier
            // 
            this.lblSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSupplier.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblSupplier.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblSupplier.Location = new System.Drawing.Point(38, 190);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(197, 20);
            this.lblSupplier.Text = "ΤΕΣΤ ΚΕΙΜΕΝΟ";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(7, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 20);
            this.label1.Text = "ΑΧ:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label2.Location = new System.Drawing.Point(7, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 20);
            this.label2.Text = "ΒΠ:";
            // 
            // PriceCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 269);
            this.ControlBox = false;
            this.Controls.Add(this.lblSupplier);
            this.Controls.Add(this.lblExtraInfo);
            this.Controls.Add(this.btnAdvancedPC);
            this.Controls.Add(this.lblStockValue);
            this.Controls.Add(this.lblStockName);
            this.Controls.Add(this.stbMain);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.txtDescr);
            this.Controls.Add(this.txtInputCode);
            this.Controls.Add(this.lblDescr);
            this.Controls.Add(this.lblInputCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "PriceCheck";
            this.Text = "Έλεγχος Τιμής";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PriceCheck_Paint);
            this.ResumeLayout(false);

        }
        #endregion

        #region Disposer
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        private void button21_Click(object sender, EventArgs e)
        {
            using (AdvancedPriceCheckingSettingsForm form = new AdvancedPriceCheckingSettingsForm())
            {
                if (!(form.DialogResult == DialogResult.OK))
                {
                    form.ShowDialog();
                }
                txtInputCode.Focus();
            }
        }

        private void PriceCheck_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
