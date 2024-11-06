using DevExpress.XtraEditors;

namespace ITS.POS.Client.Forms
{
    partial class frmSelectCustomerGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule2 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule3 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule4 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule5 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule6 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule7 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule8 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule9 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule10 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule11 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule12 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectCustomerGrid));
            this.validationProviderCustomer = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.txtName = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtSurname = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtTaxCode = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtCompanyName = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtCustomerCode = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtTaxOffice = new DevExpress.XtraEditors.LookUpEdit();
            this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.txtThirdPartNum = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtAddressProfession = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblCityPhone = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.txtAddressCity = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtPhone = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtAddressStreet = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.txtAddressPostalCode = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblFullname = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.lblTaxCodeOffice = new DevExpress.XtraEditors.LabelControl();
            this.lblCardID = new DevExpress.XtraEditors.LabelControl();
            this.lblCompanyName = new DevExpress.XtraEditors.LabelControl();
            this.lblCustomerCode = new DevExpress.XtraEditors.LabelControl();
            this.txtCardID = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddNewCustomer = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddNewCustomerAddress = new DevExpress.XtraEditors.SimpleButton();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.panelControl6 = new DevExpress.XtraEditors.PanelControl();
            this.OKButton = new DevExpress.XtraEditors.SimpleButton();
            this.CancelButton = new DevExpress.XtraEditors.SimpleButton();
            this.btnPreviousAddress = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextAddress = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl7 = new DevExpress.XtraEditors.PanelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.bsCustomers = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.CodeCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CompanyCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ProfessionCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TaxCodeCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TaxOfficeCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CardIDCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.validationProviderCustomerAddress = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.validationProviderCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSurname.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxOffice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
            this.panelControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtThirdPartNum.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressProfession.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressCity.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressStreet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressPostalCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
            this.panelControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl7)).BeginInit();
            this.panelControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validationProviderCustomerAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // validationProviderCustomer
            // 
            this.validationProviderCustomer.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            // 
            // txtName
            // 
            this.txtName.AutoHideTouchPad = false;
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(517, 188);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.PoleDisplayName = "";
            this.txtName.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtName.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtName.Properties.Appearance.Options.UseBackColor = true;
            this.txtName.Properties.Appearance.Options.UseFont = true;
            this.txtName.Properties.Appearance.Options.UseForeColor = true;
            this.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtName.Size = new System.Drawing.Size(248, 26);
            this.txtName.TabIndex = 6;
            conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule1.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomer.SetValidationRule(this.txtName, conditionValidationRule1);
            // 
            // txtSurname
            // 
            this.txtSurname.AutoHideTouchPad = false;
            this.txtSurname.Enabled = false;
            this.txtSurname.Location = new System.Drawing.Point(9, 188);
            this.txtSurname.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSurname.Name = "txtSurname";
            this.txtSurname.PoleDisplayName = "";
            this.txtSurname.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtSurname.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtSurname.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtSurname.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtSurname.Properties.Appearance.Options.UseBackColor = true;
            this.txtSurname.Properties.Appearance.Options.UseFont = true;
            this.txtSurname.Properties.Appearance.Options.UseForeColor = true;
            this.txtSurname.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtSurname.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtSurname.Size = new System.Drawing.Size(502, 26);
            this.txtSurname.TabIndex = 5;
            conditionValidationRule2.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule2.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomer.SetValidationRule(this.txtSurname, conditionValidationRule2);
            // 
            // txtTaxCode
            // 
            this.txtTaxCode.AutoHideTouchPad = false;
            this.txtTaxCode.Enabled = false;
            this.txtTaxCode.Location = new System.Drawing.Point(9, 120);
            this.txtTaxCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTaxCode.Name = "txtTaxCode";
            this.txtTaxCode.PoleDisplayName = "";
            this.txtTaxCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtTaxCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtTaxCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtTaxCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtTaxCode.Properties.Appearance.Options.UseBackColor = true;
            this.txtTaxCode.Properties.Appearance.Options.UseFont = true;
            this.txtTaxCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtTaxCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtTaxCode.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtTaxCode.Size = new System.Drawing.Size(160, 26);
            this.txtTaxCode.TabIndex = 2;
            conditionValidationRule3.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule3.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomer.SetValidationRule(this.txtTaxCode, conditionValidationRule3);
            this.txtTaxCode.Validating += new System.ComponentModel.CancelEventHandler(this.SearchTraderOnValidation);
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.AutoHideTouchPad = false;
            this.txtCompanyName.Enabled = false;
            this.txtCompanyName.Location = new System.Drawing.Point(175, 45);
            this.txtCompanyName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.PoleDisplayName = "";
            this.txtCompanyName.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtCompanyName.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtCompanyName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtCompanyName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCompanyName.Properties.Appearance.Options.UseBackColor = true;
            this.txtCompanyName.Properties.Appearance.Options.UseFont = true;
            this.txtCompanyName.Properties.Appearance.Options.UseForeColor = true;
            this.txtCompanyName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtCompanyName.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtCompanyName.Size = new System.Drawing.Size(585, 26);
            this.txtCompanyName.TabIndex = 1;
            conditionValidationRule4.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule4.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomer.SetValidationRule(this.txtCompanyName, conditionValidationRule4);
            // 
            // txtCustomerCode
            // 
            this.txtCustomerCode.AutoHideTouchPad = false;
            this.txtCustomerCode.Enabled = false;
            this.txtCustomerCode.Location = new System.Drawing.Point(8, 45);
            this.txtCustomerCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCustomerCode.Name = "txtCustomerCode";
            this.txtCustomerCode.PoleDisplayName = "";
            this.txtCustomerCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtCustomerCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtCustomerCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtCustomerCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCustomerCode.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerCode.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtCustomerCode.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtCustomerCode.Size = new System.Drawing.Size(161, 26);
            this.txtCustomerCode.TabIndex = 0;
            conditionValidationRule5.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule5.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomer.SetValidationRule(this.txtCustomerCode, conditionValidationRule5);
            this.txtCustomerCode.Validating += new System.ComponentModel.CancelEventHandler(this.SearchTraderOnValidation);
            // 
            // txtTaxOffice
            // 
            this.txtTaxOffice.Location = new System.Drawing.Point(175, 120);
            this.txtTaxOffice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTaxOffice.Name = "txtTaxOffice";
            this.txtTaxOffice.Properties.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.False;
            this.txtTaxOffice.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtTaxOffice.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtTaxOffice.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtTaxOffice.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtTaxOffice.Properties.Appearance.Options.UseBackColor = true;
            this.txtTaxOffice.Properties.Appearance.Options.UseFont = true;
            this.txtTaxOffice.Properties.Appearance.Options.UseForeColor = true;
            this.txtTaxOffice.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtTaxOffice.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtTaxOffice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtTaxOffice.Properties.NullText = "";
            this.txtTaxOffice.Size = new System.Drawing.Size(238, 26);
            this.txtTaxOffice.TabIndex = 3;
            conditionValidationRule6.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.Equals;
            conditionValidationRule6.ErrorText = "Εισάγετε τιμές";
            conditionValidationRule6.Value1 = "\"\"";
            conditionValidationRule6.Values.Add(" ");
            this.validationProviderCustomer.SetValidationRule(this.txtTaxOffice, conditionValidationRule6);
            this.txtTaxOffice.Validating += new System.ComponentModel.CancelEventHandler(this.txtTaxOffice_Validating);
            // 
            // panelControl5
            // 
            this.panelControl5.Appearance.BorderColor = System.Drawing.Color.Blue;
            this.panelControl5.Appearance.Options.UseBorderColor = true;
            this.panelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl5.Controls.Add(this.panelControl4);
            this.panelControl5.Controls.Add(this.panelControl2);
            this.panelControl5.Controls.Add(this.panelControl3);
            this.panelControl5.Controls.Add(this.panelControl6);
            this.panelControl5.Controls.Add(this.panelControl7);
            this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl5.Location = new System.Drawing.Point(0, 0);
            this.panelControl5.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl5.Name = "panelControl5";
            this.panelControl5.Padding = new System.Windows.Forms.Padding(10, 13, 10, 13);
            this.panelControl5.Size = new System.Drawing.Size(784, 584);
            this.panelControl5.TabIndex = 0;
            // 
            // panelControl4
            // 
            this.panelControl4.Appearance.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelControl4.Appearance.Options.UseBackColor = true;
            this.panelControl4.Controls.Add(this.txtThirdPartNum);
            this.panelControl4.Controls.Add(this.labelControl6);
            this.panelControl4.Controls.Add(this.labelControl5);
            this.panelControl4.Controls.Add(this.txtAddressProfession);
            this.panelControl4.Controls.Add(this.labelControl1);
            this.panelControl4.Controls.Add(this.lblCityPhone);
            this.panelControl4.Controls.Add(this.labelControl4);
            this.panelControl4.Controls.Add(this.lblAddress);
            this.panelControl4.Controls.Add(this.txtAddressCity);
            this.panelControl4.Controls.Add(this.txtPhone);
            this.panelControl4.Controls.Add(this.txtAddressStreet);
            this.panelControl4.Controls.Add(this.txtAddressPostalCode);
            this.panelControl4.Location = new System.Drawing.Point(10, 345);
            this.panelControl4.LookAndFeel.SkinName = "Office 2010 Blue";
            this.panelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Size = new System.Drawing.Size(764, 154);
            this.panelControl4.TabIndex = 1;
            // 
            // txtThirdPartNum
            // 
            this.txtThirdPartNum.AutoHideTouchPad = false;
            this.txtThirdPartNum.CausesValidation = false;
            this.txtThirdPartNum.Enabled = false;
            this.txtThirdPartNum.Location = new System.Drawing.Point(376, 116);
            this.txtThirdPartNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtThirdPartNum.Name = "txtThirdPartNum";
            this.txtThirdPartNum.PoleDisplayName = "";
            this.txtThirdPartNum.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtThirdPartNum.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtThirdPartNum.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtThirdPartNum.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtThirdPartNum.Properties.Appearance.Options.UseBackColor = true;
            this.txtThirdPartNum.Properties.Appearance.Options.UseFont = true;
            this.txtThirdPartNum.Properties.Appearance.Options.UseForeColor = true;
            this.txtThirdPartNum.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtThirdPartNum.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtThirdPartNum.Size = new System.Drawing.Size(342, 26);
            this.txtThirdPartNum.TabIndex = 29;
            conditionValidationRule7.ErrorText = "";
            conditionValidationRule7.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
            this.validationProviderCustomerAddress.SetValidationRule(this.txtThirdPartNum, conditionValidationRule7);
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl6.Location = new System.Drawing.Point(376, 83);
            this.labelControl6.LookAndFeel.SkinName = "Metropolis";
            this.labelControl6.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(204, 19);
            this.labelControl6.TabIndex = 28;
            this.labelControl6.Text = "Κώδικος Τρίτου Συστήματος";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl5.Location = new System.Drawing.Point(9, 83);
            this.labelControl5.LookAndFeel.SkinName = "Metropolis";
            this.labelControl5.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(76, 19);
            this.labelControl5.TabIndex = 26;
            this.labelControl5.Text = "Επάγγελμα";
            // 
            // txtAddressProfession
            // 
            this.txtAddressProfession.AutoHideTouchPad = false;
            this.txtAddressProfession.Enabled = false;
            this.txtAddressProfession.Location = new System.Drawing.Point(9, 116);
            this.txtAddressProfession.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddressProfession.Name = "txtAddressProfession";
            this.txtAddressProfession.PoleDisplayName = "";
            this.txtAddressProfession.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtAddressProfession.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtAddressProfession.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAddressProfession.Properties.Appearance.Options.UseBackColor = true;
            this.txtAddressProfession.Properties.Appearance.Options.UseFont = true;
            this.txtAddressProfession.Properties.Appearance.Options.UseForeColor = true;
            this.txtAddressProfession.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtAddressProfession.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtAddressProfession.Size = new System.Drawing.Size(361, 26);
            this.txtAddressProfession.TabIndex = 4;
            conditionValidationRule8.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule8.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomerAddress.SetValidationRule(this.txtAddressProfession, conditionValidationRule8);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl1.Location = new System.Drawing.Point(636, 5);
            this.labelControl1.LookAndFeel.SkinName = "Metropolis";
            this.labelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(75, 19);
            this.labelControl1.TabIndex = 21;
            this.labelControl1.Text = "Tηλέφωνο";
            // 
            // lblCityPhone
            // 
            this.lblCityPhone.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblCityPhone.Location = new System.Drawing.Point(376, 5);
            this.lblCityPhone.LookAndFeel.SkinName = "Metropolis";
            this.lblCityPhone.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblCityPhone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblCityPhone.Name = "lblCityPhone";
            this.lblCityPhone.Size = new System.Drawing.Size(37, 19);
            this.lblCityPhone.TabIndex = 22;
            this.lblCityPhone.Text = "Πόλη";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl4.Location = new System.Drawing.Point(517, 5);
            this.labelControl4.LookAndFeel.SkinName = "Metropolis";
            this.labelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(110, 19);
            this.labelControl4.TabIndex = 23;
            this.labelControl4.Text = "Διεύθυνση (ΤΚ)";
            // 
            // lblAddress
            // 
            this.lblAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblAddress.Location = new System.Drawing.Point(9, 5);
            this.lblAddress.LookAndFeel.SkinName = "Metropolis";
            this.lblAddress.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(207, 19);
            this.lblAddress.TabIndex = 24;
            this.lblAddress.Text = "Διεύθυνση (Οδός && Αριθμός)";
            // 
            // txtAddressCity
            // 
            this.txtAddressCity.AutoHideTouchPad = false;
            this.txtAddressCity.Enabled = false;
            this.txtAddressCity.Location = new System.Drawing.Point(376, 38);
            this.txtAddressCity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddressCity.Name = "txtAddressCity";
            this.txtAddressCity.PoleDisplayName = "";
            this.txtAddressCity.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtAddressCity.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtAddressCity.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAddressCity.Properties.Appearance.Options.UseBackColor = true;
            this.txtAddressCity.Properties.Appearance.Options.UseFont = true;
            this.txtAddressCity.Properties.Appearance.Options.UseForeColor = true;
            this.txtAddressCity.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtAddressCity.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtAddressCity.Size = new System.Drawing.Size(135, 26);
            this.txtAddressCity.TabIndex = 1;
            conditionValidationRule9.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule9.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomerAddress.SetValidationRule(this.txtAddressCity, conditionValidationRule9);
            // 
            // txtPhone
            // 
            this.txtPhone.AutoHideTouchPad = false;
            this.txtPhone.EditValue = "";
            this.txtPhone.Enabled = false;
            this.txtPhone.Location = new System.Drawing.Point(636, 38);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.PoleDisplayName = "";
            this.txtPhone.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtPhone.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtPhone.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtPhone.Properties.Appearance.Options.UseBackColor = true;
            this.txtPhone.Properties.Appearance.Options.UseFont = true;
            this.txtPhone.Properties.Appearance.Options.UseForeColor = true;
            this.txtPhone.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPhone.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtPhone.Size = new System.Drawing.Size(129, 26);
            this.txtPhone.TabIndex = 3;
            conditionValidationRule10.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule10.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomerAddress.SetValidationRule(this.txtPhone, conditionValidationRule10);
            // 
            // txtAddressStreet
            // 
            this.txtAddressStreet.AutoHideTouchPad = false;
            this.txtAddressStreet.Enabled = false;
            this.txtAddressStreet.Location = new System.Drawing.Point(9, 38);
            this.txtAddressStreet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddressStreet.Name = "txtAddressStreet";
            this.txtAddressStreet.PoleDisplayName = "";
            this.txtAddressStreet.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtAddressStreet.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtAddressStreet.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAddressStreet.Properties.Appearance.Options.UseBackColor = true;
            this.txtAddressStreet.Properties.Appearance.Options.UseFont = true;
            this.txtAddressStreet.Properties.Appearance.Options.UseForeColor = true;
            this.txtAddressStreet.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtAddressStreet.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtAddressStreet.Size = new System.Drawing.Size(361, 26);
            this.txtAddressStreet.TabIndex = 0;
            conditionValidationRule11.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule11.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomerAddress.SetValidationRule(this.txtAddressStreet, conditionValidationRule11);
            // 
            // txtAddressPostalCode
            // 
            this.txtAddressPostalCode.AutoHideTouchPad = false;
            this.txtAddressPostalCode.EditValue = "";
            this.txtAddressPostalCode.Enabled = false;
            this.txtAddressPostalCode.Location = new System.Drawing.Point(517, 38);
            this.txtAddressPostalCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddressPostalCode.Name = "txtAddressPostalCode";
            this.txtAddressPostalCode.PoleDisplayName = "";
            this.txtAddressPostalCode.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtAddressPostalCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtAddressPostalCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtAddressPostalCode.Properties.Appearance.Options.UseBackColor = true;
            this.txtAddressPostalCode.Properties.Appearance.Options.UseFont = true;
            this.txtAddressPostalCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtAddressPostalCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtAddressPostalCode.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtAddressPostalCode.Size = new System.Drawing.Size(113, 26);
            this.txtAddressPostalCode.TabIndex = 2;
            conditionValidationRule12.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule12.ErrorText = "Εισάγετε τιμές";
            this.validationProviderCustomerAddress.SetValidationRule(this.txtAddressPostalCode, conditionValidationRule12);
            // 
            // panelControl2
            // 
            this.panelControl2.Appearance.BackColor = System.Drawing.Color.Teal;
            this.panelControl2.Appearance.Options.UseBackColor = true;
            this.panelControl2.Controls.Add(this.labelControl2);
            this.panelControl2.Controls.Add(this.lblFullname);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Controls.Add(this.lblTaxCodeOffice);
            this.panelControl2.Controls.Add(this.lblCardID);
            this.panelControl2.Controls.Add(this.lblCompanyName);
            this.panelControl2.Controls.Add(this.lblCustomerCode);
            this.panelControl2.Controls.Add(this.txtName);
            this.panelControl2.Controls.Add(this.txtSurname);
            this.panelControl2.Controls.Add(this.txtCardID);
            this.panelControl2.Controls.Add(this.txtTaxCode);
            this.panelControl2.Controls.Add(this.txtCompanyName);
            this.panelControl2.Controls.Add(this.txtCustomerCode);
            this.panelControl2.Controls.Add(this.txtTaxOffice);
            this.panelControl2.Cursor = System.Windows.Forms.Cursors.Default;
            this.panelControl2.Location = new System.Drawing.Point(10, 116);
            this.panelControl2.LookAndFeel.SkinName = "Office 2007 Blue";
            this.panelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(764, 229);
            this.panelControl2.TabIndex = 0;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl2.Location = new System.Drawing.Point(517, 156);
            this.labelControl2.LookAndFeel.SkinName = "Metropolis";
            this.labelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(47, 19);
            this.labelControl2.TabIndex = 17;
            this.labelControl2.Text = "Ονομα";
            // 
            // lblFullname
            // 
            this.lblFullname.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblFullname.Location = new System.Drawing.Point(9, 156);
            this.lblFullname.LookAndFeel.SkinName = "Metropolis";
            this.lblFullname.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblFullname.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblFullname.Name = "lblFullname";
            this.lblFullname.Size = new System.Drawing.Size(65, 19);
            this.lblFullname.TabIndex = 18;
            this.lblFullname.Text = "Επώνυμο";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl3.Location = new System.Drawing.Point(175, 87);
            this.labelControl3.LookAndFeel.SkinName = "Metropolis";
            this.labelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(32, 19);
            this.labelControl3.TabIndex = 19;
            this.labelControl3.Text = "ΔΟΥ";
            // 
            // lblTaxCodeOffice
            // 
            this.lblTaxCodeOffice.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTaxCodeOffice.Location = new System.Drawing.Point(9, 87);
            this.lblTaxCodeOffice.LookAndFeel.SkinName = "Metropolis";
            this.lblTaxCodeOffice.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTaxCodeOffice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTaxCodeOffice.Name = "lblTaxCodeOffice";
            this.lblTaxCodeOffice.Size = new System.Drawing.Size(35, 19);
            this.lblTaxCodeOffice.TabIndex = 20;
            this.lblTaxCodeOffice.Text = "ΑΦΜ";
            // 
            // lblCardID
            // 
            this.lblCardID.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblCardID.Location = new System.Drawing.Point(419, 87);
            this.lblCardID.LookAndFeel.SkinName = "Metropolis";
            this.lblCardID.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblCardID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblCardID.Name = "lblCardID";
            this.lblCardID.Size = new System.Drawing.Size(115, 19);
            this.lblCardID.TabIndex = 21;
            this.lblCardID.Text = "Κωδικός Κάρτας";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblCompanyName.Location = new System.Drawing.Point(175, 13);
            this.lblCompanyName.LookAndFeel.SkinName = "Metropolis";
            this.lblCompanyName.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblCompanyName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(69, 19);
            this.lblCompanyName.TabIndex = 22;
            this.lblCompanyName.Text = "Επωνυμία";
            // 
            // lblCustomerCode
            // 
            this.lblCustomerCode.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblCustomerCode.Location = new System.Drawing.Point(8, 13);
            this.lblCustomerCode.LookAndFeel.SkinName = "Metropolis";
            this.lblCustomerCode.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblCustomerCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblCustomerCode.Name = "lblCustomerCode";
            this.lblCustomerCode.Size = new System.Drawing.Size(115, 19);
            this.lblCustomerCode.TabIndex = 23;
            this.lblCustomerCode.Text = "Κωδικός Πελάτη";
            // 
            // txtCardID
            // 
            this.txtCardID.AutoHideTouchPad = false;
            this.txtCardID.Enabled = false;
            this.txtCardID.Location = new System.Drawing.Point(419, 120);
            this.txtCardID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCardID.Name = "txtCardID";
            this.txtCardID.PoleDisplayName = "";
            this.txtCardID.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtCardID.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.txtCardID.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtCardID.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCardID.Properties.Appearance.Options.UseBackColor = true;
            this.txtCardID.Properties.Appearance.Options.UseFont = true;
            this.txtCardID.Properties.Appearance.Options.UseForeColor = true;
            this.txtCardID.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtCardID.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.txtCardID.Size = new System.Drawing.Size(342, 26);
            this.txtCardID.TabIndex = 4;
            this.txtCardID.Validating += new System.ComponentModel.CancelEventHandler(this.SearchTraderOnValidation);
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.btnSearch);
            this.panelControl3.Controls.Add(this.btnAddNewCustomer);
            this.panelControl3.Controls.Add(this.btnAddNewCustomerAddress);
            this.panelControl3.Controls.Add(this.textEdit1);
            this.panelControl3.Controls.Add(this.lblTitle);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl3.Location = new System.Drawing.Point(10, 13);
            this.panelControl3.LookAndFeel.SkinName = "Seven";
            this.panelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(764, 103);
            this.panelControl3.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnSearch.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnSearch.Appearance.Options.UseBackColor = true;
            this.btnSearch.Appearance.Options.UseBorderColor = true;
            this.btnSearch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnSearch.Location = new System.Drawing.Point(361, 16);
            this.btnSearch.LookAndFeel.SkinName = "Metropolis";
            this.btnSearch.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(113, 71);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Αναζήτηση";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnAddNewCustomer
            // 
            this.btnAddNewCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewCustomer.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnAddNewCustomer.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnAddNewCustomer.Appearance.Options.UseBackColor = true;
            this.btnAddNewCustomer.Appearance.Options.UseBorderColor = true;
            this.btnAddNewCustomer.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnAddNewCustomer.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNewCustomer.Image")));
            this.btnAddNewCustomer.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAddNewCustomer.Location = new System.Drawing.Point(480, 16);
            this.btnAddNewCustomer.LookAndFeel.SkinName = "Metropolis";
            this.btnAddNewCustomer.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddNewCustomer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddNewCustomer.Name = "btnAddNewCustomer";
            this.btnAddNewCustomer.Size = new System.Drawing.Size(129, 71);
            this.btnAddNewCustomer.TabIndex = 2;
            this.btnAddNewCustomer.Text = "Νέος Πελάτης";
            this.btnAddNewCustomer.Click += new System.EventHandler(this.btnAddNewCustomer_Click);
            // 
            // btnAddNewCustomerAddress
            // 
            this.btnAddNewCustomerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewCustomerAddress.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnAddNewCustomerAddress.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnAddNewCustomerAddress.Appearance.Options.UseBackColor = true;
            this.btnAddNewCustomerAddress.Appearance.Options.UseBorderColor = true;
            this.btnAddNewCustomerAddress.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnAddNewCustomerAddress.Enabled = false;
            this.btnAddNewCustomerAddress.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNewCustomerAddress.Image")));
            this.btnAddNewCustomerAddress.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAddNewCustomerAddress.Location = new System.Drawing.Point(615, 16);
            this.btnAddNewCustomerAddress.LookAndFeel.SkinName = "Metropolis";
            this.btnAddNewCustomerAddress.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddNewCustomerAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddNewCustomerAddress.Name = "btnAddNewCustomerAddress";
            this.btnAddNewCustomerAddress.Size = new System.Drawing.Size(139, 71);
            this.btnAddNewCustomerAddress.TabIndex = 3;
            this.btnAddNewCustomerAddress.Text = "Νέα Διεύθυνση";
            this.btnAddNewCustomerAddress.Click += new System.EventHandler(this.btnAddNewCustomerAddress_Click);
            // 
            // textEdit1
            // 
            this.textEdit1.EditValue = "";
            this.textEdit1.Location = new System.Drawing.Point(9, 48);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.textEdit1.Properties.Appearance.Options.UseFont = true;
            this.textEdit1.Size = new System.Drawing.Size(326, 26);
            this.textEdit1.TabIndex = 0;
            this.textEdit1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextEdit_KeyDown);
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitle.Location = new System.Drawing.Point(9, 21);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(135, 19);
            this.lblTitle.TabIndex = 20;
            this.lblTitle.Text = "Αναζήτηση Πελάτη";
            // 
            // panelControl6
            // 
            this.panelControl6.Controls.Add(this.OKButton);
            this.panelControl6.Controls.Add(this.CancelButton);
            this.panelControl6.Controls.Add(this.btnPreviousAddress);
            this.panelControl6.Controls.Add(this.btnNextAddress);
            this.panelControl6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl6.Location = new System.Drawing.Point(10, 497);
            this.panelControl6.LookAndFeel.SkinName = "Seven";
            this.panelControl6.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl6.Name = "panelControl6";
            this.panelControl6.Size = new System.Drawing.Size(764, 74);
            this.panelControl6.TabIndex = 4;
            // 
            // OKButton
            // 
            this.OKButton.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.OKButton.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.OKButton.Appearance.Options.UseBackColor = true;
            this.OKButton.Appearance.Options.UseBorderColor = true;
            this.OKButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.OKButton.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.OKButton.Location = new System.Drawing.Point(394, 10);
            this.OKButton.LookAndFeel.SkinName = "Metropolis";
            this.OKButton.LookAndFeel.UseDefaultLookAndFeel = false;
            this.OKButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(129, 58);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "Επιλογή";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.AllowFocus = false;
            this.CancelButton.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.CancelButton.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.CancelButton.Appearance.Options.UseBackColor = true;
            this.CancelButton.Appearance.Options.UseBorderColor = true;
            this.CancelButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.CancelButton.Location = new System.Drawing.Point(554, 10);
            this.CancelButton.LookAndFeel.SkinName = "Metropolis";
            this.CancelButton.LookAndFeel.UseDefaultLookAndFeel = false;
            this.CancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(138, 58);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Ακύρωση";
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // btnPreviousAddress
            // 
            this.btnPreviousAddress.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnPreviousAddress.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnPreviousAddress.Appearance.Options.UseBackColor = true;
            this.btnPreviousAddress.Appearance.Options.UseBorderColor = true;
            this.btnPreviousAddress.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnPreviousAddress.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousAddress.Image")));
            this.btnPreviousAddress.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPreviousAddress.Location = new System.Drawing.Point(88, 10);
            this.btnPreviousAddress.LookAndFeel.SkinName = "Metropolis";
            this.btnPreviousAddress.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPreviousAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPreviousAddress.Name = "btnPreviousAddress";
            this.btnPreviousAddress.Size = new System.Drawing.Size(61, 58);
            this.btnPreviousAddress.TabIndex = 0;
            this.btnPreviousAddress.Click += new System.EventHandler(this.btnPreviousAddress_Click);
            // 
            // btnNextAddress
            // 
            this.btnNextAddress.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnNextAddress.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnNextAddress.Appearance.Options.UseBackColor = true;
            this.btnNextAddress.Appearance.Options.UseBorderColor = true;
            this.btnNextAddress.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnNextAddress.Image = ((System.Drawing.Image)(resources.GetObject("btnNextAddress.Image")));
            this.btnNextAddress.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnNextAddress.Location = new System.Drawing.Point(190, 10);
            this.btnNextAddress.LookAndFeel.SkinName = "Metropolis";
            this.btnNextAddress.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNextAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNextAddress.Name = "btnNextAddress";
            this.btnNextAddress.Size = new System.Drawing.Size(65, 58);
            this.btnNextAddress.TabIndex = 1;
            this.btnNextAddress.Click += new System.EventHandler(this.btnNextAddress_Click);
            // 
            // panelControl7
            // 
            this.panelControl7.Appearance.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelControl7.Appearance.Options.UseBackColor = true;
            this.panelControl7.Controls.Add(this.gridControl1);
            this.panelControl7.Location = new System.Drawing.Point(10, 124);
            this.panelControl7.LookAndFeel.SkinName = "Office 2010 Blue";
            this.panelControl7.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl7.Name = "panelControl7";
            this.panelControl7.Size = new System.Drawing.Size(764, 363);
            this.panelControl7.TabIndex = 0;
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.bsCustomers;
            this.gridControl1.EmbeddedNavigator.Enabled = false;
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(764, 363);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            // 
            // gridView1
            // 
            this.gridView1.ActiveFilterEnabled = false;
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridView1.Appearance.HeaderPanel.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.CodeCol,
            this.CompanyCol,
            this.ProfessionCol,
            this.TaxCodeCol,
            this.TaxOfficeCol,
            this.CardIDCol});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AutoSelectAllInEditor = false;
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsBehavior.ReadOnly = true;
            this.gridView1.OptionsBehavior.SmartVertScrollBar = false;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsDetail.EnableMasterViewMode = false;
            this.gridView1.OptionsFilter.AllowFilterEditor = false;
            this.gridView1.OptionsNavigation.UseTabKey = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.RowHeight = 35;
            // 
            // CodeCol
            // 
            this.CodeCol.Caption = "Κωδικός Πελάτη";
            this.CodeCol.FieldName = "Code";
            this.CodeCol.Name = "CodeCol";
            this.CodeCol.Visible = true;
            this.CodeCol.VisibleIndex = 0;
            // 
            // CompanyCol
            // 
            this.CompanyCol.Caption = "Επωνυμία";
            this.CompanyCol.FieldName = "CompanyName";
            this.CompanyCol.Name = "CompanyCol";
            this.CompanyCol.Visible = true;
            this.CompanyCol.VisibleIndex = 1;
            // 
            // ProfessionCol
            // 
            this.ProfessionCol.Caption = "Επάγγελμα";
            this.ProfessionCol.FieldName = "Profession";
            this.ProfessionCol.Name = "ProfessionCol";
            this.ProfessionCol.Visible = true;
            this.ProfessionCol.VisibleIndex = 2;
            // 
            // TaxCodeCol
            // 
            this.TaxCodeCol.Caption = "Α.Φ.Μ.";
            this.TaxCodeCol.FieldName = "TaxCode";
            this.TaxCodeCol.Name = "TaxCodeCol";
            this.TaxCodeCol.Visible = true;
            this.TaxCodeCol.VisibleIndex = 3;
            // 
            // TaxOfficeCol
            // 
            this.TaxOfficeCol.Caption = "Δ.Ο.Υ.";
            this.TaxOfficeCol.FieldName = "TaxOfficeDescription";
            this.TaxOfficeCol.Name = "TaxOfficeCol";
            this.TaxOfficeCol.Visible = true;
            this.TaxOfficeCol.VisibleIndex = 4;
            // 
            // CardIDCol
            // 
            this.CardIDCol.Caption = "CardID";
            this.CardIDCol.FieldName = "CardID";
            this.CardIDCol.Name = "CardIDCol";
            this.CardIDCol.Visible = true;
            this.CardIDCol.VisibleIndex = 5;
            // 
            // validationProviderCustomerAddress
            // 
            this.validationProviderCustomerAddress.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            // 
            // frmSelectCustomerGrid
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 584);
            this.Controls.Add(this.panelControl5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmSelectCustomerGrid";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSelectCustomerGrid";
            ((System.ComponentModel.ISupportInitialize)(this.validationProviderCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSurname.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxOffice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
            this.panelControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            this.panelControl4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtThirdPartNum.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressProfession.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressCity.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressStreet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddressPostalCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.panelControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
            this.panelControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl7)).EndInit();
            this.panelControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validationProviderCustomerAddress)).EndInit();
            this.ResumeLayout(false);

        }

        private void GridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.BindingSource bsCustomers;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider validationProviderCustomer;
        private DevExpress.XtraEditors.PanelControl panelControl5;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider validationProviderCustomerAddress;
        private DevExpress.XtraEditors.PanelControl panelControl6;
        private DevExpress.XtraEditors.PanelControl panelControl7;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        public DevExpress.XtraEditors.SimpleButton OKButton;
        public DevExpress.XtraEditors.SimpleButton CancelButton;
        public DevExpress.XtraEditors.SimpleButton btnPreviousAddress;
        public DevExpress.XtraEditors.SimpleButton btnNextAddress;
        public DevExpress.XtraGrid.GridControl gridControl1;
        public DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        public DevExpress.XtraGrid.Columns.GridColumn CodeCol;
        public DevExpress.XtraGrid.Columns.GridColumn CompanyCol;
        public DevExpress.XtraGrid.Columns.GridColumn ProfessionCol;
        public DevExpress.XtraGrid.Columns.GridColumn TaxCodeCol;
        public DevExpress.XtraGrid.Columns.GridColumn TaxOfficeCol;
        public DevExpress.XtraGrid.Columns.GridColumn CardIDCol;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblFullname;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl lblTaxCodeOffice;
        private DevExpress.XtraEditors.LabelControl lblCardID;
        private DevExpress.XtraEditors.LabelControl lblCompanyName;
        private DevExpress.XtraEditors.LabelControl lblCustomerCode;
        private UserControls.ucTouchFriendlyInput txtName;
        private UserControls.ucTouchFriendlyInput txtSurname;
        private UserControls.ucTouchFriendlyInput txtCardID;
        private UserControls.ucTouchFriendlyInput txtTaxCode;
        private UserControls.ucTouchFriendlyInput txtCompanyName;
        private UserControls.ucTouchFriendlyInput txtCustomerCode;
        private DevExpress.XtraEditors.LookUpEdit txtTaxOffice;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private UserControls.ucTouchFriendlyInput txtAddressProfession;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblCityPhone;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl lblAddress;
        private UserControls.ucTouchFriendlyInput txtAddressCity;
        private UserControls.ucTouchFriendlyInput txtPhone;
        private UserControls.ucTouchFriendlyInput txtAddressStreet;
        private UserControls.ucTouchFriendlyInput txtAddressPostalCode;
        public DevExpress.XtraEditors.SimpleButton btnAddNewCustomerAddress;
        public DevExpress.XtraEditors.SimpleButton btnAddNewCustomer;
        public DevExpress.XtraEditors.SimpleButton btnSearch;
        private LabelControl labelControl6;
        private UserControls.ucTouchFriendlyInput txtThirdPartNum;
    }
}