using DevExpress.Data.Filtering;
using DevExpress.Pdf;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraReports.UI;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Common;
using ITS.Retail.Platform;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ITS.POS.Client.Reports
{
    public class DocumentHeaderPrinter
    {
        private XtraReportBaseExtension GetReport(byte[] reportFile, Retail.Model.DocumentHeader documentHeader, Guid currentOwner, Guid currentUser)
        {

            Type reportType = XtraReportBaseExtension.GetReportTypeFromFile(reportFile);
            XtraReportBaseExtension report = Activator.CreateInstance(reportType) as XtraReportBaseExtension;
            report.DataSource = new List<Retail.Model.DocumentHeader>() { documentHeader };

            report.CurrentOwnerOid = currentOwner;
            report.CurrentUserOid = currentUser;
            report.LoadEncrypted(reportFile);
            report.CurrentDuplicate = 1;
            report.Duplicates = 2;
            report.SetOLAPConnectionString(String.Empty);
            report.ShowPrintMarginsWarning = false;
            //(report as SingleObjectXtraReport).ObjectOid = documentHeader.Oid;
            (report as SingleObjectXtraReport).Object = new List<Retail.Model.DocumentHeader>() { documentHeader };

            return report;
        }

        private void FillBasicObjData(IFormManager formManager, ISessionManager sessionManager, Model.Transactions.DocumentHeader POSDocumentHeader, Retail.Model.DocumentHeader documentHeader, JObject jSONDocumentHeader, out string errorMessage)
        {
            errorMessage = String.Empty;
            MethodInfo methodInfo = sessionManager.GetType().GetMethod("GetObjectByKey", new Type[] { typeof(Guid?) });

            documentHeader.Store = new Retail.Model.Store(documentHeader.Session);
            string error;
            try
            {

                documentHeader.Store.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Store>(POSDocumentHeader.Store).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.ReferenceCompany = new Retail.Model.CompanyNew(documentHeader.Session);
                documentHeader.ReferenceCompany.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.CompanyNew>(POSDocumentHeader.ReferenceCompany).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.MainCompany = new Retail.Model.CompanyNew(documentHeader.Session);
                documentHeader.MainCompany.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.CompanyNew>(POSDocumentHeader.MainCompany).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.POS = new Retail.Model.POS(documentHeader.Session);
                documentHeader.POS.FromJson(JObject.Parse(sessionManager.GetObjectByKey<POS.Model.Settings.POS>(POSDocumentHeader.POS).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.DocumentType = new Retail.Model.DocumentType(documentHeader.Session);
                documentHeader.DocumentType.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.DocumentType>(POSDocumentHeader.DocumentType).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.DocumentSeries = new Retail.Model.DocumentSeries(documentHeader.Session);
                documentHeader.DocumentSeries.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.DocumentSeries>(POSDocumentHeader.DocumentSeries).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.Customer = new Retail.Model.Customer(documentHeader.Session);
                Model.Master.Customer posCustomer = sessionManager.GetObjectByKey<Model.Master.Customer>(POSDocumentHeader.Customer);
                documentHeader.Customer.FromJson(JObject.Parse(posCustomer.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.Customer.Trader = new Retail.Model.Trader(documentHeader.Session);
                documentHeader.Customer.Trader.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Trader>(posCustomer.Trader).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );

                documentHeader.Customer.DefaultAddress = new Retail.Model.Address(documentHeader.Session);
                documentHeader.Customer.DefaultAddress.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Address>(posCustomer.DefaultAddress).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );



                documentHeader.Status = new Retail.Model.DocumentStatus(documentHeader.Session);
                documentHeader.Status.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.DocumentStatus>(POSDocumentHeader.Status).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );


                documentHeader.PriceCatalogPolicy = new Retail.Model.PriceCatalogPolicy(documentHeader.Session);
                documentHeader.PriceCatalogPolicy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.PriceCatalogPolicy>(POSDocumentHeader.PriceCatalogPolicy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );


                documentHeader.UserDailyTotals = new Retail.Model.UserDailyTotals(documentHeader.Session);
                documentHeader.UserDailyTotals.FromJson(JObject.Parse(POSDocumentHeader.UserDailyTotals.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );


                documentHeader.CreatedBy = new Retail.Model.User(documentHeader.Session);
                documentHeader.CreatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(POSDocumentHeader.CreatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                              PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                              copyOid: true,
                                              errorOnOidNotFound: false,
                                              error: out error
                                              );
                if (POSDocumentHeader.UpdatedBy != Guid.Empty)
                {
                    documentHeader.UpdatedBy = new Retail.Model.User(documentHeader.Session);
                    documentHeader.UpdatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(POSDocumentHeader.UpdatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                if (POSDocumentHeader.BillingAddress != Guid.Empty)
                {
                    documentHeader.BillingAddress = new Retail.Model.Address(documentHeader.Session);
                    documentHeader.BillingAddress.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Address>(POSDocumentHeader.BillingAddress).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                if (POSDocumentHeader.DeliveryTo != Guid.Empty)
                {
                    documentHeader.DeliveryTo = new Retail.Model.Trader(documentHeader.Session);
                    documentHeader.DeliveryTo.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Trader>(POSDocumentHeader.DeliveryTo).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                if (POSDocumentHeader.DocumentDiscountType != Guid.Empty)
                {
                    documentHeader.DocumentDiscountType = new Retail.Model.DiscountType(documentHeader.Session);
                    documentHeader.DocumentDiscountType.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.DiscountType>(POSDocumentHeader.DocumentDiscountType).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                if (POSDocumentHeader.PromotionsDocumentDiscountType != Guid.Empty)
                {
                    documentHeader.PromotionsDocumentDiscountType = new Retail.Model.DiscountType(documentHeader.Session);
                    documentHeader.PromotionsDocumentDiscountType.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.DiscountType>(POSDocumentHeader.PromotionsDocumentDiscountType).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                if (POSDocumentHeader.PriceCatalog != Guid.Empty)
                {
                    documentHeader.PriceCatalog = new Retail.Model.PriceCatalog(documentHeader.Session);
                    documentHeader.PriceCatalog.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.PriceCatalog>(POSDocumentHeader.PriceCatalog).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                  PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                  copyOid: true,
                                                  errorOnOidNotFound: false,
                                                  error: out error
                                                  );
                }

                foreach (Model.Transactions.DocumentDetail posDocumentDetail in POSDocumentHeader.DocumentDetails)
                {
                    Retail.Model.DocumentDetail documentDetail = documentHeader.DocumentDetails.Where(det => det.Oid == posDocumentDetail.Oid).First();

                    if (posDocumentDetail.SpecialItem != Guid.Empty)
                    {
                        documentDetail.SpecialItem = new Retail.Model.SpecialItem(documentHeader.Session);
                        documentDetail.SpecialItem.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.SpecialItem>(posDocumentDetail.SpecialItem).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.Item != Guid.Empty)
                    {
                        documentDetail.Item = new Retail.Model.Item(documentHeader.Session);
                        documentDetail.Item.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Item>(posDocumentDetail.Item).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.Barcode != Guid.Empty)
                    {
                        documentDetail.Barcode = new Retail.Model.Barcode(documentHeader.Session);
                        documentDetail.Barcode.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Master.Barcode>(posDocumentDetail.Barcode).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.MeasurementUnit != Guid.Empty)
                    {
                        documentDetail.MeasurementUnit = new Retail.Model.MeasurementUnit(documentHeader.Session);
                        documentDetail.MeasurementUnit.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.MeasurementUnit>(posDocumentDetail.MeasurementUnit).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.PackingMeasurementUnit != Guid.Empty)
                    {
                        documentDetail.PackingMeasurementUnit = new Retail.Model.MeasurementUnit(documentHeader.Session);
                        documentDetail.PackingMeasurementUnit.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.MeasurementUnit>(posDocumentDetail.PackingMeasurementUnit).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.Reason.HasValue)
                    {
                        documentDetail.Reason = new Retail.Model.Reason(documentHeader.Session);
                        documentDetail.Reason.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.Reason>(posDocumentDetail.Reason).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.CreatedBy != Guid.Empty)
                    {
                        documentDetail.CreatedBy = new Retail.Model.User(documentHeader.Session);
                        documentDetail.CreatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(posDocumentDetail.CreatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentDetail.UpdatedBy != Guid.Empty)
                    {
                        documentDetail.UpdatedBy = new Retail.Model.User(documentHeader.Session);
                        documentDetail.UpdatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(posDocumentDetail.UpdatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }


                }

                foreach (Model.Transactions.DocumentPayment posDocumentPayment in POSDocumentHeader.DocumentPayments)
                {
                    Retail.Model.DocumentPayment documentPayment = documentHeader.DocumentPayments.Where(payment => payment.Oid == posDocumentPayment.Oid).First();

                    if (posDocumentPayment.PaymentMethod != Guid.Empty)
                    {
                        documentPayment.PaymentMethod = new Retail.Model.PaymentMethod(documentHeader.Session);
                        documentPayment.PaymentMethod.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.PaymentMethod>(posDocumentPayment.PaymentMethod).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );

                    }

                    if (posDocumentPayment.TransactionCoupon != null)
                    {
                        documentPayment.TransactionCoupon = new Retail.Model.TransactionCoupon(documentHeader.Session);
                        documentPayment.TransactionCoupon.FromJson(JObject.Parse(posDocumentPayment.TransactionCoupon.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CustomEnumerationValue1 != Guid.Empty)
                    {
                        documentPayment.CustomEnumerationValue1 = new Retail.Model.CustomEnumerationValue(documentHeader.Session);
                        documentPayment.CustomEnumerationValue1.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.CustomEnumerationValue>(posDocumentPayment.CustomEnumerationValue1).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CustomEnumerationValue2 != Guid.Empty)
                    {
                        documentPayment.CustomEnumerationValue2 = new Retail.Model.CustomEnumerationValue(documentHeader.Session);
                        documentPayment.CustomEnumerationValue2.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.CustomEnumerationValue>(posDocumentPayment.CustomEnumerationValue2).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CustomEnumerationValue3 != Guid.Empty)
                    {
                        documentPayment.CustomEnumerationValue3 = new Retail.Model.CustomEnumerationValue(documentHeader.Session);
                        documentPayment.CustomEnumerationValue3.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.CustomEnumerationValue>(posDocumentPayment.CustomEnumerationValue3).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CustomEnumerationValue4 != Guid.Empty)
                    {
                        documentPayment.CustomEnumerationValue4 = new Retail.Model.CustomEnumerationValue(documentHeader.Session);
                        documentPayment.CustomEnumerationValue4.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.CustomEnumerationValue>(posDocumentPayment.CustomEnumerationValue4).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CustomEnumerationValue5 != Guid.Empty)
                    {
                        documentPayment.CustomEnumerationValue5 = new Retail.Model.CustomEnumerationValue(documentHeader.Session);
                        documentPayment.CustomEnumerationValue5.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.CustomEnumerationValue>(posDocumentPayment.CustomEnumerationValue5).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.CreatedBy != Guid.Empty)
                    {
                        documentPayment.CreatedBy = new Retail.Model.User(documentHeader.Session);
                        documentPayment.CreatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(posDocumentPayment.CreatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                    if (posDocumentPayment.UpdatedBy != Guid.Empty)
                    {
                        documentPayment.UpdatedBy = new Retail.Model.User(documentHeader.Session);
                        documentPayment.UpdatedBy.FromJson(JObject.Parse(sessionManager.GetObjectByKey<Model.Settings.User>(posDocumentPayment.UpdatedBy).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS)),
                                                      PlatformConstants.JSON_SERIALIZER_SETTINGS,
                                                      copyOid: true,
                                                      errorOnOidNotFound: false,
                                                      error: out error
                                                      );
                    }

                }
                try
                {
                    if (posCustomer != null && posCustomer.Trader != null)
                    {
                        if (posCustomer.DefaultAddress != null)
                        {
                            if (documentHeader.BillingAddress != null)
                            {
                                ITS.POS.Model.Master.Phone phone = sessionManager.FindObject<Model.Master.Phone>(new BinaryOperator("Address", documentHeader.BillingAddress.Oid));
                                if (phone != null)
                                {
                                    documentHeader.PrintedPhone = phone.Number;
                                }
                            }
                            if (String.IsNullOrEmpty(documentHeader.PrintedPhone))
                            {
                                ITS.POS.Model.Master.Phone printedPhone = sessionManager.FindObject<Model.Master.Phone>(new BinaryOperator("Address", posCustomer.DefaultAddress));
                                if (printedPhone != null)
                                {
                                    documentHeader.PrintedPhone = printedPhone.Number;
                                }
                            }
                        }
                        Model.Master.Trader posTrader = sessionManager.GetObjectByKey<Model.Master.Trader>(posCustomer.Trader);
                        if (posTrader.TaxOfficeLookUp != null)
                        {
                            documentHeader.PrintedTaxOffice = posTrader.TaxOfficeLookUp.Description;
                        }

                    }
                }
                catch (Exception ex) { }
            }
            catch (Exception ex)
            {
                Model.Master.Customer posCustomer = sessionManager.GetObjectByKey<Model.Master.Customer>(POSDocumentHeader.Customer);

                if (POSDocumentHeader.ReferenceCompany == Guid.Empty)
                    errorMessage = ex.Message + " ReferenceCompany object empty."; // + documentHeader.ReferenceCompany.ClassInfo.TableName;
                else if (POSDocumentHeader.Store == Guid.Empty)
                    errorMessage = ex.Message + " Store object empty.";
                else if (POSDocumentHeader.MainCompany == null)
                    errorMessage = ex.Message + " MainCompany object empty.";
                else if (POSDocumentHeader.POS == Guid.Empty)
                    errorMessage = ex.Message + " POS object empty.";
                else if (POSDocumentHeader.DocumentType == Guid.Empty)
                    errorMessage = ex.Message + " DocumentType object empty.";
                else if (POSDocumentHeader.DocumentSeries == Guid.Empty)
                    errorMessage = ex.Message + " DocumentSeries object empty.";
                else if (POSDocumentHeader.Customer == Guid.Empty)
                    errorMessage = ex.Message + " Customer object empty.";
                else if (posCustomer.Trader == Guid.Empty)
                    errorMessage = ex.Message + " Trader object empty.";
                else if (posCustomer.DefaultAddress == Guid.Empty)
                    errorMessage = ex.Message + " DefaultAddress object empty.";
                else if (POSDocumentHeader.Status == Guid.Empty)
                    errorMessage = ex.Message + " Status object empty.";
                else if (POSDocumentHeader.PriceCatalogPolicy == Guid.Empty)
                    errorMessage = ex.Message + " PriceCatalogPolicy object empty.";
                else if (POSDocumentHeader.UserDailyTotals == null)
                    errorMessage = ex.Message + " UserDailyTotals object empty.";
                else if (POSDocumentHeader.CreatedBy == Guid.Empty)
                    errorMessage = ex.Message + " CreatedBy object empty.";
                else
                    errorMessage = ex.Message;
            }
        }


        public ReportPrintResult PrintDocumentHeader(byte[] reportFile, IFormManager formManager, ISessionManager sessionManager, Model.Transactions.DocumentHeader POSDocumentHeader, string printerName, Guid currentOwner, Guid currentUser, IConfigurationManager config)
        {
            ReportPrintResult reportPrintResult = new ReportPrintResult();
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                string jsonDocumentHeader = POSDocumentHeader.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);

                string errorMessage = String.Empty;
                SetUpDatabaseConnection();
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.DocumentHeader documentHeader = new ITS.Retail.Model.DocumentHeader(uow);
                    JObject JSONDocumentHeader = JObject.Parse(jsonDocumentHeader);
                    documentHeader.FromJson(JSONDocumentHeader, PlatformConstants.JSON_SERIALIZER_SETTINGS, copyOid: true, errorOnOidNotFound: false, error: out errorMessage);
                    FillBasicObjData(formManager, sessionManager, POSDocumentHeader, documentHeader, JSONDocumentHeader, out errorMessage);

                    if ((config.DbCommands.Where(x => x.ApplyTime == "BeforeDocumentPrint")).Count() > 0)
                    {
                        foreach (DatabaseCommand command in config.DbCommands.Where(x => x.ApplyTime == "BeforeDocumentPrint"))
                        {
                            try
                            {
                                command.ExecuteCommand(ref documentHeader);
                            }
                            catch (Exception ex)
                            {
                                string errMessage = ex.GetFullMessage();
                                continue;
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(errorMessage))
                    {
                        XtraReportBaseExtension xtraReportBaseExtension = GetReport(reportFile, documentHeader, currentOwner, currentUser);
                        using (ReportPrintTool printTool = new ReportPrintTool(xtraReportBaseExtension))
                        {
                            xtraReportBaseExtension.Duplicates = 2;
                            xtraReportBaseExtension.ShowPrintStatusDialog = false;
                            xtraReportBaseExtension.ShowPrintMarginsWarning = false;
                            xtraReportBaseExtension.PrintingSystem.StartPrint += new DevExpress.XtraPrinting.PrintDocumentEventHandler(PrintingSystem_StartPrint);
                            printTool.Print(printerName);
                        }
                    }

                    reportPrintResult.ErrorMessage = errorMessage;
                    reportPrintResult.PrintResult = String.IsNullOrEmpty(errorMessage)
                                                    ? Retail.Platform.Enumerations.PrintResult.SUCCESS
                                                    : Retail.Platform.Enumerations.PrintResult.FAILURE;
                }
            }
            catch (Exception exception)
            {
                reportPrintResult.PrintResult = Retail.Platform.Enumerations.PrintResult.FAILURE;
                string errorMessage = exception.Message;
                if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message))
                {
                    errorMessage += String.Format("{0}InnerException : {1}", Environment.NewLine, exception.InnerException.Message);
                }
                if (!String.IsNullOrEmpty(exception.StackTrace))
                {
                    errorMessage += String.Format("{0}StackTrace : {1}", Environment.NewLine, exception.StackTrace);
                }
                reportPrintResult.ErrorMessage = errorMessage;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            }
            return reportPrintResult;
        }

        private static void SetUpDatabaseConnection()
        {
            XpoHelper.databasetype = DBType.Memory;
        }


        private void PrintingSystem_StartPrint(object sender, DevExpress.XtraPrinting.PrintDocumentEventArgs e)
        {
            e.PrintDocument.PrinterSettings.Copies = 2;
        }


        public void StartProcess(string path)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = path;
                process.Start();
                process.WaitForInputIdle();
            }
            catch { }
        }

    }
}
