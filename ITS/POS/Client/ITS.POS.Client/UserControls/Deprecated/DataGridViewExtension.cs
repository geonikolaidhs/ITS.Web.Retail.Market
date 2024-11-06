using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public  partial  class DataGridViewExtension : DataGridView,IObserver, IObserverGrid
    {
        public IPosKernel Kernel
        {
            get
            {
                Form parentForm = this.FindForm();
                if (parentForm is IPOSForm)
                {
                    return (parentForm as IPOSForm).Kernel;
                }
                else
                {
                    throw new POSException("Form must implement IPOSForm");
                }
            }
        }

        public DataGridViewExtension()
        {
            this.ReadOnly = true;
            this.AutoGenerateColumns = false;
            this.MultiSelect = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns =false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;

            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.ActionsToObserve = new List<eActions>();
            this.ActionsToObserve.Add(eActions.MOVE_UP);
            this.ActionsToObserve.Add(eActions.MOVE_DOWN);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO);
            this.DefaultDeletedCellStyle = new DataGridViewCellStyle(){BackColor = Color.Red, Font = new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, FontStyle.Strikeout)};
            InitializeComponent();
            this.RowPostPaint += DetailDataGrid_RowPostPaint;            
            this.DefaultCellStyleChanged += DetailDataGrid_DefaultCellStyleChanged;
        }

        Type[] paramsType = new Type[] { typeof(GridParams) };
        public virtual Type[] GetParamsTypes()
        {
            return paramsType;
        }

        public void InitializeActionSubscriptions()
        {
            foreach (eActions act in ActionsToObserve)
            {
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                actionManager.GetAction(act).Attach(this);
            }
        }

        public void DropActionSubscriptions()
        {
            foreach (eActions act in ActionsToObserve)
            {
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                actionManager.GetAction(act).Dettach(this);
            }
        }

        public bool ShouldSerializeActionsToObserve()
        {
            //DO NOT DELETE
            return false;
        }

        [Browsable(false)]
        public List<eActions> ActionsToObserve
        {
            get;
            set;
        }

        void DetailDataGrid_DefaultCellStyleChanged(object sender, EventArgs e)
        {            
            this.DefaultDeletedCellStyle.Font = new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, this.DefaultDeletedCellStyle.Font.Style);
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DataGridViewCellStyle DefaultDeletedCellStyle { get; set; }

        void DetailDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }


        public bool HideDeletedLines { get; set; }

        

        public virtual void Update(GridParams parameters)
        {

        }
    }
}


