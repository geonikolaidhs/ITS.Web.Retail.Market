using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ITS.Retail.WebClient.Helpers
{
    public class WizardStepOptions
    {
        public HtmlButtonOptions NextButton { get; protected set; }
        public HtmlButtonOptions PreviousButton { get; protected set; }
        public HtmlButtonOptions CancelButton { get; protected set; }
        public HtmlButtonOptions FinishButton { get; protected set; }

        public Unit Width { get; set; }
        public Unit Height { get; set; }
        public string HeaderText { get; set; }
        public string StepHeaderText { get; set; }
        public string BodyPartialView { get; set; }
        public string HeaderExtraHtml { get; set; }
        public string Name { get; set; }
        public string ErrorText { get; set; }

        public WizardStepOptions()
        {
            this.Name = "WizardStep";
            this.NextButton = new HtmlButtonOptions();
            this.NextButton.Text = Resources.Next;
            this.NextButton.Visible = true;
            this.NextButton.Name = "btnWizardNext";

            this.PreviousButton = new HtmlButtonOptions();
            this.PreviousButton.Text = Resources.Previous;
            this.PreviousButton.Visible = true;
            this.PreviousButton.Name = "btnWizardPrevious";

            this.CancelButton = new HtmlButtonOptions();
            this.CancelButton.Text = Resources.Close; //+ " <i class=\"fa fa-times fa-lg fa-custom\"></i>";
            this.CancelButton.EncodeHtml = false;
            this.CancelButton.Visible = false;
            this.CancelButton.Name = "btnWizardCancel";

            this.FinishButton = new HtmlButtonOptions();
            this.FinishButton.Text = Resources.Finish;
            this.FinishButton.Visible = false;
            this.FinishButton.Name = "btnWizardFinish";
        }
    }
}
