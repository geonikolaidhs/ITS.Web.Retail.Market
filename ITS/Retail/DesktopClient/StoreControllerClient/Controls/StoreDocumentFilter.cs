using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class StoreDocumentFilter : DocumentFilter
    {
        public StoreDocumentFilter():base(Platform.Enumerations.eDivision.Store, false)
        {
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "ITS.Retail.DesktopClient.StoreControllerClient.Layouts.store_document_filter_layout.xml";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                this.layoutControlFilter.RestoreLayoutFromStream(stream);
            }
            SecondaryFilterControl = new StoreDocumentSecondaryFilterControl();
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
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentCustomerFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSupplierFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlItemDocumentType
            // 
            this.layoutControlItemDocumentType.Size = new System.Drawing.Size(217, 40);
            // 
            // layoutControlItemDocumentSeries
            // 
            this.layoutControlItemDocumentSeries.Location = new System.Drawing.Point(217, 0);
            this.layoutControlItemDocumentSeries.Size = new System.Drawing.Size(175, 40);
            // 
            // layoutControlItemTransformationStatus
            // 
            this.layoutControlItemTransformationStatus.Location = new System.Drawing.Point(0, 80);
            this.layoutControlItemTransformationStatus.Size = new System.Drawing.Size(217, 120);
            // 
            // layoutControlItemFiscalFrom
            // 
            this.layoutControlItemFiscalFrom.Location = new System.Drawing.Point(392, 80);
            this.layoutControlItemFiscalFrom.Size = new System.Drawing.Size(176, 40);
            // 
            // layoutControlItemFromDocumentNumber
            // 
            this.layoutControlItemFromDocumentNumber.Location = new System.Drawing.Point(392, 0);
            this.layoutControlItemFromDocumentNumber.Size = new System.Drawing.Size(176, 40);
            // 
            // layoutControlItemFromDate
            // 
            this.layoutControlItemFromDate.Size = new System.Drawing.Size(217, 40);
            // 
            // layoutControlItemToDocumentNumber
            // 
            this.layoutControlItemToDocumentNumber.Location = new System.Drawing.Point(568, 0);
            this.layoutControlItemToDocumentNumber.Size = new System.Drawing.Size(236, 40);
            // 
            // layoutControlItemToDate
            // 
            this.layoutControlItemToDate.Location = new System.Drawing.Point(392, 40);
            this.layoutControlItemToDate.Size = new System.Drawing.Size(176, 40);
            // 
            // layoutControlItemFromTime
            // 
            this.layoutControlItemFromTime.Location = new System.Drawing.Point(217, 40);
            this.layoutControlItemFromTime.Size = new System.Drawing.Size(175, 40);
            // 
            // layoutControlItemToTime
            // 
            this.layoutControlItemToTime.Location = new System.Drawing.Point(568, 40);
            this.layoutControlItemToTime.Size = new System.Drawing.Size(236, 40);
            // 
            // layoutControlItemFiscalTo
            // 
            this.layoutControlItemFiscalTo.Location = new System.Drawing.Point(568, 80);
            this.layoutControlItemFiscalTo.Size = new System.Drawing.Size(236, 40);
            // 
            // layoutControlItemStatus
            // 
            this.layoutControlItemStatus.Location = new System.Drawing.Point(217, 160);
            this.layoutControlItemStatus.Size = new System.Drawing.Size(175, 40);
            // 
            // lueDocumentType
            // 
            this.lueDocumentType.Size = new System.Drawing.Size(213, 20);
            // 
            // lueDocumentSeries
            // 
            this.lueDocumentSeries.Location = new System.Drawing.Point(229, 28);
            this.lueDocumentSeries.Size = new System.Drawing.Size(171, 20);
            // 
            // lueTransformationStatus
            // 
            this.lueTransformationStatus.Location = new System.Drawing.Point(12, 108);
            this.lueTransformationStatus.Size = new System.Drawing.Size(213, 20);
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
            this.dateFromDate.Size = new System.Drawing.Size(213, 20);
            // 
            // timeEditFrom
            // 
            this.timeEditFrom.EditValue = null;
            this.timeEditFrom.Location = new System.Drawing.Point(229, 68);
            this.timeEditFrom.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.EditFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.Mask.EditMask = "90:00";
            this.timeEditFrom.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            this.timeEditFrom.Size = new System.Drawing.Size(171, 20);
            // 
            // dateToDate
            // 
            this.dateToDate.EditValue = null;
            this.dateToDate.Location = new System.Drawing.Point(404, 68);
            this.dateToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateToDate.Size = new System.Drawing.Size(172, 20);
            // 
            // timeToTime
            // 
            this.timeToTime.EditValue = null;
            this.timeToTime.Location = new System.Drawing.Point(580, 68);
            this.timeToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.Mask.EditMask = "90:00";
            this.timeToTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            this.timeToTime.Size = new System.Drawing.Size(232, 20);
            // 
            // layoutControlItemCreatedBy
            // 
            this.layoutControlItemCreatedBy.Location = new System.Drawing.Point(0, 200);
            this.layoutControlItemCreatedBy.Size = new System.Drawing.Size(804, 40);
            // 
            // layoutControlItemFiscalFromTime
            // 
            this.layoutControlItemFiscalFromTime.Location = new System.Drawing.Point(217, 120);
            this.layoutControlItemFiscalFromTime.Size = new System.Drawing.Size(587, 40);
            // 
            // layoutControlItemFiscalToTime
            // 
            this.layoutControlItemFiscalToTime.Location = new System.Drawing.Point(217, 80);
            this.layoutControlItemFiscalToTime.Size = new System.Drawing.Size(175, 40);
            // 
            // dateFiscalFromDate
            // 
            this.dateFiscalFromDate.EditValue = null;
            this.dateFiscalFromDate.Location = new System.Drawing.Point(404, 108);
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateFiscalFromDate.Size = new System.Drawing.Size(172, 20);
            // 
            // timeFiscalFromTime
            // 
            this.timeFiscalFromTime.EditValue = null;
            this.timeFiscalFromTime.Location = new System.Drawing.Point(229, 148);
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.Mask.EditMask = "";
            this.timeFiscalFromTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.timeFiscalFromTime.Size = new System.Drawing.Size(583, 20);
            // 
            // dateFiscalToDate
            // 
            this.dateFiscalToDate.EditValue = null;
            this.dateFiscalToDate.Location = new System.Drawing.Point(580, 108);
            this.dateFiscalToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.Mask.EditMask = "";
            this.dateFiscalToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateFiscalToDate.Size = new System.Drawing.Size(232, 20);
            // 
            // timeFiscalToTime
            // 
            this.timeFiscalToTime.EditValue = null;
            this.timeFiscalToTime.Location = new System.Drawing.Point(229, 108);
            this.timeFiscalToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.Mask.EditMask = "";
            this.timeFiscalToTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.timeFiscalToTime.Size = new System.Drawing.Size(171, 20);
            // 
            // lueDocumentStatus
            // 
            this.lueDocumentStatus.Location = new System.Drawing.Point(229, 188);
            this.lueDocumentStatus.Size = new System.Drawing.Size(171, 20);
            // 
            // layoutControlItemCreatedByDevice
            // 
            this.layoutControlItemCreatedByDevice.Location = new System.Drawing.Point(392, 160);
            this.layoutControlItemCreatedByDevice.Size = new System.Drawing.Size(412, 40);
            // 
            // lueCreatedBy
            // 
            this.lueCreatedBy.Location = new System.Drawing.Point(12, 228);
            this.lueCreatedBy.Size = new System.Drawing.Size(800, 20);
            // 
            // lueCreatedByDevice
            // 
            this.lueCreatedByDevice.Location = new System.Drawing.Point(404, 188);
            this.lueCreatedByDevice.Size = new System.Drawing.Size(408, 20);
            // 
            // txtFromDocumentNumber
            // 
            this.txtFromDocumentNumber.Location = new System.Drawing.Point(404, 28);
            this.txtFromDocumentNumber.Size = new System.Drawing.Size(172, 20);
            // 
            // txtToDocumentNumber
            // 
            this.txtToDocumentNumber.Location = new System.Drawing.Point(580, 28);
            this.txtToDocumentNumber.Size = new System.Drawing.Size(232, 20);
            // 
            // searchLookUpEditDocumentCustomerFilter
            // 
            this.searchLookUpEditDocumentCustomerFilter.Location = new System.Drawing.Point(12, 268);
            // 
            // searchLookUpEditDocumentSupplierFilter
            // 
            this.searchLookUpEditDocumentSupplierFilter.Location = new System.Drawing.Point(437, 268);
            // 
            // layoutControlItemDocumentCustomerFilter
            // 
            this.layoutControlItemDocumentCustomerFilter.Location = new System.Drawing.Point(0, 240);
            this.layoutControlItemDocumentCustomerFilter.Size = new System.Drawing.Size(425, 81);
            // 
            // layoutControlItemDocumentSupplierFilter
            // 
            this.layoutControlItemDocumentSupplierFilter.Location = new System.Drawing.Point(425, 240);
            this.layoutControlItemDocumentSupplierFilter.Size = new System.Drawing.Size(379, 81);
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemCreatedByDevice});
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(979, 318, 738, 619);
            this.layoutControlFilter.Controls.SetChildIndex(this.searchLookUpEditDocumentCustomerFilter, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.searchLookUpEditDocumentSupplierFilter, 0);
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
            this.layoutControlFilter.Controls.SetChildIndex(this.txtFromDocumentNumber, 0);
            this.layoutControlFilter.Controls.SetChildIndex(this.txtToDocumentNumber, 0);
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.CustomizationFormText = "Root";
            // 
            // StoreDocumentFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "StoreDocumentFilter";
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
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditDocumentSupplierFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentCustomerFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDocumentSupplierFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            this.ResumeLayout(false);

        }

    }

}
