using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class CustomerCategoryInlineEdit : XtraLocalizedForm
    {
        protected CustomerAnalyticTree _CustomerAnalyticTree;
        protected CustomerAnalyticTree CustomerAnalyticTree { get { return _CustomerAnalyticTree; } }

        public CustomerCategoryInlineEdit(CustomerAnalyticTree analyticTree)
        {
            this._CustomerAnalyticTree = analyticTree;
            InitializeComponent();
        }

        private void InitializeDataSources()
        {
            this.trListCustomerCategories.DataSource = new XPCollection<CustomerCategory>(this.CustomerAnalyticTree.Session);
            this.trListCustomerCategories.KeyFieldName = "Oid";
            this.trListCustomerCategories.ParentFieldName = "ParentOid";
        }

        private void CustomerCategoryInlineEdit_Load(object sender, EventArgs e)
        {
            this.treeListColumn1.Caption = Resources.CustomerCategory;
            InitializeDataSources();
        }


        private void btnSaveCustomerCategory_Click(object sender, EventArgs e)
        {
            CustomerCategory currentCategory = (CustomerCategory)this.trListCustomerCategories.GetDataRecordByNode(this.trListCustomerCategories.FocusedNode);
            if(!CategoryAlreadyExists(this.CustomerAnalyticTree.Object, currentCategory))
            {
                this.CustomerAnalyticTree.Node = currentCategory;
                DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(Resources.DuplicateCustomerCategory, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCancelCustomerCategory_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool CategoryAlreadyExists(Customer customer, CustomerCategory category)
        {
            if(customer.CustomerAnalyticTrees.Count > 0)
            {
                foreach (CustomerAnalyticTree cat in customer.CustomerAnalyticTrees)
                {

                    if (cat.Node != null && cat.Node.GetNodeIDs().Contains(category.Oid))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CustomerCategoryInlineEdit_Shown(object sender, EventArgs e)
        {
            this.trListCustomerCategories.Focus();
        }
    }
}
