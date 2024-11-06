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
    //[ChildsUseSameFont]
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class ucitsButton : BaseControl// ucitsObservable
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Button Button
        {
            get
            {
                return this._button;
            }
            //set
            //{
            //    this._button = value;
            //}
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
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

        public ucitsButton()
        {
            InitializeComponent();
        }
    }
}
