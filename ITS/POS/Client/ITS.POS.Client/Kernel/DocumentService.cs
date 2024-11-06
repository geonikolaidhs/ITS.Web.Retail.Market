using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Common.ViewModel;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides the documents' business logic functions.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private ISessionManager SessionManager { get; set; }
        private IConfigurationManager Configuration { get; set; }
        private IItemService ItemService { get; set; }
        private IPlatformDocumentDiscountService PlatformDocumentDiscountService { get; set; }
        private IPlatformRoundingHandler PlatformRoundingHandler { get; set; }
        private bool DefaulDocumentDiscountApplied { get; set; } = false;

        public DocumentService(ISessionManager sessionManager, IConfigurationManager configuration, IItemService itemService,
            IPlatformDocumentDiscountService platformDocumentDiscountService, IPlatformRoundingHandler platformRoundingHandler)
        {
            this.Configuration = configuration;
            this.ItemService = itemService;
            this.SessionManager = sessionManager;
            this.PlatformDocumentDiscountService = platformDocumentDiscountService;
            this.PlatformRoundingHandler = platformRoundingHandler;

        }

        /// <summary>
        /// Checks if a Document Sequence exists for the given Document Series.
        /// </summary>
        /// <param name="documentSeriesOid"></param>
        /// <returns></returns>
        public bool SequenceExists(Guid documentSeriesOid)
        {
            DocumentSequence ds = SessionManager.FindObject<DocumentSequence>(new BinaryOperator("DocumentSeries", documentSeriesOid));
            return (ds != null);
        }

        public bool ZSequenceExists(Guid terminalOid)
        {
            ZReportSequence ds = SessionManager.FindObject<ZReportSequence>(new BinaryOperator("POS", terminalOid));
            return (ds != null);
        }

        /// <summary>
        /// New document creation.
        /// </summary>
        /// <param name="division"></param>
        /// <param name="docType"></param>
        /// <param name="docSeries"></param>
        /// <param name="store"></param>
        /// <param name="customer"></param>
        /// <param name="customerCode"></param>
        /// <param name="customerName"></param>
        /// <param name="priceCatalog"></param>
        /// <param name="status"></param>
        /// <param name="userDailyTotals"></param>
        /// <returns></returns>
        public DocumentHeader CreateDocumentHeader(eDivision division, Guid pos, Guid docType,
            Guid docSeries, Guid store, Guid customer, string customerCode, string customerName, Guid priceCatalogPolicy, Guid status, Guid userDailyTotals)
        {
            if (!SequenceExists(docSeries))
            {
                string seriesDescription = docSeries.ToString();
                DocumentSeries series = SessionManager.GetObjectByKey<DocumentSeries>(docSeries);
                if (series != null)
                {
                    seriesDescription = series.Description;
                }

                throw new POSException(String.Format(POSClientResources.DOCUMENT_SEQUENCE_NOT_FOUND_FOR_SERIES, seriesDescription));
            }

            Store currentStore = SessionManager.GetObjectByKey<Store>(store);
            DocumentHeader documentHeader = new DocumentHeader(SessionManager.GetSession<DocumentHeader>())
            {
                Division = division,
                POS = pos,
                CreatedByDevice = pos.ToString(),
                DocumentType = docType,
                DocumentSeries = docSeries,
                Store = store,
                ReferenceCompany = currentStore.ReferenceCompany,
                MainCompany = currentStore.Owner,
                Customer = customer,
                CustomerCode = customerCode,
                CustomerName = customerName,
                Status = status,
                PriceCatalogPolicy = priceCatalogPolicy,
                UserDailyTotals = SessionManager.GetObjectByKey<UserDailyTotals>(userDailyTotals),
                DeliveryAddress = "-",
                Source = DocumentSource.POS
            };
            SessionManager.FillDenormalizedFields(documentHeader);
            return documentHeader;
        }

        /// <summary>
        /// Deletes all the payments of the document.
        /// </summary>
        /// <param name="header"></param>
        public void ClearPayments(DocumentHeader header)
        {
            if (header.DocumentPayments.Count > 0)
            {
                //Remove Coupons
                List<TransactionCoupon> headerTransactionCouponsWhere = header.TransactionCoupons.Where(tran => tran.DocumentPayment != null).ToList();
                header.Session.Delete(headerTransactionCouponsWhere);

                header.Session.Delete(header.DocumentPayments);
                header.Session.CommitTransaction();
            }
        }

        /// <summary>
        /// Returns the Non Discountable Value of Document
        /// </summary>
        /// <param name="header"></param>
        public decimal GetTotalNonDiscountableValue(DocumentHeader document)
        {
            decimal amount = 0;
            foreach (DocumentDetail det in document.DocumentDetails.Where(x => x.IsTax || x.DoesNotAllowDiscount))
            {
                if (det.IsCanceled == false && det.IsReturn == false)
                    amount += det.GrossTotal;
            }
            return amount;
        }

        /// <summary>
        /// Creates a withdraw or deposit document.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="currentUserDailyTotals"></param>
        /// <param name="docType"></param>
        /// <param name="docSeries"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <param name="priceCatalog"></param>
        /// <param name="customDescription"></param>
        /// <param name="isShiftStartingDeposit"></param>
        /// <param name="isDayStartingDeposit"></param>
        /// <returns></returns>
        public DocumentHeader CreateWithdrawOrDeposit(User currentUser, UserDailyTotals currentUserDailyTotals, Guid docType, Guid docSeries, decimal value,
            SpecialItem item, Guid priceCatalogPolicy, string customDescription, bool isShiftStartingDeposit, bool isDayStartingDeposit, Guid? reasonGuid, string comment = null, string taxCode = null)
        {
            Customer defaultCustomer = SessionManager.GetObjectByKey<Customer>(Configuration.DefaultCustomerOid);

            DocumentHeader header = this.CreateDocumentHeader(eDivision.Store, Configuration.CurrentTerminalOid, docType, docSeries,
                                Configuration.CurrentStoreOid, Configuration.DefaultCustomerOid, defaultCustomer.Code, defaultCustomer.CompanyName,
                                priceCatalogPolicy, Configuration.DefaultDocumentStatusOid, currentUserDailyTotals.Oid);

            header.CreatedBy = currentUser.Oid;
            header.IsShiftStartingAmount = isShiftStartingDeposit;
            header.IsDayStartingAmount = isDayStartingDeposit;

            //Reason reason = SessionManager.GetObjectByKey<Reason>(reasonGuid);

            DocumentDetail line = new DocumentDetail(header.Session)
            {
                DocumentHeader = header,
                Barcode = Guid.Empty,
                Item = Guid.Empty,
                CreatedBy = header.CreatedBy,
                CreatedByDevice = header.CreatedByDevice,
                CustomDescription = customDescription,
                FinalUnitPrice = value,
                GrossTotalBeforeDiscount = value,
                GrossTotal = value,
                HasCustomDescription = true,
                HasCustomPrice = true,
                ItemCode = null,
                ItemName = null,
                ItemVatCategoryDescription = "",
                LinkedLine = Guid.Empty,
                MeasurementUnit = Guid.Empty,
                NetTotal = value,
                OfferDescription = "",
                PriceListUnitPrice = value,
                Qty = 1,
                TotalDiscount = 0,
                TotalVatAmountBeforeDiscount = 0,
                TotalVatAmount = 0,
                UnitPrice = value,
                VatFactor = 0,
                VatFactorGuid = null,
                SpecialItem = item.Oid,
                SpecialItemCode = item.Code,
                Reason = reasonGuid,
                WithdrawDepositTaxCode = taxCode,
                Remarks = comment,
                DoesNotAllowDiscount = false,
                IsTax = false
            };
            header.FinalizedDate = DateTime.Now;
            header.FiscalDate = DateTime.Now;

            PaymentMethod defaultPaymentMethod = SessionManager.GetObjectByKey<PaymentMethod>(Configuration.DefaultPaymentMethodOid);
            PaymentMethod cashPaymentMethod = (defaultPaymentMethod.IncreasesDrawerAmount) ? defaultPaymentMethod : null;

            if (cashPaymentMethod == null)
            {
                cashPaymentMethod = new XPQuery<PaymentMethod>(SessionManager.GetSession<PaymentMethod>()).Where(pm => pm.IncreasesDrawerAmount).FirstOrDefault();
            }

            if (cashPaymentMethod == null)
            {
                throw new POSException(POSClientResources.NO_CASH_PAYMENT_METHOD_IS_DEFINED);
            }

            DocumentPayment documentPayment = new DocumentPayment(SessionManager.GetSession<DocumentPayment>());
            documentPayment.DocumentHeader = header;
            documentPayment.Amount = value;
            documentPayment.PaymentMethod = cashPaymentMethod.Oid;
            documentPayment.PaymentMethodDescription = cashPaymentMethod.Description;
            documentPayment.PaymentMethodType = cashPaymentMethod.PaymentMethodType;
            documentPayment.IncreasesDrawerAmount = cashPaymentMethod.IncreasesDrawerAmount;

            SessionManager.FillDenormalizedFields(line);
            this.ComputeDocumentHeader(ref header, false, line);
            this.AssignDocumentNumber(header, Configuration.CurrentTerminalOid, currentUser.Oid);
            return header;
        }

        /// <summary>
        /// Creates a document promotion.
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        public DocumentPromotion CreateDocumentPromotion(UnitOfWork uow)
        {
            return new DocumentPromotion(uow);
        }

        /// <summary>
        /// Cancels a document.
        /// </summary>
        /// <param name="document"></param>
        public void CancelDocument(DocumentHeader document, DateTime fiscalDate, int docNumber = 0, bool isFiscalPrinterHandled = false)
        {
            document.IsCanceled = true;
            document.DocumentOnHold = false;
            document.IsOpen = false;
            document.IsFiscalPrinterHandled = isFiscalPrinterHandled;
            document.DocumentNumber = docNumber;
            document.FiscalPrinterNumber = docNumber;
            document.FinalizedDate = DateTime.Now;
            document.FiscalDate = fiscalDate;
            document.Save();
            document.Session.CommitTransaction();
        }

        /// <summary>
        /// Creates a document line.
        /// </summary>
        /// <param name="header">Παραστατικό</param>
        /// <param name="user">Χρήστης</param>
        /// <param name="item">Item</param>
        /// <param name="pcd">PriceCatalogDetail</param>
        /// <param name="userBarcode">Barcode</param>
        /// <param name="qty">Ποσότητα</param>
        /// <param name="customPrice">Custom price</param>
        /// <param name="hasCustomPrice">Use customPrice</param>
        /// <param name="isReturn">Item is returned to the store</param>
        /// <param name="customDescription">Item is returned to the store</param>
        /// <returns></returns>
        public DocumentDetail CreateDocumentLine(DocumentHeader header, Guid user,
            Item item, PriceCatalogDetail pcd, Barcode userBarcode, decimal Qty, decimal customPrice, bool hasCustomPrice, bool isReturn, string customDescription = "")
        {
            bool vatIncluded = false;
            bool vatChanged = false;
            if (pcd != null)
            {
                vatIncluded = pcd.VATIncluded;
                vatChanged = true;
            }

            bool IsForWholesale = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType).IsForWholesale;
            if (hasCustomPrice)
            {
                vatIncluded = !IsForWholesale;
                vatChanged = true;
            }

            if (!vatChanged)
            {
                throw new POSException(item.Code + " - " + Resources.POSClientResources.ITEM_NOT_FOUND);
            }

            int currentMaxLineNumber = header.DocumentDetails.Count > 0 ? header.DocumentDetails.Max(det => det.LineNumber) : 0;
            currentMaxLineNumber++;

            DocumentDetail dl = new DocumentDetail(header.Session) { DocumentHeader = header, LineNumber = currentMaxLineNumber };
            this.ComputeDocumentLine(dl, userBarcode, item, pcd, false, customPrice, hasCustomPrice, vatIncluded, customDescription, null, IsForWholesale, header.CreatedByDevice, Qty);

            dl.CreatedBy = user;
            dl.IsReturn = isReturn;

            PriceCatalogPolicy headerPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(header.PriceCatalogPolicy);
            OwnerApplicationSettings appsets = Configuration.GetAppSettings();
            Store currentStore = SessionManager.GetObjectByKey<Store>(header.Store);
            PriceCatalogPolicy storePolicy;
            if (currentStore.DefaultPriceCatalogPolicy == header.PriceCatalogPolicy)
            {
                storePolicy = headerPolicy;
                headerPolicy = null;
            }
            else
            {
                storePolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(currentStore.DefaultPriceCatalogPolicy);
            }

            foreach (LinkedItem li in item.LinkedItems)
            {
                Item subItem = SessionManager.GetObjectByKey<Item>(li.SubItem);
                if (subItem != null)
                {
                    string liCode = appsets != null && appsets.PadItemCodes ? subItem.Code.PadLeft(appsets.ItemCodeLength, appsets.ItemCodePaddingCharacter[0]) : subItem.Code;
                    string sbarcode = appsets != null && appsets.PadBarcodes ? liCode.PadLeft(appsets.BarcodeLength, appsets.BarcodePaddingCharacter[0]) : liCode;
                    Barcode subItemBc = SessionManager.FindObject<Barcode>(new BinaryOperator("Code", sbarcode, BinaryOperatorType.Equal));
                    PriceCatalogDetail subItemPCD;

                    ItemService.GetUnitPriceFromPolicies(headerPolicy, storePolicy, subItem, out subItemPCD);//vat included?
                    if (subItemPCD == null)
                    {
                        dl.Delete();
                        throw new PriceNotFoundException(POSClientResources.PRICE_DOES_NOT_EXIST);
                    }
                    //if (subItemPCD.Value < 0.01m)

                    DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
                    if (subItemPCD.Value < 0.01m && (Configuration.DefaultDocumentTypeOid == documentType.Oid || documentType.UsesPrices))
                    {
                        dl.Delete();
                        throw new POSException(POSClientResources.PRICE_IS_ZERO + " " + POSClientResources.ITEM + " " + subItem.Code + " - " + subItem.Name);
                    }
                    currentMaxLineNumber++;
                    decimal linkedQty = dl.Qty * (decimal)li.QtyFactor;
                    if (linkedQty <= 0 && isReturn == false ||
                       linkedQty >= 0 && isReturn == true)
                    {
                        throw new POSException(item.Code + " - " + "Linked item error: " + POSClientResources.INVALID_QUANTITY + " (" + linkedQty + ")");
                    }

                    DocumentDetail subDocLine = new DocumentDetail(header.Session) { DocumentHeader = header, LinkedLine = dl.Oid, IsReturn = isReturn, LineNumber = currentMaxLineNumber };
                    this.ComputeDocumentLine(subDocLine, subItemBc, subItem, subItemPCD, true, -1, false, subItemPCD.VATIncluded, "", null, IsForWholesale, header.CreatedByDevice, linkedQty);
                    subDocLine.CreatedBy = user;
                }
            }

            return dl;
        }

        /// <summary>
        /// Checks if loyalty can be redeemed for the given document.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public bool CheckLoyaltyRefund(DocumentHeader header)
        {
            Customer customer = SessionManager.GetObjectByKey<Customer>(header.Customer);
            DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

            OwnerApplicationSettings settings = Configuration.GetAppSettings();
            if (settings.RefundPoints > 0 && settings.SupportLoyalty && documentType.SupportLoyalty && customer.CollectedPoints >= settings.RefundPoints &&
                customer.Oid != Configuration.DefaultCustomerOid && (settings.OnlyRefundStore == false || customer.RefundStore == Configuration.CurrentStoreOid))
            {
                decimal validLinesGrossTotalBeforPointsAndDocumentDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.POINTS);
                decimal pointsToConsume;
                decimal totalLoyaltyDiscount = GetTotalLoyatyDiscountAmount(header, customer, out pointsToConsume);

                if (settings.DiscountAmount > 0 &&
                    (totalLoyaltyDiscount > validLinesGrossTotalBeforPointsAndDocumentDiscount || totalLoyaltyDiscount == 0))
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the discount distributed at the document lines by the document header, per vat category.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="vatADiscount"></param>
        /// <param name="vatBDiscount"></param>
        /// <param name="vatCDiscount"></param>
        /// <param name="vatDDiscount"></param>
        /// <param name="vatEDiscount"></param>
        public void GetDocumentDiscountPerVatCategory(DocumentHeader header, out decimal vatADiscount, out decimal vatBDiscount, out decimal vatCDiscount, out decimal vatDDiscount, out decimal vatEDiscount)
        {
            vatADiscount = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.A && x.IsCanceled == false).Sum(x => x.AllDocumentHeaderDiscounts);
            vatBDiscount = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.B && x.IsCanceled == false).Sum(x => x.AllDocumentHeaderDiscounts);
            vatCDiscount = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.C && x.IsCanceled == false).Sum(x => x.AllDocumentHeaderDiscounts);
            vatDDiscount = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.D && x.IsCanceled == false).Sum(x => x.AllDocumentHeaderDiscounts);
            vatEDiscount = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.E && x.IsCanceled == false).Sum(x => x.AllDocumentHeaderDiscounts);
        }

        /// <summary>
        /// Cancels a document line and updates the header.
        /// </summary>
        /// <param name="line"></param>
        public void CancelDocumentLine(DocumentDetail line)
        {
            DocumentHeader documentHeader = line.DocumentHeader;

            //if (docHeader.DocumentDetails.Where(x => x.IsCanceled == false).Count() == 1 &&
            //   docHeader.DocumentDetails.FirstOrDefault(x => x.IsCanceled == false) == line)
            //{
            //    throw new POSException(POSClientResources.CANNOT_CANCEL_ALL_LINES);
            //}

            List<Guid> linkedLines = documentHeader.DocumentDetails.Where(docDet => docDet.LinkedLine == line.Oid).Select(g => g.Oid).ToList();

            if (linkedLines.Count() > 0)
            {
                foreach (Guid documentDetailOid in linkedLines)
                {
                    DocumentDetail documentDetail = documentHeader.Session.GetObjectByKey<DocumentDetail>(documentDetailOid);
                    if (documentDetail != null)
                    {
                        documentDetail.IsCanceled = true;

                        foreach (DocumentDetailDiscount documentDetailDiscount in documentDetail.DocumentDetailDiscounts)
                        {
                            TransactionCoupon transactionCoupon = documentHeader.TransactionCoupons
                            .Where(transCoupon =>
                                       transCoupon.IsCanceled == false
                                    && transCoupon.DocumentDetailDiscount != null
                                    && transCoupon.DocumentDetailDiscount.Oid == documentDetailDiscount.Oid
                                  )
                            .FirstOrDefault();

                            if (transactionCoupon != null)
                            {
                                transactionCoupon.IsCanceled = true;
                                transactionCoupon.Save();
                            }
                        }

                        documentDetail.Save();
                    }
                }
            }

            foreach (DocumentDetailDiscount documentDetailDiscount in line.DocumentDetailDiscounts)
            {
                TransactionCoupon transactionCoupon = documentHeader.TransactionCoupons
                .Where(transCoupon =>
                           transCoupon.IsCanceled == false
                        && transCoupon.DocumentDetailDiscount != null
                        && transCoupon.DocumentDetailDiscount.Oid == documentDetailDiscount.Oid
                      )
                .FirstOrDefault();

                if (transactionCoupon != null)
                {
                    transactionCoupon.IsCanceled = true;
                    transactionCoupon.Save();
                }
            }

            line.IsCanceled = true;
            line.Save();
            RecalculateDocumentCosts(documentHeader, false);
        }

        /// <summary>
        /// Changes the price of the document line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="newUnitPrice"></param>
        public void ChangeDocumentLinePrice(DocumentDetail line, decimal newUnitPrice)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            if (line.DocumentDetailDiscounts.Where(x => x.DiscountType == eDiscountType.VALUE).Sum(x => x.Value) > newUnitPrice)
            {
                throw new POSException(POSClientResources.INVALID_AMOUNT);
            }

            DocumentType docType = SessionManager.GetObjectByKey<DocumentType>(line.DocumentHeader.DocumentType);
            Customer customer = SessionManager.GetObjectByKey<Customer>(line.DocumentHeader.Customer);
            line.PriceListUnitPrice = newUnitPrice;
            line.HasCustomPrice = true;
            DocumentHeader header = line.DocumentHeader;

            if (docType.IsForWholesale)
            {
                WholesaleDocumentDetail(ref line, customer, true, true);
            }
            else
            {
                RetailDocumentDetail(ref line, customer, false, true);
            }

            RecalculateDocumentCosts(header, false);
        }

        /// <summary>
        /// Adds or updates a custom discount at the given document line.
        /// </summary>
        /// <param name="discountType"></param>
        /// <param name="valueOrPercentage"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public DocumentDetailDiscount CreateOrUpdateCustomDiscount(DiscountType discountType, decimal valueOrPercentage, DocumentDetail detail)
        {
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.Type == discountType.Oid && discountType.IsUnique);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(SessionManager.GetSession<DocumentDetailDiscount>());
            }

            decimal percentage = (discountType.eDiscountType == eDiscountType.PERCENTAGE) ? valueOrPercentage : 0;
            decimal value = (discountType.eDiscountType == eDiscountType.VALUE) ? valueOrPercentage : 0;
            eDiscountSource discountSource = discountType.IsHeaderDiscount ? eDiscountSource.DOCUMENT : eDiscountSource.CUSTOM;

            return PlatformDocumentDiscountService.UpdateDiscount(discount, percentage, value, discountType.Priority, discountSource, discountType.eDiscountType, discountType, discountType.DiscardsOtherDiscounts) as DocumentDetailDiscount;
        }

        public DocumentPaymentEdps CreateDocumentPaymentEdps(EdpsDeviceResult edpsresult, decimal amount, Session session)
        {
            DocumentPaymentEdps edpsPayment = new DocumentPaymentEdps(session);
            edpsPayment.AuthID = edpsresult.AuthID;
            edpsPayment.BankID = edpsresult.BankID;
            edpsPayment.BatchNumber = edpsresult.BatchNumber;
            edpsPayment.CardHolder = edpsresult.CardHolder;
            edpsPayment.CardProduct = edpsresult.CardProduct;
            edpsPayment.ErrorCode = edpsresult.ErrorCode;
            edpsPayment.OnTopAmount = edpsresult.OnTopAmount;
            edpsPayment.PAN = edpsresult.PAN;
            edpsPayment.ReceiptNumber = edpsresult.ReceiptNumber;
            edpsPayment.ResponseCode = edpsresult.ResponseCode;
            edpsPayment.RRN = edpsresult.RRN;
            edpsPayment.TimeStamp = edpsresult.TimeStamp;
            edpsPayment.TransactionID = edpsresult.TransactionID;
            edpsPayment.TRM = edpsresult.TRM;

            edpsPayment.Amount = amount;

            if (edpsresult.EdpsLoyaltyData != null)
            {
                edpsPayment.LoyaltyAdjustedAmount = edpsresult.EdpsLoyaltyData.AdjustedAmount;
                edpsPayment.LoyaltyAwardedPoints = edpsresult.EdpsLoyaltyData.AwardedPoints;
                edpsPayment.LoyaltyBalance = edpsresult.EdpsLoyaltyData.LoyaltyBalance;
                edpsPayment.LoyaltyConsumedPoints = edpsresult.EdpsLoyaltyData.ConsumedPoints;
                edpsPayment.LoyaltyMasterMerchantPoints = edpsresult.EdpsLoyaltyData.MasterMerchantPoints;
                edpsPayment.LoyaltyResponseCode = edpsresult.EdpsLoyaltyData.LoyaltyResponseCode;
                edpsPayment.LoyaltySchemeID = edpsresult.EdpsLoyaltyData.LoyaltySchemeID;
                edpsPayment.MerchantPoints = edpsresult.EdpsLoyaltyData.MerchantPoints;
            }

            if (edpsresult.EdpsEMVData != null)
            {
                edpsPayment.EMVApplicationID = edpsresult.EdpsEMVData.ApplicationID;
                edpsPayment.EMVApplicationName = edpsresult.EdpsEMVData.ApplicationName;
                edpsPayment.EMVCrypto = edpsresult.EdpsEMVData.Crypto;
            }

            return edpsPayment;
        }

        public DocumentPaymentCardlink CreateDocumentPaymentCardlink(CardlinkDeviceResult cardlinkresult, decimal amount, Session session)
        {
            DocumentPaymentCardlink cardlinkPayment = new DocumentPaymentCardlink(session);
            cardlinkPayment.AccNum = cardlinkresult.AccNum;
            cardlinkPayment.CardType = cardlinkresult.CardType;
            cardlinkPayment.BatchNum = cardlinkresult.BatchNum;
            cardlinkPayment.EftTid = cardlinkresult.EftTid;
            cardlinkPayment.SessionId = cardlinkresult.SessionId;
            cardlinkPayment.MsgOpt = cardlinkresult.MsgOpt;
            cardlinkPayment.MsgType = cardlinkresult.MsgType;
            cardlinkPayment.RespCode = cardlinkresult.RespCode;
            cardlinkPayment.RespMesg = cardlinkresult.RespMesg;
            cardlinkPayment.tipAmount = cardlinkresult.tipAmount;
            cardlinkPayment.RefNum = cardlinkresult.RefNum;
            cardlinkPayment.BatchNum = cardlinkresult.BatchNum;
            cardlinkPayment.AuthCode = cardlinkresult.AuthCode;
            cardlinkPayment.ExchangeRateInclMarkup = cardlinkresult.ExchangeRateInclMarkup;
            cardlinkPayment.DccMarkupPercentage = cardlinkresult.DccMarkupPercentage;
            cardlinkPayment.DccExchangeDateOfRate = cardlinkresult.DccExchangeDateOfRate;
            cardlinkPayment.Amount = amount;
            cardlinkPayment.ReceiptNumber = cardlinkresult.ReceiptNumber;
            cardlinkPayment.ErrorMessage = cardlinkresult.PosMessage;
            return cardlinkPayment;
        }

        protected DocumentDetailDiscount CreateOrUpdatePriceCatalogDetailDiscount(UnitOfWork uow, decimal priceCatalogDiscountPercentage, DocumentDetail detail)
        {
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            return PlatformDocumentDiscountService.UpdateDiscount(discount, priceCatalogDiscountPercentage, 0,
                PlatformPromotionService.PRICE_CATALOG_DISCOUNT_PRIORITY, eDiscountSource.PRICE_CATALOG,
                eDiscountType.PERCENTAGE, null, false) as DocumentDetailDiscount;
        }

        protected DocumentDetailDiscount CreateOrUpdateCustomerDiscount(UnitOfWork uow, decimal customerDiscountPercentage, DocumentDetail detail)
        {
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.CUSTOMER);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            return PlatformDocumentDiscountService.UpdateDiscount(discount, customerDiscountPercentage, 0,
                PlatformPromotionService.CUSTOMER_DISCOUNT_PRIORITY, eDiscountSource.CUSTOMER,
                eDiscountType.PERCENTAGE, null, false) as DocumentDetailDiscount;
        }
        protected DocumentDetailDiscount CreateOrUpdateDefaultDocumentDiscount(UnitOfWork uow, decimal defaultDocumentDiscountPercentage, DocumentDetail detail)
        {
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            return PlatformDocumentDiscountService.UpdateDiscount(discount, defaultDocumentDiscountPercentage, 0,
                PlatformPromotionService.DEFAULT_DOCUMENT_DISCOUNT_PRIORITY, eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT,
                eDiscountType.PERCENTAGE, null, false) as DocumentDetailDiscount;
        }

        protected DocumentDetailDiscount CreateOrUpdateDiscountFromHeaderDiscount(UnitOfWork uow, DocumentHeader header, DocumentDetail detail)
        {
            decimal percentage = header.DocumentDiscountPercentagePerLine;

            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            discount.Percentage = percentage;
            discount.Priority = PlatformPromotionService.DOCUMENT_HEADER_DISCOUNT_PRIORITY;
            discount.DiscountSource = eDiscountSource.DOCUMENT;
            discount.DiscountType = eDiscountType.PERCENTAGE;
            DiscountType discountType = SessionManager.GetObjectByKey<DiscountType>(header.DocumentDiscountType);
            return PlatformDocumentDiscountService.UpdateDiscount(discount, percentage, 0,
                PlatformPromotionService.DOCUMENT_HEADER_DISCOUNT_PRIORITY, eDiscountSource.DOCUMENT,
                eDiscountType.PERCENTAGE, discountType, false, discountType == null ? null : discountType.Description) as DocumentDetailDiscount;
        }

        protected DocumentDetailDiscount CreateOrUpdatePromotionsDocumentDiscount(UnitOfWork uow, DocumentHeader header, DocumentDetail detail)
        {
            decimal percentage = header.PromotionsDiscountPercentagePerLine;
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            discount.Percentage = percentage;
            discount.Priority = PlatformPromotionService.PROMOTIONS_DOCUMENT_DISCOUNT_PRIORITY;
            discount.DiscountSource = PlatformPromotionService.PROMOTION_DOCUMENT_DISCOUNT_SOURCE;
            discount.DiscountType = eDiscountType.PERCENTAGE;

            return PlatformDocumentDiscountService.UpdateDiscount(discount, percentage, 0,
                PlatformPromotionService.PROMOTIONS_DOCUMENT_DISCOUNT_PRIORITY, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT,
                eDiscountType.PERCENTAGE, null, false) as DocumentDetailDiscount;
        }

        protected DocumentDetailDiscount CreateOrUpdateLoyaltyDiscount(UnitOfWork uow, DocumentHeader header, DocumentDetail detail)
        {
            decimal percentage = header.PointsDiscountPercentagePerLine;
            DocumentDetailDiscount discount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.POINTS);

            if (discount == null)
            {
                discount = new DocumentDetailDiscount(uow);
            }

            discount.Percentage = percentage;
            discount.Priority = PlatformPromotionService.LOYALTY_DISCOUNT_PRIORITY;
            discount.DiscountSource = eDiscountSource.POINTS;
            discount.DiscountType = eDiscountType.PERCENTAGE;

            return PlatformDocumentDiscountService.UpdateDiscount(discount, percentage, 0,
                PlatformPromotionService.LOYALTY_DISCOUNT_PRIORITY, eDiscountSource.POINTS,
                eDiscountType.PERCENTAGE, null, false) as DocumentDetailDiscount;
        }

        private void ComputeDocumentLine(DocumentDetail documentLine, Barcode barcode, Item item, PriceCatalogDetail priceCatalogDetail,
            bool isLinkedLine, decimal customPrice, bool hasCustomPrice, bool vatIncluded,
            string customDescription, IEnumerable<DocumentDetailDiscount> customDiscounts, bool IsForWholesale, string device, decimal qty)
        {
            decimal price;  // -> PriceListUnitPrice

            try
            {
                OwnerApplicationSettings appsets = Configuration.GetAppSettings();
                ////Case 4685 Μηδενικές τιμές μονάδας στα παραστατικά
                DocumentType docType = SessionManager.GetObjectByKey<DocumentType>(documentLine.DocumentHeader.DocumentType);
                if (hasCustomPrice && customPrice <= 0 && docType != null && !docType.AllowItemZeroPrices)
                {
                    customPrice = 1;
                }
                if (Configuration.DefaultDocumentTypeOid != docType.Oid && !docType.UsesPrices)
                {
                    customPrice = 0;
                }

                //// Συμπληρώνουμε τα βασικά στοιχεία της γραμμής: Είδος, barcode & description
                documentLine.Item = item.Oid;
                documentLine.HasCustomPrice = hasCustomPrice;
                documentLine.Barcode = barcode == null ? Guid.Empty : barcode.Oid;
                documentLine.ItemCode = barcode != null ? barcode.Code : (appsets.TrimBarcodeOnDisplay ? item.Code.TrimStart(appsets.ItemCodePaddingCharacter[0]) : item.Code);
                documentLine.ItemName = item.Name;
                documentLine.Qty = qty;
                documentLine.DoesNotAllowDiscount = item.DoesNotAllowDiscount;
                documentLine.IsTax = item.IsTax;
                //(decimal)(barcode != null && appsets != null && appsets.UseBarcodeRelationFactor && barcode.RelationFactor(item.Owner) != 0 ? barcode.RelationFactor(item.Owner) * qty : qty);

                if (!String.IsNullOrEmpty(customDescription)) //&& priceCatalogDetail.Item.AcceptsCustomDescription
                {
                    documentLine.CustomDescription = customDescription;
                }
                else
                {
                    documentLine.CustomDescription = item.NameWithExtraInfo;
                }
                string extraInfoDescription = item.ItemExtraInfoObject?.Description;
                if (string.IsNullOrWhiteSpace(extraInfoDescription))
                {
                    extraInfoDescription = item.Name;
                }
                documentLine.ItemExtraInfoDescription = extraInfoDescription;
                documentLine.CreatedByDevice = device;

                //// Συμπληρώνουμε ποσότητες & εκπτώσεις της γραμμής
                //documentLine.Qty = qty; //έχει μπει από έξω
                //documentLine.FirstDiscount = priceCatalogDetail == null ? 0 : priceCatalogDetail.Discount;
                //documentLine.SecondDiscount = secondDiscount;
                if (customDiscounts != null)
                {
                    foreach (DocumentDetailDiscount discount in customDiscounts)
                    {
                        if (!discount.DocumentDetail.DoesNotAllowDiscount && !discount.DocumentDetail.IsTax)
                        {
                            documentLine.DocumentDetailDiscounts.Add(discount);
                        }
                    }
                }

                Customer customer = SessionManager.GetObjectByKey<Customer>(documentLine.DocumentHeader.Customer);


                if (!documentLine.IsTax && !documentLine.DoesNotAllowDiscount)
                {
                    if (documentLine.DocumentHeader.PromotionsDiscountPercentagePerLine > 0 && documentLine.IsReturn == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdatePromotionsDocumentDiscount((UnitOfWork)documentLine.Session, documentLine.DocumentHeader, documentLine));
                    }

                    if (documentLine.DocumentHeader.DefaultDocumentDiscountPercentagePerLine > 0 && documentLine.IsReturn == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdateDefaultDocumentDiscount((UnitOfWork)documentLine.Session, (decimal)documentLine.DocumentHeader.DefaultDocumentDiscountPercentagePerLine, documentLine));
                    }

                    if (documentLine.DocumentHeader.CustomerDiscountPercentagePerLine > 0 && documentLine.IsReturn == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdateCustomerDiscount((UnitOfWork)documentLine.Session, (decimal)documentLine.DocumentHeader.CustomerDiscountPercentagePerLine, documentLine));
                    }

                    if (documentLine.DocumentHeader.PointsDiscountPercentage > 0 && documentLine.IsReturn == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdateLoyaltyDiscount((UnitOfWork)documentLine.Session, documentLine.DocumentHeader, documentLine));
                    }

                    if (documentLine.DocumentHeader.DocumentDiscountPercentage > 0 && documentLine.IsReturn == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdateDiscountFromHeaderDiscount((UnitOfWork)documentLine.Session, documentLine.DocumentHeader, documentLine));
                    }

                    //Computes points of line
                    if (customer.Oid != Configuration.DefaultCustomerOid)
                    {
                        //PriceCatalog pc = SessionManager.GetObjectByKey<PriceCatalog>(documentLine.DocumentHeader.PriceCatalog);
                        documentLine.Points = ItemService.GetPointsOfItem(item, docType) * Math.Floor(documentLine.Qty);
                    }
                }

                //// get vat factor
                VatFactor vatFactor = null;

                VatFactor storeVatFactor = null;
                Store store = SessionManager.GetObjectByKey<Store>(documentLine.DocumentHeader.Store);
                if (store != null)
                {
                    Address address = SessionManager.GetObjectByKey<Address>(store.Address);
                    if (address != null)
                    {
                        if (docType.SupportCustomerVatLevel)
                        {
                            VatLevel vatlev = null;
                            Address vatLevelAddress = SessionManager.GetObjectByKey<Address>(documentLine.DocumentHeader.BillingAddress);
                            if (vatLevelAddress != null && vatLevelAddress.Trader == customer.Trader)
                            {
                                vatlev = SessionManager.GetObjectByKey<VatLevel>(vatLevelAddress.VatLevel);
                            }
                            if (vatlev != null)
                            {
                                vatFactor = SessionManager.FindObject<VatFactor>(
                                            CriteriaOperator.And(new BinaryOperator("VatCategory", item.VatCategory, BinaryOperatorType.Equal),
                                               new BinaryOperator("VatLevel", vatlev.Oid, BinaryOperatorType.Equal)));
                            }
                            if (vatFactor == null)
                            {
                                vatFactor = SessionManager.FindObject<VatFactor>(
                             CriteriaOperator.And(new BinaryOperator("VatCategory", item.VatCategory, BinaryOperatorType.Equal),
                                                  new BinaryOperator("VatLevel", address.VatLevel, BinaryOperatorType.Equal)));
                            }
                        }
                        else
                        {

                            vatFactor = SessionManager.FindObject<VatFactor>(
                                    CriteriaOperator.And(new BinaryOperator("VatCategory", item.VatCategory, BinaryOperatorType.Equal),
                                                         new BinaryOperator("VatLevel", address.VatLevel, BinaryOperatorType.Equal)));
                        }

                        storeVatFactor = SessionManager.FindObject<VatFactor>(
                                    CriteriaOperator.And(new BinaryOperator("VatCategory", item.VatCategory, BinaryOperatorType.Equal),
                                                         new BinaryOperator("VatLevel", address.VatLevel, BinaryOperatorType.Equal)));


                        if (vatFactor == null)
                        {
                            string message = POSClientResources.VAT_FACTOR_NOT_FOUND + ": ";
                            if (address.VatLevel == Guid.Empty)
                            {
                                message += POSClientResources.STORE_HAS_NO_VAT_LEVEL + ". ";
                            }
                            if (item.VatCategory == Guid.Empty)
                            {
                                message += POSClientResources.ITEM_HAS_NO_VAT_CATEGORY + ". ";
                            }
                            throw new POSException(message);
                        }
                    }
                    else
                    {
                        throw new POSException("Current store's address not found");
                    }
                }
                else
                {
                    throw new POSException("Current store not found");
                }

                documentLine.VatFactor = vatFactor.Factor;
                documentLine.VatFactorGuid = vatFactor.Oid;
                documentLine.PriceCatalogValueVatIncluded = vatIncluded;
                documentLine.ItemVatCategoryDescription = String.Format("{0,6:#0.0 %}", documentLine.VatFactor);
                VatCategory itemVatCategory = SessionManager.GetObjectByKey<VatCategory>(item.VatCategory);
                if (itemVatCategory != null)
                {
                    documentLine.ItemVatCategoryMinistryCode = itemVatCategory.MinistryVatCategoryCode;
                }

                //SessionManager.Settings.GetObjectByKey<VatCategory>(item.VatCategory).MinistryVatCategoryCode.GetDescription();
                //Μονάδα μέτρησης
                if (barcode != null)
                {
                    documentLine.MeasurementUnit = barcode.MeasurementUnit(item.Owner);
                }
                else
                {
                    Barcode defaultBarcode = SessionManager.GetObjectByKey<Barcode>(item.DefaultBarcode);
                    documentLine.MeasurementUnit = defaultBarcode != null ? defaultBarcode.MeasurementUnit(item.Owner) : Guid.Empty;
                }

                documentLine.PackingQuantity = documentLine.Qty;
                documentLine.PackingMeasurementUnitRelationFactor = 1;
                documentLine.PackingMeasurementUnit = documentLine.MeasurementUnit;

                // SOS
                if (hasCustomPrice)
                {
                    if (storeVatFactor != null && vatFactor != null && storeVatFactor != vatFactor)
                    {
                        decimal storeVatAmount = vatIncluded ?
                            PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value - (priceCatalogDetail.Value / (1 + storeVatFactor.Factor)))
                            : PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value * storeVatFactor.Factor);

                        decimal customerVatAmount = vatIncluded ?
                            PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value - (priceCatalogDetail.Value / (1 + vatFactor.Factor)))
                            : PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value * vatFactor.Factor);

                        decimal vatDifference = storeVatAmount - customerVatAmount;
                        price = customPrice - vatDifference;
                    }
                    else
                    {
                        price = customPrice;
                    }
                    documentLine.PriceCatalogDetail = null;
                }
                else
                {
                    if (storeVatFactor != vatFactor)
                    {
                        decimal storeVatAmount = vatIncluded ?
                            PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value - (priceCatalogDetail.Value / (1 + storeVatFactor.Factor)))
                            : PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value * storeVatFactor.Factor);

                        decimal customerVatAmount = vatIncluded ?
                            PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value - (priceCatalogDetail.Value / (1 + vatFactor.Factor)))
                            : PlatformRoundingHandler.RoundDisplayValue(priceCatalogDetail.Value * vatFactor.Factor);

                        decimal vatDifference = storeVatAmount - customerVatAmount;
                        price = priceCatalogDetail == null ? -1 : priceCatalogDetail.Value - vatDifference;
                    }
                    else
                    {
                        price = priceCatalogDetail == null ? -1 : priceCatalogDetail.Value;
                    }
                    documentLine.PriceCatalogDetail = priceCatalogDetail.Oid;
                    documentLine.PriceCatalog = priceCatalogDetail.PriceCatalog;

                    if (priceCatalogDetail.Discount > 0 && documentLine.IsTax == false && documentLine.DoesNotAllowDiscount == false)
                    {
                        documentLine.DocumentDetailDiscounts.Add(CreateOrUpdatePriceCatalogDetailDiscount((UnitOfWork)documentLine.Session, priceCatalogDetail.Discount, documentLine));
                    }
                }

                if ((price < 0 && hasCustomPrice == false) || (Configuration.DefaultDocumentTypeOid != docType.Oid && !docType.UsesPrices))
                {
                    price = .0m;
                }

                documentLine.PriceListUnitPrice = price;

                if (IsForWholesale)
                {
                    //Round στην τιμή, με βάση
                    documentLine.PriceListUnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentLine.PriceListUnitPrice);
                    WholesaleDocumentDetail(ref documentLine, customer, IsForWholesale, vatIncluded);
                }
                else
                {
                    documentLine.PriceListUnitPrice = vatIncluded ? PlatformRoundingHandler.RoundDisplayValue(documentLine.PriceListUnitPrice) : PlatformRoundingHandler.RoundDisplayValue(documentLine.PriceListUnitPrice);
                    RetailDocumentDetail(ref documentLine, customer, IsForWholesale, vatIncluded);
                }

                SessionManager.FillDenormalizedFields(documentLine);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null && String.IsNullOrEmpty(ex.InnerException.Message) == false)
                {
                    message += " " + ex.InnerException.Message;
                }
                throw new POSException(message, ex);
            }
        }

        /*
        private Guid _LinkedLine;
        private double _UnitPriceAfterDiscount;             // Τελικη τιμή άνευ ΦΠΑ μετά έκπτωσης
        private double _PriceListUnitPrice;                 // Αρχική τιμή άνευ ΦΠΑ ή τιμή καρφωτή/χρήστη
        private double _UnitPrice;                          // PriceListUnitPrice μετά έκπτωσης πελάτη προ ΦΠΑ
        private double _Qty;                                // Ποσότητα
        private double _FirstDiscount;                      // Εκπτωση τιμοκαταλόγου ανά τεμάχιο
        private double _SecondDiscount;                     // 2η Εκπτωση (επιπλέον) ανά τεμάχιο
        private double _VatFactor;                          // Ποσοστό ΦΠΑ
        private double _VatAmount;                          // Ποσό ΦΠΑ ανά τεμάχιο
        private double _NetTotalAfterDiscount;              // Καθαρή αξία (μετά εκπτώσεων)
        private double _GrossTotal;                         // Συνολική αξία γραμμής (με ΦΠΑ)
        private double _FinalUnitPrice;                     // Τελική τιμή μονάδος με ΦΠΑ
        private double _TotalDiscount;                      // Συνολικό ποσό έκπτωσης προ ΦΠΑ
        private double _TotalVatAmount;                     // Συνολικό ποσό ΦΠΑ
        private double _NetTotal;                           // Καθαρή αξία (προ εκπτώσεων)
         *
         * Γνωστά
         * _FirstDiscount;
         * _SecondDiscount;
         * _VatFactor
         * _Qty
         * _PriceListUnitPrice (w or wout VAT);
         * * */

        protected void RetailDocumentDetail(ref DocumentDetail documentDetail, Customer customer, bool IsForWholesale, bool vatIncluded)
        {
            if (IsForWholesale)
            {
                throw new Exception("This code has been called incorrectly....");
            }
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            if (vatIncluded) //(pcd.VatIncluded)
            {
                documentDetail.FinalUnitPrice = documentDetail.PriceListUnitPrice;
                documentDetail.UnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentDetail.PriceListUnitPrice / (1 + documentDetail.VatFactor));
            }
            else
            {
                documentDetail.UnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentDetail.PriceListUnitPrice);
                documentDetail.FinalUnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentDetail.PriceListUnitPrice * (1 + documentDetail.VatFactor));
            }

            documentDetail.PriceCatalogValueVatIncluded = vatIncluded;

            CalculateDocumentDetailTotals(documentDetail, false, true);
        }

        /// <summary>
        /// Calculates the TotalDiscount field of the detail using the DocumentDetailDiscounts list.
        /// </summary>
        /// <param name="documentDetail"></param>
        private void CalculateDetailTotalDiscount(ref DocumentDetail documentDetail, DocumentHeader header)
        {
            DocumentType docType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
            bool isWholeSale = docType.IsForWholesale;
            decimal totalDiscount = 0, originalAmount;
            decimal amountToApplyDiscount = originalAmount = isWholeSale ? documentDetail.NetTotalBeforeDiscount
                : documentDetail.GrossTotalBeforeDiscount;

            DocumentDetailDiscount overridesAllDiscount = documentDetail.DocumentDetailDiscounts.OrderBy(x => x.Priority).FirstOrDefault(x => x.DiscardsOtherDiscounts);
            if (overridesAllDiscount != null)
            {
                documentDetail.TotalDiscount = CalculateDiscountAmount(overridesAllDiscount, amountToApplyDiscount, overridesAllDiscount.DiscountSource);
                if (isWholeSale)
                {
                    overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value;
                    overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value * (1 + documentDetail.VatFactor);
                }
                else
                {
                    overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value;
                    overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value / (1 + documentDetail.VatFactor);
                }
                return;
            }

            foreach (DocumentDetailDiscount discount in documentDetail.DocumentDetailDiscounts.OrderBy(x => x.Priority))
            {
                decimal discountAmount = CalculateDiscountAmount(discount, amountToApplyDiscount, discount.DiscountSource);
                totalDiscount += discountAmount;
                amountToApplyDiscount = originalAmount - totalDiscount;
                if (isWholeSale)
                {
                    discount.DiscountWithoutVAT = discount.Value;
                    discount.DiscountWithVAT = discount.Value * (1 + documentDetail.VatFactor);
                }
                else
                {
                    discount.DiscountWithVAT = discount.Value;
                    discount.DiscountWithoutVAT = discount.Value / (1 + documentDetail.VatFactor);
                }
            }
            documentDetail.TotalDiscount = PlatformRoundingHandler.RoundValue(totalDiscount);
            if (isWholeSale)
            {
                documentDetail.TotalDiscountAmountWithoutVAT = documentDetail.TotalDiscount;
                documentDetail.TotalDiscountAmountWithVAT = documentDetail.TotalDiscount * (1 + documentDetail.VatFactor);
            }
            else
            {
                documentDetail.TotalDiscountAmountWithoutVAT = documentDetail.TotalDiscount / (1 + documentDetail.VatFactor);
                documentDetail.TotalDiscountAmountWithVAT = documentDetail.TotalDiscount;
            }
        }

        protected decimal CalculateDiscountAmount(DocumentDetailDiscount discount, decimal amountToApplyDiscount, eDiscountSource source)
        {
            decimal discountValue = 0;
            if (discount.DocumentDetail.IsTax == false && discount.DocumentDetail.DoesNotAllowDiscount == false)
            {
                if (discount.DiscountType == eDiscountType.PERCENTAGE)
                {
                    OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
                    discountValue = PlatformRoundingHandler.RoundDisplayValue(amountToApplyDiscount * discount.Percentage);
                    discount.Value = discountValue;
                }
                else
                {
                    discountValue = discount.Value;
                    discount.Percentage = discountValue / amountToApplyDiscount;
                }
            }

            return discountValue;
        }

        public IEnumerable<DocumentVatInfo> GetDocumentVatAnalysis(DocumentHeader documentHeader)
        {
            List<DocumentVatInfo> vatAnalysis = new List<DocumentVatInfo>();

            if (documentHeader != null && documentHeader.DocumentDetails.Count > 0)
            {
                IEnumerable<Guid?> vatFactors = documentHeader.DocumentDetails.Where(x => x.VatFactorGuid != null && x.IsCanceled == false).Select(documentDetail => documentDetail.VatFactorGuid).Distinct();

                foreach (Guid? vatFactorGuid in vatFactors)
                {
                    VatFactor vatFactor = SessionManager.GetObjectByKey<VatFactor>(vatFactorGuid);
                    VatCategory vatCat = SessionManager.GetObjectByKey<VatCategory>(vatFactor.VatCategory);
                    if (vatFactor != null)
                    {
                        DocumentVatInfo documentVatInfo = new DocumentVatInfo();
                        documentVatInfo.VatFactor = vatFactor.Factor * 100;
                        IEnumerable<DocumentDetail> vatFactorDocumentDetails = documentHeader.DocumentDetails.Where(documentDetail => documentDetail.VatFactorGuid == vatFactor.Oid && documentDetail.IsCanceled == false);
                        documentVatInfo.ItemsQuantity = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.Qty);
                        documentVatInfo.NumberOfItems = vatFactorDocumentDetails.Select(documentDetail => documentDetail.Item).Distinct().Count();
                        documentVatInfo.TotalVatAmount = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.TotalVatAmount);
                        documentVatInfo.NetTotal = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.NetTotal);
                        documentVatInfo.GrossTotal = vatFactorDocumentDetails.Sum(documentDetail => documentDetail.GrossTotal);
                        documentVatInfo.VatFactorOid = vatFactor.Oid;
                        documentVatInfo.VatFactorCode = vatFactor.Code;
                        documentVatInfo.VatCategoryDescription = vatCat == null ? "" : vatCat.Description;
                        vatAnalysis.Add(documentVatInfo);
                    }
                }
            }

            return vatAnalysis.OrderByDescending(vat => vat.VatFactor);
        }

        protected void WholesaleDocumentDetail(ref DocumentDetail documentDetail, Customer customer, bool IsForWholesale, bool vatIncluded)
        {
            if (!IsForWholesale)
            {
                throw new Exception("This code has been called incorrectly....");
            }
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            if (vatIncluded)
            {
                documentDetail.UnitPrice = documentDetail.PriceListUnitPrice * (decimal)(1 - customer.Discount) / (1 + documentDetail.VatFactor);
                documentDetail.FinalUnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentDetail.PriceListUnitPrice);
            }
            else
            {
                documentDetail.UnitPrice = documentDetail.PriceListUnitPrice * (decimal)(1 - customer.Discount);
                documentDetail.FinalUnitPrice = PlatformRoundingHandler.RoundDisplayValue(documentDetail.PriceListUnitPrice * (1 + documentDetail.VatFactor));
            }
            documentDetail.PriceCatalogValueVatIncluded = vatIncluded;

            documentDetail.UnitPrice = documentDetail.PriceListUnitPrice / (1 + (vatIncluded == true ? (decimal)documentDetail.VatFactor : 0.0m));
            CalculateDocumentDetailTotals(documentDetail, true, true);
        }

        private void CalculateDocumentDetailTotals(DocumentDetail documentDetail, bool isWholeSale, bool recalculateDiscounts)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            if (isWholeSale)
            {
                documentDetail.NetTotalBeforeDiscount = (documentDetail.UnitPrice * (decimal)documentDetail.Qty);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(documentDetail.DocumentHeader.DocumentType);
                //In case that quantity is too small and net total < 00.1
                if (documentDetail.NetTotalBeforeDiscount == 0 && documentType.UsesPrices)
                {
                    documentDetail.NetTotalBeforeDiscount = (decimal)0.01;
                }

                documentDetail.GrossTotalBeforeDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.NetTotalBeforeDiscount * (1 + (decimal)documentDetail.VatFactor));
                documentDetail.TotalVatAmountBeforeDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.NetTotalBeforeDiscount);

                if (recalculateDiscounts)
                {
                    CalculateDetailTotalDiscount(ref documentDetail, documentDetail.DocumentHeader);
                }
                else
                {
                    documentDetail.TotalDiscount = documentDetail.DocumentDetailDiscounts.Count > 0 ? documentDetail.DocumentDetailDiscounts.Sum(x => x.Value) : 0;
                }


                documentDetail.TotalDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.TotalDiscount);
                documentDetail.NetTotal = PlatformRoundingHandler.RoundDisplayValue(documentDetail.NetTotalBeforeDiscount - documentDetail.TotalDiscount);
                documentDetail.TotalVatAmount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.NetTotal * (decimal)documentDetail.VatFactor);
                documentDetail.GrossTotalBeforeDocumentDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalNonDocumentDiscount);
                documentDetail.GrossTotal = PlatformRoundingHandler.RoundDisplayValue(documentDetail.NetTotal + documentDetail.TotalVatAmount);
                //documentDetail.FinalUnitPrice = documentDetail.NetTotal / (decimal)documentDetail.Qty;
                documentDetail.CustomUnitPrice = documentDetail.UnitPrice;
            }
            else
            {
                ////Retail
                documentDetail.GrossTotalBeforeDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.FinalUnitPrice * (decimal)documentDetail.Qty);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(documentDetail.DocumentHeader.DocumentType);
                //In case that quantity is too small and gross total < 00.1
                if (documentDetail.GrossTotalBeforeDiscount == 0 && (Configuration.DefaultDocumentTypeOid == documentType.Oid || documentType.UsesPrices))
                {
                    documentDetail.GrossTotalBeforeDiscount = (decimal)0.01;
                }

                documentDetail.TotalVatAmountBeforeDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount * (decimal)documentDetail.VatFactor / (1 + (decimal)documentDetail.VatFactor));
                documentDetail.NetTotalBeforeDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalVatAmountBeforeDiscount);

                if (recalculateDiscounts)
                {
                    CalculateDetailTotalDiscount(ref documentDetail, documentDetail.DocumentHeader);
                }
                else
                {
                    documentDetail.TotalDiscount = documentDetail.DocumentDetailDiscounts.Count > 0 ? documentDetail.DocumentDetailDiscounts.Sum(x => x.Value) : 0;
                }

                documentDetail.GrossTotalBeforeDocumentDiscount = documentDetail.GrossTotalBeforeDiscount - PlatformRoundingHandler.RoundDisplayValue(documentDetail.TotalNonDocumentDiscount);
                documentDetail.TotalDiscount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.TotalDiscount);
                documentDetail.GrossTotal = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalDiscount);
                documentDetail.TotalVatAmount = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotal * (decimal)documentDetail.VatFactor / (1 + (decimal)documentDetail.VatFactor));
                documentDetail.NetTotal = PlatformRoundingHandler.RoundDisplayValue(documentDetail.GrossTotal - documentDetail.TotalVatAmount);
                documentDetail.CustomUnitPrice = documentDetail.FinalUnitPrice;
            }
        }

        /// <summary>
        /// Adds the given document detail's totals to the document header.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="ActionDelete"></param>
        /// <param name="currentLine"></param>
        /// <returns></returns>
        public DocumentHeader ComputeDocumentHeader(ref DocumentHeader header, bool actionDelete, DocumentDetail currentLine)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            //if(currentLine.)
            IEnumerable<DocumentDetail> linkedLines = header.DocumentDetails.Where(g => g.LinkedLine == currentLine.Oid);
            if (linkedLines.Count() > 0)
            {
                foreach (DocumentDetail link in linkedLines)
                {
                    ComputeDocumentHeader(ref header, actionDelete, link);
                }
            }
            /// line deleted form header
            if (actionDelete && currentLine != null)
            {
                header.GrossTotalBeforeDiscount -= currentLine.GrossTotalBeforeDiscount;
                header.GrossTotal -= currentLine.GrossTotal;
                header.NetTotal -= currentLine.NetTotal;
                header.TotalDiscountAmount -= currentLine.TotalDiscount;
                header.TotalVatAmountBeforeDiscount -= currentLine.TotalVatAmountBeforeDiscount;
                header.TotalVatAmount -= currentLine.TotalVatAmount;
                //header.TotalPoints -= currentLine.Points;
                header.GrossTotalBeforeDocumentDiscount -= currentLine.GrossTotalBeforeDocumentDiscount;
                currentLine.IsCanceled = true;
                return header;
            }
            if (!actionDelete) // Αν Θέλει διαγραφή της γραμμής;
            {
                header.GrossTotalBeforeDiscount += currentLine.GrossTotalBeforeDiscount;
                header.GrossTotal += currentLine.GrossTotal;
                header.NetTotal += currentLine.NetTotal;
                header.TotalDiscountAmount += currentLine.TotalDiscount;
                header.TotalVatAmountBeforeDiscount += currentLine.TotalVatAmountBeforeDiscount;
                header.TotalVatAmount += currentLine.TotalVatAmount;
                // header.TotalPoints += currentLine.Points;
                header.GrossTotalBeforeDocumentDiscount += currentLine.GrossTotalBeforeDocumentDiscount;
            }
            header.GrossTotal = PlatformRoundingHandler.RoundDisplayValue(header.GrossTotal);
            header.NetTotal = PlatformRoundingHandler.RoundDisplayValue(header.NetTotal);
            header.TotalVatAmount = PlatformRoundingHandler.RoundDisplayValue(header.TotalVatAmount);
            header.DocumentPoints = this.CalculateHeaderPointsFromSum(header, Configuration.DefaultCustomerOid);
            header.TotalPoints = header.DocumentPoints + header.PromotionPoints + header.DocumentDetails.Where(x => x.IsCanceled == false && x.IsTax == false && x.DoesNotAllowDiscount == false && x.IsReturn == false).Sum(x => x.Points);
            header.TotalQty = header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.Qty);
            return header;
        }

        protected decimal CalculateHeaderPointsFromSum(DocumentHeader header, Guid defaultCustomerOid)
        {
            DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
            OwnerApplicationSettings settings = Configuration.GetAppSettings();
            if (header.Customer != defaultCustomerOid &&
                documentType != null && documentType.SupportLoyalty &&
                settings.SupportLoyalty)
            {
                decimal documentPoints = (settings.LoyaltyOnDocumentSum && settings.DocumentSumForLoyalty > 0) ?
                    ((int)((header.GrossTotal - GetTotalNonDiscountableValue(header)) / settings.DocumentSumForLoyalty)) * settings.LoyaltyPointsPerDocumentSum
                    : 0;
                return documentPoints;
            }
            return 0;
        }

        public DocumentDetail FindLine(string code, DocumentHeader header)
        {
            if (header == null)
            {
                return null;
            }

            OwnerApplicationSettings appsets = Configuration.GetAppSettings();
            string barcodeCode = appsets != null && appsets.PadBarcodes ? code.PadLeft(appsets.BarcodeLength, appsets.BarcodePaddingCharacter[0]) : code;
            string itemCode = appsets != null && appsets.PadItemCodes ? code.PadLeft(appsets.ItemCodeLength, appsets.ItemCodePaddingCharacter[0]) : code;

            foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
            {
                Item detailItem = SessionManager.FindObject<Item>(new BinaryOperator("Oid", detail.Item, BinaryOperatorType.Equal));
                Barcode detailBarcode = SessionManager.FindObject<Barcode>(new BinaryOperator("Oid", detail.Barcode));
                if ((detailItem != null && detailItem.Code == itemCode) || (detailBarcode != null && barcodeCode == detailBarcode.Code))
                {
                    return detail;
                }
            }

            return null;
        }

        protected decimal CalculateTotalQuantity(DocumentHeader header)
        {
            decimal qty = 0;
            foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
            {
                qty += detail.Qty;
            }

            return qty;
        }

        public void RecalculateDocumentDetail(IDocumentDetail detail, IDocumentHeader header)
        {
            DocumentDetail posDetail = detail as DocumentDetail;
            DocumentHeader posHeader = header as DocumentHeader;
            DocumentType dType = SessionManager.GetObjectByKey<DocumentType>(posHeader.DocumentType);
            if (dType == null)
            {
                throw new POSException("Document Header has no type !");
            }

            Customer customer = SessionManager.GetObjectByKey<Customer>(posDetail.DocumentHeader.Customer);
            if (customer == null)
            {
                throw new POSException("Document Header has no customer !");
            }

            if (dType.IsForWholesale == true)
            {
                this.WholesaleDocumentDetail(ref posDetail, customer, true, posDetail.PriceCatalogValueVatIncluded);
            }
            else
            {
                this.RetailDocumentDetail(ref posDetail, customer, false, posDetail.PriceCatalogValueVatIncluded);
            }
        }

        /// <summary>
        /// Επαναυπολογίζει τα κόστη της παραγγελίας. Ανάλογα με την τιμή της recompute_document_lines
        /// απλώς προσθέτει τα κόστη των γραμμών ή επαναυπολογίζει και τις γραμμές
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="recompute_document_lines"></param>
        public void RecalculateDocumentCosts(IDocumentHeader documentHeader, bool recompute_document_lines = true, bool findValuesFromDatabase = true)
        {
            DocumentHeader posDocumentHeader = documentHeader as DocumentHeader;
            OwnerApplicationSettings settings = Configuration.GetAppSettings();
            SetDocumentHeaderValuesToZero(ref posDocumentHeader);
            CriteriaOperator documentDetaisFilterToRestore = posDocumentHeader.DocumentDetails.Filter;
            posDocumentHeader.DocumentDetails.Filter = null;
            PriceCatalogPolicy headerPolicy = null, storePolicy = null;

            Customer customer = null;
            DocumentType docType = null;
            if (recompute_document_lines)
            {
                customer = SessionManager.GetObjectByKey<Customer>(posDocumentHeader.Customer);
                docType = SessionManager.GetObjectByKey<DocumentType>(posDocumentHeader.DocumentType);

                headerPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(posDocumentHeader.PriceCatalogPolicy);
                Store currentStore = SessionManager.GetObjectByKey<Store>(posDocumentHeader.Store);
                if (currentStore.DefaultPriceCatalogPolicy == headerPolicy.Oid)
                {
                    storePolicy = headerPolicy;
                    headerPolicy = null;
                }
                else
                {
                    storePolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(currentStore.DefaultPriceCatalogPolicy);
                }
            }
            foreach (DocumentDetail documentDetail in posDocumentHeader.DocumentDetails.Where(x => x.IsCanceled == false))
            {
                if (recompute_document_lines)
                {
                    Item item = SessionManager.GetObjectByKey<Item>(documentDetail.Item);
                    Barcode barcode = SessionManager.GetObjectByKey<Barcode>(documentDetail.Barcode);

                    PriceCatalogDetail pcd = null;
                    if (documentDetail.HasCustomPrice == false)
                    {
                        if (findValuesFromDatabase == false && documentDetail.PriceCatalogDetail.HasValue)
                        {
                            pcd = SessionManager.GetObjectByKey<PriceCatalogDetail>(documentDetail.PriceCatalogDetail.Value);
                        }
                        if (pcd == null)
                        {
                            decimal price = ItemService.GetUnitPriceFromPolicies(headerPolicy, storePolicy, item, out pcd, barcode);
                        }
                        if (pcd == null)
                        {
                            throw new PriceNotFoundException(POSClientResources.PRICE_DOES_NOT_EXIST);
                        }
                    }

                    ComputeDocumentLine(documentDetail, barcode, item, pcd, documentDetail.IsLinkedLine,
                                        documentDetail.HasCustomPrice ? documentDetail.FinalUnitPrice : -1,
                                        documentDetail.HasCustomPrice,
                                        documentDetail.HasCustomPrice ? docType.IsForWholesale == false : pcd.VATIncluded,
                                        documentDetail.CustomDescription,
                                        documentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM),
                                        docType.IsForWholesale,
                                        documentDetail.CreatedByDevice,
                                        documentDetail.Qty);
                }

                posDocumentHeader.GrossTotalBeforeDiscount += documentDetail.GrossTotalBeforeDiscount;
                posDocumentHeader.GrossTotal += documentDetail.GrossTotal;
                posDocumentHeader.NetTotalBeforeDiscount += documentDetail.NetTotalBeforeDiscount;
                posDocumentHeader.NetTotal += documentDetail.NetTotal;
                posDocumentHeader.TotalDiscountAmount += documentDetail.TotalDiscount;
                posDocumentHeader.TotalVatAmountBeforeDiscount += documentDetail.TotalVatAmountBeforeDiscount;
                posDocumentHeader.TotalVatAmount += documentDetail.TotalVatAmount;
                posDocumentHeader.GrossTotalBeforeDocumentDiscount += documentDetail.GrossTotalBeforeDocumentDiscount;
                posDocumentHeader.TotalQty += documentDetail.Qty;
            }
            posDocumentHeader.GrossTotal = PlatformRoundingHandler.RoundDisplayValue(posDocumentHeader.GrossTotal);
            posDocumentHeader.NetTotal = PlatformRoundingHandler.RoundDisplayValue(posDocumentHeader.NetTotal);
            posDocumentHeader.TotalVatAmount = PlatformRoundingHandler.RoundDisplayValue(posDocumentHeader.TotalVatAmount);
            posDocumentHeader.DocumentPoints = this.CalculateHeaderPointsFromSum(posDocumentHeader, Configuration.DefaultCustomerOid);
            posDocumentHeader.TotalPoints = posDocumentHeader.DocumentPoints + posDocumentHeader.PromotionPoints + posDocumentHeader.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.Points);

            posDocumentHeader.DocumentDetails.Filter = documentDetaisFilterToRestore;
        }

        /// <summary>
        /// Μηδενίζει τα σύνολα του παραστατικού όχι όμως και των γραμμών
        /// </summary>
        /// <remarks>
        /// Μπορεί να χρησιμοποιηθεί και εκτός αλλά προορίζεται για εσωτερική χρήση κυρίως
        /// </remarks>
        /// <param name="documentHeader"></param>
        protected void SetDocumentHeaderValuesToZero(ref DocumentHeader documentHeader)
        {
            documentHeader.DocumentPoints =
            documentHeader.GrossTotal =
            documentHeader.NetTotal =
            documentHeader.TotalVatAmount =
            documentHeader.TotalPoints =
            documentHeader.GrossTotalBeforeDiscount =
            documentHeader.NetTotalBeforeDiscount =
            documentHeader.TotalDiscountAmount =
            documentHeader.GrossTotalBeforeDocumentDiscount =
            documentHeader.TotalQty =
            documentHeader.TotalVatAmountBeforeDiscount = .0m;
        }

        public void AssignDocumentNumber(DocumentHeader header, Guid currentPOSOid, Guid currentUserOid, int fiscalPrinterReceiptNumber = -1)
        {
            DocumentStatus status = SessionManager.GetObjectByKey<DocumentStatus>(header.Status);
            Store store = SessionManager.GetObjectByKey<Store>(header.Store);

            StoreDocumentSeriesType sdst = SessionManager.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", header.DocumentType),
                new BinaryOperator("DocumentSeries", header.DocumentSeries)));

            DocumentSeries documentSeries = null;
            DocumentType documentType = null;
            if (sdst != null)
            {
                documentType = SessionManager.GetObjectByKey<DocumentType>(sdst.DocumentType);
                documentSeries = SessionManager.GetObjectByKey<DocumentSeries>(sdst.DocumentSeries);
            }

            if (documentType.IsPrintedOnStoreController)
            {
                return;
            }
            if (status != null && status.TakeSequence)
            {
                if (header.DocumentNumber < 1)
                {
                    if (sdst == null || documentSeries == null)
                    {
                        throw new Exception("There is a problem with your data concerning Store's Document Type, Document Series and Document Sequence!");
                    }

                    CriteriaOperator coper = new BinaryOperator("DocumentSeries", header.DocumentSeries);
                    DocumentSequence documentSequence = SessionManager.FindObject<DocumentSequence>(coper);

                    if (documentSeries.HasAutomaticNumbering)//if (sdst.HasAutomaticNumbering)
                    {
                        if (documentSequence != null)
                        {
                            if (fiscalPrinterReceiptNumber > 0 || (header.IsFiscalPrinterHandled && header.FiscalPrinterNumber > 0))  //Got it from fiscal printer
                            {
                                if (fiscalPrinterReceiptNumber < 1)
                                {
                                    header.DocumentNumber = documentSequence.DocumentNumber = header.FiscalPrinterNumber;
                                }
                                else
                                {
                                    header.DocumentNumber = documentSequence.DocumentNumber = fiscalPrinterReceiptNumber;
                                }
                            }
                            else
                            {
                                header.DocumentNumber = ++documentSequence.DocumentNumber;
                            }
                        }
                        else
                        {
                            string seriesDescription = header.DocumentSeries.ToString();
                            DocumentSeries series = SessionManager.GetObjectByKey<DocumentSeries>(header.DocumentSeries);
                            if (series != null)
                            {
                                seriesDescription = series.Description;
                            }

                            throw new POSException(String.Format(POSClientResources.DOCUMENT_SEQUENCE_NOT_FOUND_FOR_SERIES, seriesDescription));
                        }

                        documentSequence.Save();
                        documentSequence.Session.CommitTransaction();
                    }
                    else if (fiscalPrinterReceiptNumber > 0)
                    {// !documentSeries.HasAutomaticNumbering
                        header.DocumentNumber = fiscalPrinterReceiptNumber;
                    }
                }
            }
        }

        public int GetNextDocumentNumber(DocumentHeader header, Guid currentPOSOid, Guid currentUserOid)
        {
            try
            {
                DocumentStatus status = SessionManager.GetObjectByKey<DocumentStatus>(header.Status);
                Store store = SessionManager.GetObjectByKey<Store>(header.Store);
                //StoreDocumentSeries sds = SessionManager.Settings.FindObject<StoreDocumentSeries>(CriteriaOperator.And(new BinaryOperator("Store", header.Store), new BinaryOperator("Series", header.DocumentSeries)));

                StoreDocumentSeriesType sdst = SessionManager.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentType", header.DocumentType),
                    new BinaryOperator("DocumentSeries", header.DocumentSeries)));

                DocumentSeries documentSeries = null;
                DocumentType documentType = null;
                if (sdst != null)
                {
                    documentType = SessionManager.GetObjectByKey<DocumentType>(sdst.DocumentType);
                    documentSeries = SessionManager.GetObjectByKey<DocumentSeries>(sdst.DocumentSeries);
                }
                if (documentType.IsPrintedOnStoreController)
                {
                    return 0;
                }
                if (status != null && status.TakeSequence)
                {
                    if (header.DocumentNumber < 1)
                    {
                        if (sdst == null || documentSeries == null)
                        {
                            throw new Exception("There is a problem with your data concerning Store's Document Type, Document Series and Document Sequence!");
                        }

                        if (documentSeries.HasAutomaticNumbering)
                        {
                            CriteriaOperator coper = new BinaryOperator("DocumentSeries", header.DocumentSeries, BinaryOperatorType.Equal);
                            DocumentSequence ds = SessionManager.FindObject<DocumentSequence>(coper);

                            if (ds != null)
                            {
                                return ds.DocumentNumber + 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            catch //(Exception e)
            {
            }
            return -1;
        }

        public void FixPromotionsDocumentDiscountDeviations(IDocumentHeader header)
        {
            DocumentHeader posHeader = header as DocumentHeader;
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            decimal detailTotalDocumentDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(posHeader.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.PromotionsDocumentDiscountAmount));
            decimal documentDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(posHeader.PromotionsDiscountAmount);

            if (documentDiscountAmountRounded != detailTotalDocumentDiscountAmountRounded)
            {
                decimal deviation = PlatformRoundingHandler.RoundDisplayValue(detailTotalDocumentDiscountAmountRounded - documentDiscountAmountRounded);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(posHeader.DocumentType);

                if (documentType.IsForWholesale)
                {
                    this.WholeSalesFixDiscountDeviation(deviation, posHeader, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);
                }
                else
                {
                    this.RetailFixDiscountDeviation(deviation, posHeader, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);
                }
                posHeader.PromotionsDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(posHeader.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.PromotionsDocumentDiscountAmount));
            }
        }

        /// <summary>
        /// Calculates and applies the DocumentDiscountPercentagePerLine that must be applied to all the lines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="discount"></param>
        /// <param name="discountType"></param>
        /// <param name="couponViewModel">The view model for the selected coupon. Null if no coupon is used for the Document Discount.</param>
        public void ApplyCustomDocumentHeaderDiscount(ref DocumentHeader header, decimal discount, DiscountType discountType, CouponViewModel couponViewModel = null)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();

            decimal validLinesGrossTotalBeforeCustomDocumentDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.DOCUMENT);

            decimal grossTotalBeforeCustomDocumentDiscount = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.DOCUMENT);
            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                decimal desiredDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(grossTotalBeforeCustomDocumentDiscount * discount);
                header.DocumentDiscountPercentage = discount;
                header.DocumentDiscountPercentagePerLine = (int)appSettings.ComputeDigits < 4
                                                            ? Math.Round(desiredDiscountAmount / validLinesGrossTotalBeforeCustomDocumentDiscount, 4, MidpointRounding.AwayFromZero)
                                                            : PlatformRoundingHandler.RoundValue(desiredDiscountAmount / validLinesGrossTotalBeforeCustomDocumentDiscount);
            }
            else
            {
                header.DocumentDiscountPercentage = (int)appSettings.ComputeDigits < 4
                                                    ? Math.Round(discount / validLinesGrossTotalBeforeCustomDocumentDiscount, 4, MidpointRounding.AwayFromZero)
                                                    : PlatformRoundingHandler.RoundValue(discount / validLinesGrossTotalBeforeCustomDocumentDiscount);
                header.DocumentDiscountPercentagePerLine = header.DocumentDiscountPercentage;
            }

            ////assigned for reference only. The lines will always be discounted with percentage


            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                header.DocumentDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(grossTotalBeforeCustomDocumentDiscount * header.DocumentDiscountPercentage);
            }
            else
            {
                header.DocumentDiscountAmount = discount;
            }

            //CheckTaxesValue(taxesValue, header, grossTotalBeforeCustomDocumentDiscount+taxesValue);

            if (couponViewModel != null)
            {
                CreateTransactionCoupon(header, couponViewModel);
            }
            header.DocumentDiscountType = discountType.Oid;
            RecalculateDocumentCosts(header, true);
            FixDocumentDiscountDeviations(header);
        }

        public void FixDocumentDiscountDeviations(DocumentHeader header)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            decimal detailTotalDocumentDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.DocumentDiscountAmount));
            decimal documentDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDiscountAmount);

            if (documentDiscountAmountRounded != detailTotalDocumentDiscountAmountRounded)
            {
                decimal deviation = PlatformRoundingHandler.RoundDisplayValue(detailTotalDocumentDiscountAmountRounded - documentDiscountAmountRounded);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

                if (documentType.IsForWholesale)
                {
                    this.WholeSalesFixDiscountDeviation(deviation, header, eDiscountSource.DOCUMENT);
                }
                else
                {
                    this.RetailFixDiscountDeviation(deviation, header, eDiscountSource.DOCUMENT);
                }
                header.DocumentDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.DocumentDiscountAmount));
            }
        }

        public void FixDefaultDocumentDiscountDeviations(DocumentHeader header)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            decimal detailTotalDefaultDocumentDocumentDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.DefaultDocumentDiscountAmount));
            decimal documentdefaultDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DefaultDocumentDiscountAmount);

            if (documentdefaultDiscountAmountRounded != detailTotalDefaultDocumentDocumentDiscountAmountRounded)
            {
                decimal deviation = PlatformRoundingHandler.RoundDisplayValue(detailTotalDefaultDocumentDocumentDiscountAmountRounded - documentdefaultDiscountAmountRounded);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

                if (documentType.IsForWholesale)
                {
                    this.WholeSalesFixDiscountDeviation(deviation, header, eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);
                }
                else
                {
                    this.RetailFixDiscountDeviation(deviation, header, eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);
                }
                header.DefaultDocumentDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.DefaultDocumentDiscountAmount));
            }
        }

        public void FixCustomerDiscountDeviations(DocumentHeader header)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            decimal detailTotalCustomertDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.CustomerDiscountAmount));
            decimal customerDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.CustomerDiscountAmount);

            if (customerDiscountAmountRounded != detailTotalCustomertDiscountAmountRounded)
            {
                decimal deviation = PlatformRoundingHandler.RoundDisplayValue(detailTotalCustomertDiscountAmountRounded - customerDiscountAmountRounded);

                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

                if (documentType.IsForWholesale)
                {
                    this.WholeSalesFixDiscountDeviation(deviation, header, eDiscountSource.CUSTOMER);
                }
                else
                {
                    this.RetailFixDiscountDeviation(deviation, header, eDiscountSource.CUSTOMER);
                }
                header.CustomerDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.CustomerDiscountAmount));
            }
        }

        protected void FixPointsDiscountDeviations(DocumentHeader header)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            decimal detailTotalPointsDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.PointsDiscountAmount));
            decimal documentPointsDiscountAmountRounded = PlatformRoundingHandler.RoundDisplayValue(header.PointsDiscountAmount);

            if (documentPointsDiscountAmountRounded != detailTotalPointsDiscountAmountRounded)
            {
                decimal deviation = PlatformRoundingHandler.RoundDisplayValue(detailTotalPointsDiscountAmountRounded - documentPointsDiscountAmountRounded);
                DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

                if (documentType.IsForWholesale)
                {
                    this.WholeSalesFixDiscountDeviation(deviation, header, eDiscountSource.POINTS);
                }
                else
                {
                    this.RetailFixDiscountDeviation(deviation, header, eDiscountSource.POINTS);
                }
                header.PointsDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.PointsDiscountAmount));
            }
        }

        public void FixFiscalPrinterDeviationsFromDiscounts(decimal fiscalPrinterVatGrossTotal, eMinistryVatCategoryCode vatCode, DocumentHeader header)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            DocumentType type = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

            IEnumerable<DocumentDetail> detailsOfVat = header.DocumentDetails.Where(detail => detail.ItemVatCategoryMinistryCode == vatCode && detail.IsCanceled == false);
            decimal ourVatGrossTotal = detailsOfVat.Sum(detail => detail.GrossTotal);
            if (fiscalPrinterVatGrossTotal != ourVatGrossTotal)
            {
                decimal deviation = fiscalPrinterVatGrossTotal - ourVatGrossTotal;
                int order = deviation > 0 ? -1 : 1;
                int deviationDecimals = (int)appSettings.DisplayDigits;
                decimal deviationStep = (decimal)(1.0 / Math.Pow(10, deviationDecimals));

                List<DocumentDetailDiscount> detailDiscounts = detailsOfVat.SelectMany(detail => detail.DocumentDetailDiscounts.Where(x => x.DiscountSource != eDiscountSource.CUSTOM &&
                                                                    x.DiscountSource != eDiscountSource.PROMOTION_LINE_DISCOUNT &&
                                                                    ((deviation > 0 && x.Value >= deviationStep) || deviation < 0))).ToList();
                int i = 0;
                while (deviation != 0 && detailDiscounts.Count > i)
                {
                    detailDiscounts[i].Value += (deviationStep * order);
                    deviation += deviationStep * order;
                    detailDiscounts[i].DiscountDeviation += (deviationStep * order);
                    CalculateDocumentDetailTotals(detailDiscounts[i].DocumentDetail, type.IsForWholesale, false);
                    if (detailDiscounts[i].Value == 0)
                    {
                        i++;
                    }
                }
            }
        }

        private void WholeSalesFixDiscountDeviation(decimal deviation, DocumentHeader header, eDiscountSource source)
        {
        }

        private void RetailFixDiscountDeviation(decimal deviation, DocumentHeader header, eDiscountSource source)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            DocumentType type = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

            int deviationStepDecimals = (int)appSettings.ComputeDigits;
            int maxDeviationDecimals = (int)appSettings.DisplayDigits;
            int order = deviation > 0 ? -1 : 1;
            decimal deviationStep = (decimal)(1.0 / Math.Pow(10, deviationStepDecimals));
            decimal maxDeviationPerDetail = (decimal)(1.0 / Math.Pow(10, maxDeviationDecimals));

            decimal gross;
            decimal[] grossVat = new decimal[5];
            gross = header.DocumentDetails.Where(x => x.IsCanceled == false).Sum(x => x.GrossTotal);
            grossVat[0] = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.A && x.IsCanceled == false).Sum(x => x.GrossTotal);
            grossVat[1] = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.B && x.IsCanceled == false).Sum(x => x.GrossTotal);
            grossVat[2] = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.C && x.IsCanceled == false).Sum(x => x.GrossTotal);
            grossVat[3] = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.D && x.IsCanceled == false).Sum(x => x.GrossTotal);
            grossVat[4] = header.DocumentDetails.Where(x => x.ItemVatCategoryMinistryCode == eMinistryVatCategoryCode.E && x.IsCanceled == false).Sum(x => x.GrossTotal);
            decimal[] deviationsPerVat = grossVat.Select(v => gross == 0 ? v : (v * deviation / gross)).ToArray();
            decimal[] roundedDeviationPerVat = deviationsPerVat.Select(value => PlatformRoundingHandler.RoundDisplayValue(value)).ToArray();
            decimal[] deviationDifferencesPerVat = deviationsPerVat.Select((deviationPerVat, index) => deviationPerVat - roundedDeviationPerVat[index]).ToArray();

            decimal deviationDifferences = deviationDifferencesPerVat.Sum();
            int deviationsCount = (int)(deviationDifferences / maxDeviationPerDetail);
            IEnumerable<KeyValuePair<int, decimal>> dictionary = deviationDifferencesPerVat.Select((deviationPerVat, index) => new KeyValuePair<int, decimal>(index, deviationPerVat));
            if (deviationDifferences > 0)
            {
                dictionary = dictionary.OrderByDescending(x => x.Value).Take(deviationsCount);
                dictionary.ToList().ForEach(x =>
                {
                    roundedDeviationPerVat[x.Key] += maxDeviationPerDetail;
                });
            }
            else if (deviationDifferences < 0)
            {
                dictionary = dictionary.OrderBy(x => x.Value).Take(-deviationsCount);
                dictionary.ToList().ForEach(x =>
                {
                    roundedDeviationPerVat[x.Key] -= maxDeviationPerDetail;
                });
            }

            for (int deviationIndex = 0; deviationIndex < roundedDeviationPerVat.Length; deviationIndex++)
            {
                eMinistryVatCategoryCode code = (eMinistryVatCategoryCode)(deviationIndex + 1);
                List<DocumentDetail> details = header.DiscountableDocumentDetails().Where(x => x.ItemVatCategoryMinistryCode == code).ToList();

                decimal currentLineDeviationFix = 0;
                int i = 0;
                while (((roundedDeviationPerVat[deviationIndex] > 0 && roundedDeviationPerVat[deviationIndex] >= deviationStep) ||
                    (roundedDeviationPerVat[deviationIndex] < 0 && roundedDeviationPerVat[deviationIndex] <= deviationStep)) && details.Count > i)
                {
                    DocumentDetailDiscount detailDocumentDiscount = details[i].DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == source);

                    if (detailDocumentDiscount == null)
                    {
                        detailDocumentDiscount = new DocumentDetailDiscount(details[i].Session);
                        detailDocumentDiscount.DiscountSource = source;
                        detailDocumentDiscount.DocumentDetail = details[i];
                    }

                    detailDocumentDiscount.Value += (deviationStep * order);
                    detailDocumentDiscount.DiscountWithVAT = detailDocumentDiscount.Value;
                    detailDocumentDiscount.DiscountWithoutVAT = detailDocumentDiscount.Value / (1 + detailDocumentDiscount.DocumentDetail.VatFactor);

                    details[i].Save();
                    roundedDeviationPerVat[deviationIndex] += (deviationStep * order);
                    currentLineDeviationFix += deviationStep;

                    if (currentLineDeviationFix >= maxDeviationPerDetail)
                    {
                        ////switch line
                        CalculateDocumentDetailTotals(details[i], type.IsForWholesale, false);
                        i++;
                        i = i % details.Count;
                        currentLineDeviationFix = 0;
                    }
                }
                if (details.Count > 0)
                {
                    CalculateDocumentDetailTotals(details[i], type.IsForWholesale, false);
                }
            }

            RecalculateDocumentCosts(header, false);
        }

        protected decimal GetTotalLoyatyDiscountAmount(DocumentHeader header, Customer customer, out decimal pointsToConsume)
        {
            decimal totalDiscountAmount = 0;
            pointsToConsume = 0;
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            if (appSettings.DiscountAmount > 0 && appSettings.RefundPoints > 0)
            {
                decimal grossTotal = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.POINTS);
                int loyaltyApplicationTimesByPoints = (int)(customer.CollectedPoints / appSettings.RefundPoints);
                int loyaltyApplicationTimesByAmount = (int)(grossTotal / appSettings.DiscountAmount);

                //Get the smallest of the two
                int loyaltyApplicationTimes = loyaltyApplicationTimesByPoints > loyaltyApplicationTimesByAmount ? loyaltyApplicationTimesByAmount : loyaltyApplicationTimesByPoints;

                totalDiscountAmount = loyaltyApplicationTimes * appSettings.DiscountAmount;
                pointsToConsume = loyaltyApplicationTimes * appSettings.RefundPoints;
            }

            return totalDiscountAmount;
        }

        /// <summary>
        /// Calculates and applies the PointsDiscountPercentagePerLine that must be applied to all the lines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="actionManager"></param>
        public void ApplyLoyalty(DocumentHeader header, IActionManager actionManager)
        {
            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            Customer customer = SessionManager.GetObjectByKey<Customer>(header.Customer);
            decimal customerRemainingPoints = customer.CollectedPoints - appSettings.RefundPoints;
            decimal grossTotalBeforePointsAndDocumentDiscount = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.POINTS);
            decimal validLinesGrossTotalBeforPointsAndDocumentDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.POINTS);
            decimal pointsToConsume = appSettings.RefundPoints;

            if (appSettings.LoyaltyRefundType == eLoyaltyRefundType.DISCOUNT)
            {
                if (appSettings.DiscountPercentage > 0)
                {
                    decimal percentage = appSettings.DiscountPercentage > 1 ? appSettings.DiscountPercentage / 100 : appSettings.DiscountPercentage;
                    decimal desiredDiscountAmount = grossTotalBeforePointsAndDocumentDiscount * percentage;
                    header.PointsDiscountPercentage = percentage;
                    header.PointsDiscountPercentagePerLine = desiredDiscountAmount / validLinesGrossTotalBeforPointsAndDocumentDiscount;
                    header.PointsDiscountAmount = desiredDiscountAmount;
                }
                else if (appSettings.DiscountAmount > 0)
                {
                    decimal totalDiscountAmount = GetTotalLoyatyDiscountAmount(header, customer, out pointsToConsume);
                    if (totalDiscountAmount > validLinesGrossTotalBeforPointsAndDocumentDiscount)
                    {
                        throw new Exception(POSClientResources.INVALID_DISCOUNT);
                    }

                    header.PointsDiscountPercentage = totalDiscountAmount / validLinesGrossTotalBeforPointsAndDocumentDiscount;
                    header.PointsDiscountPercentagePerLine = header.PointsDiscountPercentage;
                    header.PointsDiscountAmount = totalDiscountAmount;
                }
                else
                {
                    throw new Exception(POSClientResources.LOYALTY_DISCOUNT_NOT_DEFINED);
                }
            }
            else if (appSettings.LoyaltyRefundType == eLoyaltyRefundType.PAYMENT)
            {
                PaymentMethod method = SessionManager.GetObjectByKey<PaymentMethod>(appSettings.LoyaltyPaymentMethod);
                if (method == null)
                {
                    throw new Exception(POSClientResources.PAYMENT_METHOD_NOT_FOUND);
                }

                if (appSettings.DiscountAmount <= 0)
                {
                    throw new Exception(POSClientResources.LOYALTY_DISCOUNT_NOT_DEFINED);
                }

                decimal totalDiscountAmount = GetTotalLoyatyDiscountAmount(header, customer, out pointsToConsume);

                if (totalDiscountAmount > validLinesGrossTotalBeforPointsAndDocumentDiscount)
                {
                    throw new Exception(POSClientResources.INVALID_DISCOUNT);
                }

                actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(method, totalDiscountAmount), validateMachineStatus: false);
            }

            RecalculateDocumentCosts(header, true);
            FixPointsDiscountDeviations(header);
            header.ConsumedPointsForDiscount = pointsToConsume;
        }

        public void ClearAppliedLoyalty(DocumentHeader header)
        {
            if (header.ConsumedPointsForDiscount > 0)
            {
                header.PointsDiscountPercentage = 0;
                header.PointsDiscountPercentagePerLine = 0;
                header.PointsDiscountAmount = 0;
                header.ConsumedPointsForDiscount = 0;

                foreach (DocumentDetail detail in header.DocumentDetails)
                {
                    List<DocumentDetailDiscount> discountsToDelete = detail.DocumentDetailDiscounts.Where(disc => disc.DiscountSource == eDiscountSource.POINTS).ToList();
                    if (discountsToDelete.Count > 0)
                    {
                        header.Session.Delete(discountsToDelete);
                        RecalculateDocumentDetail(detail, header);
                    }
                }

                RecalculateDocumentCosts(header, false);
            }
        }

        public bool CheckIfShouldOpenDrawer(DocumentHeader header)
        {
            foreach (DocumentPayment payment in header.DocumentPayments)
            {
                PaymentMethod pm = SessionManager.GetObjectByKey<PaymentMethod>(payment.PaymentMethod);
                if (pm != null && pm.OpensDrawer)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckIfShouldGiveChange(DocumentHeader header)
        {
            foreach (DocumentPayment payment in header.DocumentPayments)
            {
                PaymentMethod pm = SessionManager.GetObjectByKey<PaymentMethod>(payment.PaymentMethod);
                if (pm != null && pm.GiveChange)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Σε κάθε παραστατικό που εκδίδεται είναι απαραίτητη και η ύπαρξη μιας γραμμής που περιέχει τα οικονομικά στοιχεία που πρέπει να διαβιβαστούν στην βάση δεδομένων της ΓΓΠΣ.
        ///Αυτά  έχουν περιγραφεί από την ΠΟΛ 1221 και είναι τα κάτωθι:
        ///α/α Πεδίου   Περιεχόμενο             Μήκος (χαρακτήρες)
        ///0        ΑΦΜ Εκδότη                              12
        ///1        ΑΦΜ Παραλήπτη                           12
        ///2        Αριθμός Κάρτας Αποδείξεων Πελάτη  *1    19
        ///3        Ημερομηνία και Ώρα                *4    12
        ///4        Περιγραφή Παραστατικού            *2 (Μεταβλητό)
        ///5        Σειρά Θεώρησης                          10
        ///6        Αριθμός Παραστατικού                    10
        ///7        Καθαρό Ποσό Α                           18:2
        ///8        Καθαρό Ποσό Β                           18:2
        ///9        Καθαρό Ποσό Γ                           18:2
        ///10       Καθαρό Ποσό Δ                           18:2
        ///11       Καθαρό Ποσό Ε                           18:2
        ///12       ΦΠΑ Α                                   18:2
        ///13       ΦΠΑ Β                                   18:2
        ///14       ΦΠΑ Γ                                   18:2
        ///15       ΦΠΑ Δ                                   18:2
        ///16       Γενικό Σύνολο Παρ/κού                   18:2
        ///17       Κωδικός νομίσματος
        ///
        ///*1    ΑΦΜ ή o αριθμό της πιστωτικής κάρτας του πελάτη αν δεν έχει κάρτα αποδείξεων.
        ///*2   Αν είναι γνωστός ο κωδικός του παραστατικού βάση της τυποποίησης των παραστατικών του taxis, τότε μπορεί να χρησιμοποιηθεί απ’ ευθείας στο πεδίο αυτό.
        ///Διαφορετικά μπορεί να χρησιμοποιηθεί η περιγραφή του παραστατικού. Κατόπιν χρειάζεται να προγραμματίσουμε στον driver την αντιστοίχηση της περιγραφής του πεδίου αυτού με τον σωστό κωδικό Taxis.
        ///*4 Η μορφοποίηση της ημερομηνίας είναι : YYYYMMDDHHmm
        ///
        /// YYYY = έτος   ΜΜ = Μήνας  DD = Μέρα  HH = Ώρα  mm = Λεπτά
        /// EXAMPLE: 999999999/123456789/1234567890123456789/020420131200/173/A/1001/3.00/3.00/3.00/3.00/3.00/0.19/0.39/0.69/1.08/14.35/1
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public string CreateFiscalInfoLine(DocumentHeader header, Logger logger, string customerReceiptCard = null, int currencyCode = 0, string seperator = "/")
        {
            if (header == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'DocumentHeader is null'");
            }

            Store store = SessionManager.GetObjectByKey<Store>(header.Store);
            if (store == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'Store of Document is null. (" + header.Store + ")'");
            }

            Guid mainOwner = store.ReferenceCompany != null && store.ReferenceCompany != Guid.Empty ? store.ReferenceCompany : store.Owner;
            CompanyNew company = SessionManager.GetObjectByKey<CompanyNew>(mainOwner);
            if (company == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'Company of store (" + store.Code + ") '" + store.Oid + "' is null. Trader Oid '" + mainOwner + "''");
            }

            Trader companyTrader = SessionManager.GetObjectByKey<Trader>(company.Trader);
            if (companyTrader == null)
            {
                logger.Error("Failure Creating Fiscal Info Line: 'Trader of company (" + company.Code + ") '" + company.Oid + "' is null. Trader Oid '" + company.Trader + "''");
            }
            string ownerTaxCode = companyTrader == null ? "999999999" : companyTrader.TaxCode;

            Customer customer = SessionManager.GetObjectByKey<Customer>(header.Customer);
            if (companyTrader == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'Customer of Document is null(" + header.Customer + ")'");
            }

            Trader customerTrader = SessionManager.GetObjectByKey<Trader>(customer.Trader);
            if (customerTrader == null)
            {
                logger.Error("Failure Creating Fiscal Info Line: 'Trader of customer (" + customer.Code + ") '" + customer.Oid + "' is null. Trader Oid '" + customer.Trader + "''");
            }

            string customerTaxCode = customerTrader == null ? "999999999" : customerTrader.TaxCode;

            DocumentType docType = SessionManager.GetObjectByKey<DocumentType>(header.DocumentType);

            if (docType == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'DocumentType of Document is null (" + header.DocumentType + ")'");
            }

            MinistryDocumentType ministryDocType = SessionManager.GetObjectByKey<MinistryDocumentType>(docType.MinistryDocumentType);

            if (ministryDocType == null)
            {
                throw new POSException("Document Type '" + (docType.Code ?? "") + " - " + (docType.Description ?? "") + "' has no Ministry document type assigned to it");
            }

            DocumentSeries series = SessionManager.GetObjectByKey<DocumentSeries>(header.DocumentSeries);

            if (series == null)
            {
                throw new POSException("Failure Creating Fiscal Info Line: 'Series of Document is null (" + header.DocumentSeries + ")'");
            }

            string seriesDescription = series.Description;

            decimal vatAmount1 = 0;
            decimal vatAmount2 = 0;
            decimal vatAmount3 = 0;
            decimal vatAmount4 = 0;
            decimal vatAmount5 = 0;

            decimal netAmount1 = 0;
            decimal netAmount2 = 0;
            decimal netAmount3 = 0;
            decimal netAmount4 = 0;
            decimal netAmount5 = 0;

            decimal vat1GrossTotal = 0;
            decimal vat2GrossTotal = 0;
            decimal vat3GrossTotal = 0;
            decimal vat4GrossTotal = 0;
            decimal vat5GrossTotal = 0;

            decimal vat1Factor = 0;
            decimal vat2Factor = 0;
            decimal vat3Factor = 0;
            decimal vat4Factor = 0;
            decimal vat5Factor = 0;

            foreach (DocumentDetail detail in header.DocumentDetails.Where(x => x.IsCanceled == false))
            {
                VatFactor vatFactor = SessionManager.GetObjectByKey<VatFactor>(detail.VatFactorGuid);
                if (vatFactor == null)
                    throw new POSException("Document detail has no Vat Factor");
                VatCategory vatCategory = SessionManager.GetObjectByKey<VatCategory>(vatFactor.VatCategory);
                if (vatCategory == null)
                    throw new POSException("Vat factor '" + vatFactor.Code ?? "" + " - " + vatFactor.Description ?? "" + "' has no Vat Category");
                switch (vatCategory.MinistryVatCategoryCode)
                {
                    case eMinistryVatCategoryCode.A:
                        vat1GrossTotal += detail.GrossTotal;
                        if (vat1Factor != vatFactor.Factor)
                        {
                            vat1Factor = vatFactor.Factor;
                        }
                        //vatAmount1 += detail.TotalVatAmount;
                        //netAmount1 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.B:
                        vat2GrossTotal += detail.GrossTotal;
                        if (vat2Factor != vatFactor.Factor)
                        {
                            vat2Factor = vatFactor.Factor;
                        }
                        //vatAmount2 += detail.TotalVatAmount;
                        //netAmount2 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.C:
                        vat3GrossTotal += detail.GrossTotal;
                        if (vat3Factor != vatFactor.Factor)
                        {
                            vat3Factor = vatFactor.Factor;
                        }
                        //vatAmount3 += detail.TotalVatAmount;
                        //netAmount3 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.D:
                        vat4GrossTotal += detail.GrossTotal;
                        if (vat4Factor != vatFactor.Factor)
                        {
                            vat4Factor = vatFactor.Factor;
                        }
                        //vatAmount4 += detail.TotalVatAmount;
                        //netAmount4 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.E:
                        vat5GrossTotal += detail.GrossTotal;
                        if (vat5Factor != vatFactor.Factor)
                        {
                            vat5Factor = vatFactor.Factor;
                        }
                        //netAmount5 += detail.NetTotal;
                        break;

                    case eMinistryVatCategoryCode.NONE:
                        throw new POSException("Vat Category '" + vatCategory.Code ?? "" + " - " + vatCategory.Description ?? "" + "' has no Ministry Vat Category Code assigned to it.");
                }
            }

            OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
            netAmount1 = PlatformRoundingHandler.RoundDisplayValue((vat1GrossTotal) / (1 + vat1Factor));// vat1GrossTotal - vatAmount1;
            netAmount2 = PlatformRoundingHandler.RoundDisplayValue((vat2GrossTotal) / (1 + vat2Factor));// vat2GrossTotal - vatAmount2;
            netAmount3 = PlatformRoundingHandler.RoundDisplayValue((vat3GrossTotal) / (1 + vat3Factor));// vat3GrossTotal - vatAmount3;
            netAmount4 = PlatformRoundingHandler.RoundDisplayValue((vat4GrossTotal) / (1 + vat4Factor));// vat4GrossTotal - vatAmount4;
            netAmount5 = PlatformRoundingHandler.RoundDisplayValue((vat5GrossTotal) / (1 + vat5Factor));// vat5GrossTotal - vatAmount5;

            vatAmount1 = PlatformRoundingHandler.RoundDisplayValue(vat1GrossTotal - netAmount1);
            vatAmount2 = PlatformRoundingHandler.RoundDisplayValue(vat2GrossTotal - netAmount2);
            vatAmount3 = PlatformRoundingHandler.RoundDisplayValue(vat3GrossTotal - netAmount3);
            vatAmount4 = PlatformRoundingHandler.RoundDisplayValue(vat4GrossTotal - netAmount4);
            vatAmount5 = PlatformRoundingHandler.RoundDisplayValue(vat5GrossTotal - netAmount5);

            NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." };

            string line = String.Format(nfi, "[<]{0}" + seperator + "{1}" + seperator + "{2}" + seperator + "{3:yyyyMMddHHmm}" + seperator + "{4}" + seperator + "{5}" + seperator +
                "{6}" + seperator + "{7:0.00}" + seperator + "{8:0.00}" + seperator + "{9:0.00}" + seperator + "{10:0.00}" + seperator + "{11:0.00}" + seperator + "{12:0.00}" + seperator + "{13:0.00}"
                + seperator + "{14:0.00}" + seperator + "{15:0.00}" + seperator + "{16:0.00}" + seperator + "{17}[>]",
                String.Concat(ownerTaxCode.Take(12)),
                String.Concat(customerTaxCode.Take(12)),
                customerReceiptCard == null ? String.Concat(customerTaxCode.Take(19)) : String.Concat(customerReceiptCard.Take(19)),
                header.FinalizedDate,
                ministryDocType.Code,
                String.Concat(seriesDescription.Take(10)),
                header.DocumentNumber, netAmount1, netAmount2,
                netAmount3, netAmount4, netAmount5,
                vatAmount1,
                vatAmount2,
                vatAmount3,
                vatAmount4,
                header.GrossTotal,
                currencyCode   //Euro is 0
                );
            return line;
        }

        public void CreateTransactionCoupon(DocumentHeader header, CouponViewModel couponViewModel, DocumentDetailDiscount documentDetailDiscount = null, DocumentPayment documentPayment = null)
        {
            if (couponViewModel == null)
            {
                return;
            }

            if (documentDetailDiscount == null && documentPayment == null)
            {
                //throw new POSException(Resources.POSClientResources.PLEASE_DEFINE_HOW_COUPON_SHOULD_BE_APPLIED);
            }

            TransactionCoupon transactionCoupon = new TransactionCoupon(header.Session);
            transactionCoupon.Coupon = couponViewModel.Oid;
            transactionCoupon.DocumentHeader = header;
            transactionCoupon.DocumentDetailDiscount = documentDetailDiscount;
            transactionCoupon.DocumentPayment = documentPayment;
            transactionCoupon.IsCanceled = false;
            transactionCoupon.CouponCode = couponViewModel.Code;
            transactionCoupon.CouponMask = couponViewModel.CouponMaskOid;

            transactionCoupon.Save();
            transactionCoupon.Session.CommitTransaction();
        }

        public void ClearDocumentTotalDiscounts(DocumentHeader header)
        {
            if (header.DocumentDiscountAmount != 0 || header.DefaultDocumentDiscountAmount != 0 || header.CustomerDiscountAmount != 0)
            {
                foreach (IDocumentDetail detail in header.DocumentDetails)
                {
                    List<IDocumentDetailDiscount> discountsToDelete = detail.DocumentDetailDiscounts
                                                                            .Where(disc => disc.DiscountSource == eDiscountSource.DOCUMENT
                                                                                || disc.DiscountSource == eDiscountSource.CUSTOMER
                                                                                || disc.DiscountSource == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT
                                                                                || disc.DiscountSource == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT).ToList();
                    if (discountsToDelete.Count > 0)
                    {
                        header.Session.Delete(discountsToDelete);
                        this.RecalculateDocumentDetail(detail, header);
                    }
                }

                this.RecalculateDocumentCosts(header, false);
                header.Session.CommitTransaction();
            }
            header.DefaultDocumentDiscountAmount = 0;
            header.DefaultDocumentDiscountPercentagePerLine = 0;
            header.CustomerDiscountAmount = 0;
            header.CustomerDiscountPercentagePerLine = 0;
            header.DocumentDiscountAmount = 0;
            header.DocumentDiscountPercentage = 0;
            header.DocumentDiscountPercentage = 0;
        }

        public bool CheckIfDocumentIsValidForMovingToPayment(DocumentHeader documentHeader, out string message)
        {
            message = "";
            //Positive Gross required
            if (documentHeader.GrossTotal < 0)
            {
                message = POSClientResources.NEGATIVE_TOTAL_NOT_PERMITED;
                return false;
            }

            //At least one non canceled line required
            if (documentHeader.DocumentDetails.Where(x => x.IsCanceled == false).Count() == 0)
            {
                message = POSClientResources.NO_ITEMS_ADDED;
                return false;
            }

            // Check if customer is valid
            if (documentHeader.CustomerViewModel == null || !documentHeader.CustomerViewModel.IsNew)
            {
                if (CheckIfCustomerIsValidForDocumentType(documentHeader.DocumentType, documentHeader.Customer) == false)
                {
                    message = POSClientResources.CUSTOMER_NOT_ALLOWED_TO_THIS_DOCUMENT_TYPE;
                    return false;
                }
            }
            List<DocumentDetail> invalid;
            if (CheckIfDocumentDetailsAreValidForDocumentType(documentHeader, out invalid) == false)
            {
                message = "INVALID ITEMS";
                return false;
            }

            return true;
        }

        public bool CheckIfDocumentDetailsAreValidForDocumentType(DocumentHeader documentHeader, out List<DocumentDetail> invalid)
        {
            DocumentType type = SessionManager.GetObjectByKey<DocumentType>(documentHeader.DocumentType);
            // invalid = new List<DocumentDetail>();
            foreach (DocumentDetail detail in documentHeader.DocumentDetails)
            {
                Item item = SessionManager.GetObjectByKey<Item>(detail.Item);
                detail.IsInvalid = ItemService.CheckIfItemIsValidForDocumentType(item, type) == false;
            }
            invalid = documentHeader.DocumentDetails.Where(x => x.IsInvalid).ToList();
            return invalid.Count == 0;
        }

        public bool CheckIfCustomerIsValidForDocumentType(Guid documentType, Guid customer)
        {
            DocumentType docType = this.SessionManager.GetObjectByKey<DocumentType>(documentType);
            if (docType.DocTypeCustomerCategoryMode != eDocTypeCustomerCategory.NONE)
            {
                XPCollection<CustomerAnalyticTree> custAnalyticTrees = new XPCollection<CustomerAnalyticTree>(this.SessionManager.GetSession<CustomerAnalyticTree>(),
                    new BinaryOperator("Object", customer));
                List<Guid> customerCategoriesOids = custAnalyticTrees.Select(x => x.Node).ToList();

                XPCollection<CustomerCategory> customerCategories = new XPCollection<CustomerCategory>(this.SessionManager.GetSession<CustomerCategory>(),
                    new InOperator("Oid", customerCategoriesOids));
                foreach (CustomerCategory customerCategory in customerCategories)
                {
                    CustomerCategory currentLevel = customerCategory;
                    do
                    {
                        if (customerCategoriesOids.Contains(currentLevel.Oid) == false)
                        {
                            customerCategoriesOids.Add(currentLevel.Oid);
                        }
                        currentLevel = currentLevel.Session.GetObjectByKey<CustomerCategory>(currentLevel.ParentOid);
                    } while (currentLevel != null && currentLevel.ParentOid != null && currentLevel.ParentOid != Guid.Empty);
                }

                XPCollection<DocTypeCustomerCategory> documentTypeCustomerCategories = new XPCollection<DocTypeCustomerCategory>(this.SessionManager.GetSession<DocTypeCustomerCategory>(),
                    new BinaryOperator("DocumentType", docType.Oid));
                List<Guid> documentTypeCustomerCategoriesOids = documentTypeCustomerCategories.Select(x => x.CustomerCategory).ToList();

                IEnumerable<Guid> intersection = documentTypeCustomerCategoriesOids.Intersect(customerCategoriesOids);
                int intersectCount = intersection.Count();
                if (intersectCount == 0 && docType.DocTypeCustomerCategoryMode == eDocTypeCustomerCategory.INCLUDE_CUSTOMER_CATEGORIES)
                {
                    return false;
                }
                if (intersectCount != 0 && docType.DocTypeCustomerCategoryMode == eDocTypeCustomerCategory.EXCLUDE_CUSTOMER_CATEGORIES)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the selected PriceCatalog contains all items of the document
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="priceCatalog">The new price catalog selected</param>
        /// <returns>The items not included, if any, otherwise null</returns>
        public List<DocumentDetail> PriceCatalogNotIncludedItems(IConfigurationManager configManager, ISessionManager sessionManager, DocumentHeader document, Guid priceCatalogOid)
        {
            List<DocumentDetail> notIncludedItemsOids = new List<DocumentDetail>();
            if (document.DocumentDetails != null)
            {
                foreach (DocumentDetail detail in document.DocumentDetails)
                {
                    PriceCatalogDetail priceCatalogDetail;
                    priceCatalogDetail = GetPriceCatalogDetail(configManager, sessionManager, detail.Item, priceCatalogOid, detail.Barcode);
                    if (priceCatalogDetail == null)
                    {
                        notIncludedItemsOids.Add(detail);
                    }
                }
            }
            return notIncludedItemsOids;
        }

        public PriceCatalogDetail GetPriceCatalogDetail(IConfigurationManager configManager, ISessionManager sessionManager, Guid itemOid, Guid priceCatalogOid, Guid barcodeOid)
        {
            DateTime now = DateTime.Now;
            PriceCatalogDetail pcdt = null;
            PriceCatalog priceCatalog = sessionManager.GetObjectByKey<PriceCatalog>(priceCatalogOid);

            if (barcodeOid != Guid.Empty)
            {
                pcdt = sessionManager.FindObject<PriceCatalogDetail>(CriteriaOperator.And(
                                                                    new BinaryOperator("PriceCatalog", priceCatalogOid),
                                                                    new JoinOperand("PriceCatalog",
                                                                                    CriteriaOperator.And(
                                                                                        new OperandProperty("^.PriceCatalog") == new OperandProperty("Oid"),
                                                                                        new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual),
                                                                                        new BinaryOperator("StartDate", now, BinaryOperatorType.Less))),
                                                                    new BinaryOperator("Item", itemOid),
                                                                    new BinaryOperator("Barcode", barcodeOid),
                                                                    new BinaryOperator("IsActive", true)));
            }

            if (pcdt == null)
            {
                Item item = sessionManager.GetObjectByKey<Item>(itemOid);
                OwnerApplicationSettings ownAppSet = configManager.GetAppSettings();
                string Code1 = (ownAppSet.PadBarcodes) ? item.Code.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : item.Code;
                Barcode fkOid = sessionManager.FindObject<Barcode>(new BinaryOperator("Code", Code1));  // Do Padding
                if (fkOid == null)
                {
                    return null;
                }
                pcdt = sessionManager.FindObject<PriceCatalogDetail>(CriteriaOperator.And(
                                                                    new BinaryOperator("PriceCatalog", priceCatalogOid),
                                                                    new JoinOperand("PriceCatalog",
                                                                                    CriteriaOperator.And(
                                                                                        new OperandProperty("^.PriceCatalog") == new OperandProperty("Oid"),
                                                                                        new BinaryOperator("EndDate", now, BinaryOperatorType.GreaterOrEqual),
                                                                                        new BinaryOperator("StartDate", now, BinaryOperatorType.Less))),
                                                                    new BinaryOperator("Item", itemOid),
                                                                    new BinaryOperator("Barcode", fkOid.Oid),
                                                                    new BinaryOperator("IsActive", true)));
                if (pcdt == null)
                {
                    Guid parentCatalogOid = priceCatalog.ParentCatalogOid ?? Guid.Empty;
                    if (!priceCatalog.IsRoot)
                    {
                        return GetPriceCatalogDetail(configManager, sessionManager, itemOid, parentCatalogOid, barcodeOid);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return pcdt;
        }
        public decimal GetDefaultDocumentDiscountPercentage(DocumentHeader document)
        {
            try
            {
                XPCollection<StoreDocumentSeriesType> storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>();
                DocumentType dType = SessionManager.GetSession<DocumentType>().FindObject<DocumentType>(new BinaryOperator("Oid", document.DocumentType));

                if (dType.IsPrintedOnStoreController)
                {
                    storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>(SessionManager.GetSession<StoreDocumentSeriesType>(),
                                                                                         new BinaryOperator("DocumentType", dType.Oid));
                }
                else
                {
                    XPCollection<DocumentSeries> series = new XPCollection<DocumentSeries>(SessionManager.GetSession<DocumentSeries>(),
                        CriteriaOperator.And(new BinaryOperator("POS", Configuration.CurrentTerminalOid)));

                    storeDocumentSeriesTypes = new XPCollection<StoreDocumentSeriesType>(SessionManager.GetSession<StoreDocumentSeriesType>(),
                        CriteriaOperator.And(new BinaryOperator("DocumentType", document.DocumentType), new InOperator("DocumentSeries", series.Select(x => x.Oid))));
                }

                return storeDocumentSeriesTypes.FirstOrDefault().DefaultDiscountPercentage;

            }
            catch (Exception ex) { return 0.0m; }

        }

        public void ApplyDefaultDocumentDiscount(DocumentHeader header, decimal discountPercentage)
        {
            try
            {
                if (discountPercentage > 0)
                {
                    OwnerApplicationSettings appSettings = Configuration.GetAppSettings();

                    decimal validLinesGrossTotalBeforeDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);
                    decimal grossTotalBeforeCustomDocumentDiscount = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT);
                    decimal desiredDiscountAmount = 0;
                    if (validLinesGrossTotalBeforeDiscount > 0)
                    {
                        desiredDiscountAmount = validLinesGrossTotalBeforeDiscount * discountPercentage;

                        header.DefaultDocumentDiscountPercentagePerLine = desiredDiscountAmount / validLinesGrossTotalBeforeDiscount;
                    }
                    header.DefaultDocumentDiscountAmount = desiredDiscountAmount;
                }
            }
            catch (Exception ex) { }
        }

        public void ApplyCustomerDiscount(DocumentHeader header, decimal discountPercentage)
        {
            try
            {
                if (discountPercentage > 0)
                {
                    OwnerApplicationSettings appSettings = Configuration.GetAppSettings();
                    decimal validLinesGrossTotalBeforeDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.CUSTOMER);
                    decimal grossTotalBeforeCustomDocumentDiscount = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.CUSTOMER);
                    decimal desiredDiscountAmount = 0;
                    if (validLinesGrossTotalBeforeDiscount > 0)
                    {
                        desiredDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(validLinesGrossTotalBeforeDiscount * discountPercentage);

                        header.CustomerDiscountPercentagePerLine = desiredDiscountAmount / validLinesGrossTotalBeforeDiscount;
                    }
                    header.CustomerDiscountAmount = desiredDiscountAmount;
                }
            }
            catch (Exception ex) { }
        }

        public decimal GetCustomerDiscountPercentage(Customer customer)
        {
            return (decimal)customer.Discount;
        }

        public void ApplyDocumentTotalDiscounts(DocumentHeader header)
        {
            if (header.Division == eDivision.Sales)
            {
                decimal discountPercentage = GetDefaultDocumentDiscountPercentage(header);
                decimal customerDiscount = 0;
                Customer customer = SessionManager.GetObjectByKey<Customer>(header.Customer);

                if (discountPercentage > 0)
                    ApplyDefaultDocumentDiscount(header, discountPercentage);

                if (header.Customer != Configuration?.DefaultCustomerOid && customer != null)
                    customerDiscount = GetCustomerDiscountPercentage(customer);

                if (customer != null && customerDiscount > 0)
                    ApplyCustomerDiscount(header, customerDiscount);

                if (customerDiscount > 0 || discountPercentage > 0)
                    RecalculateDocumentCosts(header, true);
            }
        }

        public POSDocumentReportSettings GetReportForDocumentType(Guid docType, IConfigurationManager configManager)
        {
            POSDocumentReportSettings posReport = configManager.DocumentReports
                                    .Where(x => x.DocumentTypeOid == docType && ((x.CustomReportOid != Guid.Empty && x.CustomReportOid != null) ||
                                                    (x.XMLPrintFormat != null))).FirstOrDefault();

            return posReport;
        }

        public XPCollection<DocumentType> GetAllValidPosDocumentTypes(IConfigurationManager config, ISessionManager sessionManager)
        {

            XPCollection<DocumentType> allDocTypes = new XPCollection<DocumentType>(sessionManager.GetSession<DocumentType>());


            XPCollection<DocumentSeries> series = new XPCollection<DocumentSeries>(sessionManager.GetSession<DocumentSeries>(),
                                                        CriteriaOperator.And(new BinaryOperator("POS", config.CurrentTerminalOid)));

            List<Guid> docTypes = (List<Guid>)new XPCollection<StoreDocumentSeriesType>(sessionManager.GetSession<StoreDocumentSeriesType>(),
                                                              CriteriaOperator.And(
                                                              new InOperator("DocumentType", allDocTypes.Select(x => x.Oid)),
                                                              new InOperator("DocumentSeries", series.Select(x => x.Oid))
                                                              )).Select(z => z.DocumentType).ToList();

            XPCollection<DocumentType> validDocTypes = this.GetDefaultPosDocumentTypes(config, sessionManager);

            foreach (DocumentType doctype in allDocTypes)
            {
                if (docTypes.Contains(doctype.Oid))
                {
                    validDocTypes.Add(doctype);
                }
            }

            return validDocTypes;
        }


        public XPCollection<DocumentType> GetDefaultPosDocumentTypes(IConfigurationManager configurationManager, ISessionManager sessionManager)
        {

            List<Guid> standardPOSDocumentTypes = new List<Guid>()
                {
                    configurationManager.DefaultDocumentTypeOid,
                    configurationManager.ProFormaInvoiceDocumentTypeOid,
                    configurationManager.SpecialProformaDocumentTypeOid
                };
            CriteriaOperator documentTypeCriteria = CriteriaOperator.Or(new InOperator("Oid", standardPOSDocumentTypes),
                                                                          new BinaryOperator("IsPrintedOnStoreController", true)
                                                                        );

            XPCollection<DocumentType> documentTypes = new XPCollection<DocumentType>(sessionManager.GetSession<DocumentType>(), documentTypeCriteria);

            return documentTypes;
        }

    }
}
