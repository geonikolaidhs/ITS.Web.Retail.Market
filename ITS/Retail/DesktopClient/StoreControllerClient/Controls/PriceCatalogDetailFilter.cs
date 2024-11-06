using System.Collections.Generic;
using ITS.Retail.Common.ViewModel;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class PriceCatalogDetailFilter : BaseSearchableFilterControl
    {
        public PriceCatalogDetailFilter()
        {
            InitializeComponent();
            CreateSearchFilter();
            this.SearchFilter.Set();
            SecondaryFilterControl = new PriceCatalogDetailSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;
            SetLookups();
        }

        protected virtual void CreateSearchFilter()
        {
            if (!DesignMode)
            {
                this.SearchFilter = new PriceCatalogDetailSearchFilter() { Owner = Program.Settings.StoreControllerSettings.Owner.Oid };
            }
        }
        private void SetLookups()
        {
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            this.lueSeasonality.Properties.DataSource = new XPCollection<Seasonality>(Program.Settings.ReadOnlyUnitOfWork, ownerCriteria);
            this.lueSeasonality.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueSeasonality.Properties.ValueMember = "Oid";

            this.lueIsActive.Properties.DataSource = new Dictionary<string, bool?>() { { "", null }, { Resources.Yes, true }, { Resources.No, false } };
            this.lueIsActive.Properties.Columns.Add(new LookUpColumnInfo("Key", Resources.Value));
            this.lueIsActive.Properties.DisplayMember = "Key";
            this.lueIsActive.Properties.ValueMember = "Value";

            this.lueBuyer.Properties.DataSource = new XPCollection<Buyer>(Program.Settings.ReadOnlyUnitOfWork, ownerCriteria);
            this.lueBuyer.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueBuyer.Properties.ValueMember = "Oid";
        }

        private void textEditInsertedDateFilter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueSeasonality_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void lueBuyer_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void textEditUpdatedOn_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
