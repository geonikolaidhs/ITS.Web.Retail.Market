using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class ButtonProperties: IDisposable
    {
        public ButtonProperties()
        {
            this.ImageAlign = ContentAlignment.MiddleCenter;
        }
        public Color BackColor { get; set; }

        public Image BackgroundImage { get; set; }

        public Font Font { get; set; }

        public ImageLayout BackgroundImageLayout { get; set; }

        public Image Image { get; set; }

        public ContentAlignment ImageAlign { get; set; }

        public TextImageRelation TextImageRelation { get; set; }

        public Color ForeColor { get; set; }



        public void Apply(Button button)
        {
            button.BackColor = this.BackColor;
            button.BackgroundImage = this.BackgroundImage;
            button.Font = this.Font;
            button.BackgroundImageLayout = this.BackgroundImageLayout;
            button.Image = this.Image;
            button.ImageAlign = this.ImageAlign;
            button.TextImageRelation = this.TextImageRelation;
            button.ForeColor = this.ForeColor;
            //Text is applied externally
        }

        public void Dispose()
        {
            DisposeObject(Image);
            DisposeObject(BackgroundImage);
            DisposeObject(Font);
        }

        private void DisposeObject(IDisposable  objectToDispose)
        {
            if (objectToDispose != null)
            {
                try
                {
                    objectToDispose.Dispose();
                }
                catch (Exception exception)
                {
                    string exceptionMessage = exception.GetFullMessage();
                }
            }
        }
    }
}
