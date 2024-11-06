using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// The base button class that all the other buttons inherit
    /// </summary>
    public partial class ucButton : ucBaseControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Button Button
        {
            get
            {
                return this._button;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public string ButtonText
        {
            get
            {
                return this._button.Text;
            }
            set
            {
                this._button.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public Image ButtonImage
        {
            get
            {
                return this._button.Image;
            }
            set
            {
                this._button.Image = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public ContentAlignment ButtonImageLocation
        {
            get
            {
                return this._button.ImageAlign;
            }
            set
            {
                this._button.ImageAlign = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public Color ButtonForeColor
        {
            get
            {
                return _button.ForeColor;
            }
            set
            {
                _button.ForeColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public Color ButtonBackColor
        {
            get
            {
                return _button.BackColor;
            }
            set
            {
                _button.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public ContentAlignment ButtonTextAlign
        {
            get
            {
                return this._button.TextAlign;
            }
            set
            {
                this._button.TextAlign = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        [Obsolete]
        public ContentAlignment ButtonImageAlign
        {
            get
            {
                return this._button.ImageAlign;
            }
            set
            {
                this._button.ImageAlign = value;
            }
        }

        public ucButton()
        {
            InitializeComponent();
        }
    }
}
