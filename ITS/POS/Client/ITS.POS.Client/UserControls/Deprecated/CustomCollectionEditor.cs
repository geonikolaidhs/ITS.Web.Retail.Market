using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ITS.POS.Client.Forms;
using ITS.POS.Client.ObserverPattern;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;

namespace ITS.POS.Client.UserControls
{

    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    [ToolboxItem(true), Designer(typeof(ControlDesigner))] //, Description("GridControl.")
    public class MyGridControl : GridControl {

        private GridView gridView1;

        private void InitializeComponent()
        {
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this;
            this.gridView1.Name = "gridView1";
            // 
            // MyGridControl
            // 
            this.MainView = this.gridView1;
            this.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }
    /*
    [ToolboxItem(true), Designer(typeof(ControlDesigner)), Description("GridColumn.")]
    class MyGridColumn : GridColumn { }
    [ToolboxItem(true), Designer(typeof(ControlDesigner)), Description("GridView.")]
    class MyGridView : GridView {
    }


    [ToolboxItem(true), Designer(typeof(ControlDesigner)), Description("RepositoryItemTextEdit.")]
    class MyRepositoryItemTextEdit : RepositoryItemTextEdit { }*/

    public class CustomCollectionEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService = null;

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context,IServiceProvider provider,object value)
        {
            if (provider != null)
            {
                editorService =
                    provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {

                frmSelectComponent editorForm = new frmSelectComponent(value as List<string>, (context.Instance as Control).FindForm(), (context.Instance as Control));
                if (editorService.ShowDialog(editorForm) == DialogResult.OK)
                {
                    value = editorForm.GetCheckedObserversNames();
                }
            }
            return value;
        }

        public override bool GetPaintValueSupported(
            ITypeDescriptorContext context)
        {
            return true;
        }


        public override void PaintValue(PaintValueEventArgs e)
        {

        }

        //IWindowsFormsEditorService editorService;

        //public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        //{
        //    if (context != null && context.Instance != null)
        //    {
        //        return UITypeEditorEditStyle.Modal;
        //    }
        //    return base.GetEditStyle(context);
        //}

        //public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        //{
        //    if (value != null && !(value.GetType() == ObservableCollection<Control)));
        //        return value;
        //        if (context != null && context.Instance != null && provider != null)
        //            if (!(context.Instance.GetType() == IControlSelector))
        //            {
        //                return value;
        //            }
        //            service = ((IWindowsFormsEditorService)(provider.GetService(typeof(IWindowsFormsEditorService))));
        //            if (service != null)
        //                System.Collections.ObjectModel.ObservableCollection[] selectedControls;
        //                Of;
        //                Control;
        //                new System.Collections.ObjectModel.ObservableCollection(Of, Control);
        //                ((System.Collections.ObjectModel.ObservableCollection[])(value));
        //                Of;
        //                Control;
        //                IControlSelector instance = ((IControlSelector)(context.Instance));
        //                ControlListDialog f = new ControlListDialog();
        //                f.Init(instance.TopLevelControl, selectedControls, instance.InitialFilter);
        //                if ((service.ShowDialog(f) == DialogResult.OK))
        //                {
        //                    return ((System.Collections.ObjectModel.ObservableCollection[])(f.SelectedList));
        //                    Of;
        //                    Control;
        //                }
        //            }
        //        }
        //        return value;
        //    }
        //}

        //public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        //{
        //    var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

        //    ListBox lb = new ListBox();
        //    if ((context.Container as Form) != null)
        //    {
        //        foreach (Control ct in (context.Container as Form).Controls)
        //        {
        //            if (ct.GetType().IsAssignableFrom(typeof(IObserver)))
        //            {
        //                lb.Items.Add(ct as IObserver);
        //            }
        //        }
        //    }

        //    if (value != null)
        //    {
        //        lb.SelectedItem = value;
        //    }

        //    edSvc.DropDownControl(lb);

        //    value = (Type)lb.SelectedItem;

        //    return value;
        //}

        //void lb_Click(object sender, EventArgs e)
        //{
        //    editorService.CloseDropDown();
        //}
    }
}
