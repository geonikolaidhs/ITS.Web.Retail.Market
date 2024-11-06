using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI.PivotGrid;

namespace ITS.Retail.Common
{
    [Designer("DevExpress.XtraReports.Design._XRPivotGridDesigner,DevExpress.XtraReports.v14.2.Design")]
    //[DXDisplayName(typeof(DevExpress.XtraReports.ResFinder), "PropertyNamesRes", "DevExpress.XtraReports.UI.XRPivotGrid", "PivotGrid")]
    [DisplayName("Pivot Grid Extension")]
    [ToolboxBitmap(typeof(DevExpress.XtraReports.ResFinder), "Bitmaps256.XRPivotGrid.bmp")]
    [ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRPivotGrid.png,DevExpress.XtraReports.v14.2.Extensions")]
    [ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRPivotGrid.png,DevExpress.XtraReports.v14.2.Extensions")]
    [ToolboxItem(true)]
    [ToolboxTabName("DX.14.2: Report Controls")]
    [XRDesigner("DevExpress.XtraReports.Design.XRPivotGridDesigner,DevExpress.XtraReports.v14.2.Extensions")]
    [XRToolboxSubcategory(2, 3)]
    public class XRPivotGridExtension : XRPivotGrid
    {
        public bool UseWebOLAPConnection { get; set; }

        public XRPivotGridExtension()
        {
            this.UseWebOLAPConnection = true; 
        }   

        protected override void OnBeforePrint(System.Drawing.Printing.PrintEventArgs e)
        {
            XtraReportBaseExtension report = this.Report as XtraReportBaseExtension;
            SetOlap(report);

            base.OnBeforePrint(e);
        }

        private void SetOlap(XtraReportBaseExtension report)
        {
            try
            {
                if (report != null)
                {
                    if (this.UseWebOLAPConnection && String.IsNullOrWhiteSpace(report.GetOLAPConnectionString()) == false)
                    {
                        this.OLAPConnectionString = report.GetOLAPConnectionString();
                    }
                }
            }
            catch (Exception e)
            {
                string errorMessage = e.GetFullMessage();
            }
        }

        protected override XRPivotGridData CreatePivotGridData()
        {
            XtraReportBaseExtension report = this.Report as XtraReportBaseExtension;
            SetOlap(report);
            return base.CreatePivotGridData();
        }

         
    }
}
