using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Helpers;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;
using ITS.POS.Client.Forms;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    public class ucLookUpEdit : LookUpEdit, IPoleDisplayLookupInput
    {

        public ucLookUpEdit()
        {
            this.PoleDisplayName = this.Name;
        }


        public void AttachOnValueChangedEvent(EventHandler handler)
        {
            this.EditValueChanged += handler;
        }

        public void DetachOnValueChangedEvent(EventHandler handler)
        {
            this.EditValueChanged -= handler;
        }

        public string GetText()
        {
            return this.Text;
        }

        private string poleDisplayName;
        public string PoleDisplayName
        {
            get
            {
                return poleDisplayName;
            }
            set
            {
                poleDisplayName = value;
            }
        }
    }
}
