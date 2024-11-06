using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars.Navigation;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraSplashScreen;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using ITS.Retail.Model;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    [ProvideProperty("BoundPropertyName", typeof(BaseEdit))]
    [ProvideProperty("BoundFieldName", typeof(BaseEdit))]
    public partial class BaseFilterControl : XtraUserControl, IExtenderProvider
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
        public BaseSecondaryFilterControl SecondaryFilterControl { get; set; }
        public bool Expandable { get; set; }
        public delegate void SearchEventHandler(BaseFilterControl sender, SearchEventArgs e);

        public event SearchEventHandler CreateUnitOfWork;

        public event EventHandler SearchComplete;
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

        public BaseSearchFilter SearchFilter { get; set; }

        public Type PersistentType { get; set; }

        //public Type ViewModelType { get; set; }

        //public BindingList<IPersistableViewModel> BindingListForViewModel { get; set; }

        public ColumnView GridView { get; set; }

        public GridControl GridControl { get; set; }


        public virtual void BeforeBuildCriteria()
        {

        }

        public virtual void BeforeSearch(ref CriteriaOperator searchCriteria)
        {

        }

        public void Search()
        {
            try
            {
                BeforeBuildCriteria();
                CriteriaOperator criteria;
                if (PersistentType != null && PersistentType.FullName == "ITS.Retail.Model.Item")
                    criteria = this.SearchFilter.BuildCriteria();
                else
                    criteria = this.SearchFilter.BuildCriteria();
                BeforeSearch(ref criteria);
                UnitOfWork uow = null;
                SearchEventArgs searchEventArgs = new SearchEventArgs();
                if (this.CreateUnitOfWork != null)
                {
                    this.CreateUnitOfWork(this, searchEventArgs);
                    uow = searchEventArgs.UnitOfWork;
                }
                if (uow == null)
                {
                    throw new Exception("CreateUnitOfWork should be consumed and return a valid UnitOfWork");
                }
                if (GridControl.MainView != this.GridView)
                {
                    this.GridView.ActiveFilterCriteria = null;
                }
                SplashScreenManager.ShowForm(this.GridControl.FindForm(), typeof(ITSWaitForm), true, true, false);
                SearchMain(criteria, uow);
                SplashScreenManager.CloseForm(false);
                if (SearchComplete != null)
                {
                    SearchComplete(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected virtual void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {
            GridControl.MainView = this.GridView;
            if (PersistentType.FullName == "ITS.Retail.Model.Item")
            {
                XPQuery<Item> items = new XPQuery<Item>(uow);
                CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                IQueryable<Item> filteredData = (new XPQuery<Item>(uow)).AppendWhere(converter, criteria) as IQueryable<Item>;
                XPQuery<ItemExtraInfo> extrainfos = new XPQuery<ItemExtraInfo>(uow);
                XPQuery<Barcode> barcode = new XPQuery<Barcode>(uow);
                XPQuery<ItemBarcode> itemBarcode = new XPQuery<ItemBarcode>(uow);
                IQueryable<dynamic> list = from e in filteredData
                                               //join b in barcode on e.DefaultBarcode equals b
                                           join i in itemBarcode on e.DefaultBarcode equals i.Barcode
                                           join o in extrainfos on new { e.Oid, StoreOid = Program.Settings.StoreControllerSettings.Store.Oid } equals new { o.Item.Oid, StoreOid = o.Store.Oid }
                                           select new { Item = e, ItemExtraInfoDescription = o == null ? "" : o.Description, MeasurementUnit = i == null ? "" : (i.MeasurementUnit.Description == null ? "" : i.MeasurementUnit.Description) }
                           ;
                GridControl.DataSource = list;
            }

            else
            {
                GridControl.DataSource = new XPServerCollectionSource(uow, PersistentType, criteria);
            }
            GridControl.Invalidate();
            GridControl.Update();
        }

        public BaseFilterControl()
        {
            InitializeComponent();
            Expandable = true;
        }

        private bool _LocalizationHasRun = false;
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
                        pair.Key.DataBindings.Add(pair.Value.BoundPropertyName, this.SearchFilter, pair.Value.BoundFieldName, true, DataSourceUpdateMode.OnPropertyChanged);
                    }
                }

                IEnumerable<Component> enumerateComponents = EnumerateComponents();
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
                this.Search();
            }
        }

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

        protected DevExpress.XtraLayout.LayoutControl layoutControlFilter;
        protected DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupFilter;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControlFilter = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroupFilter = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlFilter.Location = new System.Drawing.Point(0, 0);
            this.layoutControlFilter.Name = "layoutControlFilter";
            this.layoutControlFilter.Root = this.layoutControlGroupFilter;
            this.layoutControlFilter.Size = new System.Drawing.Size(164, 138);
            this.layoutControlFilter.TabIndex = 0;
            this.layoutControlFilter.Text = "layoutControl1";
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupFilter.GroupBordersVisible = false;
            this.layoutControlGroupFilter.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupFilter.Name = "layoutControlGroupFilter";
            this.layoutControlGroupFilter.Size = new System.Drawing.Size(164, 138);
            this.layoutControlGroupFilter.TextVisible = false;
            // 
            // BaseFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControlFilter);
            this.Name = "BaseFilterControl";
            this.Size = new System.Drawing.Size(164, 138);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private void simpleButtonSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        public bool CanExtend(object extendee)
        {
            return (extendee is BaseEdit);
        }

        public virtual int Lines { get { return 3; } }

        private Dictionary<BaseEdit, ExtraProperties> _extrProperties = new Dictionary<BaseEdit, ExtraProperties>();

        private void simpleButtonReset_Click(object sender, EventArgs e)
        {
            SearchFilter.Reset();
        }

        protected void DeleteValue(BaseEdit edit, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete && edit != null)
            {
                edit.EditValue = null;
            }
        }
    }
}
