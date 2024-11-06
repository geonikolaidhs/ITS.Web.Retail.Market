using System.Reflection;

namespace ITS.Retail.WebClient.Helpers
{
    public class ToolbarOptions
    {
        /// <summary>
        /// If at least one button is visible and ForceVisible is true or null, then returns true, else returns false
        /// </summary>
        public bool Visible
        {
            get
            {
                bool foundVisibleButton = false;
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    if (typeof(HtmlButtonOptions).IsAssignableFrom(property.PropertyType))
                    {
                        HtmlButtonOptions button = property.GetValue(this, null) as HtmlButtonOptions;
                        if (button.Visible)
                        {
                            foundVisibleButton = true;
                            break;
                        }
                    }
                }

                return foundVisibleButton && (ForceVisible == null ? true : ForceVisible.Value);
            }
        }

        public bool? ForceVisible { get; set; }

        public HtmlButtonOptions FilterButton { get; set; }
        public HtmlButtonOptions ShowHideMenu { get; set; }
        public HtmlButtonOptions PrintButton { get; set; }
        public HtmlButtonOptions ViewButton { get; set; }
        public HtmlButtonOptions NewButton { get; set; }
        public HtmlButtonOptions EditButton { get; set; }
        public HtmlButtonOptions DeleteButton { get; set; }
        public HtmlButtonOptions GridFilterButton { get; set; }
        public HtmlButtonOptions ExportToButton { get; set; }
        public HtmlButtonOptions ExportButton { get; set; }
        public HtmlButtonOptions ImportButton { get; set; }
        public HtmlButtonOptions TransformButton { get; set; }
        public HtmlButtonOptions CopyButton { get; set; }
        public HtmlButtonOptions UndoButton { get; set; }
        public HtmlButtonOptions OptionsButton { get; set; }
        public HtmlButtonOptions VariableValuesButton { get; set; }
        public HtmlCustomButtonOptions CustomButton { get; set; }
        public HtmlCustomButtonOptions PrintLabelButton { get; set; }
        public HtmlButtonOptions SendPaymentMethodsButton { get; set; }
        public HtmlButtonOptions SendItemsButton { get; set; }
        public HtmlButtonOptions IssueZ { get; set; }
        public HtmlButtonOptions IssueX { get; set; }
        public HtmlButtonOptions ClearAllItems { get; set; }
        public HtmlButtonOptions DailySales { get; set; }
        public HtmlButtonOptions DailyItemsSales { get; set; }
        public HtmlCustomButtonOptions MergeDocumentsButton { get; set; }

        public ToolbarOptions()
        {
            this.ShowHideMenu = new HtmlButtonOptions();
            this.ShowHideMenu.Visible = true;
            this.OptionsButton = new HtmlButtonOptions();
            this.OptionsButton.Visible = true;
            this.FilterButton = new HtmlButtonOptions();
            this.FilterButton.Visible = false;
            this.PrintButton = new HtmlButtonOptions();
            this.PrintButton.Visible = false;
            this.ViewButton = new HtmlButtonOptions();
            this.ViewButton.Visible = false;
            this.NewButton = new HtmlButtonOptions();
            this.NewButton.Visible = false;
            this.EditButton = new HtmlButtonOptions();
            this.EditButton.Visible = false;
            this.DeleteButton = new HtmlButtonOptions();
            this.DeleteButton.Visible = false;
            this.GridFilterButton = new HtmlButtonOptions();
            this.GridFilterButton.Visible = false;
            this.ExportToButton = new HtmlButtonOptions();
            this.ExportToButton.Visible = false;
            this.TransformButton = new HtmlButtonOptions();
            this.TransformButton.Visible = false;
            this.CopyButton = new HtmlButtonOptions();
            this.CopyButton.Visible = false;
            this.ExportButton = new HtmlButtonOptions();
            this.ExportButton.Visible = false;
            this.ImportButton = new HtmlButtonOptions();
            this.ImportButton.Visible = false;
            this.UndoButton = new HtmlButtonOptions();
            this.UndoButton.Visible = false;
            this.VariableValuesButton = new HtmlButtonOptions();
            this.VariableValuesButton.Visible = false;
            this.CustomButton = new HtmlCustomButtonOptions();
            this.CustomButton.Visible = false;
            this.CustomButton.UseSubmitBehavior = false;
            this.CustomButton.Title = "";
            this.PrintLabelButton = new HtmlCustomButtonOptions();
            this.PrintLabelButton.Visible = false;
            this.SendPaymentMethodsButton = new HtmlButtonOptions();
            this.SendPaymentMethodsButton.Visible = false;
            this.SendItemsButton = new HtmlButtonOptions();
            this.SendItemsButton.Visible = false;
            this.InitializeDefaultEvents();
            this.IssueZ = new HtmlButtonOptions();
            this.IssueZ.Visible = false;
            this.IssueX = new HtmlButtonOptions();
            this.IssueX.Visible = false;
            this.ClearAllItems = new HtmlButtonOptions();
            this.ClearAllItems.Visible = false;
            this.DailySales = new HtmlButtonOptions();
            this.DailySales.Visible = false;
            this.DailyItemsSales = new HtmlButtonOptions();
            this.DailyItemsSales.Visible = false;
            this.MergeDocumentsButton = new HtmlCustomButtonOptions();
            this.MergeDocumentsButton.Visible = false;
            //DailyItemsSales
        }

        public void InitializeDefaultEvents()
        {
            this.FilterButton.OnClick = "toolbarHideFilters";
        }
    }
}
