using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ITS.Retail.WebClient.Helpers
{
    public class DialogOptions
    {
        public bool RenderDialog { get; set; }
        public List<string> Arguments { get; set; }
        public string Name { get; set; }
        public bool AdjustSizeOnInit { get; set; }
        public string CloseUpEvent { get; set; }
        public HtmlButtonOptions OKButton { get; set; }
        public HtmlButtonOptions CancelButton { get; set; }
        public Unit Width { get; set; }
        public Unit Height { get; set; }
        public string HeaderText { get; set; }

        public string HeaderExtraHtml { get; set; }
        public string BodyPartialView { get; set; }
        public string OnShownEvent { get; set; }

        public DialogOptions()
        {
            this.Name = "Dialog";
            this.RenderDialog = false;
            this.AdjustSizeOnInit = true;
            this.OKButton = new HtmlButtonOptions();
            this.CancelButton = new HtmlButtonOptions();
            this.OKButton.Name = "btnDialogOK";
            this.OKButton.Visible = true;
            this.CancelButton.Name = "btnDialogCancel";
            this.CancelButton.Visible = true;
            this.Width = 400;
            this.Height = 200;
            //Paramenters = new List<string>();

        }
    }
}
