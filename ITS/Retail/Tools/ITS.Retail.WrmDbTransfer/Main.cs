using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WrmDbTransfer.Classes;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.XtraWaitForm;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using DbTrans.Classes;
using NLog;
using System.Threading.Tasks;

namespace ITS.Retail.WrmDbTransfer
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private static DbHelper SourceDbHelper = null;

        private static DbHelper TargetDbHelper = null;


        public Main()
        {
            Program.LogFile = LogManager.GetLogger("WrmDbTransfer");
            InitializeComponent();
            Program.LogFile.Info("Program started");
            radioVersion.SelectedIndex = 0;
            DbTransferService.Init();
            txtSrcDbType.Properties.DataSource = Enum.GetValues(typeof(DatabaseType));
            txtSrcDbType.EditValue = DatabaseType.SQLServer;
            txtTargetDbType.Properties.DataSource = Enum.GetValues(typeof(DatabaseType));
            txtTargetDbType.EditValue = DatabaseType.Oracle;

#if DEBUG
            txtsrcDatabase.Text = @"WRMDUAL3";
            txtSrcServer.Text = @"appservices.westeurope.cloudapp.azure.com";
            //txtSrcServer.Text = @"its.northeurope.cloudapp.azure.com";
            txtSrcUsername.Text = @"sa2";
            txtSrcPassword.Text = @"111111";

            //txtTargetDatabase.Text = @"orcl.wrm.cloudapp.net";
            txtTargetDatabase.Text = @"orcl.wrmdb.net";
            txtTargetServer.Text = @"appservices.westeurope.cloudapp.azure.com";
            //txtTargetServer.Text = @"its.northeurope.cloudapp.azure.com";
            txtTargetUsername.Text = @"WRM3";
            txtTargetPassword.Text = @"111111";

#endif
            DbTransferService.OnDatabaseUpdateEventHandler += OnDatabaseUpdateEventHandler;
        }


        private void btnUpdateSrcSettings_Click(object sender, EventArgs e)
        {
            if (IsSourceSettingsValid())
            {
                CreateSourceDbHelper();
                XtraMessageBox.Show(Resources.SavedSuccesfully);
            }
            else
            {
                XtraMessageBox.Show(Resources.FillAllMissingFields, "Error");
            }
        }

        private void CreateSourceDbHelper()
        {
            DatabaseType type;
            if (Enum.TryParse<DatabaseType>(txtSrcDbType.EditValue.ToString(), out type))
            {
                SourceDbHelper = new DbHelper(txtSrcServer.Text, txtsrcDatabase.Text, txtSrcUsername.Text, txtSrcPassword.Text, type);
            }
            else
            {
                throw new Exception("Invalid DbType");
            }
        }

        private void btnTestSrcConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (SourceDbHelper == null)
                {
                    if (IsSourceSettingsValid())
                    {
                        CreateSourceDbHelper();
                    }
                    if (SourceDbHelper == null)
                    {
                        throw new Exception("Fill Conection Settings");
                    }
                }
                SplashScreenManager.ShowDefaultWaitForm("Connecting", "Connecting");
                GetItem(SourceDbHelper.GetNewUnitOfWork());
                SplashScreenManager.CloseDefaultWaitForm();
                lblSrcHeader.ForeColor = Color.Green;
                XtraMessageBox.Show("Connected");
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseDefaultWaitForm();
                lblSrcHeader.ForeColor = Color.Red;
                XtraMessageBox.Show("Fail to connect " + Environment.NewLine + ex.Message, "Error");
            }
        }

        private void btnTestTargetConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (TargetDbHelper == null)
                {
                    if (IsTargetSettingsValid())
                    {
                        CreateTargetDbHelper();
                    }
                    if (TargetDbHelper == null)
                    {
                        throw new Exception("Fill Conection Settings");
                    }
                }
                SplashScreenManager.ShowDefaultWaitForm("Connecting", "Connecting");
                GetItem(TargetDbHelper.GetNewUnitOfWork());
                SplashScreenManager.CloseDefaultWaitForm();
                lblTargetHeader.ForeColor = Color.Green;
                XtraMessageBox.Show("Connected");
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseDefaultWaitForm();
                lblTargetHeader.ForeColor = Color.Red;
                XtraMessageBox.Show("Fail to connect " + Environment.NewLine + ex.Message, "Error");
                Program.LogFile.Error(ex.Message);
            }
        }

        private void btnUpdateTargetConnection_Click(object sender, EventArgs e)
        {
            if (IsTargetSettingsValid())
            {
                CreateTargetDbHelper();
                XtraMessageBox.Show(Resources.SavedSuccesfully);
            }
            else
            {
                XtraMessageBox.Show(Resources.FillAllMissingFields, "Error");
            }
        }

        private void CreateTargetDbHelper()
        {
            DatabaseType type;
            if (Enum.TryParse<DatabaseType>(txtTargetDbType.EditValue.ToString(), out type))
            {
                TargetDbHelper = new DbHelper(txtTargetServer.Text, txtTargetDatabase.Text, txtTargetUsername.Text, txtTargetPassword.Text, type);
            }
            else
            {
                throw new Exception("Invalid DbType");
            }
        }


        private bool IsSourceSettingsValid()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtsrcDatabase.Text) || string.IsNullOrWhiteSpace(txtSrcServer.Text) || string.IsNullOrWhiteSpace(txtSrcUsername.Text) ||
                string.IsNullOrWhiteSpace(txtSrcPassword.Text) || string.IsNullOrWhiteSpace(txtSrcDbType.EditValue.ToString()))
            {
                isValid = false;
            }
            return isValid;
        }

        private bool IsTargetSettingsValid()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtTargetDatabase.Text) || string.IsNullOrWhiteSpace(txtTargetServer.Text) || string.IsNullOrWhiteSpace(txtTargetUsername.Text) ||
                string.IsNullOrWhiteSpace(txtTargetPassword.Text) || string.IsNullOrWhiteSpace(txtTargetDbType.EditValue.ToString()))
            {
                isValid = false;
            }
            return isValid;
        }

        private Item GetItem(UnitOfWork uow)
        {
            Guid Oid;
            Guid.TryParse("70CBBA18-75FC-43E6-B6FD-0033F32D4AA4", out Oid);
            return uow.GetObjectByKey<Item>(Oid);
        }

        private async void btnCreateTargetSchema_Click(object sender, EventArgs e)
        {
            try
            {
                if (TargetDbHelper == null)
                {
                    if (IsTargetSettingsValid())
                    {
                        CreateTargetDbHelper();
                    }
                    if (TargetDbHelper == null)
                    {
                        throw new Exception("Fill Conection Settings");
                    }
                }

                SplashScreenManager.ShowDefaultWaitForm("Database Operation", "Creating Schema");
                await DbTransferService.UpdateSchema(TargetDbHelper, SourceDbHelper);
                SplashScreenManager.CloseDefaultWaitForm();
                XtraMessageBox.Show("Schema Created Successfully", "Success");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error");
                Program.LogFile.Error(ex.Message);
            }
        }

        //private async void btnTruncateTargetTables_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (TargetDbHelper == null)
        //        {
        //            if (IsTargetSettingsValid())
        //            {
        //                CreateTargetDbHelper();
        //            }
        //            if (TargetDbHelper == null)
        //            {
        //                throw new Exception("Fill Conection Settings");
        //            }
        //        }

        //        SplashScreenManager.ShowDefaultWaitForm("Database Operation", "Creating Schema");
        //        string result = await DbTransferService.TruncateTables(TargetDbHelper);
        //        SplashScreenManager.CloseDefaultWaitForm();
        //        XtraMessageBox.Show(result, "Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show(ex.Message, "Error");
        //    }
        //}

        private async void btnTransferData_Click(object sender, EventArgs e)
        {
            try
            {
                if (SourceDbHelper == null)
                {
                    if (IsSourceSettingsValid())
                    {
                        CreateSourceDbHelper();
                    }
                    if (SourceDbHelper == null)
                    {
                        throw new Exception("Fill Conection Settings");
                    }
                }
                if (TargetDbHelper == null)
                {
                    if (IsTargetSettingsValid())
                    {
                        CreateTargetDbHelper();
                    }
                    if (TargetDbHelper == null)
                    {
                        throw new Exception("Fill Conection Settings");
                    }
                }
                bool startFromCurrentVersion;
                bool.TryParse(radioVersion.EditValue.ToString(), out startFromCurrentVersion);
                int totalRows = DbTransferService.CalculateTotalRowsToTransfer(SourceDbHelper, TargetDbHelper, startFromCurrentVersion);
                SplashScreenManager.ShowDefaultWaitForm(this, true, true, "Database Operation", "Transfering Data");
                await DbTransferService.TransferDatabase(SourceDbHelper, TargetDbHelper, startFromCurrentVersion, totalRows);
                SplashScreenManager.CloseDefaultWaitForm();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error");
                Program.LogFile.Error(ex.Message);
            }
        }

        private async void OnDatabaseUpdateEventHandler(int totalRows, int rowsTransfered, string currenttable, int remainingrows = 0)
        {
            try
            {

                SplashScreenManager.Default.SetWaitFormCaption("Transfering Table " + Environment.NewLine + currenttable);
                if (remainingrows > 0)
                {
                    SplashScreenManager.Default.SetWaitFormDescription(rowsTransfered + " Rows Transfered" + Environment.NewLine +
                        (totalRows - remainingrows) + @"/" + totalRows);
                }
                SplashScreenManager.Default.Invalidate();
            }
            catch (Exception ex)
            {
                Program.LogFile.Error(ex.Message);
            }
        }

    }
}
