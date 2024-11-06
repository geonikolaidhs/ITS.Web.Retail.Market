using ITS.POS.Client.UserControls;
using System.ComponentModel;
using System.Windows.Forms;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]

    public abstract class GroupTemplatePosition<T> : GroupActionButtonPosition where T : ucButton, new()
    {
        protected abstract void PrepareControl(T control);

        public override ucButton ConstructUIControl(Form formToAddm, ButtonProperties defaultProperties)
        {
            T control = new T();
            control.Button.Text = this.Text;
            ButtonProperties activeProperties = this.UseDefaultProperties ? defaultProperties : this.ButtonProperties;
            activeProperties.Apply(control.Button);
            PrepareControl(control);
            return control;
        }
    }
}
