using DevExpress.XtraEditors;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmGroupButton : XtraForm, IPOSForm
    {
        public frmGroupButton()
        {
            InitializeComponent();
            CurrentPage = 0;
            ControlsToShow = new List<ucButton>();
            ControlGroupStack = new Stack<List<ucButton>>();
        }

        public IPosKernel Kernel { get; set; }

        private GroupActionButtonPositionCollection ControlDescription;

        protected int CurrentPage { get; set; }
        protected int NumberOfRows { get; set; }
        protected int NumberOfCols { get; set; }

        protected bool RemainOpenAfterClick { get; set; }

        protected int PageSize
        {
            get
            {
                return NumberOfCols * NumberOfRows;
            }
        }

        protected int MaxPages
        {
            get
            {
                return (int)Math.Ceiling(ControlsToShow.Count * 1.0 / PageSize);
            }
        }

        protected List<ucButton> ControlsToShow { get; set; }

        protected Stack<List<ucButton>> ControlGroupStack { get; set; }

        public virtual void InitializeLayout(GroupActionButtonPositionCollection controlDescription,
            int numberOfRows, int numberOfCols, ButtonProperties defaultButtonProperties, bool remainOpenAfterClick)
        {
            PrepareForm(numberOfRows, numberOfCols, remainOpenAfterClick);
            this.ControlDescription = controlDescription;

            if (ControlDescription != null && ControlDescription.Count > 0)
            {
                this.SuspendLayout();
                //Prepare TableLayout

                foreach (GroupActionButtonPosition ctrlDescription in ControlDescription)
                {
                    ucButton ctrl = ctrlDescription.ConstructUIControl(this, defaultButtonProperties);
                    ctrl.Dock = DockStyle.Fill;
                    ctrl.Name = Guid.NewGuid().ToString();
                    if (ctrl is ucGroupButton == false && RemainOpenAfterClick == false)
                    {
                        ctrl.Button.Click += Ctrl_Click;
                    }
                    ControlsToShow.Add(ctrl);
                }
                this.ResumeLayout();
                ShowButtonPage();
            }
        }

        protected void PrepareForm(int numberOfRows, int numberOfCols, bool remainOpenAfterClick)
        {
            if (ControlsToShow.Count > 0)
            {
                ControlGroupStack.Push(ControlsToShow);
                ControlsToShow = new List<ucButton>();
            }
            CurrentPage = 0;
            this.RemainOpenAfterClick = remainOpenAfterClick;
            this.NumberOfCols = numberOfCols;
            this.NumberOfRows = numberOfRows;
        }

        protected void ShowButtonPage()
        {
            this.SuspendLayout();
            btnPrevious.Enabled = (CurrentPage != 0);

            this.tableLayout.Controls.Clear();
            this.tableLayout.RowStyles.Clear();
            this.tableLayout.ColumnStyles.Clear();

            float colPercentage = 100F / NumberOfCols;
            float rowPercentage = 100F / NumberOfRows;
            tableLayout.ColumnCount = NumberOfCols;
            tableLayout.RowCount = NumberOfRows;
            for (int i = 0; i < NumberOfCols; i++)
            {
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, colPercentage));
            }
            for (int i = 0; i < NumberOfRows; i++)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, colPercentage));
            }

            var controlsToAdd = ControlsToShow.Skip(CurrentPage * PageSize);
            btnNext.Enabled = false;
            if (controlsToAdd.Count() > PageSize)
            {
                controlsToAdd = controlsToAdd.Take(PageSize);
                btnNext.Enabled = true;
            }
            foreach (ucButton btn in controlsToAdd)
            {
                tableLayout.Controls.Add(btn);
            }
            btnUp.Enabled = ControlGroupStack.Count != 0;
            this.lblPages.Text = string.Format("{0}/{1}", CurrentPage + 1, MaxPages);
            this.ResumeLayout();
        }

        protected void Ctrl_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Kernel != null)
            {
                IControlLocalizer controlLocalizer = Kernel.GetModule<IControlLocalizer>();
                controlLocalizer.LocalizeControl(this);
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            do
            {
                DisposeCurrentControls();
                if (ControlGroupStack.Count > 0)
                {
                    ControlsToShow = ControlGroupStack.Pop();
                }
            } while (ControlsToShow.Count > 0);
            if (Kernel != null)
            {
                IControlLocalizer controlLocalizer = Kernel.GetModule<IControlLocalizer>();
                controlLocalizer.ClearDisposedObjectsFromCache(false);
            }
        }

        public void Initialize(IPosKernel kernel)
        {
            this.Kernel = kernel;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
                this.ShowButtonPage();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < this.MaxPages)
            {
                CurrentPage++;
                this.ShowButtonPage();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            DisposeCurrentControls();
            ControlsToShow = ControlGroupStack.Pop();
            CurrentPage = 0;
            ShowButtonPage();
        }

        private void DisposeCurrentControls()
        {
            tableLayout.Controls.Clear();
            ControlsToShow.ForEach(x => x.Dispose());
            ControlsToShow.Clear();
        }
    }

}
