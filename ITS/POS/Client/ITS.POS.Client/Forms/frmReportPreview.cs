using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Kernel;


namespace ITS.POS.Client.Forms
{
    public partial class frmReportPreview : frmDialogBase
    {
        Device printer;
        public frmReportPreview(String formTitle, String reportTitle, List<String> report, Device prnter, IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            Text = formTitle;
            lblReportTitle.Text = reportTitle;
            rtbReport.Lines = report.ToArray();
            btnOK.Text = POSClientResources.OK;
            printer = prnter;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnOK_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (printer is Printer)
            {
                string rtbReportLinesAggregate = rtbReport.Lines.Aggregate((first, sec) => first + Environment.NewLine + sec);
                if (printer.ConType == ConnectionType.OPOS && (printer as Printer).SupportsCutter)
                {
                    rtbReportLinesAggregate += ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut();
                }
                (printer as Printer).Print(rtbReportLinesAggregate);            
            }
            else if (printer is FiscalPrinter)
            {
                FiscalLine[] lines = rtbReport.Lines.Select(x => new FiscalLine() { Type = ePrintType.NORMAL, Value = x }).ToArray();
                (printer as FiscalPrinter).PrintIllegal(lines);

                //(printer as FiscalPrinter).IssueXReport();
            }
            
        }
    }
}