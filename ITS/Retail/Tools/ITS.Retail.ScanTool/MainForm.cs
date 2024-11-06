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
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Common.Utilities.Forms;
using ITS.Retail.ScanTool.Model;
using ITS.Retail.ScanTool.RemoteHeaderService;
using Newtonsoft.Json;
using TwainDotNet;
using TwainDotNet.WinFroms;
using DevExpress.Entity.Model;
using ITS.Retail.Platform;

namespace ITS.Retail.ScanTool
{
    public partial class MainForm : Form
    {
        public static String ConnectionString { get; set; }        
        public UnitOfWork uow;
        public bool Editing;
        public bool IsNewRow;

        private ScannedDocumentHeader EditingHeader
        {
            get;
            set;
        }

        public MainForm()
        {
            uow = XpoHelper.GetNewUnitOfWork();
            XpoHelper.UpdateDatabase();
            XpoDefault.Session = uow;
            InitializeComponent();

        }
        private void RemoveDataBindings()
        {
            txtDate.DataBindings.Clear();
            txtVat.DataBindings.Clear();
            txt.DataBindings.Clear();
            txtAmount.DataBindings.Clear();
            picture.DataBindings.Clear();

            txtDate.EditValue = null;
            txtVat.EditValue = null;
            txt.EditValue = null;
            txtAmount.EditValue = null;
            picture.EditValue = null;
        }

        private void AddDataBindings()
        {
            txtDate.DataBindings.Add("EditValue", EditingHeader, "DocumentIssueDate", true);
            txtVat.DataBindings.Add("EditValue", EditingHeader, "SupplierTaxCode", true);
            txt.DataBindings.Add("EditValue", EditingHeader, "DocumentNumber", true);
            txtAmount.DataBindings.Add("EditValue", EditingHeader, "DocumentAmount", true);
            picture.DataBindings.Add("EditValue", EditingHeader, "ScannedImage", true);
        }



        private void Scan()
        {
            Stack<Image> images = new Stack<Image>();

            if (EditingHeader == null)
            {
                MessageBox.Show("Επιλέξτε Γραμμή.");
                return;
            }


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
                    EditingHeader.ScannedImage = MergeMultipleImages(images);                    
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetFullMessage();
                    MessageBox.Show("Αποτυχία συνένωσης των εικόνων. Τα έγγραφα πρέπει να είναι ιδίου μεγέθους.");
                }
                Enabled = true;
                btnSavePrint.Focus();
            };

