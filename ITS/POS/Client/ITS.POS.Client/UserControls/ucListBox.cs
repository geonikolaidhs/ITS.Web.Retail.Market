using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    public partial class ucListBox : ListBoxControl, IPoleDisplayLookupInput
    {
        public ucListBox()
        {
            InitializeComponent();
        }

        public void AttachOnValueChangedEvent(EventHandler handler)
        {
            throw new NotImplementedException();
        }

        public void DetachOnValueChangedEvent(EventHandler handler)
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public string PoleDisplayName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
