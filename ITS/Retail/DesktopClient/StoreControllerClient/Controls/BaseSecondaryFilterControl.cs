using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars.Navigation;
using System.Reflection;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraEditors.Controls;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    [ProvideProperty("BoundPropertyName", typeof(BaseEdit))]
    [ProvideProperty("BoundFieldName", typeof(BaseEdit))]
    public partial class BaseSecondaryFilterControl : DevExpress.XtraEditors.XtraUserControl, IExtenderProvider
    {
        internal class ExtraProperties
        {
            public string BoundPropertyName { get; set; }
            public string BoundFieldName { get; set; }

            public ExtraProperties()
            {
                BoundPropertyName = "EditValue";
            }
        }
        public void SetBoundFieldName(BaseEdit edit, string value)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, new ExtraProperties());
            }
            _extrProperties[edit].BoundFieldName = value;
        }

        // Do not remove. It is used by Winforms Designer
        public string GetBoundFieldName(BaseEdit edit)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, new ExtraProperties());
            }
            return _extrProperties[edit].BoundFieldName;
        }

        // Do not remove. It is used by Winforms Designer
        public string GetBoundPropertyName(BaseEdit edit)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, new ExtraProperties());
            }
            return _extrProperties[edit].BoundPropertyName;
        }

        public void SetBoundPropertyName(BaseEdit edit, string value)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, new ExtraProperties());
            }
            _extrProperties[edit].BoundPropertyName = value;
        }

        public BaseFilterControl ParentFilterControl { get; set; }
        private bool _LocalizationHasRun = false;
        protected IEnumerable<Component> EnumerateComponents()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null
                   select component;
        }
        protected virtual string LocalizeString(string input)
        {
            string result = null;
            if (input.StartsWith("@@"))
            {
                result = Resources.ResourceManager.GetString(input.TrimStart('@'));

                if (string.IsNullOrEmpty(result) && this.DesignMode == false)
                {
                    Program.Logger.Debug(String.Format("Not Translated Message : {0}", input));
                }
            }
            return String.IsNullOrWhiteSpace(result) ? input : result;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Disposing || _LocalizationHasRun)
            {
                return;
            }
            _LocalizationHasRun = true;
            if (DesignMode == false)
            {
                foreach (KeyValuePair<BaseEdit, ExtraProperties> pair in this._extrProperties)
                {
                    if (string.IsNullOrWhiteSpace(pair.Value.BoundFieldName) == false && string.IsNullOrWhiteSpace(pair.Value.BoundPropertyName) == false)
                    {
                        pair.Key.DataBindings.Add(pair.Value.BoundPropertyName, this.ParentFilterControl.SearchFilter, pair.Value.BoundFieldName, true, DataSourceUpdateMode.OnPropertyChanged);
                    }
                }
                IEnumerable<Component> enumerateComponents = EnumerateComponents();
                btnSearch.Text = Resources.Search;
                foreach (Component control in enumerateComponents)
                {

                    if (control is BarItem)
                    {
                        BarItem casted = (BarItem)control;
                        casted.Caption = LocalizeString(casted.Caption);
                    }
                    else if (control is RibbonPageGroup)
                    {
                        RibbonPageGroup casted = (RibbonPageGroup)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    else if (control is BaseLayoutItem)
                    {
                        BaseLayoutItem casted = (BaseLayoutItem)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    else if (control is GridColumn)
                    {
                        GridColumn casted = (GridColumn)control;
                        casted.Caption = LocalizeString(casted.Caption);
                    }
                    else if (control is NavElement)
                    {
                        NavElement casted = (NavElement)control;
                        casted.Caption = LocalizeString(casted.Caption);
                    }
                    else if (control is RibbonPage)
                    {
                        RibbonPage casted = (RibbonPage)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    else if (control is SearchLookUpEdit || control is CheckedComboBoxEdit)
                    {

                    }
                    else if (control is Control)
                    {
                        Control casted = (Control)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    else if (control is SimpleButton)
                    {
                        Control casted = (Control)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    if (control is BaseEdit)
                    {
                        BaseEdit editControl = (control as BaseEdit);
                        editControl.KeyDown += EditControl_KeyDown;
                    }
                }
            }
        }
        private void EditControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.None)
            {
                this.ParentFilterControl.Search();
            }
        }
        private Dictionary<BaseEdit, ExtraProperties> _extrProperties = new Dictionary<BaseEdit, ExtraProperties>();
        public bool CanExtend(object extendee)
        {
            return (extendee is BaseEdit);
        }
        public BaseSecondaryFilterControl()
        {
            InitializeComponent();
        }
        protected void DeleteValue(BaseEdit edit, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete && edit != null)
            {
                edit.EditValue = null;
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ParentFilterControl.Search();
        }
    }
}
