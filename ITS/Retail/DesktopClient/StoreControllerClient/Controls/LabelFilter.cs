using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model;
using DevExpress.XtraGrid.Views.Grid;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class LabelFilter : BaseFilterControl
    {
        public LabelFilter()
        {
            InitializeComponent();
            this.Expandable = false;
            SecondaryFilterControl = new LabelFilterSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;
            this.SearchFilter = new LabelSearchCriteria();
            this.SearchFilter.Set();
        }
        public override int Lines
        {
            get
            {
                return 1;
            }
        }

        public GridView DetailGridView { get; set; }

        protected override void SearchMain(CriteriaOperator criteria, UnitOfWork uow)
        {
            //base.SearchMain(criteria, uow);
            criteria = CriteriaOperator.And(
                criteria,
                    new BinaryOperator("Store.Oid", Program.Settings.StoreControllerSettings.Store.Oid),
                    new BinaryOperator("DocumentType.Type", eDocumentType.TAG)
                );
            XPCollection<DocumentHeader> headers = new XPCollection<DocumentHeader>(uow, criteria);
            GridControl.MainView = this.GridView;
            GridControl.LevelTree.Nodes.Clear();
            GridControl.LevelTree.Nodes.Add("Details", DetailGridView);
            GridControl.DataSource = headers;
            GridControl.Refresh();

        }
    }
}
