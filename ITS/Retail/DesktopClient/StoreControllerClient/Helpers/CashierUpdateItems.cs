using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.POS.Hardware;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public class CashierUpdateItems
    {
        private Model.SupportingClasses.CashRegisterItemUpdate itemUpdate;
        private Forms.CashRegisters.AddNewItemsInCashRegister Form;
        private DevExpress.XtraGrid.GridControl Grid;
        private UnitOfWork uow;
        private List<CashDeviceItem> AdditionalItems;
        private long FromTicks;
        private bool ReadingItemsFromDatabase = false;
        private readonly static object lockObject = new object();
        private readonly static object lockObjectForCOM = new object();
        public List<CashDeviceItem> CurrentItems { get; set; }
        private List<vatForList> CashierVats;
        public string ItemsGettingError { get; set; }
        public CashierUpdateItems(Model.SupportingClasses.CashRegisterItemUpdate itemUpdate, Forms.CashRegisters.AddNewItemsInCashRegister form)
        {
            try
            {
                this.itemUpdate = itemUpdate;
                this.Form = form;
                Grid = form.grdMain;
                uow = XpoHelper.GetNewUnitOfWork();

            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }

        }
        private int GetCashierDepartment(Decimal VatPercent)
        {
            foreach (vatForList item in CashierVats)
            {
                if (item.VatPercent == VatPercent)
                {
                    return item.CashierDepartment;
                }
            }
            return 1;
        }
        internal bool RunningUpdate = false;
        private void UpdateItemsInThread()
        {
            try
            {

                uow = XpoHelper.GetNewUnitOfWork();
                XPCollection<VatFactor> vatfactors = new XPCollection<VatFactor>(uow, CriteriaOperator.And(new BinaryOperator("VatLevel.IsDefault", true)));
                CashierVats = (from vatmaps in new XPCollection<MapVatFactor>(uow, CriteriaOperator.And(new BinaryOperator("CashRegisterDevice", itemUpdate.DeviceOid, BinaryOperatorType.Equal)))
                               join vats in vatfactors on vatmaps.Item.VatCategory equals vats.VatCategory
                               select new vatForList { VatPercent = vats.Factor * 100, CashierDepartment = int.Parse(vatmaps.DeviceVatLevel) }).ToList()
                              ;
                using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
                {
                    try
                    {
                        itemUpdate.Running = true;
                        DateTime now = DateTime.Now;
                        getItemsInThread();
                        decimal lastpercentage = 0;
                        string ItemsCount = AdditionalItems.Count().ToString();
                        int ItemsCountInt = AdditionalItems.Count();
                        int curItemcounter = 0;
                        bool error = false;
                        int errorcount = 0;
                        bool newItem;
                        string errorMessage = "";
                        UpdateCurrentAction(ResourcesLib.Resources.CashierUpdateDownloadedItems);
                        if (ItemsCountInt > 0)
                        {
                            curItemcounter = 0;
                            foreach (CashDeviceItem item in AdditionalItems)
                            {
                                UpdateStatus(string.Format(ResourcesLib.Resources.FromToString, (++curItemcounter).ToString(), ItemsCount));
                                if (lastpercentage < Math.Round((decimal)(curItemcounter * 100 / ItemsCountInt), 0))
                                {
                                    lastpercentage = Math.Round((decimal)(curItemcounter * 100 / ItemsCountInt), 0);
                                    UpdateProgressBar(lastpercentage);
                                }
                                errorcount = 0;
                                newItem = true;
                                while ((error && errorcount < 3) || newItem)
                                {
                                    newItem = false;
                                    try
                                    {
                                        if (itemUpdate.MaxItemsAdd < item.deviceIndex) throw new Exception(ResourcesLib.Resources.MaxCashierItemsExeeded);
                                        cashRegisterHardware.SendItemToDevice(item.deviceIndex, item.Code, item.Description, (double)(item.price < 0 ? 0 : item.price), (double)(item.price < 0 ? 0 : item.price) * 100, (double)(item.price < 0 ? 0 : item.price), 0, (double)item.Qty, GetCashierDepartment(item.VatPercent), out errorMessage);
                                        error = false;
                                        itemUpdate.LastSendedItemIndex = item.deviceIndex;
                                    }
                                    catch (Exception ex)
                                    {
                                        error = true;
                                        errorcount++;
                                        if (errorcount == 3) throw; else UpdateStatus(ex.Message + " (" + errorcount.ToString() + ")");
                                    }
                                }
                            }
                        }
                        while (ReadingItemsFromDatabase)
                        {
                            UpdateCurrentAction(ResourcesLib.Resources.ReadingItemsFromDatabase);
                            Thread.Sleep(100);
                        }
                        lastpercentage = 0;
                        ItemsCount = CurrentItems.Count().ToString();
                        ItemsCountInt = CurrentItems.Count();
                        curItemcounter = 0;
                        lastpercentage = 0;
                        UpdateCurrentAction(ResourcesLib.Resources.SendingChangedItems);
                        foreach (CashDeviceItem item in CurrentItems)
                        {

                            UpdateStatus(string.Format(ResourcesLib.Resources.FromToString, (++curItemcounter).ToString(), ItemsCount));
                            if (lastpercentage < Math.Round((decimal)(curItemcounter * 100 / ItemsCountInt), 0))
                            {
                                lastpercentage = Math.Round((decimal)(curItemcounter * 100 / ItemsCountInt), 0);
                                UpdateProgressBar(lastpercentage);
                            }

                            errorcount = 0;
                            newItem = true;
                            while ((error && errorcount < 3) || newItem)
                            {
                                newItem = false;
                                try
                                {
                                    if (itemUpdate.MaxItemsAdd < item.deviceIndex) throw new Exception(ResourcesLib.Resources.MaxCashierItemsExeeded);
                                    cashRegisterHardware.SendItemToDevice(item.deviceIndex, item.Code, item.Description, (double)(item.price < 0 ? 0 : item.price), (double)(item.price < 0 ? 0 : item.price) * 100, (double)(item.price < 0 ? 0 : item.price), 0, (double)item.Qty, GetCashierDepartment(item.VatPercent), out errorMessage);
                                    error = false;
                                    itemUpdate.LastSendedItemIndex = item.deviceIndex;
                                }
                                catch (Exception ex)
                                {
                                    error = true;
                                    errorcount++;
                                    if (errorcount == 3) throw; else UpdateStatus(ex.Message + " (" + errorcount.ToString() + ")");
                                }
                            }
                        }
                        if (AdditionalItems.Count() > 0)
                        {
                            var Categories = new XPCollection<Model.POSDevice>(uow)
                      .Where(x => x.Oid == itemUpdate.DeviceOid)
                      .Select(s => new { s.ItemCategory, s.BarcodeType, s.PriceCatalog }).Distinct().ToList();
                            List<ItemDeviceIndex> DeviceIndexes = (new XPCollection<ItemDeviceIndex>(uow, CriteriaOperator.And(new BinaryOperator("ItemCategory", Categories.FirstOrDefault().ItemCategory, BinaryOperatorType.Equal), new BinaryOperator("BarcodeType", Categories.FirstOrDefault().BarcodeType, BinaryOperatorType.Equal), new BinaryOperator("PriceCatalog", Categories.FirstOrDefault().PriceCatalog, BinaryOperatorType.Equal)))).ToList();
                            int Maxid = (int)DeviceIndexes.Max(m => m.DeviceIndex);
                            ItemsCount = (itemUpdate.MaxItemsAdd - Maxid).ToString();
                            ItemsCountInt = itemUpdate.MaxItemsAdd - Maxid;
                            lastpercentage = 0;
                            UpdateCurrentAction(ResourcesLib.Resources.ClearingOldPossitions);
                            for (int i = 0; i < ItemsCountInt; i++)
                            {
                                Maxid++;
                                UpdateStatus(string.Format(ResourcesLib.Resources.FromToString, (Maxid).ToString(), ItemsCount));
                                if (lastpercentage < Math.Round((decimal)(i * 100 / ItemsCountInt), 0))
                                {
                                    lastpercentage = Math.Round((decimal)(i * 100 / ItemsCountInt), 0);
                                    UpdateProgressBar(lastpercentage);
                                }
                                errorcount = 0;
                                newItem = true;
                                while ((error && errorcount < 3) || newItem)
                                {
                                    newItem = false;
                                    try
                                    {

                                        var PLUCode = Maxid.ToString().PadLeft(15 - Maxid.ToString().Length, '0');
                                        cashRegisterHardware.SendItemToDevice(Maxid,
                                                                             PLUCode,
                                                                            PLUCode,
                                                                             0.00,
                                                                             0.00,
                                                                             0.00,
                                                                             0,
                                                                             0.00,
                                                                             1,
                                                                             out errorMessage);
                                        error = false;
                                        itemUpdate.LastSendedItemIndex = Maxid;
                                    }
                                    catch (Exception ex)
                                    {
                                        error = true;
                                        errorcount++;
                                        if (errorcount == 3) throw; else UpdateStatus(ex.Message + " (" + errorcount.ToString() + ")");
                                    }
                                }
                            }
                        }
                        POSDevice device = uow.GetObjectByKey<POSDevice>(itemUpdate.DeviceOid);
                        device.LastSuccefullyItemUpdate = now;
                        device.Save();
                        uow.CommitChanges();
                        UpdateLastDateTime(now);
                        UpdateStatus(ResourcesLib.Resources.UpdatingItemsDone);
                        UpdateProgressBar(100);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Program.Logger.Error(ex);
                            UpdateStatus(ex.Message);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Program.Logger.Error(ex);
                    UpdateStatus(ex.Message);
                }
                catch { }
            }
            RunningUpdate = false;
            itemUpdate.Running = false;
        }
        public delegate void UpdateCurrentActionDelegate(string Action);
        private void UpdateCurrentAction(string Action)
        {
            if (Grid.InvokeRequired)
            {
                Grid.BeginInvoke(new UpdateCurrentActionDelegate(UpdateCurrentAction), new Object[] { Action });
            }
            else
            {
                try
                {
                    itemUpdate.CurrentAction = Action;
                    itemUpdate.Progress = "-";
                    itemUpdate.ProgressPercent = 0;
                    Grid.RefreshDataSource();
                }
                catch { }

            }
        }
        internal void UpdateItems(List<CashDeviceItem> AdditionalItems, long FromTicks)
        {
            try
            {
                bool runNow = false;
                lock (lockObject)
                {

                    if (RunningUpdate == false && ReadingFromCashier == false)
                    {
                        RunningUpdate = true;
                        runNow = true;
                    }

                }
                if (runNow == true)
                {
                    this.AdditionalItems = AdditionalItems;
                    this.FromTicks = FromTicks;
                    (new Thread(new ThreadStart(UpdateItemsInThread))).Start();
                }
                else XtraMessageBox.Show(ResourcesLib.Resources.AnOtherActionIsWorkingWithThisDevice);

            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }

        }
        public delegate void UpdateLastDateTimeDelegate(DateTime newdate);
        private void UpdateLastDateTime(DateTime newdate)
        {
            if (Grid.InvokeRequired)
            {
                Grid.BeginInvoke(new UpdateLastDateTimeDelegate(UpdateLastDateTime), new Object[] { newdate });

            }
            else
            {
                try
                {
                    itemUpdate.LastSuccefullyUpdate = newdate;
                    Grid.RefreshDataSource();
                }
                catch { }

            }
        }
        public delegate void UpdateStatusDelegate(string text);
        private void UpdateStatus(string text)
        {
            if (Grid.InvokeRequired)
            {
                Grid.BeginInvoke(new UpdateStatusDelegate(UpdateStatus), new Object[] { text });

            }
            else
            {
                try
                {
                    itemUpdate.Progress = text;
                    Grid.RefreshDataSource();
                }
                catch { }

            }
        }
        public delegate void UpdateProgressBarDelegate(decimal progress);
        private void UpdateProgressBar(decimal progress)
        {
            if (Grid.InvokeRequired)
            {
                Grid.BeginInvoke(new UpdateProgressBarDelegate(UpdateProgressBar), new Object[] { progress });
            }
            else
            {
                try
                {
                    itemUpdate.ProgressPercent = progress;
                    Grid.RefreshDataSource();
                }
                catch { }

            }
        }
        private void getItemsInThread()
        {
            ReadingItemsFromDatabase = true;
            (new Thread(new ThreadStart(GetItems))).Start();
        }
        public void GetItems()
        {

            try
            {

                lock (lockObject)
                {
                    ItemsGettingError = "";
                    FromTicks = this.FromTicks != 0 ? this.FromTicks : itemUpdate.LastSuccefullyUpdate.Ticks;
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    var Categories = new XPCollection<Model.POSDevice>(uow)
                        .Where(x => x.Oid == itemUpdate.DeviceOid)
                        .Select(s => new { s.ItemCategory, s.BarcodeType, s.PriceCatalog }).Distinct().ToList();
                    var vatcats = new XPCollection<VatFactor>(uow, CriteriaOperator.And(new BinaryOperator("VatLevel", Program.Settings.StoreControllerSettings.DefaultCustomer.VatLevel.Oid))).Select(s => new { VatCategoryOid = s.VatCategory.Oid, Factor = s.Factor * 100 }).ToList();
                    foreach (var item in Categories)
                    {
                        int decimaldigits = (int)item.BarcodeType.Owner.OwnerApplicationSettings.DisplayDigits;
                        List<ItemDeviceIndex> DeviceIndexes = (new XPCollection<ItemDeviceIndex>(uow, CriteriaOperator.And(new BinaryOperator("ItemCategory", item.ItemCategory, BinaryOperatorType.Equal), new BinaryOperator("BarcodeType", item.BarcodeType, BinaryOperatorType.Equal), new BinaryOperator("PriceCatalog", item.PriceCatalog, BinaryOperatorType.Equal)))).ToList();
                        List<Item> eidix = (from eidi in new XPCollection<Item>(uow, CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", FromTicks, BinaryOperatorType.Greater), CriteriaOperator.Or(new BinaryOperator("IsTax", false, BinaryOperatorType.Equal), new NullOperator("IsTax")))) select eidi)
                        .Union(from eidi in new XPCollection<PriceCatalogDetail>(uow, CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", FromTicks, BinaryOperatorType.Greater), new BinaryOperator("PriceCatalog.Oid", Categories.FirstOrDefault().PriceCatalog.Oid, BinaryOperatorType.Equal), CriteriaOperator.Or(new BinaryOperator("Item.IsTax", false, BinaryOperatorType.Equal), new NullOperator("Item.IsTax")))) select eidi.Item)
                        .Union(from eidi in new XPCollection<PriceCatalogDetailTimeValue>(uow, CriteriaOperator.And(CriteriaOperator.Or(new BinaryOperator("UpdatedOnTicks", FromTicks, BinaryOperatorType.Greater), new BinaryOperator("TimeValueChangedOn", FromTicks, BinaryOperatorType.Greater), new BetweenOperator("TimeValueValidFrom", FromTicks, DateTime.Now.Ticks), new BetweenOperator("TimeValueValidUntil", FromTicks, DateTime.Now.Ticks)), new BinaryOperator("PriceCatalogDetail.PriceCatalog.Oid", Categories.FirstOrDefault().PriceCatalog.Oid, BinaryOperatorType.Equal), CriteriaOperator.Or(new BinaryOperator("PriceCatalogDetail.Item.IsTax", false, BinaryOperatorType.Equal), new NullOperator("PriceCatalogDetail.Item.IsTax")))) select eidi.PriceCatalogDetail.Item)
                        .Union(from eidi in item.ItemCategory.GetAllNodeTreeItems<ItemAnalyticTree>() where eidi.UpdatedOnTicks >= FromTicks select eidi.Object).Distinct().ToList();
                        List<ItemBarcode> AllBarcodes = (from u in new XPCollection<ItemBarcode>(uow, CriteriaOperator.And(new BinaryOperator("Type.Oid", item.BarcodeType.Oid, BinaryOperatorType.Equal)))
                                                         select u).ToList();
                        var grouppedBarCodes = (
                                                        from u in AllBarcodes
                                                        group u by u.Item into g
                                                        select new { Item = g.Key, MaxCode = g.Max(x => x.Barcode.Code) }
                                                     ).ToList();
                        List<ItemBarcode> barx = (from b in grouppedBarCodes
                                                  join z in AllBarcodes on new { b.Item, b.MaxCode } equals new { z.Item, MaxCode = z.Barcode.Code }
                                                  select z).ToList();
                        var itemsInCats = (from iat in item.ItemCategory.GetAllNodeTreeItems<ItemAnalyticTree>() select new { Oid = iat.Object.Oid });
                        var ItemsToUpdate = (from eidi in eidix
                                             join bar in barx on eidi equals bar.Item
                                             where bar != null
                                             join iat in itemsInCats on eidi.Oid equals iat.Oid
                                             where iat != null
                                             join ind in DeviceIndexes on eidi equals ind.Item into ps
                                             from ind2 in ps.DefaultIfEmpty()
                                             select new { eidi, DeviceIndex = ind2 == null ? 0 : ind2.DeviceIndex }
                                                      ).ToList();
                        int Maxid = (int)DeviceIndexes.Max(m => m.DeviceIndex);
                        foreach (var ItemToUpdate in ItemsToUpdate)
                        {
                            if (ItemToUpdate.DeviceIndex == null || ItemToUpdate.DeviceIndex == 0)
                            {
                                ItemDeviceIndex x = new ItemDeviceIndex(uow) { BarcodeType = item.BarcodeType, ItemCategory = item.ItemCategory, Item = ItemToUpdate.eidi, IsActive = true, DeviceIndex = ++Maxid, PriceCatalog = item.PriceCatalog };
                                x.Save();
                                DeviceIndexes.Add(x);
                            }
                        }
                        uow.CommitChanges();
                        CurrentItems = (from eidi in eidix
                                        join bar in barx on eidi equals bar.Item
                                        where bar != null
                                        join iat in itemsInCats on eidi.Oid equals iat.Oid
                                        where iat != null
                                        join vats in vatcats on eidi.VatCategory.Oid equals vats.VatCategoryOid
                                        join ind in DeviceIndexes on eidi equals ind.Item
                                        select new CashDeviceItem()
                                        {
                                            Code = bar.Barcode.Code,
                                            Description = eidi.Name,
                                            deviceIndex = ind.DeviceIndex,
                                            IsAvaledToSale = true,
                                            Qty = (decimal)bar.RelationFactor,
                                            VatPercent = vats.Factor,
                                            price = Math.Round(eidi.GetUnitPriceWithVat(item.PriceCatalog, bar.Barcode), decimaldigits, MidpointRounding.AwayFromZero)
                                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ItemsGettingError = ex.Message;
            }
            ReadingItemsFromDatabase = false;
        }
        bool ReadingFromCashier = false;
        internal void GetDaylyTotals()
        {
            try
            {
                bool runNow = false;
                lock (lockObject)
                {

                    if (RunningUpdate == false && ReadingFromCashier == false)
                    {
                        ReadingFromCashier = true;
                        runNow = true;
                    }

                }
                if (runNow == true)
                {
                    (new Thread(new ThreadStart(GetDaylyTotalsThread))).Start();
                }
                else XtraMessageBox.Show(ResourcesLib.Resources.AnOtherActionIsWorkingWithThisDevice);

            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void GetDaylyTotalsThread()
        {
            try
            {
                UpdateCurrentAction(ResourcesLib.Resources.UpdatingDailyTotals);
                using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
                {
                    bool succes = false;
                    int retries = 0;
                    while (!succes && retries < 10)
                    {
                        try
                        {
                            retries++;
                            string message = "";
                            cashRegisterHardware.GetTotalSalesOfDay(out message);
                            DailyTotal total = cashRegisterHardware.CalculateDaylyTotals(message);
                            UpdateDailyTotals(total);
                            succes = true;
                            UpdateCurrentAction(ResourcesLib.Resources.UpdatingDailyTotals + " - Ok");
                        }
                        catch (Exception ex)
                        {
                            UpdateStatus(ResourcesLib.Resources.UpdatingDailyTotals + "(" + retries.ToString() + ") " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    UpdateStatus(ex.Message);
                }
                catch { }
            }
            ReadingFromCashier = false;
        }
        public void GetDaylyTotalsNoThread()
        {
            using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
            {
                bool succes = false;
                int retries = 0;
                while (!succes && retries < 10)
                {
                    try
                    {
                        retries++;
                        string message = "";
                        cashRegisterHardware.GetTotalSalesOfDay(out message);
                        DailyTotal total = cashRegisterHardware.CalculateDaylyTotals(message);
                        itemUpdate.DayTotals = total;
                        succes = true;

                    }
                    catch { }
                }
            }
        }
        public delegate void UpdateDailyTotalsDelegate(DailyTotal total);
        private void UpdateDailyTotals(DailyTotal total)
        {
            if (Grid.InvokeRequired)
            {
                Grid.BeginInvoke(new UpdateDailyTotalsDelegate(UpdateDailyTotals), new Object[] { total });
            }
            else
            {
                try
                {
                    itemUpdate.DayTotals = total;
                    Grid.RefreshDataSource();
                }
                catch { }

            }
        }
        internal void issuexReport()
        {
            (new Thread(new ThreadStart(IssueXReportThread))).Start();
        }
        private void IssueXReportThread()
        {
            try
            {
                UpdateCurrentAction(Resources.ISSUE_X);
                using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
                {
                    string message = string.Empty;
                    cashRegisterHardware.IssueXReport(out message);
                    if (message == "")
                    {
                        UpdateCurrentAction(Resources.ISSUE_X_OK);
                    }
                    else
                    {
                        UpdateCurrentAction(Resources.ISSUE_X_OK + " - " + message);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateCurrentAction(Resources.ISSUE_X + " - " + ex.Message);
            }
        }
        internal string DeviceInfo()
        {
            string message = string.Empty;
            string GeneralInformationDevice = string.Empty;
            using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
            {
                cashRegisterHardware.GetDeviceInfo(out message);
                string[] result = message.Split('/');

                cashRegisterHardware.GetDeviceParameters(out message);
                string[] DeviceParameters = message.Split('/');

                GeneralInformationDevice = string.Format("" + Resources.RegistrationNumber + "\n" + Resources.RegistrationDescr + "\n" + Resources.Model + "\n" + Resources.Firmware + "\n" + Resources.CompanyDeviceName + "\n", result[0], result[1], result[2], result[3], result[4], DeviceParameters[16]);
            }
            return GeneralInformationDevice;
        }
        internal void ProgramPaymentMethods()
        {
            (new Thread(new ThreadStart(ProgramPaymentMethodsThread))).Start();
        }
        private void ProgramPaymentMethodsThread()
        {
            try
            {
                UpdateCurrentAction(Resources.InserAllPaymentTypes);
                using (CashRegisterHardware cashRegisterHardware = Form.GetCashRegister(uow.GetObjectByKey<ITS.Retail.Model.POS>(itemUpdate.POSOid)))
                {
                    string failurePaymentTypes = "";
                    string HexMessage = String.Empty;
                    XPCollection<PaymentMethod> paymentMethods = new XPCollection<PaymentMethod>(uow);
                    for (int i = 2; i < 10; i++)
                    {
                        UpdateStatus(string.Format(ResourcesLib.Resources.FromToString, i.ToString(), 10));
                        UpdateProgressBar(i * 10);
                        bool found = false;
                        foreach (PaymentMethod currentPaymentMethod in paymentMethods)
                        {
                            if (i == currentPaymentMethod.CashierDeviceCode)
                            {
                                found = true;
                                try
                                {
                                    cashRegisterHardware.ProgramPaymentType((int)currentPaymentMethod.CashierDeviceCode, currentPaymentMethod.Description, true, out HexMessage);
                                }
                                catch (Exception ex)
                                {
                                    failurePaymentTypes += string.Format(ResourcesLib.Resources.FaildToAddPaymentMethod, currentPaymentMethod.Code, currentPaymentMethod.Description);
                                    continue;
                                }
                            }
                        }
                        if (found == false)
                        {
                            try
                            {
                                cashRegisterHardware.ReadPaymentType(i, out HexMessage);
                                if (HexMessage.Substring(21, 1) == "1")
                                {
                                    string[] fields = HexMessage.Split('/');
                                    //int k = int.Parse(fields[1]);
                                    cashRegisterHardware.ProgramPaymentType(i, fields[2], false, out HexMessage);
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }


                    if (failurePaymentTypes.Length > 0)
                    {
                        UpdateCurrentAction(failurePaymentTypes);
                    }
                    UpdateProgressBar(100);
                }
            }
            catch (Exception ex)
            {
                UpdateCurrentAction(Resources.ISSUE_X + " - " + ex.Message);
            }
        }
    }
    internal class vatForList
    {
        internal int CashierDepartment { get; set; }
        internal Decimal VatPercent { get; set; }
        internal Guid ItemOid { get; set; }
        internal ITS.Retail.Platform.Enumerations.eMinistryVatCategoryCode VatDepartment { get; set; }
        internal Guid VatFactorOid { get; set; }
        internal Guid VatCategoryOid { get; set; }
    }
}
