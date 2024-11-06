using DevExpress.XtraEditors;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class DocumentPrintForm : XtraLocalizedForm
    {
       
        public DocumentPrintForm(XtraReportBaseExtension report, bool autoprint=false)
        {
            InitializeComponent();
            this.documentViewer1.DocumentSource = report;
            if (autoprint)
            {
                this.Load += DocumentPrintForm_Load;
                this.documentViewer1.PrintingSystem.EndPrint += PrintingSystem_EndPrint;
            }
        }

        void PrintingSystem_EndPrint(object sender, EventArgs e)
        {
            this.Close();
        }

        void DocumentPrintForm_Load(object sender, EventArgs e)
        {
            documentViewer1.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.PrintDirect);
        }


    }
}
