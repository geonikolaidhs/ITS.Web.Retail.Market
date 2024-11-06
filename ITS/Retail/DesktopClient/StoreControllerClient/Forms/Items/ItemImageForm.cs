using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageMagick;
using System.Drawing.Imaging;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using DevExpress.XtraEditors;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class ItemImageForm : XtraLocalizedForm
    {
        protected MagickImage _Image;
        protected MagickImage Image { get { return _Image; } }

        protected Item _Item;
        protected Item Item { get { return _Item; } }

        public ItemImageForm(Item item)
        {
            this._Item = item;          
            InitializeComponent();
        }

        private void InitializeBindings()
        {
            this.txtItemImageDescription.DataBindings.Add("EditValue", this.Item, "ImageDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtItemImageInfo.DataBindings.Add("EditValue", this.Item, "ImageInfo", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Handles the Click event of the btnDeleteImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            if (this.imageEdit.Image !=null && DialogResult.Yes == XtraMessageBox.Show(
                ResourcesLib.Resources.Delete, 
                ResourcesLib.Resources.Question, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question))
            {
                this.imageEdit.Image = this.Item.ImageLarge = this.Item.ImageMedium = this.Item.ImageSmall = null;
                this._Image = null;
                this.Item.ImageDescription = this.Item.ImageInfo = null;
                this.simpleButtonOk.Visible = true;
            }

        }

        private void ItemImageForm_Load(object sender, EventArgs e)
        {
            this.imageEdit.Image = this.Item.ImageSmall;
            InitializeBindings();
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (.jpg, .jpeg, .jpe, .gif, .png)|*.jpg;*.jpeg;*.jpe;*.gif;*.png";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this._Image = new MagickImage(dialog.FileName);
                this.Item.ImageLarge = ItemHelper.PrepareImage(this.Image, 600, 600);
                this.Item.ImageMedium = ItemHelper.PrepareImage(this.Image, 300, 300);
                this.Item.ImageSmall = ItemHelper.PrepareImage(this.Image, 150, 150);
                this.imageEdit.Image = this.Image.ToBitmap(ImageFormat.Png);
                simpleButtonOk.Visible = true;
            }
        }

        private void simpleButtonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ItemImageForm_Shown(object sender, EventArgs e)
        {
            this.imageEdit.Focus();
        }
    }
}
