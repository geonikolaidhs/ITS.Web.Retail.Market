using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;

namespace ITS.POS.Client.Forms
{
    public partial class frmFindItem : frmInputFormBase, IObservable
    {

        public frmFindItem(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            labelControl1.Text = POSClientResources.ItemCodeOrBarcode;
            btnClose.Text = POSClientResources.CLOSE;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void edtItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IDocumentService documentService = Kernel.GetModule<IDocumentService>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                if (e.KeyCode == Keys.Enter)
                {
                    string code = edtItemCode.Text;
                    DocumentHeader header = appContext.CurrentDocument;
                    DocumentDetail foundDetail = documentService.FindLine(code, header);
                    if (foundDetail == null)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(code + " - " + POSClientResources.ITEM_NOT_FOUND));
                        //Notify(new ErrorMessengerParams("Το είδος δεν βρέθηκε"));
                    }
                    else
                    {
                        //Notify(new GridParams(foundDetail));
                        //GlobalContext.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(foundDetail,true,false));
                        appContext.CurrentDocumentLine = foundDetail;
                        Close();
                    }

                }
            }
            catch(Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error in frmFindItem");
            }
        }


        private ThreadSafeList<IObserver> _Observers;
        public ThreadSafeList<IObserver> Observers
        {
            get
            {
                return _Observers;
            }
            set
            {
                _Observers = value;
            }
        }

        public void Attach(IObserver os)
        {
            if (Observers == null)
                Observers = new ThreadSafeList<IObserver>();
            Observers.Add(os);
        }

        public void Dettach(IObserver os)
        {
            if (Observers != null)
                Observers.Remove(os);
        }

        public void Notify(ObserverParams parameters)
        {
            ObserverHelper.NotifyObservers(_Observers, parameters, Kernel.LogFile);
        }

        private void frmFindItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //edtItemCode.HideTouchPad();
        }
    }
}