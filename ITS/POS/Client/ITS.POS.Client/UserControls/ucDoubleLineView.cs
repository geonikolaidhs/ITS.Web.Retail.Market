using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays only the current and the previous detail of the current document
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucDoubleLineView : ucObserver, IObserverGrid
    {

        public float LabelPercentage
        {
            get
            {
                return tableLayoutPanel1.ColumnStyles[0].Width;
            }
            set
            {
                tableLayoutPanel1.ColumnStyles[0].Width = value;
            }
        }

        public ucDoubleLineView()
        {
            InitializeComponent();
            ActionsToObserve.Add(eActions.MOVE_DOWN);
            ActionsToObserve.Add(eActions.MOVE_UP);
            ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            currentLineIndex = -1;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutPanel TableLayoutPanel
        {
            get
            {
                return this.tableLayoutPanel1;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl CurrentLineLabel
        {
            get
            {
                return this.lblCurrentLine;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl CurrentLineQtyLabel
        {
            get
            {
                return this.lblCurrentLineQty;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl CurrentLineValueLabel
        {
            get
            {
                return this.lblCurrentLineValue;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl CurrentLineMuliplierLabel
        {
            get
            {
                return this.lblCurrentLineX;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl PreviousLineLabel
        {
            get
            {
                return this.lblPreviousLine;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl PreviousLineQtyLabel
        {
            get
            {
                return this.lblPreviousLineQty;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl PreviousLineValueLabel
        {
            get
            {
                return this.lblPreviousLineValue;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl PreviousLineMuliplierLabel
        {
            get
            {
                return this.lblPreviousLineX;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] //do not delete for backwards compartibility
        public string PreviousLineLabelText
        {
            get
            {
                return this.lblPreviousLine.Text;
            }
            set
            {
                this.lblPreviousLine.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] //do not delete for backwards compartibility
        public string CurrentLineLabelText
        {
            get
            {
                return this.lblCurrentLine.Text;
            }
            set
            {
                this.lblCurrentLine.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] //do not delete for backwards compartibility
        public Color PreviousLineForeColor
        {
            get
            {
                return lblPreviousLine.ForeColor;
            }
            set
            {
                lblPreviousLine.ForeColor = value;
                lblPreviousLineQty.ForeColor = value;
                lblPreviousLineValue.ForeColor = value;
                lblPreviousLineX.ForeColor = value;
                txtPreviousLineDescription.ForeColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] //do not delete for backwards compartibility
        public Color CurrentLineForeColor
        {
            get
            {
                return lblCurrentLine.ForeColor;
            }
            set
            {
                lblCurrentLine.ForeColor = value;
                lblCurrentLineQty.ForeColor = value;
                lblCurrentLineValue.ForeColor = value;
                lblCurrentLineX.ForeColor = value;
                txtCurrentLineDescription.ForeColor = value;
            }
        }


        private int currentLineIndex;

        private void SetDisplay(DocumentDetail currentLine = null, DocumentDetail previousLine = null)
        {
            if (currentLine != null)
            {
                txtCurrentLineDescription.Text = currentLine.CustomDescription;
                lblCurrentLineQty.Text = currentLine.Qty.ToString();
                lblCurrentLineValue.Text = currentLine.GrossTotal.ToString("C2");
                lblCurrentLineX.Visible = true;
            }
            else
            {
                txtCurrentLineDescription.Text ="";
                lblCurrentLineQty.Text = "";
                lblCurrentLineValue.Text = "";
                lblCurrentLineX.Visible = false;
            }
            if (previousLine != null)
            {
                txtPreviousLineDescription.Text = previousLine.CustomDescription;
                lblPreviousLineQty.Text = previousLine.Qty.ToString();
                lblPreviousLineValue.Text = previousLine.GrossTotal.ToString("C2");
                lblPreviousLineX.Visible = true;
            }
            else
            {
                txtPreviousLineDescription.Text = "";
                lblPreviousLineQty.Text = "";
                lblPreviousLineValue.Text = "";
                lblPreviousLineX.Visible = false;
            }
        }

        Type[] paramsTypes = new Type[] { typeof(GridParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public void Update(GridParams parameters)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (parameters.SelectedDocumentDetail != null && appContext.CurrentDocument!=null)
            {
                DocumentHeader header = appContext.CurrentDocument;
                int newPosition = header.DocumentDetails.Where(g=>g.IsCanceled==false).ToList().IndexOf(parameters.SelectedDocumentDetail);
                currentLineIndex = newPosition;
                DocumentDetail previousDetail = null;
                if (currentLineIndex > 0)
                {
                    previousDetail = header.DocumentDetails.Where(g => g.IsCanceled == false).ToList()[currentLineIndex - 1];
                }
                this.SetDisplay(parameters.SelectedDocumentDetail, previousDetail);
            }
            else if (parameters.Navigation != null && appContext.CurrentDocument != null)
            {
                DocumentDetail currentLine = null;
                DocumentDetail previousLine = null;
                switch (parameters.Navigation)
                {
                    case eNavigation.MOVEUP:
                        if (currentLineIndex > 0)
                        {
                            currentLineIndex--;
                        }
                        previousLine = appContext.CurrentDocument.DocumentDetails.Where(g => g.IsCanceled == false).ElementAtOrDefault(currentLineIndex - 1);
                        currentLine = appContext.CurrentDocument.DocumentDetails.Where(g => g.IsCanceled == false).ElementAtOrDefault(currentLineIndex);
                        break;
                    case eNavigation.MOVEDOWN:
                        if (appContext.CurrentDocument.DocumentDetails.Where(g => g.IsCanceled == false).Count() - 1 > currentLineIndex)
                        {
                            currentLineIndex++;
                        }
                        previousLine = appContext.CurrentDocument.DocumentDetails.Where(g => g.IsCanceled == false).ElementAtOrDefault(currentLineIndex - 1);
                        currentLine = appContext.CurrentDocument.DocumentDetails.Where(g => g.IsCanceled == false).ElementAtOrDefault(currentLineIndex);
                        break;
                }
                SetDisplay(currentLine, previousLine);
            }
            else if (parameters.DocumentHeader == null)
            {
                SetDisplay(null, null);
                currentLineIndex = -1;
            }
        }

        private void ucDoubleLineView_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                lblCurrentLineX.Visible = true;
                lblPreviousLineX.Visible = true;
                lblCurrentLineQty.Text = "QTY";
                lblCurrentLineValue.Text = "VALUE";
                lblPreviousLineQty.Text = "QTY";
                lblPreviousLineValue.Text = "VALUE";
            }
        }
    }
}
