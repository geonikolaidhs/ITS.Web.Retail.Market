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

//using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// This is the common document form that is used for all documents and modifies its behavior according to doc type
    /// </summary>
    public class MatchingForm : Form
    {
        #region Data Members
        private System.Globalization.CultureInfo ci_el;
        private Document document;

        private System.Windows.Forms.MainMenu mainMenu1;
        private Panel panForm;
        private OpenNETCF.Windows.Forms.Button2 btnReturn;
        private System.Windows.Forms.Label lblProduct;
        private TextBox txtProduct;
        private Label lblRecords;
        private TextBox txtRecords;
        private Button2 btnExport;
        private Label lblProductMatching;
        private TextBox txtProductMatching;
        #endregion

        #region Constructors
        public MatchingForm()
        {
            InitializeComponent();
            if ((Environment.OSVersion.Platform.ToString() == "WinCE") && (Environment2.OSVersion.Platform.ToString() != "PocketPC"))
            {
                this.Menu = null;
            }
            this.ci_el = ObjectModel.Common.CultureInfo;

            this.document = new Document(MobileAtStore.TransactionsDL).Load(DOC_TYPES.MATCHING, false);
            this.document.Header.TerminalID = AppSettings.Terminal.ID;
            this.txtRecords.Text = document.Header.CountLines().ToString();
            this.Paint += new PaintEventHandler(Main.Form_Paint);
            this.txtProduct.Focus();
        }
        #endregion

        #region Methods

        private void AddMatching()
        {
            if (Validate())
            {
                decimal qty = 0;
                if (KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, AppSettings.QuantityNumberOfDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, 0m, ""))
                {
                    if (qty >= AppSettings.MaximumAllowedQuantity)
                    {
                        MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                        return;
                    }
                    document.AddMatching(txtProduct.Text.Trim(), txtProductMatching.Text.Trim(), qty, 0);
                }
                txtRecords.Text = document.Header.CountLines().ToString();
                txtProduct.Text = "";
                txtProductMatching.Text = "";
                txtProduct.Focus();
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(txtProduct.Text.Trim()))
            {
                MessageForm.Execute("Ενημέρωση", "Παρακαλώ εισάγετε κωδικό και ξαναπροσπαθήστε");
                txtProduct.Focus();
                return false;
            }
            else
                if (string.IsNullOrEmpty(txtProductMatching.Text.Trim()))
                {
                    MessageForm.Execute("Ενημέρωση", "Παρακαλώ εισάγετε κωδικό αντιστοιχίας και ξαναπροσπαθήστε");
                    txtProductMatching.Focus();
                    return false;
                }
            return true;
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Validates and saves the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.document.Header.Save();
            this.Close();
        }

        /// <summary>
        /// Processing of keypresses inside the product field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtProduct))
                return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                string search = txtProduct.Text.Trim();
                EAN128Barcode b128 = new EAN128Barcode(search, AppSettings.B128Settings);
                search = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.MATCHING, b128, search, txtProduct);
                txtProductMatching.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// If the user presses escape then we hit the return button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MatchingForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                this.btnReturn_Click(this, new EventArgs());
            }
        }

        /// <summary>
        /// On form closing we nullify the form document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MatchingForm_Closing(object sender, CancelEventArgs e)
        {
            this.document = null;
        }
        private void radionButtonReplace_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void radioButtonAppend_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void txtRecords_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void txtProductMatching_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtProduct))
                return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string productMatchingCode = txtProductMatching.Text.Trim();
                EAN128Barcode b = new EAN128Barcode(productMatchingCode, AppSettings.B128Settings);
                productMatchingCode = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.MATCHING, b, productMatchingCode, txtProductMatching);
                AddMatching();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                MessageForm.Execute("Πληροφορία", "Πρέπει να είστε Online για να πραγματοποιηθεί η εξαγωγή.");
                return;
            }
            try
            {
                btnExport.Enabled = false;
                if (this.document.Header.CountLines() <= 0)
                {
                    MessageForm.Execute("Πληροφορία", "Έισάγετε τουλάχιστον μία εγγραφή και ξαναπροσπαθήστε");
                    return;
                }
                
                //using (SelectOutputPath sop = new SelectOutputPath(DOC_TYPES.MATCHING))
                //{
                //    if (sop.SelectedOutputPath == null && sop.DialogResult != DialogResult.Cancel)
                //        sop.ShowDialog();
                //    if (sop.DialogResult == DialogResult.OK)
                //        outputPath = sop.SelectedOutputPath;
                //    this.Show(); // in case someone minimized it in the meanwhile..
                //}

                //if (outputPath != null)
                //{
                    this.document.Header.Save();
                    string errorMessage;
                    if (Main.ExportDocument(document.Header.DocType, false, out errorMessage))
                    {
                        MessageForm.Execute("Επιτυχία", "H εξαγωγή ήταν επιτυχής!\r\n" + errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                        this.Close();
                    }
                //}
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα κατά την Online εξαγωγή\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
            finally
            {
                btnExport.Enabled = true;
            }
        }

        private void txtProductMatching_GotFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProduct.Text.Trim()))
                txtProduct.Focus();
        }
        #endregion

        #region Properties
        #endregion

        #region Disposer
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatchingForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.panForm = new System.Windows.Forms.Panel();
            this.txtProductMatching = new System.Windows.Forms.TextBox();
            this.lblProductMatching = new System.Windows.Forms.Label();
            this.btnExport = new OpenNETCF.Windows.Forms.Button2();
            this.txtRecords = new System.Windows.Forms.TextBox();
            this.lblRecords = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.btnReturn = new OpenNETCF.Windows.Forms.Button2();
            this.panForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // panForm
            // 
            this.panForm.Controls.Add(this.txtProductMatching);
            this.panForm.Controls.Add(this.lblProductMatching);
            this.panForm.Controls.Add(this.btnExport);
            this.panForm.Controls.Add(this.txtRecords);
            this.panForm.Controls.Add(this.lblRecords);
            this.panForm.Controls.Add(this.lblProduct);
            this.panForm.Controls.Add(this.txtProduct);
            this.panForm.Controls.Add(this.btnReturn);
            this.panForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panForm.Location = new System.Drawing.Point(0, 0);
            this.panForm.Name = "panForm";
            this.panForm.Size = new System.Drawing.Size(238, 268);
            // 
            // txtProductMatching
            // 
            this.txtProductMatching.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProductMatching.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProductMatching.Location = new System.Drawing.Point(107, 69);
            this.txtProductMatching.Multiline = true;
            this.txtProductMatching.Name = "txtProductMatching";
            this.txtProductMatching.Size = new System.Drawing.Size(128, 24);
            this.txtProductMatching.TabIndex = 58;
            this.txtProductMatching.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProductMatching.GotFocus += new System.EventHandler(this.txtProductMatching_GotFocus);
            this.txtProductMatching.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProductMatching_KeyPress);
            // 
            // lblProductMatching
            // 
            this.lblProductMatching.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductMatching.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProductMatching.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProductMatching.ForeColor = System.Drawing.Color.Black;
            this.lblProductMatching.Location = new System.Drawing.Point(4, 69);
            this.lblProductMatching.Name = "lblProductMatching";
            this.lblProductMatching.Size = new System.Drawing.Size(97, 24);
            this.lblProductMatching.Text = "Κωδ. Αντιστ.";
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
            this.btnExport.Size = new System.Drawing.Size(113, 32);
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
            this.txtRecords.Location = new System.Drawing.Point(107, 9);
            this.txtRecords.Multiline = true;
            this.txtRecords.Name = "txtRecords";
            this.txtRecords.Size = new System.Drawing.Size(128, 24);
            this.txtRecords.TabIndex = 3;
            this.txtRecords.TabStop = false;
            this.txtRecords.Text = "0";
            this.txtRecords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecords.GotFocus += new System.EventHandler(this.txtRecords_GotFocus);
            // 
            // lblRecords
            // 
            this.lblRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecords.BackColor = System.Drawing.Color.Gainsboro;
            this.lblRecords.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblRecords.ForeColor = System.Drawing.Color.Black;
            this.lblRecords.Location = new System.Drawing.Point(4, 9);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(97, 24);
            this.lblRecords.Text = "Εγγραφές";
            // 
            // lblProduct
            // 
            this.lblProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.lblProduct.ForeColor = System.Drawing.Color.Black;
            this.lblProduct.Location = new System.Drawing.Point(4, 39);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(97, 24);
            this.lblProduct.Text = "Κωδικός";
            // 
            // txtProduct
            // 
            this.txtProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProduct.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
            this.txtProduct.Location = new System.Drawing.Point(107, 39);
            this.txtProduct.Multiline = true;
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(128, 24);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
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
            this.btnReturn.Location = new System.Drawing.Point(123, 233);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(111, 32);
            this.btnReturn.TabIndex = 1;
            this.btnReturn.Text = "Έξοδος";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // MatchingForm
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
            this.Name = "MatchingForm";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MatchingForm_Paint);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MatchingForm_Closing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MatchingForm_KeyPress);
            this.panForm.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void MatchingForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
