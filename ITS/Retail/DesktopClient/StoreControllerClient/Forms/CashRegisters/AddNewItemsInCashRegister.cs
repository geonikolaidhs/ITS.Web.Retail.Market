using DevExpress.Xpo;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using System;
using ITS.Retail.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using System.IO;
using Ionic.Zip;
using DevExpress.XtraEditors;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model.SupportingClasses;
using Newtonsoft.Json;
using ITS.POS.Hardware;
using ITS.Retail.WebClient.Helpers.Factories;
using ITS.Retail.Platform.Enumerations;
using System.Threading;
using DevExpress.XtraSplashScreen;
using DevExpress.Utils.Serializing;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters
{
    public partial class AddNewItemsInCashRegister : XtraLocalizedForm
    {
        List<CashRegisterItemUpdate> AllPoses;

        public AddNewItemsInCashRegister()
        {
            InitializeComponent();
            if (!String.IsNullOrEmpty(Program.Settings.CashierMultithredCardViewSettings)) RestoreLayout(grvMain, Program.Settings.CashierMultithredCardViewSettings);
            loadDevices();
        }
        private void loadDevices()
        {
            try
            {
                using (System.Net.WebClient myWebClient = new System.Net.WebClient())
                {
                    string file_url = Program.Settings.StoreControllerURL + "/POS/PosDeviceVersion.txt";
                    string versionString = myWebClient.DownloadString(file_url);
                    long newVersion = long.Parse(versionString);
                    long oldVersion = 0;
                    try
                    {
                        if (Directory.Exists(Application.StartupPath + "\\PosDeviceDatabase"))
                        {
                            if (File.Exists(Application.StartupPath + "\\PosDeviceDatabase\\PosDeviceVersion.txt"))
                            {
                                oldVersion = long.Parse(System.IO.File.ReadAllText(Application.StartupPath + "\\PosDeviceDatabase\\PosDeviceVersion.txt"));
                            }
                        }
                        else Directory.CreateDirectory(Application.StartupPath + "\\PosDeviceDatabase");
                    }
                    catch { }
                    if (newVersion > oldVersion)
                    {
                        string[] files = Directory.GetFiles(Application.StartupPath + "\\PosDeviceDatabase");
                        foreach (string item in files)
                        {
                            File.Delete(item);
                        }
                        file_url = Program.Settings.StoreControllerURL + "/POS/POSDeviceItems.zip";
                        myWebClient.DownloadFile(file_url, Application.StartupPath + "\\PosDeviceDatabase\\POSDeviceItems.zip");
                        Unzip(Application.StartupPath + "\\PosDeviceDatabase\\POSDeviceItems.zip", ExtractExistingFileAction.OverwriteSilently, ".\\PosDeviceDatabase");
                        if (File.Exists(Application.StartupPath + "\\PosDeviceDatabase\\POSDeviceItems.zip"))
                        {
                            File.Delete(Application.StartupPath + "\\PosDeviceDatabase\\POSDeviceItems.zip");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(Resources.PosDeviceDatabaseDownloadError + " " + ex.Message);
            }
            try
            {
                //ITS.Retail.Model.POS
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                AllPoses = new XPCollection<Model.POSDevice>(uow)
                            .Where(x => x.DeviceSettings.DeviceType == Platform.Enumerations.DeviceType.RBSElioWebCashRegister && x.TerminalDeviceAssociations.FirstOrDefault().Terminal.Store.Oid == Program.Settings.StoreControllerSettings.Store.Oid)
                            .Select(s => new Model.SupportingClasses.CashRegisterItemUpdate()
                            {
                                POSName = s.TerminalDeviceAssociations.FirstOrDefault().Terminal.Name,
                                DeviceName = s.Name,
                                DeviceType = s.DeviceSettings.DeviceType.ToString(),
                                DeviceOid = s.Oid,
                                POSOid = s.TerminalDeviceAssociations.FirstOrDefault().Terminal.Oid,
                                LastSuccefullyUpdate = s.LastSuccefullyItemUpdate,
                                ItemCategoryOid = s.ItemCategory.Oid,
                                BarcodeTypeOid = s.BarcodeType.Oid,
                                PriceCatalogOid = s.PriceCatalog.Oid,
                                MaxItemsAdd = s.MaxItemsAdd
                            }).ToList();
                foreach (CashRegisterItemUpdate item in AllPoses)
                {
                    Helpers.CashierUpdateItems z = new CashierUpdateItems(item, this);
                    item.CashierUpdateItems = z;
                }
                //AllPosesBind = new BindingList<Model.SupportingClasses.CashRegisterItemUpdate>(AllPoses);
                //AllPosesObs = new System.Collections.ObjectModel.ObservableCollection<Model.SupportingClasses.CashRegisterItemUpdate>(AllPoses);
                grdMain.DataSource = AllPoses;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        private static void Unzip(string file, ExtractExistingFileAction defaultAction, string TargetDir = ".")
        {
            using (ZipFile zip = ZipFile.Read(file))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(TargetDir, defaultAction);
                }
            }
        }
        private void grvMain_CustomDrawCardCaption(object sender, DevExpress.XtraGrid.Views.Card.CardCaptionCustomDrawEventArgs e)
        {
            try
            {
                // DevExpress.XtraGrid.Views.Card.CardView view = sender as DevExpress.XtraGrid.Views.Card.CardView;
                (e.CardInfo as DevExpress.XtraGrid.Views.Card.ViewInfo.CardInfo).CaptionInfo.CardCaption = grvMain.GetRowCellDisplayText(e.RowHandle, grvMain.Columns["DeviceName"]);
            }
            catch { }

        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    item.Selected = true;
                }
                grdMain.RefreshDataSource();
            }
            catch (Exception ex)
            {


            }

        }
        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    item.Selected = false;
                }
                grdMain.RefreshDataSource();
            }
            catch (Exception ex)
            {


            }
        }
        private void btnSendItemsToCashiers_Click(object sender, EventArgs e)
        {
            try
            {
                long versionTicks = 0;
                List<CashDeviceItem> listOfItems;
                long UpdateFromTicks = 0;
                if (File.Exists(Application.StartupPath + "\\PosDeviceDatabase\\PosDeviceVersion.txt"))
                {
                    versionTicks = long.Parse(System.IO.File.ReadAllText(Application.StartupPath + "\\PosDeviceDatabase\\PosDeviceVersion.txt"));
                }
                bool selectedItems = false;
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        listOfItems = new List<CashDeviceItem>();
                        UpdateFromTicks = 0;
                        if (versionTicks > item.LastSuccefullyUpdate.Ticks)
                        {
                            if (File.Exists(Application.StartupPath + "\\PosDeviceDatabase\\" + item.ItemCategoryOid.ToString() + "_" + item.BarcodeTypeOid.ToString() + "_" + item.PriceCatalogOid.ToString() + ".json"))
                            {
                                listOfItems = JsonConvert.DeserializeObject<IEnumerable<CashDeviceItem>>(File.ReadAllText(Application.StartupPath + "\\PosDeviceDatabase\\" + item.ItemCategoryOid.ToString() + "_" + item.BarcodeTypeOid.ToString() + "_" + item.PriceCatalogOid.ToString() + ".json")) as List<CashDeviceItem>;
                                UpdateFromTicks = versionTicks;
                            }
                        }
                        ((Helpers.CashierUpdateItems)item.CashierUpdateItems).UpdateItems(listOfItems, UpdateFromTicks);
                        selectedItems = true;

                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void btnGetTotalSales_Click(object sender, EventArgs e)
        {
            try
            {
                bool selectedItems = false;
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        ((Helpers.CashierUpdateItems)item.CashierUpdateItems).GetDaylyTotals();
                        selectedItems = true;
                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void grvMain_CustomCardCaptionImage(object sender, DevExpress.XtraGrid.Views.Card.CardCaptionImageEventArgs e)
        {
            e.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Binoculars_32;
        }
        internal CashRegisterHardware GetCashRegister(ITS.Retail.Model.POS cashierDevice)
        {
            try
            {
                DeviceSettings settings = (cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice).DeviceSettings;
                CashRegisterFactory cashRegisterFactory = new CashRegisterFactory();
                ConnectionType connectionType = cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice.ConnectionType;
                CashRegisterHardware cashRegisterHardware = cashRegisterFactory.GetCashRegisterHardware(settings.DeviceType, settings, cashierDevice.Name, cashierDevice.ID, connectionType, settings.LineChars, settings.CommandChars);
                return cashRegisterHardware;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private void grvMain_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                CardHitInfo hitInfo = grvMain.CalcHitInfo(grdMain.PointToClient(Cursor.Position)) as CardHitInfo;
                if (hitInfo.HitTest == CardHitTest.CardCaption)
                {
                    CashRegisterItemUpdate row = grvMain.GetFocusedRow() as Model.SupportingClasses.CashRegisterItemUpdate;
                    CashierUpdateItems cashierUpdateItems = new Helpers.CashierUpdateItems(row, this);
                    cashierUpdateItems.GetItems();
                    ItemsToUpdateForm x = new ItemsToUpdateForm(cashierUpdateItems.CurrentItems);
                    x.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
            finally
            {
                SplashScreenManager.CloseForm(false);
            }
        }
        private void btnSendItemSales_Click(object sender, EventArgs e)
        {
            try
            {
                bool selectedItems = false;
                List<CashRegisterItemUpdate> SelectedPoses = new List<CashRegisterItemUpdate>();
                foreach (CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        SelectedPoses.Add(item);
                        selectedItems = true;
                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers);
                else
                {
                    try
                    {
                        DailyItemSales x = new DailyItemSales(this, SelectedPoses);
                        x.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void grvMain_Layout(object sender, EventArgs e)
        {
            try
            {
                Program.Settings.CashierMultithredCardViewSettings = MainForm.GetLayout(grvMain);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void RestoreLayout(ISupportXtraSerializer view, string setting)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(setting);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    view.RestoreLayoutFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Info(ex, "Error Restoring Layout");
            }
        }
        private void btnIssueX_Click(object sender, EventArgs e)
        {
            try
            {
                bool selectedItems = false;
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        ((Helpers.CashierUpdateItems)item.CashierUpdateItems).issuexReport();
                        selectedItems = true;
                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void btnInserAllPaymentTypes_Click(object sender, EventArgs e)
        {
            try
            {
                bool selectedItems = false;
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        ((Helpers.CashierUpdateItems)item.CashierUpdateItems).ProgramPaymentMethods();
                        selectedItems = true;
                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void btnCashierInfo_Click(object sender, EventArgs e)
        {
            try
            {
                bool selectedItems = false;
                string message = string.Empty;
                foreach (Model.SupportingClasses.CashRegisterItemUpdate item in AllPoses)
                {
                    if (item.Selected == true)
                    {
                        message += ((Helpers.CashierUpdateItems)item.CashierUpdateItems).DeviceInfo() + Environment.NewLine;
                        selectedItems = true;
                    }
                }
                if (!selectedItems) XtraMessageBox.Show(Resources.CashierPleaseSelectCashiers); else XtraMessageBox.Show(message);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
    }
}
