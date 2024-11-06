using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public class CashierDocumentHelper
    {
        public static UserDailyTotals SaveDocument(CashRegisterItemUpdate posdev, List<ItemSales> ItemSales)
        {
            bool test = false;
            foreach (ItemSales sale in ItemSales)
            {
                if (sale.DeviceOid == posdev.DeviceOid)
                { test = true; break; }
            }
            if (test == false) return null;
            DateTime tora = DateTime.Now;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            ITS.Retail.Model.POSDevice device = uow.GetObjectByKey<ITS.Retail.Model.POSDevice>(posdev.DeviceOid);
            ITS.Retail.Model.POS POS = uow.GetObjectByKey<ITS.Retail.Model.POS>(posdev.POSOid);
            Store store = uow.GetObjectByKey<Store>(Program.Settings.StoreControllerSettings.Store.Oid);
            if (store == null)
            {
                throw new Exception(Resources.AnErrorOccurred);
            }
            UserDailyTotals userDailyTotals = CreateDailyTotals(uow, POS, tora, store, posdev, ItemSales, device);

            DocumentHeader newHeader = new DocumentHeader(uow);
            newHeader.FinalizedDate = DateTime.Now;
            newHeader.DocumentSeries = device.DocumentSeries;
            newHeader.DocumentType = device.DocumentType;
            newHeader.CreatedBy = uow.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid);
            newHeader.UserDailyTotals = userDailyTotals;
            newHeader.Store = store;
            newHeader.Customer = POS.DefaultCustomer;
            newHeader.DeliveryAddress = "";
            newHeader.Source = DocumentSource.CASHIER;
            newHeader.CreatedByDevice = posdev.DeviceOid.ToString().ToLower();
            newHeader.UpdateByDevice = "";
            newHeader.POS = POS;
            newHeader.POSID = POS.ID;
            newHeader.FiscalDate = newHeader.FinalizedDate;
            newHeader.DocumentTypeCode = device.DocumentType.Code;
            newHeader.DocumentSeriesCode = device.DocumentSeries.Code;
            newHeader.StoreCode = store.Code;
            newHeader.PriceCatalogPolicy = device.PriceCatalog.PriceCatalogPolicyDetails.FirstOrDefault().PriceCatalogPolicy;
            newHeader.CustomerCode = POS.DefaultCustomer.Code;
            newHeader.CustomerName = POS.DefaultCustomer.CompanyName;
            newHeader.CustomerLookupCode = "";
            newHeader.DeliveryAddress = "-";
            newHeader.DeliveryProfession = "-";
            IEnumerable<Address> billingAddresses = POS.DefaultCustomer.Trader.Addresses;
            if (billingAddresses.Count() > 0)
            {
                if (POS.DefaultCustomer.DefaultAddress != null)
                {
                    newHeader.BillingAddress = POS.DefaultCustomer.DefaultAddress;
                }
                else
                {
                    newHeader.BillingAddress = billingAddresses.First();
                }
            }
            newHeader.Status = device.DocumentStatus;
            newHeader.DocumentStatusCode = device.DocumentStatus.Code;
            if (newHeader.Status == null)
            {
                DocumentStatus docStatus = uow.FindObject<DocumentStatus>(new BinaryOperator("IsDefault", true));
                if (docStatus == null)
                {
                    XPCollection<DocumentStatus> docStatuses = new XPCollection<DocumentStatus>(uow);
                    if (docStatuses.Count > 0)
                    {
                        docStatus = docStatuses.First();
                    }
                }

                if (docStatus != null)
                {
                    newHeader.Status = docStatus;
                }
            }
            newHeader.Division = eDivision.Sales;

            newHeader.Remarks = device.Name;

            foreach (ItemSales sale in ItemSales)
            {
                //DocumentDetail documentDetail = new DocumentDetail(uow);
                if (sale.DeviceOid == posdev.DeviceOid)
                {


                    Item item = uow.GetObjectByKey<Item>(sale.ItemOid);

                    Barcode barcode;
                    if (sale.deviceIndex == 0)
                    {
                        if (item.DefaultBarcode != null)
                            barcode = item.DefaultBarcode;
                        else
                            barcode = item.ItemBarcodes.FirstOrDefault().Barcode;
                    }
                    else
                    {
                        barcode = new XPCollection<Barcode>(uow, CriteriaOperator.And(new BinaryOperator("Code", sale.Code))).FirstOrDefault();
                    }

                    //PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetail(initHeader.Store, barcode.Code, initHeader.Customer);
                    //PriceCatalogDetail pricecatdet = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    DocumentDetail derivedDocumentDetail;

                    if (!newHeader.DocumentType.AcceptsGeneralItems && item.IsGeneralItem)
                    {
                        throw new Exception(Resources.DocTypeNotUsesGeneralItems);
                    }

                    decimal price = newHeader.DocumentType.IsForWholesale ? Math.Round(sale.NetValue / sale.SoldQTY, 2, MidpointRounding.AwayFromZero) : Math.Round(sale.TotalSalesAmount / sale.SoldQTY, 2, MidpointRounding.AwayFromZero);
                    derivedDocumentDetail = DocumentHelper.ComputeDocumentLine(ref newHeader, item,
                                                                barcode, sale.SoldQTY,
                                                                false,
                                                                price,
                                                                 true,
                                                                item.Name,
                                                                new List<DocumentDetailDiscount>(),
                                                                UseDetailAssociation: false
                                                            );
                    derivedDocumentDetail.MeasurementUnitCode = derivedDocumentDetail.MeasurementUnit.Code;
                    derivedDocumentDetail.CreatedBy = newHeader.CreatedBy;
                    derivedDocumentDetail.CreatedByDevice = newHeader.CreatedByDevice;
                    derivedDocumentDetail.UpdateByDevice = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.WithdrawDepositTaxCode = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.POSGeneratedPriceCatalogDetailSerialized = "";
                    derivedDocumentDetail.Remarks = "";
                    DocumentHelper.AddItem(ref newHeader, derivedDocumentDetail);
                }
            }
            DocumentHelper.RecalculateDocumentCosts(ref newHeader, false, false);
            DocumentPayment defaultDocumentPayment = new DocumentPayment(newHeader.Session);
            defaultDocumentPayment.PaymentMethod = device.PaymentMethod;
            defaultDocumentPayment.Amount = newHeader.GrossTotal;
            newHeader.DocumentPayments.Add(defaultDocumentPayment);
            //newHeader.RefferenceDate = null;
            newHeader.Save();
            uow.CommitChanges();
            return userDailyTotals;
        }
        public static UserDailyTotals CreateDailyTotals(UnitOfWork uow, Model.POS POS, DateTime tora, Store store, CashRegisterItemUpdate posdev, List<ItemSales> ItemSales, POSDevice device)
        {
            DailyTotals dt = new DailyTotals(uow)
            {
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                CreatedBy = uow.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid),
                POS = POS,
                IsActive = true,
                POSID = POS.ID,
                FiscalDate = tora,
                PrintedDate = tora,
                Store = store,
                StoreCode = store.Code,
                FiscalDateOpen = false,
                ZReportNumber = 0,
                FiscalDeviceSerialNumber = "",
                InEmulationMode = false
            };
            DailyTotalsDetail dtd0 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DISCOUNTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd0);
            CashierTotals cashierTotals = new Helpers.CashierDocumentHelper.CashierTotals(ItemSales);
            DailyTotalsDetail dtd1 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = cashierTotals.VatAmount,
                DailyTotals = dt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd1);
            ((CashierUpdateItems)posdev.CashierUpdateItems).GetDaylyTotalsNoThread();
            DailyTotalsDetail dtd2 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL,
                QtyValue = 0,
                Amount = posdev.DayTotals.CancelsTotal,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd2);

            DailyTotalsDetail dtd4 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.LOYALTYPOINTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd4);

            DailyTotalsDetail dtd5 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DRAWERS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd5);

            foreach (CashierTotals item in cashierTotals.VatTotals)
            {
                DailyTotalsDetail dtd6 = new DailyTotalsDetail(uow)
                {
                    CreatedBy = dt.CreatedBy,
                    CreatedByDevice = POS.Oid.ToString().ToLower(),
                    UpdateByDevice = "",
                    IsActive = true,
                    DetailType = eDailyRecordTypes.TAXRECORD,
                    QtyValue = item.Qty,
                    Amount = item.Amount,
                    VatAmount = item.VatAmount,
                    DailyTotals = dt,
                    DocumentType = Guid.Empty,
                    MasterObjOid = Guid.Empty
                    , VatFactor = uow.GetObjectByKey<VatFactor>(item.MasterOid)
                    //, Payment =
                };
                dt.DailyTotalsDetails.Add(dtd6);
            }
            DailyTotalsDetail dtd7 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = cashierTotals.VatAmount,
                DailyTotals = dt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                , Payment = uow.GetObjectByKey<PaymentMethod>(device.PaymentMethod.Oid)
            };
            dt.DailyTotalsDetails.Add(dtd7);
            DailyTotalsDetail dtd8 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = 0,//cashierTotals.VatAmount,
                DailyTotals = dt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                ,
                Payment = uow.GetObjectByKey<PaymentMethod>(device.PaymentMethod.Oid)
            };
            dt.DailyTotalsDetails.Add(dtd8);
            DailyTotalsDetail dtd9 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.COUPONS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd9);
            DailyTotalsDetail dtd10 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.EMPTYBOTLLES,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd10);
            DailyTotalsDetail dtd11 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.RETURNS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd11);
            DailyTotalsDetail dtd13 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CASH,
                QtyValue = 0,
                Amount = cashierTotals.Amount,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd13);
            DailyTotalsDetail dtd14 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DOCUMENT_DISCOUNTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd14);
            DailyTotalsDetail dtd15 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CANCELED_RETURNS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd15);
            DailyTotalsDetail dtd16 = new DailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.STARTING_AMOUNT,
                QtyValue = 0,
                Amount = cashierTotals.Amount,
                VatAmount = 0,
                DailyTotals = dt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            dt.DailyTotalsDetails.Add(dtd16);
            dt.Save();
            UserDailyTotals udt = new UserDailyTotals(uow)
            {
                DailyTotals = dt,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                CreatedBy = dt.CreatedBy,
                POS = POS,
                IsActive = true,
                FiscalDate = tora,
                PrintedDate = tora,
                Store = store,
                StoreCode = store.Code,
                User= dt.CreatedBy,
                UserName=dt.CreatedBy.UserName,
                IsOpen=false,
                UserCashFinalAmount= cashierTotals.Amount,
                CashDifference = 0,
                InEmulationMode = false
            };
            udt.DailyTotals = dt;
            UserDailyTotalsDetail udtd0 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DISCOUNTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd0);
            UserDailyTotalsDetail udtd1 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = cashierTotals.VatAmount,
                UserDailyTotals = udt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd1);

            UserDailyTotalsDetail udtd2 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CANCELED_DOCUMENT_DETAIL,
                QtyValue = 0,
                Amount = posdev.DayTotals.CancelsTotal,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd2);

            UserDailyTotalsDetail udtd4 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.LOYALTYPOINTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd4);

            UserDailyTotalsDetail udtd5 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DRAWERS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd5);

            foreach (CashierTotals item in cashierTotals.VatTotals)
            {
                UserDailyTotalsDetail udtd6 = new UserDailyTotalsDetail(uow)
                {
                    CreatedBy = dt.CreatedBy,
                    CreatedByDevice = POS.Oid.ToString().ToLower(),
                    UpdateByDevice = "",
                    IsActive = true,
                    DetailType = eDailyRecordTypes.TAXRECORD,
                    QtyValue = item.Qty,
                    Amount = item.Amount,
                    VatAmount = item.VatAmount,
                    UserDailyTotals = udt,
                    DocumentType = Guid.Empty,
                    MasterObjOid = Guid.Empty
                    ,
                    VatFactor = uow.GetObjectByKey<VatFactor>(item.MasterOid)
                    //, Payment =
                };
                udt.UserDailyTotalsDetails.Add(udtd6);
            }
            UserDailyTotalsDetail udtd7 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = cashierTotals.VatAmount,
                UserDailyTotals = udt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                ,
                Payment = uow.GetObjectByKey<PaymentMethod>(device.PaymentMethod.Oid)
            };
            udt.UserDailyTotalsDetails.Add(udtd7);
            UserDailyTotalsDetail udtd8 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.INVOICES,
                QtyValue = cashierTotals.Qty,
                Amount = cashierTotals.Amount,
                VatAmount = 0,//cashierTotals.VatAmount,
                UserDailyTotals = udt,
                DocumentType = device.DocumentType.Oid,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                ,
                Payment = uow.GetObjectByKey<PaymentMethod>(device.PaymentMethod.Oid)
            };
            udt.UserDailyTotalsDetails.Add(udtd8);
            UserDailyTotalsDetail udtd9 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.COUPONS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd9);
            UserDailyTotalsDetail udtd10 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.EMPTYBOTLLES,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd10);
            UserDailyTotalsDetail udtd11 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.RETURNS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd11);
            UserDailyTotalsDetail udtd13 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CASH,
                QtyValue = 0,
                Amount = cashierTotals.Amount,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd13);
            UserDailyTotalsDetail udtd14 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.DOCUMENT_DISCOUNTS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd14);
            UserDailyTotalsDetail udtd15 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.CANCELED_RETURNS,
                QtyValue = 0,
                Amount = 0,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd15);
            UserDailyTotalsDetail udtd16 = new UserDailyTotalsDetail(uow)
            {
                CreatedBy = dt.CreatedBy,
                CreatedByDevice = POS.Oid.ToString().ToLower(),
                UpdateByDevice = "",
                IsActive = true,
                DetailType = eDailyRecordTypes.STARTING_AMOUNT,
                QtyValue = 0,
                Amount = cashierTotals.Amount,
                VatAmount = 0,
                UserDailyTotals = udt,
                DocumentType = Guid.Empty,
                MasterObjOid = Guid.Empty
                //, VatFactor =
                //, Payment =
            };
            udt.UserDailyTotalsDetails.Add(udtd16);
            udt.Save();
            return udt;
        }
        internal class CashierTotals
        {
            internal CashierTotals()
            {

            }
            internal CashierTotals(List<ItemSales> ItemSales)
            {
                Qty = 0;
                Amount = 0;
                VatAmount = 0;
                VatTotals = new List<CashierTotals>();
                bool found;
                foreach (ItemSales item in ItemSales)
                {
                    found = false;
                    Qty += item.SoldQTY;
                    Amount += item.NetValue;
                    VatAmount += item.VatValue;
                    foreach (CashierTotals vatItem in VatTotals)
                    {
                        if (vatItem.MasterOid == item.VatFactorOid)
                        {
                            vatItem.Qty += item.Qty;
                            vatItem.Amount += item.NetValue+ item.VatValue;
                            vatItem.VatAmount += item.VatValue;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        VatTotals.Add(new CashierTotals() { Qty = item.SoldQTY, Amount = item.NetValue+ item.VatValue, VatAmount = item.VatValue, MasterOid = item.VatFactorOid });
                    }
                }
            }
            internal Guid MasterOid { get; set; }
            internal decimal Qty { get; set; }
            internal decimal Amount { get; set; }
            internal decimal VatAmount { get; set; }
            internal List<CashierTotals> VatTotals { get; set; }

        }
    }
}

