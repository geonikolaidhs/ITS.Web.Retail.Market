using ITS.POS.Client.UserControls;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public abstract class GroupActionButtonPosition: IDisposable
    {

        public GroupActionButtonPosition()
        {
            ButtonProperties = new ButtonProperties();
        }
        public abstract ucButton ConstructUIControl(Form formToAdd, ButtonProperties defaultProperties);

        public void Dispose()
        {
            if (ButtonProperties != null)
            {
                ButtonProperties.Dispose();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Button")]
        public ButtonProperties ButtonProperties { get; set; }

        [Category("Button")]
        public bool UseDefaultProperties { get; set; }


        public string Text { get; set; }
    }
}
