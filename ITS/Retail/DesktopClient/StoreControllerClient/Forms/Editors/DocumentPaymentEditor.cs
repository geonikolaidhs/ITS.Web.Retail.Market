using DevExpress.Utils.Menu;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.Editors
{
    public class DocumentPaymentEditor : EditFormUserControl
    {
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SpinEdit txtAmount;
        private DevExpress.XtraEditors.LookUpEdit luePaymentMethod;
        private DevExpress.XtraLayout.LayoutControlItem lcPaymentMethod;
        private DevExpress.XtraLayout.LayoutControlItem lcAmount;
        private DevExpress.XtraEditors.DateEdit decimalField1;
        private DevExpress.XtraEditors.DateEdit dateEdit4;
        private DevExpress.XtraEditors.DateEdit dateEdit3;
        private DevExpress.XtraEditors.DateEdit dateEdit2;
        private DevExpress.XtraEditors.DateEdit dateEdit1;

        private DevExpress.XtraEditors.SpinEdit intField5;
        private DevExpress.XtraEditors.SpinEdit intField4;
        private DevExpress.XtraEditors.SpinEdit intField3;
        private DevExpress.XtraEditors.SpinEdit intField2;
        private DevExpress.XtraEditors.SpinEdit intField1;
        private DevExpress.XtraEditors.TextEdit textEdit5;
        private DevExpress.XtraEditors.TextEdit textEdit4;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.SpinEdit decimalField5;
        private DevExpress.XtraEditors.SpinEdit decimalField4;
        private DevExpress.XtraEditors.SpinEdit decimalField3;
        private DevExpress.XtraEditors.SpinEdit decimalField2;
        private DevExpress.XtraEditors.SpinEdit spinEdit2;
        private DevExpress.XtraEditors.LookUpEdit lueCustom5;
        private DevExpress.XtraEditors.LookUpEdit lueCustom4;
        private DevExpress.XtraEditors.LookUpEdit lueCustom3;
        private DevExpress.XtraEditors.LookUpEdit lueCustom2;
        private DevExpress.XtraEditors.LookUpEdit lueCustom1;

        private DevExpress.XtraLayout.LayoutControlItem lcDateField1;
        private DevExpress.XtraLayout.LayoutControlItem lcDateField2;
        private DevExpress.XtraLayout.LayoutControlItem lcDateField3;
        private DevExpress.XtraLayout.LayoutControlItem lcDateField4;
        private DevExpress.XtraLayout.LayoutControlItem lcDateField5;
        private DevExpress.XtraLayout.LayoutControlItem lcDecimalField1;
        private DevExpress.XtraLayout.LayoutControlItem lcDecimalField2;
        private DevExpress.XtraLayout.LayoutControlItem lcDecimalField3;
        private DevExpress.XtraLayout.LayoutControlItem lcDecimalField4;
        private DevExpress.XtraLayout.LayoutControlItem lcDecimalField5;
        private DevExpress.XtraLayout.LayoutControlItem lcStringField1;
        private DevExpress.XtraLayout.LayoutControlItem lcStringField2;
        private DevExpress.XtraLayout.LayoutControlItem lcStringField3;
        private DevExpress.XtraLayout.LayoutControlItem lcStringField4;
        private DevExpress.XtraLayout.LayoutControlItem lcStringField5;
        private DevExpress.XtraLayout.LayoutControlItem lcIntegerField1;
        private DevExpress.XtraLayout.LayoutControlItem lcIntegerField2;
        private DevExpress.XtraLayout.LayoutControlItem lcIntegerField3;
        private DevExpress.XtraLayout.LayoutControlItem lcIntegerFiel4;
        private DevExpress.XtraLayout.LayoutControlItem lcIntegerField5;
        private DevExpress.XtraLayout.LayoutControlItem lcCustomEnumerationValue1;
        private DevExpress.XtraLayout.LayoutControlItem lcCustomEnumerationValue2;
        private DevExpress.XtraLayout.LayoutControlItem lcCustomEnumerationValue3;
        private DevExpress.XtraLayout.LayoutControlItem lcCustomEnumerationValue4;
        private DevExpress.XtraLayout.LayoutControlItem lcCustomEnumerationValue5;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lueCustom5 = new DevExpress.XtraEditors.LookUpEdit();
            this.lueCustom4 = new DevExpress.XtraEditors.LookUpEdit();
            this.lueCustom3 = new DevExpress.XtraEditors.LookUpEdit();
            this.lueCustom2 = new DevExpress.XtraEditors.LookUpEdit();
            this.lueCustom1 = new DevExpress.XtraEditors.LookUpEdit();
            this.intField5 = new DevExpress.XtraEditors.SpinEdit();
            this.intField4 = new DevExpress.XtraEditors.SpinEdit();
            this.intField3 = new DevExpress.XtraEditors.SpinEdit();
            this.intField2 = new DevExpress.XtraEditors.SpinEdit();
            this.intField1 = new DevExpress.XtraEditors.SpinEdit();
            this.textEdit5 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit4 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit3 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.decimalField5 = new DevExpress.XtraEditors.SpinEdit();
            this.decimalField4 = new DevExpress.XtraEditors.SpinEdit();
            this.decimalField3 = new DevExpress.XtraEditors.SpinEdit();
            this.decimalField2 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit2 = new DevExpress.XtraEditors.SpinEdit();
            this.decimalField1 = new DevExpress.XtraEditors.DateEdit();
            this.dateEdit4 = new DevExpress.XtraEditors.DateEdit();
            this.dateEdit3 = new DevExpress.XtraEditors.DateEdit();
            this.dateEdit2 = new DevExpress.XtraEditors.DateEdit();
            this.dateEdit1 = new DevExpress.XtraEditors.DateEdit();
            this.txtAmount = new DevExpress.XtraEditors.SpinEdit();
            this.luePaymentMethod = new DevExpress.XtraEditors.LookUpEdit();
            this.lcCustomEnumerationValue5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStringField2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateField2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateField3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCustomEnumerationValue2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIntegerFiel4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIntegerField3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIntegerField2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIntegerField1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStringField4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStringField3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDecimalField3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDecimalField5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStringField5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateField4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDecimalField2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateField5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDecimalField1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIntegerField5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDecimalField4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcStringField1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateField1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCustomEnumerationValue4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCustomEnumerationValue3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCustomEnumerationValue1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcPaymentMethod = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcAmount = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField1.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit4.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit3.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePaymentMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerFiel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPaymentMethod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.SetBoundPropertyName(this.layoutControl1, "");
            this.layoutControl1.Controls.Add(this.lueCustom5);
            this.layoutControl1.Controls.Add(this.lueCustom4);
            this.layoutControl1.Controls.Add(this.lueCustom3);
            this.layoutControl1.Controls.Add(this.lueCustom2);
            this.layoutControl1.Controls.Add(this.lueCustom1);
            this.layoutControl1.Controls.Add(this.intField5);
            this.layoutControl1.Controls.Add(this.intField4);
            this.layoutControl1.Controls.Add(this.intField3);
            this.layoutControl1.Controls.Add(this.intField2);
            this.layoutControl1.Controls.Add(this.intField1);
            this.layoutControl1.Controls.Add(this.textEdit5);
            this.layoutControl1.Controls.Add(this.textEdit4);
            this.layoutControl1.Controls.Add(this.textEdit3);
            this.layoutControl1.Controls.Add(this.textEdit2);
            this.layoutControl1.Controls.Add(this.textEdit1);
            this.layoutControl1.Controls.Add(this.decimalField5);
            this.layoutControl1.Controls.Add(this.decimalField4);
            this.layoutControl1.Controls.Add(this.decimalField3);
            this.layoutControl1.Controls.Add(this.decimalField2);
            this.layoutControl1.Controls.Add(this.spinEdit2);
            this.layoutControl1.Controls.Add(this.decimalField1);
            this.layoutControl1.Controls.Add(this.dateEdit4);
            this.layoutControl1.Controls.Add(this.dateEdit3);
            this.layoutControl1.Controls.Add(this.dateEdit2);
            this.layoutControl1.Controls.Add(this.dateEdit1);
            this.layoutControl1.Controls.Add(this.txtAmount);
            this.layoutControl1.Controls.Add(this.luePaymentMethod);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(750, 51, 833, 745);
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.layoutControl1.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControl1.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.layoutControl1.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(312, 667);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lueCustom5
            // 
            this.SetBoundFieldName(this.lueCustom5, "CustomEnumerationValue5");
            this.SetBoundPropertyName(this.lueCustom5, "EditValue");
            this.lueCustom5.Location = new System.Drawing.Point(150, 155);
            this.lueCustom5.Name = "lueCustom5";
            this.lueCustom5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustom5.Properties.DisplayMember = "Description";
            this.lueCustom5.Properties.ValueMember = "Oid";
            this.lueCustom5.Size = new System.Drawing.Size(133, 20);
            this.lueCustom5.StyleController = this.layoutControl1;
            this.lueCustom5.TabIndex = 30;
            // 
            // lueCustom4
            // 
            this.SetBoundFieldName(this.lueCustom4, "CustomEnumerationValue4");
            this.SetBoundPropertyName(this.lueCustom4, "EditValue");
            this.lueCustom4.Location = new System.Drawing.Point(150, 131);
            this.lueCustom4.Name = "lueCustom4";
            this.lueCustom4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustom4.Properties.DisplayMember = "Description";
            this.lueCustom4.Properties.ValueMember = "Oid";
            this.lueCustom4.Size = new System.Drawing.Size(133, 20);
            this.lueCustom4.StyleController = this.layoutControl1;
            this.lueCustom4.TabIndex = 29;
            // 
            // lueCustom3
            // 
            this.SetBoundFieldName(this.lueCustom3, "CustomEnumerationValue3");
            this.SetBoundPropertyName(this.lueCustom3, "EditValue");
            this.lueCustom3.Location = new System.Drawing.Point(150, 107);
            this.lueCustom3.Name = "lueCustom3";
            this.lueCustom3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustom3.Properties.DisplayMember = "Description";
            this.lueCustom3.Properties.ValueMember = "Oid";
            this.lueCustom3.Size = new System.Drawing.Size(133, 20);
            this.lueCustom3.StyleController = this.layoutControl1;
            this.lueCustom3.TabIndex = 28;
            // 
            // lueCustom2
            // 
            this.SetBoundFieldName(this.lueCustom2, "CustomEnumerationValue2");
            this.SetBoundPropertyName(this.lueCustom2, "EditValue");
            this.lueCustom2.Location = new System.Drawing.Point(150, 83);
            this.lueCustom2.Name = "lueCustom2";
            this.lueCustom2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustom2.Properties.DisplayMember = "Description";
            this.lueCustom2.Properties.ValueMember = "Oid";
            this.lueCustom2.Size = new System.Drawing.Size(133, 20);
            this.lueCustom2.StyleController = this.layoutControl1;
            this.lueCustom2.TabIndex = 27;
            // 
            // lueCustom1
            // 
            this.SetBoundFieldName(this.lueCustom1, "CustomEnumerationValue1");
            this.SetBoundPropertyName(this.lueCustom1, "EditValue");
            this.lueCustom1.Location = new System.Drawing.Point(150, 59);
            this.lueCustom1.Name = "lueCustom1";
            this.lueCustom1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustom1.Properties.DisplayMember = "Description";
            this.lueCustom1.Properties.ValueMember = "Oid";
            this.lueCustom1.Size = new System.Drawing.Size(133, 20);
            this.lueCustom1.StyleController = this.layoutControl1;
            this.lueCustom1.TabIndex = 26;
            // 
            // intField5
            // 
            this.SetBoundFieldName(this.intField5, "IntegerField5");
            this.SetBoundPropertyName(this.intField5, "EditValue");
            this.intField5.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.intField5.Location = new System.Drawing.Point(150, 515);
            this.intField5.Name = "intField5";
            this.intField5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.intField5.Properties.IsFloatValue = false;
            this.intField5.Properties.Mask.EditMask = "N00";
            this.intField5.Size = new System.Drawing.Size(133, 20);
            this.intField5.StyleController = this.layoutControl1;
            this.intField5.TabIndex = 25;
            // 
            // intField4
            // 
            this.SetBoundFieldName(this.intField4, "IntegerField4");
            this.SetBoundPropertyName(this.intField4, "EditValue");
            this.intField4.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.intField4.Location = new System.Drawing.Point(150, 491);
            this.intField4.Name = "intField4";
            this.intField4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.intField4.Properties.IsFloatValue = false;
            this.intField4.Properties.Mask.EditMask = "N00";
            this.intField4.Size = new System.Drawing.Size(133, 20);
            this.intField4.StyleController = this.layoutControl1;
            this.intField4.TabIndex = 24;
            // 
            // intField3
            // 
            this.SetBoundFieldName(this.intField3, "IntegerField3");
            this.SetBoundPropertyName(this.intField3, "EditValue");
            this.intField3.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.intField3.Location = new System.Drawing.Point(150, 467);
            this.intField3.Name = "intField3";
            this.intField3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.intField3.Properties.IsFloatValue = false;
            this.intField3.Properties.Mask.EditMask = "N00";
            this.intField3.Size = new System.Drawing.Size(133, 20);
            this.intField3.StyleController = this.layoutControl1;
            this.intField3.TabIndex = 23;
            // 
            // intField2
            // 
            this.SetBoundFieldName(this.intField2, "IntegerField2");
            this.SetBoundPropertyName(this.intField2, "EditValue");
            this.intField2.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.intField2.Location = new System.Drawing.Point(150, 443);
            this.intField2.Name = "intField2";
            this.intField2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.intField2.Properties.IsFloatValue = false;
            this.intField2.Properties.Mask.EditMask = "N00";
            this.intField2.Size = new System.Drawing.Size(133, 20);
            this.intField2.StyleController = this.layoutControl1;
            this.intField2.TabIndex = 22;
            // 
            // intField1
            // 
            this.SetBoundFieldName(this.intField1, "IntegerField1");
            this.SetBoundPropertyName(this.intField1, "EditValue");
            this.intField1.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.intField1.Location = new System.Drawing.Point(150, 419);
            this.intField1.Name = "intField1";
            this.intField1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.intField1.Properties.IsFloatValue = false;
            this.intField1.Properties.Mask.EditMask = "N00";
            this.intField1.Size = new System.Drawing.Size(133, 20);
            this.intField1.StyleController = this.layoutControl1;
            this.intField1.TabIndex = 21;
            // 
            // textEdit5
            // 
            this.SetBoundFieldName(this.textEdit5, "StringField5");
            this.SetBoundPropertyName(this.textEdit5, "EditValue");
            this.textEdit5.Location = new System.Drawing.Point(150, 635);
            this.textEdit5.Name = "textEdit5";
            this.textEdit5.Size = new System.Drawing.Size(133, 20);
            this.textEdit5.StyleController = this.layoutControl1;
            this.textEdit5.TabIndex = 20;
            // 
            // textEdit4
            // 
            this.SetBoundFieldName(this.textEdit4, "StringField4");
            this.SetBoundPropertyName(this.textEdit4, "EditValue");
            this.textEdit4.Location = new System.Drawing.Point(150, 611);
            this.textEdit4.Name = "textEdit4";
            this.textEdit4.Size = new System.Drawing.Size(133, 20);
            this.textEdit4.StyleController = this.layoutControl1;
            this.textEdit4.TabIndex = 19;
            // 
            // textEdit3
            // 
            this.SetBoundFieldName(this.textEdit3, "StringField3");
            this.SetBoundPropertyName(this.textEdit3, "EditValue");
            this.textEdit3.Location = new System.Drawing.Point(150, 587);
            this.textEdit3.Name = "textEdit3";
            this.textEdit3.Size = new System.Drawing.Size(133, 20);
            this.textEdit3.StyleController = this.layoutControl1;
            this.textEdit3.TabIndex = 18;
            // 
            // textEdit2
            // 
            this.SetBoundFieldName(this.textEdit2, "StringField2");
            this.SetBoundPropertyName(this.textEdit2, "EditValue");
            this.textEdit2.Location = new System.Drawing.Point(150, 563);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Size = new System.Drawing.Size(133, 20);
            this.textEdit2.StyleController = this.layoutControl1;
            this.textEdit2.TabIndex = 17;
            // 
            // textEdit1
            // 
            this.SetBoundFieldName(this.textEdit1, "StringField1");
            this.SetBoundPropertyName(this.textEdit1, "EditValue");
            this.textEdit1.Location = new System.Drawing.Point(150, 539);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(133, 20);
            this.textEdit1.StyleController = this.layoutControl1;
            this.textEdit1.TabIndex = 16;
            // 
            // decimalField5
            // 
            this.SetBoundFieldName(this.decimalField5, "DecimalField5");
            this.SetBoundPropertyName(this.decimalField5, "EditValue");
            this.decimalField5.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalField5.Location = new System.Drawing.Point(150, 395);
            this.decimalField5.Name = "decimalField5";
            this.decimalField5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField5.Size = new System.Drawing.Size(133, 20);
            this.decimalField5.StyleController = this.layoutControl1;
            this.decimalField5.TabIndex = 15;
            // 
            // decimalField4
            // 
            this.SetBoundFieldName(this.decimalField4, "DecimalField4");
            this.SetBoundPropertyName(this.decimalField4, "EditValue");
            this.decimalField4.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalField4.Location = new System.Drawing.Point(150, 371);
            this.decimalField4.Name = "decimalField4";
            this.decimalField4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField4.Size = new System.Drawing.Size(133, 20);
            this.decimalField4.StyleController = this.layoutControl1;
            this.decimalField4.TabIndex = 14;
            // 
            // decimalField3
            // 
            this.SetBoundFieldName(this.decimalField3, "DecimalField3");
            this.SetBoundPropertyName(this.decimalField3, "EditValue");
            this.decimalField3.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalField3.Location = new System.Drawing.Point(150, 347);
            this.decimalField3.Name = "decimalField3";
            this.decimalField3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField3.Size = new System.Drawing.Size(133, 20);
            this.decimalField3.StyleController = this.layoutControl1;
            this.decimalField3.TabIndex = 13;
            // 
            // decimalField2
            // 
            this.SetBoundFieldName(this.decimalField2, "DecimalField2");
            this.SetBoundPropertyName(this.decimalField2, "EditValue");
            this.decimalField2.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalField2.Location = new System.Drawing.Point(150, 323);
            this.decimalField2.Name = "decimalField2";
            this.decimalField2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField2.Size = new System.Drawing.Size(133, 20);
            this.decimalField2.StyleController = this.layoutControl1;
            this.decimalField2.TabIndex = 12;
            // 
            // spinEdit2
            // 
            this.SetBoundFieldName(this.spinEdit2, "DecimalField1");
            this.SetBoundPropertyName(this.spinEdit2, "EditValue");
            this.spinEdit2.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit2.Location = new System.Drawing.Point(150, 299);
            this.spinEdit2.Name = "spinEdit2";
            this.spinEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinEdit2.Size = new System.Drawing.Size(133, 20);
            this.spinEdit2.StyleController = this.layoutControl1;
            this.spinEdit2.TabIndex = 11;
            // 
            // decimalField1
            // 
            this.SetBoundFieldName(this.decimalField1, "DateField5");
            this.SetBoundPropertyName(this.decimalField1, "EditValue");
            this.decimalField1.EditValue = null;
            this.decimalField1.Location = new System.Drawing.Point(150, 275);
            this.decimalField1.Name = "decimalField1";
            this.decimalField1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField1.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.decimalField1.Size = new System.Drawing.Size(133, 20);
            this.decimalField1.StyleController = this.layoutControl1;
            this.decimalField1.TabIndex = 10;
            // 
            // dateEdit4
            // 
            this.SetBoundFieldName(this.dateEdit4, "DateField4");
            this.SetBoundPropertyName(this.dateEdit4, "EditValue");
            this.dateEdit4.EditValue = null;
            this.dateEdit4.Location = new System.Drawing.Point(150, 251);
            this.dateEdit4.Name = "dateEdit4";
            this.dateEdit4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit4.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit4.Size = new System.Drawing.Size(133, 20);
            this.dateEdit4.StyleController = this.layoutControl1;
            this.dateEdit4.TabIndex = 9;
            // 
            // dateEdit3
            // 
            this.SetBoundFieldName(this.dateEdit3, "DateField3");
            this.SetBoundPropertyName(this.dateEdit3, "EditValue");
            this.dateEdit3.EditValue = null;
            this.dateEdit3.Location = new System.Drawing.Point(150, 227);
            this.dateEdit3.Name = "dateEdit3";
            this.dateEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit3.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit3.Size = new System.Drawing.Size(133, 20);
            this.dateEdit3.StyleController = this.layoutControl1;
            this.dateEdit3.TabIndex = 8;
            // 
            // dateEdit2
            // 
            this.SetBoundFieldName(this.dateEdit2, "DateField2");
            this.SetBoundPropertyName(this.dateEdit2, "EditValue");
            this.dateEdit2.EditValue = null;
            this.dateEdit2.Location = new System.Drawing.Point(150, 203);
            this.dateEdit2.Name = "dateEdit2";
            this.dateEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit2.Size = new System.Drawing.Size(133, 20);
            this.dateEdit2.StyleController = this.layoutControl1;
            this.dateEdit2.TabIndex = 7;
            // 
            // dateEdit1
            // 
            this.SetBoundFieldName(this.dateEdit1, "DateField1");
            this.SetBoundPropertyName(this.dateEdit1, "EditValue");
            this.dateEdit1.EditValue = null;
            this.dateEdit1.Location = new System.Drawing.Point(150, 179);
            this.dateEdit1.Name = "dateEdit1";
            this.dateEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit1.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit1.Size = new System.Drawing.Size(133, 20);
            this.dateEdit1.StyleController = this.layoutControl1;
            this.dateEdit1.TabIndex = 6;
            // 
            // txtAmount
            // 
            this.SetBoundFieldName(this.txtAmount, "Amount");
            this.SetBoundPropertyName(this.txtAmount, "EditValue");
            this.txtAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtAmount.Location = new System.Drawing.Point(150, 35);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAmount.Size = new System.Drawing.Size(133, 20);
            this.txtAmount.StyleController = this.layoutControl1;
            this.txtAmount.TabIndex = 5;
            // 
            // luePaymentMethod
            // 
            this.SetBoundFieldName(this.luePaymentMethod, "PaymentMethod!Key");
            this.SetBoundPropertyName(this.luePaymentMethod, "EditValue");
            this.luePaymentMethod.Location = new System.Drawing.Point(150, 11);
            this.luePaymentMethod.Name = "luePaymentMethod";
            this.luePaymentMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.luePaymentMethod.Properties.ValueMember = "Oid";
            this.luePaymentMethod.Size = new System.Drawing.Size(133, 20);
            this.luePaymentMethod.StyleController = this.layoutControl1;
            this.luePaymentMethod.TabIndex = 4;
            this.luePaymentMethod.EditValueChanged += new System.EventHandler(this.luePaymentMethod_EditValueChanged);
            // 
            // lcCustomEnumerationValue5
            // 
            this.lcCustomEnumerationValue5.Control = this.lueCustom5;
            this.lcCustomEnumerationValue5.Location = new System.Drawing.Point(0, 144);
            this.lcCustomEnumerationValue5.Name = "lcCustomEnumerationValue5";
            this.lcCustomEnumerationValue5.Size = new System.Drawing.Size(275, 24);
            this.lcCustomEnumerationValue5.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcStringField2
            // 
            this.lcStringField2.Control = this.textEdit2;
            this.lcStringField2.Location = new System.Drawing.Point(0, 552);
            this.lcStringField2.Name = "lcStringField2";
            this.lcStringField2.Size = new System.Drawing.Size(275, 24);
            this.lcStringField2.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDateField2
            // 
            this.lcDateField2.Control = this.dateEdit2;
            this.lcDateField2.Location = new System.Drawing.Point(0, 192);
            this.lcDateField2.Name = "lcDateField2";
            this.lcDateField2.Size = new System.Drawing.Size(275, 24);
            this.lcDateField2.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDateField3
            // 
            this.lcDateField3.Control = this.dateEdit3;
            this.lcDateField3.Location = new System.Drawing.Point(0, 216);
            this.lcDateField3.Name = "lcDateField3";
            this.lcDateField3.Size = new System.Drawing.Size(275, 24);
            this.lcDateField3.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcCustomEnumerationValue2
            // 
            this.lcCustomEnumerationValue2.Control = this.lueCustom2;
            this.lcCustomEnumerationValue2.Location = new System.Drawing.Point(0, 72);
            this.lcCustomEnumerationValue2.Name = "lcCustomEnumerationValue2";
            this.lcCustomEnumerationValue2.Size = new System.Drawing.Size(275, 24);
            this.lcCustomEnumerationValue2.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcIntegerFiel4
            // 
            this.lcIntegerFiel4.Control = this.intField4;
            this.lcIntegerFiel4.Location = new System.Drawing.Point(0, 480);
            this.lcIntegerFiel4.Name = "lcIntegerFiel4";
            this.lcIntegerFiel4.Size = new System.Drawing.Size(275, 24);
            this.lcIntegerFiel4.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcIntegerField3
            // 
            this.lcIntegerField3.Control = this.intField3;
            this.lcIntegerField3.Location = new System.Drawing.Point(0, 456);
            this.lcIntegerField3.Name = "lcIntegerField3";
            this.lcIntegerField3.Size = new System.Drawing.Size(275, 24);
            this.lcIntegerField3.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcIntegerField2
            // 
            this.lcIntegerField2.Control = this.intField2;
            this.lcIntegerField2.Location = new System.Drawing.Point(0, 432);
            this.lcIntegerField2.Name = "lcIntegerField2";
            this.lcIntegerField2.Size = new System.Drawing.Size(275, 24);
            this.lcIntegerField2.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcIntegerField1
            // 
            this.lcIntegerField1.Control = this.intField1;
            this.lcIntegerField1.Location = new System.Drawing.Point(0, 408);
            this.lcIntegerField1.Name = "lcIntegerField1";
            this.lcIntegerField1.Size = new System.Drawing.Size(275, 24);
            this.lcIntegerField1.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcStringField4
            // 
            this.lcStringField4.Control = this.textEdit4;
            this.lcStringField4.Location = new System.Drawing.Point(0, 600);
            this.lcStringField4.Name = "lcStringField4";
            this.lcStringField4.Size = new System.Drawing.Size(275, 24);
            this.lcStringField4.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcStringField3
            // 
            this.lcStringField3.Control = this.textEdit3;
            this.lcStringField3.Location = new System.Drawing.Point(0, 576);
            this.lcStringField3.Name = "lcStringField3";
            this.lcStringField3.Size = new System.Drawing.Size(275, 24);
            this.lcStringField3.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDecimalField3
            // 
            this.lcDecimalField3.Control = this.decimalField3;
            this.lcDecimalField3.Location = new System.Drawing.Point(0, 336);
            this.lcDecimalField3.Name = "lcDecimalField3";
            this.lcDecimalField3.Size = new System.Drawing.Size(275, 24);
            this.lcDecimalField3.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDecimalField5
            // 
            this.lcDecimalField5.Control = this.decimalField5;
            this.lcDecimalField5.Location = new System.Drawing.Point(0, 384);
            this.lcDecimalField5.Name = "lcDecimalField5";
            this.lcDecimalField5.Size = new System.Drawing.Size(275, 24);
            this.lcDecimalField5.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcStringField5
            // 
            this.lcStringField5.Control = this.textEdit5;
            this.lcStringField5.Location = new System.Drawing.Point(0, 624);
            this.lcStringField5.Name = "lcStringField5";
            this.lcStringField5.Size = new System.Drawing.Size(275, 24);
            this.lcStringField5.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDateField4
            // 
            this.lcDateField4.Control = this.dateEdit4;
            this.lcDateField4.Location = new System.Drawing.Point(0, 240);
            this.lcDateField4.Name = "lcDateField4";
            this.lcDateField4.Size = new System.Drawing.Size(275, 24);
            this.lcDateField4.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDecimalField2
            // 
            this.lcDecimalField2.Control = this.decimalField2;
            this.lcDecimalField2.Location = new System.Drawing.Point(0, 312);
            this.lcDecimalField2.Name = "lcDecimalField2";
            this.lcDecimalField2.Size = new System.Drawing.Size(275, 24);
            this.lcDecimalField2.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDateField5
            // 
            this.lcDateField5.Control = this.decimalField1;
            this.lcDateField5.Location = new System.Drawing.Point(0, 264);
            this.lcDateField5.Name = "lcDateField5";
            this.lcDateField5.Size = new System.Drawing.Size(275, 24);
            this.lcDateField5.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDecimalField1
            // 
            this.lcDecimalField1.Control = this.spinEdit2;
            this.lcDecimalField1.Location = new System.Drawing.Point(0, 288);
            this.lcDecimalField1.Name = "lcDecimalField1";
            this.lcDecimalField1.Size = new System.Drawing.Size(275, 24);
            this.lcDecimalField1.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcIntegerField5
            // 
            this.lcIntegerField5.Control = this.intField5;
            this.lcIntegerField5.Location = new System.Drawing.Point(0, 504);
            this.lcIntegerField5.Name = "lcIntegerField5";
            this.lcIntegerField5.Size = new System.Drawing.Size(275, 24);
            this.lcIntegerField5.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDecimalField4
            // 
            this.lcDecimalField4.Control = this.decimalField4;
            this.lcDecimalField4.Location = new System.Drawing.Point(0, 360);
            this.lcDecimalField4.Name = "lcDecimalField4";
            this.lcDecimalField4.Size = new System.Drawing.Size(275, 24);
            this.lcDecimalField4.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcStringField1
            // 
            this.lcStringField1.Control = this.textEdit1;
            this.lcStringField1.Location = new System.Drawing.Point(0, 528);
            this.lcStringField1.Name = "lcStringField1";
            this.lcStringField1.Size = new System.Drawing.Size(275, 24);
            this.lcStringField1.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcDateField1
            // 
            this.lcDateField1.Control = this.dateEdit1;
            this.lcDateField1.Location = new System.Drawing.Point(0, 168);
            this.lcDateField1.Name = "lcDateField1";
            this.lcDateField1.Size = new System.Drawing.Size(275, 24);
            this.lcDateField1.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcCustomEnumerationValue4
            // 
            this.lcCustomEnumerationValue4.Control = this.lueCustom4;
            this.lcCustomEnumerationValue4.Location = new System.Drawing.Point(0, 120);
            this.lcCustomEnumerationValue4.Name = "lcCustomEnumerationValue4";
            this.lcCustomEnumerationValue4.Size = new System.Drawing.Size(275, 24);
            this.lcCustomEnumerationValue4.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcCustomEnumerationValue3
            // 
            this.lcCustomEnumerationValue3.Control = this.lueCustom3;
            this.lcCustomEnumerationValue3.Location = new System.Drawing.Point(0, 96);
            this.lcCustomEnumerationValue3.Name = "lcCustomEnumerationValue3";
            this.lcCustomEnumerationValue3.Size = new System.Drawing.Size(275, 24);
            this.lcCustomEnumerationValue3.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcCustomEnumerationValue1
            // 
            this.lcCustomEnumerationValue1.Control = this.lueCustom1;
            this.lcCustomEnumerationValue1.Location = new System.Drawing.Point(0, 48);
            this.lcCustomEnumerationValue1.Name = "lcCustomEnumerationValue1";
            this.lcCustomEnumerationValue1.Size = new System.Drawing.Size(275, 24);
            this.lcCustomEnumerationValue1.TextSize = new System.Drawing.Size(135, 13);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcPaymentMethod,
            this.lcAmount,
            this.lcCustomEnumerationValue2,
            this.lcCustomEnumerationValue1,
            this.lcCustomEnumerationValue3,
            this.lcCustomEnumerationValue4,
            this.lcCustomEnumerationValue5,
            this.lcDateField1,
            this.lcDateField2,
            this.lcDateField3,
            this.lcDateField4,
            this.lcDateField5,
            this.lcDecimalField1,
            this.lcDecimalField2,
            this.lcDecimalField3,
            this.lcDecimalField4,
            this.lcDecimalField5,
            this.lcIntegerField1,
            this.lcIntegerField2,
            this.lcIntegerField3,
            this.lcIntegerFiel4,
            this.lcIntegerField5,
            this.lcStringField1,
            this.lcStringField2,
            this.lcStringField3,
            this.lcStringField4,
            this.lcStringField5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, -1);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(295, 668);
            this.layoutControlGroup1.TextVisible = false;
            this.layoutControlGroup1.Shown += new System.EventHandler(this.layoutControlGroup1_Shown);
            // 
            // lcPaymentMethod
            // 
            this.lcPaymentMethod.Control = this.luePaymentMethod;
            this.lcPaymentMethod.Location = new System.Drawing.Point(0, 0);
            this.lcPaymentMethod.Name = "lcPaymentMethod";
            this.lcPaymentMethod.Size = new System.Drawing.Size(275, 24);
            this.lcPaymentMethod.Text = "@@PaymentMethod";
            this.lcPaymentMethod.TextSize = new System.Drawing.Size(135, 13);
            // 
            // lcAmount
            // 
            this.lcAmount.Control = this.txtAmount;
            this.lcAmount.Location = new System.Drawing.Point(0, 24);
            this.lcAmount.Name = "lcAmount";
            this.lcAmount.Size = new System.Drawing.Size(275, 24);
            this.lcAmount.Text = "@@Amount";
            this.lcAmount.TextSize = new System.Drawing.Size(135, 13);
            // 
            // DocumentPaymentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.layoutControl1);
            this.Name = "DocumentPaymentEditor";
            this.Size = new System.Drawing.Size(312, 667);
            this.VisibleChanged += new System.EventHandler(this.DocumentPaymentEditor_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustom1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intField1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField1.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decimalField1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit4.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit3.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luePaymentMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerFiel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIntegerField5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDecimalField4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcStringField1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateField1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCustomEnumerationValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPaymentMethod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAmount)).EndInit();
            this.ResumeLayout(false);

        }

        

        protected override void SetMenuManager(IDXMenuManager menuManager)
        {
            base.SetMenuManager(menuManager);
            foreach (Control ctrl in layoutControl1.Controls)
            {
                BaseEdit edit = ctrl as BaseEdit;
                if (edit != null)
                {
                    edit.MenuManager = menuManager;
                }
            }
            lcPaymentMethod.Text = Resources.PaymentMethod;
            lcAmount.Text = Resources.Amount;
        }

        protected UnitOfWork Uow { get; set; }
        Dictionary<string, LayoutControlItem> lcs;
        public DocumentPaymentEditor(UnitOfWork uow)
        {
            InitializeComponent();
            this.Uow = uow;
            luePaymentMethod.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            luePaymentMethod.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            luePaymentMethod.Properties.DataSource = new XPCollection<PaymentMethod>(uow);
            lcs = new Dictionary<string, LayoutControlItem>(){
                {"DateField1",lcDateField1}, {"DateField2",lcDateField2}, {"DateField3",lcDateField3}, {"DateField4",lcDateField4}, {"DateField5",lcDateField5}, {"DecimalField1",lcDecimalField1}, {"DecimalField2",lcDecimalField2}, {"DecimalField3",lcDecimalField3},
                {"DecimalField4",lcDecimalField4}, {"DecimalField5",lcDecimalField5}, {"StringField1",lcStringField1}, {"StringField2",lcStringField2},
                {"StringField3",lcStringField3}, {"StringField4",lcStringField4}, {"StringField5",lcStringField5}, {"IntegerField1",lcIntegerField1},
                {"IntegerField2",lcIntegerField2}, {"IntegerField3",lcIntegerField3}, {"IntegerField4",lcIntegerFiel4}, {"IntegerField5",lcIntegerField5},
                {"CustomEnumerationValue1",lcCustomEnumerationValue1}, {"CustomEnumerationValue2",lcCustomEnumerationValue2}, {"CustomEnumerationValue3",lcCustomEnumerationValue3},                
                {"CustomEnumerationValue4",lcCustomEnumerationValue4}, {"CustomEnumerationValue5",lcCustomEnumerationValue5}
            };

        }

        private void luePaymentMethod_EditValueChanged(object sender, EventArgs e)
        {
            Guid methodGuid = luePaymentMethod.EditValue is Guid ? (Guid)luePaymentMethod.EditValue : Guid.Empty;
            IEnumerable<PaymentMethod> methods = luePaymentMethod.Properties.DataSource as IEnumerable<PaymentMethod>;
            PaymentMethod method = methods == null ? null : methods.FirstOrDefault(x=>x.Oid == methodGuid);
            //PaymentMethod method = luePaymentMethod.EditValue as PaymentMethod;
            ArrangeLayout(method);
        }

        private void ArrangeLayout(PaymentMethod method)
        {
            
            lcs.ToList().ForEach(x => x.Value.HideToCustomization());
            if (method != null)
            {
                method.PaymentMethodFields.ToList().ForEach(x =>
                {
                    if (lcs.ContainsKey(x.FieldName))
                    {
                        lcs[x.FieldName].RestoreFromCustomization();
                        this.SetBoundFieldName(lcs[x.FieldName].Control, x.FieldName);
                        lcs[x.FieldName].Text = x.Label;
                        if (x.CustomEnumeration != null && lcs[x.FieldName].Control is LookUpEditBase)
                        {
                            ((LookUpEditBase)lcs[x.FieldName].Control).Properties.DataSource = x.CustomEnumeration.CustomEnumerationValues.OrderBy(value => value.Ordering);
                            ((LookUpEditBase)lcs[x.FieldName].Control).Properties.ValueMember = "Oid";
                            ((LookUpEditBase)lcs[x.FieldName].Control).Properties.DisplayMember = "Description";
                        }
                    }
                });
            }
        }

        private void layoutControlGroup1_Shown(object sender, EventArgs e)
        {

        }

        private void DocumentPaymentEditor_VisibleChanged(object sender, EventArgs e)
        {
            luePaymentMethod_EditValueChanged(sender,e);
        }

    }
}
