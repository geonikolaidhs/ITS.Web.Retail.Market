using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    [ProvideProperty("IsRequired", typeof(LayoutControlItem))]
    public partial class XtraLocalizedForm : XtraForm, IExtenderProvider
    {
        Dictionary<LayoutControlItem, bool> _extrProperties = new Dictionary<LayoutControlItem, bool>();
        // Do not remove. It is used by Winforms Designer
        public void SetIsRequired(LayoutControlItem edit, bool value)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, value);
            }
            _extrProperties[edit] = value;
        }

        // Do not remove. It is used by Winforms Designer
        public bool GetIsRequired(LayoutControlItem edit)
        {
            if (_extrProperties.ContainsKey(edit) == false)
            {
                _extrProperties.Add(edit, false);
            }
            return _extrProperties[edit];
        }


        public XtraLocalizedForm()
        {
            InitializeComponent();        
        }

        protected IEnumerable<Component> EnumerateComponents()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null
                   select component;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Text = LocalizeString(this.Text);
            IEnumerable<Component> components = EnumerateComponents();
            
            foreach (Component control in components)
            {

                if (control is TextEdit)
                {
                    (control as TextEdit).EnableAutoSelectAllOnFirstMouseUp();
                }

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
                 
                else if(control is BaseLayoutItem)
                {
                    BaseLayoutItem casted = (BaseLayoutItem)control;
                    casted.Text = LocalizeString(casted.Text);
                    if (control is LayoutControlItem)
                    {
                        LayoutControlItem innerCasted = (LayoutControlItem)control;
                        if (_extrProperties.ContainsKey(innerCasted) && _extrProperties[innerCasted])
                        {
                            innerCasted.Text = LocalizeString(innerCasted.Text.TrimEnd('*')) + "*";
                            innerCasted.AppearanceItemCaption.ForeColor = Color.IndianRed;
                            innerCasted.AppearanceItemCaption.Font = new Font(innerCasted.AppearanceItemCaption.Font.FontFamily,
                                                                              innerCasted.AppearanceItemCaption.Font.Size,
                                                                               FontStyle.Bold
                                                                             );
                        }

                    }
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
                else if (control is BaseView)
                {
                    ((BaseView)control).ViewCaption = LocalizeString(((BaseView)control).ViewCaption);
                }
                else if (control is TileItem)
                {
                    TileItem casted = (TileItem)control;
                    casted.Text = LocalizeString(casted.Text);
                }
                else if(control is BaseEdit && control is BaseCheckEdit == false)
                {
                    if (control is LookUpEdit || control is SearchLookUpEdit)
                    {
                        (control as LookUpEditBase).Properties.NullText = "";
                    }
                }                
                else if (control is TreeList)
                {
                    TreeList casted = (TreeList)control;
                    foreach (TreeListColumn col in casted.Columns)
                    {
                        col.Caption = LocalizeString(col.Caption);
                    }
                }
                else if (control is XtraTabControl)
                {
                    XtraTabControl tabControl = (XtraTabControl)control;
                    tabControl.AppearancePage.Header.Font = new Font("Tahoma", 9.25F);
                    tabControl.AppearancePage.Header.Options.UseFont = true;
                    tabControl.AppearancePage.HeaderActive.Font = new Font("Tahoma", 9.25F,FontStyle.Bold);
                    tabControl.AppearancePage.HeaderActive.Options.UseFont = true;
                }
                
                else if (control is Control)
                {
                    if (control.GetType().ToString() != "System.Windows.Forms.WebBrowser")
                    {
                        Control casted = (Control)control;
                        casted.Text = LocalizeString(casted.Text);
                    }
                    
                }
                else
                {
                    int i = 0;
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SplashScreenManager.CloseForm(false);
        }

        protected virtual string LocalizeString(string input)
        {

            string result = null;
            if(input.StartsWith("@@") && this.DesignMode == false)
            {
                 result = Resources.ResourceManager.GetString(input.TrimStart('@'));
                 if (string.IsNullOrEmpty(result))
                 {
                     Program.Logger.Debug(String.Format("Not Translated Message : {0}", input));
                 }
            }
            return String.IsNullOrWhiteSpace(result) ? input : result;
        }

        public bool CanExtend(object extendee)
        {
            return extendee is LayoutControlItem;
        }
    }
}
