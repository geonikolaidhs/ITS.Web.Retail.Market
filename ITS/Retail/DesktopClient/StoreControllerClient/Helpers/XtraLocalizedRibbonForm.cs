using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraNavBar;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList.Columns;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public class XtraLocalizedRibbonForm : RibbonForm
    {
        public XtraLocalizedRibbonForm()
            : base()
        {

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Text = LocalizeString(this.Text);
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
                else if (control is BaseView)
                {
                    ((BaseView)control).ViewCaption = LocalizeString(((BaseView)control).ViewCaption);
                }
                else if (control is Control)
                {
                    Control casted = (Control)control;
                    casted.Text = LocalizeString(casted.Text);
                }
                else if (control is TreeListColumn)
                {
                    TreeListColumn casted = (TreeListColumn)control;
                    casted.Caption = LocalizeString(casted.Caption);
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
            if (input.StartsWith("@@"))
            {
                result = Resources.ResourceManager.GetString(input.TrimStart('@'));
                if (string.IsNullOrEmpty(result))
                {
                    Program.Logger.Debug(String.Format("Not Translated Message : {0}", input));
                }
            }
            return String.IsNullOrWhiteSpace(result) ? input : result;
        }

        protected IEnumerable<Component> EnumerateComponents()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null
                   select component;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraLocalizedRibbonForm));
            this.SuspendLayout();
            // 
            // XtraLocalizedRibbonForm
            // 
            this.ClientSize = new System.Drawing.Size(290, 295);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XtraLocalizedRibbonForm";
            this.ResumeLayout(false);

        }
    }
}
