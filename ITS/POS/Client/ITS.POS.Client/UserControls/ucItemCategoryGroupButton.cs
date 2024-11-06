using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    public partial class ucItemCategoryGroupButton : ucGroupButton
    {
        public ucItemCategoryGroupButton()
        {
            this.InitializeComponent();
            this.ItemCategoryButtonProperties = new ButtonProperties()
            {
                BackColor = this.BackColor,
                Font = this.Font,
                ForeColor = this.ForeColor
            };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override GroupActionButtonPositionCollection GroupActionButtonPosition
        {
            get
            {
                return base.GroupActionButtonPosition;
            }

            set
            {
                base.GroupActionButtonPosition = value;
            }
        }

        public bool PrintItemCode { get; set; }

        public string ItemCategoryCode { get; set; }

        public ButtonProperties ItemCategoryButtonProperties { get; set; }

        protected override void Button_Click(object sender, EventArgs e)
        {
            if (this.FindForm() is frmMainBase)
            {
                using (frmItemCategoryGroupButton frmItemCategoryGroup = new frmItemCategoryGroupButton())
                {

                    frmItemCategoryGroup.Initialize(this.Kernel);
                    frmItemCategoryGroup.InitializeItemCategories (this.ItemCategoryCode,ItemCategoryButtonProperties, DefaultButtonProperties, this.NumberOfRows, this.NumberOfColumns, this.PrintItemCode, this.RemainOpenAfterClick);
                    ArrangeButtonSizeProperties(frmItemCategoryGroup); 

                    frmItemCategoryGroup.ShowDialog(this.FindForm());
                }
            }
            else if (this.FindForm() is frmItemCategoryGroupButton)
            {
                frmItemCategoryGroupButton frmGroup = (frmItemCategoryGroupButton)this.FindForm();
                frmGroup.InitializeItemCategories(this.ItemCategoryCode, ItemCategoryButtonProperties, DefaultButtonProperties, this.NumberOfRows, this.NumberOfColumns, this.PrintItemCode, this.RemainOpenAfterClick);                
            }
        }

    }




}
