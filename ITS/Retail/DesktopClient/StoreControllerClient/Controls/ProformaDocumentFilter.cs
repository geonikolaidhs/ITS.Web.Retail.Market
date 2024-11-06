using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public class ProformaDocumentFilter : DocumentFilter
    {
        public ProformaDocumentFilter(List<Guid> proformaTypes) : base(Platform.Enumerations.eDivision.Sales, true, proformaTypes)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "ITS.Retail.DesktopClient.StoreControllerClient.Layouts.proforma_filter_layout.xml";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                this.layoutControlFilter.RestoreLayoutFromStream(stream);
            }
            SecondaryFilterControl = new ProformaDocumentSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;
            CriteriaOperator proformaDocumentSeriesCriteria =CriteriaOperator.And(new BinaryOperator("Store", Program.Settings.StoreControllerSettings.Store.Oid),
                                                                           new BinaryOperator("IsCancelingSeries", false)
                                                                         );
            lueDocumentSeries.Properties.DataSource = new XPCollection<DocumentSeries>(Program.Settings.ReadOnlyUnitOfWork, proformaDocumentSeriesCriteria);
            lueDocumentSeries.Properties.ValueMember = "Oid";
            lueDocumentSeries.Properties.Columns.Clear();
            lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));

            layoutControlItemDocumentCustomerFilter.HideToCustomization();
            layoutControlItemDocumentSupplierFilter.HideToCustomization();
        }

        //protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        //{
        //    base.SearchMain(criteria, uow);                
        //}

        public override int Lines
        {
            get
            {
                return 2;
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlItemDocumentType
            // 
            this.layoutControlItemDocumentType.Size = new System.Drawing.Size(218, 40);
            // 
            // layoutControlItemDocumentSeries
            // 
            this.layoutControlItemDocumentSeries.Location = new System.Drawing.Point(218, 0);
            this.layoutControlItemDocumentSeries.Size = new System.Drawing.Size(209, 40);
            // 
            // layoutControlItemTransformationStatus
            // 
            this.layoutControlItemTransformationStatus.Location = new System.Drawing.Point(402, 40);
            this.layoutControlItemTransformationStatus.Size = new System.Drawing.Size(402, 40);
            // 
            // layoutControlItemExecutionFrom
            // 
            this.layoutControlItemFiscalFrom.Location = new System.Drawing.Point(427, 160);
            this.layoutControlItemFiscalFrom.Size = new System.Drawing.Size(201, 40);
            // 
            // layoutControlItemFromDocumentNumber
            // 
            this.layoutControlItemFromDocumentNumber.Location = new System.Drawing.Point(427, 0);
            this.layoutControlItemFromDocumentNumber.Size = new System.Drawing.Size(201, 40);
            // 
            // layoutControlItemFromDate
            // 
            this.layoutControlItemFromDate.Size = new System.Drawing.Size(186, 80);
            // 
            // layoutControlItemToDocumentNumber
            // 
            this.layoutControlItemToDocumentNumber.Location = new System.Drawing.Point(628, 0);
            this.layoutControlItemToDocumentNumber.Size = new System.Drawing.Size(176, 40);
            // 
            // layoutControlItemToDate
            // 
            this.layoutControlItemToDate.Location = new System.Drawing.Point(402, 80);
            this.layoutControlItemToDate.Size = new System.Drawing.Size(402, 40);
            // 
            // layoutControlItemFromTime
            // 
            this.layoutControlItemFromTime.Location = new System.Drawing.Point(186, 40);
            this.layoutControlItemFromTime.Size = new System.Drawing.Size(216, 40);
            // 
            // layoutControlItemToTime
            // 
            this.layoutControlItemToTime.Location = new System.Drawing.Point(186, 80);
            this.layoutControlItemToTime.Size = new System.Drawing.Size(216, 40);
            // 
            // layoutControlItemExecutionTo
            // 
            this.layoutControlItemFiscalTo.Location = new System.Drawing.Point(628, 160);
            this.layoutControlItemFiscalTo.Size = new System.Drawing.Size(176, 40);
            // 
            // layoutControlItemStatus
            // 
            this.layoutControlItemStatus.Location = new System.Drawing.Point(0, 200);
            this.layoutControlItemStatus.Size = new System.Drawing.Size(427, 40);
            // 
            // lueDocumentType
            // 
            this.lueDocumentType.Size = new System.Drawing.Size(214, 20);
            // 
            // lueDocumentSeries
            // 
            this.lueDocumentSeries.Location = new System.Drawing.Point(230, 28);
            this.lueDocumentSeries.Size = new System.Drawing.Size(205, 20);
            // 
            // lueTransformationStatus
            // 
            this.lueTransformationStatus.Location = new System.Drawing.Point(414, 68);
            this.lueTransformationStatus.Size = new System.Drawing.Size(398, 20);
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
            this.dateFromDate.Size = new System.Drawing.Size(182, 20);
            // 
            // timeEditFrom
            // 
            this.timeEditFrom.EditValue = null;
            this.timeEditFrom.Location = new System.Drawing.Point(198, 68);
            this.timeEditFrom.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.EditFormat.FormatString = "HH:mm";
            this.timeEditFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeEditFrom.Properties.Mask.EditMask = "HH:mm";
            this.timeEditFrom.Size = new System.Drawing.Size(212, 20);
            // 
            // dateToDate
            // 
            this.dateToDate.EditValue = null;
            this.dateToDate.Location = new System.Drawing.Point(414, 108);
            this.dateToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateToDate.Size = new System.Drawing.Size(398, 20);
            // 
            // timeToTime
            // 
            this.timeToTime.EditValue = null;
            this.timeToTime.Location = new System.Drawing.Point(198, 108);
            this.timeToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeToTime.Properties.Mask.EditMask = "HH:mm";
            this.timeToTime.Size = new System.Drawing.Size(212, 20);
            // 
            // layoutControlItemCreatedBy
            // 
            this.layoutControlItemCreatedBy.Location = new System.Drawing.Point(427, 200);
            this.layoutControlItemCreatedBy.Size = new System.Drawing.Size(377, 40);
            // 
            // layoutControlItemExecutionFromTime
            // 
            this.layoutControlItemFiscalFromTime.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItemFiscalFromTime.Size = new System.Drawing.Size(218, 80);
            // 
            // layoutControlItemExecutionToTime
            // 
            this.layoutControlItemFiscalToTime.Location = new System.Drawing.Point(218, 120);
            this.layoutControlItemFiscalToTime.Size = new System.Drawing.Size(209, 80);
            // 
            // dateExecutionFromDate
            // 
            this.dateFiscalFromDate.EditValue = null;
            this.dateFiscalFromDate.Location = new System.Drawing.Point(439, 188);
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dateFiscalFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateFiscalFromDate.Size = new System.Drawing.Size(197, 20);
            // 
            // timeExecutionFromTime
            // 
            this.timeFiscalFromTime.EditValue = null;
            this.timeFiscalFromTime.Location = new System.Drawing.Point(12, 148);
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalFromTime.Properties.Mask.EditMask = "HH:mm";
            this.timeFiscalFromTime.Size = new System.Drawing.Size(214, 20);
            // 
            // dateExecutionToDate
            // 
            this.dateFiscalToDate.EditValue = null;
            this.dateFiscalToDate.Location = new System.Drawing.Point(640, 188);
            this.dateFiscalToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dateFiscalToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateFiscalToDate.Properties.Mask.EditMask = "";
            this.dateFiscalToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dateFiscalToDate.Size = new System.Drawing.Size(172, 20);
            // 
            // timeExecutionToTime
            // 
            this.timeFiscalToTime.EditValue = null;
            this.timeFiscalToTime.Location = new System.Drawing.Point(230, 148);
            this.timeFiscalToTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.EditFormat.FormatString = "HH:mm";
            this.timeFiscalToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeFiscalToTime.Properties.Mask.EditMask = "HH:mm";
            this.timeFiscalToTime.Size = new System.Drawing.Size(205, 20);
            // 
            // lueDocumentStatus
            // 
            this.lueDocumentStatus.Location = new System.Drawing.Point(12, 228);
            this.lueDocumentStatus.Size = new System.Drawing.Size(423, 20);
            // 
            // layoutControlItemCreatedByDevice
            // 
            this.layoutControlItemCreatedByDevice.Location = new System.Drawing.Point(427, 120);
            this.layoutControlItemCreatedByDevice.Size = new System.Drawing.Size(377, 40);
            // 
            // lueCreatedBy
            // 
            this.lueCreatedBy.Location = new System.Drawing.Point(439, 228);
            this.lueCreatedBy.Size = new System.Drawing.Size(373, 20);
            // 
            // lueCreatedByDevice
            // 
            this.lueCreatedByDevice.Location = new System.Drawing.Point(439, 148);
            this.lueCreatedByDevice.Size = new System.Drawing.Size(373, 20);
            // 
            // txtFromDocumentNumber
            // 
            this.txtFromDocumentNumber.Location = new System.Drawing.Point(439, 28);
            this.txtFromDocumentNumber.Size = new System.Drawing.Size(197, 20);
            // 
            // txtToDocumentNumber
            // 
            this.txtToDocumentNumber.Location = new System.Drawing.Point(640, 28);
            this.txtToDocumentNumber.Size = new System.Drawing.Size(172, 20);
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemStatus,
            this.layoutControlItemCreatedBy,
            this.layoutControlItemFiscalTo,
            this.layoutControlItemFiscalToTime,
            this.layoutControlItemFiscalFromTime,
            this.layoutControlItemFiscalFrom,
            this.layoutControlItemDocumentSeries,
            this.layoutControlItemDocumentType,
            this.layoutControlItemFromDocumentNumber,
            this.layoutControlItemToDocumentNumber});
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(762, 170, 738, 619);
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
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.CustomizationFormText = "Root";
            // 
            // ProformaDocumentFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ProformaDocumentFilter";
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            this.ResumeLayout(false);

        }



    }
}
