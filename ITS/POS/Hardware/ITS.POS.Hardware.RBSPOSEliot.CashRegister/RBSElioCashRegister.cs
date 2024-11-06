using ITS.POS.Hardware.Common;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;
using DevExpress.Xpo;
using System.Collections;
using System.Linq;
using System.Globalization;
using System.Threading;

namespace ITS.Hardware.RBSPOSEliot.CashRegister
{
    public class RBSElioCashRegister : ITS.POS.Hardware.CashRegisterHardware
    {
        private ConnectionType _ConnectionType;
        public RBSElioCashRegister(DeviceType deviceType, Settings settings, string device, int id, ConnectionType Type, int lineChars, int commandChars) : base(deviceType, settings, device, id, Type)
        {
            FiscalPrinter = new MicrelecFiscalPrinter(Type, device, id, lineChars, commandChars);
            FiscalPrinter.Settings = settings;
            _ConnectionType = Type;
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            if (_ConnectionType == ConnectionType.COM)
            {
                this.FiscalPrinter.InitializeConnection();
            }
            else if (_ConnectionType == ConnectionType.ETHERNET)
            {
                this.FiscalPrinter.InitializeEthernetConnection();
            }
            return this.FiscalPrinter.CheckDevice(out message);
        }

        public override void GetDocumentFromDevice()
        {
            throw new NotImplementedException();
        }

