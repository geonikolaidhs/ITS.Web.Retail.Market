using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Model.Settings;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class ucItsObserver : BaseControl, IObserver
    {
        //Form frm;
        public ucItsObserver()
        {
            ActionsToObserve = new List<eActions>();
            InitializeComponent();
            //this.frm = this.ParentForm;
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

        public virtual Type[] GetParamsTypes()
        {
            return new Type[0];
        }


    }
}
