using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using POSLoader.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ITS.OfficeManager.Loader
{
    public partial class SettingsForm : Form
    {
        private LogHelper logger;
        StoreControllerClientSettings Settings;

        public SettingsForm(StoreControllerClientSettings settings, LogHelper logger)
        {
            InitializeComponent();
            
            this.logger = logger;
            this.Settings = settings;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnInitialise_Click(object sender, EventArgs e)
        {


            if (txtbxServerUrl.Text.Length == 0)
            {
                logger.Error("Please specify Server URL");
                return;
            }

            Settings.StoreControllerURL = txtbxServerUrl.Text;
            this.Close();
           

        }
    }
}
