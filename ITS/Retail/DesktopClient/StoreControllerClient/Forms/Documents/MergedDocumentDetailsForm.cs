using DevExpress.XtraEditors;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class MergedDocumentDetailsForm : XtraLocalizedForm
    {
        private List<MergedDocumentDetail> Details;

        public MergedDocumentDetailsForm(List<MergedDocumentDetail> details)
        {
            this.Details = details;
            InitializeComponent();
            gridControlMergeDocumentDetails.DataSource = Details;
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog2.RestoreDirectory = true;
        }



        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void gridViewMarkUpValues_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.RowHandle < 0)
            {
                return;
            }

        }

        private void simpleButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    var filename = saveFileDialog2.FileName;
                    if (filename != null)
                    {
                        string[] array = filename.Split('.');
                        if (array != null && array.Count() > 0 && array.Last() != "xlsx")
                        {
                            filename = filename + ".xlsx";
                        }
                        else if (array != null && array.Count() < 1)
                        {
                            filename = filename + ".xlsx";
                        }
                        else if (array == null)
                        {
                            filename = filename + ".xlsx";
                        }
                        gridControlMergeDocumentDetails.ExportToXlsx(filename);
                    }
                    else
                    {
                        using (Stream myStream = saveFileDialog2.OpenFile())
                        {
                            if (myStream != null)
                            {
                                gridControlMergeDocumentDetails.ExportToXlsx(myStream);
                                myStream.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
