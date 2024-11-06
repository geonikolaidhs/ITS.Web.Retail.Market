using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Extensions;

namespace ITS.POS.Client.UserControls
{
    public partial class ucCustomEnumerationGrid : ucDataGridViewExtension
    {
        private static string KEY_FIELD = "Oid";
        private static string VALUE_FIELD = "Description";
        public static int CUSTOM_ENUMERATION_HEIGHT = 100;

        public Guid SelectedValue { get; set; }
        private int SelectedRowIndex { get; set; }

        public ucCustomEnumerationGrid()
        {
            InitializeComponent();

            this.Columns.Clear();

            DataGridViewColumn oidColumn = new DataGridViewTextBoxColumn();
            oidColumn.DataPropertyName = KEY_FIELD;
            oidColumn.Name = KEY_FIELD;
            oidColumn.Visible = false;
            this.Columns.Add(oidColumn);

            DataGridViewColumn descriptionColumn = new DataGridViewTextBoxColumn();
            descriptionColumn.DataPropertyName = VALUE_FIELD;
            descriptionColumn.Name = VALUE_FIELD;
            descriptionColumn.HeaderText = POS.Resources.POSClientResources.DESCRIPTION;
            this.Columns.Add(descriptionColumn);

            this.Width = 80;
            this.Height = CUSTOM_ENUMERATION_HEIGHT;
            this.AutoSize = true;

            this.SelectionChanged += SelectionChangedEvent;
            if ( this.Rows.Count > 0 )
            {
                this.Rows[0].Selected = true;
            }
            this.SelectedRowIndex = 0;
        }

        private void SelectionChangedEvent(object sender, EventArgs e)
        {
            if ( this.SelectedRows.Count == 1 )
            {
                this.SelectedValue = (Guid)this.SelectedRows[0].Cells[KEY_FIELD].Value;
                this.CurrentCell = this.SelectedRows[0].Cells[VALUE_FIELD];
            }
        }

        public void MoveToNextRow(GridNavigationDirection gridNavigationDirection)
        {
            if (this.SelectedRows.Count == 1)
            {
                this.SelectedRowIndex = this.SelectedRows[0].Index;
            }
            switch (gridNavigationDirection)
            {
                case GridNavigationDirection.DOWN:
                    this.SelectedRowIndex++;
                    if (this.SelectedRowIndex >= this.RowCount)
                    {
                        this.SelectedRowIndex = this.RowCount - 1;
                    }
                    break;
                case GridNavigationDirection.UP:
                    this.SelectedRowIndex--;
                    if (this.SelectedRowIndex < 0)
                    {
                        this.SelectedRowIndex = 0;
                    }
                    break;
                default:
                    break;
            }
            for (int counter = 0; counter < this.Rows.Count; counter++)
            {
                this.Rows[counter].Selected = counter == this.SelectedRowIndex;
            }
        }
    }
}
