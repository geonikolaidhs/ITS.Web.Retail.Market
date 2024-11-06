using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    public partial class ucReportGroupButton : ucGroupButton
    {
        public ucReportGroupButton()
        {
            this.InitializeComponent();
            this.ReportButtonProperties = new ButtonProperties()
            {
                BackColor = this.BackColor,
                Font = this.Font,
                ForeColor = this.ForeColor
            };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override GroupActionButtonPositionCollection GroupActionButtonPosition
        {
            get
            {
                return base.GroupActionButtonPosition;
            }

            set
            {
                base.GroupActionButtonPosition = value;
            }
        }

        //public bool PrintItemCode { get; set; }

        //public string ItemCategoryCode { get; set; }

        public ButtonProperties ReportButtonProperties { get; set; }

        protected override void Button_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is frmMainBase)
            {
                using (frmReportGroupButton frmReportGroup = new frmReportGroupButton())
                {

                    frmReportGroup.Initialize(this.Kernel);
                    frmReportGroup.InitializePosReports(DefaultButtonProperties, this.NumberOfRows, this.NumberOfColumns, this.RemainOpenAfterClick);
                    ArrangeButtonSizeProperties(frmReportGroup);

                    frmReportGroup.ShowDialog(this.FindForm());
                }
            }
            else if (this.FindForm() is frmReportGroupButton)
            {
                frmReportGroupButton frmGroup = (frmReportGroupButton)this.FindForm();
                frmGroup.InitializePosReports(DefaultButtonProperties, this.NumberOfRows, this.NumberOfColumns, this.RemainOpenAfterClick);
            }
        }

    }




}