            ScanSettings settings = new ScanSettings
            {
                UseDocumentFeeder = ApplicationSettings.UseDocumentFeeder,
                ShowTwainUI = ApplicationSettings.ShowTwainUI,
                ShowProgressIndicatorUI = ApplicationSettings.ShowProgressIndicatorUI,
                UseDuplex = ApplicationSettings.UseDuplex,
                Resolution = null,
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
                case Keys.F9:
                    btnSavePrint.PerformClick();
                    break;
                case Keys.Escape:
                    btnCancel_Click(sender, e);
                    break;
                case Keys.F5:
                    btnNew.Focus();
                    Application.DoEvents();
                    btnNew.PerformClick();
                    break;
                case Keys.F8:
                    btnScan.PerformClick();
                    break;
                case Keys.F7:
                    btnScanAll.PerformClick();
                    break;
                case Keys.Delete:
                    btnDelete.PerformClick();
                    break;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("Οι αλλαγές σας δεν έχουν αποθηκευτεί. Θέλετε να παραμείνετε στην οθόνη επεξεργασίας;", "Ερώτηση", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                dxErrorProvider.ClearErrors();
                EditingHeader = null;
                uow.RollbackTransaction();
                RemoveDataBindings();
                grpDocuments.Enabled = true;
                grpEdit.Enabled = false;

            }
        }



        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }


        




        private void btnUpload_Click(object sender, EventArgs e)
        {
            int suc = 0, fail = 0, notsend;
            using (ScannedDocumentHeaderService ws = new ScannedDocumentHeaderService())
            {
                
                ws.Url = ApplicationSettings.Url.Trim().TrimEnd('/') +"/ScannedDocumentHeaderService.asmx" ;
                ws.Timeout = 100000;
                XPCollection<ScannedDocumentHeader> heads = new XPCollection<ScannedDocumentHeader>(uow);
                notsend = heads.Where(g => g.IsValid == false).Count();
                foreach (ScannedDocumentHeader head in heads.Where(g=>g.IsValid&& g.Uploaded!=UploadResult.SUCCESS))
                {
                    try
                    {
                        String messages;
                        if (ws.GetData(head.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true), out messages))
                        {
                            head.Uploaded = UploadResult.SUCCESS;
                            suc++;
                        }
                        else
                        {
                            head.Uploaded = UploadResult.FAIL;
                            fail++;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.GetFullMessage();
                        head.Uploaded = UploadResult.FAIL;
                        fail++;
                    }
                }
                
            }
            uow.CommitChanges();
            MessageBox.Show("Αποτελέσματα αποστολής" + Environment.NewLine + "Επιτυχίες: " + suc + Environment.NewLine + "Αποτυχίες: " + fail + Environment.NewLine + "Ελλειπή Στοχεία:" + notsend,"Αποτελέσματα αποστολής",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ConfigurationHelper.AskUserAndSaveFileStatic(typeof(ApplicationSettings), Program.ConfigurationFile);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteUploaded_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes== MessageBox.Show("Είστε σίγουροι ότι θέλετε να διαγραφούν όλα τα επιτυχώς απεσταλμένα παραστατικά ;", "Ερώτηση", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                XPCollection<ScannedDocumentHeader> docHeads = new XPCollection<ScannedDocumentHeader>(uow, new BinaryOperator("Uploaded", UploadResult.SUCCESS));
                uow.Delete(docHeads);
                uow.CommitTransaction();
            }
        }



        private bool Validate(ScannedDocumentHeader head)
        {
            
            if (head != null )
            {
                dxErrorProvider.ClearErrors();
                bool returnValue = true;
                if (head.DocumentIssueDate.Ticks <= 10000)
                {
                    dxErrorProvider.SetError(txtDate, "Συμπληρώστε ημερομηνία");
                    dxErrorProvider.SetErrorType(txtDate, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    returnValue = false;
                }
                if (String.IsNullOrWhiteSpace(head.DocumentNumber))
                {
                    dxErrorProvider.SetError(txt, "Συμπληρώστε Αρ. παραστατικού");
                    dxErrorProvider.SetErrorType(txt, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    returnValue = false;
                }
                if (String.IsNullOrWhiteSpace(head.SupplierTaxCode))
                {
                    dxErrorProvider.SetError(txtVat, "Συμπληρώστε ΑΦΜ");
                    dxErrorProvider.SetErrorType(txtVat, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    returnValue = false;
                }
                if (head.ScannedImage == null || head.ScannedImage.Width == 0|| head.ScannedImage.Height==0)
                {
                    dxErrorProvider.SetError(picture, "Εισάγετε Σάρωση");
                    dxErrorProvider.SetErrorType(picture, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    returnValue = false;
                }
                return returnValue;
                
            }
            return false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (EditingHeader != null) return;

            EditingHeader = new ScannedDocumentHeader(uow);
            grpDocuments.Enabled = false;
            grpEdit.Enabled = true;
            txtVat.Focus();
            AddDataBindings();
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            if (EditingHeader == null) return;

            if (Validate(EditingHeader))
            {
                dxErrorProvider.ClearErrors();
                EditingHeader.Save();                
                RemoveDataBindings();
                
                grpDocuments.Enabled = true;
                grpEdit.Enabled = false;
                uow.CommitTransaction();
                xpcScannedDocumentHeaders.Reload();
                DetailView.RefreshData();
                EditingHeader = null;
                btnNew.Focus();
            }

        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (EditingHeader == null) return;
            Scan();
            btnSavePrint.Focus();
        }


        private void btnExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DetailView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.Clicks >= 2 && !e.Handled)
            {
                e.Handled = true;
                var v = DetailView.GetRow(e.RowHandle);
                if (v is ScannedDocumentHeader)
                {
                    EditingHeader = (ScannedDocumentHeader)v;
                    grpDocuments.Enabled = false;
                    grpEdit.Enabled = true;
                    txtVat.Focus();
                    AddDataBindings();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Είστε σίγουροι ότι θέλετε να διαγράψετε την συγκεκριμένη εγγαρφή;", "Ερώτηση", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int selectedRowHandle = DetailView.GetSelectedRows().First();
                var v = DetailView.GetRow(selectedRowHandle);
                if (v is ScannedDocumentHeader)
                {
                    ScannedDocumentHeader head = (ScannedDocumentHeader)v;
                    uow.Delete(head);
                    uow.CommitTransaction();
                }
            }
        }

        private void splitterControl1_Move(object sender, EventArgs e)
        {
            if (grpDocuments.Height < grpDocuments.MinimumSize.Height)
            {
                
            }
        }

        private void btnScanAll_Click(object sender, EventArgs e)
        {
            ScanAll();
            xpcScannedDocumentHeaders.Reload();
            DetailView.RefreshData();
        }



        private void ScanAll()
        {
            Stack<Image> images = new Stack<Image>();

            if (EditingHeader != null)
            {                
                return;
            }


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
                    foreach (Image img in images)
                    {
                        ScannedDocumentHeader dc = new ScannedDocumentHeader(uow);
                        dc.ScannedImage = img;
                        dc.Save();
                    }
                    uow.CommitChanges();
                    xpcScannedDocumentHeaders.Reload();
                    DetailView.RefreshData();
                    //EditingHeader.ScannedImage = MergeMultipleImages(images);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Αποτυχία συνένωσης των εικόνων. Τα έγγραφα πρέπει να είναι ιδίου μεγέθους.");
                    string errorMessage = ex.GetFullMessage();
                }
                Enabled = true;
            };

            ScanSettings settings = new ScanSettings
            {
                UseDocumentFeeder = ApplicationSettings.UseDocumentFeeder,
                ShowTwainUI = ApplicationSettings.ShowTwainUI,
                ShowProgressIndicatorUI = ApplicationSettings.ShowProgressIndicatorUI,
                UseDuplex = ApplicationSettings.UseDuplex,
                Resolution = null,
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



    }
}
