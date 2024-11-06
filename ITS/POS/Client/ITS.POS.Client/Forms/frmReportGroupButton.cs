using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.UserControls;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Forms
{
    public partial class frmReportGroupButton : frmGroupButton
    {

        public BaseObj SelectedObject { get; set; }


        public frmReportGroupButton()
        {
            InitializeComponent();
        }


        public void InitializePosReports(ButtonProperties buttonReportProperties, int numberOfRows, int numberOfCols, bool remainOpenAfterClick)
        {
            PrepareForm(numberOfRows, numberOfCols, remainOpenAfterClick);
            ControlsToShow.AddRange(CreatePosReportButtons(buttonReportProperties));
            ShowButtonPage();
        }




        private List<ucButton> CreatePosReportButtons(ButtonProperties buttonReportProperties)
        {
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            XPCollection<PosOposReportSettings> posReportsettings = new XPCollection<PosOposReportSettings>(sessionManager.GetSession<PosOposReportSettings>());

            CriteriaOperator criteria = CriteriaOperator.And(new InOperator("Oid", posReportsettings.Select(x => x.PrintFormat)));
            XPCollection<PosReport> posReports = new XPCollection<PosReport>(sessionManager.GetSession<PosReport>(), criteria);

            List<ucButton> listToReturn = new List<ucButton>();
            foreach (PosReport posReport in posReports)
            {
                ucPosReportButton btn = new ucPosReportButton();
                btn.Name = Guid.NewGuid().ToString();
                btn.Dock = DockStyle.Fill;
                btn.ButtonText = posReport.Description;
                btn.posReport = posReport;
                btn.RemainOpenAfterClick = this.RemainOpenAfterClick;

                listToReturn.Add(btn);

            }


            listToReturn.ForEach(x =>
            {
                buttonReportProperties.Apply(x.Button);

                //x.Button.Click += new EventHandler(this.btnClicked);

            });
            return listToReturn;
        }


        private void btnClicked(Object sender, EventArgs eventArgs)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SelectedObject = sender as BaseObj;
            this.Close();

        }

    }
}
