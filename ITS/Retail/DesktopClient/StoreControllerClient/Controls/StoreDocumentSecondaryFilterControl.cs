using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class StoreDocumentSecondaryFilterControl : BaseSecondaryFilterControl
    {
        public StoreDocumentSecondaryFilterControl()
        {
            InitializeComponent();
            eModule officeManagerModule = eModule.STORECONTROLLER;
            switch (Program.Settings.MasterAppInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    officeManagerModule = eModule.DUAL;
                    break;
                case eApplicationInstance.RETAIL:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
                default:
                    officeManagerModule = eModule.STORECONTROLLER;
                    break;
            }
            lueDocumentSeries.Properties.DataSource = StoreHelper.StoreDocumentTypes(Program.Settings.StoreControllerSettings.Store, Platform.Enumerations.eDivision.Store, officeManagerModule, false, false, true);
            lueDocumentSeries.Properties.Columns.Clear();
            lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDocumentSeries.Properties.ValueMember = "Oid";
        }

        private void lueDocumentSeries_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
