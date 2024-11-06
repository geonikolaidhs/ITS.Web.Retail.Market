using ITS.POS.Client.Helpers;
using System;
using System.ComponentModel.Design;

namespace ITS.POS.Client.UserControls
{
    public class GroupActionButtonPositionEditor : CollectionEditor
    {
        public GroupActionButtonPositionEditor() : base(typeof(GroupActionButtonPositionCollection))
        {
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[]
                {
                    typeof(GroupTemplatePositionAddItemButton),
                    typeof(GroupTemplatePositionAddDiscountButton),
                    typeof(GroupTemplatePositionActionButton),
                    typeof(GroupTemplatePositionGroupButton)
                };
        }
    }
}
