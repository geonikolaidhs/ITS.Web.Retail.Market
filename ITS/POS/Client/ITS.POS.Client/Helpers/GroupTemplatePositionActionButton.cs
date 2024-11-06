using ITS.POS.Client.UserControls;
using ITS.Retail.Platform.Enumerations;
using System.ComponentModel;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class GroupTemplatePositionActionButton : GroupTemplatePosition<ucActionButton>
    {
        public eActions Action { get; set; }
        protected override void PrepareControl(ucActionButton control)
        {
            control.Action = this.Action;
        }
    }
}
