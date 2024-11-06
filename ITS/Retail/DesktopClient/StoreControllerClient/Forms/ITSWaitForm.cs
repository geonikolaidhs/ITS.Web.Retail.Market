using DevExpress.XtraWaitForm;
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
    public partial class ITSWaitForm : WaitForm
    {
        public ITSWaitForm()
        {
            
            InitializeComponent();
            progressPanel1.Caption = ResourcesLib.Resources.PleaseWait;
            progressPanel1.Description = ResourcesLib.Resources.Loading;
  
        }
        
    }
}
