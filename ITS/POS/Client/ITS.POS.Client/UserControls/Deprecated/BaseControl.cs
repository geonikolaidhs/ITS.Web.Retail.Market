using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Helpers;
using DevExpress.XtraLayout;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    //[ToolboxBitmap()]
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class BaseControl : XtraUserControl
    {
        public BaseControl()
        {
            InitializeComponent();
        }

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
  
        public override Font Font
        {
            get
            {
                return Appearance.Font;
            }
            set
            {
                //_font = value;
                Appearance.Font = value;
                object[] attributes = this.GetType().GetCustomAttributes(true);
                foreach(object attrib in attributes)
                {
                    if(attrib.GetType() == typeof(ChildsUseSameFontAttribute))
                    {
                        //SetChildsFont(value);
                        
                        break;
                    }
                }
            }
        }

        //private void SetChildsFont(Font font)
        //{
        //    if (this.Controls != null)
        //    {
        //        IEnumerable<Component> comps = FormHandler.GetAllChildControls(this);
        //        foreach (Component cont in comps)
        //        {
        //            if(cont == this)
        //            {
        //                continue;
        //            }
        //            if (cont is LayoutControlItem)
        //            {
        //                (cont as LayoutControlItem).AppearanceItemCaption.Font = font;
        //            }
        //            else if (cont is LayoutControl)
        //            {
        //                if ((cont as LayoutControl).Root != null)
        //                {
        //                    foreach (LayoutItem v in (cont as LayoutControl).Root.Items)
        //                    {
        //                        v.AppearanceItemCaption.Font = font;
        //                    }
        //                }
        //            }
        //            else if (cont is BaseStyleControl)
        //            {
        //                (cont as BaseStyleControl).Appearance.Font = font;
        //            }
        //            else if (cont is Control)
        //            {

        //                (cont as Control).Font = font;
        //            }
        //        }
        //    }
        //}

        private void ucITS_Load(object sender, EventArgs e)
        {
            this.Font = Appearance.Font;
        }
    }
}
