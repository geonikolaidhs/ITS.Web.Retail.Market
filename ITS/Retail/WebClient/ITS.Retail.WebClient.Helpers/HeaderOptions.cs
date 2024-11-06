using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Helpers
{
    
    public class HeaderOptions
    {
        public HtmlCustomButtonOptions SaveButton { get; set; }
        public HtmlCustomButtonOptions SaveAndPrintButton { get; set; }
        public HtmlCustomButtonOptions CancelButton { get; set; }
        public HtmlCustomButtonOptions SaveAndRecalculateButton {get; set;}
        public HtmlCustomButtonOptions RecalculateButton { get; set; }
                
        public string HeaderText { get; set; }
        public eViewType ViewType { get; set; }

        public string ContainerClass
        {
            get
            {
                if (ViewType == eViewType.PopUp)
                {
                    return "PopUpView";
                }
                else if (ViewType == eViewType.Modal)
                {
                    return "ModalView";
                }
                else if (ViewType == eViewType.Simple)
                {
                    return "SimpleView";
                }
                else { return "";}
            }
        }

        public HeaderOptions(string headerText) {
            this.SaveButton = new HtmlCustomButtonOptions();
            SaveButton.Visible = true;
            SaveButton.CCSClass = "button";
            SaveButton.Text = 
            SaveButton.Title = Resources.Save;
            SaveButton.OnClick = "ValidateForm";
            SaveButton.Name = "btnUpdate";
            SaveButton.UseSubmitBehavior = false;
            SaveButton.EncodeHtml = true;

            this.SaveAndPrintButton = new HtmlCustomButtonOptions();
            SaveAndPrintButton.Visible = false;
            SaveAndPrintButton.CCSClass = "button";
            SaveAndPrintButton.Text =
            SaveAndPrintButton.OnClick = "UpdateAndPrint";
            SaveAndPrintButton.Name = "btnUpdateAndPrint";
            SaveAndPrintButton.EncodeHtml = true;

            this.SaveAndRecalculateButton = new HtmlCustomButtonOptions();
            SaveAndRecalculateButton.Visible = false;
            SaveAndRecalculateButton.CCSClass = "button";
            SaveAndRecalculateButton.OnClick = "btnSaveAndRecalculate";
            SaveAndRecalculateButton.ClientEnabled = true;
            SaveAndRecalculateButton.Text = "";            
            SaveAndRecalculateButton.Name = "btnSaveAndRecalculate";
            SaveAndRecalculateButton.EncodeHtml = true;


            this.RecalculateButton = new HtmlCustomButtonOptions();
            RecalculateButton.Visible = false;
            RecalculateButton.Name = "btnRecalculate";
            RecalculateButton.CCSClass = "button";
            RecalculateButton.Text = "";
            RecalculateButton.OnClick = "Recalculate";
            RecalculateButton.EncodeHtml = true;
            

            this.CancelButton = new HtmlCustomButtonOptions();
            CancelButton.Visible = true;
            CancelButton.CCSClass = "button cancel";
            CancelButton.Text =
            CancelButton.Title = Resources.Close; //+ " <i class=\"fa fa-times fa-lg fa-custom\"></i>";
            CancelButton.OnClick = "btnCancelClick";
            CancelButton.Name = "btnCancel";
            CancelButton.EncodeHtml = true; 

            this.HeaderText = headerText;
            this.ViewType = eViewType.Modal;
        }

        public HeaderOptions()
        {
            // TODO: Complete member initialization
        }
    }
}
