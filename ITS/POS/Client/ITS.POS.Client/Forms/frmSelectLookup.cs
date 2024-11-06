using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmSelectLookup : frmInputFormBase
    {
        public BaseObj SelectedObject { get; set; }

        
        protected GridColumn KeyColumn { get; set; }

        public frmSelectLookup(string title,IEnumerable<BaseObj> datasource, string displayMember, string valueMember, List<string> columns, object selectedValue, IPosKernel Kernel)
            : base(Kernel)
        {
            InitializeComponent();
            btnClose.Text = POSClientResources.CLOSE;
            lblTitle.Text = title;
            btnOK.Text = POSClientResources.OK;

           /* cmbLookupList.Properties.DisplayMember = displayMember;
            foreach(string column in columns)
            {
                cmbLookupList.Properties.Columns.Add(new LookUpColumnInfo(column));
            }
            
            cmbLookupList.Properties.ValueMember = valueMember;
            cmbLookupList.Properties.NullText = "";
            cmbLookupList.Properties.DataSource = datasource;
            cmbLookupList.EditValue = selectedValue;*/

            
            GridView gridView = (this.grcLookupList.MainView as GridView);
            //DisplayColumn = gridView.Columns.AddField(displayMember);
            KeyColumn = gridView.Columns.AddField(valueMember);
            //DisplayColumn.Visible = true;
            foreach (string column in columns)
            {
                GridColumn col = gridView.Columns.AddField(column);
                col.Visible = true;
                string caption = POSClientResources.ResourceManager.GetString(column);
                if(string.IsNullOrWhiteSpace(caption)==false)
                {
                    col.Caption = caption;
                }
            }

            this.grcLookupList.DataSource = datasource;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            /*if (cmbLookupList.EditValue != null)
            {
                this.SelectedObject = (cmbLookupList.Properties.DataSource as IEnumerable<BaseObj>).Where(g => g.Oid == (Guid)cmbLookupList.EditValue).FirstOrDefault();
            }*/
            GridView view = (this.grcLookupList.MainView as GridView);
            this.SelectedObject = view.GetRow(view.FocusedRowHandle) as BaseObj;
            this.Close();
        }

        private void cmbLookupList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

        public override void Update(ObserverPattern.ObserverParameters.KeyListenerParams parameters)
        {
            base.Update(parameters);
            GridView view = (this.grcLookupList.MainView as GridView);
            if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_UP)
            {
                //cmbLookupList.ItemIndex--;                
                if(view.FocusedRowHandle >0)
                {
                    view.FocusedRowHandle--;
                }
            }
            else if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_DOWN)
            {
               // cmbLookupList.ItemIndex++;
                if (view.FocusedRowHandle >= 0 && view.FocusedRowHandle < (view.RowCount - 1))
                {
                    view.FocusedRowHandle++;
                }
            }
        }

        private void frmSelectLookup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }
    }
}
