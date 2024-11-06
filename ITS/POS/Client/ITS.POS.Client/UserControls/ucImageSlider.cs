using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Timers;
using System.IO;

namespace ITS.POS.Client.UserControls
{
    public class ucImageSlider : Panel
    {


        private System.Timers.Timer SliderTimer { get; set; }
        public string MediaFolder { get; set; }
        public int AnimationInterval { get; set; } = 4000;
        private int _pageIndex { get; set; } = 0;
        public int SliderWidth { get; set; } = 1024;
        public int SliderHeight { get; set; } = 768;



        protected List<Image> _imageList = new List<Image>();

        public ucImageSlider()
        {
            this.SliderTimer = new System.Timers.Timer();
            this.SliderTimer.Interval = AnimationInterval;
            this.SliderTimer.Elapsed += new ElapsedEventHandler(this.Slide);
            this.SliderTimer.Enabled = true;
            if (_imageList == null || _imageList.Count < 1)
                LoadImages();
            SetSliderDismensions();
        }

        public ucImageSlider(bool enabled)
        {
            this.SliderTimer = new System.Timers.Timer();
            this.SliderTimer.Interval = AnimationInterval;
            this.SliderTimer.Elapsed += new ElapsedEventHandler(this.Slide);
            this.SliderTimer.Enabled = enabled;
            SetSliderDismensions();
        }

        public void StartTimer()
        {
            if (SliderTimer.Enabled == false)
                SliderTimer.Enabled = true;
        }

        void Slide(object sender, EventArgs e)
        {

            if (_pageIndex < _imageList.Count - 1)
            {
                ++_pageIndex;
            }
            else
            {
                _pageIndex = 0;
            }

            if (_imageList == null || _imageList.Count < 1)
            { LoadImages(); }

            if (_imageList != null && _imageList.Count > 0)
            { this.Invalidate(); }



        }

        public void AddImage(string imageName)
        {
            Image img = Image.FromFile(@MediaFolder + "/" + imageName);
            _AddImage(img);
        }


        public void AddImage(Image img)
        {
            _AddImage(img);
        }

        public void SetListImages(List<Image> list)
        {
            this._imageList = list;
        }


        protected void _AddImage(Image img)
        {
            _imageList.Add(img);


        }

        private void SetSliderDismensions()
        {
            this.SuspendLayout();
            this.Width = SliderWidth;
            this.Height = SliderHeight;
            this.ResumeLayout();
        }

        public void LoadImages()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(MediaFolder);
                if (Directory.Exists(MediaFolder))
                {
                    string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff";
                    foreach (FileInfo imageFile in directory.GetFiles("*.*", SearchOption.AllDirectories).Where(s => supportedExtensions.Contains(Path.GetExtension(s.Name).ToLower())))
                    {
                        this.AddImage(imageFile.Name);
                    }
                }

            }
            catch (Exception ex) { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            try
            {
                if (_imageList != null && _imageList.Count > 0)
                    g.DrawImage(_imageList[_pageIndex], new Rectangle(0, 0, this.SliderWidth, this.SliderHeight));

            }
            catch (Exception ex) { }
        }


    }
}
