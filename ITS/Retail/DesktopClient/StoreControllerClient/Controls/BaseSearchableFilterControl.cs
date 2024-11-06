using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System.Collections;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class BaseSearchableFilterControl : BaseFilterControl
    {
        IList LiveDataSource;

        public BaseSearchableFilterControl()
        {
            InitializeComponent();
        }


        public PersistentCriteriaEvaluationBehavior DefaultSearchMethod { get; set; }

        protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {
            //base.SearchMain(criteria, uow);//Maybe unecessary. I do not know for sure...
            GridControl.MainView = this.GridView;
            LiveDataSource = new XPCollection(DefaultSearchMethod, uow, PersistentType, criteria);
            GridControl.DataSource = LiveDataSource;// new XPServerCollectionSource(uow, PersistentType, criteria);
            GridControl.Invalidate();
            GridControl.Update();
        }
    }
}
