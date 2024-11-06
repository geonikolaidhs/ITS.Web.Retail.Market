using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Base control for having a label with two parts
    /// </summary>
    public partial class ucDoubleLabel : ucObserver
    {
        public ucDoubleLabel()
        {
            InitializeComponent();
            this.LabelPercentage = 10;

        }

        private Orientation _Orientation;
        public Orientation Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                _Orientation = value;
                ResetLayout();
            }
        }

        private float _perc;
        public float LabelPercentage
        {
            get
            {
                return _perc;
            }
            set
            {
                if (value <= 100 && value >= 0)
                {
                    _perc = value;
                    ResetLayout();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl TitleLabel
        {
            get
            {
                
                return this.lblTitle;
            }

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl ValueLabel
        {
            get
            {
                return this.lblValue;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            set
            {
                this.layout.CellBorderStyle = value;
            }
            get
            {
                return this.layout.CellBorderStyle;
            }
        }


        private String _LabelText;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public String LabelText 
        {
            get
            {
                return _LabelText;
            }
            set
            {
                _LabelText = value;
                this.lblTitle.Text = _LabelText;
            }
        }


        private void ResetLayout()
        {
            layout.ColumnStyles.Clear();
            layout.RowStyles.Clear();
            layout.Controls.Clear();
            switch (Orientation)
            {
                case Orientation.Vertical:
                    {
                        layout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                        layout.ColumnCount = 1;
                        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                        layout.Controls.Add(this.lblValue, 0, 1);
                        layout.Controls.Add(this.lblTitle, 0, 0);
                        layout.Dock = DockStyle.Fill;
                        layout.Location = new Point(0, 0);
                        layout.Name = "layout";
                        layout.RowCount = 2;
                        layout.RowStyles.Add(new RowStyle(SizeType.Percent, _perc));
                        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f-_perc));
                        layout.Size = new Size(762, 54);
                        layout.TabIndex = 0;                        
                    }
                    break;
                case Orientation.Horizontal:
                    {
                        layout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                        layout.ColumnCount = 2;
                        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, _perc));
                        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f - _perc));
                        layout.Controls.Add(this.lblValue, 1, 0);
                        layout.Controls.Add(this.lblTitle, 0, 0);
                        layout.Dock = DockStyle.Fill;
                        layout.Location = new Point(0, 0);
                        layout.Name = "layout";
                        layout.RowCount = 1;
                        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));                        
                        layout.Size = new Size(762, 54);
                        layout.TabIndex = 0;
                    }
                    break;
            }
        }

        
    }
}
