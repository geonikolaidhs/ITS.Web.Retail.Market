using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class PurchaseDocumentFilter : DocumentFilter
    {
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;

        public PurchaseDocumentFilter() : base(Platform.Enumerations.eDivision.Purchase, false)
        {
            layoutControlItemDocumentCustomerFilter.HideToCustomization();
            SecondaryFilterControl = new PurchaseDocumentSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;
        }

        public override int Lines
        {
            get
            {
                return 4;
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSeries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDocumentNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDocumentNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentSeries.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueTransformationStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeEditFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateToDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFiscalFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalToDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFiscalToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedByDevice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCreatedBy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCreatedByDevice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromDocumentNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToDocumentNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentCustomerFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpCustomerView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpSupplierView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentCustomerFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSupplierFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // lueDocumentType
            // 
            // 
            // lueDocumentSeries
            // 
            // 
            // lueTransformationStatus
            // 
            // 
            // dateFromDate
            // 
            this.dateFromDate.EditValue = null;
            this.dateFromDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "d";
            this.dateFromDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateFromDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "d";
            this.dateFromDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // timeEditFrom
            // 
            this.timeEditFrom.EditValue = null;
            this.timeEditFrom.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.EditFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.Mask.EditMask = "";
            this.timeEditFrom.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            // 
            // dateToDate
            // 
            this.dateToDate.EditValue = null;
            this.dateToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // timeToTime
            // 
            this.timeToTime.EditValue = null;
            this.timeToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.Mask.EditMask = "";
            this.timeToTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            // 
            // dateFiscalFromDate
            // 
            this.dateFiscalFromDate.EditValue = null;
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // timeFiscalFromTime
            // 
            this.timeFiscalFromTime.EditValue = null;
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.Mask.EditMask = "";
            this.timeFiscalFromTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // dateFiscalToDate
            // 
            this.dateFiscalToDate.EditValue = null;
            this.dateFiscalToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.Mask.EditMask = "";
            this.dateFiscalToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // timeFiscalToTime
            // 
            this.timeFiscalToTime.EditValue = null;
            this.timeFiscalToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.Mask.EditMask = "";
            this.timeFiscalToTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            // 
            // lueDocumentStatus
            // 
            // 
            // lueCreatedBy
            // 
            // 
            // lueCreatedByDevice
            // 
            // 
            // txtFromDocumentNumber
            // 
            // 
            // txtToDocumentNumber
            // 
            // 
            // searchLookUpEditDocumentCustomerFilter
            // 
            // 
            // searchLookUpCustomerView
            // 
            this.searchLookUpCustomerView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpCustomerView.OptionsView.ShowGroupPanel = false;
            // 
            // searchLookUpEditDocumentSupplierFilter
            // 
            // 
            // searchLookUpSupplierView
            // 
            this.searchLookUpSupplierView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpSupplierView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnDescription
            // 
            this.gridColumnDescription.OptionsColumn.AllowEdit = false;
            // 
            // gridColumnCode
            // 
            this.gridColumnCode.OptionsColumn.AllowEdit = false;
            // 
            // gridColumnTaxCode
            // 
            this.gridColumnTaxCode.OptionsColumn.AllowEdit = false;
            // 
            // gridColumnSupplierDescription
            // 
            this.gridColumnSupplierDescription.OptionsColumn.AllowEdit = false;
            // 
            // gridColumnSupplierCode
            // 
            this.gridColumnSupplierCode.OptionsColumn.AllowEdit = false;
            // 
            // gridColumnSupplierTaxCode
            // 
            this.gridColumnSupplierTaxCode.OptionsColumn.AllowEdit = false;
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(634, 433, 738, 619);
            this.layoutControlFilter.Controls.SetChildIndex(this.searchLookUpEditDocumentCustomerFilter, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.searchLookUpEditDocumentSupplierFilter, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.txtToDocumentNumber, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.txtFromDocumentNumber, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueCreatedByDevice, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueCreatedBy, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueDocumentStatus, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.timeFiscalToTime, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.dateFiscalToDate, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.timeFiscalFromTime, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.dateFiscalFromDate, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.timeToTime, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.dateToDate, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.timeEditFrom, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.dateFromDate, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueTransformationStatus, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueDocumentSeries, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.lueDocumentType, 0);
            // 
            // PurchaseDocumentFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "PurchaseDocumentFilter";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSeries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDocumentNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDocumentNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentSeries.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueTransformationStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeEditFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateToDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFiscalToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFiscalFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateFiscalToDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFiscalToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueDocumentStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedByDevice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCreatedBy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCreatedByDevice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromDocumentNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToDocumentNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentCustomerFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpCustomerView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpSupplierView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentCustomerFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSupplierFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
