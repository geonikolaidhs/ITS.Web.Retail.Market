using ITS.POS.Client.UserControls;
using System.ComponentModel;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class GroupTemplatePositionAddItemButton : GroupTemplatePosition<ucAddItemButton>
    {
        public string Barcode { get; set; }
        protected override void PrepareControl(ucAddItemButton control)
        {
            control.Barcode = this.Barcode;
        }
    }
}
