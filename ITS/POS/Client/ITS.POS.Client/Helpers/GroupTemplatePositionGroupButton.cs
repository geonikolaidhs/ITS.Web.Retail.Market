using ITS.POS.Client.UserControls;
using System.ComponentModel;
using System.Drawing.Design;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class GroupTemplatePositionGroupButton : GroupTemplatePosition<ucGroupButton>
    {
        public GroupTemplatePositionGroupButton()
        {
            GroupActionButtonPosition = new GroupActionButtonPositionCollection();
        }

        [Editor(typeof(GroupActionButtonPositionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GroupActionButtonPositionCollection GroupActionButtonPosition { get; private set; }

        public int NumberOfRows { get; set; }

        public int NumberOfColumns { get; set; }

        protected override void PrepareControl(ucGroupButton control)
        {
            control.NumberOfColumns = NumberOfColumns;
            control.NumberOfRows = NumberOfRows;
            control.GroupActionButtonPosition = GroupActionButtonPosition;
        }
    }
}
