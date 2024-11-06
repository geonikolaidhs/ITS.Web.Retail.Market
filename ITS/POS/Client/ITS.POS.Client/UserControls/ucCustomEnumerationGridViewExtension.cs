using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.ObserverPattern.ObserverParameters;

namespace ITS.POS.Client.UserControls
{
    public partial class ucCustomEnumerationGridViewExtension : DataGridView, IObserver, IObserverCustomEnumerationGrid
    {
        public ucCustomEnumerationGridViewExtension()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public List<eActions> ActionsToObserve
        {
            get;
            set;
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

        public void DropActionSubscriptions()
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            foreach (eActions action in ActionsToObserve)
            {
                actionManager.GetAction(action).Dettach(this);
            }
        }

        Type[] paramsType = new Type[] { typeof(CustomEnumerationGridParams) };
        public virtual Type[] GetParamsTypes()
        {
            return paramsType;
        }

        public void InitializeActionSubscriptions()
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            foreach (eActions action in ActionsToObserve)
            {                
                actionManager.GetAction(action).Attach(this);
            }
        }

        public void Update(CustomEnumerationGridParams parameters)
        {
            throw new NotImplementedException();
        }
    }
}
