using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Linq;
using DevExpress.Xpo;
using ITS.Retail.Model;

namespace ITS.Retail.Common
{
    public class ITSTypeEditor : UITypeEditor, IDisposable
    {

        private ListBox _box;
        public void Dispose()
        {
            if (_box != null)
            {
                _box.Dispose();
                _box = null;
            }
        }
        public ITSTypeEditor()
            : base()
        {
            _box = new ListBox();
            IEnumerable<Type> types = typeof(Address).Assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(LookupField)) && g.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0);
            _box.Items.AddRange(types.ToArray());
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            //return base.GetEditStyle(context);
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }
        
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                // Access the Property Browser's UI display service
                IWindowsFormsEditorService editorService =(IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    // Create an instance of the UI editor control
                     
                    editorService.DropDownControl(_box);

                    // Return the new property value from the UI editor control
                    return _box.SelectedItem;
                }
            }            
            return base.EditValue(context, provider, value);
        }
    }
}
