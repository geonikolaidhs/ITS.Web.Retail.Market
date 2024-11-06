using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.WebClient.Helpers;
using DevExpress.XtraEditors;
using ITS.Retail.ResourcesLib;


namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class HQReportBrowser : XtraLocalizedForm, INotifyPropertyChanged
    {
        Guid ReportId;
        Timer CheckLoaded = new Timer();
        public void ShowReport(Guid ReportId)
        {
            if (Program.Settings.HQURL == null)
            {
                using (SettingsForm form = new SettingsForm(Program.SettingsFilePath))
                {
                    form.ShowDialog();
                }
            }
            else
            {
                //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ITSWaitForm));
                this.ReportId = ReportId;
                string postData = "UserName=" + Program.Settings.CurrentUser.UserName + "&password=" + Program.UserPassword + "&Login=Είσοδος";
                //postData = "UserName=admin&password=1t$erviceS2015&Login=Είσοδος";
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(postData);
                char[] MyChar = { '/', ' ' };
                string url = Program.Settings.HQURL.TrimEnd(MyChar) + "/Login";
                webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
                //application/x-www-form-urlencoded; charset=UTF-8
                webBrowser1.Navigate(url, string.Empty, bytes, "Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
                //CheckLoaded.Interval = 60000;
                //CheckLoaded.Tick += new EventHandler(CheckLoaded_Tick);
                //CheckLoaded.Start();
                this.Show();
            }

        }
        void CheckLoaded_Tick(object sender, EventArgs e)
        {
            CheckLoaded.Stop();
            CheckLoaded.Tick -= new EventHandler(CheckLoaded_Tick);
            if (this.Visible == false)
            {
                this.Close();
                this.Dispose();
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(Resources.HQCustomReportViewMaxTries);
            }

        }
        int LoadCounter = 0;
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //

            if (webBrowser1.DocumentTitle == "Web Retail Market")
            {
                object[] args = { Program.Settings.Culture };
                webBrowser1.Document.InvokeScript("chooseLanguage", args);
                System.Threading.Thread.Sleep(100);
                char[] MyChar = { '/', ' ' };
                string url = Program.Settings.HQURL.TrimEnd(MyChar) + "/Reports/CustomReport?Oid=" + ReportId.ToString();
                webBrowser1.Navigate(url, string.Empty, null, "Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
            }
            else if (webBrowser1.Url.ToString().Contains("CustomReport?Oid"))
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                this.Show();
            }
            LoadCounter += 1;
            if (LoadCounter > 4)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(Resources.HQCustomReportViewMaxTries);
                this.Dispose();
            }
        }

        public HQReportBrowser()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
