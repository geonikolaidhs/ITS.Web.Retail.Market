using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// The base grid control that all the other grids inherit from
    /// </summary>
    public partial class ucDataGridViewExtension : DataGridView, IObserver, IObserverGrid
    {
        public IPosKernel Kernel
        {
            get
            {
                Form parentForm = this.FindForm();
                if (parentForm is IPOSForm)
                {
                    return (parentForm as IPOSForm).Kernel;
                }
                else
                {
                    throw new POSException("Form must implement IPOSForm");
                }
            }
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public ucDataGridViewExtension()
        {
            this.ReadOnly = true;
            this.AutoGenerateColumns = false;
            this.MultiSelect = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;

            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.ActionsToObserve = new List<eActions>();
            this.ActionsToObserve.Add(eActions.MOVE_UP);
            this.ActionsToObserve.Add(eActions.MOVE_DOWN);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO);
            this.DefaultDeletedCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.Red,
                Font =
                    new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, FontStyle.Strikeout)
            };

            this.DefaultInvalidCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.Maroon,
                Font =
                    new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, FontStyle.Italic)
            };
            InitializeComponent();
            this.RowPostPaint += DetailDataGrid_RowPostPaint;
            this.DefaultCellStyleChanged += DetailDataGrid_DefaultCellStyleChanged;
            this.BackgroundColor = Color.White;
            this.ColumnHeadersDefaultCellStyleChanged += ucDataGridViewExtension_ColumnHeadersDefaultCellStyleChanged;

        }


        protected bool _LocalizationExecuted = false;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode == false && Visible && Disposing == false && _LocalizationExecuted == false & this.Kernel != null)
            {
                ILocalizationResolver localizationResolver = this.Kernel.GetModule<ILocalizationResolver>();
                foreach (DataGridViewColumn column in this.Columns)
                {
                    column.HeaderText = localizationResolver.ResolveDisplayText(column.HeaderText);
                }
                _LocalizationExecuted = true;
            }
        }

        void ucDataGridViewExtension_ColumnHeadersDefaultCellStyleChanged(object sender, EventArgs e)
        {

        }

        int x, y, gridHeight, gridWidth;
        Image backgroundImage;

        protected override void PaintBackground(Graphics graphics, Rectangle clipBounds, Rectangle gridBounds)
        {
            base.PaintBackground(graphics, clipBounds, gridBounds);
            if (Kernel != null)
            {
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                if (!config.EnableLowEndMode)
                {
                    graphics.FillRectangle(new SolidBrush(this.BackgroundColor), gridBounds);
                    if (gridBounds.Height != gridHeight || gridBounds.Width != gridWidth || backgroundImage == null)
                    {
                        gridHeight = gridBounds.Height;
                        gridWidth = gridBounds.Width;
                        if (backgroundImage != null)
                        {
                            try
                            {
                                backgroundImage.Dispose();
                            }
                            catch (Exception ex)
                            {
                                string errorMessage = ex.GetFullMessage();
                            }
                            backgroundImage = null;
                        }
                        double percx = Properties.Resources.WRM_LOGO.Height * 1.0 / (gridHeight - 20);
                        double percy = Properties.Resources.WRM_LOGO.Width * 1.0 / gridWidth;
                        double perc = Math.Min(1.0 / percx, 1.0 / percy);
                        backgroundImage = ResizeImage(Properties.Resources.WRM_LOGO, (int)Math.Round(perc * Properties.Resources.WRM_LOGO.Width), (int)Math.Round(perc * Properties.Resources.WRM_LOGO.Height));
                        x = (gridWidth - backgroundImage.Width) / 2;
                        y = 20 + (gridHeight - backgroundImage.Height) / 2;

                    }
                    this.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Transparent;

                    graphics.DrawImage(backgroundImage, x, y);
                }
                else
                {
                    this.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                }
            }
        }

        Type[] paramsType = new Type[] { typeof(GridParams) };
        public virtual Type[] GetParamsTypes()
        {
            return paramsType;
        }

        public void InitializeActionSubscriptions()
        {
            foreach (eActions act in ActionsToObserve)
            {
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                actionManager.GetAction(act).Attach(this);
            }
        }

        public void DropActionSubscriptions()
        {
            foreach (eActions act in ActionsToObserve)
            {
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                actionManager.GetAction(act).Dettach(this);
            }
        }

        public bool ShouldSerializeActionsToObserve()
        {
            //DO NOT DELETE
            return false;
        }

        [Browsable(false)]
        public List<eActions> ActionsToObserve
        {
            get;
            set;
        }

        void DetailDataGrid_DefaultCellStyleChanged(object sender, EventArgs e)
        {
            this.DefaultDeletedCellStyle.Font = new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, this.DefaultDeletedCellStyle.Font.Style);

            this.DefaultInvalidCellStyle.Font = new Font(this.DefaultCellStyle.Font.Name, this.DefaultCellStyle.Font.Size, this.DefaultInvalidCellStyle.Font.Style);

        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DataGridViewCellStyle DefaultDeletedCellStyle { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DataGridViewCellStyle DefaultInvalidCellStyle { get; set; }

        void DetailDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }


        public bool HideDeletedLines { get; set; }



        public virtual void Update(GridParams parameters)
        {

        }
    }
}


