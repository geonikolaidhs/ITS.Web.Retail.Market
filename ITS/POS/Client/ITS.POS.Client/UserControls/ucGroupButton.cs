using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;
using System.Drawing.Design;
using ITS.POS.Client.Forms;

namespace ITS.POS.Client.UserControls
{
    public partial class ucGroupButton : ucButton
    {
        public ucGroupButton()
        {
            InitializeComponent();
            GroupActionButtonPosition = new GroupActionButtonPositionCollection();
            DefaultButtonProperties = new ButtonProperties()
            {
                BackColor = this.BackColor,
                Font = this.Font,
                ForeColor = this.ForeColor
            };
            this.Button.Click += Button_Click;
            this.RelativeControl = this.FindForm();
        }

        [Editor(typeof(GroupActionButtonPositionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual GroupActionButtonPositionCollection GroupActionButtonPosition { get; set; }


        private bool ShouldSerializeGroupActionButtonPosition()
        {

            return (GroupActionButtonPosition.Count > 0);
        }


        public int NumberOfRows { get; set; }

        public int NumberOfColumns { get; set; }

        public int InnerWindowTop { get; set; }

        public int InnerWindowLeft { get; set; }

        public Size InnerWindowSize { get; set; }

        public bool RemainOpenAfterClick { get; set; }


        public Control RelativeControl { get; set; }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Button")]
        public ButtonProperties DefaultButtonProperties { get; set; }

        protected virtual void Button_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is frmMainBase)
            {
                using (frmGroupButton frmGroup = new frmGroupButton())
                {

                    frmGroup.Initialize(this.Kernel);
                    frmGroup.InitializeLayout(this.GroupActionButtonPosition, this.NumberOfRows, this.NumberOfColumns, DefaultButtonProperties, RemainOpenAfterClick);
                    ArrangeButtonSizeProperties(frmGroup);

                    frmGroup.ShowDialog(this.FindForm());
                }
            }
            else if (this.FindForm() is frmGroupButton)
            {
                frmGroupButton frmGroup = (frmGroupButton)this.FindForm();
                frmGroup.InitializeLayout(this.GroupActionButtonPosition, this.NumberOfRows, this.NumberOfColumns, DefaultButtonProperties, RemainOpenAfterClick);
            }
        }

        protected void ArrangeButtonSizeProperties(frmGroupButton frmGroup)
        {
            frmGroup.StartPosition = FormStartPosition.Manual;
            frmGroup.Size = this.InnerWindowSize;
            Form currentForm = this.FindForm();
            if (currentForm != null && this.RelativeControl != null)
            {
                frmGroup.Top = currentForm.Top + this.RelativeControl.Top + this.InnerWindowTop;
                frmGroup.Left = currentForm.Left + this.RelativeControl.Left + this.InnerWindowLeft;
            }
        }
    }
}
