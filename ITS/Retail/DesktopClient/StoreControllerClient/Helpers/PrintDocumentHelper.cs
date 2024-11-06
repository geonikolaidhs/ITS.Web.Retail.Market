using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using ITS.Retail.PrintServer.Common;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public static class PrintDocumentHelper
    {
        public static void PrintDocument(DocumentHeader documentHeader, bool autoprint)
        {
            IEnumerable<DocumentTypeCustomReport> validReports = ReportsHelper.GetValidDocumentTypeCustomReports(Program.Settings.CurrentUser, documentHeader.DocumentType,
                Program.Settings.ReadOnlyUnitOfWork);

            int validReportsCount = validReports.Count();

            if (validReportsCount == 0)
            {
                //Print default report
                XtraMessageBox.Show(ResourcesLib.Resources.PleaseReviseDocumentTypeSettingsDocumentViewForm, ResourcesLib.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                
            }
            else if (validReportsCount == 1)
            {
                StoreDocumentSeriesType storeDocumentSeriesType = documentHeader.DocumentType
                                                                .StoreDocumentSeriesTypes
                                                                .FirstOrDefault(storeDocSeriesType => storeDocSeriesType.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid);

                POSDevice remotePrinterService = storeDocumentSeriesType.PrintServiceSettings ==null ? null : storeDocumentSeriesType.PrintServiceSettings.RemotePrinterService;
                if (autoprint && remotePrinterService != null && remotePrinterService.IsActive)
                {
                    PrintServerPrintDocumentResponse response = PrinterServiceHelper.PrintDocument(remotePrinterService, Program.Settings.CurrentUser.Oid, documentHeader.Oid, documentHeader.POSID, storeDocumentSeriesType.PrintServiceSettings.PrinterNickName);
                    if (response == null)
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show( ResourcesLib.Resources.CouldNotEstablishConnection + " Remote Print Service :" + remotePrinterService.Name,ResourcesLib.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);                        
                        return;
                    }
                    switch (response.Result)
                    {
                        case ePrintServerResponseType.FAILURE:
                            SplashScreenManager.CloseForm(false);
                            XtraMessageBox.Show(response.Explanation + Environment.NewLine + response.ErrorMessage, ResourcesLib.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);                            
                            return;
                        case ePrintServerResponseType.SUCCESS:
                            SplashScreenManager.CloseForm(false);
                            XtraMessageBox.Show(ResourcesLib.Resources.SuccefullyCompleted, ResourcesLib.Resources.SuccefullyCompleted, MessageBoxButtons.OK, MessageBoxIcon.Warning);                            
                            return;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    //Print this report
                    DocumentTypeCustomReport dtreport = validReports.First();
                    CustomReport report = dtreport.Report;
                    string title, description;
                    XtraReportBaseExtension xtraReport = ReportsHelper.GetXtraReport(report.Oid, Program.Settings.StoreControllerSettings.Owner,
                        Program.Settings.CurrentUser, null, out title, out description);
                    (xtraReport as SingleObjectXtraReport).ObjectOid = documentHeader.Oid;
                    XtraReportBaseExtension dupls = ReportsHelper.DuplicateReport(xtraReport, dtreport.Duplicates);

                    using (DocumentPrintForm documentPrintForm = new DocumentPrintForm(dupls, autoprint))
                    {
                        documentPrintForm.ShowDialog();
                    }
                }
            }
            else if (validReportsCount > 1)
            {
                //Ask user...
                using (SelectDocumentReportForm selectDocumentReportForm = new SelectDocumentReportForm(validReports, documentHeader.Oid, autoprint))
                {
                    selectDocumentReportForm.ShowDialog();
                }
            }
            SplashScreenManager.CloseForm(false);
        }
    }
}
