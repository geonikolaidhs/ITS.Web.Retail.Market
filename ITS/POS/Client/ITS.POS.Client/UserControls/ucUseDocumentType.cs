using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;

namespace ITS.POS.Client.UserControls
{
    public partial class ucUseDocumentType :  ucButton
    {
        public string DocumentTypeCode { get; set; }

        public ucUseDocumentType()
        {
            InitializeComponent();
        }

        private void ucUseDocumentType_Button_Click(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                eMachineStatus status = appContext.GetMachineStatus();

                if (status != eMachineStatus.SALE && status != eMachineStatus.OPENDOCUMENT)
                {
                    return;
                }                
                actionManager.GetAction(eActions.USE_DOCUMENT_TYPE).
                    Execute(new ActionUserDocumentTypeParams(DocumentTypeCode));
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex,"ucUseDocumentType:Button_Click,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }            
        }
    }
}
