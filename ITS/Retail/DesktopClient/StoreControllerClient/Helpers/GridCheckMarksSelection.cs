using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;
using System.Data;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    
    public class GridCheckMarksSelection
    {
        protected GridView _view;
        protected ArrayList selection;
        GridColumn column;
        RepositoryItemCheckEdit edit;
        const int CheckboxIndent = 4;
        public GridCheckMarksSelection()
        {
            selection = new ArrayList();
        }
        public GridCheckMarksSelection(GridView view)
            : this()
        {
            View = view;
        }
        public GridView View
        {
            get { return _view; }
            set
            {
                if (_view != value)
                {
                    Detach();
                    Attach(value);
                }
            }
        }
        public GridColumn CheckMarkColumn { get { return column; } }
        public int SelectedCount { get { return selection.Count; } }
        public object GetSelectedRow(int index)
        {
            return selection[index];
        }
        public int GetSelectedIndex(object row)
        {
            foreach (object record in selection)
                if ((record as DataRowView).Row == (row as DataRowView).Row)
                    return selection.IndexOf(record);
            return selection.IndexOf(row);
        }
        public void ClearSelection(GridView view)
        {
            selection.Clear();
            Invalidate(view);
        }
        public void SelectAll(GridView view)
        {
            selection.Clear();
            // fast (won't work if the grid is filtered)
            //if(_view.DataSource is ICollection)
            //	selection.AddRange(((ICollection)_view.DataSource));
            //else
            // slow:
            for (int i = 0; i < view.DataRowCount; i++)
                selection.Add(view.GetRow(i));
            Invalidate(view);
        }
        public void SelectGroup(int rowHandle, bool select, GridView view)
        {
            if (IsGroupRowSelected(rowHandle, view) && select) return;
            for (int i = 0; i < view.GetChildRowCount(rowHandle); i++)
            {
                int childRowHandle = view.GetChildRowHandle(rowHandle, i);
                if (view.IsGroupRow(childRowHandle))
                    SelectGroup(childRowHandle, select, view);
                else
                    SelectRow(childRowHandle, select, false, view);
            }
            Invalidate(view);
        }
        void SelectRow(int rowHandle, bool select, bool invalidate, GridView view)
        {
            if (IsRowSelected(rowHandle, view) == select) return;
            object row = view.GetRow(rowHandle);
            if (select)
                selection.Add(row);
            else
                selection.Remove(row);
            if (invalidate)
            {
                Invalidate(view);
            }
        }
        void SelectRow(object row, bool select, bool invalidate, GridView view)
        {
            if (IsRowSelected(row, view) == select) return;
            if (select)
                selection.Add(row);
            else
                selection.Remove(row);
            if (invalidate)
            {
                Invalidate(view);
            }
        }
        public void SelectRow(int rowHandle, bool select, GridView view)
        {
            SelectRow(rowHandle, select, true, view);
        }
        public void SelectRow(object row, bool select, GridView view)
        {
            SelectRow(row, select, true, view);
        }
        public void InvertRowSelection(int rowHandle, GridView view)
        {
            if (view.IsDataRow(rowHandle))
            {
                SelectRow(rowHandle, !IsRowSelected(rowHandle, view), view);
            }
            if (view.IsGroupRow(rowHandle))
            {
                SelectGroup(rowHandle, !IsGroupRowSelected(rowHandle, view), view);
            }
        }
        public bool IsGroupRowSelected(int rowHandle, GridView view)
        {
            for (int i = 0; i < view.GetChildRowCount(rowHandle); i++)
            {
                int row = _view.GetChildRowHandle(rowHandle, i);
                if (view.IsGroupRow(row))
                {
                    if (!IsGroupRowSelected(row, view)) return false;
                }
                else
                    if (!IsRowSelected(row, view)) return false;
            }
            return true;
        }
        public bool IsRowSelected(int rowHandle, GridView view)
        {
            if (view.IsGroupRow(rowHandle))
                return IsGroupRowSelected(rowHandle, view);

            object row = view.GetRow(rowHandle);

            return GetSelectedIndex(row) != -1;
        }
        public bool IsRowSelected(object row, GridView view)
        {
            return GetSelectedIndex(row) != -1;
        }
        protected virtual void Attach(GridView view)
        {
            if (view == null) return;
            selection.Clear();
            this._view = view;
            edit = view.GridControl.RepositoryItems.Add("CheckEdit") as RepositoryItemCheckEdit;
            column = view.Columns.Add();
            column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            column.Visible = true;
            column.VisibleIndex = 0;
            column.FieldName = "CheckMarkSelection";
            column.Caption = "Mark";
            column.OptionsColumn.ShowCaption = false;
            column.OptionsColumn.AllowEdit = false;
            column.OptionsColumn.AllowSize = false;
            column.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            column.Width = GetCheckBoxWidth();
            column.ColumnEdit = edit;

            view.Click += new EventHandler(View_Click);
            view.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
            view.CustomDrawGroupRow += new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
            view.CustomUnboundColumnData += new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
            view.KeyDown += new KeyEventHandler(view_KeyDown);
            view.RowStyle += new RowStyleEventHandler(view_RowStyle);
        }
        public virtual void DetailViewAttach(GridView view)
        {
            if (view == null) return;
            this._view = view;
            _view.BeginUpdate();
            try
            {
                column = view.Columns["CheckMarkSelection"];
                if (column == null)
                {
                    selection.Clear();
                    edit = view.GridControl.RepositoryItems.Add("CheckEdit") as RepositoryItemCheckEdit;
                    column = view.Columns.Add();
                    column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                    column.Visible = true;
                    column.VisibleIndex = 0;
                    column.FieldName = "CheckMarkSelection";
                    column.Caption = "Mark";
                    column.OptionsColumn.ShowCaption = false;
                    column.OptionsColumn.AllowEdit = false;
                    column.OptionsColumn.AllowSize = false;
                    column.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
                    column.Width = GetCheckBoxWidth();
                    column.ColumnEdit = edit;
                }
                edit = view.Columns["CheckMarkSelection"].ColumnEdit as RepositoryItemCheckEdit;//view.GridControl.RepositoryItems.Add("CheckEdit") as RepositoryItemCheckEdit;
                view.Click += new EventHandler(View_Click);
                view.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
                view.CustomDrawGroupRow += new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
                view.CustomUnboundColumnData += new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
                view.KeyDown += new KeyEventHandler(view_KeyDown);
                view.RowStyle += new RowStyleEventHandler(view_RowStyle);
            }
            finally
            {
                _view.EndUpdate();
            }
        }
        protected virtual void Detach()
        {
            if (_view == null) return;
            if (column != null)
                column.Dispose();
            if (edit != null)
            {
                _view.GridControl.RepositoryItems.Remove(edit);
                edit.Dispose();
            }

            _view.Click -= new EventHandler(View_Click);
            _view.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
            _view.CustomDrawGroupRow -= new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
            _view.CustomUnboundColumnData -= new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
            _view.KeyDown -= new KeyEventHandler(view_KeyDown);
            _view.RowStyle -= new RowStyleEventHandler(view_RowStyle);

            _view = null;
        }
        public virtual void DetailViewDetach()
        {
            if (_view == null) return;

            _view.Click -= new EventHandler(View_Click);
            _view.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
            _view.CustomDrawGroupRow -= new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
            _view.CustomUnboundColumnData -= new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
            _view.KeyDown -= new KeyEventHandler(view_KeyDown);
            _view.RowStyle -= new RowStyleEventHandler(view_RowStyle);
            _view = null;
        }
        protected int GetCheckBoxWidth()
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            int width = 0;
            GraphicsInfo.Default.AddGraphics(null);
            try
            {
                width = info.CalcBestFit(GraphicsInfo.Default.Graphics).Width;
            }
            finally
            {
                GraphicsInfo.Default.ReleaseGraphics();
            }
            return width + CheckboxIndent * 2;
        }
        protected void DrawCheckBox(Graphics g, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }
        void Invalidate(GridView view)
        {
            view.CloseEditor();
            view.BeginUpdate();
            view.EndUpdate();
        }
        void view_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "Mark")
            {
                if (e.IsGetData)
                    e.Value = /*IsRowSelected((view.DataSource as IList)[e.ListSourceRowIndex],view);*/  IsRowSelected(view.GetRowHandle(e.ListSourceRowIndex), view);
                else
                    SelectRow((view.DataSource as IList)[e.ListSourceRowIndex], (bool)e.Value, view);
            }
        }
        void view_KeyDown(object sender, KeyEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.Caption != "Mark" || e.KeyCode != Keys.Space) return;
            InvertRowSelection(view.FocusedRowHandle, view);
        }
        void View_Click(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo info;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            Point viewOffset = view.GetViewInfo().Bounds.Location;
            //pt.Offset(-viewOffset.X, -viewOffset.Y);
            info = view.CalcHitInfo(pt);
            if (info.Column != null && info.Column.Caption == "Mark")
            {
                if (info.InColumn)
                {
                    if (SelectedCount == view.DataRowCount)
                        ClearSelection(view);
                    else
                        SelectAll(view);
                }
                if (info.InRowCell)
                {
                    InvertRowSelection(info.RowHandle, view);
                }
            }
            if (info.InRow && view.IsGroupRow(info.RowHandle) && info.HitTest != GridHitTest.RowGroupButton)
            {
                InvertRowSelection(info.RowHandle, view);
            }
        }
        void View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {

            if (e.Column != null && e.Column.Caption == "Mark")
            {
                GridView view = sender as GridView;
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                DrawCheckBox(e.Graphics, e.Bounds, SelectedCount == view.DataRowCount);
                e.Handled = true;
            }
        }
        void View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info;
            info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            info.GroupText = "         " + info.GroupText.TrimStart();
            e.Info.Paint.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds);
            e.Painter.DrawObject(e.Info);

            Rectangle r = info.ButtonBounds;
            r.Offset(r.Width + CheckboxIndent * 2 - 1, 0);
            DrawCheckBox(e.Graphics, r, IsGroupRowSelected(e.RowHandle, view));
            e.Handled = true;
        }
        void view_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (IsRowSelected(e.RowHandle, view))
            {
                e.Appearance.BackColor = SystemColors.Highlight;
                e.Appearance.ForeColor = SystemColors.HighlightText;
            }
        }
    }
}
