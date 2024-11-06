using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.Hardware.RBSPOSEliot.CashRegister;
using System.Globalization;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;
using ITS.POS.Hardware;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Common;
using DevExpress.XtraSplashScreen;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters
{
    public partial class DailyItemSales : XtraLocalizedForm
    {
        private List<CashRegisterItemUpdate> SelectedPoses;
        private List<ItemSales> _ItemSales = new List<ItemSales>();
        private AddNewItemsInCashRegister ParentForm;
        private Dictionary<eMinistryVatCategoryCode, Decimal> totals;
        public DailyItemSales(AddNewItemsInCashRegister ParentForm, List<CashRegisterItemUpdate> SelectedPoses)
        {
            InitializeComponent();
            this.SelectedPoses = SelectedPoses;
            //items = ItemSales;
            this.ParentForm = ParentForm;
            PopulateItemsInGrid();
            this.MinimizeBox = true;
            this.CloseBox = true;
            this.Text = ResourcesLib.Resources.DailyItemSales;
        }

        private void CashRegister_Load(object sender, EventArgs e)
        {

        }

        private void btnAddItemToDevice_Click(object sender, EventArgs e)
        {

        }
        private void PopulateItemsInGrid()
        {
            _ItemSales.Clear();
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            XPCollection<VatFactor> vatfactors = new XPCollection<VatFactor>(uow, CriteriaOperator.And(new BinaryOperator("VatLevel.IsDefault", true)));
            foreach (CashRegisterItemUpdate item in SelectedPoses)
            {
                try
                {
                    using (CashRegisterHardware cashRegisterHardware = ParentForm.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(item.POSOid)))
                    {
                        //, VatDepartment=vats.VatCategory.MinistryVatCategoryCode
                        List<vatForList> CashierVats = (from vatmaps in new XPCollection<MapVatFactor>(uow, CriteriaOperator.And(new BinaryOperator("CashRegisterDevice", item.DeviceOid, BinaryOperatorType.Equal)))
                                                        join vats in vatfactors on vatmaps.Item.VatCategory equals vats.VatCategory

                                                        select new vatForList { VatPercent = vats.Factor * 100, CashierDepartment = int.Parse(vatmaps.DeviceVatLevel), ItemOid = vatmaps.Item.Oid, VatFactorOid = vats.Oid, VatCategoryOid = vats.VatCategory.Oid }).ToList();
                        CashierVatRates cashierVatRates = new CashierVatRates();
                        if (cashRegisterHardware.GetVatRates(cashierVatRates) != eDeviceCheckResult.SUCCESS)
                            throw new Exception(Resources.ErrorReadingFromCashier);
                        foreach (vatForList v in CashierVats)
                        {
                            if (v.VatPercent == cashierVatRates.VatRateA) v.VatDepartment = eMinistryVatCategoryCode.A; else if (v.VatPercent == cashierVatRates.VatRateB) v.VatDepartment = eMinistryVatCategoryCode.B; else if (v.VatPercent == cashierVatRates.VatRateC) v.VatDepartment = eMinistryVatCategoryCode.C; else if (v.VatPercent == cashierVatRates.VatRateD) v.VatDepartment = eMinistryVatCategoryCode.D; else if (v.VatPercent == cashierVatRates.VatRateE) v.VatDepartment = eMinistryVatCategoryCode.E;

                        }
                        try
                        {
                            string message = string.Empty;
                            //cashRegisterHardware.GetDailySalesOfItem(out message);
                            List<string> mashineItemSalesResults = new List<string>();
                            //mashineItemSalesResults.Add(message);
                            bool ExceptionHappent = false;
                            do
                            {
                                try
                                {
                                    cashRegisterHardware.GetDailySalesOfItem(out message);
                                    if (message.Contains("The requested fiscal record number is wrong"))
                                    {
                                        break;
                                    }
                                    mashineItemSalesResults.Add(message);
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHappent = true;
                                    break;
                                    //continue;
                                }
                            }
                            while (!ExceptionHappent);

                            string message2 = "";
                            cashRegisterHardware.GetTotalSalesOfDay(out message2);
                            DailyTotal total = cashRegisterHardware.CalculateDaylyTotals(message2);

                            ConvertSales(mashineItemSalesResults, item, total, cashRegisterHardware, CashierVats, uow);
                            //DailyItemSales DailyTotalItemSales = new DailyItemSales(mashineItemSalesResults);
                            //DailyTotalItemSales.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(String.Format(Resources.CashierErrorReadingSales, item.DeviceName, ex.Message));
                }

            }
            gridSaleItems.DataSource = _ItemSales;
            gridSaleItems.Refresh();
        }
        private void ConvertSales(List<string> items, CashRegisterItemUpdate pos, DailyTotal total, CashRegisterHardware cashRegisterHardware, List<vatForList> VatList, UnitOfWork uow)
        {
            decimal vata = 0; decimal vatb = 0; decimal vatc = 0; decimal vatd = 0; decimal vate = 0;
            foreach (string item in items)
            {
                if (item == "")
                {
                    continue;
                }
                string[] currentItem = item.Split('/');
                ItemSales currentItemSale = new ItemSales();

                int vatCode = 0;
                int deviceIndex = 0;
                int points = 0;
                decimal price = 0;
                decimal Qty = 0;
                decimal soldQty = 0;
                decimal totalSalesAmount = 0;
                //convertions
                int.TryParse(currentItem[2], out vatCode);
                int.TryParse(currentItem[3], out deviceIndex);
                int.TryParse(currentItem[7], out points);
                decimal.TryParse(currentItem[4].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out price);
                decimal.TryParse(currentItem[8].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out Qty);
                decimal.TryParse(currentItem[9].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out soldQty);
                decimal.TryParse(currentItem[10].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out totalSalesAmount);

                currentItemSale.deviceIndex = deviceIndex;
                currentItemSale.Code = currentItem[0];
                currentItemSale.Code = currentItemSale.Code.Trim();
                currentItemSale.Description = currentItem[1];
                currentItemSale.VatCode = vatCode;
                currentItemSale.price = price;
                currentItemSale.Points = points;
                currentItemSale.SoldQTY = soldQty;
                currentItemSale.TotalSalesAmount = totalSalesAmount;
                currentItemSale.Qty = Qty;
                if (deviceIndex != 0)
                {
                    try
                    {
                        currentItemSale.ItemOid = new XPCollection<Barcode>(uow, CriteriaOperator.And(new BinaryOperator("Code", currentItemSale.Code))).Select(s => s.ItemBarcodes.FirstOrDefault().Item).ToList().FirstOrDefault().Oid;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception (Resources.ItemNotFound);
                    }
                }
                foreach (vatForList VatListItem in VatList)
                {
                    if (vatCode == VatListItem.CashierDepartment)
                    {
                        currentItemSale.VatPercent = VatListItem.VatPercent;
                        currentItemSale.MinistryVatCategoryCode = VatListItem.VatDepartment;
                        currentItemSale.VatCategoryOid = VatListItem.VatCategoryOid;
                        currentItemSale.VatFactorOid = VatListItem.VatFactorOid;
                        break;
                    }
                }
                if (totalSalesAmount > 0)
                {
                    currentItemSale.NetValue = Math.Round((totalSalesAmount / (1 + (currentItemSale.VatPercent / 100))), 2, MidpointRounding.AwayFromZero);
                    currentItemSale.VatValue = totalSalesAmount - currentItemSale.NetValue;
                }
                currentItemSale.DeviceOid = pos.DeviceOid;
                currentItemSale.DeviceName = pos.DeviceName;
                bool test = false;
                switch (currentItemSale.MinistryVatCategoryCode)
                {
                    case eMinistryVatCategoryCode.NONE:
                        throw new Exception(Resources.CashierDeviceFailure);
                        break;
                    case eMinistryVatCategoryCode.A:
                        vata += currentItemSale.TotalSalesAmount;
                        test = true;
                        break;
                    case eMinistryVatCategoryCode.B:
                        vatb += currentItemSale.TotalSalesAmount;
                        test = true;
                        break;
                    case eMinistryVatCategoryCode.C:
                        vatc += currentItemSale.TotalSalesAmount;
                        test = true;
                        break;
                    case eMinistryVatCategoryCode.D:
                        vatd += currentItemSale.TotalSalesAmount;
                        test = true;
                        break;
                    case eMinistryVatCategoryCode.E:
                        vate += currentItemSale.TotalSalesAmount;
                        test = true;
                        break;
                    default:
                        throw new Exception(Resources.CashierDeviceFailure);
                        break;
                }

                _ItemSales.Add(currentItemSale);
            }
            decimal totalnet = 0;
            if (vata != total.VatCategoryA)
            {
                totalnet = Math.Round((total.VatCategoryA - vata) / (1 + VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.A).Select(s => s.VatPercent / 100).FirstOrDefault()), 2);
                _ItemSales.Add(new ItemSales() { Code = "A", deviceIndex = 0, Description = Resources.VatCategoryA, DeviceName = pos.DeviceName, DeviceOid = pos.DeviceOid, IsAvaledToSale = true, IsChecked = true, Points = 0, price = 0, Qty = 0, SoldQTY = 1, TotalSalesAmount = total.VatCategoryA - vata, NetValue = totalnet, VatValue = total.VatCategoryA - vata - totalnet, ItemOid = VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.A).Select(s => s.ItemOid).FirstOrDefault() });
            }
            if (vatb != total.VatCategoryB)
            {
                totalnet = Math.Round((total.VatCategoryB - vatb) / (1 + VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.B).Select(s => s.VatPercent / 100).FirstOrDefault()), 2);
                _ItemSales.Add(new ItemSales() { Code = "B", deviceIndex = 0, Description = Resources.VatCategoryB, DeviceName = pos.DeviceName, DeviceOid = pos.DeviceOid, IsAvaledToSale = true, IsChecked = true, Points = 0, price = 0, Qty = 0, SoldQTY = 1, TotalSalesAmount = total.VatCategoryB - vatb, NetValue = totalnet, VatValue = total.VatCategoryA - vata - totalnet, ItemOid = VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.B).Select(s => s.ItemOid).FirstOrDefault() });
            }
            if (vatc != total.VatCategoryC)
            {
                totalnet = Math.Round((total.VatCategoryC - vatc) / (1 + VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.C).Select(s => s.VatPercent / 100).FirstOrDefault()), 2);
                _ItemSales.Add(new ItemSales() { Code = "C", deviceIndex = 0, Description = Resources.VatCategoryC, DeviceName = pos.DeviceName, DeviceOid = pos.DeviceOid, IsAvaledToSale = true, IsChecked = true, Points = 0, price = 0, Qty = 0, SoldQTY = 1, TotalSalesAmount = total.VatCategoryC - vatc, NetValue = totalnet, VatValue = total.VatCategoryA - vata - totalnet, ItemOid = VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.C).Select(s => s.ItemOid).FirstOrDefault() });
            }
            if (vatd != total.VatCategoryD)
            {
                totalnet = Math.Round((total.VatCategoryD - vatd) / (1 + VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.D).Select(s => s.VatPercent / 100).FirstOrDefault()), 2);
                _ItemSales.Add(new ItemSales() { Code = "D", deviceIndex = 0, Description = Resources.VatCategoryD, DeviceName = pos.DeviceName, DeviceOid = pos.DeviceOid, IsAvaledToSale = true, IsChecked = true, Points = 0, price = 0, Qty = 0, SoldQTY = 1, TotalSalesAmount = total.VatCategoryD - vatd, NetValue = totalnet, VatValue = total.VatCategoryA - vata - totalnet, ItemOid = VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.D).Select(s => s.ItemOid).FirstOrDefault() });
            }
            if (vate != total.VatCategoryE)
            {
                totalnet = Math.Round((total.VatCategoryE - vate) / (1 + VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.E).Select(s => s.VatPercent / 100).FirstOrDefault()), 2);
                _ItemSales.Add(new ItemSales() { Code = "E", deviceIndex = 0, Description = Resources.VatCategoryD, DeviceName = pos.DeviceName, DeviceOid = pos.DeviceOid, IsAvaledToSale = true, IsChecked = true, Points = 0, price = 0, Qty = 0, SoldQTY = 1, TotalSalesAmount = total.VatCategoryE - vate, NetValue = totalnet, VatValue = total.VatCategoryA - vata - totalnet, ItemOid = VatList.Where(w => w.VatDepartment == eMinistryVatCategoryCode.E).Select(s => s.ItemOid).FirstOrDefault() });
            }
            //UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            //List<vatForList> CashierVats = new XPCollection<MapVatFactor>(uow, CriteriaOperator.And(new BinaryOperator("CashRegisterDevice", pos.DeviceOid, BinaryOperatorType.Equal))).Select(s => new vatForList { VatPercent = s.VatFactor.Factor * 100, CashierDepartment = int.Parse(s.DeviceVatLevel) }).ToList();
            //foreach (vatForList vat in CashierVats)
            //{
            //    if (vat.CashierDepartment == currentItemSale.VatCode)
            //    {

            //    }
            //}
        }
        private CashierDepartment GetCashierDepartmentFromMessage(string message)
        {

            string[] spl = message.Split('/');
            CashierDepartment x = new CashierDepartment();
            x.Description = spl[1];
            switch (spl[2])
            {
                case "1":
                    x.VatRateCode = eMinistryVatCategoryCode.A;
                    break;
                case "2":
                    x.VatRateCode = eMinistryVatCategoryCode.B;
                    break;
                case "3":
                    x.VatRateCode = eMinistryVatCategoryCode.C;
                    break;
                case "4":
                    x.VatRateCode = eMinistryVatCategoryCode.D;
                    break;
                case "5":
                    x.VatRateCode = eMinistryVatCategoryCode.E;
                    break;
                default:
                    break;
            }

            return x;
        }
        private void btnSendDocument_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (_ItemSales.Count < 0)
            //    {
            //        //TODO message box
            //        return;
            //    }
            //    DocumentType documentType = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentType>(Guid.Parse("A2CDD631-AA2F-44B8-A541-C34AF12E1F4B"));
            //    DocumentSeries documentSeries = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentSeries>(Guid.Parse("DCD3FA9E-827B-45EB-A173-3FF0B5AFD33D"));
            //    DocumentStatus documentStatus = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentStatus>(Guid.Parse("3150C8AB-F725-4E15-A143-65CFFF9A7084"));
            //    DocumentHelper.ComputeDocumentHeader(_ItemSales, documentType, documentSeries, documentStatus, Program.Settings.StoreControllerSettings.DefaultCustomer, Program.Settings.StoreControllerSettings.Store,Program.Settings.CurrentUser);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void btnSendSales_Click(object sender, EventArgs e)
        {
            string Errors = string.Empty;
            try
            {
                SplashScreenManager.ShowForm(this, typeof(ITSWaitForm), true, true, false);
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                foreach (CashRegisterItemUpdate posdev in SelectedPoses)
                {
                    try
                    {
                        UserDailyTotals userDailyTotals = null;
                        try
                        {
                            userDailyTotals = Helpers.CashierDocumentHelper.SaveDocument(posdev, _ItemSales);
                        }
                        catch (Exception ex)
                        {

                            throw new Exception(string.Format(Resources.CashierDeviceSendingSalesDocumentError, posdev.DeviceName, ex.Message));
                        }
                        try
                        {
                            if (userDailyTotals != null)
                            {
                                using (CashRegisterHardware cashRegisterHardware = ParentForm.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(posdev.POSOid)))
                                {
                                    string message = string.Empty;
                                    if (cashRegisterHardware.IssueXReport(out message) == eDeviceCheckResult.SUCCESS)
                                    {
                                        if (cashRegisterHardware.IssueZReport(out message) != eDeviceCheckResult.SUCCESS)
                                        {
                                            throw new Exception(string.Format(Resources.CashierDevicePrintingZReportError, posdev.DeviceName, message));
                                        }
                                        else
                                        {
                                            userDailyTotals.DailyTotals.ZReportNumber = int.Parse(message);
                                            userDailyTotals.Save();
                                            userDailyTotals.Session.CommitTransaction();
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(string.Format(Resources.CashierDevicePrintingXReportError, posdev.DeviceName, message));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format(Resources.CashierDevicePrintingZReportError, posdev.DeviceName, ex.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        Errors += ex + Environment.NewLine;
                    }

                }
                SplashScreenManager.CloseForm();
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(Environment.NewLine + ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (Errors != string.Empty)
                XtraMessageBox.Show(Errors);
            this.Close();
            this.Dispose();
        }


    }
}