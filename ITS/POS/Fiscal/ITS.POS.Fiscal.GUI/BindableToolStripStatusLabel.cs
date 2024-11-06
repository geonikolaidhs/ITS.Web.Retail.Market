using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ITS.POS.Fiscal.GUI
{

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public class BindableToolStripStatusLabel : ToolStripStatusLabel, IBindableComponent
    {
        private BindingContext _context = null;

        public BindingContext BindingContext
        {
            get
            {
                if (null == _context)
                {
                    _context = new BindingContext();
                }

                return _context;
            }
            set { _context = value; }
        }

        private ControlBindingsCollection _bindings;

        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (null == _bindings)
                {
                    _bindings = new ControlBindingsCollection(this);
                }
                return _bindings;
            }
            set { _bindings = value; }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
    }
}
