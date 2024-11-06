using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using ITS.POS.Client.UserControls;
using System.IO;
using System.Diagnostics;

namespace ITS.POS.Client.Forms
{
    public partial class frmPauseSlider : frmInputFormBase
    {


        private string pauseFormMediaPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Media\\Images";

        public frmPauseSlider(IPosKernel kernel, List<Image> sliderImages) : base(kernel)
        {
            InitializeComponent();

            this.CanBeClosedByUser = false;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            this.lblUser.Text = appContext.CurrentUser.POSUserName;
            if (sliderImages != null && sliderImages.Count > 0)
            {
                ImageSlider.SetListImages(sliderImages);
            }



            /** Create instance of ImageSlider **/

            this.SuspendLayout();
            this.Width = appContext.MainForm.Size.Width;
            this.Height = appContext.MainForm.Size.Height;
            this.Location = new Point(appContext.MainForm.Location.X, appContext.MainForm.Location.Y);
            this.tePassword.BackColor = Color.FromArgb(0, 0, 0, 0);
            this.tePassword.Location = new Point(appContext.MainForm.Location.X - 5000, appContext.MainForm.Location.Y - 5000);
            ImageSlider.SuspendLayout();
            ImageSlider.SliderWidth = appContext.MainForm.Size.Width;
            ImageSlider.SliderHeight = appContext.MainForm.Size.Height;
            ImageSlider.MediaFolder = pauseFormMediaPath;
            ImageSlider.Location = appContext.MainForm.Location;
            ImageSlider.Size = new Size(appContext.MainForm.Size.Width, appContext.MainForm.Size.Height);
            ImageSlider.ResumeLayout();
            this.ResumeLayout();
            this.tePassword.Focus();


        }




        private void tePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) && e.KeyCode != Keys.Back)
            {
                //only numbers are allowed
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                if (tePassword.Text == appContext.CurrentUser.POSPassword)
                {
                    this.Close();
                }
                else
                {
                    UtilsHelper.HardwareBeep();
                    tePassword.Text = "";
                }
            }
        }

        private void ToogleVirtualKeyboard()
        {
            try
            {
                var path64 = Path.Combine(Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
                var path32 = @"C:\windows\system32\osk.exe";
                var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
                Process[] procs = Process.GetProcessesByName("osk");
                if (procs.Count() > 0)
                    for (int i = 0; i < procs.Count(); i++)
                        procs[i].Kill();
                else
                    Process.Start(path);
            }
            catch (Exception ex) { }

            try
            {
                this.tePassword.Focus();
            }
            catch (Exception ex) { }

        }

        private void CloseVirtualKeyboard()
        {
            try
            {
                var path64 = Path.Combine(Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
                var path32 = @"C:\windows\system32\osk.exe";
                var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
                Process[] procs = Process.GetProcessesByName("osk");
                if (procs.Count() > 0)
                    for (int i = 0; i < procs.Count(); i++)
                        procs[i].Kill();
            }
            catch (Exception ex) { }

        }

        private void frmPauseSlider_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVirtualKeyboard();
        }

        private void frmPauseSlider_Click(object sender, EventArgs e)
        {
            ToogleVirtualKeyboard();
        }

        private void ImageSlider_Click(object sender, EventArgs e)
        {
            ToogleVirtualKeyboard();
        }
    }
}
