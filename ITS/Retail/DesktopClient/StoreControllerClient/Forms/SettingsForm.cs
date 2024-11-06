using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopPOSUpdateService;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class SettingsForm : XtraLocalizedForm
    {
        public class CultureSelect
        {
            public CultureSelect(eCultureInfo culture)
            {
                Code = culture.ToLocalizedString();
                Description = culture.ToString();
                switch (culture)
                {
                    case eCultureInfo.Deutch:
                        Flag = Properties.Resources.Germany_32;
                        break;
                    case eCultureInfo.English:
                        Flag = Properties.Resources.USA_32;
                        break;
                    case eCultureInfo.Norsk:
                        Flag = Properties.Resources.Norway_32;
                        break;
                    case eCultureInfo.Ελληνικά:
                        Flag = Properties.Resources.Greece_32;
                        break;
                }
            }
            [Display(Description = "Language", ResourceType = typeof(Resources), Order = 0)]
            public Image Flag { get; set; }
            [Display(AutoGenerateField = false)]
            public string Code { get; set; }
            [Display(Description = "Language", ResourceType = typeof(Resources), Order = 1)]
            public string Description { get; set; }

        }

        private StoreControllerClientSettings EditingSettings { get; set; }
        public SettingsForm(string filename)
        {
            InitializeComponent();
            EditingSettings = new StoreControllerClientSettings(filename, false);
            EditingSettings.Load();

            lcDefaultPrinter.HideToCustomization();
            List<CultureSelect> languageSelection = new List<CultureSelect>();
            languageSelection.Add(new CultureSelect(eCultureInfo.English));
            languageSelection.Add(new CultureSelect(eCultureInfo.Ελληνικά));
            languageSelection.Add(new CultureSelect(eCultureInfo.Deutch));
            languageSelection.Add(new CultureSelect(eCultureInfo.Norsk));

            this.lueCulture.Properties.DataSource = languageSelection;
            this.lueCulture.Properties.ValueMember = "Code";
            this.lueCulture.Properties.DisplayMember = "Description";

            this.lueCulture.DataBindings.Add("EditValue", this.EditingSettings, "Culture");
            this.lueDefaultPrinter.DataBindings.Add("EditValue", this.EditingSettings, "DefaultLabelPrinter");
            this.txtStoreControllerURL.DataBindings.Add("EditValue", this.EditingSettings, "StoreControllerURL");
            this.txtHQUrl.DataBindings.Add("EditValue", this.EditingSettings, "HQURL");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = XtraMessageBox.Show(Resources.ApplicationMustCloseAreYouSure, Resources.ApplicationMustCloseAreYouSure, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (UrlIsOK(string.Format("{0}/{1}", EditingSettings.StoreControllerURL.TrimEnd('/'), "POSUpdateService.asmx")))
                {
                    EditingSettings.Save();
#if DEBUG
                    Application.Exit();
#else 
                    Application.Restart();    
#endif
                }
                else
                {
                    XtraMessageBox.Show(ResourcesLib.Resources.ConnectionErrorReviseSettings);
                }

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            this.lueDefaultPrinter.Focus();
        }

        private bool UrlIsOK(string url)
        {
            try
            {
                using (POSUpdateService webservice = new POSUpdateService())
                {
                    webservice.Url = url;
                    return webservice.Ping();
                }
            }
            catch(Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                return false;
            }
        }

    }
}