        public override eDeviceCheckResult SendItemToDevice(int index, string Code, string ItemDescription, double itemPrice1, double itemMaxPrice, double itemPrice2, int Points, double stockQTY, int vatCode, out string message)
        {
            try
            {
                message = String.Empty;
                DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).SendItemToDevice(index, Code, ItemDescription, itemPrice1, itemMaxPrice, itemPrice2, Points, stockQTY, vatCode);
                switch (result)
                {
                    case DeviceResult.SUCCESS:
                        return eDeviceCheckResult.SUCCESS;
                    default:
                        message = result.ToString();
                        return eDeviceCheckResult.FAILURE;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void SendPriceToDevice()
        {

        }

        public override eDeviceCheckResult GetTotalSalesOfDay(out string HexMessage)
        {

            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).GetTotalSales(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult GetDailySalesOfItem(out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).GetTotalDailySalesOfItem(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult GetDeviceInfo(out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).GetDeviceInfo(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult ProgramDevice(int PosNumber, string deviceDescription, out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).ProgramDevice(PosNumber, deviceDescription, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult GetDeviceParameters(out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).GetDeviceParameters(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult GetEthernetSettings(out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).GetEthernetParameters(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }
        public void closePort()
        {
            (this.FiscalPrinter as MicrelecFiscalPrinter).ClosePort();
        }

        public override eDeviceCheckResult SetEthernetSettings(int portNumber, int WatchdogTime, int Delay, string IP, string RemoteIP, string Gateway, string DNS1, string DNS2, string MASK, int EnableEthernt, int EnableDHCP, int TCPUDP, int EnableWatchdogTimer, int CloseOpenReceipt, out string HexMessage)
        {
            HexMessage = String.Empty;
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).SetEthernetParameters(portNumber, WatchdogTime, Delay, IP, RemoteIP, Gateway, DNS1, DNS2, MASK, EnableEthernt, EnableDHCP, TCPUDP, EnableWatchdogTimer, CloseOpenReceipt, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult IssueZReport(out string HexMessage)
        {
            try
            {
                NumberFormatInfo NumberFormat = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." };
                int zReportNumber = -1;
                String command = String.Format(NumberFormat, "x/7////");
                string fields;
                eDeviceCheckResult result = SendComand(command, out fields);
                //if (result == MicrelecStatusCode.NOZERODM)
                //{
                //    return DeviceResult.SUCCESS;
                //}
                int retries = 0;
                do
                {
                    retries++;
                    Thread.Sleep(2000);
                    result = SendComand("?/", out fields);
                } while (result != eDeviceCheckResult.SUCCESS && retries <= 20);
                //if (result != eDeviceCheckResult.SUCCESS)
                //{
                //    int internalRetries = 0;
                //    do
                //    {
                //        Thread.Sleep(1000);
                //        internalRetries++;
                //        result = this.SendComand("x/10////", out fields);
                //    } while (result != eDeviceCheckResult.SUCCESS && internalRetries <= 20);
                //    internalRetries = 0;
                //    do
                //    {
                //        Thread.Sleep(1000);
                //        internalRetries++;
                //        result = this.SendComand("x/10////", out fields);
                //    } while (result != eDeviceCheckResult.SUCCESS && internalRetries <= 20);

                //}
                HexMessage = "";
                if (result == eDeviceCheckResult.SUCCESS)
                {
                    int internalRetries = 0;
                    do
                    {
                        Thread.Sleep(1000);
                        internalRetries++;
                        result = this.SendComand("x/10////", out fields);
                    } while (result != eDeviceCheckResult.SUCCESS && internalRetries <= 20);

                    if (result == eDeviceCheckResult.SUCCESS)
                    {
                        internalRetries = 0;
                        do
                        {
                            Thread.Sleep(1000);
                            internalRetries++;
                            result = this.SendComand("i/", out fields);
                        } while (result != eDeviceCheckResult.SUCCESS && internalRetries <= 20);
                        if (result == eDeviceCheckResult.SUCCESS)
                        {
                            HexMessage = fields.Split('/')[2];
                            return eDeviceCheckResult.SUCCESS;
                        }
                    }
                }
                return eDeviceCheckResult.FAILURE;

            }
            catch (Exception ex)
            {
                throw;
            }
            DeviceResult result2 = (this.FiscalPrinter as MicrelecFiscalPrinter).IssueZReport(out HexMessage);
            switch (result2)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result2.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult IssueXReport(out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).IssueXreport(out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult SendComand(string command, out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).TestSend(command, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }
        public override eDeviceCheckResult ProgramPaymentType(int paymentCode, string paymentDescription, bool active, out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).ProgramPaymentType(paymentCode, paymentDescription, active, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult ReadDailyPaymentMethods(int paymentCode, out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).ReadDailyPaymentMethods(paymentCode, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult IssueZReportCashierRegister(string pathToAbc, out int zReportNumber, out string pathToEJFiles, out string HexMessage)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).IssueZReportCashierRegister(pathToAbc, out zReportNumber, out pathToEJFiles, out HexMessage);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    HexMessage = result.ToString();
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eMachineStatus ReadStatusOfDevice(out string HexMessage)
        {

            eMachineStatus result = (this.FiscalPrinter as MicrelecFiscalPrinter).ReadStatusOfDevice();
            HexMessage = "";
            return result;
        }

        public List<ItemCashRegister> PreparationItemsToAdd(Dictionary<int, ItemCashRegister> dictionaryItems, List<ItemCashRegister> dataToSend, int lastIndex, int deviceIndex, UnitOfWork _Uow)
        {
            List<ItemCashRegister> data = new List<ItemCashRegister>();

            foreach (KeyValuePair<int, ItemCashRegister> pair in dictionaryItems)
            {
                try
                {
                    if (pair.Value.Item.CashierDeviceIndex == null || pair.Value.Item.CashierDeviceIndex == String.Empty)
                    {
                        lastIndex++;
                        deviceIndex = lastIndex;
                    }
                    else
                    {
                        deviceIndex = 0;
                        Int32.TryParse(pair.Value.Item.CashierDeviceIndex, out deviceIndex);
                        if (deviceIndex == 0)
                        {
                            throw new Exception("NoIndexFoundForDevice");
                        }
                    }
                    string errorMessage = String.Empty;
                    SendItemToDevice(deviceIndex, pair.Value.CashRegisterBarcode,
                                                      pair.Value.Item.Description,
                                                      (double)pair.Value.RetailPriceValue,
                                                      (double)pair.Value.RetailPriceValue * 100,
                                                      (double)pair.Value.RetailPriceValue,
                                                      (int)pair.Value.Item.Points,
                                                      (double)pair.Value.CashRegisterQTY,
                                                      pair.Value.CashRegisterVatLevel,
                                                      out errorMessage);

                    int row = pair.Key;

                    pair.Value.eSItemtatus = eCashRegisterItemStatus.SENDED;
                    if (pair.Value.Item.CashierDeviceIndex == null || pair.Value.Item.CashierDeviceIndex == String.Empty)
                    {
                        Item item = _Uow.GetObjectByKey<Item>(pair.Value.Item.Oid);
                        item.CashierDeviceIndex = deviceIndex.ToString();
                        item.Save();
                        _Uow.CommitChanges();
                        var itemID = dataToSend.FirstOrDefault(x => x.Item.Oid == pair.Value.Item.Oid);//.Select(x => x.Item.CashierDeviceIndex = deviceIndex.ToString());
                        if (itemID != null)
                        {
                            itemID.Item.CashierDeviceIndex = deviceIndex.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    pair.Value.eSItemtatus = eCashRegisterItemStatus.FAILURE;
                    continue;
                }
            }
            return dataToSend;
        }
        public void PreparationDeleteAllItems(List<ItemCashRegister> itemCashRegister, string resourceItem, XPCollection<Item> items, UnitOfWork _Uow)
        {
            List<Item> faildToRemoveItemList = new List<Item>();
            foreach (var current in itemCashRegister.Where(x => x.Item.CashierDeviceIndex != null))
            {
                try
                {
                    int deviceIndex = 0;
                    Int32.TryParse(current.Item.CashierDeviceIndex, out deviceIndex);

                    //send data to device
                    string errorMessage = string.Empty;
                    var PLUCode = deviceIndex.ToString().PadLeft(15 - deviceIndex.ToString().Length, '0');
                    SendItemToDevice(deviceIndex,
                                                         PLUCode,
                                                         resourceItem + deviceIndex,
                                                         0.00,
                                                         0.00,
                                                         0.00,
                                                         0,
                                                         0.00,
                                                         1,
                                                         out errorMessage);
                }
                catch (Exception ex)
                {
                    faildToRemoveItemList.Add(current.Item);
                    continue;
                }
            }
            var countList = items.Where(x => !faildToRemoveItemList.Select(y => y.Oid).Contains(x.Oid)).Count();

            if (itemCashRegister.Where(x => x.Item.CashierDeviceIndex != null).ToList().Count > 0)
            {
                foreach (Item curentItemToSave in items.Where(x => !faildToRemoveItemList.Select(y => y.Oid).Contains(x.Oid)))
                {
                    Item item = _Uow.GetObjectByKey<Item>(curentItemToSave.Oid);
                    item.CashierDeviceIndex = null;
                    item.Save();
                    _Uow.CommitChanges();
                }
            }
        }
        public List<ItemCashRegister> PreperationDeleteSpecificItems(List<ItemCashRegister> itemsToRemove, string resourceItem, List<ItemCashRegister> sourceList, UnitOfWork _Uow)
        {
            List<Item> faildToRemoveItemList = new List<Item>();

            foreach (var current in itemsToRemove)
            {
                try
                {
                    int deviceIndex = 0;
                    Int32.TryParse(current.Item.CashierDeviceIndex, out deviceIndex);

                    //send data to device
                    string errorMessage = string.Empty;
                    var PLUCode = deviceIndex.ToString().PadLeft(15 - deviceIndex.ToString().Length, '0');
                    SendItemToDevice(deviceIndex,
                                                         PLUCode,
                                                         resourceItem + deviceIndex,
                                                         0.00,
                                                         0.00,
                                                         0.00,
                                                         0,
                                                         0.00,
                                                         1,
                                                         out errorMessage);
                }
                catch (Exception ex)
                {
                    faildToRemoveItemList.Add(current.Item);
                    continue;
                }

            }
            var countList = itemsToRemove.Where(x => !faildToRemoveItemList.Select(y => y.Oid).Contains(x.Item.Oid)).Count();

            if (itemsToRemove.Where(x => x.Item.CashierDeviceIndex != null).ToList().Count > 0)
            {
                foreach (Item curentItemToSave in itemsToRemove.Select(x => x.Item).Where(x => !faildToRemoveItemList.Select(y => y.Oid).Contains(x.Oid)))
                {
                    Item item = _Uow.GetObjectByKey<Item>(curentItemToSave.Oid);
                    item.CashierDeviceIndex = null;
                    item.Save();
                    _Uow.CommitChanges();
                    var itemID = itemsToRemove.FirstOrDefault(x => x.Item.Oid == curentItemToSave.Oid);
                    if (itemID != null)
                    {
                        itemID.Item.CashierDeviceIndex = null;
                    }
                }
            }
            foreach (var currentItem in itemsToRemove.Select(x => x.Item).Where(x => !faildToRemoveItemList.Select(y => y.Oid).Contains(x.Oid)))
            {
                var itemID = sourceList.FirstOrDefault(x => x.Item.Oid == currentItem.Oid);
                if (itemID != null)
                {
                    itemID.Item.CashierDeviceIndex = string.Empty;
                    itemID.eSItemtatus = eCashRegisterItemStatus.WAITING;
                }
            }
            return sourceList;
        }
        public override DailyTotal CalculateDaylyTotals(string dailyTotal)
        {
            DailyTotal dailyTotalSums;

            string[] currentItem = dailyTotal.Split('/');

            int ReceiptNumber = 0;
            int IllegalReceiptNumber = 0;

            decimal VatA = 0;
            decimal VatB = 0;
            decimal VatC = 0;
            decimal VatD = 0;
            decimal VatE = 0;
            decimal dailyTotals = 0;
            decimal VoidsTotal = 0;
            decimal RefundsTotal = 0;
            decimal CancelsTotal = 0;

            //convertions
            int.TryParse(currentItem[6], out ReceiptNumber);
            int.TryParse(currentItem[7], out IllegalReceiptNumber);

            decimal.TryParse(currentItem[0].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VatA);
            decimal.TryParse(currentItem[1].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VatB);
            decimal.TryParse(currentItem[2].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VatC);
            decimal.TryParse(currentItem[3].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VatD);
            decimal.TryParse(currentItem[4].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VatE);
            decimal.TryParse(currentItem[5].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out dailyTotals);
            decimal.TryParse(currentItem[8].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out VoidsTotal);
            decimal.TryParse(currentItem[9].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out RefundsTotal);
            decimal.TryParse(currentItem[10].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out CancelsTotal);

            dailyTotalSums = new DailyTotal()
            {
                VatCategoryA = VatA,
                VatCategoryB = VatB,
                VatCategoryC = VatC,
                VatCategoryD = VatD,
                VatCategoryE = VatE,
                DailyTotals = dailyTotals,
                VoidsTotal = VoidsTotal,
                RefundsTotal = RefundsTotal,
                CancelsTotal = CancelsTotal,
                ReceiptNumber = ReceiptNumber,
                IllegalReceiptNumber = IllegalReceiptNumber
            };
            return dailyTotalSums;
        }
        public void CalculateDailyTotalsExplantionDetails(DateTime zDate, int Znumber, DailyTotal dailyTotal, string SerialNumber, ITS.Retail.Model.POS pos, User user, UnitOfWork unitOfWork)
        {
            try
            {
                Dictionary<int, DailyTotalDetailMachine> dailysDict = new Dictionary<int, DailyTotalDetailMachine>();
                dailysDict.Add(0, new DailyTotalDetailMachine() { receips = dailyTotal.ReceiptNumber, amount = dailyTotal.DailyTotals, dailyType = eDailyRecordTypes.PAYMENTS });
                dailysDict.Add(1, new DailyTotalDetailMachine() { receips = 0, amount = dailyTotal.CancelsTotal, dailyType = eDailyRecordTypes.CANCELED_DOCUMENT });
                dailysDict.Add(2, new DailyTotalDetailMachine() { receips = 0, amount = dailyTotal.RefundsTotal, dailyType = eDailyRecordTypes.RETURNS });
                dailysDict.Add(3, new DailyTotalDetailMachine() { receips = 0, amount = dailyTotal.VoidsTotal, dailyType = eDailyRecordTypes.ZERO_RECEIPTS });


                User currentUser = unitOfWork.GetObjectByKey<User>(user.Oid);
                ITS.Retail.Model.POS currentPOS = unitOfWork.GetObjectByKey<ITS.Retail.Model.POS>(pos.Oid);
                Store currentStore = unitOfWork.GetObjectByKey<Store>(pos.Store.Oid);

                DailyTotals dailyTotals = new DailyTotals(unitOfWork)
                {
                    FiscalDeviceSerialNumber = SerialNumber,
                    POS = currentPOS,
                    POSID = pos.ID,
                    Store = currentStore,
                    CreatedOnTicks = DateTime.Now.Ticks,
                    FiscalDate = DateTime.Now,
                    PrintedDate = zDate,
                    StoreCode = pos.Store.Code,
                    CreatedBy = currentUser,
                    UpdatedBy = currentUser,
                    CreatedByDevice = pos.Oid.ToString(),
                    UpdateByDevice = pos.Oid.ToString(),
                    ZReportNumber = Znumber
                };

                foreach (KeyValuePair<int, DailyTotalDetailMachine> pair in dailysDict)
                {
                    decimal sumVatFactor = 0;
                    if (pair.Key == 0)
                    {
                        sumVatFactor = dailyTotal.VatCategoryA + dailyTotal.VatCategoryB + dailyTotal.VatCategoryC + dailyTotal.VatCategoryD + dailyTotal.VatCategoryE;
                    }
                    DailyTotalsDetail dailyTotalDetails = new DailyTotalsDetail(unitOfWork)
                    {
                        DailyTotals = dailyTotals,
                        Amount = pair.Value.amount,
                        CreatedBy = currentUser,
                        UpdatedBy = currentUser,
                        DetailType = pair.Value.dailyType,
                        QtyValue = 0,
                        CreatedOnTicks = DateTime.Now.Ticks,
                        CreatedByDevice = pos.Oid.ToString(),
                        UpdateByDevice = pos.Oid.ToString(),
                        UpdatedOnTicks = DateTime.Now.Ticks,
                        VatAmount = sumVatFactor
                    };
                    dailyTotalDetails.Save();

                }
                dailyTotals.Save();
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ItemSales> GetItemDailySales()
        {


            string message = string.Empty;
            List<string> mashineItemSalesResults = new List<string>();
            List<ItemSales> ItemSales = new List<ItemSales>();
            bool ExceptionHappent = false;
            do
            {
                try
                {
                    GetDailySalesOfItem(out message);
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
                }
            }
            while (!ExceptionHappent);
            foreach (var item in mashineItemSalesResults)
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
                currentItemSale.Description = currentItem[1];
                currentItemSale.VatCode = vatCode;
                currentItemSale.price = price;
                currentItemSale.Points = points;
                currentItemSale.SoldQTY = soldQty;
                currentItemSale.TotalSalesAmount = totalSalesAmount;
                currentItemSale.Qty = Qty;
                ItemSales.Add(currentItemSale);

            }
            return ItemSales;
        }
        public List<CashRegisterPaymentMethods> LoadDailyPaymentMethods(UnitOfWork _Uow)
        {
            string HexMessage = String.Empty;

            XPCollection<PaymentMethod> paymentMethods = new XPCollection<PaymentMethod>(_Uow);

            List<string> mashineResults = new List<string>();
            List<CashRegisterPaymentMethods> DailyPayments = new List<CashRegisterPaymentMethods>();

            foreach (PaymentMethod currentPaymentMethod in paymentMethods)
            {
                try
                {
                    int paymentCode = 0;
                    Int32.TryParse(currentPaymentMethod.Code, out paymentCode);
                    ReadDailyPaymentMethods(paymentCode, out HexMessage);
                    mashineResults.Add(HexMessage);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            foreach (var currentItem in mashineResults)
            {

                string[] currentPayment = currentItem.Split('/');
                CashRegisterPaymentMethods DailyPaymentMethodsSale = new CashRegisterPaymentMethods();

                decimal sumDaily = 0;
                decimal cashIn = 0;
                decimal cashOut = 0;
                decimal totalSum = 0;
                //convertions

                decimal.TryParse(currentPayment[5].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out sumDaily);
                decimal.TryParse(currentPayment[6].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out cashIn);
                decimal.TryParse(currentPayment[7].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out cashOut);
                decimal.TryParse(currentPayment[8].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out totalSum);

                DailyPaymentMethodsSale.Description = currentPayment[0];
                DailyPaymentMethodsSale.ShortcutDescription = currentPayment[1];
                DailyPaymentMethodsSale.Code = currentPayment[2];
                DailyPaymentMethodsSale.CreditType = currentPayment[4];
                DailyPaymentMethodsSale.DailySum = sumDaily;
                DailyPaymentMethodsSale.CashIn = cashIn;
                DailyPaymentMethodsSale.CashOut = cashOut;
                DailyPaymentMethodsSale.TotalSum = totalSum;

                DailyPayments.Add(DailyPaymentMethodsSale);
            }

            return DailyPayments;
        }
        public override eDeviceCheckResult GetDepartmentInfo(int DepartmentID, out string message)
        {
            return SendComand(String.Format("d/{0}/", DepartmentID.ToString()), out message);
        }
        public override eDeviceCheckResult ReadPaymentType(int paymentCode, out string message)
        {
            DeviceResult result = (this.FiscalPrinter as MicrelecFiscalPrinter).ReadPaymentType(paymentCode, out message);
            switch (result)
            {
                case DeviceResult.SUCCESS:
                    return eDeviceCheckResult.SUCCESS;
                default:
                    return eDeviceCheckResult.FAILURE;
            }
        }

        public override eDeviceCheckResult GetVatRates(CashierVatRates VatRates)
        {
            string message = string.Empty;
            eDeviceCheckResult result = SendComand("e", out message);

            string[] x = message.Split('/');
            VatRates.VatRateA = decimal.Parse(x[0]);
            VatRates.VatRateB = decimal.Parse(x[1]);
            VatRates.VatRateC = decimal.Parse(x[2]);
            VatRates.VatRateD = decimal.Parse(x[3]);
            VatRates.VatRateE = decimal.Parse(x[4]);
            return result;



        }
    }



    class DailyTotalDetailMachine
    {
        public int receips { get; set; }
        public decimal amount { get; set; }
        public eDailyRecordTypes dailyType { get; set; }
    }
}
