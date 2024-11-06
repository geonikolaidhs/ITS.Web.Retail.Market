using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using ITS.Common.Utilities.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using TwainDotNet;
using TwainDotNet.WinFroms;

namespace ITS.Retail.ScanTool.PDF
{
    public partial class MainForm : Form
    {
        public static String ConnectionString { get; set; }        
        public bool Editing;
        public bool IsNewRow;

        public MainForm()
        {
            InitializeComponent();

        }

        private void Scan()
        {
            Stack<Image> images = new Stack<Image>();

            Twain twain = new Twain(new WinFormsWindowMessageHook(this));
            if (twain.DefaultSourceName != ApplicationSettings.DefaultScanner && !String.IsNullOrWhiteSpace(ApplicationSettings.DefaultScanner))
            {
                twain.SelectSource(ApplicationSettings.DefaultScanner);
            }
            twain.TransferImage += delegate(Object sender2, TransferImageEventArgs args)
            {
                if (args.Image != null)
                {
                    images.Push(args.Image);
                }
            };



            twain.ScanningComplete += delegate
            {
                //DocumentDetail currentDetail2 = DetailView.GetFocusedRow() as DocumentDetail;
                try
                {
                    Image img1 = MergeMultipleImages(images);
                    PdfDocument doc = new PdfDocument();
                    PdfPage pdfPage = new PdfPage();
                    
                    doc.Pages.Add(pdfPage); 
                    XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                    XImage img = XImage.FromGdiPlusImage(img1);
                       
                    xgr.DrawImage(img, 0, 0);
                    string filename = ApplicationSettings.SaveLocation.TrimEnd('\\') + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".pdf";
                    doc.Save(filename); 
                    doc.Close();

                    if (MessageBox.Show("Η σάρωση αποθηκεύτηκε. Θέλετε να δείτε το αρχείο;", "Ολοκλήρωση", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(filename);
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    MessageBox.Show("Αποτυχία συνένωσης των εικόνων. Τα έγγραφα πρέπει να είναι ιδίου μεγέθους.");
                }
                Enabled = true;
            };

            ScanSettings settings = new ScanSettings
            {
                UseDocumentFeeder = ApplicationSettings.UseDocumentFeeder,
                ShowTwainUI = ApplicationSettings.ShowTwainUI,
                ShowProgressIndicatorUI = ApplicationSettings.ShowProgressIndicatorUI,
                UseDuplex = ApplicationSettings.UseDuplex,
                Resolution = new ResolutionSettings()
                {
                    ColourSetting = ApplicationSettings.ColourSetting,
                    Dpi = ApplicationSettings.Dpi
                },
                Area = null                
                //Rotation = new RotationSettings() { AutomaticBorderDetection = true }
            };
            if (ApplicationSettings.AutomaticBorderDetection)
            {
                settings.Rotation.AutomaticBorderDetection = true;
            }

            try
            {
                Enabled = false;
                twain.StartScanning(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Αποτυχία Σάρωσης: " + ex.Message);
                Enabled = true;
            }

        }

        private static Image MergeMultipleImages(Stack<Image> images)
        {
            if (images.Count() == 0)
            {
                return null;
            }
            else if (images.Count() == 1)
            {
                return images.First();
            }
            else
            {
                Image im1, im2;

                im1 = images.Pop();
                im2 = images.Pop();
                images.Push(MergeTwoImages(im2, im1));
                return MergeMultipleImages(images);
            }

        }

        private static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width > secondImage.Width ? firstImage.Width : secondImage.Width;

            int outputImageHeight = firstImage.Height + secondImage.Height + 1;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(0, firstImage.Height + 1), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    btnCancel_Click(sender, e);
                    break;
                case Keys.F8:
                    btnScan.PerformClick();
                    break;
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            Scan();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }


        

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ConfigurationHelper.AskUserAndSaveFileStatic(typeof(ApplicationSettings), Program.ConfigurationFile);
        }


    }
}
